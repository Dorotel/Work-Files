using Avalonia.Controls;
using MTM_Template_Application.ViewModels.Settings;

namespace MTM_Template_Application.Views.Settings;

/// <summary>
/// Settings window with tabbed navigation for 8 configuration categories.
/// Phase 2 - T055
/// </summary>
public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor with ViewModel injection for DI.
    /// </summary>
    public SettingsWindow(SettingsViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
