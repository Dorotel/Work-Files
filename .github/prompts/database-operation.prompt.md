---
description: 'Generate database operation code using stored procedures with MTM patterns'
---

# Database Operation

Generate database operation code using stored procedures following MTM MySQL 5.7 patterns with Helper_Database_StoredProcedure and connection pooling.

## Prerequisites

- Stored procedure name must be known
- Connection string configured in appsettings.json
- MySql.Data 9.4.0 connector available
- Understanding of manufacturing domain context

## User Input

```text
$ARGUMENTS
```

Parse arguments to extract:
- Operation name (e.g., `GetInventory`, `SaveTransaction`)
- Stored procedure name (e.g., `usp_GetInventory`)
- Parameters needed
- Return type (DataTable, single row, scalar, none)
- Service context (which service should contain this operation)

If arguments are incomplete, prompt for:
1. What data operation? (Get, Save, Update, Delete)
2. Stored procedure name
3. Input parameters
4. Expected return data
5. Which service class?

## Implementation Steps

### Step 1: Identify Stored Procedure Pattern

Common MTM stored procedures:
- **usp_Get{Entity}**: Retrieve data (returns DataTable)
- **usp_Save{Entity}**: Insert/Update data (returns status)
- **usp_Delete{Entity}**: Delete data (returns status)
- **usp_{Entity}Transaction**: Manufacturing transaction (returns status)

### Step 2: Create Service Method

Add method to appropriate service class:

```csharp
public async Task<ServiceResult<List<{Entity}>>> Get{Entity}Async(parameters)
{
    _logger.LogInformation("Getting {Entity} with {Parameters}", parameters);
    
    try
    {
        var parameters = new Dictionary<string, object>
        {
            { "ParameterName", parameterValue }
        };

        var (result, status, error) = Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usp_StoredProcedureName",
            parameters,
            30
        );

        if (status != "SUCCESS")
        {
            _logger.LogError("Database operation failed: {Error}", error);
            return ServiceResult<List<{Entity}>>.Failure(error ?? "Unknown database error");
        }

        var entities = ConvertDataTableToList(result);
        _logger.LogInformation("Retrieved {Count} {Entity} records", entities.Count);
        
        return ServiceResult<List<{Entity}>>.Success(entities);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to get {Entity}");
        return ServiceResult<List<{Entity}>>.Failure($"Database operation failed: {ex.Message}");
    }
}
```

### Step 3: Add DataTable Conversion

Create helper method to convert DataTable to entity list:

```csharp
private List<{Entity}> ConvertDataTableToList(DataTable dataTable)
{
    var entities = new List<{Entity}>();
    
    foreach (DataRow row in dataTable.Rows)
    {
        var entity = new {Entity}
        {
            Property1 = row["ColumnName1"]?.ToString() ?? string.Empty,
            Property2 = Convert.ToInt32(row["ColumnName2"]),
            Property3 = row["ColumnName3"] as DateTime? ?? DateTime.MinValue
        };
        
        entities.Add(entity);
    }
    
    return entities;
}
```

### Step 4: Add Parameter Validation

Validate parameters before database call:

```csharp
if (string.IsNullOrWhiteSpace(locationCode))
{
    _logger.LogWarning("Invalid location code provided");
    return ServiceResult<List<Inventory>>.Failure("Location code is required");
}

if (!ValidLocations.Contains(locationCode))
{
    _logger.LogWarning("Invalid location code: {LocationCode}", locationCode);
    return ServiceResult<List<Inventory>>.Failure($"Invalid location code: {locationCode}");
}
```

### Step 5: Add Manufacturing Domain Validation

For manufacturing operations, validate against MTM configuration:

