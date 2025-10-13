---
description: 'Lessons learned and persistent knowledge for MVVM Community Toolkit 8.3.2 patterns and implementation'
---

# MVVM Patterns Memory

**Purpose**: Capture lessons learned, debugging patterns, and discoveries related to MVVM Community Toolkit 8.3.2 development in the MTM application.

**Usage**: This file is automatically referenced by GitHub Copilot to provide context-aware assistance for MVVM development. Add lessons as they are discovered during development.

---

## ObservableProperty Patterns

### Source Generator Naming Convention

**Lesson**: `[ObservableProperty]` generates public properties from private fields - field name minus underscore prefix becomes property name.

**Pattern**:
```csharp
[ObservableObject]
public partial class InventoryViewModel : ObservableObject
{
    // Private field with underscore
    [ObservableProperty]
    private string _partNumber = string.Empty;
    
    // Generated property (no underscore, PascalCase)
    // public string PartNumber { get; set; }
    
    [ObservableProperty]
    private int _quantity;
    
    // Generated: public int Quantity { get; set; }
}
```

**Why**: Source generator uses field name to create property name automatically. The underscore prefix is removed, first letter capitalized.

**Critical**: Always use camelCase with underscore prefix for backing fields: `_propertyName`.

**Discovered**: 2025-10-10 - Documented during MVVM Community Toolkit migration from ReactiveUI.

---

### Property Change Handlers

**Lesson**: Use partial methods `OnPropertyNameChanging` and `OnPropertyNameChanged` for property change logic.

**Pattern**:
```csharp
[ObservableProperty]
private string _partNumber = string.Empty;

// Called BEFORE property value changes
partial void OnPartNumberChanging(string value)
{
    // Validate before change
    // Log the change attempt
}

// Called AFTER property value changes
partial void OnPartNumberChanged(string value)
{
    // Update dependent properties
    // Trigger validation
    // Notify commands that CanExecute changed
    SaveCommand.NotifyCanExecuteChanged();
}
```

**Why**: Partial methods allow custom logic to run on property changes without losing source-generated code benefits.

**Use Cases**:
- Validation before/after property change
- Updating dependent properties
- Logging property changes
- Notifying commands to re-evaluate CanExecute

**Discovered**: During ViewModel refactoring - needed custom validation on property changes.

---

### NotifyPropertyChangedFor Attribute

**Lesson**: Use `[NotifyPropertyChangedFor]` to automatically notify when dependent properties should update.

**Pattern**:
```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(DisplayName))]
[NotifyPropertyChangedFor(nameof(IsValid))]
private string _firstName = string.Empty;

[ObservableProperty]
[NotifyPropertyChangedFor(nameof(DisplayName))]
[NotifyPropertyChangedFor(nameof(IsValid))]
private string _lastName = string.Empty;

// Computed property - updates when FirstName or LastName changes
public string DisplayName => $"{FirstName} {LastName}";

public bool IsValid => !string.IsNullOrWhiteSpace(FirstName) 
                    && !string.IsNullOrWhiteSpace(LastName);
```

**Why**: Eliminates manual `OnPropertyChanged(nameof(DisplayName))` calls. Source generator handles property change cascades automatically.

**Discovered**: During computed property implementation - discovered more elegant solution than manual notifications.

---

### NotifyCanExecuteChangedFor Attribute

**Lesson**: Use `[NotifyCanExecuteChangedFor]` to automatically update command CanExecute state when property changes.

**Pattern**:
```csharp
[ObservableProperty]
[NotifyCanExecuteChangedFor(nameof(SaveCommand))]
private string _partNumber = string.Empty;

[ObservableProperty]
[NotifyCanExecuteChangedFor(nameof(SaveCommand))]
private string _locationCode = string.Empty;

[RelayCommand(CanExecute = nameof(CanSave))]
private async Task SaveAsync()
{
    // Save logic
}

private bool CanSave()
{
    return !string.IsNullOrWhiteSpace(PartNumber) 
        && !string.IsNullOrWhiteSpace(LocationCode);
}
```

**Why**: Command buttons automatically enable/disable when properties change, without manual `SaveCommand.NotifyCanExecuteChanged()` calls.

**Discovered**: During command implementation - reduced boilerplate for CanExecute updates.

