---
description: 'Lessons learned and persistent knowledge for Avalonia UI 11.3.4 development patterns'
---

# Avalonia UI Patterns Memory

**Purpose**: Capture lessons learned, debugging patterns, and discoveries related to Avalonia UI 11.3.4 development in the MTM application.

**Usage**: This file is automatically referenced by GitHub Copilot to provide context-aware assistance for Avalonia UI development. Add lessons as they are discovered during development.

---

## AXAML Syntax Lessons

### DataType Declarations for Binding Resolution

**Lesson**: Always declare `x:DataType` on UserControl root element to enable IntelliSense and prevent AVLN2000 binding errors.

**Pattern**:
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             x:Class="MTM_WIP_Application_Avalonia.Views.SomeView"
             x:DataType="vm:SomeViewModel">
```

**Why**: Without x:DataType, Avalonia's AXAML compiler cannot validate bindings at design time, leading to runtime errors that are difficult to debug.

**Discovered**: 2025-10-10 - Pattern established during comprehensive GitHub Copilot configuration implementation.

---

### Grid RowDefinitions for Responsive Forms

**Lesson**: Use fixed/dynamic row pattern for form layouts: `RowDefinitions="80,12,80,12,*"`

**Pattern**:
- Fixed height rows (80px) for standard input fields
- Small spacing rows (12px) between fields for consistent vertical rhythm
- Star row (*) for expandable content areas (like Notes fields)

**Why**: This pattern provides consistent field heights, proper spacing, and allows the last field to expand to fill available space without complex calculation.

**Example**:
```xml
<Grid RowDefinitions="80,12,80,12,80,12,*">
    <Border Grid.Row="0" Classes="ManufacturingField"><!-- Part Number --></Border>
    <!-- Grid.Row="1" is spacer -->
    <Border Grid.Row="2" Classes="ManufacturingField"><!-- Location --></Border>
    <!-- Grid.Row="3" is spacer -->
    <Border Grid.Row="4" Classes="ManufacturingField"><!-- Quantity --></Border>
    <!-- Grid.Row="5" is spacer -->
    <Border Grid.Row="6" Classes="ManufacturingField Notes"><!-- Notes - expandable --></Border>
</Grid>
```

**Discovered**: During ManufacturingField control implementation and layout standardization.

---

## Layout Debugging Patterns

### Container Boundary Overflow Issues

**Problem**: Elements extending beyond their intended container boundaries.

**Debugging Steps**:
1. Check for `ClipToBounds="True"` on parent container
2. Verify `Margin="0"` on child elements that should extend to edges
3. Confirm `HorizontalAlignment="Stretch"` and `VerticalAlignment="Stretch"` for full-size children
4. Look for competing `MaxHeight`/`MaxWidth` constraints

**Solution Pattern**:
```xml
<Border Classes="ManufacturingField Notes" 
        VerticalAlignment="Stretch" 
        Margin="0" 
        ClipToBounds="True">
    <!-- Content will be clipped if it overflows -->
</Border>
```

**Why**: Avalonia's layout system allows content to overflow by default. Explicit boundary management prevents visual artifacts and ensures consistent layouts.

**Discovered**: During Notes field expandable implementation - elements were extending beyond card boundaries.

---

### AVLN2000 Binding Errors Resolution

**Problem**: Binding errors in Output window: "Could not resolve DataContext binding for property..."

**Common Causes**:
1. Missing `x:DataType` attribute on UserControl
2. Property doesn't exist in ViewModel
3. RelativeSource binding path incorrect
4. DataContext switches in DataGrid columns (row item context vs ViewModel context)

**Resolution Strategy**:
1. **Add x:DataType**: Enables compile-time binding validation
2. **Verify property exists**: Check ViewModel has the bound property (use F12 Go To Definition)
3. **Simplify binding**: Remove complex RelativeSource patterns when possible
4. **Use backing properties**: Replace computed properties with `[ObservableProperty]` fields for AXAML compatibility

**Example Fix**:
```xml
<!-- Before: AVLN2000 error -->
<UserControl>
    <TextBox Text="{Binding UserName}"/>
</UserControl>

<!-- After: No errors -->
<UserControl x:DataType="vm:MyViewModel">
    <TextBox Text="{Binding UserName}"/>
</UserControl>
```

**Discovered**: Throughout development - AVLN2000 is the most common Avalonia binding error.

---

## Theme V2 System Discoveries

### Dynamic Resource Usage for Adaptive Theming

**Lesson**: Always use `{DynamicResource}` for colors, never hardcode values like `Background="#FFFFFF"`.

**Pattern**:
```xml
<!-- ✅ Correct: Theme-aware -->
<Border Background="{DynamicResource ThemeV2.Surface.Background}"
        BorderBrush="{DynamicResource ThemeV2.Border.Default}">

