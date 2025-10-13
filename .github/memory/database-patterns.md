---
description: 'Lessons learned and persistent knowledge for MySQL 5.7 database patterns and stored procedure development'
---

# Database Patterns Memory

**Purpose**: Capture lessons learned, debugging patterns, and discoveries related to MySQL 5.7 database operations in the MTM application.

**Usage**: This file is automatically referenced by GitHub Copilot to provide context-aware assistance for database development. Add lessons as they are discovered during development.

---

## MySQL 5.7 Specific Patterns

### No Common Table Expressions (CTEs)

**Limitation**: MySQL 5.7 does not support WITH (Common Table Expressions) - this was added in MySQL 8.0.

**Workaround Pattern** (subqueries):
```sql
-- ❌ Won't work in MySQL 5.7
WITH TotalInventory AS (
    SELECT LocationCode, SUM(Quantity) AS Total
    FROM Inventory
    GROUP BY LocationCode
)
SELECT * FROM TotalInventory WHERE Total > 100;

-- ✅ Works in MySQL 5.7
SELECT * FROM (
    SELECT LocationCode, SUM(Quantity) AS Total
    FROM Inventory
    GROUP BY LocationCode
) AS TotalInventory
WHERE Total > 100;
```

**Alternative Pattern** (temporary tables):
```sql
CREATE TEMPORARY TABLE IF NOT EXISTS TotalInventory AS
SELECT LocationCode, SUM(Quantity) AS Total
FROM Inventory
GROUP BY LocationCode;

SELECT * FROM TotalInventory WHERE Total > 100;

DROP TEMPORARY TABLE IF EXISTS TotalInventory;
```

**Why**: MySQL 5.7 is the version used by MAMP on development machines. Code must be compatible with this version.

**Discovered**: 2025-10-10 - Documented during MySQL compatibility research for GitHub Copilot configuration.

---

### No Window Functions

**Limitation**: MySQL 5.7 does not support window functions (ROW_NUMBER, RANK, DENSE_RANK, LEAD, LAG) - added in MySQL 8.0.

**Workaround Pattern** (variables for ranking):
```sql
-- ❌ Won't work in MySQL 5.7
SELECT 
    PartNumber,
    Quantity,
    ROW_NUMBER() OVER (PARTITION BY LocationCode ORDER BY Quantity DESC) AS RowNum
FROM Inventory;

-- ✅ Works in MySQL 5.7
SET @row_num = 0;
SET @current_location = '';

SELECT 
    PartNumber,
    Quantity,
    @row_num := IF(@current_location = LocationCode, @row_num + 1, 1) AS RowNum,
    @current_location := LocationCode AS LocationCode
FROM Inventory
ORDER BY LocationCode, Quantity DESC;
```

**Why**: Window functions are powerful but not available in MySQL 5.7. Use variables and subqueries instead.

**Discovered**: During reporting feature development - needed row numbering within groups.

---

## Stored Procedure Patterns

### Helper_Database_StoredProcedure.ExecuteDataTableWithStatus Pattern

**Lesson**: Always use the Helper class for stored procedure execution - it provides consistent status checking and error handling.

**C# Pattern**:
```csharp
var parameters = new Dictionary<string, object>
{
    { "PartNumber", partNumber },
    { "LocationCode", locationCode }
};

var (result, status, error) = await Task.Run(() =>
    Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString,
        "usp_GetInventory",
        parameters,
        30)); // 30 second timeout

if (status == "SUCCESS")
{
    // Process result DataTable
    foreach (DataRow row in result.Rows)
    {
        // Transform to model
    }
}
else
{
    _logger.LogError("Database operation failed: {Error}", error);
    return ServiceResult.Failure(error ?? "Database operation failed");
}
```

**Why**: The Helper class provides:
- Consistent error handling
- Status field checking
- Parameter binding
- Connection management
- Timeout handling

**Critical**: Always check `status == "SUCCESS"` before processing results.

**Discovered**: Pattern established in Database.cs service implementation.

---

### Stored Procedure Naming Convention

**Lesson**: Use `usp_` prefix for user stored procedures to distinguish from system procedures.

**Pattern**:
- `usp_GetInventory` - Retrieves inventory data
- `usp_SaveInventory` - Saves inventory changes
- `usp_TransferInventory` - Transfers inventory between locations
- `usp_GetTransactionHistory` - Retrieves transaction log

**Why**: Clear naming prevents conflicts with system procedures and makes it obvious which procedures are custom application logic.

**Discovered**: During 45+ stored procedure inventory audit.

---

### Parameter Naming Convention

**Lesson**: Use PascalCase for parameter names to match C# conventions and make code more readable.

