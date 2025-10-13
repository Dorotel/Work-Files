using Xunit;
using FluentAssertions;
using NSubstitute;
using Microsoft.Extensions.Logging;
using MTM_Template_Application.ViewModels;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.Services.Navigation;
using System;
using System.Threading.Tasks;

namespace MTM_Template_Tests.Unit.ViewModels;

/// <summary>
/// Unit tests for ConfigurationErrorDialogViewModel
/// Tests error message display, recovery options, and command execution
/// </summary>
[Trait("Category", "Unit")]
[Trait("Component", "ViewModel")]
public class ConfigurationErrorDialogViewModelTests
{
    private readonly ILogger<ConfigurationErrorDialogViewModel> _mockLogger;
    private readonly IConfigurationService _mockConfigService;
    private readonly INavigationService _mockNavigationService;

    public ConfigurationErrorDialogViewModelTests()
    {
        _mockLogger = Substitute.For<ILogger<ConfigurationErrorDialogViewModel>>();
        _mockConfigService = Substitute.For<IConfigurationService>();
        _mockNavigationService = Substitute.For<INavigationService>();
    }

    private ConfigurationErrorDialogViewModel CreateViewModel(
        string errorMessage = "Test error",
        string errorCategory = "Database",
        string? affectedSettingKey = "ConnectionString")
    {
        return new ConfigurationErrorDialogViewModel(
            _mockLogger,
            _mockConfigService,
            _mockNavigationService)
        {
            ErrorMessage = errorMessage,
            ErrorCategory = errorCategory,
            AffectedSettingKey = affectedSettingKey
        };
    }

    #region Property Tests

    [Fact]
    public void ErrorMessage_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var newMessage = "New error message";

        // Act
        viewModel.ErrorMessage = newMessage;

