using Avalonia;
using Avalonia.Controls.Primitives;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MTM_Template_Application.Controls;

public class ActionButtonGroup : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<object>?> ButtonsProperty =
        AvaloniaProperty.Register<ActionButtonGroup, IEnumerable<object>?>(nameof(Buttons), null);

    public IEnumerable<object>? Buttons
    {
        get => GetValue(ButtonsProperty);
        set => SetValue(ButtonsProperty, value);
    }

    public ActionButtonGroup()
    {
        Buttons = new ObservableCollection<object>();
    }
}
