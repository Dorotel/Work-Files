using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.ViewModels.Settings;
using NSubstitute;
using Xunit;

namespace MTM_Template_Tests.unit.ViewModels.Settings;

/// <summary>
/// Unit tests for DeveloperSettingsViewModel (Phase 2 - T042)
/// Tests debug mode toggle and trace levels
/// </summary>
[Collection("AvaloniaTests")]
public class DeveloperSettingsViewModelTests
{
    private readonly IConfigurationService _mockConfigService;

    public DeveloperSettingsViewModelTests()
    {
        _mockConfigService = Substitute.For<IConfigurationService>();
        _mockConfigService.GetValue<bool>("Developer:DebugMode", false).Returns(false);
    }

    [AvaloniaFact]
    public void DeveloperSettings_Construction_ShouldLoadDebugMode()
    {
        // Arrange & Act
        var viewModel = new DeveloperSettingsViewModel(_mockConfigService);

        // Assert
        viewModel.DebugMode.Should().BeFalse();
    }

    [AvaloniaFact]
    public void DebugMode_ShouldToggle()
    {
        // Arrange
        var viewModel = new DeveloperSettingsViewModel(_mockConfigService);

        // Act
        viewModel.DebugMode = true;

        // Assert
        viewModel.DebugMode.Should().BeTrue();
    }

    [AvaloniaFact]
    public async Task SaveAsync_ShouldPersistDebugMode()
    {
        // Arrange
        var viewModel = new DeveloperSettingsViewModel(_mockConfigService);
        viewModel.DebugMode = true;

        // Act
        await viewModel.SaveAsync();

        // Assert
        await _mockConfigService.Received(1).SetValue("Developer:DebugMode", true, Arg.Any<CancellationToken>());
    }

    [AvaloniaFact]
    public async Task ReloadAsync_ShouldRefreshDebugMode()
    {
        // Arrange
        var viewModel = new DeveloperSettingsViewModel(_mockConfigService);
        viewModel.DebugMode = true;

        _mockConfigService.GetValue<bool>("Developer:DebugMode", false).Returns(false);

        // Act
        await viewModel.ReloadAsync();

        // Assert
        viewModel.DebugMode.Should().BeFalse();
    }

    [AvaloniaFact]
    public void DebugMode_MultipleToggles_ShouldWork()
    {
        // Arrange
        var viewModel = new DeveloperSettingsViewModel(_mockConfigService);

        // Act & Assert
        viewModel.DebugMode = true;
        viewModel.DebugMode.Should().BeTrue();

        viewModel.DebugMode = false;
        viewModel.DebugMode.Should().BeFalse();

        viewModel.DebugMode = true;
        viewModel.DebugMode.Should().BeTrue();
    }
}
