using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Controls;
using Xunit;

namespace MTM_Template_Tests.unit.Controls;

[Collection("AvaloniaTests")]
public class NavigationMenuItemTests
{
    [AvaloniaFact]
    public void NavigationMenuItem_DefaultValues_ShouldBeInitialized()
    {
        var control = new NavigationMenuItem();
        control.MenuText.Should().Be(string.Empty);
        control.Icon.Should().Be("📄");
        control.IsSelected.Should().BeFalse();
        control.Command.Should().BeNull();
    }

    [AvaloniaFact]
    public void NavigationMenuItem_SetProperties_ShouldUpdate()
    {
        var control = new NavigationMenuItem
        {
            MenuText = "Feature 001: Boot",
            Icon = "🚀",
            IsSelected = true
        };

        control.MenuText.Should().Be("Feature 001: Boot");
        control.Icon.Should().Be("🚀");
        control.IsSelected.Should().BeTrue();
    }
}
