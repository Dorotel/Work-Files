---
description: 'Avalonia UI 11.3.4 patterns for cross-platform XAML development'
applyTo: '**/*.axaml,**/*.axaml.cs'
---

<!-- Based on patterns from: https://github.com/github/awesome-copilot -->

# Avalonia UI 11.3.4 Development Guidelines

## Overview

This file defines Avalonia UI patterns and AXAML standards for the MTM WIP Application. Avalonia 11.3.4 provides cross-platform UI capabilities with XAML-based UI definition.

## Core Principles

### Cross-Platform First
- Design for Windows, macOS, and Linux compatibility
- Test layouts on multiple platforms during development
- Avoid platform-specific hacks unless absolutely necessary
- Use platform detection only when required

### MVVM Pattern Compliance
- Views (.axaml) define UI structure
- ViewModels handle business logic and state
- Data binding connects Views to ViewModels
- Minimize code-behind logic

### Theme V2 System Integration
- Use Theme V2 dynamic resources for all styling
- 17 theme files provide comprehensive theming
- Support light and dark mode switching
- Never hardcode colors or styles

## AXAML Syntax and Patterns

### Layout Containers
- **Grid**: Primary layout container for complex UIs with rows and columns
- **StackPanel**: For simple linear layouts (Vertical or Horizontal orientation)
- **DockPanel**: For docking elements to edges
- **Border**: For visual boundaries, backgrounds, and padding
- **ScrollViewer**: For scrollable content areas

### Grid Layout Best Practices
- Use RowDefinitions and ColumnDefinitions for structure
- Star sizing (*) for flexible/expandable rows/columns
- Auto sizing for content-dependent dimensions
- Fixed pixel sizing for consistent elements (buttons, headers)
- Use Grid.Row and Grid.Column attached properties

### Binding Syntax
- Use `{Binding PropertyName}` for ViewModel property binding
- Use `{Binding PropertyName, Mode=TwoWay}` for editable controls
- Use `{Binding CommandName}` for Button Command bindings
- RelativeSource bindings for ancestor context: `{Binding DataContext.Property, RelativeSource={RelativeSource AncestorType=UserControl}}`

### Data Binding Best Practices
- Prefer XAML binding over code-behind data manipulation
- Use converters for complex value transformations
- OneWay binding for read-only data (default for most controls)
- TwoWay binding for user input controls (TextBox, CheckBox, etc.)

## Control Selection and Usage

### Common Controls
- **TextBox**: User text input with AcceptsReturn, TextWrapping properties
- **Button**: Command execution with Command binding
- **ComboBox**: Selection from dropdown list
- **CheckBox**: Boolean selection
- **DataGrid**: Tabular data display with sorting and filtering
- **ListBox**: Selection from vertical list

### Custom Controls
- Use existing custom controls from `Controls/` directory:
  - **CollapsiblePanel**: Expandable/collapsible sections
  - **CustomDataGrid**: Enhanced DataGrid with column configuration
  - **SessionHistoryPanel**: Manufacturing session history display

### Material Design Icons
- Use Material.Icons.Avalonia for consistent iconography
- Reference icons with `<PathIcon Data="{x:Static icons:MaterialIconData.Icon}"/>` pattern
- Apply theme-aware styling to icons

## Theme V2 Dynamic Resources

### Semantic Token Usage
- **Backgrounds**: `{DynamicResource ThemeV2.Surface.Background}`, `ThemeV2.Card.Background`
- **Foregrounds**: `{DynamicResource ThemeV2.Text.Primary}`, `ThemeV2.Text.Secondary`
- **Borders**: `{DynamicResource ThemeV2.Border.Default}`, `ThemeV2.Border.Accent`
- **Inputs**: `{DynamicResource ThemeV2.Input.Background}`, `ThemeV2.Input.Border`
- **Buttons**: `{DynamicResource ThemeV2.Button.Primary.Background}`, `ThemeV2.Button.Secondary.Background`

### Manufacturing-Specific Styles
- **ManufacturingField**: Base class for form input fields
- **ManufacturingField.Notes**: Expandable text input variant
- **ManufacturingFieldIcon**: Icon sizing and color for field icons
- **ManufacturingButton**: Consistent button styling
- **Card**: Card container with proper elevation and borders

### Style Classes
- Apply styles with `Classes="StyleName"` property
- Multiple classes: `Classes="ManufacturingField Notes"`
- State-based classes for :focus, :pointerover, :disabled states

## Container and Layout Patterns

### Card-Based Layouts
- Wrap content sections in Border with Card styling
- Use proper padding for internal spacing (typically 16-24)
- Apply CornerRadius for rounded corners
- Use BoxShadow for elevation effects

