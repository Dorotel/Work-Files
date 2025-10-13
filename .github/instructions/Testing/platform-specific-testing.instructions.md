---
description: 'Platform-specific xUnit test patterns using custom Fact attributes for cross-platform test execution'
applyTo: '**/tests/**/*.cs'
---

# Platform-Specific xUnit Testing Patterns

## Overview

This instruction file defines patterns for creating and using platform-specific xUnit test attributes that enable tests to run only on specific operating systems. This is essential for testing platform-specific APIs like Windows DPAPI, Android KeyStore, or iOS Keychain.

## Core Principles

### Why Platform-Specific Tests Are Needed

1. **Platform-Specific APIs**: Some services use OS-native APIs that only exist on specific platforms
   - Windows: DPAPI (Data Protection API), Credential Manager
   - Android: KeyStore with hardware-backed encryption
   - iOS: Keychain Services
   - Linux: Secret Service API

2. **Cross-Platform Development**: Developers work on different platforms but code must work on all
3. **CI/CD Flexibility**: Different test agents can run appropriate tests without failures
4. **Clear Communication**: Skip messages explain why tests don't run on certain platforms

### When to Use Platform-Specific Tests

✅ **Use platform-specific test attributes when**:
- Testing services that use OS-native credential storage
- Testing file system operations that behave differently by platform
- Testing platform-specific UI behaviors
- Testing hardware-specific functionality (camera, sensors on mobile)

❌ **Don't use platform-specific tests when**:
- Logic is platform-agnostic (use regular `[Fact]` or `[Theory]`)
- You can mock the platform dependency instead
- The code already has platform abstraction layers

## Pattern 1: Creating Custom Platform Fact Attributes

### Windows-Only Tests

**File Location**: `tests/TestHelpers/WindowsOnlyFactAttribute.cs`

```csharp
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
```

### Android-Only Tests (Template)

**File Location**: `tests/TestHelpers/AndroidOnlyFactAttribute.cs`

```csharp
using System.Runtime.InteropServices;
using Xunit;

namespace MTM_Template_Tests.TestHelpers;

/// <summary>
/// Custom xUnit Fact attribute that only runs tests on Android platform.
/// Skips tests on non-Android platforms with a clear message.
/// </summary>
public sealed class AndroidOnlyFactAttribute : FactAttribute
{
    public AndroidOnlyFactAttribute()
    {
        // Note: Android detection requires additional checks beyond RuntimeInformation
        // Check for Android-specific environment or use OperatingSystem.IsAndroid() (.NET 6+)
        if (!IsAndroid())
        {
            Skip = "Test requires Android platform (uses Android KeyStore APIs)";
        }
    }

    private static bool IsAndroid()
    {
        // .NET 6+ has OperatingSystem.IsAndroid()
        return OperatingSystem.IsAndroid();
    }
}
```

### iOS-Only Tests (Template)

```csharp
public sealed class iOSOnlyFactAttribute : FactAttribute
{
    public iOSOnlyFactAttribute()
    {
        if (!OperatingSystem.IsIOS())
        {
            Skip = "Test requires iOS platform (uses iOS Keychain Services APIs)";
        }
    }
}
```

### Linux-Only Tests (Template)

```csharp
public sealed class LinuxOnlyFactAttribute : FactAttribute
{
    public LinuxOnlyFactAttribute()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Skip = "Test requires Linux platform (uses Secret Service API)";
        }
    }
}
```

## Pattern 2: Using Platform-Specific Attributes with CA1416 Warnings

### The Problem: Code Analyzer Warnings

When using platform-specific APIs, C# analyzer CA1416 warns:
```
CA1416: This call site is reachable on all platforms. 'WindowsSecretsService' is only supported on: 'windows'.
```

### The Solution: Dual Protection

**Attribute + Platform Guard Pattern**:

```csharp
[WindowsOnlyFact]
[Trait("Category", "Contract")]
public async Task SecretsService_NeverLogsCredentialValues()
{
    // Platform guard - eliminates CA1416 analyzer warnings
    if (!OperatingSystem.IsWindows())
    {
        return;
    }

    // Arrange
    var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(_serilogLogger));
    var logger = loggerFactory.CreateLogger<WindowsSecretsService>();
    var service = new WindowsSecretsService(logger); // No warning!
    
    // Act - Platform-specific operations
    await service.StoreSecretAsync(testKey, testValue, CancellationToken.None);
    var retrievedValue = await service.RetrieveSecretAsync(testKey, CancellationToken.None);
    
    // Assert
    retrievedValue.Should().Be(testValue);
    
    // Cleanup
    await service.DeleteSecretAsync(testKey, CancellationToken.None);
}
```