**Pattern**:
```sql
CREATE PROCEDURE usp_GetInventory(
    IN PartNumber VARCHAR(50),
    IN LocationCode VARCHAR(20),
    IN IncludeInactive BOOLEAN
)
BEGIN
    SELECT * FROM Inventory 
    WHERE PartNumber = PartNumber
      AND LocationCode = LocationCode
      AND (IsActive = 1 OR IncludeInactive = 1);
END;
```

**C# Parameter Binding**:
```csharp
var parameters = new Dictionary<string, object>
{
    { "PartNumber", partNumber },      // Matches SQL parameter exactly
    { "LocationCode", locationCode },  // Matches SQL parameter exactly
    { "IncludeInactive", includeInactive }
};
```

**Why**: Consistent naming between C# and SQL reduces errors and improves code readability.

**Discovered**: Standardization during stored procedure refactoring.

---

## Connection Pooling Patterns

### Connection String Configuration

**Lesson**: Always enable connection pooling with appropriate pool size limits.

**Pattern** (appsettings.json):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=mtm_wip_application;User=root;Password=root;SslMode=none;AllowPublicKeyRetrieval=true;MinPoolSize=5;MaxPoolSize=100;ConnectionTimeout=30;"
  }
}
```

**Pool Size Guidelines**:
- **MinPoolSize=5**: Maintains 5 warm connections ready for use
- **MaxPoolSize=100**: Prevents connection exhaustion under load
- **ConnectionTimeout=30**: 30 seconds before connection attempt fails

**Why**: Connection pooling dramatically improves performance by reusing database connections instead of creating new ones for each operation.

**Discovered**: Performance optimization during load testing.

---

### Proper Connection Disposal

**Lesson**: Always use `using` statements with MySqlConnection, even when using Helper class.

**Anti-Pattern** (connection leak):
```csharp
// ❌ Connection not properly disposed
var connection = new MySqlConnection(_connectionString);
connection.Open();
var command = new MySqlCommand("SELECT * FROM Inventory", connection);
var reader = command.ExecuteReader();
// Missing connection.Close() or Dispose()
```

**Correct Pattern**:
```csharp
// ✅ Connection automatically returned to pool
using (var connection = new MySqlConnection(_connectionString))
{
    await connection.OpenAsync();
    // Execute operations
    // Connection automatically closed and returned to pool
}
```

**Why**: Without proper disposal, connections remain open and pool exhaustion occurs, leading to timeouts and performance degradation.

**Discovered**: Memory profiling identified connection leaks in early implementations.

---

## Transaction Management

### Transaction Pattern for Multi-Step Operations

**Lesson**: Use transactions for operations that modify multiple tables or require atomicity.

**Pattern**:
```csharp
using (var connection = new MySqlConnection(_connectionString))
{
    await connection.OpenAsync();
    using (var transaction = connection.BeginTransaction())
    {
        try
        {
            // Step 1: Update inventory
            var updateParams = new Dictionary<string, object>
            {
                { "PartNumber", partNumber },
                { "NewQuantity", quantity }
            };
            
            var (result1, status1, error1) = Helper_Database_StoredProcedure
                .ExecuteDataTableWithStatus(_connectionString, "usp_UpdateInventory", updateParams, 30);
            
            if (status1 != "SUCCESS")
                throw new Exception(error1);
            
            // Step 2: Log transaction
            var logParams = new Dictionary<string, object>
            {
                { "PartNumber", partNumber },
                { "Action", "UPDATE" },
                { "UserID", userId }
            };
            
            var (result2, status2, error2) = Helper_Database_StoredProcedure
                .ExecuteDataTableWithStatus(_connectionString, "usp_LogTransaction", logParams, 30);
            
            if (status2 != "SUCCESS")
                throw new Exception(error2);
            
            transaction.Commit();
            _logger.LogInformation("Transaction completed successfully");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Transaction failed, rolled back");
            throw;
        }
    }
}
```

**Why**: Transactions ensure all-or-nothing semantics - either all operations succeed or all are rolled back, maintaining data integrity.

**When to Use**:
- Multiple table updates that must be atomic
- Inventory transfers (deduct from one location, add to another)
- Order processing with inventory updates
- Audit logging that must match data changes

**Discovered**: During inventory transfer feature implementation.

---

## Manufacturing Domain Database Patterns

### Operations as Work Order Sequence Steps

**Critical Distinction**: Operations (10, 20, 30, 90, 100, 110, 120, 130) represent WHERE a part is in the manufacturing workflow sequence, NOT transaction types.

**Database Pattern**:
```sql
CREATE TABLE WorkOrderRouting (
    WorkOrderID INT,
    OperationNumber INT, -- 10, 20, 30, 90, 100, 110, etc.
    OperationDescription VARCHAR(100),
    Sequence INT,
    PRIMARY KEY (WorkOrderID, OperationNumber)
);