### Form Layouts
- Use Grid with fixed and star row definitions
- Example: `RowDefinitions="80,12,80,12,*"` (fixed heights with spacing rows, expandable bottom)
- Consistent spacing rows (12-16 pixels) between form sections
- Field labels in first column, inputs in second column

### Scrollable Content
- Wrap long content in ScrollViewer
- Set VerticalScrollBarVisibility="Auto" for automatic scrollbar display
- Set HorizontalScrollBarVisibility="Disabled" to prevent horizontal scrolling
- Use ScrollViewer properties on TextBox for integrated scrolling

### Container Boundary Management
- Use ClipToBounds="True" on containers to prevent content overflow
- Use Margin="0" on elements that should extend to container edges
- VerticalAlignment="Stretch" for full-height elements
- HorizontalAlignment="Stretch" for full-width elements

## Custom Control Development

### Control Structure
- Inherit from appropriate base control (UserControl, TemplatedControl)
- Define styled properties with AvaloniaProperty
- Use PseudoClasses for state management
- Implement proper event handling

### Properties and Events
- Use AvaloniaProperty for bindable properties
- Raise PropertyChanged events for observable properties
- Define custom routed events for control interactions
- Document properties with XML comments

## Avalonia Behaviors

### Existing Behaviors
- **AutoCompleteBoxNavigationBehavior**: Keyboard navigation for autocomplete
- **ComboBoxBehavior**: Enhanced ComboBox interactions
- **TextBoxFuzzyValidationBehavior**: Real-time text validation

### Behavior Usage
- Attach behaviors in XAML with `<Interaction.Behaviors>` tag
- Configure behavior properties directly in XAML
- Avoid creating new behaviors when existing controls suffice

## Value Converters

### Existing Converters
- **ColorToBoxShadowConverter**: Color to shadow effect conversion
- **ColorToBrushConverter**: Color to Brush conversion
- **NullToBoolConverter**: Null checking for visibility/enabled state
- **StringEqualsConverter**: String comparison for conditional styling

### Converter Usage
- Register converters as resources in App.axaml
- Reference with `{StaticResource ConverterName}` in bindings
- Pass parameters to converters when needed

## Navigation and Focus Management

### Focus Management
- Use `FocusManagementService` for programmatic focus control
- TabIndex for keyboard navigation order
- IsTabStop property to control tab navigation
- Manage focus state in response to user actions

### View Navigation
- Views are loaded dynamically via ViewModel changes
- Navigation state managed in parent ViewModel
- Child views are UserControls with their own ViewModels

## Performance Optimization

### Virtualization
- Use VirtualizingStackPanel for large lists
- Enable DataGrid virtualization for large datasets
- Lazy-load data when possible

### Binding Performance
- Avoid excessive binding expressions
- Use Mode=OneWay when data doesn't change in UI
- Cache complex converter results when possible

### UI Responsiveness
- Keep UI updates on UI thread
- Use async operations for long-running tasks
- Show loading indicators during data operations

## Cross-Platform Layout Considerations

### Platform-Specific Layout
- Test layouts on all target platforms
- Account for different DPI scaling
- Handle different font rendering across platforms
- Test with different screen sizes and resolutions

### File Path Handling
- Use URI format for resource paths: `avares://AppName/Assets/icon.png`
- Platform-neutral path separators in resource URIs

## Debugging and DevTools

### Avalonia DevTools
- Available in Debug builds only
- Press F12 to open DevTools during runtime
- Inspect visual tree, properties, and styles
- Debug binding errors and layout issues

### Binding Errors
- Check Output window for binding warnings
- AVLN2000 errors indicate binding resolution failures
- Validate DataContext and property paths

## Code-Behind Best Practices

### Minimal Code-Behind
- Code-behind should only handle view-specific logic
- Examples: focus management, visual state changes, direct UI manipulation
- Business logic belongs in ViewModels
- Avoid event handlers when commands can be used

### View Constructor Pattern
```
public MyView()
{
    InitializeComponent();
}
```

### DataContext Setup
- Set DataContext in XAML when using design-time ViewModels
- Or inject ViewModel via constructor when using dependency injection

## Accessibility

### Accessible Controls
- Set AutomationProperties.Name for screen readers
- Use semantic controls (Button, TextBox) over generic containers
- Ensure keyboard navigation works for all interactive elements
- Provide text alternatives for icons and images

## Documentation

### AXAML Comments
- Comment complex layout structures
- Document non-obvious binding paths
- Explain purpose of custom controls

### Design-Time ViewModel
- Use design-time DataContext for XAML previews
- Define sample data for design-time visualization
