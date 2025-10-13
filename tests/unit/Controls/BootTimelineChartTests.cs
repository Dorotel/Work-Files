using Avalonia.Headless.XUnit;
using Avalonia.Media;
using FluentAssertions;
using MTM_Template_Application.Controls;
using System.Collections.ObjectModel;
using Xunit;

namespace MTM_Template_Tests.unit.Controls;

[Collection("AvaloniaTests")]
public class BootTimelineChartTests
{
    [AvaloniaFact]
    public void BootTimelineChart_DefaultValue_ShouldBeEmptyCollection()
    {
        // Arrange & Act
        var control = new BootTimelineChart();

        // Assert
        control.Stages.Should().NotBeNull();
        control.Stages.Should().BeEmpty();
    }

    [AvaloniaFact]
    public void BootTimelineChart_SetStages_ShouldUpdateProperty()
    {
        // Arrange
        var control = new BootTimelineChart();
        var stages = new ObservableCollection<BootStageEntry>
        {
            new() { StageNumber = 0, StageName = "Splash", DurationMs = 850, TargetMs = 1000 }
        };

        // Act
        control.Stages = stages;

        // Assert
        control.Stages.Should().BeSameAs(stages);
        control.Stages.Should().HaveCount(1);
    }

    [AvaloniaFact]
    public void BootStageEntry_MeetsTarget_WhenDurationLessThanTarget()
    {
        // Arrange & Act
        var entry = new BootStageEntry
        {
            DurationMs = 800,
            TargetMs = 1000
        };

        // Assert
        entry.MeetsTarget.Should().BeTrue();
    }

    [AvaloniaFact]
    public void BootStageEntry_DoesNotMeetTarget_WhenDurationExceedsTarget()
    {
        // Arrange & Act
        var entry = new BootStageEntry
        {
            DurationMs = 1200,
            TargetMs = 1000
        };

        // Assert
        entry.MeetsTarget.Should().BeFalse();
    }

    [AvaloniaFact]
    public void BootStageEntry_DurationText_ShouldFormatCorrectly()
    {
        // Arrange & Act
        var entry = new BootStageEntry { DurationMs = 2450 };

        // Assert
        entry.DurationText.Should().Be("2450ms");
    }

    [AvaloniaFact]
    public void BootStageEntry_TargetText_ShouldFormatCorrectly()
    {
        // Arrange & Act
        var entry = new BootStageEntry { TargetMs = 3000 };

        // Assert
        entry.TargetText.Should().Be("Target: 3000ms");
    }

    [AvaloniaFact]
    public void BootStageEntry_PerformanceIndicator_WhenOnTarget()
    {
        // Arrange & Act
        var entry = new BootStageEntry { DurationMs = 500, TargetMs = 1000 };

        // Assert
        entry.PerformanceIndicator.Should().Be("✓ On Target");
    }

    [AvaloniaFact]
    public void BootStageEntry_PerformanceIndicator_WhenOverTarget()
    {
        // Arrange & Act
        var entry = new BootStageEntry { DurationMs = 1500, TargetMs = 1000 };

        // Assert
        entry.PerformanceIndicator.Should().Be("⚠ Over Target");
    }

    [AvaloniaFact]
    public void BootStageEntry_PerformanceColor_ShouldBeGreen_WhenOnTarget()
    {
        // Arrange & Act
        var entry = new BootStageEntry { DurationMs = 500, TargetMs = 1000 };

        // Assert
        entry.PerformanceColor.Should().BeOfType<SolidColorBrush>();
        ((SolidColorBrush)entry.PerformanceColor).Color.Should().Be(Color.FromRgb(76, 175, 80));
    }

    [AvaloniaFact]
    public void BootStageEntry_PerformanceColor_ShouldBeOrange_WhenOverTarget()
    {
        // Arrange & Act
        var entry = new BootStageEntry { DurationMs = 1500, TargetMs = 1000 };

        // Assert
        entry.PerformanceColor.Should().BeOfType<SolidColorBrush>();
        ((SolidColorBrush)entry.PerformanceColor).Color.Should().Be(Color.FromRgb(255, 152, 0));
    }
}
