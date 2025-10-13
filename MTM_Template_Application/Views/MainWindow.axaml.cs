using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_Template_Application.Controls;
using MTM_Template_Application.ViewModels;

namespace MTM_Template_Application.Views;

public partial class MainWindow : Window
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly ILogger<MainWindow>? _logger;

    /// <summary>
    /// Parameterless constructor for design-time only
    /// </summary>
    public MainWindow() : this(null!, null!)
    {
    }

    public MainWindow(IServiceProvider? serviceProvider, ILogger<MainWindow>? logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        InitializeComponent();
    }

    private void TitleBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }

    private void MinimizeWindow(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeWindow(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void CloseWindow(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Status bar clicked - show error details dialog
    /// </summary>
    private void StatusBar_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not MainViewModel viewModel ||
            viewModel.ActiveErrors == null ||
            !viewModel.ActiveErrors.Any() ||
            _serviceProvider == null)
        {
            return;
        }

        try
        {
            // Get first error for dialog display
            var firstError = viewModel.ActiveErrors.First();
            
            // Resolve ViewModel from service provider
            var errorViewModel = _serviceProvider.GetRequiredService<ConfigurationErrorDialogViewModel>();
            
            // Map error properties (using correct property names from ConfigurationError model)
            errorViewModel.ErrorMessage = firstError.Message;
            
            // Determine category from error Key (simple mapping for now)
            errorViewModel.ErrorCategory = DetermineCategory(firstError.Key);
            errorViewModel.AffectedSettingKey = firstError.Key;
            errorViewModel.Timestamp = firstError.Timestamp.DateTime; // Convert DateTimeOffset to DateTime
            
            // Create dialog control and set DataContext
            var dialog = new ConfigurationErrorDialog
            {
                DataContext = errorViewModel
            };
            
            // TODO: For now, just instantiate the control - in future, add modal overlay to MainWindow.axaml
            // to display this control as a modal dialog. This demonstrates the integration pattern for T092.
            // The commands (EditSettings, Retry, Exit) are already wired in the ViewModel.
            
            _logger?.LogInformation(
                "Configuration error dialog prepared for error: {Message}, Category: {Category}",
                errorViewModel.ErrorMessage,
                errorViewModel.ErrorCategory);
        }
        catch (Exception ex)
        {
            // Log error but don't crash - worst case, error dialog doesn't show
            System.Diagnostics.Debug.WriteLine($"Error showing configuration error dialog: {ex.Message}");
        }
    }

    /// <summary>
    /// Maps configuration key to error category for Settings navigation
    /// </summary>
    private static string DetermineCategory(string key)
    {
        // Simple category mapping based on key prefix
        if (key.StartsWith("Database", StringComparison.OrdinalIgnoreCase))
        {
            return "Database";
        }
        
        if (key.StartsWith("Visual", StringComparison.OrdinalIgnoreCase))
        {
            return "Visual";
        }
        
        if (key.StartsWith("Performance", StringComparison.OrdinalIgnoreCase))
        {
            return "Performance";
        }
        
        return "General"; // Default category
    }
}
