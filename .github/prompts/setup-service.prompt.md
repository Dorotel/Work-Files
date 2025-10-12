---
description: 'Generate a new service with interface, DI registration, and logging'
---

# Setup Service

Generate a complete service class with interface following MTM architectural patterns with dependency injection and comprehensive logging.

## Prerequisites

- Service name must be specified (ends with "Service")
- Service purpose must be defined
- Dependencies must be identified

## User Input

```text
$ARGUMENTS
```

Parse arguments to extract:
- Service name (e.g., `InventoryService`)
- Service purpose (brief description)
- Dependencies (e.g., `DatabaseService`, other services)
- Async operations needed (yes/no)

If arguments are incomplete, prompt for:
1. Service name
2. Service purpose/responsibility
3. Required dependencies
4. Key methods to implement

## Implementation Steps

### Step 1: Create Service Interface

Create file at `Services/I{ServiceName}.cs`:

```csharp
namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Interface for {service purpose}.
/// </summary>
public interface I{ServiceName}
{
    /// <summary>
    /// {Method description}.
    /// </summary>
    Task<ServiceResult> MethodNameAsync(parameters);
}
```

### Step 2: Create Service Implementation

Create file at `Services/{ServiceName}.cs`:

1. **File header and namespace**:
   - File-scoped namespace
   - Required using statements

2. **Class declaration**:
   ```csharp
   public class {ServiceName} : I{ServiceName}
   ```

3. **Dependency injection constructor**:
   - Inject `ILogger<{ServiceName}>`
   - Inject required dependencies
   - Validate all parameters with `ArgumentNullException.ThrowIfNull()`
   - Store as `private readonly` fields

4. **Method implementations**:
   - Use async/await for I/O operations
   - Comprehensive logging (entry, exit, errors)
   - Try-catch with proper error handling
   - Return `ServiceResult` or `ServiceResult<T>` for operation status

### Step 3: Add Logging

Implement structured logging at key points:

```csharp
public async Task<ServiceResult> SaveDataAsync(Data data)
{
    _logger.LogInformation("Saving data for {DataId}", data.Id);
    
    try
    {
        // Implementation
        
        _logger.LogInformation("Successfully saved data for {DataId}", data.Id);
        return ServiceResult.Success();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to save data for {DataId}", data.Id);
        return ServiceResult.Failure($"Failed to save data: {ex.Message}");
    }
}
```

### Step 4: Add Error Handling

Implement comprehensive error handling:

- Validate input parameters
- Catch specific exceptions when possible
- Log errors with context
- Return meaningful error messages
- Use ServiceResult pattern for operation status

### Step 5: Register Service in DI

Add registration to `Extensions/ServiceCollectionExtensions.cs`:

```csharp
// In ConfigureServices method
services.AddTransient<I{ServiceName}, {ServiceName}>();
```

Or for singleton services:
```csharp
services.AddSingleton<I{ServiceName}, {ServiceName}>();
```

## Service Lifetime Guidelines

- **Transient**: Stateless services, created per request (most services)
- **Scoped**: Per-request lifetime (not typically used in desktop apps)
- **Singleton**: Shared state across application lifetime (configuration, caching)

## Common Service Patterns

### Database Service Pattern

```csharp
public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    private readonly DatabaseService _databaseService;
    private readonly string _connectionString;

    public InventoryService(
        ILogger<InventoryService> logger,
        DatabaseService databaseService,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(databaseService);
        ArgumentNullException.ThrowIfNull(configuration);

        _logger = logger;
        _databaseService = databaseService;
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not configured");
    }

    public async Task<ServiceResult<List<InventoryItem>>> GetInventoryAsync(string locationCode)
    {
        _logger.LogInformation("Getting inventory for location {LocationCode}", locationCode);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "LocationCode", locationCode }
            };

            var (result, status, error) = Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "usp_GetInventory",
                parameters,
                30
            );

            if (status != "SUCCESS")
            {
                _logger.LogError("Database operation failed: {Error}", error);
                return ServiceResult<List<InventoryItem>>.Failure(error ?? "Unknown database error");
            }

            var items = ConvertDataTableToList(result);
            _logger.LogInformation("Retrieved {Count} inventory items", items.Count);
            
            return ServiceResult<List<InventoryItem>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get inventory for location {LocationCode}", locationCode);
            return ServiceResult<List<InventoryItem>>.Failure($"Failed to retrieve inventory: {ex.Message}");
        }
    }
}
```

