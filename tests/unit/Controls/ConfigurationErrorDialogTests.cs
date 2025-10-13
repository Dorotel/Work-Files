using Avalonia.Headless.XUnit;
using FluentAssertions;
using MTM_Template_Application.Controls;
using System.Collections.Generic;
using Xunit;

namespace MTM_Template_Tests.unit.Controls;

[Collection("AvaloniaTests")]
public class ConfigurationErrorDialogTests
{
    [AvaloniaFact]
    public void ConfigurationErrorDialog_DefaultValues_ShouldBeInitialized()
    {
        var control = new ConfigurationErrorDialog();
        control.ErrorMessage.Should().Be(string.Empty);
        control.RecoveryOptions.Should().BeNull();
    }

    [AvaloniaFact]
    public void ConfigurationErrorDialog_SetProperties_ShouldUpdate()
    {
        var control = new ConfigurationErrorDialog();
        var options = new List<object> { "Retry", "Exit" };

        control.ErrorMessage = "Database connection failed";
        control.RecoveryOptions = options;

        control.ErrorMessage.Should().Be("Database connection failed");
        control.RecoveryOptions.Should().BeSameAs(options);
    }
}