-- Example: Part routing sequence
-- Operation 10: Raw material receiving
-- Operation 20: Cutting
-- Operation 30: Welding
-- Operation 90: Assembly
-- Operation 100: Testing
-- Operation 110: Packaging
```

**Transaction Type Pattern** (separate from operations):
```sql
CREATE TABLE InventoryTransaction (
    TransactionID INT PRIMARY KEY,
    PartNumber VARCHAR(50),
    TransactionType ENUM('IN', 'OUT', 'TRANSFER'),  -- Intent of movement
    OperationNumber INT,  -- Where in workflow (separate concept)
    LocationCode VARCHAR(20),
    Quantity DECIMAL(10,2),
    TransactionDate DATETIME
);
```

**Why**: Operations indicate manufacturing sequence position. Transaction types indicate movement intent (receiving, shipping, moving). These are independent concepts that work together.

**Example**:
- **Operation 100** (Testing) + **Transaction Type IN** = Parts arriving at testing station
- **Operation 100** (Testing) + **Transaction Type OUT** = Parts leaving testing (failed or passed)
- **Operation 100** (Testing) + **Transaction Type TRANSFER** = Parts moving between testing bays

**Discovered**: 2025-10-10 - Critical distinction documented during manufacturing domain clarification.

---

### Location Code Validation

**Lesson**: Use ENUM or CHECK constraints to enforce valid location codes.

**Pattern**:
```sql
-- Option 1: ENUM type
CREATE TABLE Inventory (
    PartNumber VARCHAR(50),
    LocationCode ENUM('FLOOR', 'RECEIVING', 'SHIPPING', 'STORAGE', 'INSPECTION'),
    Quantity DECIMAL(10,2)
);

-- Option 2: Foreign key to Locations table (more flexible)
CREATE TABLE Locations (
    LocationCode VARCHAR(20) PRIMARY KEY,
    LocationName VARCHAR(100),
    LocationType VARCHAR(50),
    IsActive BOOLEAN
);

