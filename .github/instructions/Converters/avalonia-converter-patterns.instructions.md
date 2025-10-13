---
description: 'Avalonia IValueConverter patterns with focus on thread safety and primitive return types'
applyTo: '**/Converters/**/*.cs, **/tests/unit/Converters/**/*.cs'
---

# Avalonia Converter Patterns

## Overview

This instruction file defines patterns for implementing `IValueConverter` and `IMultiValueConverter` in Avalonia UI applications, with special focus on thread safety, primitive return types, and testability. Following these patterns prevents common threading errors and ensures converters work correctly in both UI and test contexts.

## Core Principles

### Why Return Primitives, Not UI Objects

1. **Thread Safety**: UI objects (Brush, Pen, Geometry) can only be created on UI thread
2. **Testability**: Unit tests run on test thread, not UI thread
3. **Performance**: XAML binding system creates UI objects on correct thread automatically
4. **Flexibility**: Primitives can be consumed by multiple control types

### Threading Model

```
┌─────────────────────────────────────────────────────────────┐
│                     Avalonia Threading                      │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  Converter.Convert()                                        │
│  ├─ Return Color ✅                                         │
│  │  └─ XAML: Creates SolidColorBrush on UI thread          │
│  │                                                          │
│  └─ Return SolidColorBrush ❌                               │
│     └─ Test: InvalidOperationException                     │
│        "Call from invalid thread"                           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

**Rule**: Converters return primitives (Color, double, bool, string), XAML binding creates UI objects (Brush, Thickness, Visibility).

## Pattern 1: Color Converter (Primitive Return)

### Correct Implementation

```csharp
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace MTM_Template_Application.Converters
{
    /// <summary>
    /// Converts memory usage values to color-coded visual feedback.
    /// Returns Color primitive (not Brush) for thread safety.
    /// </summary>
    public class MemoryUsageToColorConverter : IValueConverter
    {
        // Define colors as static readonly (reusable, thread-safe)
        private static readonly Color GreenColor = Colors.LimeGreen;
        private static readonly Color YellowColor = Colors.Yellow;
        private static readonly Color OrangeColor = Colors.Orange;
        private static readonly Color RedColor = Colors.Red;
        private static readonly Color GrayColor = Colors.Gray;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Validate target type
            if (targetType != typeof(Color) && targetType != typeof(object))
                return GrayColor;

            // Validate input
            if (value is not double memoryMB)
                return GrayColor;

            // Business logic: Memory usage thresholds
            return memoryMB switch
            {
                < 50 => GreenColor,      // Good
                < 70 => YellowColor,     // Warning
                < 90 => OrangeColor,     // High
                _ => RedColor            // Critical
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("MemoryUsageToColorConverter does not support ConvertBack");
        }
    }
}
```

**Key Points**:
- ✅ Returns `Color` primitive (not `SolidColorBrush`)
- ✅ Static readonly color definitions (performance + thread safety)
- ✅ Clear validation of input and target type
- ✅ ConvertBack throws NotSupportedException (one-way converter)
- ✅ XML documentation explains return type choice

### XAML Usage

```xml
<Window xmlns:converters="using:MTM_Template_Application.Converters">
    <Window.Resources>
        <converters:MemoryUsageToColorConverter x:Key="MemoryToColor"/>
    </Window.Resources>
    
    <!-- Binding creates SolidColorBrush automatically on UI thread -->
    <TextBlock Text="{Binding MemoryUsageMB}"
               Foreground="{Binding MemoryUsageMB, Converter={StaticResource MemoryToColor}}"/>
    
    <!-- Works with any property accepting Brush -->
    <Border Background="{Binding MemoryUsageMB, Converter={StaticResource MemoryToColor}}"
            CornerRadius="4"/>
