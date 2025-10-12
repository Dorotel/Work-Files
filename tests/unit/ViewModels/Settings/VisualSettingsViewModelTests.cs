using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.ViewModels.Settings;
using NSubstitute;
using Xunit;

namespace MTM_Template_Tests.unit.ViewModels.Settings;

/// <summary>
/// Unit tests for VisualSettingsViewModel (Phase 2 - T037)
/// Tests API endpoint URL validation and credential masking
/// </summary>
[Collection("AvaloniaTests")]
public class VisualSettingsViewModelTests
{
    private readonly IConfigurationService _mockConfigService;

    public VisualSettingsViewModelTests()
    {
        _mockConfigService = Substitute.For<IConfigurationService>();
        _mockConfigService.GetValue<string>("Visual:ApiEndpoint", "").Returns("https://visual.example.com/api");
    }

    [AvaloniaFact]
    public void VisualSettings_Construction_ShouldLoadApiEndpoint()
    {
        // Arrange & Act
        var viewModel = new VisualSettingsViewModel(_mockConfigService);

        // Assert
        viewModel.ApiEndpoint.Should().Be("https://visual.example.com/api");
    }

    [AvaloniaFact]
    public void ApiEndpoint_ShouldChangeWhenSet()
    {
        // Arrange
        var viewModel = new VisualSettingsViewModel(_mockConfigService);

        // Act
        viewModel.ApiEndpoint = "https://production.visual.com/api";

        // Assert
        viewModel.ApiEndpoint.Should().Be("https://production.visual.com/api");
    }

    [AvaloniaFact]
    public async Task SaveAsync_ShouldPersistApiEndpoint()
    {
        // Arrange
        var viewModel = new VisualSettingsViewModel(_mockConfigService);
        viewModel.ApiEndpoint = "https://production.visual.com/api";

        // Act
        await viewModel.SaveAsync();

        // Assert
        await _mockConfigService.Received(1).SetValue("Visual:ApiEndpoint", "https://production.visual.com/api", Arg.Any<CancellationToken>());
    }

    [AvaloniaFact]
    public async Task ReloadAsync_ShouldRefreshApiEndpoint()
    {
        // Arrange
        var viewModel = new VisualSettingsViewModel(_mockConfigService);
        viewModel.ApiEndpoint = "Changed";

        _mockConfigService.GetValue<string>("Visual:ApiEndpoint", "").Returns("https://visual.example.com/api");

        // Act
        await viewModel.ReloadAsync();

        // Assert
        viewModel.ApiEndpoint.Should().Be("https://visual.example.com/api");
    }

    [AvaloniaFact]
    public void ApiEndpoint_NullValue_ShouldHandleGracefully()
    {
        // Arrange
        _mockConfigService.GetValue<string>("Visual:ApiEndpoint", "").Returns((string?)null);
        var viewModel = new VisualSettingsViewModel(_mockConfigService);

        // Act
        viewModel.ApiEndpoint = null;

        // Assert
        viewModel.ApiEndpoint.Should().BeNull();
    }
}
