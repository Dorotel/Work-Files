---
description: 'Refactor code to follow MTM patterns with pattern compliance checks'
---

# Refactor Code

Analyze and refactor existing code to follow MTM architectural patterns, MVVM Community Toolkit, Avalonia UI, and MySQL database standards.

## Prerequisites

- Code file or section to refactor must be specified
- Understanding of target patterns
- Ability to run tests after refactoring

## User Input

```text
$ARGUMENTS
```

Parse arguments to extract:
- File path to refactor
- Specific issues to address (optional)
- Scope (whole file, specific method, class)
- Pattern compliance target (MVVM, database, UI)

If arguments are incomplete, prompt for:
1. Which file needs refactoring?
2. What issues have you noticed?
3. Refactor entire file or specific section?
4. Which patterns should be enforced?

## Refactoring Workflow

### Step 1: Analyze Current Code

Scan the code for common anti-patterns:

**MVVM Anti-Patterns**:
- ✗ ReactiveUI usage (ReactiveObject, ReactiveCommand, this.RaiseAndSetIfChanged())
- ✗ Manual INotifyPropertyChanged implementation
- ✗ Manual ICommand implementation
- ✗ Business logic in code-behind
- ✗ Missing dependency injection

**Database Anti-Patterns**:
- ✗ Inline SQL queries
- ✗ String concatenation in SQL
- ✗ Not using stored procedures
- ✗ Synchronous database operations
- ✗ Missing error handling
- ✗ Hardcoded connection strings

**UI Anti-Patterns**:
- ✗ Hardcoded colors/styles
- ✗ Not using Theme V2 resources
- ✗ Missing ScrollViewer for scrollable content
- ✗ AVLN2000 binding errors
- ✗ Business logic in code-behind

**General Anti-Patterns**:
- ✗ Missing XML documentation
- ✗ Missing logging
- ✗ Using .Result or .Wait() on async
- ✗ Not validating constructor parameters

### Step 2: Create Refactoring Plan

Document what needs to change:
1. List anti-patterns found
2. Identify target patterns
3. Note dependencies to update
4. Plan breaking changes (if any)

### Step 3: Execute Refactoring

Apply changes systematically:

#### MVVM Refactoring

**Replace ReactiveUI with MVVM Community Toolkit**:

Before:
```csharp
public class MyViewModel : ReactiveObject
{
    private string _name;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
}
```

After:
```csharp
[ObservableObject]
public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
    
    [RelayCommand]
    private async Task SaveAsync()
    {
        // Implementation
    }
}
```

**Add Dependency Injection**:

Before:
```csharp
public MyViewModel()
{
    _service = new MyService();
}
```

After:
```csharp
private readonly ILogger<MyViewModel> _logger;
private readonly IMyService _service;

public MyViewModel(ILogger<MyViewModel> logger, IMyService service)
{
    ArgumentNullException.ThrowIfNull(logger);
    ArgumentNullException.ThrowIfNull(service);
    
    _logger = logger;
    _service = service;
}
```

#### Database Refactoring

**Replace inline SQL with stored procedures**:

Before:
```csharp
var sql = $"SELECT * FROM Inventory WHERE LocationCode = '{locationCode}'";
var result = await connection.QueryAsync<Inventory>(sql);
```

After:
```csharp
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
    _logger.LogError("Database operation failed: {Error}", error);
    return ServiceResult<List<Inventory>>.Failure(error ?? "Unknown error");
}
```

**Add async/await**:

Before:
```csharp
public List<Inventory> GetInventory(string location)
{
    // Synchronous database call
    return items;
}
```

After:
```csharp
public async Task<ServiceResult<List<Inventory>>> GetInventoryAsync(string location)
{
    _logger.LogInformation("Getting inventory for location {Location}", location);
    
    try
    {
        // Async database operation
        return ServiceResult<List<Inventory>>.Success(items);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to get inventory");
        return ServiceResult<List<Inventory>>.Failure(ex.Message);
    }
}
```

#### UI Refactoring

**Replace hardcoded styles with Theme V2**:

Before:
```xml
<Border Background="#FFFFFF" BorderBrush="#CCCCCC">
```

After:
```xml
<Border Background="{DynamicResource ThemeV2.Surface.Background}"
        BorderBrush="{DynamicResource ThemeV2.Border.Default}">
```

**Add x:DataType for binding validation**:

Before:
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             x:Class="Views.MyView">
```

After:
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             x:Class="Views.MyView"
             x:DataType="vm:MyViewModel">
```

