using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Controls;
using System.Collections.ObjectModel;
using Xunit;

namespace MTM_Template_Tests.unit.Controls;

[Collection("AvaloniaTests")]
public class ActionButtonGroupTests
{
    [AvaloniaFact]
    public void ActionButtonGroup_DefaultValue_ShouldBeEmptyCollection()
    {
        var control = new ActionButtonGroup();
        control.Buttons.Should().NotBeNull();
        control.Buttons.Should().BeEmpty();
    }

    [AvaloniaFact]
    public void ActionButtonGroup_SetButtons_ShouldUpdate()
    {
        var control = new ActionButtonGroup();
        var buttons = new ObservableCollection<object> { "Save", "Cancel", "Reset" };

        control.Buttons = buttons;

        control.Buttons.Should().BeSameAs(buttons);
        control.Buttons.Should().HaveCount(3);
    }
}
