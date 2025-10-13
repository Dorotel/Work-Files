---
description: 'Generate Avalonia custom control with properties, events, and styling'
---

# Setup Custom Control

Generate a complete Avalonia custom control following MTM patterns with styled properties, events, and Theme V2 integration.

## Prerequisites

- Control name must be specified
- Control purpose must be defined
- Base control type identified (UserControl or TemplatedControl)

## User Input

```text
$ARGUMENTS
```

Parse arguments to extract:
- Control name (e.g., `CollapsiblePanel`, `CustomDataGrid`)
- Base type (UserControl or TemplatedControl)
- Properties needed (e.g., IsExpanded, HeaderText)
- Events needed (e.g., ExpandedChanged, ItemSelected)

If arguments are incomplete, prompt for:
1. Control name
2. Control purpose
3. Properties to expose
4. Events to raise
5. Should it be UserControl (composition) or TemplatedControl (templated)?

## Implementation Steps

### Step 1: Choose Control Type

**UserControl** (composition-based):
- Compose existing controls
- Simpler implementation
- Good for complex layouts with fixed structure
- Example: CollapsiblePanel, SessionHistoryPanel

**TemplatedControl** (template-based):
- Define control template
- More flexible styling
- Good for reusable, styleable controls
- Example: Custom button, custom input field

### Step 2: Create Control Directory Structure

For complex controls:
```
Controls/{ControlName}/
├── {ControlName}.axaml          # UserControl definition
├── {ControlName}.axaml.cs       # Code-behind
└── {ControlName}Styles.axaml    # Styles (optional)
```

For simple controls:
```
Controls/{ControlName}.cs         # TemplatedControl
Controls/{ControlName}Styles.axaml  # Control styles
```

### Step 3: Define Styled Properties

Use AvaloniaProperty for bindable properties:

```csharp
public static readonly StyledProperty<bool> IsExpandedProperty =
    AvaloniaProperty.Register<CollapsiblePanel, bool>(
        nameof(IsExpanded),
        defaultValue: true);

public bool IsExpanded
{
    get => GetValue(IsExpandedProperty);
    set => SetValue(IsExpandedProperty, value);
}
```

### Step 4: Define Routed Events

For custom events:

```csharp
public static readonly RoutedEvent<RoutedEventArgs> ExpandedChangedEvent =
    RoutedEvent.Register<CollapsiblePanel, RoutedEventArgs>(
        nameof(ExpandedChanged),
        RoutingStrategies.Bubble);

public event EventHandler<RoutedEventArgs> ExpandedChanged
{
    add => AddHandler(ExpandedChangedEvent, value);
    remove => RemoveHandler(ExpandedChangedEvent, value);
}

// Raise event:
RaiseEvent(new RoutedEventArgs(ExpandedChangedEvent));
```

### Step 5: Implement Property Changed Handlers

```csharp
protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    base.OnPropertyChanged(change);

    if (change.Property == IsExpandedProperty)
    {
        // Update visual state
        PseudoClasses.Set(":expanded", IsExpanded);
        RaiseEvent(new RoutedEventArgs(ExpandedChangedEvent));
    }
}
```

### Step 6: Add PseudoClasses for Styling

```csharp
static CollapsiblePanel()
{
    IsExpandedProperty.Changed.AddClassHandler<CollapsiblePanel>(
        (control, e) => control.UpdatePseudoClasses());
}

private void UpdatePseudoClasses()
{
    PseudoClasses.Set(":expanded", IsExpanded);
    PseudoClasses.Set(":collapsed", !IsExpanded);
}
```

### Step 7: Create Control Styles

Create styles file that integrates with Theme V2:

```xml
<Styles xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:controls="using:MTM_WIP_Application_Avalonia.Controls">
    
    <Style Selector="controls|CollapsiblePanel">
        <Setter Property="Background" Value="{DynamicResource ThemeV2.Surface.Background}"/>
        <Setter Property="BorderBrush" Value="{DynamicResource ThemeV2.Border.Default}"/>
        <Setter Property="BorderThickness" Value="1"/>
        <Setter Property="CornerRadius" Value="4"/>
    </Style>
    
    <Style Selector="controls|CollapsiblePanel:expanded">
        <Setter Property="Background" Value="{DynamicResource ThemeV2.Card.Background}"/>
    </Style>
</Styles>
```

## UserControl Pattern Example

