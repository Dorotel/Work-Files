using Avalonia.Headless.XUnit;
using Avalonia.Media;
using FluentAssertions;
using MTM_Template_Application.Controls;
using System;
using Xunit;

namespace MTM_Template_Tests.unit.Controls;

[Collection("AvaloniaTests")]
public class ConnectionHealthBadgeTests
{
    [AvaloniaFact]
    public void ConnectionHealthBadge_DefaultValue_ShouldBeOffline()
    {
        // Arrange & Act
        var control = new ConnectionHealthBadge();

        // Assert
        control.Status.Should().Be(ConnectionStatus.Offline);
        control.StatusText.Should().Be("Offline");
        control.LastChecked.Should().BeNull();
        control.LastCheckedText.Should().Be("Never");
    }

    [AvaloniaTheory]
    [InlineData(ConnectionStatus.Healthy, "Healthy")]
    [InlineData(ConnectionStatus.Degraded, "Degraded")]
    [InlineData(ConnectionStatus.Offline, "Offline")]
    public void ConnectionHealthBadge_SetStatus_ShouldUpdateStatusText(ConnectionStatus status, string expectedText)
    {
        // Arrange
        var control = new ConnectionHealthBadge();

        // Act
        control.Status = status;

        // Assert
        control.StatusText.Should().Be(expectedText);
    }

    [AvaloniaFact]
    public void ConnectionHealthBadge_HealthyStatus_ShouldHaveGreenColor()
    {
        // Arrange
        var control = new ConnectionHealthBadge();

        // Act
        control.Status = ConnectionStatus.Healthy;

        // Assert
        control.StatusText.Should().Be("Healthy");
        control.StatusColor.Should().BeOfType<SolidColorBrush>();
        ((SolidColorBrush)control.StatusColor).Color.Should().Be(Color.FromRgb(76, 175, 80));
    }

    [AvaloniaFact]
    public void ConnectionHealthBadge_DegradedStatus_ShouldHaveOrangeColor()
    {
        // Arrange
        var control = new ConnectionHealthBadge();

        // Act
        control.Status = ConnectionStatus.Degraded;

        // Assert
        control.StatusText.Should().Be("Degraded");
        control.StatusColor.Should().BeOfType<SolidColorBrush>();
        ((SolidColorBrush)control.StatusColor).Color.Should().Be(Color.FromRgb(255, 152, 0));
    }

    [AvaloniaFact]
    public void ConnectionHealthBadge_OfflineStatus_ShouldHaveRedColor()
    {
        // Arrange
        var control = new ConnectionHealthBadge();

        // Act
        control.Status = ConnectionStatus.Offline;

        // Assert
        control.StatusText.Should().Be("Offline");
        control.StatusColor.Should().BeOfType<SolidColorBrush>();
        ((SolidColorBrush)control.StatusColor).Color.Should().Be(Color.FromRgb(244, 67, 54));
    }

    [AvaloniaFact]
    public void ConnectionHealthBadge_SetLastChecked_ShouldUpdateLastCheckedText()
    {
        // Arrange
        var control = new ConnectionHealthBadge();
        var lastChecked = DateTime.Now.AddMinutes(-5);

        // Act
        control.LastChecked = lastChecked;

        // Assert
        control.LastCheckedText.Should().Be("5m ago");
    }

    [AvaloniaFact]
    public void ConnectionHealthBadge_JustNow_ShouldShowJustNow()
    {
        // Arrange
        var control = new ConnectionHealthBadge();

        // Act
        control.LastChecked = DateTime.Now.AddSeconds(-30);

        // Assert
        control.LastCheckedText.Should().Be("Just now");
    }

    [AvaloniaFact]
    public void ConnectionHealthBadge_HoursAgo_ShouldShowHours()
    {
        // Arrange
        var control = new ConnectionHealthBadge();

        // Act
        control.LastChecked = DateTime.Now.AddHours(-3);

        // Assert
        control.LastCheckedText.Should().Be("3h ago");
    }

    [AvaloniaFact]
    public void ConnectionHealthBadge_DaysAgo_ShouldShowDays()
    {
        // Arrange
        var control = new ConnectionHealthBadge();

        // Act
        control.LastChecked = DateTime.Now.AddDays(-2);

        // Assert
        control.LastCheckedText.Should().Be("2d ago");
    }
}
