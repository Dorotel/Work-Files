using CommunityToolkit.Mvvm.ComponentModel;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.Services.Theme;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MTM_Template_Application.ViewModels.Settings;

/// <summary>
/// ViewModel for General settings category
/// Manages theme, language, and startup settings
/// </summary>
public partial class GeneralSettingsViewModel : ObservableObject
{
    private readonly IConfigurationService _configService;
    private readonly IThemeService _themeService;

    [ObservableProperty]
    private string? _theme;

    [ObservableProperty]
    private string? _language;

    [ObservableProperty]
    private bool _startWithWindows;

    public GeneralSettingsViewModel(IConfigurationService configService, IThemeService themeService)
    {
        ArgumentNullException.ThrowIfNull(configService);
        ArgumentNullException.ThrowIfNull(themeService);
        _configService = configService;
        _themeService = themeService;

        // Load current values
        var currentTheme = _themeService.GetCurrentTheme();
        _theme = currentTheme.ThemeMode; // Get from ThemeService, not config
        _language = _configService.GetValue<string>("UI:Language", "en-US");
        _startWithWindows = _configService.GetValue<bool>("App:StartWithWindows", false);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        // Apply theme immediately via ThemeService
        if (!string.IsNullOrWhiteSpace(Theme))
        {
            _themeService.SetTheme(Theme);
        }

        // Save to configuration
        await _configService.SetValue("UI:Theme", Theme ?? "Light", cancellationToken);
        await _configService.SetValue("UI:Language", Language ?? "en-US", cancellationToken);
        await _configService.SetValue("App:StartWithWindows", StartWithWindows, cancellationToken);
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        await _configService.ReloadAsync(cancellationToken);
        var currentTheme = _themeService.GetCurrentTheme();
        Theme = currentTheme.ThemeMode; // Reload from ThemeService
        Language = _configService.GetValue<string>("UI:Language", "en-US");
        StartWithWindows = _configService.GetValue<bool>("App:StartWithWindows", false);
    }
}

/// <summary>
/// ViewModel for Database settings category
/// </summary>
public partial class DatabaseSettingsViewModel : ObservableObject
{
    private readonly IConfigurationService _configService;

    [ObservableProperty]
    private string? _connectionString;

    [ObservableProperty]
    private string? _validationError;

    [ObservableProperty]
    private bool _isValid = true;

    public DatabaseSettingsViewModel(IConfigurationService configService)
    {
        ArgumentNullException.ThrowIfNull(configService);
        _configService = configService;
        _connectionString = _configService.GetValue<string>("Database:ConnectionString", "");
    }

    partial void OnConnectionStringChanged(string? value)
    {
        ValidateConnectionString(value);
    }

    private void ValidateConnectionString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ValidationError = "Connection string cannot be empty";
            IsValid = false;
            return;
        }

        // Basic MySQL connection string validation - at least one required field
        var hasServer = value.Contains("Server=", StringComparison.OrdinalIgnoreCase);
        var hasDatabase = value.Contains("Database=", StringComparison.OrdinalIgnoreCase);

        if (!hasServer && !hasDatabase)
        {
            ValidationError = "Connection string must contain Server or Database";
            IsValid = false;
            return;
        }

        ValidationError = null;
        IsValid = true;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (!IsValid)
        {
            throw new InvalidOperationException($"Cannot save invalid settings: {ValidationError}");
        }

        await _configService.SetValue("Database:ConnectionString", ConnectionString ?? "", cancellationToken);
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        await _configService.ReloadAsync(cancellationToken);
        ConnectionString = _configService.GetValue<string>("Database:ConnectionString", "");
    }
}

/// <summary>
/// ViewModel for Visual ERP settings category
/// </summary>
public partial class VisualSettingsViewModel : ObservableObject
{
    private readonly IConfigurationService _configService;

    [ObservableProperty]
    private string? _apiEndpoint;

    [ObservableProperty]
    private string? _validationError;

    [ObservableProperty]
    private bool _isValid = true;

    public VisualSettingsViewModel(IConfigurationService configService)
    {
        ArgumentNullException.ThrowIfNull(configService);
        _configService = configService;
        _apiEndpoint = _configService.GetValue<string>("Visual:ApiEndpoint", "");
    }

    partial void OnApiEndpointChanged(string? value)
    {
        ValidateApiEndpoint(value);
    }

    private void ValidateApiEndpoint(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ValidationError = "API endpoint cannot be empty";
            IsValid = false;
            return;
        }

        // Validate URL format
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            ValidationError = "Invalid URL format";
            IsValid = false;
            return;
        }

        // Must be HTTP or HTTPS
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            ValidationError = "URL must use HTTP or HTTPS protocol";
            IsValid = false;
            return;
        }

        ValidationError = null;
        IsValid = true;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (!IsValid)
        {
            throw new InvalidOperationException($"Cannot save invalid settings: {ValidationError}");
        }

        await _configService.SetValue("Visual:ApiEndpoint", ApiEndpoint ?? "", cancellationToken);
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        await _configService.ReloadAsync(cancellationToken);
        ApiEndpoint = _configService.GetValue<string>("Visual:ApiEndpoint", "");
    }
}

/// <summary>
/// ViewModel for Logging settings category
/// </summary>
public partial class LoggingSettingsViewModel : ObservableObject
{
    private readonly IConfigurationService _configService;

