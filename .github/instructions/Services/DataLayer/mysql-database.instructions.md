---
description: 'MySQL 5.7 database patterns and manufacturing domain context for MTM'
applyTo: '**/Services/Database.cs,**/Services/*Service.cs,**/*Repository.cs'
---

<!-- Based on patterns from: https://github.com/github/awesome-copilot -->

# MySQL Database Patterns for MTM Manufacturing

## Overview

This file defines MySQL 5.7 database patterns, connection management, and manufacturing domain context for the MTM WIP Application. The application uses MySQL 5.7 via MAMP with MySql.Data 9.4.0 connector and Dapper ORM for data access.

## Core Principles

### Stored Procedure First
- All database operations use stored procedures
- No inline SQL in application code (except simple queries if necessary)
- 45+ stored procedures handle all CRUD and business logic operations
- Stored procedures encapsulate manufacturing business rules

### Connection Pooling
- Always use connection pooling for performance
- Configuration: MinPoolSize=5, MaxPoolSize=100
- Timeout: 30 seconds (CommandTimeoutSeconds in appsettings.json)
- MaxRetryAttempts: 3 with exponential backoff

### Async/Await for All Operations
- All database operations must be asynchronous
- Use `async Task<T>` for methods that return data
- Use `async Task` for methods that don't return data
- Never block with `.Result` or `.Wait()`

## Database Connection Configuration

### MAMP MySQL 5.7 Credentials
- Server: `localhost`
- Port: `3306` (default)
- Database: `mtm_wip_application` (production)
- Database: `mtm_wip_application_test` (development)
- Username: `root`
- Password: `root`
- Connection String: `Server=localhost;Database=mtm_wip_application;SslMode=none;AllowPublicKeyRetrieval=true;`

### Connection String Pattern
```
Server=localhost;Port=3306;Database=mtm_wip_application;User=root;Password=root;SslMode=none;AllowPublicKeyRetrieval=true;MinPoolSize=5;MaxPoolSize=100;ConnectionTimeout=30;
```

### Configuration in appsettings.json
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=mtm_wip_application;User=root;Password=root;SslMode=none;AllowPublicKeyRetrieval=true;MinPoolSize=5;MaxPoolSize=100;"
},
"Database": {
  "CommandTimeoutSeconds": 30,
  "MaxRetryAttempts": 3,
  "RetryDelaySeconds": 5,
  "EnableConnectionPooling": true,
  "MinPoolSize": 5,
  "MaxPoolSize": 100
}
```

## Stored Procedure Execution Patterns

### Helper_Database_StoredProcedure.ExecuteDataTableWithStatus
This is the primary method for executing stored procedures:

```
public static (DataTable result, string status, string error) ExecuteDataTableWithStatus(
    string connectionString,
    string storedProcedureName,
    Dictionary<string, object> parameters,
    int commandTimeout = 30)
```

### Usage Example
```
var parameters = new Dictionary<string, object>
{
    { "UserID", userId },
    { "StartDate", startDate },
    { "EndDate", endDate }
};

var (result, status, error) = Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    _connectionString,
    "usp_GetInventoryTransactions",
    parameters,
    30
);

if (status == "SUCCESS")
{
    // Process result DataTable
}
else
{
    _logger.LogError("Database operation failed: {Error}", error);
    // Handle error
}
```

### Return Value Handling
- **result**: DataTable containing query results (may be empty)
- **status**: "SUCCESS" or "ERROR"
- **error**: Error message string (null on success)

## Manufacturing Domain Context

### Work Order Operations (Sequence Steps)
Operations represent steps in a work order's manufacturing routing sequence, NOT transaction types:

- **10, 20, 30**: Early routing steps in manufacturing sequence
- **90, 100, 110**: Standard manufacturing sequence steps (ValidOperations from appsettings.json)
- **120, 130**: Additional sequence steps (extended in code beyond config)
- **NOT transaction types**: Operations indicate where a part is in its manufacturing workflow

### ValidOperations Configuration
From appsettings.json `MTM.ValidOperations`:
```json
"ValidOperations": [ "90", "100", "110" ]
```

These are the commonly validated operation numbers, but other operations (10, 20, 30, 120, 130) may be used depending on manufacturing routing requirements.

### Transaction Types (Separate from Operations)
Transaction types represent inventory movement intent:

- **IN**: Receiving inventory into the system (incoming shipments, returned items)
- **OUT**: Removing inventory from the system (shipments, scrap, consumption)
- **TRANSFER**: Moving inventory between locations or operations (internal movements)

**Determination**: Transaction type is determined by user intent and workflow context, not by the operation number.

### Location Codes
From appsettings.json `MTM.DefaultLocations`:
```json
"DefaultLocations": [ "FLOOR", "RECEIVING", "SHIPPING" ]
```

- **FLOOR**: Shop floor inventory (active manufacturing)
- **RECEIVING**: Incoming shipments and receiving area
- **SHIPPING**: Outbound shipments and staging area
- **Custom locations**: Additional locations can be defined in the database

### Session Management
From appsettings.json `MTM` configuration:
```json
"SessionTimeoutMinutes": 60,
"MaxQuickButtons": 10,
"AutoSaveUserPreferences": true,
"AutoSaveIntervalMinutes": 5
```

- Sessions expire after 60 minutes of inactivity
- Maximum 10 quick buttons per user for rapid transaction shortcuts
- User preferences auto-save every 5 minutes

## Dapper ORM Patterns

### Query Execution
```
using (var connection = new MySqlConnection(_connectionString))
{
    await connection.OpenAsync();
    
    var result = await connection.QueryAsync<InventoryItem>(
        "SELECT * FROM Inventory WHERE LocationCode = @LocationCode",
        new { LocationCode = locationCode }
    );
    
    return result.ToList();
}
```

### Parameterized Queries
- Always use parameterized queries for SQL injection prevention
- Dapper handles parameter binding securely
- Never concatenate user input into SQL strings

### Multiple Result Sets
```
using (var connection = new MySqlConnection(_connectionString))
{
    await connection.OpenAsync();
    
    using (var multi = await connection.QueryMultipleAsync(
        "usp_GetOrderDetails",
        new { OrderID = orderId },
        commandType: CommandType.StoredProcedure))
    {
        var header = await multi.ReadFirstOrDefaultAsync<OrderHeader>();
        var lines = (await multi.ReadAsync<OrderLine>()).ToList();
        
        return new OrderDetails { Header = header, Lines = lines };
    }
}
```

## Error Handling and Retry Logic

### Transient Error Retry
```
private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation)
{
    int attempt = 0;
    while (true)
    {
        try
        {
            return await operation();
        }
        catch (MySqlException ex) when (IsTransientError(ex) && attempt < _maxRetryAttempts)
        {
            attempt++;
            _logger.LogWarning("Database operation failed, retrying {Attempt}/{MaxAttempts}", 
                attempt, _maxRetryAttempts);
            await Task.Delay(TimeSpan.FromSeconds(_retryDelaySeconds * attempt));
        }
    }
}

