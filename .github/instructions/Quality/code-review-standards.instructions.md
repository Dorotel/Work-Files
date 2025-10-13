---
description: 'Code review standards and checklist for MTM application'
applyTo: '**/*.cs,**/*.axaml'
---

<!-- Based on patterns from: https://github.com/github/awesome-copilot -->

# Code Review Standards

## Overview

This file defines code review standards, checklists, and quality gates for the MTM WIP Application to ensure consistent code quality and adherence to MTM patterns.

## Core Principles

### Code Review Goals
- Ensure pattern compliance
- Catch bugs before production
- Share knowledge across team
- Maintain code quality standards
- Improve code readability

### Constructive Feedback
- Focus on code, not the person
- Explain reasoning behind suggestions
- Offer alternatives when critiquing
- Acknowledge good code
- Be respectful and professional

## General Code Quality Checklist

### Readability
- [ ] Code is easy to read and understand
- [ ] Variable and method names are descriptive
- [ ] Code follows consistent formatting
- [ ] Complex logic has explanatory comments
- [ ] No unnecessary complexity

### Correctness
- [ ] Code solves the intended problem
- [ ] No obvious bugs or logical errors
- [ ] Edge cases are handled
- [ ] Error handling is appropriate
- [ ] null checks where necessary

### Maintainability
- [ ] Code follows SOLID principles
- [ ] Methods are focused and single-purpose
- [ ] Proper separation of concerns
- [ ] DRY principle followed (Don't Repeat Yourself)
- [ ] Code is testable

## C# and .NET 8 Compliance

### Language Features
- [ ] Uses C# 12 features appropriately
- [ ] Leverages .NET 8 performance improvements
- [ ] File-scoped namespaces used
- [ ] Modern pattern matching where applicable
- [ ] Nullable reference types handled correctly

### Async/Await
- [ ] All I/O operations are async
- [ ] No .Result or .Wait() calls
- [ ] ConfigureAwait used appropriately
- [ ] Async method names end with "Async"
- [ ] CancellationToken support where appropriate

### Dependency Injection
- [ ] Constructor injection used for dependencies
- [ ] ArgumentNullException.ThrowIfNull() for validation
- [ ] Dependencies stored as private readonly fields
- [ ] Services registered in ServiceCollectionExtensions
- [ ] Interface-based dependencies, not concrete types

## MVVM Community Toolkit Compliance

### ObservableProperty Usage
- [ ] [ObservableProperty] attribute used, not manual INotifyPropertyChanged
- [ ] Partial classes declared correctly
- [ ] Property naming conventions followed (camelCase backing field)
- [ ] Property change methods implemented when needed
- [ ] NotifyCanExecuteChangedFor used correctly

### RelayCommand Usage
- [ ] [RelayCommand] attribute used, not manual ICommand
- [ ] Command method names match conventions
- [ ] CanExecute implemented when needed
- [ ] Async commands use proper async patterns
- [ ] Command parameters typed correctly

### No ReactiveUI
- [ ] No ReactiveObject or ReactiveCommand usage
- [ ] No this.RaiseAndSetIfChanged() calls
- [ ] All MVVM patterns use Community Toolkit only

## Avalonia UI Compliance

### AXAML Structure
- [ ] Grid used for complex layouts
- [ ] Theme V2 dynamic resources used for styling
- [ ] Proper binding syntax and paths
- [ ] No AVLN2000 binding errors
- [ ] Classes applied correctly (ManufacturingField, etc.)

### Data Binding
- [ ] Bindings reference correct ViewModel properties
- [ ] TwoWay binding used only when necessary
- [ ] Converters used for complex transformations
- [ ] RelativeSource bindings correct when used

### Code-Behind
- [ ] Minimal code-behind logic
- [ ] Business logic in ViewModels, not code-behind
- [ ] Proper DataContext setup
- [ ] Event handlers only for view-specific logic

## Database Compliance

### Stored Procedures
- [ ] All operations use stored procedures
- [ ] Helper_Database_StoredProcedure.ExecuteDataTableWithStatus used
- [ ] Parameters passed via Dictionary<string, object>
- [ ] Status and error return values handled
- [ ] No inline SQL (except simple queries if justified)

### Connection Management
- [ ] Connection pooling configuration correct
- [ ] Connections disposed properly with using statements
- [ ] Connection strings from configuration, not hardcoded
- [ ] Timeout values appropriate (30 seconds default)

### Error Handling
- [ ] Retry logic for transient failures
- [ ] Database errors logged appropriately
- [ ] User-friendly error messages
- [ ] Transactions used when appropriate

## Security Compliance

### Input Validation
- [ ] All user input validated
- [ ] Parameterized queries used
- [ ] No SQL injection vulnerabilities
- [ ] Validation errors displayed to user

### Sensitive Data
- [ ] No hardcoded credentials
- [ ] No sensitive data in logs
- [ ] Connection strings secured
- [ ] Error messages don't reveal internals

### Authentication/Authorization
- [ ] User permissions checked appropriately
- [ ] Session management secure
- [ ] Manufacturing operations authorized

## Performance Compliance

### Async Operations
- [ ] I/O operations are async
- [ ] UI thread not blocked
- [ ] Parallel operations used when appropriate
- [ ] Task.WhenAll for multiple async operations

### Database Performance
- [ ] Queries optimized
- [ ] Appropriate indexes used
- [ ] Result sets limited when appropriate
- [ ] Connection pooling utilized

### Memory Management
- [ ] IDisposable resources disposed
- [ ] No obvious memory leaks
- [ ] ObservableCollections sized appropriately
- [ ] Large objects cleared when not needed

## Testing Compliance

### Testability
- [ ] Code is written to be testable
- [ ] Dependencies injected, not hardcoded
- [ ] Business logic in services, not ViewModels
- [ ] Methods focused and single-purpose

### Manual Validation
- [ ] Success criteria defined
- [ ] Feature tested manually
- [ ] Error scenarios tested
- [ ] Cross-platform testing done where applicable

## Documentation Compliance

### XML Comments
- [ ] Public APIs have XML documentation
- [ ] Summary tags present and clear
- [ ] Parameter and return tags included
- [ ] Exception tags for thrown exceptions
- [ ] Remarks tag for additional context

### Code Comments
- [ ] Non-obvious logic explained
- [ ] Complex algorithms documented
- [ ] Business rules clarified
- [ ] No commented-out code
- [ ] TODO comments tracked

## Manufacturing Domain Compliance

### Operations Validation
- [ ] Operations validated against ValidOperations config
- [ ] Transaction types (IN/OUT/TRANSFER) used correctly
- [ ] Location codes validated against DefaultLocations
- [ ] Work order operations understood as sequence steps

### Session Management
- [ ] Session timeout respected (60 minutes)
- [ ] Quick buttons limited to 10 per user
- [ ] User preferences auto-saved
- [ ] Session data managed securely

## Git and Version Control

### Commit Quality
- [ ] Commit messages clear and descriptive
- [ ] Commits focused and atomic
- [ ] No unnecessary files committed
- [ ] .gitignore properly configured

### Branch Management
- [ ] Feature branch named appropriately
- [ ] No merge conflicts
- [ ] Branch up-to-date with master
- [ ] Clean commit history

## Review Process

### Before Requesting Review
1. Self-review code changes
2. Run application and test changes
3. Check for compilation errors and warnings
4. Review against this checklist
5. Update documentation if needed
6. Write clear pull request description

### During Review
1. Address all reviewer comments
2. Explain design decisions when questioned
3. Be open to suggestions
4. Make requested changes or discuss alternatives
5. Request re-review after changes

### After Review Approval
1. Verify all comments addressed
2. Ensure all checks pass
3. Merge using appropriate strategy
4. Delete feature branch after merge

## Common Issues to Watch For

### C# Issues
- Using .Result or .Wait() on async methods
- Not disposing IDisposable resources
- Hardcoded values that should be configuration
- Overly complex methods
- Poor naming conventions

### MVVM Issues
- Business logic in code-behind
- Not using MVVM Community Toolkit attributes
- ReactiveUI patterns (should be removed)
- Missing property change notifications
- Command CanExecute not implemented

### Avalonia UI Issues
- AVLN2000 binding errors
- Not using Theme V2 resources
- Hardcoded colors/styles
- Improper layout container usage
- Missing ClipToBounds or Margin settings

### Database Issues
- Not using stored procedures
- SQL injection vulnerabilities
- Not using connection pooling
- Blocking database calls
- Poor error handling

### Performance Issues
- Synchronous I/O operations
- Excessive database queries (N+1 problem)
- Memory leaks
- Inefficient LINQ queries
- Not using caching when appropriate

## Review Approval Criteria

### Must Pass
- No compilation errors
- No critical security vulnerabilities
- Follows MTM architectural patterns
- Passes manual validation testing
- Documentation updated

### Should Pass
- All checklist items addressed
- Performance acceptable
- Code readable and maintainable
- Follows style guidelines
- Proper error handling

## Continuous Improvement

### Learning from Reviews
- Track common issues
- Update instruction files based on findings
- Share lessons learned
- Improve review checklist over time
- Celebrate good code practices