        // Assert
        viewModel.ErrorMessage.Should().Be(newMessage);
    }

    [Fact]
    public void ErrorCategory_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var newCategory = "Visual";

        // Act
        viewModel.ErrorCategory = newCategory;

        // Assert
        viewModel.ErrorCategory.Should().Be(newCategory);
    }

    [Fact]
    public void AffectedSettingKey_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var newKey = "VisualApiEndpoint";

        // Act
        viewModel.AffectedSettingKey = newKey;

        // Assert
        viewModel.AffectedSettingKey.Should().Be(newKey);
    }

    [Fact]
    public void Timestamp_WhenInitialized_ShouldBeRecent()
    {
        // Arrange & Act
        var viewModel = CreateViewModel();

        // Assert
        viewModel.Timestamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void AffectedSettingKey_CanBeNull()
    {
        // Arrange & Act
        var viewModel = CreateViewModel(affectedSettingKey: null);

        // Assert
        viewModel.AffectedSettingKey.Should().BeNull();
    }

    #endregion

    #region EditSettings Command Tests

    [Fact]
    public void EditSettingsCommand_ShouldBeAvailable()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act & Assert
        viewModel.EditSettingsCommand.Should().NotBeNull();
    }

    [Fact]
    public async Task EditSettingsCommand_WhenExecuted_ShouldNavigateToSettings()
    {
        // Arrange
        var viewModel = CreateViewModel(errorCategory: "Database", affectedSettingKey: "ConnectionString");

        // Act
        await viewModel.EditSettingsCommand.ExecuteAsync(null);

        // Assert
        await _mockNavigationService.Received(1).NavigateToAsync(
            Arg.Is<string>(s => s.Contains("Settings")),
            Arg.Any<Dictionary<string, object>?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EditSettingsCommand_WithErrorCategory_ShouldPassCategoryToNavigation()
    {
        // Arrange
        var category = "Visual";
        var viewModel = CreateViewModel(errorCategory: category);

        // Act
        await viewModel.EditSettingsCommand.ExecuteAsync(null);

        // Assert
        await _mockNavigationService.Received(1).NavigateToAsync(
            Arg.Any<string>(),
            Arg.Is<Dictionary<string, object>?>(d => d != null && d.ContainsKey("Category")),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EditSettingsCommand_WithAffectedSettingKey_ShouldHighlightSetting()
    {
        // Arrange
        var settingKey = "VisualApiEndpoint";
        var viewModel = CreateViewModel(affectedSettingKey: settingKey);

        // Act
        await viewModel.EditSettingsCommand.ExecuteAsync(null);

        // Assert
        await _mockNavigationService.Received(1).NavigateToAsync(
            Arg.Any<string>(),
            Arg.Is<Dictionary<string, object>?>(d => d != null && d.ContainsKey("HighlightKey")),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region Retry Command Tests

    [Fact]
    public void RetryCommand_ShouldBeAvailable()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act & Assert
        viewModel.RetryCommand.Should().NotBeNull();
    }

    [Fact]
    public async Task RetryCommand_WhenExecuted_ShouldAttemptConfigurationRetrieval()
    {
        // Arrange
        var settingKey = "ConnectionString";
        var viewModel = CreateViewModel(affectedSettingKey: settingKey);
        _mockConfigService.GetValue<string>(settingKey, Arg.Any<string>())
            .Returns("ValidConnectionString");

        // Act
        await viewModel.RetryCommand.ExecuteAsync(null);

        // Assert
        _mockConfigService.Received(1).GetValue<string>(settingKey, Arg.Any<string>());
    }

    [Fact]
    public async Task RetryCommand_WhenSuccessful_ShouldSetSuccessFlag()
    {
        // Arrange
        var viewModel = CreateViewModel(affectedSettingKey: "TestKey");
        _mockConfigService.GetValue<string>(Arg.Any<string>(), Arg.Any<string>())
            .Returns("ValidValue");

        // Act
        await viewModel.RetryCommand.ExecuteAsync(null);

        // Assert
        viewModel.RetrySucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task RetryCommand_WhenFailed_ShouldKeepFailureState()
    {
        // Arrange
        var viewModel = CreateViewModel(affectedSettingKey: "TestKey");
        _mockConfigService.When(x => x.GetValue<string>(Arg.Any<string>(), Arg.Any<string>()))
            .Do(x => throw new InvalidOperationException("Retry failed"));

        // Act
        await viewModel.RetryCommand.ExecuteAsync(null);

        // Assert
        viewModel.RetrySucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task RetryCommand_WhenAffectedSettingKeyIsNull_ShouldLogWarning()
    {
        // Arrange
        var viewModel = CreateViewModel(affectedSettingKey: null);

        // Act
        await viewModel.RetryCommand.ExecuteAsync(null);

        // Assert
        _mockLogger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    #endregion

    #region Exit Command Tests

    [Fact]
    public void ExitCommand_ShouldBeAvailable()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act & Assert
        viewModel.ExitCommand.Should().NotBeNull();
    }

    [Fact]
    public async Task ExitCommand_WhenExecuted_ShouldSetExitRequestedFlag()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        await viewModel.ExitCommand.ExecuteAsync(null);

        // Assert
        viewModel.ExitRequested.Should().BeTrue();
    }

    [Fact]
    public async Task ExitCommand_ShouldLogExitRequest()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        await viewModel.ExitCommand.ExecuteAsync(null);

        // Assert
        _mockLogger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("exit")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    #endregion

    #region Error Scenario Tests

    [Theory]
    [InlineData("Database", "ConnectionString", "Failed to connect to database")]
    [InlineData("Visual", "VisualApiEndpoint", "Invalid Visual API endpoint")]
    [InlineData("Cache", "CacheTTL", "Cache configuration error")]
    [InlineData("Performance", "MaxThreads", "Invalid performance setting")]
    public void Constructor_WithVariousErrorCategories_ShouldInitializeCorrectly(
        string category, string settingKey, string errorMessage)
    {
        // Arrange & Act
        var viewModel = CreateViewModel(errorMessage, category, settingKey);

        // Assert
        viewModel.ErrorCategory.Should().Be(category);
        viewModel.AffectedSettingKey.Should().Be(settingKey);
        viewModel.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void ViewModel_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ConfigurationErrorDialogViewModel(
                null!,
                _mockConfigService,
                _mockNavigationService));
    }

    [Fact]
    public void ViewModel_WithNullConfigService_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ConfigurationErrorDialogViewModel(
                _mockLogger,
                null!,
                _mockNavigationService));
    }

    [Fact]
    public void ViewModel_WithNullNavigationService_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ConfigurationErrorDialogViewModel(
                _mockLogger,
                _mockConfigService,
                null!));
    }

    #endregion

    #region RecoveryOptions Tests

    [Fact]
    public void RecoveryOptions_ShouldIncludeEditSettings()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        var options = viewModel.RecoveryOptions;

        // Assert
        options.Should().Contain(o => o.Contains("Edit Settings"));
    }

    [Fact]
    public void RecoveryOptions_ShouldIncludeRetry()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        var options = viewModel.RecoveryOptions;

        // Assert
        options.Should().Contain(o => o.Contains("Retry"));
    }

    [Fact]
    public void RecoveryOptions_ShouldIncludeExit()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        var options = viewModel.RecoveryOptions;

        // Assert
        options.Should().Contain(o => o.Contains("Exit"));
    }

    [Fact]
    public void RecoveryOptions_ShouldHaveThreeOptions()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        var options = viewModel.RecoveryOptions;

        // Assert
        options.Should().HaveCount(3);
    }

    #endregion
}
