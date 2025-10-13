using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.Services.Navigation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MTM_Template_Application.ViewModels;

/// <summary>
/// ViewModel for the Configuration Error Dialog
/// Handles display of configuration errors and provides recovery options
/// </summary>
public partial class ConfigurationErrorDialogViewModel : ObservableObject
{
    private readonly ILogger<ConfigurationErrorDialogViewModel> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly INavigationService _navigationService;

    /// <summary>
    /// The error message to display to the user
    /// </summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>
    /// The category of the configuration error (Database, Visual, Cache, Performance, etc.)
    /// </summary>
    [ObservableProperty]
    private string _errorCategory = string.Empty;

    /// <summary>
    /// The specific configuration key that caused the error (nullable)
    /// </summary>
    [ObservableProperty]
    private string? _affectedSettingKey;

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    [ObservableProperty]
    private DateTime _timestamp = DateTime.Now;

    /// <summary>
    /// Flag indicating whether the retry operation succeeded
    /// </summary>
    [ObservableProperty]
    private bool _retrySucceeded;

    /// <summary>
    /// Flag indicating whether the user requested to exit the application
    /// </summary>
    [ObservableProperty]
    private bool _exitRequested;

    /// <summary>
    /// Available recovery options for the user
    /// </summary>
    public List<string> RecoveryOptions { get; } = new()
    {
        "Edit Settings",
        "Retry",
        "Exit Application"
    };

    public ConfigurationErrorDialogViewModel(
        ILogger<ConfigurationErrorDialogViewModel> logger,
        IConfigurationService configurationService,
        INavigationService navigationService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configurationService);
        ArgumentNullException.ThrowIfNull(navigationService);

        _logger = logger;
        _configurationService = configurationService;
        _navigationService = navigationService;
    }

    /// <summary>
    /// Command to open the Settings window to the relevant category and highlight the affected setting
    /// </summary>
    [RelayCommand]
    private async Task EditSettingsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Opening settings for error category: {Category}, affected key: {Key}",
                ErrorCategory,
                AffectedSettingKey ?? "None");

            // Build navigation parameters (non-nullable dictionary)
            var navigationParams = new Dictionary<string, object>
            {
                { "Category", ErrorCategory },
                { "HighlightKey", AffectedSettingKey ?? string.Empty }
            };

            // Navigate to settings window
            await _navigationService.NavigateToAsync("SettingsWindow", navigationParams, cancellationToken);

            _logger.LogInformation("Successfully navigated to Settings window");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to Settings window");
        }
    }

    /// <summary>
    /// Command to retry the configuration operation that failed
    /// </summary>
    [RelayCommand]
    private async Task RetryAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(AffectedSettingKey))
            {
                _logger.LogWarning("Cannot retry: AffectedSettingKey is null or empty");
                RetrySucceeded = false;
                return;
            }

            _logger.LogInformation("Retrying configuration for key: {Key}", AffectedSettingKey);

            // Attempt to retrieve the configuration value
            // This will throw if the configuration is still invalid
            var value = _configurationService.GetValue<string>(AffectedSettingKey, string.Empty);

            if (!string.IsNullOrWhiteSpace(value))
            {
                _logger.LogInformation("Retry succeeded for key: {Key}", AffectedSettingKey);
                RetrySucceeded = true;
            }
            else
            {
                _logger.LogWarning("Retry failed: Configuration value is still empty for key: {Key}", AffectedSettingKey);
                RetrySucceeded = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Retry failed for key: {Key}", AffectedSettingKey);
            RetrySucceeded = false;
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Command to exit the application gracefully
    /// </summary>
    [RelayCommand]
    private async Task ExitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("User requested application exit due to configuration error");
            ExitRequested = true;

            // Note: Actual application shutdown should be handled by the MainWindow/MainViewModel
            // This just sets the flag for the UI to respond to
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing exit request");
        }

        await Task.CompletedTask;
    }
}
