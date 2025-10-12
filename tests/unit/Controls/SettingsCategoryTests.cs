using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Controls;
using System.Collections.ObjectModel;
using Xunit;

namespace MTM_Template_Tests.unit.Controls;

[Collection("AvaloniaTests")]
public class SettingsCategoryTests
{
    [AvaloniaFact]
    public void SettingsCategory_DefaultValues_ShouldBeInitialized()
    {
        var control = new SettingsCategory();
        control.CategoryName.Should().Be(string.Empty);
        control.Icon.Should().Be("⚙️");
        control.Children.Should().NotBeNull();
        control.IsExpanded.Should().BeTrue();
    }

    [AvaloniaFact]
    public void SettingsCategory_SetProperties_ShouldUpdate()
    {
        var control = new SettingsCategory();
        var children = new ObservableCollection<object> { "Item1", "Item2" };

        control.CategoryName = "General";
        control.Icon = "🏠";
        control.Children = children;
        control.IsExpanded = false;

        control.CategoryName.Should().Be("General");
        control.Icon.Should().Be("🏠");
        control.Children.Should().BeSameAs(children);
        control.IsExpanded.Should().BeFalse();
    }
}
