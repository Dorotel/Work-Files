using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.ViewModels.Settings;
using NSubstitute;
using Xunit;

namespace MTM_Template_Tests.unit.ViewModels.Settings;

/// <summary>
/// Unit tests for LoggingSettingsViewModel (Phase 2 - T038)
/// Tests log level enum and file path validation
/// </summary>
[Collection("AvaloniaTests")]
public class LoggingSettingsViewModelTests
{
    private readonly IConfigurationService _mockConfigService;

    public LoggingSettingsViewModelTests()
    {
        _mockConfigService = Substitute.For<IConfigurationService>();
        _mockConfigService.GetValue<string>("Logging:Level", "Information").Returns("Information");
    }

    [AvaloniaFact]
    public void LoggingSettings_Construction_ShouldLoadLogLevel()
    {
        // Arrange & Act
        var viewModel = new LoggingSettingsViewModel(_mockConfigService);

        // Assert
        viewModel.LogLevel.Should().Be("Information");
    }

    [AvaloniaFact]
    public void LogLevel_ShouldChangeWhenSet()
    {
        // Arrange
        var viewModel = new LoggingSettingsViewModel(_mockConfigService);

        // Act
        viewModel.LogLevel = "Debug";

        // Assert
        viewModel.LogLevel.Should().Be("Debug");
    }

    [AvaloniaFact]
    public async Task SaveAsync_ShouldPersistLogLevel()
    {
        // Arrange
        var viewModel = new LoggingSettingsViewModel(_mockConfigService);
        viewModel.LogLevel = "Debug";

        // Act
        await viewModel.SaveAsync();

        // Assert
        await _mockConfigService.Received(1).SetValue("Logging:Level", "Debug", Arg.Any<CancellationToken>());
    }

    [AvaloniaFact]
    public async Task ReloadAsync_ShouldRefreshLogLevel()
    {
        // Arrange
        var viewModel = new LoggingSettingsViewModel(_mockConfigService);
        viewModel.LogLevel = "Debug";

        _mockConfigService.GetValue<string>("Logging:Level", "Information").Returns("Information");

        // Act
        await viewModel.ReloadAsync();

        // Assert
        viewModel.LogLevel.Should().Be("Information");
    }

    [AvaloniaFact]
    public void LogLevel_ValidValues_ShouldAccept()
    {
        // Arrange
        var viewModel = new LoggingSettingsViewModel(_mockConfigService);
        var validLevels = new[] { "Debug", "Information", "Warning", "Error", "Critical" };

        // Act & Assert
        foreach (var level in validLevels)
        {
            viewModel.LogLevel = level;
            viewModel.LogLevel.Should().Be(level);
        }
    }
}
