---
description: 'Lessons learned and persistent knowledge for manual validation testing and quality assurance patterns'
---

# Testing Patterns Memory

**Purpose**: Capture lessons learned, debugging patterns, and discoveries related to manual validation testing in the MTM application.

**Usage**: This file is automatically referenced by GitHub Copilot to provide context-aware assistance for testing approaches. Add lessons as they are discovered during development.

---

## Manual Validation Approach

### Success Criteria Pattern

**Lesson**: Define measurable success criteria for every feature before implementation.

**Pattern**:
```markdown
## Success Criteria

**SC-001: ViewModel Generation Correctness**
- Target: 95% of generated ViewModels follow MVVM Community Toolkit patterns without manual corrections
- Validation: Generate 20 ViewModels, count those requiring fixes
- Pass: ≤ 1 ViewModel requires corrections

**SC-002: Database Operation Compliance**
- Target: 100% of database operations use stored procedures (no inline SQL)
- Validation: Code review of all Service layer database calls
- Pass: Zero inline SQL queries found

**SC-003: Theme V2 Integration**
- Target: 100% of Views use Theme V2 DynamicResource (no hardcoded colors)
- Validation: AXAML review of all Views
- Pass: Zero hardcoded color values found
```

**Why**: Measurable criteria provide objective validation, enable consistent quality assessment, and create clear definition of "done".

**Discovered**: 2025-10-10 - Pattern established during comprehensive GitHub Copilot configuration specification.

---

### Validation Checklist Pattern

**Lesson**: Create comprehensive validation checklists for complex features covering all quality dimensions.

**Pattern**:
```markdown
## Validation Checklist

### ✅ Compilation and Build
- [ ] Project compiles without errors
- [ ] No AVLN2000 binding errors in output window
- [ ] No warnings related to feature

### ✅ Functionality
- [ ] Feature works as specified in acceptance criteria
- [ ] All user scenarios complete successfully
- [ ] Error handling works correctly
- [ ] Loading indicators display during operations

### ✅ Pattern Compliance
- [ ] MVVM Community Toolkit patterns used correctly
- [ ] Theme V2 resources used (no hardcoded colors)
- [ ] Stored procedures used (no inline SQL)
- [ ] Async/await patterns followed

### ✅ Cross-Platform
- [ ] Layout works on different screen sizes
- [ ] Feature tested on Windows (primary platform)
- [ ] No platform-specific code without documentation

### ✅ Performance
- [ ] No UI thread blocking
- [ ] Database queries complete within timeout
- [ ] Memory usage reasonable during operation
```

**Why**: Systematic validation ensures no quality dimensions are overlooked, provides repeatable testing process, documents testing results.

**Discovered**: During feature validation process development.

---

## Integration Testing Patterns

### End-to-End Workflow Validation

**Lesson**: Test complete user workflows, not just individual components.

**Pattern**:
```markdown
## E2E Test: Inventory Receive Workflow

**Scenario**: Operator receives parts into inventory

**Steps**:
1. Open Inventory tab
2. Enter Part Number: "ABC123"
3. Select Location: "RECEIVING"
4. Enter Quantity: 100
5. Select Operation: "100" (Receive)
6. Enter Notes: "Shipment #12345"
7. Click Save

**Expected Results**:
- ✅ Part number validated (no errors for valid format)
- ✅ Location dropdown shows only valid locations (RECEIVING, FLOOR, SHIPPING)
- ✅ Operation dropdown shows only valid operations (90, 100, 110)
- ✅ Save button disabled until all required fields filled
- ✅ Loading indicator shows during save
- ✅ Success message displayed after save
- ✅ Transaction logged in database with Operation 100 (not transaction type)
- ✅ Transaction type determined by workflow context (IN for receiving)
- ✅ Inventory quantity updated correctly
- ✅ Session history updated with new transaction

**Actual Results**: [Document results here]

**Status**: ⬜ Pass / ⬜ Fail
**Tester**: [Name]
**Date**: [Date]
```

**Why**: End-to-end testing validates that all components work together correctly, catches integration issues, mirrors real user behavior.

**Discovered**: Integration testing standardization during feature validation.

---

### Database Integration Validation

**Lesson**: Validate database operations with actual MySQL 5.7 server (MAMP), not mocks.

