using System.Runtime.InteropServices;
using Xunit;

namespace MTM_Template_Tests.TestHelpers;

/// <summary>
/// Custom xUnit Fact attribute that only runs tests on Windows platform.
/// Skips tests on non-Windows platforms with a clear message.
/// </summary>
/// <remarks>
/// Use this attribute for tests that use Windows-specific APIs like WindowsSecretsService
/// which depend on DPAPI (Data Protection API) or Credential Manager.
/// 
/// Example:
/// <code>
/// [WindowsOnlyFact]
/// public async Task MyTest_UsesWindowsSecrets()
/// {
///     var secretsService = new WindowsSecretsService(loggerFactory);
///     await secretsService.StoreSecretAsync("key", "value", cancellationToken);
/// }
/// </code>
/// </remarks>
public sealed class WindowsOnlyFactAttribute : FactAttribute
{
    public WindowsOnlyFactAttribute()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Skip = "Test requires Windows platform (uses Windows-specific APIs like DPAPI/Credential Manager)";
        }
    }
}
