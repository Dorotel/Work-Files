using Avalonia.Headless.XUnit;
using Avalonia.Media;
using FluentAssertions;
using MTM_Template_Application.Controls;
using Xunit;

namespace MTM_Template_Tests.unit.Controls;

[Collection("AvaloniaTests")]
public class MetricDisplayTests
{
    [AvaloniaFact]
    public void MetricDisplay_DefaultValues_ShouldBeInitialized()
    {
        // Arrange & Act
        var control = new MetricDisplay();

        // Assert
        control.Label.Should().Be(string.Empty);
        control.Value.Should().Be(0.0);
        control.Format.Should().Be("N2");
        control.Trend.Should().Be(TrendDirection.Neutral);
        control.FormattedValue.Should().Be("0.00");
        control.TrendIndicator.Should().BeEmpty();
    }

    [AvaloniaFact]
    public void MetricDisplay_SetValue_ShouldUpdateFormattedValue()
    {
        // Arrange
        var control = new MetricDisplay { Format = "N2" };

        // Act
        control.Value = 123.456;

        // Assert
        control.Value.Should().Be(123.456);
        control.FormattedValue.Should().Be("123.46");
    }

    [AvaloniaFact]
    public void MetricDisplay_SetFormat_ShouldUpdateFormattedValue()
    {
        // Arrange
        var control = new MetricDisplay { Value = 123.456 };

        // Act
        control.Format = "F1";

        // Assert
        control.FormattedValue.Should().Be("123.5");
    }

    [AvaloniaTheory]
    [InlineData(TrendDirection.Up, "↑")]
    [InlineData(TrendDirection.Down, "↓")]
    [InlineData(TrendDirection.Neutral, "")]
    public void MetricDisplay_SetTrend_ShouldUpdateTrendIndicator(TrendDirection trend, string expectedIndicator)
    {
        // Arrange
        var control = new MetricDisplay();

        // Act
        control.Trend = trend;

        // Assert
        control.TrendIndicator.Should().Be(expectedIndicator);
    }

    [AvaloniaFact]
    public void MetricDisplay_SetColor_ShouldUpdateProperty()
    {
        // Arrange
        var control = new MetricDisplay();
        var expectedColor = Brushes.Red;

        // Act
        control.Color = expectedColor;

        // Assert
        control.Color.Should().Be(expectedColor);
    }

    [AvaloniaFact]
    public void MetricDisplay_SetLabel_ShouldUpdateProperty()
    {
        // Arrange
        var control = new MetricDisplay();
        const string expectedLabel = "Memory Usage";

        // Act
        control.Label = expectedLabel;

        // Assert
        control.Label.Should().Be(expectedLabel);
    }

    [AvaloniaFact]
    public void MetricDisplay_IntegerFormat_ShouldFormatCorrectly()
    {
        // Arrange
        var control = new MetricDisplay { Value = 4520 };

        // Act
        control.Format = "N0";

        // Assert
        control.FormattedValue.Should().Be("4,520");
    }


    [AvaloniaFact]
    public void MetricDisplay_CreateWithProperties_ShouldRenderCorrectly()
    {
        // Arrange & Act
        var control = new MetricDisplay
        {
            Label = "Boot Time",
            Value = 4520,
            Format = "N0",
            Trend = TrendDirection.Down,
            Color = Brushes.Green
        };

        // Assert
        control.Label.Should().Be("Boot Time");
        control.Value.Should().Be(4520);
        control.FormattedValue.Should().Be("4,520");
        control.TrendIndicator.Should().Be("↓");
        control.Color.Should().Be(Brushes.Green);
    }
}
