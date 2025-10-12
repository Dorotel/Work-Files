using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Controls;
using Xunit;

namespace MTM_Template_Tests.unit.Controls;

[Collection("AvaloniaTests")]
public class SettingRowTests
{
    [AvaloniaFact]
    public void SettingRow_DefaultValues_ShouldBeInitialized()
    {
        var control = new SettingRow();
        control.SettingKey.Should().Be(string.Empty);
        control.SettingValue.Should().Be(string.Empty);
        control.IsValid.Should().BeTrue();
        control.ValidationMessage.Should().Be(string.Empty);
    }

    [AvaloniaFact]
    public void SettingRow_SetProperties_ShouldUpdate()
    {
        var control = new SettingRow
        {
            SettingKey = "UI:Theme",
            SettingValue = "Dark",
            IsValid = false,
            ValidationMessage = "Invalid theme"
        };

        control.SettingKey.Should().Be("UI:Theme");
        control.SettingValue.Should().Be("Dark");
        control.IsValid.Should().BeFalse();
        control.ValidationMessage.Should().Be("Invalid theme");
    }
}
