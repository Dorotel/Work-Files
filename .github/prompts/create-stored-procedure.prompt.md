---
description: 'Generate MySQL 5.7 stored procedure with MTM patterns'
---

# Create Stored Procedure

Generate MySQL 5.7 stored procedure following MTM database patterns with proper parameter handling, error management, and result set formatting.

## Prerequisites

- Database schema understanding
- Procedure purpose defined
- Input/output requirements known
- MySQL 5.7 compatibility requirements

## User Input

```text
$ARGUMENTS
```

Parse arguments to extract:
- Procedure name (e.g., `usp_GetInventory`, `usp_SaveTransaction`)
- Operation type (SELECT, INSERT, UPDATE, DELETE, mixed)
- Parameters needed
- Return data requirements
- Manufacturing domain context

If arguments are incomplete, prompt for:
1. What should the stored procedure do?
2. What input parameters are needed?
3. What data should it return?
4. What manufacturing domain rules apply?

## MySQL 5.7 Constraints

### Known Limitations
- **No CTEs**: Common Table Expressions not supported
- **No window functions**: Use variables and subqueries instead
- **Limited JSON**: JSON functions available but limited vs MySQL 8.0
- **Variables for ranking**: Use user variables (@var) for row numbering

### Compatibility Patterns
- Use subqueries instead of CTEs
- Use user variables for complex calculations
- Avoid JSON_TABLE (MySQL 8.0 feature)
- Use IFNULL() and COALESCE() for null handling

## Stored Procedure Structure

### Basic Template

```sql
DELIMITER $$

DROP PROCEDURE IF EXISTS {ProcedureName}$$

CREATE PROCEDURE {ProcedureName}(
    IN p_Parameter1 VARCHAR(50),
    IN p_Parameter2 INT,
    OUT p_Status VARCHAR(10),
    OUT p_Message VARCHAR(255)
)
BEGIN
    -- Error handling
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = 'ERROR';
        SET p_Message = 'Database error occurred';
        ROLLBACK;
    END;

    -- Validation
    IF p_Parameter1 IS NULL OR p_Parameter1 = '' THEN
        SET p_Status = 'ERROR';
        SET p_Message = 'Parameter1 is required';
        LEAVE proc_end;
    END IF;

    -- Main logic
    START TRANSACTION;
    
    -- Your SQL operations here
    
    COMMIT;
    
    -- Success
    SET p_Status = 'SUCCESS';
    SET p_Message = 'Operation completed successfully';
    
    proc_end:
END$$

DELIMITER ;
```

## Stored Procedure Patterns

### Pattern 1: GET Operation (Read)

```sql
DELIMITER $$

DROP PROCEDURE IF EXISTS usp_GetInventoryByLocation$$

CREATE PROCEDURE usp_GetInventoryByLocation(
    IN p_LocationCode VARCHAR(20),
    OUT p_Status VARCHAR(10),
    OUT p_Message VARCHAR(255)
)
BEGIN
    DECLARE v_LocationValid INT DEFAULT 0;
    
    -- Error handler
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = 'ERROR';
        SET p_Message = 'Failed to retrieve inventory';
        ROLLBACK;
    END;
    
    -- Validate location code
    IF p_LocationCode IS NULL OR p_LocationCode = '' THEN
        SET p_Status = 'ERROR';
        SET p_Message = 'Location code is required';
        LEAVE proc_end;
    END IF;
    
    -- Check if location exists
    SELECT COUNT(*) INTO v_LocationValid
    FROM Locations
    WHERE LocationCode = p_LocationCode AND IsActive = 1;
    
    IF v_LocationValid = 0 THEN
        SET p_Status = 'ERROR';
        SET p_Message = CONCAT('Invalid location: ', p_LocationCode);
        LEAVE proc_end;
    END IF;
    
    -- Return inventory data
    SELECT 
        PartID,
        LocationCode,
        Quantity,
        LastUpdated,
        UpdatedBy
    FROM Inventory
    WHERE LocationCode = p_LocationCode
      AND IsActive = 1
    ORDER BY PartID;
    
    SET p_Status = 'SUCCESS';
    SET p_Message = 'Inventory retrieved successfully';
    
    proc_end:
END$$

DELIMITER ;
```

### Pattern 2: SAVE Operation (Insert/Update)

