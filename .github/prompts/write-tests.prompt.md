---
description: 'Generate manual validation test scenarios with success criteria'
---

# Write Tests

Generate manual validation test scenarios following MTM testing standards with clear success criteria and validation workflows.

## Prerequisites

- Feature or component to test must be identified
- Acceptance criteria should be defined
- Test environment available

## User Input

```text
$ARGUMENTS
```

Parse arguments to extract:
- Feature or component name
- Test type (feature test, integration test, UI test)
- Acceptance criteria
- Expected behavior

If arguments are incomplete, prompt for:
1. What needs testing?
2. What are the acceptance criteria?
3. What should happen when it works correctly?
4. What error scenarios need testing?

## MTM Testing Approach

MTM uses **manual validation testing** with defined success criteria rather than automated unit tests.

### Testing Philosophy
- Manual testing through application execution
- Clear success criteria define expected outcomes
- Cross-platform validation on Windows, macOS, Linux
- Test both happy path and error scenarios

## Test Documentation Structure

### Test Specification Format

```markdown
# Test Specification: {Feature Name}

**Feature**: {Feature name}
**Date**: {Test date}
**Tester**: {Name}
**Platform**: {Windows/macOS/Linux}
**Build**: {Version/commit}

## Test Scenarios

### Scenario 1: {Scenario Name}

**Prerequisites**:
- {Required state or data}
- {Configuration requirements}

**Test Steps**:
1. {Action to perform}
2. {Action to perform}
3. {Action to perform}

**Expected Result**:
- {Expected outcome}
- {Expected behavior}

**Success Criteria**:
- [ ] {Measurable criterion}
- [ ] {Measurable criterion}

**Actual Result**:
{Fill during testing}

**Status**: [PASS/FAIL]

**Notes**:
{Observations, issues, deviations}

---

### Scenario 2: {Next scenario}
...
```

## Test Scenario Categories

### Feature Tests
Test complete user-facing features end-to-end.

### Integration Tests
Test interaction between components (ViewModel + Service + Database).

### UI Tests
Test Avalonia UI layouts, bindings, and theme integration.

### Error Handling Tests
Test system behavior under error conditions.

### Cross-Platform Tests
Test feature behavior across different operating systems.

### Performance Tests
Test responsiveness and resource usage.

## Creating Test Scenarios

### Step 1: Identify Test Cases

From acceptance criteria, extract testable scenarios:

**Example Acceptance Criteria**:
- ViewModel must use MVVM Community Toolkit patterns
- Database operations must use stored procedures
- UI must use Theme V2 resources
- Commands must handle errors gracefully

**Test Scenarios**:
1. Verify ViewModel uses [ObservableProperty]
2. Verify service calls usp_StoredProcedure
3. Verify UI uses DynamicResource
4. Verify command error handling

### Step 2: Define Success Criteria

Make criteria measurable and unambiguous:

**❌ Vague**: "Works well"
**✅ Specific**: "Search completes in < 2 seconds with correct results"

**❌ Vague**: "Looks good"
**✅ Specific**: "Uses ThemeV2.Surface.Background, switches correctly between light/dark modes"

**❌ Vague**: "Handles errors"
**✅ Specific**: "Logs error with LogError(), displays user-friendly message, returns ServiceResult.Failure()"

### Step 3: Create Test Steps

Write clear, repeatable actions:

```markdown
**Test Steps**:
1. Open Inventory Search view
2. Enter part number "ABC123" in search box
3. Click Search button
4. Observe results grid

**Expected Result**:
- Search completes within 2 seconds
- Results grid shows 3 items
- Each item displays PartNumber, Description, Quantity
- No errors in log
```

### Step 4: Define Edge Cases

Test boundary conditions and error scenarios:

```markdown
### Scenario: Search with No Results

**Test Steps**:
1. Enter non-existent part number "INVALID999"
2. Click Search
3. Observe results

**Expected Result**:
- Message displays "No results found"
- Grid is empty
- No errors logged
- Search button remains enabled

### Scenario: Search with Database Error

**Prerequisites**:
- Stop MySQL service

**Test Steps**:
1. Enter part number "ABC123"
2. Click Search
3. Observe behavior

**Expected Result**:
- Error message displays "Unable to connect to database"
- Error logged with context
- Application remains responsive
- User can retry after database restored
```

## Example Test Specifications

### ViewModel Test Specification

```markdown
# Test Specification: InventorySearchViewModel

## Scenario 1: ViewModel Code Generation

**Prerequisites**:
- ViewModel generated using `/setup-viewmodel`

**Test Steps**:
1. Open `ViewModels/InventorySearchViewModel.cs`
2. Review generated code

**Success Criteria**:
- [ ] Class has `[ObservableObject]` attribute
- [ ] Class is declared `partial class`
- [ ] Inherits from `ObservableObject`
- [ ] Constructor uses dependency injection
- [ ] All parameters validated with `ArgumentNullException.ThrowIfNull()`
- [ ] Properties use `[ObservableProperty]` attribute
- [ ] Commands use `[RelayCommand]` attribute
- [ ] Async operations use `async Task` pattern
- [ ] No ReactiveUI patterns (ReactiveObject, ReactiveCommand)
- [ ] ILogger injected and used
- [ ] XML documentation on public members
- [ ] File-scoped namespace used

**Status**: [PASS/FAIL]

## Scenario 2: Property Change Notifications

**Test Steps**:
1. Run application
2. Set breakpoint in property Changed handler
3. Modify property through UI
4. Observe notification

**Success Criteria**:
- [ ] OnPropertyChanged fires when property changes
- [ ] UI updates reflect property change
- [ ] Dependent properties update correctly

**Status**: [PASS/FAIL]

## Scenario 3: Command Execution

**Test Steps**:
1. Run application
2. Set breakpoint in command method
3. Click button bound to command
4. Observe execution

**Success Criteria**:
- [ ] Command executes when button clicked
- [ ] CanExecute controls button enabled state
- [ ] Async commands complete successfully
- [ ] Errors logged and handled gracefully

**Status**: [PASS/FAIL]
```

