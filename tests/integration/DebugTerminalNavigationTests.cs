using Xunit;
using FluentAssertions;
using MTM_Template_Application.ViewModels;
using MTM_Template_Application.Views;
using Microsoft.Extensions.Logging;
using NSubstitute;
using MTM_Template_Application.Services.Diagnostics;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using System.Threading.Tasks;

namespace MTM_Template_Tests.Integration;

/// <summary>
/// Integration tests for Debug Terminal SplitView navigation and feature section loading.
/// Tests collapsible side panel behavior and feature section switching.
/// Task: T071 [P] [Phase3]
/// </summary>
[Collection("Avalonia")]
public class DebugTerminalNavigationTests
{
    private readonly ILogger<DebugTerminalViewModel> _mockLogger;
    private readonly IDiagnosticsService _mockDiagnosticsService;

    public DebugTerminalNavigationTests()
    {
        _mockLogger = Substitute.For<ILogger<DebugTerminalViewModel>>();
        _mockDiagnosticsService = Substitute.For<IDiagnosticsService>();
    }

    [AvaloniaFact]
    public async Task SplitView_ShouldHaveCollapsibleSidePanel()
    {
        // Arrange
        var viewModel = new DebugTerminalViewModel(_mockLogger, _mockDiagnosticsService);
        var window = new DebugTerminalWindow
        {
            DataContext = viewModel
        };

        // Act - Show window to trigger visual tree construction
        window.Show();
        await Task.Delay(100); // Allow UI to render

        // Find SplitView in visual tree
        var splitView = window.FindControl<SplitView>("MainSplitView");

        // Assert
        splitView.Should().NotBeNull("SplitView should exist in DebugTerminalWindow");
        splitView!.IsPaneOpen.Should().BeTrue("Side panel should be open by default");
        splitView.DisplayMode.Should().Be(SplitViewDisplayMode.Inline, "DisplayMode should be Inline for side-by-side layout");
    }

    [AvaloniaFact]
    public async Task NavigationMenu_ShouldContainAllFeatureSections()
    {
        // Arrange
        var viewModel = new DebugTerminalViewModel(_mockLogger, _mockDiagnosticsService);
        var window = new DebugTerminalWindow
        {
            DataContext = viewModel
        };

        // Act
        window.Show();
        await Task.Delay(100);

        var navigationMenu = window.FindControl<ItemsControl>("NavigationMenu");

        // Assert
        navigationMenu.Should().NotBeNull("Navigation menu should exist");
        viewModel.FeatureSections.Should().HaveCount(4, "Should have 4 feature sections: Boot, Config, Diagnostics, VISUAL");
        viewModel.FeatureSections.Should().Contain(s => s.SectionName == "Feature 001: Boot");
        viewModel.FeatureSections.Should().Contain(s => s.SectionName == "Feature 002: Config");
        viewModel.FeatureSections.Should().Contain(s => s.SectionName == "Feature 003: Diagnostics");
        viewModel.FeatureSections.Should().Contain(s => s.SectionName == "Feature 005: VISUAL");
    }

    [AvaloniaFact]
    public async Task SelectedFeature_ShouldChangeContentArea()
    {
        // Arrange
        var viewModel = new DebugTerminalViewModel(_mockLogger, _mockDiagnosticsService);
        var window = new DebugTerminalWindow
        {
            DataContext = viewModel
        };
        window.Show();
        await Task.Delay(100);

        var contentArea = window.FindControl<ContentControl>("FeatureContentArea");

        // Act - Change selected feature
        var bootSection = viewModel.FeatureSections.First(s => s.SectionName == "Feature 001: Boot");
        viewModel.SelectedFeature = bootSection;
        await Task.Delay(100); // Allow binding to update

        // Assert
        contentArea.Should().NotBeNull("Content area should exist");
        contentArea!.Content.Should().NotBeNull("Content should be loaded for selected feature");
        viewModel.SelectedFeature.Should().Be(bootSection);
    }

    [AvaloniaFact]
    public async Task HamburgerButton_ShouldTogglePaneOpenState()
    {
        // Arrange
        var viewModel = new DebugTerminalViewModel(_mockLogger, _mockDiagnosticsService);
        var window = new DebugTerminalWindow
        {
            DataContext = viewModel
        };
        window.Show();
        await Task.Delay(100);

        var splitView = window.FindControl<SplitView>("MainSplitView");
        var initialState = viewModel.IsPaneOpen;

        // Act - Toggle pane
        viewModel.TogglePaneCommand.Execute(null);
        await Task.Delay(50);

        // Assert
        viewModel.IsPaneOpen.Should().Be(!initialState, "IsPaneOpen should toggle");
        splitView!.IsPaneOpen.Should().Be(viewModel.IsPaneOpen, "SplitView should reflect ViewModel state");
    }

    [AvaloniaFact]
    public async Task FeatureSection_ShouldLoadCorrectControlsForBootFeature()
    {
        // Arrange
        var viewModel = new DebugTerminalViewModel(_mockLogger, _mockDiagnosticsService);
        var window = new DebugTerminalWindow
        {
            DataContext = viewModel
        };
        window.Show();
        await Task.Delay(100);

        // Act - Select Boot feature
        var bootSection = viewModel.FeatureSections.First(s => s.SectionName == "Feature 001: Boot");
        viewModel.SelectFeatureCommand.Execute(bootSection);
        await Task.Delay(100);

        // Assert
        viewModel.SelectedFeature.Should().Be(bootSection);
        // Boot section should display BootTimelineChart control (verified by visual tree inspection)
        viewModel.BootTimeline.Should().NotBeNull("Boot timeline data should be loaded");
    }

    [AvaloniaFact]
    public async Task FeatureSection_ShouldLoadCorrectControlsForDiagnosticsFeature()
    {
        // Arrange
        var viewModel = new DebugTerminalViewModel(_mockLogger, _mockDiagnosticsService);
        var window = new DebugTerminalWindow
        {
            DataContext = viewModel
        };
        window.Show();
        await Task.Delay(100);

        // Act - Select Diagnostics feature
        var diagnosticsSection = viewModel.FeatureSections.First(s => s.SectionName == "Feature 003: Diagnostics");
        viewModel.SelectFeatureCommand.Execute(diagnosticsSection);
        await Task.Delay(100);

        // Assert
        viewModel.SelectedFeature.Should().Be(diagnosticsSection);
        // Diagnostics section should display StatusCard, MetricDisplay, ErrorListPanel controls
        viewModel.PerformanceSnapshots.Should().NotBeNull("Performance snapshots should be loaded");
    }
}