    [ObservableProperty]
    private string? _logLevel;

    public LoggingSettingsViewModel(IConfigurationService configService)
    {
        ArgumentNullException.ThrowIfNull(configService);
        _configService = configService;
        _logLevel = _configService.GetValue<string>("Logging:Level", "Information");
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _configService.SetValue("Logging:Level", LogLevel ?? "Information", cancellationToken);
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        await _configService.ReloadAsync(cancellationToken);
        LogLevel = _configService.GetValue<string>("Logging:Level", "Information");
    }
}

/// <summary>
/// ViewModel for UI settings category
/// </summary>
public partial class UiSettingsViewModel : ObservableObject
{
    private readonly IConfigurationService _configService;

    [ObservableProperty]
    private string? _fontFamily;

    public UiSettingsViewModel(IConfigurationService configService)
    {
        ArgumentNullException.ThrowIfNull(configService);
        _configService = configService;
        _fontFamily = _configService.GetValue<string>("UI:FontFamily", "Segoe UI");
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _configService.SetValue("UI:FontFamily", FontFamily ?? "Segoe UI", cancellationToken);
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        await _configService.ReloadAsync(cancellationToken);
        FontFamily = _configService.GetValue<string>("UI:FontFamily", "Segoe UI");
    }
}

/// <summary>
/// ViewModel for Cache settings category
/// </summary>
public partial class CacheSettingsViewModel : ObservableObject
{
    private readonly IConfigurationService _configService;

    [ObservableProperty]
    private int _ttlSeconds;

    [ObservableProperty]
    private string? _validationError;

    [ObservableProperty]
    private bool _isValid = true;

    public CacheSettingsViewModel(IConfigurationService configService)
    {
        ArgumentNullException.ThrowIfNull(configService);
        _configService = configService;
        _ttlSeconds = _configService.GetValue<int>("Cache:TTLSeconds", 3600);
    }

    partial void OnTtlSecondsChanged(int value)
    {
        ValidateTtlSeconds(value);
    }

    private void ValidateTtlSeconds(int value)
    {
        // TTL must be between 1 second and 24 hours (86400 seconds)
        if (value < 1)
        {
            ValidationError = "TTL must be at least 1 second";
            IsValid = false;
            return;
        }

        if (value > 86400)
        {
            ValidationError = "TTL cannot exceed 24 hours (86400 seconds)";
            IsValid = false;
            return;
        }

        ValidationError = null;
        IsValid = true;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (!IsValid)
        {
            throw new InvalidOperationException($"Cannot save invalid settings: {ValidationError}");
        }

        await _configService.SetValue("Cache:TTLSeconds", TtlSeconds, cancellationToken);
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        await _configService.ReloadAsync(cancellationToken);
        TtlSeconds = _configService.GetValue<int>("Cache:TTLSeconds", 3600);
    }
}

/// <summary>
/// ViewModel for Performance settings category
/// </summary>
public partial class PerformanceSettingsViewModel : ObservableObject
{
    private readonly IConfigurationService _configService;

    [ObservableProperty]
    private int _bootTargetMs;

    [ObservableProperty]
    private string? _validationError;

    [ObservableProperty]
    private bool _isValid = true;

    public PerformanceSettingsViewModel(IConfigurationService configService)
    {
        ArgumentNullException.ThrowIfNull(configService);
        _configService = configService;
        _bootTargetMs = _configService.GetValue<int>("Performance:BootTargetMs", 10000);
    }

    partial void OnBootTargetMsChanged(int value)
    {
        ValidateBootTargetMs(value);
    }

    private void ValidateBootTargetMs(int value)
    {
        // Boot target must be positive and reasonable (between 1 second and 5 minutes)
        if (value < 1000)
        {
            ValidationError = "Boot target must be at least 1000ms (1 second)";
            IsValid = false;
            return;
        }

        if (value > 300000)
        {
            ValidationError = "Boot target cannot exceed 300000ms (5 minutes)";
            IsValid = false;
            return;
        }

        ValidationError = null;
        IsValid = true;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (!IsValid)
        {
            throw new InvalidOperationException($"Cannot save invalid settings: {ValidationError}");
        }

        await _configService.SetValue("Performance:BootTargetMs", BootTargetMs, cancellationToken);
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        await _configService.ReloadAsync(cancellationToken);
        BootTargetMs = _configService.GetValue<int>("Performance:BootTargetMs", 10000);
    }
}

/// <summary>
/// ViewModel for Developer settings category
/// </summary>
public partial class DeveloperSettingsViewModel : ObservableObject
{
    private readonly IConfigurationService _configService;

    [ObservableProperty]
    private bool _debugMode;

    public DeveloperSettingsViewModel(IConfigurationService configService)
    {
        ArgumentNullException.ThrowIfNull(configService);
        _configService = configService;
        _debugMode = _configService.GetValue<bool>("Developer:DebugMode", false);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _configService.SetValue("Developer:DebugMode", DebugMode, cancellationToken);
    }

    public async Task ReloadAsync(CancellationToken cancellationToken = default)
    {
        await _configService.ReloadAsync(cancellationToken);
        DebugMode = _configService.GetValue<bool>("Developer:DebugMode", false);
    }
}
