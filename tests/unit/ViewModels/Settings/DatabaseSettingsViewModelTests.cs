using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.ViewModels.Settings;
using NSubstitute;
using Xunit;

namespace MTM_Template_Tests.unit.ViewModels.Settings;

/// <summary>
/// Unit tests for DatabaseSettingsViewModel (Phase 2 - T036)
/// Tests connection string validation using FluentValidation patterns
/// </summary>
[Collection("AvaloniaTests")]
public class DatabaseSettingsViewModelTests
{
    private readonly IConfigurationService _mockConfigService;

    public DatabaseSettingsViewModelTests()
    {
        _mockConfigService = Substitute.For<IConfigurationService>();
        _mockConfigService.GetValue<string>("Database:ConnectionString", "").Returns("Server=localhost;Database=test");
    }

    [AvaloniaFact]
    public void DatabaseSettings_Construction_ShouldLoadConnectionString()
    {
        // Arrange & Act
        var viewModel = new DatabaseSettingsViewModel(_mockConfigService);

        // Assert
        viewModel.ConnectionString.Should().Be("Server=localhost;Database=test");
    }

    [AvaloniaFact]
    public void ConnectionString_ShouldChangeWhenSet()
    {
        // Arrange
        var viewModel = new DatabaseSettingsViewModel(_mockConfigService);

        // Act
        viewModel.ConnectionString = "Server=production;Database=prod";

        // Assert
        viewModel.ConnectionString.Should().Be("Server=production;Database=prod");
    }

    [AvaloniaFact]
    public async Task SaveAsync_ShouldPersistConnectionString()
    {
        // Arrange
        var viewModel = new DatabaseSettingsViewModel(_mockConfigService);
        viewModel.ConnectionString = "Server=production;Database=prod";

        // Act
        await viewModel.SaveAsync();

        // Assert
        await _mockConfigService.Received(1).SetValue("Database:ConnectionString", "Server=production;Database=prod", Arg.Any<CancellationToken>());
    }

    [AvaloniaFact]
    public async Task ReloadAsync_ShouldRefreshConnectionString()
    {
        // Arrange
        var viewModel = new DatabaseSettingsViewModel(_mockConfigService);
        viewModel.ConnectionString = "Changed";

        _mockConfigService.GetValue<string>("Database:ConnectionString", "").Returns("Server=localhost;Database=test");

        // Act
        await viewModel.ReloadAsync();

        // Assert
        viewModel.ConnectionString.Should().Be("Server=localhost;Database=test");
    }

    [AvaloniaFact]
    public void ConnectionString_NullValue_ShouldHandleGracefully()
    {
        // Arrange
        _mockConfigService.GetValue<string>("Database:ConnectionString", "").Returns((string?)null);
        var viewModel = new DatabaseSettingsViewModel(_mockConfigService);

        // Act
        viewModel.ConnectionString = null;

        // Assert
        viewModel.ConnectionString.Should().BeNull();
    }
}