</Window>
```

**How XAML Handles Type Conversion**:
1. Converter returns `Color` value
2. XAML binding system detects target property type is `IBrush`
3. XAML creates `SolidColorBrush` on UI thread automatically
4. No threading error, works in both UI and tests

## Pattern 2: Incorrect Implementation (Threading Error)

### ❌ What NOT to Do

```csharp
// DON'T DO THIS - Creates threading errors in tests
public class MemoryUsageToColorConverter : IValueConverter
{
    private static readonly SolidColorBrush GreenBrush = new SolidColorBrush(Colors.LimeGreen);  // ❌
    private static readonly SolidColorBrush YellowBrush = new SolidColorBrush(Colors.Yellow);    // ❌
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double memoryMB)
            return new SolidColorBrush(Colors.Gray);  // ❌ Creates UI object
        
        return memoryMB switch
        {
            < 50 => GreenBrush,     // ❌ Returns UI object
            < 70 => YellowBrush,    // ❌ Returns UI object
            _ => new SolidColorBrush(Colors.Red)  // ❌ Creates UI object
        };
    }
}
```

**Problems**:
- ❌ Static brushes created before UI thread exists (startup crash risk)
- ❌ Creating `SolidColorBrush` in Convert() requires UI thread
- ❌ Unit tests fail with "InvalidOperationException: Call from invalid thread"
- ❌ Cannot mock or test without UI thread

### Error You'll See in Tests

```
System.InvalidOperationException : Call from invalid thread
   at Avalonia.Threading.Dispatcher.VerifyAccess()
   at Avalonia.AvaloniaObject..ctor()
   at Avalonia.Media.SolidColorBrush..ctor(Color color)
   at MemoryUsageToColorConverter.Convert(...)
```

## Pattern 3: Testing Converters

### Unit Test Pattern (Expects Primitives)

```csharp
using FluentAssertions;
using MTM_Template_Application.Converters;
using System.Globalization;
using Avalonia.Media;
using Xunit;

namespace MTM_Template_Tests.Unit.Converters
{
    public class MemoryUsageToColorConverterTests
    {
        private readonly MemoryUsageToColorConverter _converter = new();
        
        [Theory]
        [InlineData(30.0, nameof(Colors.LimeGreen))]    // Good
        [InlineData(60.0, nameof(Colors.Yellow))]       // Warning
        [InlineData(80.0, nameof(Colors.Orange))]       // High
        [InlineData(95.0, nameof(Colors.Red))]          // Critical
        public void Convert_MemoryValue_ReturnsExpectedColor(double memoryMB, string expectedColorName)
        {
            // Act
            var result = _converter.Convert(memoryMB, typeof(Color), null, CultureInfo.InvariantCulture);
            
            // Assert - Expect Color primitive
            result.Should().BeOfType<Color>();
            
            var color = (Color)result!;
            var expectedColor = (Color)typeof(Colors).GetProperty(expectedColorName)!.GetValue(null)!;
            color.Should().Be(expectedColor);
        }
        
        [Fact]
        public void Convert_InvalidInput_ReturnsGray()
        {
            // Act
            var result = _converter.Convert("invalid", typeof(Color), null, CultureInfo.InvariantCulture);
            
            // Assert
            result.Should().BeOfType<Color>();
            ((Color)result!).Should().Be(Colors.Gray);
        }
        
        [Fact]
        public void Convert_NullInput_ReturnsGray()
        {
            // Act
            var result = _converter.Convert(null, typeof(Color), null, CultureInfo.InvariantCulture);
            
            // Assert
            result.Should().BeOfType<Color>();
            ((Color)result!).Should().Be(Colors.Gray);
        }
        
        [Fact]
        public void ConvertBack_ShouldThrow_NotSupportedException()
        {
            // Arrange
            var color = Colors.Green;
            
            // Act & Assert
            _converter.Invoking(c => c.ConvertBack(color, typeof(double), null, CultureInfo.InvariantCulture))
                .Should().Throw<NotSupportedException>();
        }
    }
}
```

**Key Testing Patterns**:
- ✅ Test expects `Color` return type (`typeof(Color)`)
- ✅ Validates with `.Should().BeOfType<Color>()`
- ✅ Casts result to Color for value comparison
- ✅ Tests null/invalid inputs return fallback color
- ✅ Verifies ConvertBack throws NotSupportedException
- ✅ No UI thread required (runs on test thread)

### ❌ Incorrect Test Pattern

```csharp
// DON'T DO THIS - Expects UI objects in tests
[Fact]
public void Convert_LowMemory_ReturnsGreenBrush()
{
    var result = _converter.Convert(30.0, typeof(SolidColorBrush), null, CultureInfo.InvariantCulture);
    
    // ❌ Expects SolidColorBrush (requires UI thread)
    result.Should().BeOfType<SolidColorBrush>();
    
    // ❌ InvalidOperationException: Call from invalid thread
    var brush = (SolidColorBrush)result!;
}
```

## Pattern 4: Boolean Converter

### Boolean to Visibility Pattern

```csharp
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace MTM_Template_Application.Converters
{
    /// <summary>
    /// Converts boolean values to visibility (bool, not Visibility enum for thread safety).
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return false;  // Hidden by default
            
            // Check for invert parameter
            bool invert = parameter is string param && param.Equals("Invert", StringComparison.OrdinalIgnoreCase);
            
            return invert ? !boolValue : boolValue;
        }
        
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return false;
            
            bool invert = parameter is string param && param.Equals("Invert", StringComparison.OrdinalIgnoreCase);
            
            return invert ? !boolValue : boolValue;
        }
    }
}
```

**XAML Usage**:
```xml
<!-- Normal: true = visible, false = hidden -->
<TextBlock Text="Error Message"
           IsVisible="{Binding HasError, Converter={StaticResource BoolToVisibility}}"/>