---

## RelayCommand Patterns

### Async Command Pattern

**Lesson**: Use `[RelayCommand]` on async Task methods to generate async relay commands.

**Pattern**:
```csharp
[RelayCommand]
private async Task LoadDataAsync()
{
    try
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        
        var result = await _service.GetDataAsync();
        
        if (result.IsSuccess)
        {
            Items.Clear();
            foreach (var item in result.Data)
            {
                Items.Add(item);
            }
        }
        else
        {
            ErrorMessage = result.ErrorMessage;
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error loading data");
        ErrorMessage = "An error occurred. Please try again.";
    }
    finally
    {
        IsLoading = false;
    }
}

// Generated: public IAsyncRelayCommand LoadDataCommand { get; }
```

**Why**: Async commands handle asynchronous operations properly, including cancellation support and completion tracking.

**Critical**: Always wrap service calls in try/catch/finally with proper error handling and loading indicators.

**Discovered**: Standard pattern for all async ViewMod operations.

---

### CanExecute Pattern

**Lesson**: Use `CanExecute` parameter in `[RelayCommand]` to control command availability.

**Pattern**:
```csharp
[RelayCommand(CanExecute = nameof(CanSave))]
private async Task SaveAsync()
{
    // Save logic
}

private bool CanSave()
{
    return !string.IsNullOrWhiteSpace(PartNumber) 
        && !string.IsNullOrWhiteSpace(LocationCode)
        && !IsLoading;
}
```

**XAML Binding**:
```xml
<Button Content="Save" 
        Command="{Binding SaveCommand}"
        IsEnabled="{Binding SaveCommand.CanExecute}"/>
```

**Why**: Buttons automatically disable when conditions aren't met, providing better UX and preventing invalid operations.

**Discovered**: User experience improvement - disabled buttons prevent error states.

---

### Command Parameters

**Lesson**: Use method parameters for commands that need context.

**Pattern**:
```csharp
[RelayCommand]
private async Task DeleteItemAsync(int itemId)
{
    var result = await _service.DeleteAsync(itemId);
    if (result.IsSuccess)
    {
        Items.RemoveAll(x => x.Id == itemId);
    }
}

// Generated: public IAsyncRelayCommand<int> DeleteItemCommand { get; }
```

**XAML Binding**:
```xml
<Button Content="Delete" 
        Command="{Binding DeleteItemCommand}"
        CommandParameter="{Binding ItemId}"/>
```

**Why**: Commands can operate on specific items without storing selection state in ViewModel.

**Discovered**: During list item operations implementation.

---

### Cancellable Async Commands

**Lesson**: Async commands support cancellation via CancellationToken parameter.

**Pattern**:
```csharp
[RelayCommand]
private async Task LongRunningOperationAsync(CancellationToken cancellationToken)
{
    try
    {
        IsProcessing = true;
        
        for (int i = 0; i < 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await Task.Delay(100, cancellationToken);
            Progress = i + 1;
        }
    }
    catch (OperationCanceledException)
    {
        _logger.LogInformation("Operation cancelled by user");
    }
    finally
    {
        IsProcessing = false;
    }
}

// Generated command has CancelCommand property
// LongRunningOperationCommand.Cancel() to cancel
```

**XAML Binding**:
```xml
<StackPanel>
    <Button Content="Start" Command="{Binding LongRunningOperationCommand}"/>
    <Button Content="Cancel" Command="{Binding LongRunningOperationCommand.CancelCommand}"/>
</StackPanel>
```

**Why**: Provides built-in cancellation support for long-running operations, improving responsiveness.

**Discovered**: During progress indicator implementation.

---

## ObservableObject Base Class

### Constructor Dependency Injection Pattern

**Lesson**: Always validate injected dependencies in constructor with `ArgumentNullException.ThrowIfNull()`.

**Pattern**:
```csharp
[ObservableObject]
public partial class InventoryViewModel : ObservableObject
{
    private readonly ILogger<InventoryViewModel> _logger;
    private readonly IInventoryService _inventoryService;
    private readonly IDatabaseService _databaseService;
    
    public InventoryViewModel(
        ILogger<InventoryViewModel> logger,
        IInventoryService inventoryService,
        IDatabaseService databaseService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(inventoryService);
        ArgumentNullException.ThrowIfNull(databaseService);
        
        _logger = logger;
        _inventoryService = inventoryService;
        _databaseService = databaseService;
    }
}
```