**Why Both Are Needed**:
1. **Attribute (`[WindowsOnlyFact]`)**: xUnit framework skips test on wrong platform at runtime
2. **Platform Guard (`if (!OperatingSystem.IsWindows())`)**: C# analyzer understands code won't execute on wrong platform, eliminates warnings

## Pattern 3: Platform-Specific Theory Tests

For parameterized tests that are platform-specific:

```csharp
/// <summary>
/// Custom xUnit Theory attribute for Windows-only parameterized tests.
/// </summary>
public sealed class WindowsOnlyTheoryAttribute : TheoryAttribute
{
    public WindowsOnlyTheoryAttribute()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Skip = "Test requires Windows platform";
        }
    }
}
```

**Usage**:
```csharp
[WindowsOnlyTheory]
[InlineData("password")]
[InlineData("token")]
[InlineData("secret")]
public async Task WindowsSecrets_RedactsSensitiveKeys(string sensitiveKey)
{
    if (!OperatingSystem.IsWindows())
    {
        return;
    }
    
    var service = new WindowsSecretsService(logger);
    await service.StoreSecretAsync($"Test:{sensitiveKey}", "value", CancellationToken.None);
    // Test implementation
}
```

## Pattern 4: Multi-Platform Test Organization

### File Organization Strategy

```
tests/
├── TestHelpers/
│   ├── WindowsOnlyFactAttribute.cs
│   ├── AndroidOnlyFactAttribute.cs
│   ├── iOSOnlyFactAttribute.cs
│   ├── LinuxOnlyFactAttribute.cs
│   └── PlatformTestBase.cs
├── contract/
│   ├── WindowsSecretsServiceContractTests.cs
│   ├── AndroidSecretsServiceContractTests.cs
│   └── CrossPlatformSecretsContractTests.cs
└── unit/
    └── Services/
        └── Secrets/
            ├── WindowsSecretsServiceTests.cs
            ├── AndroidSecretsServiceTests.cs
            └── SecretsServiceFactoryTests.cs (cross-platform)
```

### Naming Convention

- **Platform-Specific Test Files**: `[Platform][ServiceName]Tests.cs`
  - `WindowsSecretsServiceTests.cs`
  - `AndroidSecretsServiceTests.cs`
  
- **Cross-Platform Test Files**: `[ServiceName]Tests.cs`
  - `SecretsServiceFactoryTests.cs` (tests factory pattern)
  - `ConfigurationServiceTests.cs` (platform-agnostic)

## Pattern 5: CI/CD Integration

### GitHub Actions Example

```yaml
name: Test Suite

jobs:
  test-windows:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - name: Run Windows-specific tests
        run: dotnet test --filter "Category=WindowsOnly"
  
  test-linux:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Run Linux-specific tests
        run: dotnet test --filter "Category=LinuxOnly"
  
  test-macos:
    runs-on: macos-latest
    steps:
      - uses: actions/checkout@v3
      - name: Run macOS-specific tests
        run: dotnet test --filter "Category=MacOSOnly"
  
  test-all:
    runs-on: ubuntu-latest
    steps:
      - name: Run all platform-agnostic tests
        run: dotnet test --filter "Category!=WindowsOnly&Category!=LinuxOnly&Category!=MacOSOnly"
```

### Test Filtering by Platform

```powershell
# Run only Windows-specific tests
dotnet test --filter "FullyQualifiedName~Windows"

# Run all tests except platform-specific
dotnet test --filter "FullyQualifiedName!~Windows&FullyQualifiedName!~Android&FullyQualifiedName!~iOS"

# Run tests suitable for current platform (auto-detects via attributes)
dotnet test  # WindowsOnlyFact tests auto-skip on Linux/macOS
```

## Testing Best Practices

### 1. Clear Skip Messages

Always provide informative skip messages:

```csharp
// ✅ Good: Explains why and what APIs are needed
Skip = "Test requires Windows platform (uses Windows-specific APIs like DPAPI/Credential Manager)";

// ❌ Bad: Not informative
Skip = "Windows only";
```

### 2. Platform Detection Methods

Use appropriate detection for each platform:

```csharp
// .NET 5+ preferred methods
OperatingSystem.IsWindows()
OperatingSystem.IsLinux()
OperatingSystem.IsMacOS()
OperatingSystem.IsAndroid()
OperatingSystem.IsIOS()

// RuntimeInformation (cross-.NET support)
RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
```

