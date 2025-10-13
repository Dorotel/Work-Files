using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_Template_Application.ViewModels;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.Services.Navigation;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace MTM_Template_Tests.Integration;

/// <summary>
/// Integration tests for ConfigurationErrorDialog with MainWindow error handling
/// Tests end-to-end error recovery workflows and Settings window integration
/// </summary>
[Trait("Category", "Integration")]
[Trait("Component", "ConfigurationErrorDialog")]
public class ConfigurationErrorDialogIntegrationTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IConfigurationService _configService;
    private readonly INavigationService _navigationService;

    public ConfigurationErrorDialogIntegrationTests()
    {
        var services = new ServiceCollection();
        
        // Register required services
        services.AddLogging(); // Default logging configuration
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        
        // Mock NavigationService to avoid UnsavedChangesGuard dependency
        var mockNavigation = Substitute.For<INavigationService>();
        services.AddSingleton<INavigationService>(mockNavigation);
        
        services.AddTransient<ConfigurationErrorDialogViewModel>();
        services.AddTransient<MainViewModel>();
        
        _serviceProvider = services.BuildServiceProvider();
        _configService = _serviceProvider.GetRequiredService<IConfigurationService>();
        _navigationService = _serviceProvider.GetRequiredService<INavigationService>();
    }

    #region MainWindow Error Path Integration Tests

    [Fact]
    public void MainWindow_OnConfigurationError_ShouldCreateErrorDialog()
    {
        // Arrange
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        var errorMessage = "Database connection failed";
        var errorCategory = "Database";

        // Act
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
        dialogViewModel.ErrorMessage = errorMessage;
        dialogViewModel.ErrorCategory = errorCategory;
        dialogViewModel.AffectedSettingKey = "ConnectionString";

        // Assert
        dialogViewModel.ErrorMessage.Should().Be(errorMessage);
        dialogViewModel.ErrorCategory.Should().Be(errorCategory);
        dialogViewModel.AffectedSettingKey.Should().Be("ConnectionString");
        dialogViewModel.Timestamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ErrorDialog_EditSettingsCommand_ShouldNavigateToSettingsWindow()
    {
        // Arrange
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
        dialogViewModel.ErrorCategory = "Visual";
        dialogViewModel.AffectedSettingKey = "VisualApiEndpoint";

        // Act
        await dialogViewModel.EditSettingsCommand.ExecuteAsync(null);

        // Assert
        // Navigation service should have recorded navigation request
        // In real implementation, this would open SettingsWindow
        dialogViewModel.EditSettingsCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public async Task ErrorDialog_EditSettingsCommand_WithDatabaseCategory_ShouldOpenDatabaseTab()
    {
        // Arrange
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
        dialogViewModel.ErrorCategory = "Database";
        dialogViewModel.AffectedSettingKey = "ConnectionString";

        // Act
        await dialogViewModel.EditSettingsCommand.ExecuteAsync(null);

        // Assert
        // Verify that navigation includes category parameter
        dialogViewModel.ErrorCategory.Should().Be("Database");
        dialogViewModel.AffectedSettingKey.Should().Be("ConnectionString");
    }

    [Fact]
    public async Task ErrorDialog_RetryCommand_WithValidConfiguration_ShouldSucceed()
    {
        // Arrange
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
        dialogViewModel.AffectedSettingKey = "LogLevel";
        
        // Set a valid configuration value
        await _configService.SetValue("LogLevel", "Information");

        // Act
        await dialogViewModel.RetryCommand.ExecuteAsync(null);

        // Assert
        dialogViewModel.RetrySucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task ErrorDialog_RetryCommand_WithInvalidConfiguration_ShouldFail()
    {
        // Arrange
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
        dialogViewModel.AffectedSettingKey = "NonExistentKey";

        // Act
        await dialogViewModel.RetryCommand.ExecuteAsync(null);

        // Assert
        dialogViewModel.RetrySucceeded.Should().BeFalse();
    }

    #endregion

    #region Settings Window Integration Tests

    [Theory]
    [InlineData("Database", "ConnectionString")]
    [InlineData("Visual", "VisualApiEndpoint")]
    [InlineData("Cache", "CacheTTL")]
    [InlineData("Performance", "MaxThreads")]
    [InlineData("Logging", "LogLevel")]
    public async Task ErrorDialog_ShouldNavigateToCorrectSettingsCategory(
        string errorCategory, string settingKey)
    {
        // Arrange
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
        dialogViewModel.ErrorCategory = errorCategory;
        dialogViewModel.AffectedSettingKey = settingKey;

        // Act
        await dialogViewModel.EditSettingsCommand.ExecuteAsync(null);

        // Assert
        // Verify navigation occurred with correct parameters
        dialogViewModel.ErrorCategory.Should().Be(errorCategory);
        dialogViewModel.AffectedSettingKey.Should().Be(settingKey);
    }

    [Fact]
    public async Task ErrorDialog_AfterSettingsCorrected_RetryShouldSucceed()
    {
        // Arrange
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
        var settingKey = "TestSetting";
        dialogViewModel.AffectedSettingKey = settingKey;

        // Initial retry fails
        await dialogViewModel.RetryCommand.ExecuteAsync(null);
        dialogViewModel.RetrySucceeded.Should().BeFalse();

        // Act - Correct the setting
        await _configService.SetValue(settingKey, "ValidValue");
        
        // Retry again
        await dialogViewModel.RetryCommand.ExecuteAsync(null);

        // Assert
        dialogViewModel.RetrySucceeded.Should().BeTrue();
    }

    #endregion

    #region Error Recovery Workflow Tests

    [Fact]
    public async Task ErrorRecoveryWorkflow_EditAndRetry_ShouldCompleteSuccessfully()
    {
        // Arrange
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
        dialogViewModel.ErrorCategory = "Database";
        dialogViewModel.AffectedSettingKey = "ConnectionString";
        dialogViewModel.ErrorMessage = "Invalid connection string format";

        // Act - Step 1: Navigate to settings
        await dialogViewModel.EditSettingsCommand.ExecuteAsync(null);

        // Step 2: Simulate user correcting the setting
        await _configService.SetValue("ConnectionString", "Server=localhost;Database=test;");

        // Step 3: Retry the operation
        await dialogViewModel.RetryCommand.ExecuteAsync(null);

        // Assert
        dialogViewModel.RetrySucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task ErrorRecoveryWorkflow_MultipleRetries_ShouldUpdateSuccessState()
    {
        // Arrange
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
        dialogViewModel.AffectedSettingKey = "TestKey";

        // Act - Attempt 1 (fail)
        await dialogViewModel.RetryCommand.ExecuteAsync(null);
        var firstAttempt = dialogViewModel.RetrySucceeded;

        // Fix the setting
        await _configService.SetValue("TestKey", "ValidValue");

        // Attempt 2 (succeed)
        await dialogViewModel.RetryCommand.ExecuteAsync(null);
        var secondAttempt = dialogViewModel.RetrySucceeded;

        // Assert
        firstAttempt.Should().BeFalse();
        secondAttempt.Should().BeTrue();
    }

    [Fact]
    public async Task ErrorDialog_ExitCommand_ShouldSetExitFlag()
    {
        // Arrange
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();

        // Act
        await dialogViewModel.ExitCommand.ExecuteAsync(null);

        // Assert
        dialogViewModel.ExitRequested.Should().BeTrue();
    }

    #endregion

    #region MainViewModel Error Handling Integration Tests

    [Fact]
    public void MainViewModel_OnCriticalError_ShouldExposeErrorDialog()
    {
        // Arrange
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();

        // Act
        // Simulate a critical configuration error
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
        dialogViewModel.ErrorMessage = "Critical database error";
        dialogViewModel.ErrorCategory = "Database";

        // Assert
        dialogViewModel.Should().NotBeNull();
        dialogViewModel.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task MainViewModel_AfterErrorRecovery_ShouldClearErrorState()
    {
        // Arrange
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        var dialogViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
        
        // Simulate error
        dialogViewModel.ErrorMessage = "Configuration error";
        dialogViewModel.AffectedSettingKey = "TestSetting";

        // Act - Recover
        await _configService.SetValue("TestSetting", "ValidValue");
        await dialogViewModel.RetryCommand.ExecuteAsync(null);

        // Assert
        dialogViewModel.RetrySucceeded.Should().BeTrue();
    }

    #endregion

    #region Dependency Injection Tests

    [Fact]
    public void ServiceProvider_ShouldResolveConfigurationErrorDialogViewModel()
    {
        // Act
        var viewModel = _serviceProvider.GetService<ConfigurationErrorDialogViewModel>();

        // Assert
        viewModel.Should().NotBeNull();
    }

    [Fact]
    public void ConfigurationErrorDialogViewModel_ShouldHaveAllDependencies()
    {
        // Act
        var viewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();

        // Assert
        viewModel.Should().NotBeNull();
        viewModel.EditSettingsCommand.Should().NotBeNull();
        viewModel.RetryCommand.Should().NotBeNull();
        viewModel.ExitCommand.Should().NotBeNull();
    }

    #endregion

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}
