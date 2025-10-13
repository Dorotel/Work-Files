---
description: 'MVVM Community Toolkit 8.3.2 patterns for observable properties and commands'
applyTo: '**/*ViewModel.cs,**/ViewModels/**/*.cs'
---

<!-- Based on patterns from: https://github.com/github/awesome-copilot -->

# MVVM Community Toolkit 8.3.2 Guidelines

## Overview

This file defines MVVM patterns using MVVM Community Toolkit 8.3.2 for the MTM WIP Application. The toolkit provides source generators for ObservableProperty, RelayCommand, and other MVVM patterns, eliminating boilerplate code.

## Core Principles

### Source Generator-Based MVVM
- Use attributes that generate code at compile time
- No manual INotifyPropertyChanged implementation
- No manual ICommand implementation
- Generated code is efficient and maintainable

### Never Use ReactiveUI
- Do NOT use `ReactiveObject`, `ReactiveCommand`, or `this.RaiseAndSetIfChanged()`
- MVVM Community Toolkit 8.3.2 is the exclusive MVVM framework
- If ReactiveUI patterns are found, refactor to MVVM Community Toolkit

### Dependency Injection Integration
- All ViewModels registered with DI
- Constructor injection for services
- Validate dependencies in constructor

## ObservableObject Base Class

### ViewModel Base Pattern
```
[ObservableObject]
public partial class MyViewModel : ObservableObject
{
    private readonly ILogger<MyViewModel> _logger;
    private readonly IMyService _myService;
    
    public MyViewModel(ILogger<MyViewModel> logger, IMyService myService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(myService);
        
        _logger = logger;
        _myService = myService;
    }
}
```

### [ObservableObject] Attribute
- Apply to ViewModel classes
- Must be `partial class` to enable source generation
- Inherits from `ObservableObject` base class
- Generates INotifyPropertyChanged implementation automatically

## ObservableProperty Pattern

### Basic Property Declaration
```
[ObservableProperty]
private string _userName;

// Generated property: public string UserName { get; set; }
```

### Property Naming Convention
- Backing field: camelCase with underscore prefix (`_userName`)
- Generated property: PascalCase (`UserName`)
- Source generator handles naming conversion automatically

### Property Change Notifications
- Property change notifications generated automatically
- No need to call `OnPropertyChanged()` manually
- UI bindings update automatically when property changes

### Validation and Notification Attributes
- `[NotifyPropertyChangedFor(nameof(OtherProperty))]`: Notify when related property changes
- `[NotifyCanExecuteChangedFor(nameof(MyCommand))]`: Update command CanExecute state
- `[NotifyDataErrorInfo]`: Integrate with data validation

### Property Change Methods
- `partial void OnPropertyNameChanging(Type value)`: Called before property changes
- `partial void OnPropertyNameChanged(Type value)`: Called after property changes
- Implement these methods to add custom logic on property changes

## RelayCommand Pattern

### Synchronous Commands
```
[RelayCommand]
private void SaveData()
{
    // Command implementation
}

// Generated: public IRelayCommand SaveDataCommand { get; }
```

### Async Commands
```
[RelayCommand]
private async Task LoadDataAsync()
{
    // Async command implementation
}

// Generated: public IAsyncRelayCommand LoadDataAsyncCommand { get; }
```

### Command Naming Convention
- Method name: `SaveData` → Generated command: `SaveDataCommand`
- Method name: `LoadDataAsync` → Generated command: `LoadDataAsyncCommand`
- Always suffix method name with `Async` for async operations

### CanExecute Pattern
```
[RelayCommand(CanExecute = nameof(CanSaveData))]
private void SaveData()
{
    // Command implementation
}

private bool CanSaveData()
{
    return !string.IsNullOrEmpty(UserName);
}
```

### Command Parameter Binding
```
[RelayCommand]
private void DeleteItem(int itemId)
{
    // Command with parameter
}

// XAML: Command="{Binding DeleteItemCommand}" CommandParameter="{Binding ItemId}"
```

### Cancellable Async Commands
```
[RelayCommand]
private async Task LongRunningOperationAsync(CancellationToken cancellationToken)
{
    // Use cancellationToken for cancellation support
}

// Generated command supports cancellation via CancelCommand property
```

### NotifyCanExecuteChanged
- Call `CommandName.NotifyCanExecuteChanged()` when CanExecute conditions change
- Use `[NotifyCanExecuteChangedFor(nameof(CommandName))]` on properties that affect CanExecute
- Commands automatically re-evaluate CanExecute when notified

## Observable Collections

### ObservableCollection<T>
- Use `ObservableCollection<T>` for collections bound to UI
- Automatically notifies UI of collection changes (add, remove, clear)
- Suitable for ListBox, DataGrid, ComboBox ItemsSource binding

### Collection Initialization
```
[ObservableProperty]
private ObservableCollection<Item> _items = new();
```

### Collection Operations
- Add items: `Items.Add(newItem)`
- Remove items: `Items.Remove(item)`
- Clear collection: `Items.Clear()`
- UI updates automatically on all operations

## Messenger Pattern (Event Aggregation)