**CollapsiblePanel.axaml**:
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:icons="clr-namespace:Material.Icons.Avalonia;assembly=Material.Icons.Avalonia"
             x:Class="MTM_WIP_Application_Avalonia.Controls.CollapsiblePanel">
    
    <Border BorderBrush="{DynamicResource ThemeV2.Border.Default}"
            BorderThickness="1"
            CornerRadius="4"
            Background="{DynamicResource ThemeV2.Card.Background}">
        <Grid RowDefinitions="Auto,*">
            <!-- Header -->
            <Button Grid.Row="0" 
                    Name="HeaderButton"
                    Classes="CollapsibleHeader"
                    HorizontalAlignment="Stretch"
                    HorizontalContentAlignment="Left">
                <Grid ColumnDefinitions="Auto,*,Auto">
                    <PathIcon Grid.Column="0" 
                              Data="{x:Static icons:MaterialIconData.ChevronDown}"
                              Width="20" Height="20"/>
                    <TextBlock Grid.Column="1" 
                               Text="{Binding $parent[controls:CollapsiblePanel].HeaderText}"
                               Margin="8,0"/>
                </Grid>
            </Button>
            
            <!-- Content -->
            <Border Grid.Row="1" 
                    Name="ContentBorder"
                    Padding="16">
                <ContentPresenter Name="PART_ContentPresenter"
                                 Content="{Binding $parent[controls:CollapsiblePanel].Content}"/>
            </Border>
        </Grid>
    </Border>
</UserControl>
```

**CollapsiblePanel.axaml.cs**:
```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MTM_WIP_Application_Avalonia.Controls;

/// <summary>
/// A collapsible panel control with header and expandable content.
/// </summary>
public partial class CollapsiblePanel : UserControl
{
    public static readonly StyledProperty<string> HeaderTextProperty =
        AvaloniaProperty.Register<CollapsiblePanel, string>(
            nameof(HeaderText),
            defaultValue: "Header");

    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<CollapsiblePanel, bool>(
            nameof(IsExpanded),
            defaultValue: true);

    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<CollapsiblePanel, object?>(nameof(Content));

    public static readonly RoutedEvent<RoutedEventArgs> ExpandedChangedEvent =
        RoutedEvent.Register<CollapsiblePanel, RoutedEventArgs>(
            nameof(ExpandedChanged),
            RoutingStrategies.Bubble);

    public CollapsiblePanel()
    {
        InitializeComponent();
        
        var headerButton = this.FindControl<Button>("HeaderButton");
        if (headerButton != null)
        {
            headerButton.Click += OnHeaderClick;
        }
        
        UpdateVisualState();
    }

    public string HeaderText
    {
        get => GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public event EventHandler<RoutedEventArgs> ExpandedChanged
    {
        add => AddHandler(ExpandedChangedEvent, value);
        remove => RemoveHandler(ExpandedChangedEvent, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsExpandedProperty)
        {
            UpdateVisualState();
            RaiseEvent(new RoutedEventArgs(ExpandedChangedEvent));
        }
    }

    private void OnHeaderClick(object? sender, RoutedEventArgs e)
    {
        IsExpanded = !IsExpanded;
    }

    private void UpdateVisualState()
    {
        var contentBorder = this.FindControl<Border>("ContentBorder");
        if (contentBorder != null)
        {
            contentBorder.IsVisible = IsExpanded;
        }

        PseudoClasses.Set(":expanded", IsExpanded);
        PseudoClasses.Set(":collapsed", !IsExpanded);
    }
}
```

## TemplatedControl Pattern Example

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace MTM_WIP_Application_Avalonia.Controls;

/// <summary>
/// A custom button with icon and text.
/// </summary>
public class IconButton : TemplatedControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<IconButton, string>(nameof(Text));

    public static readonly StyledProperty<string> IconDataProperty =
        AvaloniaProperty.Register<IconButton, string>(nameof(IconData));

    static IconButton()
    {
        // Set default style key
        // Avalonia will look for a style with this key in resources
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string IconData
    {
        get => GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }
}
```

## Validation Checklist

Before completion, verify:

- [ ] Control files created in `Controls/` directory
- [ ] StyledProperty used for all bindable properties
- [ ] RoutedEvent used for custom events
- [ ] Property changed handlers implemented
- [ ] PseudoClasses used for styling states
- [ ] Control integrates with Theme V2 system
- [ ] XML documentation on public members
- [ ] Control can be used in XAML with proper namespace
- [ ] Styles file created (if needed)
- [ ] Material icons integrated (if applicable)

## Anti-Patterns to Avoid

❌ **Do NOT**:
- Use regular properties instead of StyledProperty
- Hardcode colors or styles (use Theme V2)
- Put business logic in custom controls
- Forget to register routed events
- Skip XML documentation
- Ignore property changed notifications

## Success Criteria

✅ **Success** when:
- Control compiles without errors
- Properties are bindable in XAML
- Events fire correctly
- Styling integrates with Theme V2
- Control is reusable across application
- Documentation is complete

## Next Steps

After creating the custom control:
1. Register control styles in App.axaml StyleIncludes
2. Create usage examples in Views
3. Test property bindings and events
4. Validate theme switching works correctly
5. Document control usage patterns
