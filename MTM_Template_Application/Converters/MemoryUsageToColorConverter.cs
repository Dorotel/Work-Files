using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MTM_Template_Application.Converters;

/// <summary>
/// Converts memory usage (in MB) to a color brush for visual feedback.
/// Green: &lt;70MB (good performance)
/// Yellow: 70-90MB (moderate performance)
/// Red: &gt;90MB (high memory usage)
/// </summary>
public class MemoryUsageToColorConverter : IValueConverter
{
    // Define colors as static readonly to avoid UI thread issues during static initialization
    private static readonly Color GreenColor = Color.FromRgb(76, 175, 80);   // #4CAF50
    private static readonly Color YellowColor = Color.FromRgb(255, 235, 59); // #FFEB3B
    private static readonly Color RedColor = Color.FromRgb(244, 67, 54);     // #F44336
    private static readonly Color GrayColor = Color.FromRgb(158, 158, 158);  // #9E9E9E (fallback)

    /// <summary>
    /// Threshold for yellow warning (MB)
    /// </summary>
    public const double YellowThreshold = 70.0;

    /// <summary>
    /// Threshold for red critical (MB)
    /// </summary>
    public const double RedThreshold = 90.0;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double memoryUsageMB)
        {
            return GrayColor; // Fallback for invalid input
        }

        return memoryUsageMB switch
        {
            < YellowThreshold => GreenColor,           // Good: <70MB
            >= YellowThreshold and < RedThreshold => YellowColor, // Moderate: 70-90MB
            >= RedThreshold => RedColor,               // Critical: >90MB
            _ => GrayColor                             // Fallback
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("MemoryUsageToColorConverter only supports one-way binding.");
    }
}
