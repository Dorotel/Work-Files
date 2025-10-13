using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System.Windows.Input;

namespace MTM_Template_Application.Controls;

public class NavigationMenuItem : TemplatedControl
{
    public static readonly StyledProperty<string?> MenuTextProperty =
        AvaloniaProperty.Register<NavigationMenuItem, string?>(nameof(MenuText), string.Empty);

    public static readonly StyledProperty<string?> IconProperty =
        AvaloniaProperty.Register<NavigationMenuItem, string?>(nameof(Icon), "📄");

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<NavigationMenuItem, bool>(nameof(IsSelected), false);

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<NavigationMenuItem, ICommand?>(nameof(Command), null);

    public static readonly StyledProperty<IBrush?> ItemBackgroundProperty =
        AvaloniaProperty.Register<NavigationMenuItem, IBrush?>(nameof(ItemBackground), Brushes.Transparent);

    public string? MenuText
    {
        get => GetValue(MenuTextProperty);
        set => SetValue(MenuTextProperty, value);
    }

    public string? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public IBrush? ItemBackground
    {
        get => GetValue(ItemBackgroundProperty);
        set => SetValue(ItemBackgroundProperty, value);
    }
}
