using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.ViewModels.Settings;
using NSubstitute;
using Xunit;

namespace MTM_Template_Tests.unit.ViewModels.Settings;

/// <summary>
/// Unit tests for CacheSettingsViewModel (Phase 2 - T040)
/// Tests TTL numeric validation and size limits
/// </summary>
[Collection("AvaloniaTests")]
public class CacheSettingsViewModelTests
{
    private readonly IConfigurationService _mockConfigService;

    public CacheSettingsViewModelTests()
    {
        _mockConfigService = Substitute.For<IConfigurationService>();
        _mockConfigService.GetValue<int>("Cache:TTLSeconds", 3600).Returns(3600);
    }

    [AvaloniaFact]
    public void CacheSettings_Construction_ShouldLoadTTL()
    {
        // Arrange & Act
        var viewModel = new CacheSettingsViewModel(_mockConfigService);

        // Assert
        viewModel.TtlSeconds.Should().Be(3600);
    }

    [AvaloniaFact]
    public void TtlSeconds_ShouldChangeWhenSet()
    {
        // Arrange
        var viewModel = new CacheSettingsViewModel(_mockConfigService);

        // Act
        viewModel.TtlSeconds = 7200;

        // Assert
        viewModel.TtlSeconds.Should().Be(7200);
    }

    [AvaloniaFact]
    public async Task SaveAsync_ShouldPersistTTL()
    {
        // Arrange
        var viewModel = new CacheSettingsViewModel(_mockConfigService);
        viewModel.TtlSeconds = 7200;

        // Act
        await viewModel.SaveAsync();

        // Assert
        await _mockConfigService.Received(1).SetValue("Cache:TTLSeconds", 7200, Arg.Any<CancellationToken>());
    }

    [AvaloniaFact]
    public async Task ReloadAsync_ShouldRefreshTTL()
    {
        // Arrange
        var viewModel = new CacheSettingsViewModel(_mockConfigService);
        viewModel.TtlSeconds = 7200;

        _mockConfigService.GetValue<int>("Cache:TTLSeconds", 3600).Returns(3600);

        // Act
        await viewModel.ReloadAsync();

        // Assert
        viewModel.TtlSeconds.Should().Be(3600);
    }

    [AvaloniaFact]
    public void TtlSeconds_PositiveValues_ShouldAccept()
    {
        // Arrange
        var viewModel = new CacheSettingsViewModel(_mockConfigService);

        // Act
        viewModel.TtlSeconds = 1;

        // Assert
        viewModel.TtlSeconds.Should().Be(1);
    }

    [AvaloniaFact]
    public void TtlSeconds_CommonValues_ShouldAccept()
    {
        // Arrange
        var viewModel = new CacheSettingsViewModel(_mockConfigService);
        var commonValues = new[] { 60, 300, 600, 1800, 3600, 7200, 86400 }; // 1min to 1day

        // Act & Assert
        foreach (var value in commonValues)
        {
            viewModel.TtlSeconds = value;
            viewModel.TtlSeconds.Should().Be(value);
        }
    }
}
