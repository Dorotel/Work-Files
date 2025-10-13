using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MTM_Template_Application.Controls;

/// <summary>
/// A chart control that visualizes boot timeline stages with duration comparison against targets.
/// </summary>
public class BootTimelineChart : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Stages"/> property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable<BootStageEntry>?> StagesProperty =
        AvaloniaProperty.Register<BootTimelineChart, IEnumerable<BootStageEntry>?>(
            nameof(Stages),
            defaultValue: null);

    /// <summary>
    /// Gets or sets the collection of boot stage entries to display.
    /// </summary>
    public IEnumerable<BootStageEntry>? Stages
    {
        get => GetValue(StagesProperty);
        set => SetValue(StagesProperty, value);
    }

    public BootTimelineChart()
    {
        Stages = new ObservableCollection<BootStageEntry>();
    }
}

/// <summary>
/// Represents a boot stage entry for display in BootTimelineChart.
/// </summary>
public class BootStageEntry
{
    /// <summary>
    /// Gets or sets the stage number (0, 1, 2, etc.).
    /// </summary>
    public int StageNumber { get; set; }

    /// <summary>
    /// Gets or sets the stage name (e.g., "Splash", "Core Services").
    /// </summary>
    public string StageName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the actual duration in milliseconds.
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Gets or sets the target duration in milliseconds.
    /// </summary>
    public long TargetMs { get; set; }

    /// <summary>
    /// Gets whether the stage meets the target duration.
    /// </summary>
    public bool MeetsTarget => DurationMs <= TargetMs;

    /// <summary>
    /// Gets the formatted duration text.
    /// </summary>
    public string DurationText => $"{DurationMs}ms";

    /// <summary>
    /// Gets the formatted target text.
    /// </summary>
    public string TargetText => $"Target: {TargetMs}ms";

    /// <summary>
    /// Gets the performance indicator text.
    /// </summary>
    public string PerformanceIndicator => MeetsTarget ? "✓ On Target" : "⚠ Over Target";

    /// <summary>
    /// Gets the performance indicator color.
    /// </summary>
    public IBrush PerformanceColor => MeetsTarget
        ? new SolidColorBrush(Color.FromRgb(76, 175, 80)) // Green
        : new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange
}
