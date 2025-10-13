---
description: 'Manual validation and testing standards for MTM application'
applyTo: '**/*.cs,**/*.axaml'
---

<!-- Based on patterns from: https://github.com/github/awesome-copilot -->

# Testing Standards for MTM Application

## Overview

The MTM WIP Application uses manual validation testing approach. This file defines success criteria patterns, validation workflows, and quality assurance standards.

## Core Principles

### Manual Validation Approach
- No automated unit/integration tests currently implemented
- Manual testing through application execution
- Success criteria define expected outcomes
- Cross-platform validation on Windows, macOS, Linux where applicable

### Success Criteria Pattern
Every feature must define testable success criteria:
- Clear expected outcome
- Measurable validation method
- Pass/fail determination
- Test data and prerequisites

## Test Categories

### Feature Testing
- Verify feature matches specification requirements
- Test all user scenarios from acceptance criteria
- Validate both happy path and error scenarios
- Test edge cases and boundary conditions

### Integration Testing
- Verify service interactions work correctly
- Test database operations end-to-end
- Validate UI updates from data changes
- Test cross-component communication

### Cross-Platform Testing
- Test on Windows (primary platform)
- Test on macOS where applicable
- Test on Linux where applicable
- Verify layouts work across screen sizes

### Performance Testing
- Verify UI responsiveness (sub-100ms interactions)
- Test database query performance (30-second timeout)
- Monitor memory usage during extended sessions
- Validate startup time

## Success Criteria Examples

### ViewModel Generation Success
- ViewModel inherits from ObservableObject
- Uses [ObservableProperty] for properties
- Uses [RelayCommand] for commands
- Constructor uses dependency injection
- Logging implemented with ILogger<T>
- No compilation errors

### View Generation Success
- AXAML uses Grid layout patterns
- ScrollViewer wraps scrollable content
- Card-based section styling applied
- Theme V2 dynamic resources used
- Binds to ViewModel properties correctly
- No AVLN2000 binding errors

### Database Operation Success
- Uses Helper_Database_StoredProcedure.ExecuteDataTableWithStatus
- Handles status/error return values
- Implements retry logic for transient failures
- Logs operations appropriately
- Validates parameters before execution
- Connection pooling utilized

## Validation Workflow

### Pre-Implementation
1. Review feature specification
2. Identify success criteria
3. Prepare test data
4. Document test scenarios

### During Implementation
1. Test incrementally as code is written
2. Verify compilation after each change
3. Check for warnings and errors
4. Validate against success criteria continuously

### Post-Implementation
1. Execute complete feature test scenarios
2. Test error handling paths
3. Validate cross-platform compatibility
4. Document test results
5. Report any deviations from success criteria

## Test Documentation

### Test Result Format
```
Feature: [Feature Name]
Date: [Test Date]
Tester: [Name]
Platform: [Windows/macOS/Linux]

Test Scenario 1: [Scenario Name]
- Expected: [Expected outcome]
- Actual: [Actual outcome]
- Status: [PASS/FAIL]
- Notes: [Any observations]

Test Scenario 2: [Scenario Name]
...
```

### Success Criteria Tracking
- Track percentage of criteria met
- Goal: 95% success rate for code generation
- Document any failures and root causes
- Update instruction files based on recurring issues

## Quality Gates

### Code Quality
- No compilation errors
- No unhandled exceptions in normal operation
- Proper error handling and logging
- Follows MTM architectural patterns

### UI Quality
- No AVLN2000 binding errors
- Layouts work across different screen sizes
- Theme switching works correctly
- Proper focus management

### Database Quality
- All queries use stored procedures
- Connection pooling configured correctly
- Proper error handling and retry logic
- Transactions managed appropriately

## Regression Testing

### When to Regression Test
- After adding new features
- After refactoring existing code
- Before major releases
- When changing shared infrastructure

### Regression Test Checklist
- Verify existing features still work
- Test common workflows end-to-end
- Check for performance degradation
- Validate cross-platform compatibility

## Performance Validation

### UI Responsiveness
- Button clicks respond within 100ms
- Form field updates are instantaneous
- Data grid scrolling is smooth
- Application startup under 5 seconds

### Database Performance
- Simple queries complete under 1 second
- Complex reports complete under 10 seconds
- Connection acquisition under 500ms
- No connection pool exhaustion

### Memory Management
- No memory leaks during extended use
- Working set stays reasonable (< 500MB typical)
- GC pauses don't cause UI freezes

## Error Scenario Testing

### Database Failures
- Test with database offline
- Test with invalid connection string
- Test with query timeout
- Verify retry logic activates

### UI Error Handling
- Test with invalid user input
- Test with missing required fields
- Verify validation error display
- Test error message clarity

### Integration Failures
- Test with service unavailable
- Test with network interruptions
- Verify graceful degradation

## Documentation Standards

### Test Coverage Documentation
- Document which features have been tested
- Track success rates for validation
- Note any known issues or limitations
- Update after each testing cycle

### Issue Reporting
- Clear description of problem
- Steps to reproduce
- Expected vs actual behavior
- Environment details (OS, .NET version)
- Screenshots or logs if applicable

## Future Testing Enhancements

### Automated Testing (Future)
- Unit tests for ViewModels and Services
- Integration tests for database operations
- UI automation tests for workflows
- Load testing for performance validation

### Code Written for Testability
- Keep business logic in services
- Use dependency injection for mockable dependencies
- Avoid static dependencies
- Minimize side effects in methods