```csharp
// Validate operation number (work order sequence step)
var validOperations = _configuration.GetSection("MTM:ValidOperations")
    .Get<List<string>>() ?? new List<string>();

if (!validOperations.Contains(operationNumber))
{
    _logger.LogWarning("Invalid operation number: {OperationNumber}", operationNumber);
    return ServiceResult.Failure($"Invalid operation: {operationNumber}");
}

// Validate location code
var defaultLocations = _configuration.GetSection("MTM:DefaultLocations")
    .Get<List<string>>() ?? new List<string>();

if (!defaultLocations.Contains(locationCode))
{
    _logger.LogWarning("Invalid location code: {LocationCode}", locationCode);
    return ServiceResult.Failure($"Invalid location: {locationCode}");
}
```

## Common Database Operation Patterns

### Get List Pattern

```csharp
public async Task<ServiceResult<List<InventoryItem>>> GetInventoryByLocationAsync(string locationCode)
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
            "usp_GetInventoryByLocation",
            parameters,
            30
        );

        if (status != "SUCCESS")
        {
            _logger.LogError("Failed to get inventory: {Error}", error);
            return ServiceResult<List<InventoryItem>>.Failure(error ?? "Database error");
        }

        var items = new List<InventoryItem>();
        foreach (DataRow row in result.Rows)
        {
            items.Add(new InventoryItem
            {
                PartID = row["PartID"]?.ToString() ?? string.Empty,
                LocationCode = row["LocationCode"]?.ToString() ?? string.Empty,
                Quantity = Convert.ToInt32(row["Quantity"]),
                LastUpdated = row["LastUpdated"] as DateTime? ?? DateTime.MinValue
            });
        }

        _logger.LogInformation("Retrieved {Count} inventory items", items.Count);
        return ServiceResult<List<InventoryItem>>.Success(items);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Exception getting inventory");
        return ServiceResult<List<InventoryItem>>.Failure(ex.Message);
    }
}
```

### Save/Update Pattern

```csharp
public async Task<ServiceResult> SaveInventoryAsync(InventoryItem item)
{
    _logger.LogInformation("Saving inventory for part {PartID}", item.PartID);
    
    try
    {
        var parameters = new Dictionary<string, object>
        {
            { "PartID", item.PartID },
            { "LocationCode", item.LocationCode },
            { "Quantity", item.Quantity },
            { "UserID", _currentUser.UserID }
        };

        var (result, status, error) = Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usp_SaveInventory",
            parameters,
            30
        );

        if (status != "SUCCESS")
        {
            _logger.LogError("Failed to save inventory: {Error}", error);
            return ServiceResult.Failure(error ?? "Save operation failed");
        }

        _logger.LogInformation("Successfully saved inventory for part {PartID}", item.PartID);
        return ServiceResult.Success();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Exception saving inventory");
        return ServiceResult.Failure($"Save failed: {ex.Message}");
    }
}
```

### Manufacturing Transaction Pattern

```csharp
public async Task<ServiceResult> ProcessTransactionAsync(
    string partID,
    string operation,
    string transactionType,
    string locationCode,
    int quantity)
{
    _logger.LogInformation(
        "Processing {TransactionType} transaction for part {PartID}, operation {Operation}, location {LocationCode}",
        transactionType, partID, operation, locationCode);
    
    try
    {
        // Validate operation (work order sequence step)
        var validOps = _configuration.GetSection("MTM:ValidOperations").Get<List<string>>() ?? new();
        if (!validOps.Contains(operation))
        {
            return ServiceResult.Failure($"Invalid operation: {operation}");
        }

        // Validate transaction type
        var validTypes = new[] { "IN", "OUT", "TRANSFER" };
        if (!validTypes.Contains(transactionType))
        {
            return ServiceResult.Failure($"Invalid transaction type: {transactionType}");
        }

        var parameters = new Dictionary<string, object>
        {
            { "PartID", partID },
            { "Operation", operation },
            { "TransactionType", transactionType },
            { "LocationCode", locationCode },
            { "Quantity", quantity },
            { "UserID", _currentUser.UserID },
            { "SessionID", _sessionService.CurrentSessionID }
        };

        var (result, status, error) = Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usp_ProcessTransaction",
            parameters,
            30
        );

        if (status != "SUCCESS")
        {
            _logger.LogError("Transaction failed: {Error}", error);
            return ServiceResult.Failure(error ?? "Transaction failed");
        }

        _logger.LogInformation("Transaction processed successfully");
        return ServiceResult.Success();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Exception processing transaction");
        return ServiceResult.Failure($"Transaction failed: {ex.Message}");
    }
}
```