### Database Operation Test Specification

```markdown
# Test Specification: GetInventoryByLocation

## Scenario 1: Successful Data Retrieval

**Prerequisites**:
- MySQL running
- Test data in database

**Test Steps**:
1. Call `GetInventoryByLocationAsync("FLOOR")`
2. Observe results

**Success Criteria**:
- [ ] Uses Helper_Database_StoredProcedure.ExecuteDataTableWithStatus
- [ ] Calls stored procedure usp_GetInventoryByLocation
- [ ] Passes LocationCode parameter correctly
- [ ] Checks status == "SUCCESS"
- [ ] Handles error return value
- [ ] Logs operation entry and exit
- [ ] Returns ServiceResult<List<Inventory>>
- [ ] Converts DataTable to entity list
- [ ] Connection timeout set to 30 seconds

**Status**: [PASS/FAIL]

## Scenario 2: Invalid Location Code

**Test Steps**:
1. Call `GetInventoryByLocationAsync("INVALID")`
2. Observe behavior

**Success Criteria**:
- [ ] Validates location code before database call
- [ ] Returns ServiceResult.Failure() with clear message
- [ ] Logs warning about invalid location
- [ ] Does not call stored procedure

**Status**: [PASS/FAIL]

## Scenario 3: Database Connection Failure

**Prerequisites**:
- Stop MySQL service

**Test Steps**:
1. Call `GetInventoryByLocationAsync("FLOOR")`
2. Observe error handling

**Success Criteria**:
- [ ] Catches exception
- [ ] Logs error with full context
- [ ] Returns ServiceResult.Failure() with error message
- [ ] Application remains stable

**Status**: [PASS/FAIL]
```

### UI Test Specification

```markdown
# Test Specification: InventorySearchView

## Scenario 1: View Code Generation

**Prerequisites**:
- View generated using `/setup-view`

**Test Steps**:
1. Open `Views/InventorySearchView.axaml`
2. Review generated code

**Success Criteria**:
- [ ] UserControl declaration includes all namespaces
- [ ] x:DataType set to ViewModel type
- [ ] ScrollViewer wraps scrollable content
- [ ] Grid layouts use RowDefinitions/ColumnDefinitions
- [ ] Card styling applied to sections
- [ ] All backgrounds use ThemeV2.*.Background
- [ ] Material icons used for visual elements
- [ ] Data bindings use correct Mode
- [ ] No hardcoded colors or styles
- [ ] Code-behind file created with InitializeComponent()

**Status**: [PASS/FAIL]

## Scenario 2: Theme Switching

**Test Steps**:
1. Run application
2. View uses light theme initially
3. Switch to dark theme
4. Observe visual changes

**Success Criteria**:
- [ ] All backgrounds change to dark colors
- [ ] Text remains readable
- [ ] Borders visible in both themes
- [ ] Icons adapt to theme
- [ ] No visual glitches during switch

**Status**: [PASS/FAIL]

## Scenario 3: Data Binding

**Test Steps**:
1. Run application
2. Modify ViewModel properties
3. Observe UI updates

**Success Criteria**:
- [ ] No AVLN2000 binding errors
- [ ] UI updates when properties change
- [ ] TwoWay bindings update ViewModel
- [ ] Commands execute on button click

**Status**: [PASS/FAIL]
```

### Cross-Platform Test Specification

```markdown
# Test Specification: Cross-Platform Compatibility

## Scenario 1: Windows Testing

**Platform**: Windows 10/11

**Test Steps**:
1. Build and run application
2. Test all features
3. Verify file paths work correctly

**Success Criteria**:
- [ ] Application starts successfully
- [ ] All features function correctly
- [ ] File I/O works as expected
- [ ] Database connection successful
- [ ] Theme rendering correct

**Status**: [PASS/FAIL]

## Scenario 2: macOS Testing

**Platform**: macOS {version}

**Test Steps**:
1. Build and run application
2. Test all features
3. Verify file paths work correctly

**Success Criteria**:
- [ ] Application starts successfully
- [ ] All features function correctly
- [ ] File paths handle Unix separators
- [ ] Database connection successful
- [ ] Theme rendering correct

**Status**: [PASS/FAIL]

## Scenario 3: Linux Testing

**Platform**: Ubuntu {version}

**Test Steps**:
1. Build and run application
2. Test all features
3. Verify case-sensitive file system handling

**Success Criteria**:
- [ ] Application starts successfully
- [ ] All features function correctly
- [ ] File system case sensitivity handled
- [ ] Database connection successful
- [ ] Theme rendering correct

**Status**: [PASS/FAIL]
```

## Validation Checklist

After creating test scenarios, verify:

- [ ] All acceptance criteria have corresponding test scenarios
- [ ] Success criteria are measurable and unambiguous
- [ ] Test steps are clear and repeatable
- [ ] Edge cases and error scenarios included
- [ ] Prerequisites clearly stated
- [ ] Expected results defined
- [ ] Status tracking included
- [ ] Cross-platform testing considered

## Success Criteria

✅ **Success** when:
- Test scenarios cover all acceptance criteria
- Success criteria are measurable
- Test steps are repeatable
- Both happy path and error scenarios included
- Documentation clear and complete
- Ready for manual validation testing

## Next Steps

After creating test scenarios:
1. Execute test scenarios
2. Document actual results
3. Mark scenarios as PASS/FAIL
4. Report any failures
5. Update test scenarios based on findings
6. Archive test results for reference