**Fix binding errors**:

Before:
```xml
<TextBlock Text="{Binding SomeProperty}"/>  <!-- AVLN2000 error -->
```

After:
```csharp
// Add [ObservableProperty] in ViewModel
[ObservableProperty]
private string _someProperty = string.Empty;
```

#### Error Handling Refactoring

**Add comprehensive error handling**:

Before:
```csharp
public async Task SaveData()
{
    await _service.SaveAsync(data);
}
```

After:
```csharp
public async Task<ServiceResult> SaveDataAsync()
{
    try
    {
        _logger.LogInformation("Saving data");
        
        var result = await _service.SaveAsync(data);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("Data saved successfully");
        }
        else
        {
            _logger.LogWarning("Save failed: {Error}", result.ErrorMessage);
        }
        
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Exception during save operation");
        return ServiceResult.Failure($"Save failed: {ex.Message}");
    }
}
```

### Step 4: Update Dependencies

Update registrations and references:
- Register new services in DI
- Update using statements
- Add NuGet packages if needed
- Update related files

### Step 5: Add Documentation

Add or update:
- XML documentation comments
- Inline code comments for complex logic
- Update README if public API changed

### Step 6: Test Refactored Code

Validate changes:
1. Build project (check for compilation errors)
2. Run application
3. Test affected functionality
4. Check logs for proper logging
5. Verify no regressions

## Common Refactoring Scenarios

### Scenario 1: Convert ReactiveUI ViewModel

```csharp
// Before: ReactiveUI
public class OldViewModel : ReactiveObject
{
    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }
    
    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    
    public OldViewModel()
    {
        SearchCommand = ReactiveCommand.CreateFromTask(SearchAsync);
    }
}

// After: MVVM Community Toolkit
[ObservableObject]
public partial class NewViewModel : ObservableObject
{
    private readonly ILogger<NewViewModel> _logger;
    
    public NewViewModel(ILogger<NewViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    [RelayCommand]
    private async Task SearchAsync()
    {
        try
        {
            _logger.LogInformation("Searching: {SearchText}", SearchText);
            // Implementation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search failed");
        }
    }
}
```

### Scenario 2: Refactor Database Service

```csharp
// Before: Inline SQL
public class OldService
{
    public List<Item> GetItems(string location)
    {
        var sql = $"SELECT * FROM Items WHERE Location = '{location}'";  // SQL injection risk!
        return _connection.Query<Item>(sql).ToList();  // Synchronous
    }
}

// After: Stored procedure with async
public class NewService : INewService
{
    private readonly ILogger<NewService> _logger;
    private readonly string _connectionString;
    
    public NewService(ILogger<NewService> logger, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configuration);
        
        _logger = logger;
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not configured");
    }
    
    public async Task<ServiceResult<List<Item>>> GetItemsAsync(string location)
    {
        _logger.LogInformation("Getting items for location {Location}", location);
        
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "LocationCode", location }
            };

            var (result, status, error) = Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "usp_GetItemsByLocation",
                parameters,
                30
            );

            if (status != "SUCCESS")
            {
                _logger.LogError("Database operation failed: {Error}", error);
                return ServiceResult<List<Item>>.Failure(error ?? "Database error");
            }

            var items = ConvertDataTableToItems(result);
            return ServiceResult<List<Item>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get items");
            return ServiceResult<List<Item>>.Failure(ex.Message);
        }
    }
}
```

## Validation Checklist

After refactoring, verify:

- [ ] No compilation errors
- [ ] No ReactiveUI patterns remain
- [ ] MVVM Community Toolkit patterns used correctly
- [ ] All database operations use stored procedures
- [ ] Async/await used for I/O operations
- [ ] Dependency injection implemented
- [ ] Comprehensive error handling added
- [ ] Logging implemented throughout
- [ ] XML documentation added/updated
- [ ] No hardcoded styles in AXAML
- [ ] Theme V2 resources used
- [ ] Application runs successfully
- [ ] No regressions in functionality
- [ ] Tests pass (if applicable)

## Success Criteria

✅ **Success** when:
- Code follows all MTM patterns
- No anti-patterns remain
- Functionality preserved or improved
- Code is more maintainable
- Proper error handling and logging
- Ready for code review

## Next Steps

After refactoring:
1. Commit changes with descriptive message
2. Request code review
3. Document any breaking changes
4. Update related documentation
5. Monitor application for issues
