---
description: 'Generate a new ViewModel following MTM MVVM Community Toolkit patterns'
---

# Setup ViewModel

Generate a complete ViewModel class following MTM architectural patterns with MVVM Community Toolkit 8.3.2.

## Prerequisites

- Target namespace must be specified
- ViewModel name must end with "ViewModel"
- Service dependencies must be identified

## User Input

```text
$ARGUMENTS
```

Parse arguments to extract:
- ViewModel name (e.g., `InventorySearchViewModel`)
- Target namespace (e.g., `MTM_WIP_Application_Avalonia.ViewModels`)
- Service dependencies (e.g., `IInventoryService`, `ILogger<T>`)

If arguments are incomplete, prompt for:
1. ViewModel name
2. Service dependencies (comma-separated)
3. Initial properties needed

## Implementation Steps

### Step 1: Create ViewModel File

Create file at `ViewModels/{ViewModelName}.cs` with:

1. **File header and namespace**:
   - File-scoped namespace declaration
   - Required using statements (MVVM Community Toolkit, services)

2. **Class declaration**:
   ```csharp
   [ObservableObject]
   public partial class {ViewModelName} : ObservableObject
   ```

3. **Dependency injection constructor**:
   - Inject `ILogger<{ViewModelName}>`
   - Inject required services
   - Validate all parameters with `ArgumentNullException.ThrowIfNull()`
   - Store dependencies as `private readonly` fields

### Step 2: Add Observable Properties

For each property needed:
- Use `[ObservableProperty]` attribute
- camelCase backing field with underscore prefix
- Add XML documentation comments
- Include property change methods if needed

Example:
```csharp
/// <summary>
/// Gets or sets the search text for inventory filtering.
/// </summary>
[ObservableProperty]
private string _searchText = string.Empty;
```

### Step 3: Add Commands

For each user action:
- Use `[RelayCommand]` attribute
- Async methods for I/O operations (suffix with `Async`)
- Implement `CanExecute` methods when needed
- Add XML documentation
- Include error handling with logging

Example:
```csharp
/// <summary>
/// Searches for inventory items matching the search criteria.
/// </summary>
[RelayCommand]
private async Task SearchInventoryAsync()
{
    try
    {
        _logger.LogInformation("Searching inventory with text: {SearchText}", SearchText);
        // Implementation
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to search inventory");
        // Handle error
    }
}
```

### Step 4: Add Property Change Handlers

If properties need custom logic on change:
```csharp
partial void OnSearchTextChanged(string value)
{
    // Update dependent properties or trigger commands
    SearchInventoryCommand.NotifyCanExecuteChanged();
}
```

### Step 5: Add Lifecycle Methods

If needed:
- Initialization logic
- Cleanup/disposal logic
- Navigation awareness

## Validation Checklist

Before completion, verify:

- [ ] File created in `ViewModels/` directory
- [ ] Class uses `[ObservableObject]` attribute
- [ ] Class is declared as `partial class`
- [ ] Constructor uses dependency injection
- [ ] All constructor parameters validated with `ArgumentNullException.ThrowIfNull()`
- [ ] Properties use `[ObservableProperty]` attribute
- [ ] Commands use `[RelayCommand]` attribute
- [ ] Async operations use proper `async Task` patterns
- [ ] No ReactiveUI patterns (ReactiveObject, ReactiveCommand)
- [ ] XML documentation comments on public members
- [ ] Error handling with ILogger
- [ ] File-scoped namespace used

## Anti-Patterns to Avoid

❌ **Do NOT**:
- Use manual `INotifyPropertyChanged` implementation
- Use ReactiveUI patterns (`ReactiveObject`, `ReactiveCommand`, `this.RaiseAndSetIfChanged()`)
- Use `.Result` or `.Wait()` on async operations
- Hardcode dependencies (use DI)
- Put business logic in ViewModel (belongs in Services)

## Success Criteria

✅ **Success** when:
- ViewModel compiles without errors
- Follows MVVM Community Toolkit 8.3.2 patterns
- Uses dependency injection correctly
- Includes proper error handling and logging
- No ReactiveUI patterns present
- Ready for View binding

## Example Output

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for inventory search functionality.
/// </summary>
[ObservableObject]
public partial class InventorySearchViewModel : ObservableObject
{
    private readonly ILogger<InventorySearchViewModel> _logger;
    private readonly IInventoryService _inventoryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventorySearchViewModel"/> class.
    /// </summary>
    public InventorySearchViewModel(
        ILogger<InventorySearchViewModel> logger,
        IInventoryService inventoryService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(inventoryService);

        _logger = logger;
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// Gets or sets the search text.
    /// </summary>
    [ObservableProperty]
    private string _searchText = string.Empty;

    /// <summary>
    /// Searches for inventory items.
    /// </summary>
    [RelayCommand]
    private async Task SearchAsync()
    {
        try
        {
            _logger.LogInformation("Searching inventory: {SearchText}", SearchText);
            var results = await _inventoryService.SearchInventoryAsync(SearchText);
            // Process results
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search failed");
        }
    }
}
```

## Next Steps

After creating the ViewModel:
1. Register ViewModel in `ServiceCollectionExtensions.cs` as Transient
2. Create corresponding View with `/setup-view`
3. Implement service methods if needed
4. Test ViewModel instantiation through DI