### Business Logic Service Pattern

```csharp
public class ValidationService : IValidationService
{
    private readonly ILogger<ValidationService> _logger;

    public ValidationService(ILogger<ValidationService> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    public ValidationResult ValidatePartNumber(string partNumber)
    {
        _logger.LogDebug("Validating part number: {PartNumber}", partNumber);

        if (string.IsNullOrWhiteSpace(partNumber))
        {
            return ValidationResult.Failure("Part number is required");
        }

        if (partNumber.Length > 50)
        {
            return ValidationResult.Failure("Part number exceeds maximum length");
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(partNumber, @"^[A-Z0-9-]+$"))
        {
            return ValidationResult.Failure("Part number contains invalid characters");
        }

        _logger.LogDebug("Part number validation successful");
        return ValidationResult.Success();
    }
}
```

### Configuration Service Pattern

```csharp
public class ConfigurationService : IConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;
    private readonly IConfiguration _configuration;

    public ConfigurationService(
        ILogger<ConfigurationService> logger,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configuration);

        _logger = logger;
        _configuration = configuration;
    }

    public string GetConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not configured");
    }

    public T GetSetting<T>(string key, T defaultValue = default!)
    {
        var value = _configuration[key];
        
        if (string.IsNullOrEmpty(value))
        {
            _logger.LogWarning("Configuration key {Key} not found, using default value", key);
            return defaultValue;
        }

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert configuration value for {Key}", key);
            return defaultValue;
        }
    }
}
```

## Validation Checklist

Before completion, verify:

- [ ] Interface created in `Services/` directory
- [ ] Implementation class created in `Services/` directory
- [ ] Class implements interface
- [ ] Constructor uses dependency injection
- [ ] All constructor parameters validated with `ArgumentNullException.ThrowIfNull()`
- [ ] ILogger injected and used throughout
- [ ] All I/O operations are async
- [ ] Comprehensive error handling with try-catch
- [ ] Structured logging with contextual information
- [ ] ServiceResult pattern used for operation status
- [ ] Service registered in DI container
- [ ] XML documentation comments on interface and public methods
- [ ] No hardcoded configuration values

## Anti-Patterns to Avoid

❌ **Do NOT**:
- Hardcode connection strings or configuration values
- Use synchronous I/O operations (use async/await)
- Swallow exceptions without logging
- Return null instead of ServiceResult.Failure()
- Put UI logic in services
- Create services without interfaces
- Forget to register service in DI

## Success Criteria

✅ **Success** when:
- Service compiles without errors
- Interface and implementation follow naming conventions
- Dependency injection configured correctly
- Comprehensive logging implemented
- Error handling covers all failure scenarios
- Service registered in DI container
- Ready for consumption by ViewModels

## Example Output

**IInventoryService.cs**:
```csharp
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Interface for inventory management operations.
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// Gets inventory items for the specified location.
    /// </summary>
    Task<ServiceResult<List<InventoryItem>>> GetInventoryAsync(string locationCode);
}
```

**InventoryService.cs**:
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Services;

public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    private readonly DatabaseService _databaseService;
    private readonly string _connectionString;

    public InventoryService(
        ILogger<InventoryService> logger,
        DatabaseService databaseService,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(databaseService);
        ArgumentNullException.ThrowIfNull(configuration);

        _logger = logger;
        _databaseService = databaseService;
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not configured");
    }

    public async Task<ServiceResult<List<InventoryItem>>> GetInventoryAsync(string locationCode)
    {
        _logger.LogInformation("Getting inventory for location {LocationCode}", locationCode);

        try
        {
            // Implementation
            _logger.LogInformation("Successfully retrieved inventory");
            return ServiceResult<List<InventoryItem>>.Success(new List<InventoryItem>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get inventory");
            return ServiceResult<List<InventoryItem>>.Failure(ex.Message);
        }
    }
}
```

## Next Steps

After creating the service:
1. Verify service is registered in `ServiceCollectionExtensions.cs`
2. Create unit tests for service methods (if applicable)
3. Inject service into ViewModels as needed
4. Test service through ViewModels in running application
5. Monitor logs to ensure proper logging coverage