**Why**: Ensures dependencies are valid, prevents null reference exceptions, follows defensive programming principles.

**Critical**: This is a constitutional requirement for all ViewModels.

**Discovered**: Established pattern during dependency injection standardization.

---

### ObservableCollection Management

**Lesson**: Use `ObservableCollection<T>` for collections bound to UI - it automatically notifies on Add/Remove/Clear.

**Pattern**:
```csharp
[ObservableProperty]
private ObservableCollection<InventoryItem> _items = new();

[RelayCommand]
private async Task LoadItemsAsync()
{
    var result = await _service.GetItemsAsync();
    
    Items.Clear(); // UI updates automatically
    
    foreach (var item in result.Data)
    {
        Items.Add(item); // UI updates automatically for each add
    }
}
```

**Performance Consideration**:
For large collections, consider batching:
```csharp
Items.Clear();
var newItems = result.Data;
foreach (var item in newItems)
{
    Items.Add(item);
}
// Or use: Items = new ObservableCollection<T>(newItems);
```

**Why**: ObservableCollection implements INotifyCollectionChanged, enabling automatic UI updates when collection changes.

**Discovered**: Standard pattern for ListBox/DataGrid ItemsSource binding.

---

## ReactiveUI Migration Patterns

### ReactiveObject to ObservableObject

**Anti-Pattern** (ReactiveUI):
```csharp
public class InventoryViewModel : ReactiveObject
{
    private string _partNumber;
    public string PartNumber
    {
        get => _partNumber;
        set => this.RaiseAndSetIfChanged(ref _partNumber, value);
    }
}
```

**Correct Pattern** (MVVM Community Toolkit):
```csharp
[ObservableObject]
public partial class InventoryViewModel : ObservableObject
{
    [ObservableProperty]
    private string _partNumber = string.Empty;
    
    // Generated property automatically
}
```

**Why**: MVVM Community Toolkit 8.3.2 is the standard. ReactiveUI is deprecated in MTM application.

**Migration Steps**:
1. Change base class from `ReactiveObject` to `ObservableObject`
2. Add `[ObservableObject]` attribute to class
3. Make class `partial`
4. Replace property implementations with `[ObservableProperty]` fields
5. Replace `ReactiveCommand` with `[RelayCommand]`

**Discovered**: 2025-10-10 - Migration path documented during ReactiveUI elimination.

---

### ReactiveCommand to RelayCommand

**Anti-Pattern** (ReactiveUI):
```csharp
public ReactiveCommand<Unit, Unit> SaveCommand { get; }

public InventoryViewModel()
{
    SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, this.WhenAnyValue(x => x.CanSave));
}

private async Task SaveAsync()
{
    // Implementation
}
```

**Correct Pattern** (MVVM Community Toolkit):
```csharp
[RelayCommand(CanExecute = nameof(CanSave))]
private async Task SaveAsync()
{
    // Implementation
}

private bool CanSave()
{
    return !string.IsNullOrWhiteSpace(PartNumber);
}
```

**Why**: RelayCommand is simpler, has better tooling support, and integrates seamlessly with source generators.

**Discovered**: Command migration during ReactiveUI elimination.

---

## Error Handling Patterns

### Service Call Error Handling

**Lesson**: Always wrap service calls in try/catch/finally with loading indicators and user-friendly error messages.

**Pattern**:
```csharp
[ObservableProperty]
private bool _isLoading;

[ObservableProperty]
private string _errorMessage = string.Empty;

[RelayCommand]
private async Task SaveAsync()
{
    try
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        
        // Validation
        if (string.IsNullOrWhiteSpace(PartNumber))
        {
            ErrorMessage = "Part number is required";
            return;
        }
        
        // Service call
        var result = await _inventoryService.SaveAsync(PartNumber, Quantity);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("Inventory saved successfully");
            // Navigate or reload
        }
        else
        {
            ErrorMessage = result.ErrorMessage;
            _logger.LogWarning("Save failed: {Error}", result.ErrorMessage);
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error saving inventory");
        ErrorMessage = "An unexpected error occurred. Please try again.";
    }
    finally
    {
        IsLoading = false;
    }
}
```