<!-- Inverted: true = hidden, false = visible -->
<TextBlock Text="No Errors"
           IsVisible="{Binding HasError, Converter={StaticResource BoolToVisibility}, ConverterParameter=Invert}"/>
```

**Why Not Return Visibility Enum?**:
- Avalonia's `IsVisible` property accepts `bool` directly
- No need for enum conversion
- Simpler, more testable
- Consistent with Avalonia conventions

## Pattern 5: Numeric Converter

### Threshold to Status Indicator

```csharp
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace MTM_Template_Application.Converters
{
    /// <summary>
    /// Converts numeric values to status strings based on thresholds.
    /// </summary>
    public class ThresholdToStatusConverter : IValueConverter
    {
        // Thresholds can be customized via parameters
        private const double DefaultWarningThreshold = 70.0;
        private const double DefaultCriticalThreshold = 90.0;
        
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not double numericValue)
                return "Unknown";
            
            // Parse custom thresholds from parameter (format: "70|90")
            var (warning, critical) = ParseThresholds(parameter as string);
            
            return numericValue switch
            {
                < 0 => "Invalid",
                var v when v < warning => "Good",
                var v when v < critical => "Warning",
                _ => "Critical"
            };
        }
        
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ThresholdToStatusConverter does not support ConvertBack");
        }
        
        private (double Warning, double Critical) ParseThresholds(string? parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return (DefaultWarningThreshold, DefaultCriticalThreshold);
            
            var parts = parameter.Split('|');
            if (parts.Length != 2)
                return (DefaultWarningThreshold, DefaultCriticalThreshold);
            
            double warning = double.TryParse(parts[0], out var w) ? w : DefaultWarningThreshold;
            double critical = double.TryParse(parts[1], out var c) ? c : DefaultCriticalThreshold;
            
            return (warning, critical);
        }
    }
}
```

**XAML Usage with Custom Thresholds**:
```xml
<!-- Default thresholds (70|90) -->
<TextBlock Text="{Binding CpuUsage, Converter={StaticResource ThresholdToStatus}}"/>

<!-- Custom thresholds (50|80) -->
<TextBlock Text="{Binding MemoryUsage, Converter={StaticResource ThresholdToStatus}, ConverterParameter='50|80'}"/>
```

## Pattern 6: Multi-Value Converter

### Combining Multiple Values

```csharp
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MTM_Template_Application.Converters
{
    /// <summary>
    /// Combines multiple boolean values with AND logic.
    /// Returns primitive bool (not Visibility for thread safety).
    /// </summary>
    public class MultiBoolAndConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            // Validate all values are boolean
            if (values == null || values.Count == 0)
                return false;
            
            // All must be true
            return values.All(v => v is true);
        }
    }
}
```

**XAML Usage**:
```xml
<Window.Resources>
    <converters:MultiBoolAndConverter x:Key="BoolAnd"/>
</Window.Resources>

<!-- Button enabled only when both conditions true -->
<Button Content="Save">
    <Button.IsEnabled>
        <MultiBinding Converter="{StaticResource BoolAnd}">
            <Binding Path="IsDataValid"/>
            <Binding Path="IsConnected"/>
        </MultiBinding>
    </Button.IsEnabled>
