---
description: 'Debugging workflow guidance for Avalonia, MVVM, and MySQL issues'
---

# Debug Issue

Provide systematic debugging guidance for common MTM application issues including Avalonia UI problems, MVVM binding errors, and MySQL database connectivity.

## Prerequisites

- Issue description or error message
- Context where issue occurs
- Steps to reproduce (if known)

## User Input

```text
$ARGUMENTS
```

Parse arguments to extract:
- Error message or issue description
- Component affected (UI, ViewModel, Service, Database)
- When does it occur (startup, specific action, intermittent)
- Any error codes (AVLN2000, SQL errors, exceptions)

If arguments are incomplete, prompt for:
1. What is the problem?
2. Where does it occur?
3. What error messages appear?
4. When did it start?
5. Can you reproduce it consistently?

## Debugging Workflow

### Step 1: Classify Issue Type

**UI Issues**:
- AVLN2000 binding errors
- Layout problems
- Theme not applying
- Controls not responding
- Visual glitches

**MVVM Issues**:
- Properties not updating
- Commands not executing
- Binding failures
- ViewModel not instantiating

**Database Issues**:
- Connection failures
- Stored procedure errors
- Timeout errors
- Data not loading

**Performance Issues**:
- Slow UI response
- Memory leaks
- High CPU usage

### Step 2: Gather Information

**Check logs**:
- Application logs in configured location
- Console output in debug mode
- Visual Studio Output window
- Avalonia DevTools console

**Check configuration**:
- appsettings.json values
- Connection strings
- MTM configuration section

**Check recent changes**:
- What was changed before issue appeared?
- New dependencies or updates?
- Configuration modifications?

### Step 3: Apply Debugging Strategy

Follow issue-specific debugging steps.

## Common Issue Patterns

### AVLN2000 Binding Error

**Symptoms**:
- "Cannot resolve symbol" in AXAML
- Bindings don't work
- Design-time errors

**Debugging Steps**:

1. **Check x:DataType declaration**:
```xml
<UserControl x:DataType="vm:MyViewModel">
    <TextBlock Text="{Binding PropertyName}"/>
</UserControl>
```

2. **Verify property exists in ViewModel**:
```csharp
[ObservableProperty]
private string _propertyName = string.Empty;
// Generates: public string PropertyName
```

3. **Check for computed properties**:
```csharp
// ❌ PROBLEMATIC: Computed property
public bool IsVisible => GetVisibility();

// ✅ CORRECT: ObservableProperty with backing field
[ObservableProperty]
private bool _isVisible = true;

public void UpdateVisibility()
{
    IsVisible = GetVisibility();
}
```

4. **Check namespace imports**:
```xml
xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
```

5. **Rebuild solution**:
- MVVM Community Toolkit uses source generators
- Sometimes clean build fixes binding issues

**Solution Pattern**:
- Replace computed properties with ObservableProperty
- Update backing properties explicitly when needed
- Ensure x:DataType matches ViewModel type

### ViewModel Properties Not Updating

**Symptoms**:
- UI doesn't reflect ViewModel changes
- Bindings seem to work initially but stop updating

**Debugging Steps**:

1. **Verify [ObservableProperty] attribute**:
```csharp
// ✅ CORRECT
[ObservableProperty]
private string _userName = string.Empty;

// ❌ WRONG
private string _userName = string.Empty;
public string UserName
{
    get => _userName;
    set => _userName = value;  // Missing notification!
}
```

2. **Check property change notifications**:
```csharp
partial void OnUserNameChanged(string value)
{
    // This should fire when property changes
    _logger.LogDebug("UserName changed to: {Value}", value);
}
```

3. **Verify binding mode**:
```xml
<!-- For user input, use TwoWay -->
<TextBox Text="{Binding UserName, Mode=TwoWay}"/>

<!-- For display only, OneWay is fine -->
<TextBlock Text="{Binding UserName}"/>
```

4. **Check for ReactiveUI remnants**:
- Replace `ReactiveObject` with `ObservableObject`
- Replace `this.RaiseAndSetIfChanged()` with `[ObservableProperty]`

### Commands Not Executing

**Symptoms**:
- Button click does nothing
- Command never fires

**Debugging Steps**:

1. **Verify [RelayCommand] attribute**:
```csharp
[RelayCommand]
private async Task SaveAsync()
{
    _logger.LogInformation("Save command executing");
    // Implementation
}
// Generates: public IAsyncRelayCommand SaveCommand
```

2. **Check command binding**:
```xml
<Button Content="Save" Command="{Binding SaveCommand}"/>
```

3. **Check CanExecute**:
```csharp
[RelayCommand(CanExecute = nameof(CanSave))]
private async Task SaveAsync()
{
    // Implementation
}

private bool CanSave()
{
    return !string.IsNullOrEmpty(UserName);
}
```

4. **Update CanExecute when conditions change**:
```csharp
partial void OnUserNameChanged(string value)
{
    SaveCommand.NotifyCanExecuteChanged();
}
```

5. **Check for exceptions**:
```csharp
[RelayCommand]
private async Task SaveAsync()
{
    try
    {
        _logger.LogInformation("Starting save");
        // Implementation
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Save failed");
        // Without this, exceptions may be silent!
    }
}
```

### Database Connection Failures

**Symptoms**:
- "Unable to connect to MySQL server"
- Timeout errors
- Authentication failures

**Debugging Steps**:

1. **Verify connection string**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mtm_wip_application;User=root;Password=root;SslMode=none;AllowPublicKeyRetrieval=true;"
  }
}
```

2. **Test MySQL service running**:
```powershell
# Check if MySQL is running
Get-Service *mysql*

