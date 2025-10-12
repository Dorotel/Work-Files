using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Models.Theme;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.Services.Theme;
using MTM_Template_Application.ViewModels.Settings;
using NSubstitute;
using Xunit;

namespace MTM_Template_Tests.unit.ViewModels.Settings;

/// <summary>
/// Unit tests for SettingsViewModel (Phase 2)
/// Tests tab navigation, Save/Cancel commands, and validation state management
/// </summary>
[Collection("AvaloniaTests")]
public class SettingsViewModelTests
{
    private readonly IConfigurationService _mockConfigService;
    private readonly IThemeService _mockThemeService;

    public SettingsViewModelTests()
    {
        _mockConfigService = Substitute.For<IConfigurationService>();
        _mockThemeService = Substitute.For<IThemeService>();
        _mockThemeService.GetCurrentTheme().Returns(new ThemeConfiguration { ThemeMode = "Light", IsDarkMode = false });
    }

    [AvaloniaFact]
    public void SettingsViewModel_Construction_ShouldInitializeAllCategories()
    {
        // Arrange & Act
        var viewModel = new SettingsViewModel(_mockConfigService, _mockThemeService);

        // Assert
        viewModel.GeneralSettings.Should().NotBeNull();
        viewModel.DatabaseSettings.Should().NotBeNull();
        viewModel.VisualSettings.Should().NotBeNull();
        viewModel.LoggingSettings.Should().NotBeNull();
        viewModel.UiSettings.Should().NotBeNull();
        viewModel.CacheSettings.Should().NotBeNull();
        viewModel.PerformanceSettings.Should().NotBeNull();
        viewModel.DeveloperSettings.Should().NotBeNull();
    }

    [AvaloniaFact]
    public async Task SaveCommand_ShouldPersistAllChanges()
    {
        // Arrange
        var viewModel = new SettingsViewModel(_mockConfigService, _mockThemeService);
        viewModel.GeneralSettings.Theme = "Dark";

        // Act
        await viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        await _mockConfigService.Received(1).SetValue("UI:Theme", "Dark", Arg.Any<CancellationToken>());
    }

    [AvaloniaFact]
    public async Task CancelCommand_ShouldRevertChanges()
    {
        // Arrange
        var viewModel = new SettingsViewModel(_mockConfigService, _mockThemeService);
        var originalTheme = viewModel.GeneralSettings.Theme;
        viewModel.GeneralSettings.Theme = "Dark";

        // Act
        await viewModel.CancelCommand.ExecuteAsync(null);

        // Assert
        viewModel.GeneralSettings.Theme.Should().Be(originalTheme);
    }

    [AvaloniaFact]
    public void HasUnsavedChanges_ShouldBeFalseInitially()
    {
        // Arrange & Act
        var viewModel = new SettingsViewModel(_mockConfigService, _mockThemeService);

        // Assert
        viewModel.HasUnsavedChanges.Should().BeFalse();
    }

    [AvaloniaFact]
    public void HasUnsavedChanges_ShouldBeTrueAfterModification()
    {
        // Arrange
        var viewModel = new SettingsViewModel(_mockConfigService, _mockThemeService);

        // Act
        viewModel.GeneralSettings.Theme = "Dark";

        // Assert
        viewModel.HasUnsavedChanges.Should().BeTrue();
    }
}