### Get Single Row Pattern

```csharp
public async Task<ServiceResult<WorkOrder>> GetWorkOrderAsync(string workOrderNumber)
{
    _logger.LogInformation("Getting work order {WorkOrderNumber}", workOrderNumber);
    
    try
    {
        var parameters = new Dictionary<string, object>
        {
            { "WorkOrderNumber", workOrderNumber }
        };

        var (result, status, error) = Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usp_GetWorkOrder",
            parameters,
            30
        );

        if (status != "SUCCESS")
        {
            _logger.LogError("Failed to get work order: {Error}", error);
            return ServiceResult<WorkOrder>.Failure(error ?? "Database error");
        }

        if (result.Rows.Count == 0)
        {
            _logger.LogWarning("Work order not found: {WorkOrderNumber}", workOrderNumber);
            return ServiceResult<WorkOrder>.Failure("Work order not found");
        }

        var row = result.Rows[0];
        var workOrder = new WorkOrder
        {
            WorkOrderNumber = row["WorkOrderNumber"]?.ToString() ?? string.Empty,
            PartID = row["PartID"]?.ToString() ?? string.Empty,
            Quantity = Convert.ToInt32(row["Quantity"]),
            Status = row["Status"]?.ToString() ?? string.Empty
        };

        return ServiceResult<WorkOrder>.Success(workOrder);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Exception getting work order");
        return ServiceResult<WorkOrder>.Failure(ex.Message);
    }
}
```

## Manufacturing Domain Context

### Operations (Work Order Sequence Steps)

From appsettings.json MTM:ValidOperations:
- **90**: Move (standard manufacturing step)
- **100**: Receive (standard manufacturing step)
- **110**: Ship (standard manufacturing step)
- **10, 20, 30**: Early routing steps
- **120, 130**: Additional sequence steps

Operations represent **WHERE** a part is in its manufacturing workflow, NOT transaction types.

### Transaction Types

Separate from operations, represent **INTENT**:
- **IN**: Receiving inventory into system
- **OUT**: Removing inventory from system
- **TRANSFER**: Moving inventory between locations/operations

### Location Codes

From appsettings.json MTM:DefaultLocations:
- **FLOOR**: Shop floor manufacturing
- **RECEIVING**: Incoming shipments
- **SHIPPING**: Outbound shipments
- Custom locations also supported

## Validation Checklist

Before completion, verify:

- [ ] Uses Helper_Database_StoredProcedure.ExecuteDataTableWithStatus
- [ ] Checks status == "SUCCESS"
- [ ] Handles error return value
- [ ] Logs operation entry and exit
- [ ] Validates input parameters
- [ ] Returns ServiceResult or ServiceResult<T>
- [ ] Converts DataTable to entities correctly
- [ ] Uses connection string from configuration
- [ ] Timeout set to 30 seconds
- [ ] Manufacturing domain validation (operations, locations, transaction types)

## Anti-Patterns to Avoid

❌ **Do NOT**:
- Use inline SQL queries
- Concatenate user input into SQL
- Ignore status/error return values
- Forget to log operations
- Skip parameter validation
- Hardcode connection strings
- Confuse operations with transaction types
- Use synchronous database calls

## Success Criteria

✅ **Success** when:
- Operation uses stored procedure
- Status and error handling correct
- Comprehensive logging implemented
- Parameter validation complete
- Manufacturing domain rules enforced
- Returns appropriate ServiceResult
- Ready for ViewModel consumption

## Next Steps

After creating the database operation:
1. Test with actual database connection
2. Verify stored procedure exists and works
3. Test error scenarios (invalid parameters, connection failure)
4. Monitor logs for proper logging coverage
5. Integrate into ViewModel command handlers