# Test connection
mysql -h localhost -u root -p
```

3. **Check port availability**:
```powershell
# Check if port 3306 is listening
netstat -an | findstr :3306
```

4. **Verify connection pooling configuration**:
```json
{
  "Database": {
    "EnableConnectionPooling": true,
    "MinPoolSize": 5,
    "MaxPoolSize": 100,
    "ConnectionTimeout": 30
  }
}
```

5. **Test with simple query**:
```csharp
try
{
    using (var connection = new MySqlConnection(_connectionString))
    {
        await connection.OpenAsync();
        _logger.LogInformation("Connection successful, state: {State}", connection.State);
    }
}
catch (Exception ex)
{
    _logger.LogError(ex, "Connection failed: {Message}", ex.Message);
}
```

### Stored Procedure Errors

**Symptoms**:
- "Procedure not found"
- "Wrong number of arguments"
- Status returns "ERROR"

**Debugging Steps**:

1. **Verify stored procedure exists**:
```sql
SHOW PROCEDURE STATUS WHERE Db = 'mtm_wip_application';
```

2. **Check parameter names and types**:
```csharp
var parameters = new Dictionary<string, object>
{
    { "PartID", partId },           // Must match SP parameter name
    { "LocationCode", locationCode },
    { "UserID", userId }
};
```

3. **Check Helper_Database_StoredProcedure usage**:
```csharp
var (result, status, error) = Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    _connectionString,
    "usp_GetInventory",  // Exact SP name
    parameters,
    30  // Timeout in seconds
);

if (status != "SUCCESS")
{
    _logger.LogError("SP failed: {Error}", error);
    // Error contains detailed MySQL error message
}
```

4. **Enable MySQL query logging**:
- Check MySQL general log for actual queries
- Verify parameter values being passed

### Theme Not Applying

**Symptoms**:
- Controls have default appearance
- Theme V2 resources not resolving
- Colors wrong or missing

**Debugging Steps**:

1. **Check theme includes in App.axaml**:
```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://MTM_WIP_Application_Avalonia/Resources/ThemesV2/Combined.axaml"/>
</Application.Styles>
```

2. **Verify DynamicResource usage**:
```xml
<!-- ✅ CORRECT -->
<Border Background="{DynamicResource ThemeV2.Surface.Background}"/>

<!-- ❌ WRONG -->
<Border Background="#FFFFFF"/>
```

3. **Check resource key exists**:
- Look in ThemesV2 directory for resource definitions
- Verify exact resource key spelling

4. **Test theme switching**:
```csharp
// Should trigger theme updates
Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
```

### Memory Leaks

**Symptoms**:
- Memory usage grows over time
- Application slows down
- Eventually crashes

**Debugging Steps**:

1. **Check event handler cleanup**:
```csharp
public void Cleanup()
{
    // Unsubscribe from events
    _service.DataChanged -= OnDataChanged;
}
```

2. **Check collection growth**:
```csharp
// Clear collections when done
Items.Clear();

// Don't hold references longer than needed
_cachedData = null;
```

3. **Check IDisposable implementation**:
```csharp
public void Dispose()
{
    _disposableService?.Dispose();
    _connection?.Dispose();
}
```

4. **Use memory profiler**:
- Visual Studio Diagnostic Tools
- dotMemory
- PerfView

### Manufacturing Domain Issues

**Symptoms**:
- Invalid operation errors
- Transaction failures
- Location validation failures

**Debugging Steps**:

1. **Check operation validation**:
```csharp
// Operations are work order sequence steps, NOT transaction types
var validOps = _configuration.GetSection("MTM:ValidOperations").Get<List<string>>();
// ["90", "100", "110"]

if (!validOps.Contains(operationNumber))
{
    _logger.LogWarning("Invalid operation: {Op}", operationNumber);
}
```

2. **Check transaction type** (separate from operations):
```csharp
// Transaction types: IN, OUT, TRANSFER
var validTypes = new[] { "IN", "OUT", "TRANSFER" };
if (!validTypes.Contains(transactionType))
{
    _logger.LogWarning("Invalid transaction type: {Type}", transactionType);
}
```

3. **Check location codes**:
```csharp
var defaultLocations = _configuration.GetSection("MTM:DefaultLocations").Get<List<string>>();
// ["FLOOR", "RECEIVING", "SHIPPING"]
```

## Debugging Tools

### Avalonia DevTools

Enable in Program.cs:
```csharp
#if DEBUG
    .UseDevTools()
#endif
```

Press F12 while application running to open DevTools.

### Logging Analysis

Check logs for patterns:
```powershell
# Find errors in logs
Get-Content "path\to\log.txt" | Select-String "ERROR"

# Find specific operation
Get-Content "path\to\log.txt" | Select-String "GetInventory"
```

### Visual Studio Debugger

- Set breakpoints in ViewModels and Services
- Watch property values
- Step through command execution
- Inspect DataContext in XAML

## Validation Checklist

After debugging, verify:

- [ ] Root cause identified
- [ ] Fix applied
- [ ] Issue no longer reproduces
- [ ] No new issues introduced
- [ ] Logs show expected behavior
- [ ] Related functionality still works

## Success Criteria

✅ **Success** when:
- Issue no longer occurs
- Root cause understood
- Fix documented
- Preventive measures identified
- No regressions introduced

## Next Steps

After resolving issue:
1. Document solution for future reference
2. Add to memory files if pattern-related
3. Update instruction files if necessary
4. Consider adding validation to prevent recurrence
5. Update related documentation