private bool IsTransientError(MySqlException ex)
{
    // Transient error codes: connection timeout, deadlock, etc.
    return ex.Number == 1205 || // Deadlock
           ex.Number == 1213 || // Lock wait timeout
           ex.Number == 2006 || // Server has gone away
           ex.Number == 2013;   // Lost connection during query
}
```

### Connection Validation
```
private async Task<bool> ValidateConnectionAsync()
{
    try
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            return connection.State == ConnectionState.Open;
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Connection validation failed");
        return false;
    }
}
```

## MySQL 5.7 Specific Considerations

### Known Limitations
- **No CTEs (Common Table Expressions)**: Use subqueries or temporary tables instead
- **No window functions**: Use variables and subqueries for ranking/aggregation
- **Limited JSON support**: JSON functions available but limited compared to MySQL 8.0

### Date and Time Handling
- Use `DateTime` in C# code
- MySQL 5.7 stores as `DATETIME` or `TIMESTAMP`
- Always specify time zone handling explicitly
- Use UTC for storage, convert to local time in application layer

### String Encoding
- Default charset: utf8mb4 for full Unicode support
- Collation: utf8mb4_general_ci (case-insensitive) or utf8mb4_bin (case-sensitive)
- Handle emoji and special characters correctly

## Performance Optimization

### Connection Pooling Best Practices
- Dispose connections properly (use `using` statements)
- Don't hold connections open longer than necessary
- Let connection pool manage connection lifecycle
- Monitor connection pool metrics in production

### Query Optimization
- Index columns used in WHERE clauses and JOINs
- Use EXPLAIN to analyze query execution plans
- Avoid SELECT * - specify columns explicitly
- Limit result sets with LIMIT clauses when appropriate

### Stored Procedure Performance
- Keep stored procedures focused and single-purpose
- Avoid cursors - use set-based operations
- Cache execution plans by using parameterized procedures
- Monitor slow query log for optimization opportunities

## Transaction Management

### Transaction Pattern
```
using (var connection = new MySqlConnection(_connectionString))
{
    await connection.OpenAsync();
    using (var transaction = connection.BeginTransaction())
    {
        try
        {
            // Execute operations
            await connection.ExecuteAsync("usp_Operation1", param1, transaction: transaction);
            await connection.ExecuteAsync("usp_Operation2", param2, transaction: transaction);
            
            transaction.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed, rolling back");
            transaction.Rollback();
            throw;
        }
    }
}
```

### Transaction Isolation Levels
- Default: Read Committed
- Use explicit isolation levels when needed for consistency
- Be aware of locking implications

## Logging Database Operations

### Log Structured Data
```
_logger.LogInformation(
    "Executing stored procedure {StoredProcedure} with parameters {Parameters}",
    storedProcedureName,
    JsonSerializer.Serialize(parameters)
);
```

### Log Performance Metrics
```
var stopwatch = Stopwatch.StartNew();
var result = await ExecuteStoredProcedureAsync(procName, parameters);
stopwatch.Stop();

if (stopwatch.ElapsedMilliseconds > 1000)
{
    _logger.LogWarning(
        "Slow query detected: {StoredProcedure} took {ElapsedMs}ms",
        procName,
        stopwatch.ElapsedMilliseconds
    );
}
```

### Never Log Sensitive Data
- Don't log passwords, connection strings, or sensitive business data
- Sanitize parameters before logging
- Use log level appropriately (Debug for detailed parameters)

## Security Best Practices

### SQL Injection Prevention
- Always use parameterized queries or stored procedures
- Dapper handles parameter binding securely
- Never concatenate user input into SQL strings
- Validate and sanitize input at application layer before database calls

### Connection String Security
- Store connection strings in appsettings.json
- Never hardcode credentials in code
- Use secure configuration for production (Azure Key Vault, environment variables)
- Rotate passwords regularly

### Least Privilege Access
- Database user should have only necessary permissions
- Use separate credentials for different environments (dev, test, prod)
- Audit database access and operations

## Testing Database Operations

### Manual Validation Approach
- Test stored procedure calls with valid and invalid parameters
- Verify error handling for database failures
- Test transaction rollback scenarios
- Validate connection pooling behavior under load

### Integration Testing
- Use test database (`mtm_wip_application_test`) for integration tests
- Clean up test data after test execution
- Test with realistic data volumes
- Verify stored procedure results match expectations