<!-- ❌ Incorrect: Hardcoded -->
<Border Background="#FFFFFF" BorderBrush="#CCCCCC">
```

**Available Theme V2 Tokens**:
- **Backgrounds**: `ThemeV2.Surface.Background`, `ThemeV2.Card.Background`, `ThemeV2.Surface.Secondary`
- **Foregrounds**: `ThemeV2.Text.Primary`, `ThemeV2.Text.Secondary`, `ThemeV2.Text.Disabled`
- **Borders**: `ThemeV2.Border.Default`, `ThemeV2.Border.Accent`, `ThemeV2.Border.Focus`
- **Inputs**: `ThemeV2.Input.Background`, `ThemeV2.Input.Border`, `ThemeV2.Input.Focus`
- **Buttons**: `ThemeV2.Button.Primary.Background`, `ThemeV2.Button.Secondary.Background`

**Why**: Dynamic resources enable theme switching (light/dark modes) without code changes. Hardcoded colors break theme system and create inconsistent UI.

**Discovered**: Theme V2 system standardization during comprehensive UI audit.

---

### ManufacturingField Style Class System

**Lesson**: Use `Classes="ManufacturingField"` for consistent form field styling across the application.

**Pattern**:
```xml
<Border Classes="ManufacturingField">
    <Grid ColumnDefinitions="40,*" RowDefinitions="Auto,*">
        <!-- Icon (40px width) -->
        <PathIcon Grid.Column="0" Grid.RowSpan="2"
                  Data="{x:Static icons:MaterialIconDataProvider.Inventory}"
                  Classes="ManufacturingFieldIcon"/>
        
        <!-- Label (Auto height) -->
        <TextBlock Grid.Column="1" Grid.Row="0" 
                   Text="Part Number"
                   Classes="ManufacturingFieldLabel"/>
        
        <!-- Input (Star height) -->
        <TextBox Grid.Column="1" Grid.Row="1"
                 Classes="ManufacturingInput"
                 Text="{Binding PartNumber, Mode=TwoWay}"/>
    </Grid>
</Border>
```

**Variants**:
- `Classes="ManufacturingField"` - Standard 80px height field
- `Classes="ManufacturingField Notes"` - Expandable field with Height="NaN", fills available space

**Why**: Consistent visual language across all forms, reduces styling duplication, enforces icon/label/input structure.

**Discovered**: During form standardization initiative to create consistent manufacturing UX.

---

## Custom Control Patterns

### UserControl vs TemplatedControl Decision

**Lesson**: Use `UserControl` for composition-based controls, `TemplatedControl` for style-based controls.

**UserControl Pattern** (composition):
```csharp
public partial class CustomDataGrid : UserControl
{
    public CustomDataGrid()
    {
        InitializeComponent(); // AXAML file defines structure
    }
}
```

**TemplatedControl Pattern** (styling):
```csharp
public class StyledButton : TemplatedControl
{
    static StyledButton()
    {
        // Style defined in Themes, no AXAML file
    }
}
```

**Decision Criteria**:
- **UserControl**: When control is specific composition of existing controls (CustomDataGrid, SessionHistoryPanel, CollapsiblePanel)
- **TemplatedControl**: When control needs multiple style variations via themes (theme buttons, custom inputs)

**Why**: UserControls are easier to develop (XAML designer support) but less flexible for theming. TemplatedControls require more setup but support complete style customization.

**Discovered**: During custom control architecture review.

---

## ScrollViewer Integration

### TextBox with Integrated Scrolling

**Lesson**: Use `ScrollViewer` attached properties on `TextBox` directly, don't wrap in `ScrollViewer` container.

**Pattern**:
```xml
<!-- ✅ Correct: Integrated scrolling -->
<TextBox Classes="ManufacturingInput Notes"
         Text="{Binding Notes, Mode=TwoWay}"
         AcceptsReturn="True"
         TextWrapping="Wrap"
         VerticalContentAlignment="Top"
         ScrollViewer.VerticalScrollBarVisibility="Auto"
         ScrollViewer.HorizontalScrollBarVisibility="Disabled"/>

<!-- ❌ Incorrect: Wrapped ScrollViewer (unnecessary nesting) -->
<ScrollViewer VerticalScrollBarVisibility="Auto">
    <TextBox Text="{Binding Notes}" TextWrapping="Wrap"/>
