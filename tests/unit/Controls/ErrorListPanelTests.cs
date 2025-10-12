using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Controls;
using System;
using System.Collections.ObjectModel;
using Xunit;

namespace MTM_Template_Tests.unit.Controls;

[Collection("AvaloniaTests")]
public class ErrorListPanelTests
{
    [AvaloniaFact]
    public void ErrorListPanel_DefaultValue_ShouldBeEmptyCollection()
    {
        // Arrange & Act
        var control = new ErrorListPanel();

        // Assert
        control.Errors.Should().NotBeNull();
        control.Errors.Should().BeEmpty();
    }

    [AvaloniaFact]
    public void ErrorListPanel_SetErrors_ShouldUpdateProperty()
    {
        // Arrange
        var control = new ErrorListPanel();
        var errors = new ObservableCollection<ErrorEntry>
        {
            new() { Message = "Test error", Severity = ErrorSeverity.Error }
        };

        // Act
        control.Errors = errors;

        // Assert
        control.Errors.Should().BeSameAs(errors);
        control.Errors.Should().HaveCount(1);
    }

    [AvaloniaFact]
    public void ErrorEntry_DefaultValues_ShouldBeInitialized()
    {
        // Arrange & Act
        var entry = new ErrorEntry();

        // Assert
        entry.Message.Should().Be(string.Empty);
        entry.Severity.Should().Be(ErrorSeverity.Error);
        entry.Timestamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [AvaloniaTheory]
    [InlineData(ErrorSeverity.Critical, "🔴")]
    [InlineData(ErrorSeverity.Error, "⚠️")]
    [InlineData(ErrorSeverity.Warning, "⚡")]
    [InlineData(ErrorSeverity.Info, "ℹ️")]
    public void ErrorEntry_SeverityIcon_ShouldMapCorrectly(ErrorSeverity severity, string expectedIcon)
    {
        // Arrange & Act
        var entry = new ErrorEntry { Severity = severity };

        // Assert
        entry.SeverityIcon.Should().Be(expectedIcon);
    }

    [AvaloniaFact]
    public void ErrorEntry_SetProperties_ShouldUpdateValues()
    {
        // Arrange
        var entry = new ErrorEntry();
        var testTime = DateTime.Now.AddMinutes(-5);

        // Act
        entry.Message = "Critical failure";
        entry.Severity = ErrorSeverity.Critical;
        entry.Timestamp = testTime;

        // Assert
        entry.Message.Should().Be("Critical failure");
        entry.Severity.Should().Be(ErrorSeverity.Critical);
        entry.Timestamp.Should().Be(testTime);
        entry.SeverityIcon.Should().Be("🔴");
    }

    [AvaloniaFact]
    public void ErrorListPanel_MultipleErrors_ShouldDisplayAll()
    {
        // Arrange
        var control = new ErrorListPanel();
        var errors = new ObservableCollection<ErrorEntry>
        {
            new() { Message = "Error 1", Severity = ErrorSeverity.Error },
            new() { Message = "Warning 1", Severity = ErrorSeverity.Warning },
            new() { Message = "Critical 1", Severity = ErrorSeverity.Critical }
        };

        // Act
        control.Errors = errors;

        // Assert
        control.Errors.Should().HaveCount(3);
    }
}