**Pattern**:
```markdown
## Database Integration Test

**Test**: Save Inventory with Stored Procedure

**Prerequisites**:
- MAMP MySQL 5.7 running on localhost:3306
- Database: mtm_wip_application
- Stored procedure: usp_SaveInventory exists

**Test Steps**:
1. Call InventoryService.SaveInventoryAsync() with valid data
2. Verify status = "SUCCESS"
3. Query database to confirm record created
4. Verify audit trail created in transaction log

**Expected**:
- ✅ Status returned is "SUCCESS"
- ✅ Error is null
- ✅ DataTable result contains expected columns
- ✅ Database record created with correct values
- ✅ Transaction log entry created
- ✅ Connection returned to pool (no leak)

**Validation SQL**:
```sql
SELECT * FROM Inventory 
WHERE PartNumber = 'ABC123' AND LocationCode = 'RECEIVING';

SELECT * FROM TransactionLog 
WHERE PartNumber = 'ABC123' 
ORDER BY TransactionDate DESC LIMIT 1;
```

**Status**: ⬜ Pass / ⬜ Fail
```

**Why**: Testing with actual database catches MySQL 5.7 specific issues, validates stored procedures work correctly, tests connection pooling behavior.

**Discovered**: During database integration testing development.

---

## Regression Testing Patterns

### Regression Test Suite Pattern

**Lesson**: Maintain regression test scenarios for critical workflows to ensure changes don't break existing functionality.

**Pattern**:
```markdown
## Regression Test Suite

### Critical Workflow 1: Inventory Receive
- **Last Tested**: 2025-10-10
- **Status**: ✅ Pass
- **Notes**: All 12 steps completed successfully

### Critical Workflow 2: Inventory Transfer
- **Last Tested**: 2025-10-10
- **Status**: ✅ Pass
- **Notes**: Transfer between locations working correctly

### Critical Workflow 3: Session History
- **Last Tested**: 2025-10-09
- **Status**: ⚠️ Needs Retest
- **Notes**: New changes may affect history display

### Critical Workflow 4: Quick Button Execution
- **Last Tested**: 2025-10-09
- **Status**: ✅ Pass
- **Notes**: 10 quick buttons limit enforced correctly
```

**Why**: Regression testing prevents breaking existing functionality, provides confidence in code changes, documents testing history.

**When to Run**:
- After refactoring existing code
- Before major releases
- After adding new features that touch shared components
- After dependency updates (NuGet packages)

**Discovered**: Regression prevention strategy during feature additions.

---

## Cross-Platform Testing Patterns

### Screen Size Validation

**Lesson**: Test layouts on different screen sizes and resolutions to ensure responsive design works.

**Pattern**:
```markdown
## Screen Size Test Matrix

### Resolution Testing

**1920x1080 (Full HD)**:
- [ ] All form fields visible without scrolling
- [ ] ManufacturingField icons properly sized
- [ ] Action bar buttons accessible
- [ ] No horizontal scrolling required

**1366x768 (Laptop)**:
- [ ] Vertical scrolling works smoothly
- [ ] No content cut off
- [ ] Form fields remain usable
- [ ] Buttons remain accessible

**1280x720 (Minimum Supported)**:
- [ ] All critical functions accessible
- [ ] Layout scales down appropriately
- [ ] No overlapping content
- [ ] Text remains readable

**High DPI (150%, 200% scaling)**:
- [ ] No blurry text or icons
- [ ] Layout maintains proper proportions
- [ ] Click targets remain appropriate size
```

**Why**: Cross-platform support requires responsive layouts that work across different screen configurations.

**Discovered**: During cross-platform testing initiative.

---

### Platform-Specific Testing

**Lesson**: Test on all target platforms to catch platform-specific issues.

**Pattern**:
```markdown
## Platform Test Matrix

### Windows 10/11 (Primary Platform)
- [ ] Application launches successfully
- [ ] MAMP MySQL connection works
- [ ] File paths resolve correctly
- [ ] Theme switching works (light/dark)
- [ ] Keyboard shortcuts functional

### macOS (Secondary Platform)
- [ ] Application launches successfully
- [ ] MySQL connection works
- [ ] File paths use proper separators
- [ ] Theme integrates with system preference
- [ ] Cmd key shortcuts work

### Linux (Tertiary Platform)
- [ ] Application launches successfully
- [ ] MySQL connection works
- [ ] Case-sensitive file paths handled
- [ ] Theme works correctly
- [ ] Keyboard shortcuts functional
```

