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
/// Unit tests for GeneralSettingsViewModel (Phase 2 - T035)
/// Tests theme/language/startup settings and validation
/// </summary>
[Collection("AvaloniaTests")]
public class GeneralSettingsViewModelTests
{
    private readonly IConfigurationService _mockConfigService;
    private readonly IThemeService _mockThemeService;

    public GeneralSettingsViewModelTests()
    {
        _mockConfigService = Substitute.For<IConfigurationService>();
        _mockConfigService.GetValue<string>("UI:Theme", "Light").Returns("Light");
        _mockConfigService.GetValue<string>("UI:Language", "en-US").Returns("en-US");
        _mockConfigService.GetValue<bool>("App:StartWithWindows", false).Returns(false);

        _mockThemeService = Substitute.For<IThemeService>();
        _mockThemeService.GetCurrentTheme().Returns(new ThemeConfiguration { ThemeMode = "Light", IsDarkMode = false });
    }

    [AvaloniaFact]
    public void GeneralSettings_Construction_ShouldLoadDefaultValues()
    {
        // Arrange & Act
        var viewModel = new GeneralSettingsViewModel(_mockConfigService, _mockThemeService);

        // Assert
        viewModel.Theme.Should().Be("Light");
        viewModel.Language.Should().Be("en-US");
        viewModel.StartWithWindows.Should().BeFalse();
    }

    [AvaloniaFact]
    public void Theme_ShouldChangeWhenSet()
    {
        // Arrange
        var viewModel = new GeneralSettingsViewModel(_mockConfigService, _mockThemeService);

        // Act
        viewModel.Theme = "Dark";

        // Assert
        viewModel.Theme.Should().Be("Dark");
    }

    [AvaloniaFact]
    public void Language_ShouldChangeWhenSet()
    {
        // Arrange
        var viewModel = new GeneralSettingsViewModel(_mockConfigService, _mockThemeService);

        // Act
        viewModel.Language = "fr-FR";

        // Assert
        viewModel.Language.Should().Be("fr-FR");
    }

    [AvaloniaFact]
    public void StartWithWindows_ShouldToggle()
    {
        // Arrange
        var viewModel = new GeneralSettingsViewModel(_mockConfigService, _mockThemeService);

        // Act
        viewModel.StartWithWindows = true;

        // Assert
        viewModel.StartWithWindows.Should().BeTrue();
    }

    [AvaloniaFact]
    public async Task SaveAsync_ShouldPersistAllSettings()
    {
        // Arrange
        var viewModel = new GeneralSettingsViewModel(_mockConfigService, _mockThemeService);
        viewModel.Theme = "Dark";
        viewModel.Language = "fr-FR";
        viewModel.StartWithWindows = true;

        // Act
        await viewModel.SaveAsync();

        // Assert
        _mockThemeService.Received(1).SetTheme("Dark");
        await _mockConfigService.Received(1).SetValue("UI:Theme", "Dark", Arg.Any<CancellationToken>());
        await _mockConfigService.Received(1).SetValue("UI:Language", "fr-FR", Arg.Any<CancellationToken>());
        await _mockConfigService.Received(1).SetValue("App:StartWithWindows", true, Arg.Any<CancellationToken>());
    }

    [AvaloniaFact]
    public async Task ReloadAsync_ShouldRefreshValues()
    {
        // Arrange
        var viewModel = new GeneralSettingsViewModel(_mockConfigService, _mockThemeService);
        viewModel.Theme = "Dark";

        _mockThemeService.GetCurrentTheme().Returns(new ThemeConfiguration { ThemeMode = "Light", IsDarkMode = false });

        // Act
        await viewModel.ReloadAsync();

        // Assert
        viewModel.Theme.Should().Be("Light");
    }
}
