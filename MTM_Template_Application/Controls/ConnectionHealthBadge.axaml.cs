using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System;

namespace MTM_Template_Application.Controls;

/// <summary>
/// A badge control that displays connection health status with color coding.
/// </summary>
public class ConnectionHealthBadge : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Status"/> property.
    /// </summary>
    public static readonly StyledProperty<ConnectionStatus> StatusProperty =
        AvaloniaProperty.Register<ConnectionHealthBadge, ConnectionStatus>(
            nameof(Status),
            defaultValue: ConnectionStatus.Offline);

    /// <summary>
    /// Defines the <see cref="LastChecked"/> property.
    /// </summary>
    public static readonly StyledProperty<DateTime?> LastCheckedProperty =
        AvaloniaProperty.Register<ConnectionHealthBadge, DateTime?>(
            nameof(LastChecked),
            defaultValue: null);

    /// <summary>
    /// Defines the <see cref="StatusText"/> property (computed).
    /// </summary>
    public static readonly DirectProperty<ConnectionHealthBadge, string> StatusTextProperty =
        AvaloniaProperty.RegisterDirect<ConnectionHealthBadge, string>(
            nameof(StatusText),
            o => o.StatusText);

    /// <summary>
    /// Defines the <see cref="StatusColor"/> property (computed).
    /// </summary>
    public static readonly DirectProperty<ConnectionHealthBadge, IBrush> StatusColorProperty =
        AvaloniaProperty.RegisterDirect<ConnectionHealthBadge, IBrush>(
            nameof(StatusColor),
            o => o.StatusColor);

    /// <summary>
    /// Defines the <see cref="StatusBackground"/> property (computed).
    /// </summary>
    public static readonly DirectProperty<ConnectionHealthBadge, IBrush> StatusBackgroundProperty =
        AvaloniaProperty.RegisterDirect<ConnectionHealthBadge, IBrush>(
            nameof(StatusBackground),
            o => o.StatusBackground);

    /// <summary>
    /// Defines the <see cref="StatusBorderBrush"/> property (computed).
    /// </summary>
    public static readonly DirectProperty<ConnectionHealthBadge, IBrush> StatusBorderBrushProperty =
        AvaloniaProperty.RegisterDirect<ConnectionHealthBadge, IBrush>(
            nameof(StatusBorderBrush),
            o => o.StatusBorderBrush);

    /// <summary>
    /// Defines the <see cref="LastCheckedText"/> property (computed).
    /// </summary>
    public static readonly DirectProperty<ConnectionHealthBadge, string> LastCheckedTextProperty =
        AvaloniaProperty.RegisterDirect<ConnectionHealthBadge, string>(
            nameof(LastCheckedText),
            o => o.LastCheckedText);

    private string _statusText = "Offline";
    private IBrush _statusColor = Brushes.Gray;
    private IBrush _statusBackground = Brushes.LightGray;
    private IBrush _statusBorderBrush = Brushes.Gray;
    private string _lastCheckedText = "Never";

    public ConnectionHealthBadge()
    {
        UpdateStatus();
        UpdateLastCheckedText();
    }

    static ConnectionHealthBadge()
    {
        StatusProperty.Changed.AddClassHandler<ConnectionHealthBadge>((x, _) => x.UpdateStatus());
        LastCheckedProperty.Changed.AddClassHandler<ConnectionHealthBadge>((x, _) => x.UpdateLastCheckedText());
    }

    /// <summary>
    /// Gets or sets the connection health status.
    /// </summary>
    public ConnectionStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>
    /// Gets or sets the last time the connection was checked.
    /// </summary>
    public DateTime? LastChecked
    {
        get => GetValue(LastCheckedProperty);
        set => SetValue(LastCheckedProperty, value);
    }

    /// <summary>
    /// Gets the status text (computed from Status).
    /// </summary>
    public string StatusText
    {
        get => _statusText;
        private set => SetAndRaise(StatusTextProperty, ref _statusText, value);
    }

    /// <summary>
    /// Gets the status indicator color (computed from Status).
    /// </summary>
    public IBrush StatusColor
    {
        get => _statusColor;
        private set => SetAndRaise(StatusColorProperty, ref _statusColor, value);
    }

    /// <summary>
    /// Gets the status background color (computed from Status).
    /// </summary>
    public IBrush StatusBackground
    {
        get => _statusBackground;
        private set => SetAndRaise(StatusBackgroundProperty, ref _statusBackground, value);
    }

    /// <summary>
    /// Gets the status border brush (computed from Status).
    /// </summary>
    public IBrush StatusBorderBrush
    {
        get => _statusBorderBrush;
        private set => SetAndRaise(StatusBorderBrushProperty, ref _statusBorderBrush, value);
    }

    /// <summary>
    /// Gets the last checked text (computed from LastChecked).
    /// </summary>
    public string LastCheckedText
    {
        get => _lastCheckedText;
        private set => SetAndRaise(LastCheckedTextProperty, ref _lastCheckedText, value);
    }

    private void UpdateStatus()
    {
        switch (Status)
        {
            case ConnectionStatus.Healthy:
                StatusText = "Healthy";
                StatusColor = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green
                StatusBackground = new SolidColorBrush(Color.FromRgb(232, 245, 233)); // Light green
                StatusBorderBrush = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                break;
            case ConnectionStatus.Degraded:
                StatusText = "Degraded";
                StatusColor = new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange
                StatusBackground = new SolidColorBrush(Color.FromRgb(255, 243, 224)); // Light orange
                StatusBorderBrush = new SolidColorBrush(Color.FromRgb(255, 152, 0));
                break;
            case ConnectionStatus.Offline:
                StatusText = "Offline";
                StatusColor = new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red
                StatusBackground = new SolidColorBrush(Color.FromRgb(255, 235, 238)); // Light red
                StatusBorderBrush = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                break;
        }
    }

    private void UpdateLastCheckedText()
    {
        if (LastChecked.HasValue)
        {
            var timeAgo = DateTime.Now - LastChecked.Value;
            LastCheckedText = timeAgo.TotalMinutes < 1
                ? "Just now"
                : timeAgo.TotalMinutes < 60
                    ? $"{(int)timeAgo.TotalMinutes}m ago"
                    : timeAgo.TotalHours < 24
                        ? $"{(int)timeAgo.TotalHours}h ago"
                        : $"{(int)timeAgo.TotalDays}d ago";
        }
        else
        {
            LastCheckedText = "Never";
        }
    }
}

/// <summary>
/// Represents the health status of a connection.
/// </summary>
public enum ConnectionStatus
{
    /// <summary>Connection is offline or unavailable.</summary>
    Offline,
    /// <summary>Connection is experiencing issues but functional.</summary>
    Degraded,
    /// <summary>Connection is healthy and performing well.</summary>
    Healthy
}
