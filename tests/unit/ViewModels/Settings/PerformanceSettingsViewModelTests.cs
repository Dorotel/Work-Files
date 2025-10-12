using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.ViewModels.Settings;
using NSubstitute;
using Xunit;

namespace MTM_Template_Tests.unit.ViewModels.Settings;

/// <summary>
/// Unit tests for PerformanceSettingsViewModel (Phase 2 - T041)
/// Tests budget threshold validation (positive integers only)
/// </summary>
[Collection("AvaloniaTests")]
public class PerformanceSettingsViewModelTests
{
    private readonly IConfigurationService _mockConfigService;

    public PerformanceSettingsViewModelTests()
    {
        _mockConfigService = Substitute.For<IConfigurationService>();
        _mockConfigService.GetValue<int>("Performance:BootTargetMs", 10000).Returns(10000);
    }

    [AvaloniaFact]
    public void PerformanceSettings_Construction_ShouldLoadBootTarget()
    {
        // Arrange & Act
        var viewModel = new PerformanceSettingsViewModel(_mockConfigService);

        // Assert
        viewModel.BootTargetMs.Should().Be(10000);
    }

    [AvaloniaFact]
    public void BootTargetMs_ShouldChangeWhenSet()
    {
        // Arrange
        var viewModel = new PerformanceSettingsViewModel(_mockConfigService);

        // Act
        viewModel.BootTargetMs = 8000;

        // Assert
        viewModel.BootTargetMs.Should().Be(8000);
    }

    [AvaloniaFact]
    public async Task SaveAsync_ShouldPersistBootTarget()
    {
        // Arrange
        var viewModel = new PerformanceSettingsViewModel(_mockConfigService);
        viewModel.BootTargetMs = 8000;

        // Act
        await viewModel.SaveAsync();

        // Assert
        await _mockConfigService.Received(1).SetValue("Performance:BootTargetMs", 8000, Arg.Any<CancellationToken>());
    }

    [AvaloniaFact]
    public async Task ReloadAsync_ShouldRefreshBootTarget()
    {
        // Arrange
        var viewModel = new PerformanceSettingsViewModel(_mockConfigService);
        viewModel.BootTargetMs = 8000;

        _mockConfigService.GetValue<int>("Performance:BootTargetMs", 10000).Returns(10000);

        // Act
        await viewModel.ReloadAsync();

        // Assert
        viewModel.BootTargetMs.Should().Be(10000);
    }

    [AvaloniaFact]
    public void BootTargetMs_PositiveValues_ShouldAccept()
    {
        // Arrange
        var viewModel = new PerformanceSettingsViewModel(_mockConfigService);

        // Act
        viewModel.BootTargetMs = 1;

        // Assert
        viewModel.BootTargetMs.Should().Be(1);
    }

    [AvaloniaFact]
    public void BootTargetMs_ReasonableValues_ShouldAccept()
    {
        // Arrange
        var viewModel = new PerformanceSettingsViewModel(_mockConfigService);
        var reasonableValues = new[] { 5000, 8000, 10000, 15000, 20000 }; // 5s to 20s

        // Act & Assert
        foreach (var value in reasonableValues)
        {
            viewModel.BootTargetMs = value;
            viewModel.BootTargetMs.Should().Be(value);
        }
    }
}