</Button>
```

## Pattern 7: String Formatting Converter

### Complex String Formatting

```csharp
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace MTM_Template_Application.Converters
{
    /// <summary>
    /// Formats numeric values with units and precision.
    /// Returns string primitive (thread-safe, testable).
    /// </summary>
    public class NumericUnitConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not double numericValue)
                return "N/A";
            
            // Parameter format: "unit|decimals" (e.g., "MB|2" or "GB|1")
            var (unit, decimals) = ParseParameter(parameter as string);
            
            return $"{numericValue.ToString($"F{decimals}", culture)} {unit}";
        }
        
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string stringValue)
                return 0.0;
            
            // Extract numeric part (remove unit)
            var (unit, _) = ParseParameter(parameter as string);
            var numericPart = stringValue.Replace(unit, "").Trim();
            
            return double.TryParse(numericPart, NumberStyles.Float, culture, out var result) 
                ? result 
                : 0.0;
        }
        
        private (string Unit, int Decimals) ParseParameter(string? parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return ("", 2);
            
            var parts = parameter.Split('|');
            string unit = parts.Length > 0 ? parts[0] : "";
            int decimals = parts.Length > 1 && int.TryParse(parts[1], out var d) ? d : 2;
            
            return (unit, decimals);
        }
    }
}
```

**XAML Usage**:
```xml
<!-- Memory: 45.23 MB -->
<TextBlock Text="{Binding MemoryUsage, Converter={StaticResource NumericUnit}, ConverterParameter='MB|2'}"/>

<!-- Storage: 1.5 GB -->
<TextBlock Text="{Binding StorageSize, Converter={StaticResource NumericUnit}, ConverterParameter='GB|1'}"/>
```

## Pattern 8: Converter Parameter Parsing

### Robust Parameter Handling

```csharp
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace MTM_Template_Application.Converters
{
    /// <summary>
    /// Base class for converters with parameter parsing.
    /// </summary>
    public abstract class ParameterizedConverterBase : IValueConverter
    {
        public abstract object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture);
        
        public virtual object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{GetType().Name} does not support ConvertBack");
        }
        
        /// <summary>
        /// Safely parses string parameter to specified type.
        /// </summary>
        protected T? ParseParameter<T>(object? parameter, T? defaultValue = default)
        {
            if (parameter == null)
                return defaultValue;
            
            try
            {
                if (parameter is T typedParam)
                    return typedParam;
                
                if (parameter is string stringParam)
                {
                    var targetType = typeof(T);
                    
                    if (targetType == typeof(double))
                        return double.TryParse(stringParam, out var d) ? (T)(object)d : defaultValue;
                    
                    if (targetType == typeof(int))
                        return int.TryParse(stringParam, out var i) ? (T)(object)i : defaultValue;
                    
                    if (targetType == typeof(bool))
                        return bool.TryParse(stringParam, out var b) ? (T)(object)b : defaultValue;
                    
                    // Fallback: try Convert.ChangeType
                    return (T)System.Convert.ChangeType(stringParam, targetType);
                }
            }
            catch
            {
                // Return default on any parsing error
            }
            
            return defaultValue;
        }
    }
}
```

**Usage in Derived Converter**:
```csharp
public class CustomConverter : ParameterizedConverterBase
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Safely parse parameter with type safety and default
        double threshold = ParseParameter<double>(parameter, defaultValue: 50.0);
        
        if (value is double numericValue)
            return numericValue > threshold;
        
        return false;
    }
}
```

## Common Pitfalls and Solutions

### Pitfall 1: Creating UI Objects in Converter

```csharp
// ❌ WRONG: Creates Brush (UI object)
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
{
    return new SolidColorBrush(Colors.Red);  // Threading error in tests!
}

// ✅ CORRECT: Returns Color (primitive)
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
{
    return Colors.Red;  // XAML creates Brush on UI thread
}
```

---

### Pitfall 2: Not Validating Input Types

```csharp
// ❌ WRONG: Assumes correct type
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
{
    var number = (double)value;  // Crash if value is not double!
    return number > 50 ? Colors.Red : Colors.Green;
}

