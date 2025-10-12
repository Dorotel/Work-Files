using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Controls;
using Xunit;

namespace MTM_Template_Tests.unit.Controls;

[Collection("AvaloniaTests")]
public class StatusCardTests
{
    [AvaloniaFact]
    public void StatusCard_DefaultValues_ShouldBeEmpty()
    {
        // Arrange & Act
        var control = new StatusCard();

        // Assert
        control.Title.Should().Be(string.Empty);
        control.Status.Should().Be(string.Empty);
        control.IconSource.Should().BeNull();
    }

    [AvaloniaFact]
    public void StatusCard_SetTitle_ShouldUpdateProperty()
    {
        // Arrange
        var control = new StatusCard();
        const string expectedTitle = "Boot Sequence";

        // Act
        control.Title = expectedTitle;

        // Assert
        control.Title.Should().Be(expectedTitle);
    }

    [AvaloniaFact]
    public void StatusCard_SetStatus_ShouldUpdateProperty()
    {
        // Arrange
        var control = new StatusCard();
        const string expectedStatus = "Complete";

        // Act
        control.Status = expectedStatus;

        // Assert
        control.Status.Should().Be(expectedStatus);
    }

    [AvaloniaFact]
    public void StatusCard_SetIconSource_ShouldUpdateProperty()
    {
        // Arrange
        var control = new StatusCard();
        const string expectedIcon = "CheckCircle";

        // Act
        control.IconSource = expectedIcon;

        // Assert
        control.IconSource.Should().Be(expectedIcon);
    }

    [AvaloniaFact]
    public void StatusCard_SetNullTitle_ShouldAcceptNull()
    {
        // Arrange
        var control = new StatusCard { Title = "Test" };

        // Act
        control.Title = null;

        // Assert
        control.Title.Should().BeNull();
    }

    [AvaloniaFact]
    public void StatusCard_SetNullStatus_ShouldAcceptNull()
    {
        // Arrange
        var control = new StatusCard { Status = "Test" };

        // Act
        control.Status = null;

        // Assert
        control.Status.Should().BeNull();
    }

    [AvaloniaFact]
    public void StatusCard_SetNullIconSource_ShouldAcceptNull()
    {
        // Arrange
        var control = new StatusCard { IconSource = "Test" };

        // Act
        control.IconSource = null;

        // Assert
        control.IconSource.Should().BeNull();
    }

    [AvaloniaFact]
    public void StatusCard_CreateWithProperties_ShouldRenderCorrectly()
    {
        // Arrange & Act
        var control = new StatusCard
        {
            Title = "Configuration",
            Status = "Loaded",
            IconSource = "⚙️"
        };

        // Assert
        control.Title.Should().Be("Configuration");
        control.Status.Should().Be("Loaded");
        control.IconSource.Should().Be("⚙️");
    }
}