CREATE TABLE Inventory (
    PartNumber VARCHAR(50),
    LocationCode VARCHAR(20),
    Quantity DECIMAL(10,2),
    FOREIGN KEY (LocationCode) REFERENCES Locations(LocationCode)
);
```

**Configuration-Driven**:
From `appsettings.json`:
```json
{
  "MTM": {
    "DefaultLocations": ["FLOOR", "RECEIVING", "SHIPPING"]
  }
}
```

**Why**: Enforcing valid location codes prevents data entry errors and ensures referential integrity.

**Discovered**: During location management feature implementation.

---

## Error Handling Patterns

### Status Field Pattern in Stored Procedures

**Lesson**: Stored procedures should return consistent status/error information.

**Pattern**:
```sql
CREATE PROCEDURE usp_SaveInventory(
    IN PartNumber VARCHAR(50),
    IN LocationCode VARCHAR(20),
    IN Quantity DECIMAL(10,2),
    OUT Status VARCHAR(20),
    OUT ErrorMessage VARCHAR(500)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET Status = 'ERROR';
        SET ErrorMessage = 'Database error occurred';
        ROLLBACK;
    END;
    
    START TRANSACTION;
    
    -- Validation
    IF PartNumber IS NULL OR PartNumber = '' THEN
        SET Status = 'ERROR';
        SET ErrorMessage = 'Part number is required';
        ROLLBACK;
    ELSE
        -- Perform operation
        INSERT INTO Inventory (PartNumber, LocationCode, Quantity)
        VALUES (PartNumber, LocationCode, Quantity)
        ON DUPLICATE KEY UPDATE Quantity = Quantity;
        
        SET Status = 'SUCCESS';
        SET ErrorMessage = NULL;
        COMMIT;
    END IF;
END;
```

**C# Handling**:
```csharp
var (result, status, error) = Helper_Database_StoredProcedure
    .ExecuteDataTableWithStatus(_connectionString, "usp_SaveInventory", parameters, 30);

if (status == "SUCCESS")
{
    // Success path
}
else
{
    // Error path - error message is user-friendly
    _logger.LogWarning("Database operation failed: {Error}", error);
    return ServiceResult.Failure(error);
}
```

**Why**: Consistent status/error pattern makes error handling predictable and enables user-friendly error messages.

**Discovered**: Error handling standardization initiative.

---

### Retry Logic for Transient Failures

**Lesson**: Implement retry logic for transient database errors (deadlocks, connection timeouts).

**Pattern**:
```csharp
private async Task<ServiceResult<T>> ExecuteWithRetryAsync<T>(
    Func<Task<ServiceResult<T>>> operation,
    int maxRetries = 3)
{
    int attempt = 0;
    while (true)
    {
        try
        {
            return await operation();
        }
        catch (MySqlException ex) when (IsTransientError(ex) && attempt < maxRetries)
        {
            attempt++;
            _logger.LogWarning(
                "Transient database error on attempt {Attempt}/{MaxRetries}: {Error}",
                attempt, maxRetries, ex.Message);
            
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Exponential backoff
        }
    }
}

private bool IsTransientError(MySqlException ex)
{
    // Transient error codes
    return ex.Number == 1205 || // Deadlock
           ex.Number == 1213 || // Lock wait timeout
           ex.Number == 2006 || // Server has gone away
           ex.Number == 2013;   // Lost connection during query
}
```

**Why**: Network issues, deadlocks, and connection timeouts can occur in production. Retry logic with exponential backoff handles these gracefully.

**Discovered**: Production incident analysis - transient errors resolved by retry.

---

## Performance Patterns

### Index Strategy for Common Queries

**Lesson**: Add indexes for columns used in WHERE, JOIN, and ORDER BY clauses.

**Pattern**:
```sql
-- Inventory table indexes
CREATE INDEX idx_inventory_partnumber ON Inventory(PartNumber);
CREATE INDEX idx_inventory_location ON Inventory(LocationCode);
CREATE INDEX idx_inventory_composite ON Inventory(PartNumber, LocationCode);

-- Transaction table indexes
CREATE INDEX idx_transaction_date ON InventoryTransaction(TransactionDate);
CREATE INDEX idx_transaction_part ON InventoryTransaction(PartNumber);
CREATE INDEX idx_transaction_operation ON InventoryTransaction(OperationNumber);
```

**Guidelines**:
- Index foreign keys
- Index columns in WHERE clauses
- Composite indexes for multi-column queries
- Don't over-index (slows down INSERTs/UPDATEs)

**Why**: Indexes dramatically improve query performance by avoiding full table scans.

**Discovered**: Query performance profiling using EXPLAIN.

---

### Query Optimization with EXPLAIN

**Lesson**: Use EXPLAIN to analyze query execution plans before optimizing.

**Pattern**:
```sql
EXPLAIN SELECT * FROM Inventory 
WHERE PartNumber = 'ABC123' 
  AND LocationCode = 'FLOOR';

-- Look for:
-- - type: ALL (bad - full table scan) vs ref/eq_ref (good - using index)
-- - rows: Lower is better
-- - Extra: "Using filesort" or "Using temporary" indicates poor performance
```

**Optimization Example**:
```sql
-- Before: Full table scan
SELECT * FROM Inventory WHERE YEAR(LastUpdated) = 2025;

-- After: Uses index
SELECT * FROM Inventory WHERE LastUpdated >= '2025-01-01' AND LastUpdated < '2026-01-01';
```

**Why**: EXPLAIN shows how MySQL executes queries, revealing missing indexes and inefficient operations.

**Discovered**: Performance tuning during slow query investigation.

---

## Data Type Best Practices

### DECIMAL for Currency and Quantities

**Lesson**: Always use DECIMAL for monetary values and precise quantities, never FLOAT or DOUBLE.

**Pattern**:
```sql
CREATE TABLE Inventory (
    PartNumber VARCHAR(50),
    Quantity DECIMAL(10,2),  -- ✅ Precise
    UnitPrice DECIMAL(10,2), -- ✅ No rounding errors
    TotalValue DECIMAL(12,2) -- ✅ Calculated precisely
);

-- ❌ Don't use FLOAT/DOUBLE for money
-- Quantity FLOAT  -- Causes rounding errors!
```

**Why**: FLOAT and DOUBLE have rounding errors that accumulate. DECIMAL provides exact precision for financial calculations.

**Discovered**: Financial discrepancy investigation - traced to FLOAT usage.

---

## Common Pitfalls

### Pitfall 1: Not Checking Status Field

**Issue**: Assuming stored procedure success without checking status field leads to silent failures.

**Fix**: Always check `status == "SUCCESS"` after every Helper_Database_StoredProcedure call.

---

### Pitfall 2: Using MySQL 8.0+ Features

**Issue**: Using CTEs or window functions breaks compatibility with MySQL 5.7 (MAMP).

**Fix**: Use subqueries and variables instead of CTEs/window functions.

---

### Pitfall 3: Connection Leaks

**Issue**: Not disposing MySqlConnection objects exhausts connection pool.

**Fix**: Always use `using` statements for database connections.

---

### Pitfall 4: Confusing Operations with Transaction Types

**Issue**: Treating operation numbers (90/100/110) as transaction types (IN/OUT/TRANSFER).

**Fix**: Understand operations indicate WHERE in workflow, transaction types indicate movement INTENT. They are independent concepts.

---

## Memory File Maintenance

**Last Updated**: 2025-10-10  
**Maintainer**: GitHub Copilot (via user input)

**How to Add Lessons**:
1. Identify a recurring database pattern or solved problem
2. Document the lesson with Pattern/Why/Discovered sections
3. Include SQL and C# code examples for clarity
4. Commit changes to memory file

**Review Frequency**: After database schema changes or when patterns evolve