// ✅ CORRECT: Validates input type
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
{
    if (value is not double number)
        return Colors.Gray;  // Fallback color
    
    return number > 50 ? Colors.Red : Colors.Green;
}
```

---

### Pitfall 3: Mutable Static Fields

```csharp
// ❌ WRONG: Mutable static (not thread-safe)
private static Color CurrentColor = Colors.Red;  // Can be changed!

// ✅ CORRECT: Immutable static readonly
private static readonly Color DefaultColor = Colors.Red;  // Cannot be changed
```

---

### Pitfall 4: Not Handling Null

```csharp
// ❌ WRONG: Doesn't handle null
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
{
    return ((double)value).ToString();  // NullReferenceException!
}

// ✅ CORRECT: Handles null explicitly
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
{
    if (value is not double numericValue)
        return "N/A";  // Clear fallback for null or wrong type
    
    return numericValue.ToString();
}
```

---

### Pitfall 5: Complex Logic in Converter

```csharp
// ❌ WRONG: Business logic in converter
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
{
    // 50 lines of complex calculation and validation
    // Makes converter hard to test and maintain
}

// ✅ CORRECT: Move logic to ViewModel, converter formats only
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
{
    // ViewModel does calculation, returns simple value
    // Converter just formats for display
    return value is string status ? status.ToUpperInvariant() : "UNKNOWN";
}
```

## Testing Checklist

When testing Avalonia converters:

- [ ] Test returns primitive type (Color, double, bool, string)
- [ ] Test with valid input values (boundary values)
- [ ] Test with null input (should return fallback)
- [ ] Test with wrong type input (should return fallback)
- [ ] Test with various parameter values
- [ ] Test ConvertBack throws NotSupportedException (if one-way)
- [ ] Test ConvertBack roundtrip (if two-way)
- [ ] No UI thread required for tests (no [WpfFact] needed)
- [ ] Use FluentAssertions for readable assertions
- [ ] Verify target type validation

## Conversion Table: UI Objects → Primitives

| ❌ UI Object (Avoid)     | ✅ Primitive (Use) | XAML Auto-Converts To |
| ------------------------ | ------------------ | --------------------- |
| `SolidColorBrush`        | `Color`            | `IBrush`              |
| `Thickness`              | `double`           | `Thickness`           |
| `Visibility` enum        | `bool`             | `IsVisible` property  |
| `Geometry`               | `string` (path)    | `Geometry`            |
| `FontWeight`             | `int`              | `FontWeight`          |
| `CornerRadius`           | `double`           | `CornerRadius`        |
| `GridLength`             | `double`           | `GridLength`          |
| `Duration`               | `TimeSpan`         | `Duration`            |

**Rule**: When in doubt, return the simplest primitive type that XAML can convert to the target property type.

## Performance Best Practices

1. **Cache Static Values**: Use `static readonly` for constant colors, thresholds
2. **Avoid Allocations**: Reuse objects when possible
3. **Fast Validation**: Check types with `is` pattern matching
4. **Minimize String Operations**: Cache formatted strings if repeatedly used
5. **Simple Converters**: Keep logic minimal, move complexity to ViewModel

## Debugging Converter Issues

### Enable Converter Tracing

```xml
<!-- Add to XAML for debugging -->
<TextBlock Text="{Binding Value, Converter={StaticResource MyConverter}, Diagnostics=true}"/>
```

### Add Debug Logging

```csharp
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
{
    #if DEBUG
    System.Diagnostics.Debug.WriteLine($"Converter: value={value}, targetType={targetType}, parameter={parameter}");
    #endif
    
    // Converter logic
}
```

### Common Error Messages

| Error Message                              | Cause                               | Solution                         |
| ------------------------------------------ | ----------------------------------- | -------------------------------- |
| "Call from invalid thread"                 | Creating UI object in converter     | Return primitive instead         |
| "Unable to cast object"                    | Wrong return type for target        | Check targetType parameter       |
| "NullReferenceException"                   | Not handling null input             | Add null check before conversion |
| "Object reference not set to an instance"  | Parameter parsing error             | Validate parameter before use    |
| "Converter returned incompatible type"     | Mismatch between return and target  | Return correct primitive type    |

## Real-World Example: Complete Converter

### Boot Stage Color Converter (Production Code)

```csharp
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace MTM_Template_Application.Converters
{
    /// <summary>
    /// Converts boot stage duration to performance-indicating colors.
    /// Green: Meeting target, Red: Exceeding target, Gray: Invalid/missing data.
    /// Returns Color primitive for thread safety in tests and UI.
    /// </summary>
    public class BootStageToColorConverter : IValueConverter
    {
        // Static readonly colors for performance and thread safety
        private static readonly Color GreenColor = Colors.LimeGreen;
        private static readonly Color RedColor = Colors.Red;
        private static readonly Color GrayColor = Colors.Gray;
        
        /// <summary>
        /// Converts boot stage duration to color based on target performance.
        /// </summary>
        /// <param name="value">Tuple of (long durationMs, long targetMs)</param>
        /// <param name="targetType">Expected to be Color or object</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Culture info for formatting</param>
        /// <returns>Color primitive: Green (good), Red (slow), Gray (invalid)</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Validate target type
            if (targetType != typeof(Color) && targetType != typeof(object))
                return GrayColor;
            
            // Handle null or wrong type
            if (value is not (long durationMs, long targetMs))
                return GrayColor;
            
            // Performance comparison
            return durationMs <= targetMs ? GreenColor : RedColor;
        }
        
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BootStageToColorConverter does not support ConvertBack");
        }
    }
}
```

**Complete Test Suite**:

```csharp
using Avalonia.Media;
using FluentAssertions;
using MTM_Template_Application.Converters;
using System.Globalization;
using Xunit;