### When to Use Messenger
- Communication between loosely coupled ViewModels
- Publishing events from services to ViewModels
- Avoiding tight coupling between components

### Message Definition
```
public class DataChangedMessage : ValueChangedMessage<string>
{
    public DataChangedMessage(string value) : base(value) { }
}
```

### Sending Messages
```
[ObservableObject]
public partial class SenderViewModel : ObservableObject
{
    private readonly IMessenger _messenger;
    
    public SenderViewModel(IMessenger messenger)
    {
        _messenger = messenger;
    }
    
    [RelayCommand]
    private void SendMessage()
    {
        _messenger.Send(new DataChangedMessage("New Data"));
    }
}
```

### Receiving Messages
```
[ObservableObject]
public partial class ReceiverViewModel : ObservableObject, IRecipient<DataChangedMessage>
{
    private readonly IMessenger _messenger;
    
    public ReceiverViewModel(IMessenger messenger)
    {
        _messenger = messenger;
        _messenger.Register<DataChangedMessage>(this);
    }
    
    public void Receive(DataChangedMessage message)
    {
        // Handle message
    }
}
```

## Validation

### INotifyDataErrorInfo Support
- Use `[NotifyDataErrorInfo]` attribute on properties requiring validation
- Implement validation logic in property setters or validation methods
- UI controls display validation errors automatically

### Validation Attributes
- Data Annotations attributes work with MVVM Community Toolkit
- `[Required]`, `[Range]`, `[StringLength]`, etc.
- Validation errors propagate to UI automatically

## Dependency Injection Integration

### Service Registration
```
services.AddTransient<MyViewModel>();
services.AddTransient<MyService>();
```

### Constructor Injection in ViewModels
- Always use constructor injection for dependencies
- Validate parameters with `ArgumentNullException.ThrowIfNull()`
- Store dependencies as private readonly fields

### ViewModel Lifetime
- ViewModels typically registered as Transient (new instance per request)
- Singleton ViewModels for global state (rare)
- Scoped ViewModels for specific workflows

## Property Change Handling

### OnPropertyChanging and OnPropertyChanged
```
[ObservableProperty]
private string _userName;

partial void OnUserNameChanging(string value)
{
    // Called before property changes
    // Validate or log the change
}

partial void OnUserNameChanged(string value)
{
    // Called after property changes
    // Update dependent properties or execute logic
    SaveCommand.NotifyCanExecuteChanged();
}
```

### Cascading Property Updates
- Use `[NotifyPropertyChangedFor(nameof(DependentProperty))]` for dependent properties
- Chain notifications to update multiple UI elements
- Keep property change logic lightweight

## Performance Considerations

### Avoid Excessive Property Changes
- Batch property updates when possible
- Suspend change notifications during bulk updates (advanced scenarios)
- Keep property setters and change handlers fast

### Command Execution Performance
- Long-running operations should be async commands
- Show loading indicators during command execution
- Implement cancellation for user-interruptible operations

### Collection Performance
- Use ObservableCollection for small to medium collections (< 1000 items)
- Consider virtualization for large collections
- Avoid adding items to ObservableCollection in tight loops

## Error Handling in ViewModels

### Command Error Handling
```
[RelayCommand]
private async Task SaveDataAsync()
{
    try
    {
        await _service.SaveAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to save data");
        // Show error message to user
        ErrorMessage = "Failed to save data. Please try again.";
    }
}
```

### Validation Errors
- Display validation errors in UI with data binding
- Use `[NotifyDataErrorInfo]` for validation error propagation
- Provide clear error messages to users

## Testing ViewModels

### Unit Testing Best Practices
- ViewModels should be testable without UI
- Mock injected dependencies (services, loggers)
- Test property changes, command execution, and business logic
- Verify property change notifications if needed

### Test Structure Example
```
public class MyViewModelTests
{
    [Fact]
    public void SaveCommand_WhenUserNameEmpty_CannotExecute()
    {
        var logger = Mock.Of<ILogger<MyViewModel>>();
        var service = Mock.Of<IMyService>();
        var vm = new MyViewModel(logger, service);
        
        vm.UserName = string.Empty;
        
        Assert.False(vm.SaveCommand.CanExecute(null));
    }
}
```

## Anti-Patterns to Avoid

### Manual INotifyPropertyChanged
- Never implement INotifyPropertyChanged manually
- Always use `[ObservableProperty]` attribute

### ReactiveUI Patterns
- Never use ReactiveObject or ReactiveCommand
- Never use `this.RaiseAndSetIfChanged()`
- Refactor any ReactiveUI code to MVVM Community Toolkit

### Tight Coupling
- Don't reference other ViewModels directly
- Use Messenger pattern for cross-ViewModel communication
- Avoid circular dependencies between ViewModels

### Business Logic in ViewModels
- ViewModels orchestrate, Services implement business logic
- Keep ViewModels thin and focused on UI state management
- Complex logic belongs in Services

## Documentation Standards

### XML Comments for ViewModels
- Document ViewModel purpose and responsibilities
- Document properties that aren't self-explanatory
- Document commands and their side effects

### Command Documentation
- Explain what the command does
- Document any prerequisites or validation requirements
- Note any async operations or long-running tasks