```sql
DELIMITER $$

DROP PROCEDURE IF EXISTS usp_SaveInventory$$

CREATE PROCEDURE usp_SaveInventory(
    IN p_PartID VARCHAR(50),
    IN p_LocationCode VARCHAR(20),
    IN p_Quantity INT,
    IN p_UserID VARCHAR(50),
    OUT p_Status VARCHAR(10),
    OUT p_Message VARCHAR(255)
)
BEGIN
    DECLARE v_Exists INT DEFAULT 0;
    
    -- Error handler
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = 'ERROR';
        SET p_Message = 'Failed to save inventory';
        ROLLBACK;
    END;
    
    -- Validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 'ERROR';
        SET p_Message = 'Part ID is required';
        LEAVE proc_end;
    END IF;
    
    IF p_LocationCode IS NULL OR p_LocationCode = '' THEN
        SET p_Status = 'ERROR';
        SET p_Message = 'Location code is required';
        LEAVE proc_end;
    END IF;
    
    IF p_Quantity IS NULL OR p_Quantity < 0 THEN
        SET p_Status = 'ERROR';
        SET p_Message = 'Invalid quantity';
        LEAVE proc_end;
    END IF;
    
    START TRANSACTION;
    
    -- Check if record exists
    SELECT COUNT(*) INTO v_Exists
    FROM Inventory
    WHERE PartID = p_PartID AND LocationCode = p_LocationCode;
    
    IF v_Exists > 0 THEN
        -- Update existing
        UPDATE Inventory
        SET Quantity = p_Quantity,
            LastUpdated = NOW(),
            UpdatedBy = p_UserID
        WHERE PartID = p_PartID AND LocationCode = p_LocationCode;
        
        SET p_Message = 'Inventory updated successfully';
    ELSE
        -- Insert new
        INSERT INTO Inventory (PartID, LocationCode, Quantity, LastUpdated, UpdatedBy, IsActive)
        VALUES (p_PartID, p_LocationCode, p_Quantity, NOW(), p_UserID, 1);
        
        SET p_Message = 'Inventory created successfully';
    END IF;
    
    COMMIT;
    SET p_Status = 'SUCCESS';
    
    proc_end:
END$$

DELIMITER ;
```

### Pattern 3: Manufacturing Transaction

```sql
DELIMITER $$

DROP PROCEDURE IF EXISTS usp_ProcessTransaction$$

CREATE PROCEDURE usp_ProcessTransaction(
    IN p_PartID VARCHAR(50),
    IN p_Operation VARCHAR(10),
    IN p_TransactionType VARCHAR(10),
    IN p_LocationCode VARCHAR(20),
    IN p_Quantity INT,
    IN p_UserID VARCHAR(50),
    IN p_SessionID VARCHAR(50),
    OUT p_Status VARCHAR(10),
    OUT p_Message VARCHAR(255)
)
BEGIN
    DECLARE v_CurrentQuantity INT DEFAULT 0;
    DECLARE v_OperationValid INT DEFAULT 0;
    
    -- Error handler
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = 'ERROR';
        SET p_Message = 'Transaction failed';
        ROLLBACK;
    END;
    
    -- Validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 'ERROR';
        SET p_Message = 'Part ID is required';
        LEAVE proc_end;
    END IF;
    
    -- Validate operation (work order sequence step)
    -- Operations: 10,20,30,90,100,110,120,130
    IF p_Operation NOT IN ('10','20','30','90','100','110','120','130') THEN
        SET p_Status = 'ERROR';
        SET p_Message = CONCAT('Invalid operation: ', p_Operation);
        LEAVE proc_end;
    END IF;
    
    -- Validate transaction type (separate from operations)
    IF p_TransactionType NOT IN ('IN', 'OUT', 'TRANSFER') THEN
        SET p_Status = 'ERROR';
        SET p_Message = CONCAT('Invalid transaction type: ', p_TransactionType);
        LEAVE proc_end;
    END IF;
    
    -- Validate location
    IF p_LocationCode NOT IN ('FLOOR', 'RECEIVING', 'SHIPPING') THEN
        -- Check if custom location exists
        SELECT COUNT(*) INTO v_OperationValid
        FROM Locations
        WHERE LocationCode = p_LocationCode AND IsActive = 1;
        
        IF v_OperationValid = 0 THEN
            SET p_Status = 'ERROR';
            SET p_Message = CONCAT('Invalid location: ', p_LocationCode);
            LEAVE proc_end;
        END IF;
    END IF;
    
    START TRANSACTION;
    
    -- Get current inventory
    SELECT IFNULL(Quantity, 0) INTO v_CurrentQuantity
    FROM Inventory
    WHERE PartID = p_PartID AND LocationCode = p_LocationCode;
    
    -- Process based on transaction type
    CASE p_TransactionType
        WHEN 'IN' THEN
            -- Add to inventory
            IF v_CurrentQuantity > 0 THEN
                UPDATE Inventory
                SET Quantity = Quantity + p_Quantity,
                    LastUpdated = NOW(),
                    UpdatedBy = p_UserID
                WHERE PartID = p_PartID AND LocationCode = p_LocationCode;
            ELSE
                INSERT INTO Inventory (PartID, LocationCode, Quantity, LastUpdated, UpdatedBy, IsActive)
                VALUES (p_PartID, p_LocationCode, p_Quantity, NOW(), p_UserID, 1);
            END IF;
            
        WHEN 'OUT' THEN
            -- Remove from inventory
            IF v_CurrentQuantity < p_Quantity THEN
                SET p_Status = 'ERROR';
                SET p_Message = 'Insufficient inventory';
                ROLLBACK;
                LEAVE proc_end;
            END IF;
            
            UPDATE Inventory
            SET Quantity = Quantity - p_Quantity,
                LastUpdated = NOW(),
                UpdatedBy = p_UserID
            WHERE PartID = p_PartID AND LocationCode = p_LocationCode;
            
        WHEN 'TRANSFER' THEN
            -- Transfer handled by separate procedure
            SET p_Status = 'ERROR';
            SET p_Message = 'Use usp_TransferInventory for transfers';
            ROLLBACK;
            LEAVE proc_end;
    END CASE;
    
    -- Log transaction
    INSERT INTO TransactionHistory (
        TransactionDate,
        PartID,
        Operation,
        TransactionType,
        LocationCode,
        Quantity,
        UserID,
        SessionID
    ) VALUES (
        NOW(),
        p_PartID,
        p_Operation,
        p_TransactionType,
        p_LocationCode,
        p_Quantity,
        p_UserID,
        p_SessionID
    );
    
    COMMIT;
    SET p_Status = 'SUCCESS';
    SET p_Message = 'Transaction completed successfully';
    
    proc_end:
END$$

DELIMITER ;
```

