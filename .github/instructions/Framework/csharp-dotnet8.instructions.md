---
description: 'C# and .NET 8 development guidelines for MTM manufacturing application'
applyTo: '**/*.cs'
---

<!-- Based on patterns from: https://github.com/github/awesome-copilot -->

# C# and .NET 8 Development Guidelines

## Overview

This file defines C# coding standards and .NET 8 patterns for the MTM WIP Application. All C# code must follow these guidelines to ensure consistency, maintainability, and alignment with modern .NET 8 best practices.

## Core Principles

### Single Target Framework
- Use .NET 8.0 as the single target framework
- No multi-targeting required for this application
- Leverage .NET 8-specific features and performance improvements

### Modern C# Language Features
- Use C# 12 language features where appropriate
- Prefer pattern matching over traditional if/else chains
- Use records for immutable data models
- Leverage file-scoped namespaces to reduce indentation
- Use global usings sparingly and document in `GlobalUsings.cs`

### Dependency Injection
- Use Microsoft.Extensions.DependencyInjection throughout
- Constructor injection is the preferred pattern
- Register services in `ServiceCollectionExtensions.cs`
- Use interface-based dependencies, not concrete implementations
- Validate constructor parameters with `ArgumentNullException.ThrowIfNull()`

### Async/Await Patterns
- All I/O operations must be asynchronous
- Use `async Task` for methods that don't return values
- Use `async Task<T>` for methods that return values
- Never use `.Result` or `.Wait()` - causes deadlocks
- Use `ConfigureAwait(false)` in library code, not in UI code (Avalonia UI runs on SynchronizationContext)

## Naming Conventions

### Classes and Interfaces
- PascalCase for class names: `InventoryService`, `MainWindowViewModel`
- Interfaces start with `I`: `IInventoryService`, `ILogger<T>`
- ViewModels end with `ViewModel`: `InventoryTabViewModel`
- Services end with `Service`: `DatabaseService`, `FileLoggingService`

### Methods and Properties
- PascalCase for public methods and properties: `GetInventoryAsync()`, `CurrentUser`
- PascalCase for private methods: `ValidateInput()`
- Async methods end with `Async`: `SaveInventoryAsync()`, `LoadDataAsync()`

### Fields and Parameters
- camelCase with underscore prefix for private fields: `_logger`, `_serviceProvider`
- camelCase for parameters: `userId`, `inventoryItem`
- Use `readonly` for fields that don't change after construction

### Constants
- PascalCase for constants: `MaxRetryAttempts`, `DefaultTimeout`
- Group related constants in static classes when appropriate

## MVVM Community Toolkit Integration

### ObservableProperty Pattern
- Use `[ObservableProperty]` attribute instead of manual INotifyPropertyChanged
- Generated properties are PascalCase, backing fields are camelCase with underscore
- Example: `[ObservableProperty] private string _userName;` generates `UserName` property

### RelayCommand Pattern
- Use `[RelayCommand]` attribute for command methods
- Async commands automatically supported with `[RelayCommand]` on async methods
- Generated command names: Method `SaveData()` → Command `SaveDataCommand`
- Can enable/disable commands with `CanExecute` parameter

### Never Use ReactiveUI
- Do NOT use `ReactiveObject`, `ReactiveCommand`, or `this.RaiseAndSetIfChanged()`
- MVVM Community Toolkit 8.3.2 is the standard for all MVVMpatterns
- If you see ReactiveUI patterns, refactor to MVVM Community Toolkit

## Project Structure

### ViewModels
- Inherit from `ObservableObject` base class
- Use MVVM Community Toolkit attributes: `[ObservableObject]`, `[ObservableProperty]`, `[RelayCommand]`
- Constructor injection for services
- Validate all dependencies in constructor

### Services
- Define service interfaces in same file or separate interface files
- Register in `ServiceCollectionExtensions.ConfigureServices()`
- Use ILogger<T> for logging
- Implement proper disposal patterns (IDisposable/IAsyncDisposable)

