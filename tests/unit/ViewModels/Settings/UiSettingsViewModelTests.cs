using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.ViewModels.Settings;
using NSubstitute;
using Xunit;

namespace MTM_Template_Tests.unit.ViewModels.Settings;

/// <summary>
/// Unit tests for UiSettingsViewModel (Phase 2 - T039)
/// Tests theme/font/layout settings
/// </summary>
[Collection("AvaloniaTests")]
public class UiSettingsViewModelTests
{
    private readonly IConfigurationService _mockConfigService;

    public UiSettingsViewModelTests()
    {
        _mockConfigService = Substitute.For<IConfigurationService>();
        _mockConfigService.GetValue<string>("UI:FontFamily", "Segoe UI").Returns("Segoe UI");
    }

    [AvaloniaFact]
    public void UiSettings_Construction_ShouldLoadFontFamily()
    {
        // Arrange & Act
        var viewModel = new UiSettingsViewModel(_mockConfigService);

        // Assert
        viewModel.FontFamily.Should().Be("Segoe UI");
    }

    [AvaloniaFact]
    public void FontFamily_ShouldChangeWhenSet()
    {
        // Arrange
        var viewModel = new UiSettingsViewModel(_mockConfigService);

        // Act
        viewModel.FontFamily = "Arial";

        // Assert
        viewModel.FontFamily.Should().Be("Arial");
    }

    [AvaloniaFact]
    public async Task SaveAsync_ShouldPersistFontFamily()
    {
        // Arrange
        var viewModel = new UiSettingsViewModel(_mockConfigService);
        viewModel.FontFamily = "Arial";

        // Act
        await viewModel.SaveAsync();

        // Assert
        await _mockConfigService.Received(1).SetValue("UI:FontFamily", "Arial", Arg.Any<CancellationToken>());
    }

    [AvaloniaFact]
    public async Task ReloadAsync_ShouldRefreshFontFamily()
    {
        // Arrange
        var viewModel = new UiSettingsViewModel(_mockConfigService);
        viewModel.FontFamily = "Changed";

        _mockConfigService.GetValue<string>("UI:FontFamily", "Segoe UI").Returns("Segoe UI");

        // Act
        await viewModel.ReloadAsync();

        // Assert
        viewModel.FontFamily.Should().Be("Segoe UI");
    }

    [AvaloniaFact]
    public void FontFamily_CommonFonts_ShouldAccept()
    {
        // Arrange
        var viewModel = new UiSettingsViewModel(_mockConfigService);
        var commonFonts = new[] { "Segoe UI", "Arial", "Calibri", "Inter", "Roboto" };

        // Act & Assert
        foreach (var font in commonFonts)
        {
            viewModel.FontFamily = font;
            viewModel.FontFamily.Should().Be(font);
        }
    }
}
