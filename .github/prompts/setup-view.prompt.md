---
description: 'Generate a new Avalonia AXAML View with Theme V2 integration'
---

# Setup View

Generate a complete Avalonia AXAML View (UserControl) following MTM UI patterns with Theme V2 system integration.

## Prerequisites

- View name must be specified
- Corresponding ViewModel should exist (or will be created)
- UI layout requirements should be identified

## User Input

```text
$ARGUMENTS
```

Parse arguments to extract:
- View name (e.g., `InventorySearchView`)
- Target namespace (e.g., `MTM_WIP_Application_Avalonia.Views`)
- Layout type (Grid, StackPanel, ScrollViewer)
- ViewModel name for binding

If arguments are incomplete, prompt for:
1. View name
2. ViewModel to bind to
3. Layout structure (simple form, data grid, complex layout)
4. Scrollable content? (yes/no)

## Implementation Steps

### Step 1: Create AXAML File

Create file at `Views/{ViewName}.axaml` with:

1. **UserControl declaration**:
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             xmlns:icons="clr-namespace:Material.Icons.Avalonia;assembly=Material.Icons.Avalonia"
             x:Class="MTM_WIP_Application_Avalonia.Views.{ViewName}"
             x:DataType="vm:{ViewModelName}">
```

2. **Design-time DataContext** (optional for IntelliSense):
```xml
<Design.DataContext>
    <vm:{ViewModelName} />
</Design.DataContext>
```

3. **Root layout structure**:
   - Use `ScrollViewer` for scrollable content
   - Use `Grid` for complex layouts with RowDefinitions/ColumnDefinitions
   - Wrap sections in `Border` with `Classes="Card"` for card-based styling

### Step 2: Apply Theme V2 Resources

Use DynamicResource for all styling:
- `{DynamicResource ThemeV2.Surface.Background}` for backgrounds
- `{DynamicResource ThemeV2.Text.Primary}` for text colors
- `{DynamicResource ThemeV2.Border.Default}` for borders
- `{DynamicResource ThemeV2.Input.Background}` for input fields

### Step 3: Add Layout Structure

Example card-based form layout:
```xml
<ScrollViewer>
    <Grid RowDefinitions="*,Auto" Margin="24">
        <!-- Main Content Area -->
        <Border Grid.Row="0" Classes="Card" Padding="24">
            <Grid RowDefinitions="Auto,12,Auto,12,*">
                <!-- Form fields with consistent spacing -->
            </Grid>
        </Border>
        
        <!-- Action Buttons -->
        <StackPanel Grid.Row="1" Orientation="Horizontal" 
                    HorizontalAlignment="Right" Margin="0,12,0,0">
            <Button Content="Save" Command="{Binding SaveCommand}" />
        </StackPanel>
    </Grid>
</ScrollViewer>
```

### Step 4: Add Data Bindings

Bind controls to ViewModel properties:
- TextBox: `Text="{Binding PropertyName, Mode=TwoWay}"`
- Button: `Command="{Binding CommandName}"`
- ComboBox: `ItemsSource="{Binding Items}" SelectedItem="{Binding SelectedItem}"`
- DataGrid: `ItemsSource="{Binding DataItems}"`

### Step 5: Add Material Icons

For buttons and visual indicators:
```xml
<Button Command="{Binding SearchCommand}">
    <StackPanel Orientation="Horizontal" Spacing="8">
        <PathIcon Data="{x:Static icons:MaterialIconData.Magnify}" 
                  Width="20" Height="20"/>
        <TextBlock Text="Search"/>
    </StackPanel>
</Button>
```

### Step 6: Create Code-Behind

Create file at `Views/{ViewName}.axaml.cs`:

```csharp
using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// View for {description}.
/// </summary>
public partial class {ViewName} : UserControl
{
    public {ViewName}()
    {
        InitializeComponent();
    }
}
```

## Common Layout Patterns

### Simple Form Layout
```xml
<Grid RowDefinitions="80,12,80,12,Auto" ColumnDefinitions="200,12,*">
    <!-- Field 1 -->
    <TextBlock Grid.Row="0" Grid.Column="0" Text="Label:" />
    <TextBox Grid.Row="0" Grid.Column="2" Text="{Binding Property1}" />
    
    <!-- Field 2 -->
    <TextBlock Grid.Row="2" Grid.Column="0" Text="Label:" />
    <TextBox Grid.Row="2" Grid.Column="2" Text="{Binding Property2}" />
    
    <!-- Buttons -->
    <StackPanel Grid.Row="4" Grid.Column="2" Orientation="Horizontal">
        <Button Content="Save" Command="{Binding SaveCommand}" />
    </StackPanel>
</Grid>
```

### Data Grid Layout
```xml
<Grid RowDefinitions="Auto,12,*,12,Auto">
    <!-- Search Bar -->
    <TextBox Grid.Row="0" Watermark="Search..." 
             Text="{Binding SearchText, Mode=TwoWay}"/>
    
    <!-- Data Grid -->
    <DataGrid Grid.Row="2" ItemsSource="{Binding Items}"
              SelectedItem="{Binding SelectedItem}"
              IsReadOnly="True">
        <DataGrid.Columns>
            <DataGridTextColumn Header="Name" Binding="{Binding Name}"/>
            <DataGridTextColumn Header="Value" Binding="{Binding Value}"/>
        </DataGrid.Columns>
    </DataGrid>
    
    <!-- Action Buttons -->
    <StackPanel Grid.Row="4" Orientation="Horizontal">
        <Button Content="Add" Command="{Binding AddCommand}"/>
        <Button Content="Edit" Command="{Binding EditCommand}"/>
    </StackPanel>
