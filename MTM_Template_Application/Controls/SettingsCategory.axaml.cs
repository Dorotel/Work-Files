using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MTM_Template_Application.Controls;

public class SettingsCategory : TemplatedControl
{
    public static readonly StyledProperty<string?> CategoryNameProperty =
        AvaloniaProperty.Register<SettingsCategory, string?>(nameof(CategoryName), string.Empty);

    public static readonly StyledProperty<string?> IconProperty =
        AvaloniaProperty.Register<SettingsCategory, string?>(nameof(Icon), "⚙️");

    public static readonly StyledProperty<IEnumerable<object>?> ChildrenProperty =
        AvaloniaProperty.Register<SettingsCategory, IEnumerable<object>?>(nameof(Children), null);

    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<SettingsCategory, bool>(nameof(IsExpanded), true);

    public string? CategoryName
    {
        get => GetValue(CategoryNameProperty);
        set => SetValue(CategoryNameProperty, value);
    }

    public string? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IEnumerable<object>? Children
    {
        get => GetValue(ChildrenProperty);
        set => SetValue(ChildrenProperty, value);
    }

    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public SettingsCategory()
    {
        Children = new ObservableCollection<object>();
    }
}
