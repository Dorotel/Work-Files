using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace MTM_Template_Application.Controls;

/// <summary>
/// A card control that displays status information with an icon, title, and status text.
/// </summary>
public class StatusCard : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Title"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<StatusCard, string?>(
            nameof(Title),
            defaultValue: string.Empty);

    /// <summary>
    /// Defines the <see cref="Status"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> StatusProperty =
        AvaloniaProperty.Register<StatusCard, string?>(
            nameof(Status),
            defaultValue: string.Empty);

    /// <summary>
    /// Defines the <see cref="IconSource"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> IconSourceProperty =
        AvaloniaProperty.Register<StatusCard, string?>(
            nameof(IconSource),
            defaultValue: null);

    /// <summary>
    /// Gets or sets the title displayed in the status card.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the status text displayed in the status card.
    /// </summary>
    public string? Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon displayed in the status card (can be text, emoji, or icon font).
    /// </summary>
    public string? IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }
}
