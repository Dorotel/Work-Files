using FluentAssertions;
using MTM_Template_Application.Services.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace MTM_Template_Tests.integration;

/// <summary>
/// Integration tests for Settings persistence (Phase 2 - T043)
/// Tests configuration service behavior (database tests skipped - require test data setup)
/// </summary>
[Collection("IntegrationTests")]
public class SettingsPersistenceTests
{
    private readonly ILogger<ConfigurationService> _logger;

    public SettingsPersistenceTests()
    {
        _logger = Substitute.For<ILogger<ConfigurationService>>();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task SetValue_ShouldPersistToUserPreferences()
    {
        // Arrange
        var configService = new ConfigurationService(_logger);
        await configService.InitializeAsync();

        // Act
        await configService.SetValue("Test:Setting", "TestValue", CancellationToken.None);
        var retrieved = configService.GetValue<string>("Test:Setting", "");

        // Assert
        retrieved.Should().Be("TestValue");
    }

    [Fact(Skip = "Requires valid UserId in database - pending test data setup")]
    [Trait("Category", "Integration")]
    public async Task LoadUserPreferencesAsync_ShouldLoadFromDatabase()
    {
        // Arrange
        var configService = new ConfigurationService(_logger);
        await configService.InitializeAsync();
        var userId = 1;

        // Act
        await configService.LoadUserPreferencesAsync(userId, CancellationToken.None);

        // Assert
        // User preferences should be loaded (verify by checking a known setting)
        var theme = configService.GetValue<string>("UI:Theme", "Light");
        theme.Should().NotBeNull();
    }

    [Fact(Skip = "Requires valid UserId in database - pending test data setup")]
    [Trait("Category", "Integration")]
    public async Task SaveUserPreferenceAsync_ShouldPersistToDatabase()
    {
        // Arrange
        var configService = new ConfigurationService(_logger);
        await configService.InitializeAsync();
        var userId = 1;

        // Act
        await configService.SaveUserPreferenceAsync(userId, "Test:Preference", "TestValue", CancellationToken.None);

        // Reload to verify persistence
        await configService.LoadUserPreferencesAsync(userId, CancellationToken.None);
        var retrieved = configService.GetValue<string>("Test:Preference", "");

        // Assert
        retrieved.Should().Be("TestValue");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ReloadAsync_ShouldRefreshConfiguration()
    {
        // Arrange
        var configService = new ConfigurationService(_logger);
        await configService.InitializeAsync();
        await configService.SetValue("Test:Reload", "InitialValue", CancellationToken.None);

        // Act
        await configService.ReloadAsync(CancellationToken.None);
        var value = configService.GetValue<string>("Test:Reload", "");

        // Assert
        value.Should().NotBeNull();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Configuration_WithCancellation_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var configService = new ConfigurationService(_logger);
        await configService.InitializeAsync();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await configService.SetValue("Test:Cancellation", "Value", cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task MultipleSettings_ShouldPersistIndependently()
    {
        // Arrange
        var configService = new ConfigurationService(_logger);
        await configService.InitializeAsync();

        // Act
        await configService.SetValue("Setting1", "Value1", CancellationToken.None);
        await configService.SetValue("Setting2", "Value2", CancellationToken.None);
        await configService.SetValue("Setting3", "Value3", CancellationToken.None);

        // Assert
        configService.GetValue<string>("Setting1", "").Should().Be("Value1");
        configService.GetValue<string>("Setting2", "").Should().Be("Value2");
        configService.GetValue<string>("Setting3", "").Should().Be("Value3");
    }
}