**Why**: Platform-specific behaviors can cause issues that only appear on certain operating systems.

**Discovered**: Cross-platform compatibility testing.

---

## Performance Validation Patterns

### UI Responsiveness Testing

**Lesson**: Validate UI remains responsive during all operations.

**Pattern**:
```markdown
## UI Responsiveness Test

### Button Click Response Time
- **Target**: < 100ms perceived response
- **Test**: Click button, measure time until visual feedback
- **Results**:
  - Save button: [X]ms ✅ Pass
  - Cancel button: [X]ms ✅ Pass
  - Search button: [X]ms ✅ Pass

### Form Field Input Lag
- **Target**: No perceptible delay when typing
- **Test**: Type rapidly in TextBox, observe lag
- **Results**: No lag detected ✅ Pass

### Data Grid Scrolling
- **Target**: Smooth 60 FPS scrolling
- **Test**: Scroll through 1000+ row DataGrid
- **Results**: Smooth scrolling ✅ Pass

### Async Operation Blocking
- **Target**: UI never freezes during async operations
- **Test**: Trigger long-running operation, try to interact with UI
- **Results**: UI remains responsive, loading indicator shown ✅ Pass
```

**Why**: Poor UI responsiveness creates bad user experience. Sub-100ms response time feels instant to users.

**Discovered**: Performance requirement validation.

---

### Database Performance Testing

**Lesson**: Validate database operations complete within acceptable timeframes.

**Pattern**:
```markdown
## Database Performance Test

### Query Performance
- **Simple Query** (SELECT with WHERE clause):
  - Target: < 1 second
  - Actual: [X]ms
  - Status: ✅ Pass

- **Complex Report** (multiple JOINs, aggregations):
  - Target: < 10 seconds
  - Actual: [X] seconds
  - Status: ✅ Pass

- **Large Dataset** (1000+ rows):
  - Target: < 5 seconds
  - Actual: [X] seconds
  - Status: ✅ Pass

### Connection Pool Behavior
- **Concurrent Operations**: 50 simultaneous queries
  - Target: No timeouts, connections reused
  - Results: All queries completed, pool managed correctly ✅ Pass

- **Connection Leak Test**: 100 sequential operations
  - Target: No pool exhaustion
  - Results: All connections returned to pool ✅ Pass
```

**Why**: Database performance directly impacts user experience. 30-second timeout ensures operations complete within acceptable timeframes.

**Discovered**: Database performance monitoring implementation.

---

## Error Scenario Testing Patterns

### Error Handling Validation

**Lesson**: Test error scenarios explicitly to ensure graceful degradation.

**Pattern**:
```markdown
## Error Scenario Tests

### Database Offline
- **Test**: Stop MySQL server, attempt database operation
- **Expected**: User-friendly error message, no application crash
- **Result**: ✅ Pass - "Database connection failed" message shown

### Invalid Input
- **Test**: Enter invalid part number format (special characters)
- **Expected**: Validation error before save attempt
- **Result**: ✅ Pass - "Invalid part number format" message shown

### Network Interruption
- **Test**: Disconnect network during operation
- **Expected**: Retry logic activates, user notified
- **Result**: ✅ Pass - Retry attempted 3 times, timeout message shown

### Insufficient Permissions
- **Test**: User attempts operation without permission
- **Expected**: Clear permission denied message
- **Result**: ⬜ Not Tested - Permission system not yet implemented

### Data Validation Failures
- **Test**: Attempt to transfer more inventory than available
- **Expected**: Validation prevents operation, clear message
- **Result**: ✅ Pass - "Insufficient inventory" message shown
```

**Why**: Error scenarios are often overlooked but critical for application reliability and user experience.

**Discovered**: Error handling testing standardization.

---

## Manufacturing Domain Testing Patterns

### Operations vs Transaction Types Validation

**Lesson**: Explicitly test that operations (workflow sequence) and transaction types (movement intent) are handled correctly as independent concepts.

