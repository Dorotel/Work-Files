using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace MTM_Template_Application.Controls;

public class SettingRow : ContentControl
{
    public static readonly StyledProperty<string?> SettingKeyProperty =
        AvaloniaProperty.Register<SettingRow, string?>(nameof(SettingKey), string.Empty);

    public static readonly StyledProperty<string?> SettingValueProperty =
        AvaloniaProperty.Register<SettingRow, string?>(nameof(SettingValue), string.Empty);

    public static readonly StyledProperty<bool> IsValidProperty =
        AvaloniaProperty.Register<SettingRow, bool>(nameof(IsValid), true);

    public static readonly StyledProperty<string?> ValidationMessageProperty =
        AvaloniaProperty.Register<SettingRow, string?>(nameof(ValidationMessage), string.Empty);

    public string? SettingKey
    {
        get => GetValue(SettingKeyProperty);
        set => SetValue(SettingKeyProperty, value);
    }

    public string? SettingValue
    {
        get => GetValue(SettingValueProperty);
        set => SetValue(SettingValueProperty, value);
    }

    public bool IsValid
    {
        get => GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }

    public string? ValidationMessage
    {
        get => GetValue(ValidationMessageProperty);
        set => SetValue(ValidationMessageProperty, value);
    }
}