</Grid>
```

### Card-Based Section Layout
```xml
<ScrollViewer>
    <StackPanel Spacing="16" Margin="24">
        <!-- Section 1 -->
        <Border Classes="Card" Padding="16">
            <StackPanel Spacing="8">
                <TextBlock Text="Section Title" Classes="SectionHeader"/>
                <TextBox Text="{Binding Property1}"/>
            </StackPanel>
        </Border>
        
        <!-- Section 2 -->
        <Border Classes="Card" Padding="16">
            <StackPanel Spacing="8">
                <TextBlock Text="Another Section" Classes="SectionHeader"/>
                <TextBox Text="{Binding Property2}"/>
            </StackPanel>
        </Border>
    </StackPanel>
</ScrollViewer>
```

## Validation Checklist

Before completion, verify:

- [ ] AXAML file created in `Views/` directory
- [ ] Code-behind created with same name
- [ ] UserControl properly declared with namespace imports
- [ ] `x:DataType` set to ViewModel type for compile-time binding validation
- [ ] All backgrounds use Theme V2 DynamicResource
- [ ] Material icons used for visual elements
- [ ] ScrollViewer wraps scrollable content
- [ ] Grid layouts use proper RowDefinitions/ColumnDefinitions
- [ ] Card styling applied to sections
- [ ] Data bindings use correct Mode (OneWay, TwoWay)
- [ ] No hardcoded colors or styles
- [ ] Proper spacing and margins (12, 16, 24 pattern)

## Anti-Patterns to Avoid

❌ **Do NOT**:
- Hardcode colors (use Theme V2 resources)
- Put business logic in code-behind (belongs in ViewModel)
- Use nested ScrollViewers (causes scrolling issues)
- Forget `x:DataType` (causes binding errors)
- Use fixed pixel sizes for content (use Star sizing)
- Forget Material icon namespace import

## Success Criteria

✅ **Success** when:
- View compiles without AVLN2000 binding errors
- Theme switching works correctly (light/dark modes)
- Layout is responsive to window resizing
- All controls properly bound to ViewModel
- Material icons display correctly
- Scrolling works smoothly
- Ready for data binding testing

## Example Output (Search View)

**InventorySearchView.axaml**:
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             xmlns:icons="clr-namespace:Material.Icons.Avalonia;assembly=Material.Icons.Avalonia"
             x:Class="MTM_WIP_Application_Avalonia.Views.InventorySearchView"
             x:DataType="vm:InventorySearchViewModel">
    
    <Design.DataContext>
        <vm:InventorySearchViewModel />
    </Design.DataContext>
    
    <ScrollViewer>
        <Grid RowDefinitions="*,Auto" Margin="24">
            <Border Grid.Row="0" Classes="Card" Padding="24">
                <Grid RowDefinitions="Auto,12,*">
                    <!-- Search Box -->
                    <TextBox Grid.Row="0" 
                             Watermark="Search inventory..."
                             Text="{Binding SearchText, Mode=TwoWay}"
                             Background="{DynamicResource ThemeV2.Input.Background}"/>
                    
                    <!-- Results Grid -->
                    <DataGrid Grid.Row="2" 
                              ItemsSource="{Binding SearchResults}"
                              SelectedItem="{Binding SelectedItem}"
                              IsReadOnly="True">
                        <DataGrid.Columns>
                            <DataGridTextColumn Header="Part Number" Binding="{Binding PartNumber}"/>
                            <DataGridTextColumn Header="Description" Binding="{Binding Description}"/>
                            <DataGridTextColumn Header="Quantity" Binding="{Binding Quantity}"/>
                        </DataGrid.Columns>
                    </DataGrid>
                </Grid>
            </Border>
            
            <StackPanel Grid.Row="1" Orientation="Horizontal" 
                       HorizontalAlignment="Right" Margin="0,12,0,0" Spacing="8">
                <Button Command="{Binding SearchCommand}">
                    <StackPanel Orientation="Horizontal" Spacing="8">
                        <PathIcon Data="{x:Static icons:MaterialIconData.Magnify}" Width="20" Height="20"/>
                        <TextBlock Text="Search"/>
                    </StackPanel>
                </Button>
                <Button Command="{Binding ClearCommand}">
                    <StackPanel Orientation="Horizontal" Spacing="8">
                        <PathIcon Data="{x:Static icons:MaterialIconData.Close}" Width="20" Height="20"/>
                        <TextBlock Text="Clear"/>
                    </StackPanel>
                </Button>
            </StackPanel>
        </Grid>
    </ScrollViewer>
</UserControl>
```

**InventorySearchView.axaml.cs**:
```csharp
using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// View for inventory search functionality.
/// </summary>
public partial class InventorySearchView : UserControl
{
    public InventorySearchView()
    {
        InitializeComponent();
    }
}
```

## Next Steps

After creating the View:
1. Test view in Avalonia Previewer (if available)
2. Connect View to ViewModel via DataContext
3. Test theme switching (light/dark modes)
4. Validate all bindings work correctly
5. Test responsive layout at different window sizes