**UI Binding**:
```xml
<StackPanel>
    <ProgressBar IsVisible="{Binding IsLoading}" IsIndeterminate="True"/>
    <TextBlock Text="{Binding ErrorMessage}" 
               IsVisible="{Binding ErrorMessage, Converter={x:Static StringConverters.IsNotNullOrEmpty}}"
               Classes="ErrorText"/>
</StackPanel>
```

**Why**: Provides clear user feedback, logs errors for debugging, prevents UI freezing during operations.

**Critical**: Never show technical error details (stack traces, SQL errors) to users - use user-friendly messages.

**Discovered**: Error handling standardization across all ViewModels.

---

## Validation Patterns

### Input Validation Pattern

**Lesson**: Validate input in ViewModel before calling services.

**Pattern**:
```csharp
private ValidationResult ValidateInput()
{
    if (string.IsNullOrWhiteSpace(PartNumber))
        return ValidationResult.Failure("Part number is required");
    
    if (PartNumber.Length > 50)
        return ValidationResult.Failure("Part number exceeds maximum length");
    
    if (!Regex.IsMatch(PartNumber, @"^[A-Z0-9-]+$"))
        return ValidationResult.Failure("Part number contains invalid characters");
    
    if (Quantity < 0)
        return ValidationResult.Failure("Quantity cannot be negative");
    
    return ValidationResult.Success();
}

[RelayCommand]
private async Task SaveAsync()
{
    var validation = ValidateInput();
    if (!validation.IsValid)
    {
        ErrorMessage = validation.ErrorMessage;
        return;
    }
    
    // Proceed with service call
}
```

**Why**: Prevents invalid data from reaching services, provides immediate user feedback, reduces unnecessary service calls.

**Discovered**: During input validation standardization.

---

## Common Pitfalls

### Pitfall 1: Forgetting [ObservableObject] Attribute

**Issue**: Properties don't notify UI of changes if [ObservableObject] attribute is missing.

**Fix**: Always add `[ObservableObject]` attribute to ViewModels and make them `partial class`.

---

### Pitfall 2: Not Making Class Partial

**Issue**: Source generators cannot add code if class isn't `partial`.

**Fix**: Declare ViewModel as `partial class`.

---

### Pitfall 3: Using ReactiveUI Patterns

**Issue**: ReactiveObject, ReactiveCommand, and `this.RaiseAndSetIfChanged()` are incompatible with MVVM Community Toolkit.

**Fix**: Migrate to `[ObservableObject]`, `[ObservableProperty]`, and `[RelayCommand]`.

---

### Pitfall 4: Business Logic in ViewModel

**Issue**: ViewModels should orchestrate, not implement business logic.

**Fix**: Move business logic to Services, keep ViewModel focused on UI state management.

**Pattern**:
```csharp
// ❌ Business logic in ViewModel
[RelayCommand]
private async Task CalculateInventoryValue()
{
    decimal totalValue = 0;
    foreach (var item in Items)
    {
        totalValue += item.Quantity * item.UnitPrice;
        // Complex calculation logic...
    }
    TotalInventoryValue = totalValue;
}

// ✅ Business logic in Service
[RelayCommand]
private async Task CalculateInventoryValue()
{
    var result = await _inventoryService.CalculateTotalValueAsync(Items.ToList());
    if (result.IsSuccess)
    {
        TotalInventoryValue = result.Data;
    }
}
```

---

### Pitfall 5: Not Validating Constructor Parameters

**Issue**: Null dependencies cause runtime exceptions.

**Fix**: Always use `ArgumentNullException.ThrowIfNull()` for all injected dependencies.

---

## Memory File Maintenance

**Last Updated**: 2025-10-10  
**Maintainer**: GitHub Copilot (via user input)

**How to Add Lessons**:
1. Identify a recurring MVVM pattern or solved problem
2. Document the lesson with Pattern/Why/Discovered sections
3. Include C# code examples for clarity
4. Commit changes to memory file

**Review Frequency**: After MVVM patterns evolve or when new Community Toolkit features are adopted
