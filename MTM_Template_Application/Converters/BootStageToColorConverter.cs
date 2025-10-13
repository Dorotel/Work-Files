using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MTM_Template_Application.Converters;

/// <summary>
/// Converts boot stage duration (in milliseconds) to a color brush based on performance targets.
/// Green: Meets performance target
/// Red: Exceeds performance target (slow)
/// Gray: No data available
///
/// Performance Targets (from spec):
/// - Stage 0 (Splash): &lt;1000ms
/// - Stage 1 (Services): &lt;3000ms
/// - Stage 2 (Shell): &lt;1000ms
/// </summary>
public class BootStageToColorConverter : IValueConverter
{
    // Define colors as static readonly to avoid UI thread issues during static initialization
    private static readonly Color GreenColor = Color.FromRgb(76, 175, 80);   // #4CAF50 (meets target)
    private static readonly Color RedColor = Color.FromRgb(244, 67, 54);     // #F44336 (exceeds target)
    private static readonly Color GrayColor = Color.FromRgb(158, 158, 158);  // #9E9E9E (no data)

    /// <summary>
    /// Stage 0 performance target (milliseconds)
    /// </summary>
    public const double Stage0TargetMs = 1000.0;

    /// <summary>
    /// Stage 1 performance target (milliseconds)
    /// </summary>
    public const double Stage1TargetMs = 3000.0;

    /// <summary>
    /// Stage 2 performance target (milliseconds)
    /// </summary>
    public const double Stage2TargetMs = 1000.0;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Value: boot stage duration in milliseconds (double)
        // Parameter: stage identifier ("Stage0", "Stage1", "Stage2")

        if (value is not double durationMs)
        {
            return new SolidColorBrush(GrayColor); // No data
        }

        if (parameter is not string stageId)
        {
            return new SolidColorBrush(GrayColor); // Invalid parameter
        }

        var targetMs = stageId switch
        {
            "Stage0" => Stage0TargetMs,
            "Stage1" => Stage1TargetMs,
            "Stage2" => Stage2TargetMs,
            _ => double.MaxValue // Unknown stage, always green
        };

        var color = durationMs <= targetMs ? GreenColor : RedColor;
        return new SolidColorBrush(color);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("BootStageToColorConverter only supports one-way binding.");
    }
}