### 3. Trait Categories

Add traits for test organization:

```csharp
[WindowsOnlyFact]
[Trait("Category", "Platform")]
[Trait("Category", "Windows")]
[Trait("Category", "Integration")]
public async Task WindowsSecrets_StoreAndRetrieve()
{
    // Test implementation
}
```

### 4. Documentation

Document platform requirements in test comments:

```csharp
/// <summary>
/// Tests WindowsSecretsService credential storage using Windows DPAPI.
/// </summary>
/// <remarks>
/// Platform Requirements:
/// - Windows 7+ (DPAPI availability)
/// - User must have profile loaded (credentials stored per-user)
/// - No admin privileges required
/// </remarks>
[WindowsOnlyFact]
public async Task WindowsSecrets_StoreAndRetrieve()
{
    // Test implementation
}
```

## Troubleshooting

### Issue: CA1416 Warnings Still Appear

**Symptom**: Despite using `[WindowsOnlyFact]`, CA1416 warnings remain

**Solution**: Add platform guard at method start:
```csharp
if (!OperatingSystem.IsWindows())
{
    return;
}
```

**Why**: C# analyzer checks method body regardless of xUnit attribute logic

---

### Issue: Tests Don't Skip on CI/CD

**Symptom**: Tests fail on wrong platform instead of skipping

**Solution 1**: Verify attribute is applied to test method:
```csharp
[WindowsOnlyFact]  // ✅ Applied correctly
public async Task MyTest() { }
```

**Solution 2**: Check xUnit version (requires xUnit 2.0+):
```xml
<PackageReference Include="xunit" Version="2.8.2" />
```

---

### Issue: Platform Detection Incorrect

**Symptom**: `OperatingSystem.IsWindows()` returns false on Windows

**Solution**: Ensure using .NET 5+ for `OperatingSystem` class. For older frameworks:
```csharp
RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
```

---

## Real-World Examples

### Example 1: Windows Secrets Service Test

```csharp
[WindowsOnlyFact]
[Trait("Category", "Contract")]
public async Task SecretsService_NeverLogsCredentialValues()
{
    if (!OperatingSystem.IsWindows())
    {
        return;
    }

    var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(_serilogLogger));
    var logger = loggerFactory.CreateLogger<WindowsSecretsService>();
    var service = new WindowsSecretsService(logger);
    var testKey = "Test.CredentialForLogging";
    var testValue = "VerySecretPassword123!@#";

    await service.StoreSecretAsync(testKey, testValue, CancellationToken.None);
    var retrievedValue = await service.RetrieveSecretAsync(testKey, CancellationToken.None);

    var logMessages = _logSink.LogEvents.Select(e => e.RenderMessage()).ToList();
    var containsCredential = logMessages.Any(msg => msg.Contains(testValue));
    containsCredential.Should().BeFalse("Credential values should NEVER appear in logs");

    await service.DeleteSecretAsync(testKey, CancellationToken.None);
}
```

### Example 2: Android KeyStore Test (Future)

```csharp
[AndroidOnlyFact]
[Trait("Category", "Contract")]
public async Task AndroidSecrets_UsesHardwareBackedEncryption()
{
    if (!OperatingSystem.IsAndroid())
    {
        return;
    }

    var logger = _loggerFactory.CreateLogger<AndroidSecretsService>();
    var service = new AndroidSecretsService(logger);
    var testKey = "Test.AndroidCredential";
    var testValue = "AndroidSecretValue456";

    await service.StoreSecretAsync(testKey, testValue, CancellationToken.None);
    
    // Verify hardware-backed encryption is used
    var isHardwareBacked = service.IsHardwareBackedEncryptionAvailable();
    isHardwareBacked.Should().BeTrue("Android should use hardware-backed encryption when available");
    
    var retrievedValue = await service.RetrieveSecretAsync(testKey, CancellationToken.None);
    retrievedValue.Should().Be(testValue);

    await service.DeleteSecretAsync(testKey, CancellationToken.None);
}
```

## Memory File Maintenance

**Last Updated**: October 12, 2025  
**Maintainer**: GitHub Copilot (via user input)

**How to Add Platform Patterns**:
1. Create new platform attribute file in `tests/TestHelpers/`
2. Follow naming convention: `[Platform]OnlyFactAttribute.cs`
3. Implement platform detection logic
4. Add clear skip message
5. Document in this instruction file

**Review Frequency**: When adding new platform support (Android, iOS, Linux, etc.)
