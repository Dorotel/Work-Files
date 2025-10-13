using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace MTM_Template_Application.Controls;

/// <summary>
/// A control that displays a metric with label, formatted value, trend indicator, and color coding.
/// </summary>
public class MetricDisplay : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Label"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<MetricDisplay, string?>(
            nameof(Label),
            defaultValue: string.Empty);

    /// <summary>
    /// Defines the <see cref="Value"/> property.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<MetricDisplay, double>(
            nameof(Value),
            defaultValue: 0.0);

    /// <summary>
    /// Defines the <see cref="Format"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> FormatProperty =
        AvaloniaProperty.Register<MetricDisplay, string?>(
            nameof(Format),
            defaultValue: "N2");

    /// <summary>
    /// Defines the <see cref="Trend"/> property.
    /// </summary>
    public static readonly StyledProperty<TrendDirection> TrendProperty =
        AvaloniaProperty.Register<MetricDisplay, TrendDirection>(
            nameof(Trend),
            defaultValue: TrendDirection.Neutral);

    /// <summary>
    /// Defines the <see cref="Color"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ColorProperty =
        AvaloniaProperty.Register<MetricDisplay, IBrush?>(
            nameof(Color),
            defaultValue: Brushes.Black);

    /// <summary>
    /// Defines the <see cref="FormattedValue"/> property (computed).
    /// </summary>
    public static readonly DirectProperty<MetricDisplay, string> FormattedValueProperty =
        AvaloniaProperty.RegisterDirect<MetricDisplay, string>(
            nameof(FormattedValue),
            o => o.FormattedValue);

    /// <summary>
    /// Defines the <see cref="TrendIndicator"/> property (computed).
    /// </summary>
    public static readonly DirectProperty<MetricDisplay, string> TrendIndicatorProperty =
        AvaloniaProperty.RegisterDirect<MetricDisplay, string>(
            nameof(TrendIndicator),
            o => o.TrendIndicator);

    private string _formattedValue = string.Empty;
    private string _trendIndicator = string.Empty;

    public MetricDisplay()
    {
        UpdateFormattedValue();
        UpdateTrendIndicator();
    }

    static MetricDisplay()
    {
        ValueProperty.Changed.AddClassHandler<MetricDisplay>((x, _) => x.UpdateFormattedValue());
        FormatProperty.Changed.AddClassHandler<MetricDisplay>((x, _) => x.UpdateFormattedValue());
        TrendProperty.Changed.AddClassHandler<MetricDisplay>((x, _) => x.UpdateTrendIndicator());
    }

    /// <summary>
    /// Gets or sets the label displayed above the metric value.
    /// </summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the numeric value to display.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the format string for displaying the value (e.g., "N2", "F1", "P0").
    /// </summary>
    public string? Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    /// <summary>
    /// Gets or sets the trend direction indicator (Up, Down, Neutral).
    /// </summary>
    public TrendDirection Trend
    {
        get => GetValue(TrendProperty);
        set => SetValue(TrendProperty, value);
    }

    /// <summary>
    /// Gets or sets the color for the value and trend indicator.
    /// </summary>
    public IBrush? Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Gets the formatted value string (computed from Value and Format).
    /// </summary>
    public string FormattedValue
    {
        get => _formattedValue;
        private set => SetAndRaise(FormattedValueProperty, ref _formattedValue, value);
    }

    /// <summary>
    /// Gets the trend indicator character (computed from Trend).
    /// </summary>
    public string TrendIndicator
    {
        get => _trendIndicator;
        private set => SetAndRaise(TrendIndicatorProperty, ref _trendIndicator, value);
    }

    private void UpdateFormattedValue()
    {
        try
        {
            var format = Format ?? "N2";
            FormattedValue = Value.ToString(format);
        }
        catch
        {
            // Fallback to N2 format if custom format is invalid
            try
            {
                FormattedValue = Value.ToString("N2");
            }
            catch
            {
                FormattedValue = Value.ToString();
            }
        }
    }

    private void UpdateTrendIndicator()
    {
        TrendIndicator = Trend switch
        {
            TrendDirection.Up => "↑",
            TrendDirection.Down => "↓",
            _ => ""
        };
    }
}

/// <summary>
/// Represents the trend direction for a metric.
/// </summary>
public enum TrendDirection
{
    /// <summary>No trend or flat.</summary>
    Neutral,
    /// <summary>Upward trend.</summary>
    Up,
    /// <summary>Downward trend.</summary>
    Down
}