namespace MTM_Template_Tests.Unit.Converters
{
    public class BootStageToColorConverterTests
    {
        private readonly BootStageToColorConverter _converter = new();
        
        [Fact]
        public void Convert_ValidTargetType_ReturnsColor()
        {
            var result = _converter.Convert((500L, 1000L), typeof(Color), null, CultureInfo.InvariantCulture);
            result.Should().BeOfType<Color>();
        }
        
        [Theory]
        [InlineData(500L, 1000L)]    // Well under target
        [InlineData(1000L, 1000L)]   // Exactly at target
        [InlineData(999L, 1000L)]    // Just under target
        public void Convert_DurationUnderOrEqualTarget_ReturnsGreen(long durationMs, long targetMs)
        {
            var result = _converter.Convert((durationMs, targetMs), typeof(Color), null, CultureInfo.InvariantCulture);
            
            var color = (Color)result!;
            color.Should().Be(Colors.LimeGreen);
        }
        
        [Theory]
        [InlineData(1001L, 1000L)]   // Just over target
        [InlineData(1500L, 1000L)]   // Well over target
        [InlineData(5000L, 1000L)]   // Far over target
        public void Convert_DurationOverTarget_ReturnsRed(long durationMs, long targetMs)
        {
            var result = _converter.Convert((durationMs, targetMs), typeof(Color), null, CultureInfo.InvariantCulture);
            
            var color = (Color)result!;
            color.Should().Be(Colors.Red);
        }
        
        [Fact]
        public void Convert_NullValue_ReturnsGray()
        {
            var result = _converter.Convert(null, typeof(Color), null, CultureInfo.InvariantCulture);
            
            var color = (Color)result!;
            color.Should().Be(Colors.Gray);
        }
        
        [Fact]
        public void Convert_InvalidType_ReturnsGray()
        {
            var result = _converter.Convert("invalid", typeof(Color), null, CultureInfo.InvariantCulture);
            
            var color = (Color)result!;
            color.Should().Be(Colors.Gray);
        }
        
        [Fact]
        public void ConvertBack_ShouldThrow_NotSupportedException()
        {
            var color = Colors.Green;
            
            _converter.Invoking(c => c.ConvertBack(color, typeof((long, long)), null, CultureInfo.InvariantCulture))
                .Should().Throw<NotSupportedException>();
        }
    }
}
```

## Memory File Maintenance

**Last Updated**: October 12, 2025  
**Maintainer**: GitHub Copilot (via user input)

**How to Use This Guide**:
1. When creating new converters, always return primitives (Color, double, bool, string)
2. Use static readonly fields for constant values
3. Validate input types with pattern matching (`is`)
4. Write unit tests expecting primitive return types
5. Document thread safety reasoning in XML comments

**Review Frequency**: When Avalonia version is updated or new converter patterns emerge
