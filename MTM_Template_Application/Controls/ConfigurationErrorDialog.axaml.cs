using Avalonia;
using Avalonia.Controls.Primitives;
using System.Collections.Generic;

namespace MTM_Template_Application.Controls;

public class ConfigurationErrorDialog : TemplatedControl
{
    public static readonly StyledProperty<string?> ErrorMessageProperty =
        AvaloniaProperty.Register<ConfigurationErrorDialog, string?>(nameof(ErrorMessage), string.Empty);

    public static readonly StyledProperty<IEnumerable<object>?> RecoveryOptionsProperty =
        AvaloniaProperty.Register<ConfigurationErrorDialog, IEnumerable<object>?>(nameof(RecoveryOptions), null);

    public string? ErrorMessage
    {
        get => GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public IEnumerable<object>? RecoveryOptions
    {
        get => GetValue(RecoveryOptionsProperty);
        set => SetValue(RecoveryOptionsProperty, value);
    }
}