### Pattern 4: Complex Query with Variables (MySQL 5.7)

```sql
DELIMITER $$

DROP PROCEDURE IF EXISTS usp_GetTopParts$$

CREATE PROCEDURE usp_GetTopParts(
    IN p_TopN INT,
    OUT p_Status VARCHAR(10),
    OUT p_Message VARCHAR(255)
)
BEGIN
    -- Error handler
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = 'ERROR';
        SET p_Message = 'Failed to retrieve top parts';
    END;
    
    -- Use user variables for ranking (MySQL 5.7 doesn't have ROW_NUMBER)
    SET @rank := 0;
    
    SELECT 
        (@rank := @rank + 1) AS Rank,
        PartID,
        TotalQuantity,
        TransactionCount
    FROM (
        SELECT 
            PartID,
            SUM(Quantity) AS TotalQuantity,
            COUNT(*) AS TransactionCount
        FROM TransactionHistory
        WHERE TransactionDate >= DATE_SUB(NOW(), INTERVAL 30 DAY)
        GROUP BY PartID
        ORDER BY TotalQuantity DESC
        LIMIT p_TopN
    ) AS RankedParts;
    
    SET p_Status = 'SUCCESS';
    SET p_Message = CONCAT('Retrieved top ', p_TopN, ' parts');
    
END$$

DELIMITER ;
```

## Manufacturing Domain Rules

### Operations (Work Order Sequence Steps)
```sql
-- Operations represent WHERE part is in manufacturing workflow
-- Valid operations: 10,20,30,90,100,110,120,130
-- 90 = Move, 100 = Receive, 110 = Ship (common operations)
-- NOT transaction types!
```

### Transaction Types (Intent)
```sql
-- Transaction types represent INTENT of inventory movement
-- Valid types: IN, OUT, TRANSFER
-- Separate from operations
```

### Location Codes
```sql
-- Default locations: FLOOR, RECEIVING, SHIPPING
-- Custom locations also supported (check Locations table)
```

## Validation Checklist

After creating stored procedure, verify:

- [ ] Procedure name follows usp_{Entity}{Action} convention
- [ ] OUT parameters for p_Status and p_Message
- [ ] Error handler with ROLLBACK
- [ ] Input parameter validation
- [ ] Transaction management (START/COMMIT/ROLLBACK)
- [ ] Manufacturing domain validation (operations, locations, transaction types)
- [ ] Proper NULL handling (IFNULL, COALESCE)
- [ ] No MySQL 8.0-only features (CTEs, window functions)
- [ ] User variables for ranking if needed
- [ ] Transaction logging where appropriate
- [ ] Clear success/error messages

## Success Criteria

✅ **Success** when:
- Procedure compiles without errors
- Works correctly with MySQL 5.7
- Handles all validation scenarios
- Returns proper status/message
- Follows MTM patterns
- Manufacturing domain rules enforced
- Ready for Helper_Database_StoredProcedure usage

## Next Steps

After creating stored procedure:
1. Execute in MySQL Workbench to verify syntax
2. Test with sample data
3. Test error scenarios
4. Create service method to call procedure
5. Document procedure in database documentation
6. Add to application's stored procedure list