**Pattern**:
```markdown
## Manufacturing Domain Test

### Operation Number Validation
- **Test**: Select operation number in dropdown
- **Expected**: Only valid operations shown (90, 100, 110 from ValidOperations config)
- **Result**: ✅ Pass - Dropdown limited to configured operations

### Transaction Type Determination
- **Test**: Receive inventory (Operation 100)
- **Expected**: System determines transaction type as "IN" based on receive context
- **Result**: ✅ Pass - Transaction logged with Type="IN", Operation=100

- **Test**: Transfer inventory between locations (Operation 90)
- **Expected**: System determines transaction type as "TRANSFER"
- **Result**: ✅ Pass - Transaction logged with Type="TRANSFER", Operation=90

- **Test**: Ship inventory (Operation 110)
- **Expected**: System determines transaction type as "OUT" based on shipping context
- **Result**: ✅ Pass - Transaction logged with Type="OUT", Operation=110

### Critical Validation
- [ ] Operations indicate WHERE in workflow (sequence step: 10/20/30/90/100/110/120/130)
- [ ] Transaction Types indicate INTENT (movement direction: IN/OUT/TRANSFER)
- [ ] Both concepts work together correctly
- [ ] No confusion between operation numbers and transaction types in UI
```

**Why**: Operations and transaction types are distinct concepts that are often confused. Explicit testing ensures correct implementation.

**Discovered**: 2025-10-10 - Manufacturing domain clarification testing.

---

## Test Documentation Patterns

### Test Result Documentation

**Lesson**: Document test results with sufficient detail for reproducibility and debugging.

**Pattern**:
```markdown
## Test Execution Report

**Feature**: Inventory Receive Workflow
**Date**: 2025-10-10
**Tester**: [Name]
**Build**: v5.0.0-branch-001
**Environment**: Windows 11, MAMP MySQL 5.7

### Test Results Summary
- **Total Tests**: 15
- **Passed**: 14
- **Failed**: 1
- **Blocked**: 0
- **Pass Rate**: 93.3%

### Failed Tests

**Test ID: INV-003 - Save with Duplicate Part Number**
- **Expected**: Validation error "Part already exists in this location"
- **Actual**: No validation, duplicate created
- **Impact**: High - Data integrity issue
- **Root Cause**: Missing unique constraint check in stored procedure
- **Fix Required**: Update usp_SaveInventory to check for duplicates
- **Assigned To**: [Developer]

### Passed Tests
- [List of passed test IDs with brief description]

### Notes
- Performance excellent - all operations < 1 second
- UI responsive throughout testing
- No crashes or unexpected errors
```

**Why**: Detailed documentation enables debugging, tracks testing history, communicates results to team.

**Discovered**: Test documentation standardization.

---

## Common Testing Pitfalls

### Pitfall 1: Only Testing Happy Path

**Issue**: Testing only successful scenarios misses critical error handling bugs.

**Fix**: Create explicit test scenarios for error cases, validation failures, edge conditions.

---

### Pitfall 2: Not Testing on Target Platform

**Issue**: Testing only on development machine misses platform-specific issues.

**Fix**: Test on Windows, macOS, and Linux (or document platform-specific limitations).

---

### Pitfall 3: Assuming Success Without Validation

**Issue**: Clicking "Save" and assuming it worked without checking database.

**Fix**: Validate results in database, check transaction logs, verify audit trails.

---

### Pitfall 4: Not Testing with Realistic Data Volumes

**Issue**: Testing with 10 rows doesn't reveal performance issues with 10,000 rows.

**Fix**: Test with production-realistic data volumes (1000+ rows for DataGrid, large datasets for reports).

---

### Pitfall 5: Ignoring Manufacturing Domain Rules

**Issue**: Testing inventory operations without understanding operations vs transaction types distinction.

**Fix**: Create manufacturing domain-specific test scenarios validating operations (workflow sequence) and transaction types (movement intent) work correctly together.

---

## Memory File Maintenance

**Last Updated**: 2025-10-10  
**Maintainer**: GitHub Copilot (via user input)

**How to Add Lessons**:
1. Identify a recurring testing pattern or solved problem
2. Document the lesson with Pattern/Why/Discovered sections
3. Include test scenario examples for clarity
4. Commit changes to memory file

**Review Frequency**: After major testing cycles or when testing approaches evolve