### Models
- Use records for DTOs and immutable data
- Use classes for mutable business entities
- Keep models simple - no business logic

### Configuration
- Use `IConfiguration` for appsettings.json access
- Strong-typed configuration sections using Options pattern
- Configuration files in `Config/` directory

## Error Handling

### Exception Management
- Use specific exception types, not generic `Exception`
- Validate parameters with `ArgumentNullException.ThrowIfNull()` or `ArgumentException`
- Log exceptions with full context using ILogger
- Use try-catch at service boundaries, not deeply nested
- Propagate exceptions to ViewModels for user-friendly error display

### Validation
- Input validation in ViewModels before calling services
- Business rule validation in Services
- Data validation in Models using Data Annotations where appropriate

## Logging Standards

### ILogger<T> Usage
- Inject `ILogger<T>` in all services and ViewModels
- Use structured logging with message templates
- Log levels: Debug, Information, Warning, Error, Critical
- Include contextual information in log messages

### Log Message Patterns
- Start messages with action verb: "Loading inventory...", "Saving changes..."
- Include relevant identifiers: user ID, transaction ID, etc.
- Never log sensitive data (passwords, connection strings)

## Performance Considerations

### Memory Management
- Dispose IDisposable resources properly
- Use `using` statements or `using` declarations
- Avoid holding references to large objects longer than necessary
- Use ValueTask<T> for hot paths that may complete synchronously

### Database Operations
- Use connection pooling (configured in appsettings.json)
- Always use parameterized queries (Dapper handles this)
- Set appropriate command timeouts
- Implement retry logic for transient failures

### UI Thread Management
- Don't block UI thread with long-running operations
- Use async/await for all I/O operations
- Marshal back to UI thread when updating observable properties

## Testing Approach

### Manual Validation Standards
- Define success criteria for each feature
- Test cross-platform (Windows, macOS, Linux where applicable)
- Validate both happy path and error scenarios
- Document test results in feature specifications

### Unit Testing (Future)
- While not currently implemented, code should be written to be testable
- Keep business logic in services, not ViewModels
- Use dependency injection for mockable dependencies

## Cross-Platform Considerations

### File Paths
- Use `Path.Combine()` for path construction
- Use `Path.DirectorySeparatorChar` for separators
- Handle case-sensitive file systems (Linux, macOS)

### Platform-Specific Code
- Use `RuntimeInformation.IsOSPlatform()` for platform detection
- Minimize platform-specific code
- Isolate platform-specific logic in dedicated services

## Avalonia UI Integration

### Code-Behind Patterns
- Minimize code-behind in Views (.axaml.cs files)
- Views should only handle view-specific logic (focus management, visual state)
- Business logic belongs in ViewModels
- Data binding is preferred over event handlers

### DataContext Management
- Set DataContext in XAML or constructor, not scattered through code
- ViewModels are resolved via dependency injection

## Security Best Practices

### SQL Injection Prevention
- Always use parameterized queries
- Dapper handles parameter binding securely
- Never concatenate user input into SQL strings

### Connection Strings
- Store connection strings in appsettings.json
- Never hardcode credentials
- Use secure configuration for production environments

## Documentation Standards

### XML Comments
- Public APIs must have XML documentation comments
- Include `<summary>`, `<param>`, `<returns>` tags
- Document exceptions with `<exception>` tags
- Use `<remarks>` for additional context

### Code Comments
- Comments explain "why", not "what"
- Avoid obvious comments
- Document complex algorithms or non-obvious logic

## File Organization

### Namespace Structure
- Follow folder structure: `MTM_WIP_Application_Avalonia.ViewModels`
- One primary class per file
- Nested classes only for private helper types

### Using Directives
- System namespaces first
- External library namespaces second
- Project namespaces last
- Alphabetical within each group
- Remove unused usings
