using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_Template_Application.Services.Configuration;
using MTM_Template_Application.Services.Theme;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace MTM_Template_Application.ViewModels.Settings;

/// <summary>
/// ViewModel for Settings window (Phase 2)
/// Manages 8 category ViewModels and coordinates Save/Cancel/Export/Import operations
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly IConfigurationService _configService;
    private readonly IThemeService _themeService;

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    // Category ViewModels
    [ObservableProperty]
    private GeneralSettingsViewModel _generalSettings;

    [ObservableProperty]
    private DatabaseSettingsViewModel _databaseSettings;

    [ObservableProperty]
    private VisualSettingsViewModel _visualSettings;

    [ObservableProperty]
    private LoggingSettingsViewModel _loggingSettings;

    [ObservableProperty]
    private UiSettingsViewModel _uiSettings;

    [ObservableProperty]
    private CacheSettingsViewModel _cacheSettings;

    [ObservableProperty]
    private PerformanceSettingsViewModel _performanceSettings;

    [ObservableProperty]
    private DeveloperSettingsViewModel _developerSettings;

    public SettingsViewModel(IConfigurationService configService, IThemeService themeService)
    {
        ArgumentNullException.ThrowIfNull(configService);
        ArgumentNullException.ThrowIfNull(themeService);
        _configService = configService;
        _themeService = themeService;

        // Initialize category ViewModels
        _generalSettings = new GeneralSettingsViewModel(configService, themeService);
        _databaseSettings = new DatabaseSettingsViewModel(configService);
        _visualSettings = new VisualSettingsViewModel(configService);
        _loggingSettings = new LoggingSettingsViewModel(configService);
        _uiSettings = new UiSettingsViewModel(configService);
        _cacheSettings = new CacheSettingsViewModel(configService);
        _performanceSettings = new PerformanceSettingsViewModel(configService);
        _developerSettings = new DeveloperSettingsViewModel(configService);

        // Subscribe to property changes for unsaved changes tracking
        GeneralSettings.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        DatabaseSettings.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        VisualSettings.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        LoggingSettings.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        UiSettings.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        CacheSettings.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        PerformanceSettings.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        DeveloperSettings.PropertyChanged += (s, e) => HasUnsavedChanges = true;
    }

    [RelayCommand]
    private async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        // Save all category settings
        await GeneralSettings.SaveAsync(cancellationToken);
        await DatabaseSettings.SaveAsync(cancellationToken);
        await VisualSettings.SaveAsync(cancellationToken);
        await LoggingSettings.SaveAsync(cancellationToken);
        await UiSettings.SaveAsync(cancellationToken);
        await CacheSettings.SaveAsync(cancellationToken);
        await PerformanceSettings.SaveAsync(cancellationToken);
        await DeveloperSettings.SaveAsync(cancellationToken);

        HasUnsavedChanges = false;
    }

    [RelayCommand]
    private async Task CancelAsync(CancellationToken cancellationToken = default)
    {
        // Reload all category settings to revert changes
        await GeneralSettings.ReloadAsync(cancellationToken);
        await DatabaseSettings.ReloadAsync(cancellationToken);
        await VisualSettings.ReloadAsync(cancellationToken);
        await LoggingSettings.ReloadAsync(cancellationToken);
        await UiSettings.ReloadAsync(cancellationToken);
        await CacheSettings.ReloadAsync(cancellationToken);
        await PerformanceSettings.ReloadAsync(cancellationToken);
        await DeveloperSettings.ReloadAsync(cancellationToken);

        HasUnsavedChanges = false;
    }

    [RelayCommand]
    private async Task ExportAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Collect all settings from categories
            var exportData = new Dictionary<string, object?>
            {
                ["General"] = new
                {
                    GeneralSettings.Theme,
                    GeneralSettings.Language,
                    GeneralSettings.StartWithWindows
                },
                ["Database"] = new
                {
                    ConnectionString = FilterSensitiveValue(DatabaseSettings.ConnectionString, "***FILTERED***")
                },
                ["Visual"] = new
                {
                    ApiEndpoint = FilterSensitiveValue(VisualSettings.ApiEndpoint, "***FILTERED***")
                },
                ["Logging"] = new
                {
                    LoggingSettings.LogLevel
                },
                ["UI"] = new
                {
                    UiSettings.FontFamily
                },
                ["Cache"] = new
                {
                    CacheSettings.TtlSeconds
                },
                ["Performance"] = new
                {
                    PerformanceSettings.BootTargetMs
                },
                ["Developer"] = new
                {
                    DeveloperSettings.DebugMode
                },
                ["Metadata"] = new
                {
                    ExportedAt = DateTime.UtcNow,
                    Version = "1.0",
                    Note = "Sensitive values (passwords, tokens, keys) are filtered as ***FILTERED***"
                }
            };

            // Serialize to JSON
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(exportData, jsonOptions);

            // Save to file (default: Documents/MTM_Settings_Export.json)
            var defaultFileName = $"MTM_Settings_Export_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var defaultPath = Path.Combine(documentsPath, defaultFileName);

            await File.WriteAllTextAsync(defaultPath, json, cancellationToken);

            // TODO: Show success notification to user
            // For now, just log
            System.Diagnostics.Debug.WriteLine($"Settings exported to: {defaultPath}");
        }
        catch (Exception ex)
        {
            // TODO: Show error dialog to user
            System.Diagnostics.Debug.WriteLine($"Export failed: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ImportAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Open file picker dialog
            // For now, read from default location
            var defaultFileName = "MTM_Settings_Import.json";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var defaultPath = Path.Combine(documentsPath, defaultFileName);

            if (!File.Exists(defaultPath))
            {
                System.Diagnostics.Debug.WriteLine($"Import file not found: {defaultPath}");
                return;
            }

            var json = await File.ReadAllTextAsync(defaultPath, cancellationToken);

            // Deserialize JSON
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var importData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, jsonOptions);

            if (importData == null)
            {
                System.Diagnostics.Debug.WriteLine("Failed to deserialize import data");
                return;
            }

            // TODO: Conflict resolution - for now, just overwrite
            // Apply settings from each category
            if (importData.TryGetValue("General", out var generalElement))
            {
                var general = JsonSerializer.Deserialize<GeneralSettingsData>(generalElement.GetRawText(), jsonOptions);
                if (general != null)
                {
                    GeneralSettings.Theme = general.Theme;
                    GeneralSettings.Language = general.Language;
                    GeneralSettings.StartWithWindows = general.StartWithWindows;
                }
            }

            if (importData.TryGetValue("Database", out var databaseElement))
            {
                var database = JsonSerializer.Deserialize<DatabaseSettingsData>(databaseElement.GetRawText(), jsonOptions);
                if (database != null && !string.IsNullOrEmpty(database.ConnectionString) && !database.ConnectionString.Contains("***FILTERED***"))
                {
                    DatabaseSettings.ConnectionString = database.ConnectionString;
                }
            }

            if (importData.TryGetValue("Visual", out var visualElement))
            {
                var visual = JsonSerializer.Deserialize<VisualSettingsData>(visualElement.GetRawText(), jsonOptions);
                if (visual != null && !string.IsNullOrEmpty(visual.ApiEndpoint) && !visual.ApiEndpoint.Contains("***FILTERED***"))
                {
                    VisualSettings.ApiEndpoint = visual.ApiEndpoint;
                }
            }

            if (importData.TryGetValue("Logging", out var loggingElement))
            {
                var logging = JsonSerializer.Deserialize<LoggingSettingsData>(loggingElement.GetRawText(), jsonOptions);
                if (logging != null)
                {
                    LoggingSettings.LogLevel = logging.LogLevel;
                }
            }

            if (importData.TryGetValue("UI", out var uiElement))
            {
                var ui = JsonSerializer.Deserialize<UiSettingsData>(uiElement.GetRawText(), jsonOptions);
                if (ui != null)
                {
                    UiSettings.FontFamily = ui.FontFamily;
                }
            }

            if (importData.TryGetValue("Cache", out var cacheElement))
            {
                var cache = JsonSerializer.Deserialize<CacheSettingsData>(cacheElement.GetRawText(), jsonOptions);
                if (cache != null)
                {
                    CacheSettings.TtlSeconds = cache.TtlSeconds;
                }
            }

            if (importData.TryGetValue("Performance", out var performanceElement))
            {
                var performance = JsonSerializer.Deserialize<PerformanceSettingsData>(performanceElement.GetRawText(), jsonOptions);
                if (performance != null)
                {
                    PerformanceSettings.BootTargetMs = performance.BootTargetMs;
                }
            }

            if (importData.TryGetValue("Developer", out var developerElement))
            {
                var developer = JsonSerializer.Deserialize<DeveloperSettingsData>(developerElement.GetRawText(), jsonOptions);
                if (developer != null)
                {
                    DeveloperSettings.DebugMode = developer.DebugMode;
                }
            }

            HasUnsavedChanges = true;

            // TODO: Show success notification to user
            System.Diagnostics.Debug.WriteLine($"Settings imported from: {defaultPath}");
        }
        catch (Exception ex)
        {
            // TODO: Show error dialog to user
            System.Diagnostics.Debug.WriteLine($"Import failed: {ex.Message}");
        }
    }

    private string FilterSensitiveValue(string? value, string replacement)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        // Filter sensitive keywords
        var lower = value.ToLowerInvariant();
        if (lower.Contains("password") || lower.Contains("token") || lower.Contains("key") || lower.Contains("secret"))
        {
            return replacement;
        }

        return value;
    }

    // DTOs for Import deserialization
    private record GeneralSettingsData(string? Theme, string? Language, bool StartWithWindows);
    private record DatabaseSettingsData(string? ConnectionString);
    private record VisualSettingsData(string? ApiEndpoint);
    private record LoggingSettingsData(string? LogLevel);
    private record UiSettingsData(string? FontFamily);
    private record CacheSettingsData(int TtlSeconds);
    private record PerformanceSettingsData(int BootTargetMs);
    private record DeveloperSettingsData(bool DebugMode);
}