</ScrollViewer>
```

**Why**: TextBox has built-in scroll support. Wrapping in ScrollViewer creates nested scroll containers, leading to confusing scroll behavior.

**Discovered**: During Notes field implementation - double-scrolling behavior identified and resolved.

---

## Performance Patterns

### Virtualization for Large ItemsCollections

**Lesson**: Use `VirtualizingStackPanel` for `ItemsControl` with large datasets (> 100 items).

**Pattern**:
```xml
<ListBox ItemsSource="{Binding LargeCollection}">
    <ListBox.ItemsPanel>
        <ItemsPanelTemplate>
            <VirtualizingStackPanel/>
        </ItemsPanelTemplate>
    </ListBox.ItemsPanel>
</ListBox>
```

**Why**: Without virtualization, Avalonia creates visual elements for ALL items, causing performance degradation with large collections. Virtualization only creates visuals for visible items.

**When to Use**: 
- Collections with > 100 items
- DataGrid with many rows
- ListBox with complex item templates

**Discovered**: Performance profiling during large inventory list implementation.

---

## Cross-Platform Layout Considerations

### DPI Scaling Differences

**Lesson**: Avoid pixel-perfect layouts; use relative sizing and proper margins.

**Pattern**:
```xml
<!-- ✅ Adaptive: Scales across DPI settings -->
<Grid ColumnDefinitions="*,2*,*">
    <TextBlock Grid.Column="0" Margin="8"/>
    <TextBlock Grid.Column="1" Margin="8"/>
    <TextBlock Grid.Column="2" Margin="8"/>
</Grid>

<!-- ❌ Brittle: Fixed pixels break on high-DPI displays -->
<Grid ColumnDefinitions="200,400,200">
    <TextBlock Grid.Column="0" Margin="8"/>
    <TextBlock Grid.Column="1" Margin="8"/>
    <TextBlock Grid.Column="2" Margin="8"/>
</Grid>
```

**Why**: Different platforms (Windows, macOS, Linux) have different default DPI settings. Relative sizing (star columns, percentage widths) adapts automatically.

**Discovered**: Cross-platform testing on high-DPI displays.

---

## Material Icons Integration

### MaterialIconDataProvider Pattern

**Lesson**: Use Material.Icons.Avalonia package with proper icon reference pattern.

**Pattern**:
```xml
xmlns:icons="using:Material.Icons.Avalonia"

<PathIcon Data="{x:Static icons:MaterialIconDataProvider.Inventory}"
          Classes="ManufacturingFieldIcon"/>
```

**Common Icons for Manufacturing**:
- `Inventory` - Parts and inventory
- `LocationOn` - Location codes
- `Person` - User/operator
- `CalendarToday` - Dates
- `Notes` - Notes and comments
- `Build` - Manufacturing operations
- `LocalShipping` - Shipping/receiving

**Why**: Material Design provides consistent, recognizable iconography. Using PathIcon allows Theme V2 styling (color, size) to be applied consistently.

**Discovered**: During icon standardization across application.

---

## Focus Management

### FocusManagementService Usage

**Lesson**: Use centralized `FocusManagementService` for programmatic focus control, not direct focus manipulation in Views.

**Pattern**:
```csharp
// ViewModel
private readonly IFocusManagementService _focusService;

[RelayCommand]
private async Task SaveAsync()
{
    var result = await _service.SaveAsync();
    if (!result.IsSuccess)
    {
        // Request focus on error field
        _focusService.RequestFocus("PartNumberTextBox");
    }
}
```

**Why**: Centralizing focus management allows for consistent behavior, testability, and separation of concerns. Views remain declarative.

**Discovered**: During error handling improvement initiative.

---

## Common Pitfalls

### Pitfall 1: Forgetting x:DataType

**Issue**: Missing `x:DataType` leads to no IntelliSense, no compile-time validation, and AVLN2000 errors.

**Fix**: Always add x:DataType to root UserControl element.

---

### Pitfall 2: Nested StackPanels for Complex Layouts

**Issue**: Using nested StackPanels instead of Grid for complex forms leads to inflexible layouts and difficult maintenance.

**Fix**: Use Grid with explicit RowDefinitions and ColumnDefinitions for forms.

---

### Pitfall 3: Hardcoded Colors Breaking Theme System

**Issue**: Hardcoding colors (`Background="#FFFFFF"`) breaks theme switching and creates inconsistent UI.

**Fix**: Use Theme V2 DynamicResource tokens for all colors.

---

### Pitfall 4: Not Using ClipToBounds on Expandable Containers

**Issue**: Expandable content (like Notes fields) extends beyond parent boundaries.

**Fix**: Add `ClipToBounds="True"` to parent Border and `Margin="0"` to expandable child.

---

## Memory File Maintenance

**Last Updated**: 2025-10-10  
**Maintainer**: GitHub Copilot (via user input)

**How to Add Lessons**:
1. Identify a recurring pattern or solved problem
2. Document the lesson with Pattern/Why/Discovered sections
3. Include code examples for clarity
4. Commit changes to memory file

**Review Frequency**: After major features or when patterns change
