---
description: 'Generate XML documentation comments and update markdown documentation'
---

# Generate Documentation

Generate comprehensive XML documentation comments for code and update markdown documentation files following MTM standards.

## Prerequisites

- Code file or documentation file to update
- Understanding of code purpose and functionality
- Documentation standards from instructions

## User Input

```text
$ARGUMENTS
```

Parse arguments to extract:
- File path to document
- Documentation type (XML comments, README, markdown guide)
- Scope (entire file, specific class, method)
- Level of detail (brief, comprehensive)

If arguments are incomplete, prompt for:
1. What needs documentation?
2. Code or markdown documentation?
3. Entire file or specific sections?
4. Any special requirements?

## Documentation Types

### Type 1: XML Documentation Comments

Generate XML comments for C# code following .NET conventions.

### Type 2: README Files

Create or update README.md files for projects, features, or components.

### Type 3: Technical Guides

Create markdown documentation for technical concepts, patterns, or workflows.

### Type 4: API Documentation

Document service interfaces, methods, and contracts.

## XML Documentation Generation

### Step 1: Analyze Code Structure

Identify elements needing documentation:
- Public classes and interfaces
- Public methods and properties
- Complex internal methods
- Parameters and return values
- Exceptions thrown

### Step 2: Generate Class Documentation

```csharp
/// <summary>
/// {Brief description of class purpose}.
/// </summary>
/// <remarks>
/// {Additional context, usage notes, patterns, constraints}.
/// </remarks>
public class ClassName
{
}
```

### Step 3: Generate Method Documentation

```csharp
/// <summary>
/// {Brief description of what the method does}.
/// </summary>
/// <param name="paramName">{Description of parameter purpose and constraints}.</param>
/// <returns>{Description of return value}.</returns>
/// <exception cref="ExceptionType">{When this exception is thrown}.</exception>
/// <remarks>
/// {Additional context, side effects, performance notes}.
/// </remarks>
public async Task<ServiceResult> MethodNameAsync(string paramName)
{
}
```

### Step 4: Generate Property Documentation

```csharp
/// <summary>
/// Gets or sets {property description}.
/// </summary>
/// <value>{What the property represents}.</value>
[ObservableProperty]
private string _propertyName = string.Empty;
```

### Step 5: Generate Interface Documentation

```csharp
/// <summary>
/// Interface for {service purpose and responsibilities}.
/// </summary>
/// <remarks>
/// {Usage patterns, implementation notes, dependencies}.
/// </remarks>
public interface IServiceName
{
    /// <summary>
    /// {Method description}.
    /// </summary>
    Task<ServiceResult> MethodAsync();
}
```

## XML Documentation Examples

### ViewModel Documentation

```csharp
/// <summary>
/// ViewModel for inventory search functionality.
/// </summary>
/// <remarks>
/// This ViewModel manages the inventory search UI state and coordinates with
/// <see cref="IInventoryService"/> to retrieve and display inventory data.
/// Uses MVVM Community Toolkit patterns for property and command binding.
/// </remarks>
[ObservableObject]
public partial class InventorySearchViewModel : ObservableObject
{
    /// <summary>
    /// Gets or sets the search text for filtering inventory items.
    /// </summary>
    /// <value>The current search filter text.</value>
    [ObservableProperty]
    private string _searchText = string.Empty;

    /// <summary>
    /// Searches for inventory items matching the current search criteria.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This command executes the search operation and updates <see cref="SearchResults"/>.
    /// Handles errors gracefully and logs all operations.
    /// </remarks>
    [RelayCommand]
    private async Task SearchAsync()
    {
        // Implementation
    }
}
```

### Service Documentation

```csharp
/// <summary>
/// Service for inventory management operations including create, read, update, and delete.
/// </summary>
/// <remarks>
/// This service interacts with the database through stored procedures and implements
/// retry logic for transient failures. All operations are logged for audit purposes.
/// Connection pooling is managed automatically (MinPoolSize=5, MaxPoolSize=100).
/// </remarks>
public class InventoryService : IInventoryService
{
    /// <summary>
    /// Retrieves all inventory items for the specified location.
    /// </summary>
    /// <param name="locationCode">The location code to filter by (FLOOR, RECEIVING, SHIPPING).</param>
    /// <returns>
    /// A <see cref="ServiceResult{T}"/> containing the list of inventory items on success,
    /// or an error message on failure.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when locationCode is null or empty.</exception>
    /// <remarks>
    /// This method calls stored procedure usp_GetInventoryByLocation with a 30-second timeout.
    /// Results are converted from DataTable to strongly-typed list.
    /// Manufacturing domain context: Location codes must be valid DefaultLocations from configuration.
    /// </remarks>
    public async Task<ServiceResult<List<InventoryItem>>> GetInventoryByLocationAsync(string locationCode)
    {
        // Implementation
    }
}
```

### Custom Control Documentation

```csharp
/// <summary>
/// A collapsible panel control with header and expandable content.
/// </summary>
/// <remarks>
/// This control provides a card-based collapsible section with:
/// - Clickable header with expand/collapse icon
/// - Smooth expand/collapse animation
/// - Theme V2 integration for styling
/// - Material Design icons for visual indicators
/// 
/// Usage:
/// <code>
/// &lt;CollapsiblePanel HeaderText="Section Title" IsExpanded="True"&gt;
///     &lt;TextBlock Text="Content goes here"/&gt;
/// &lt;/CollapsiblePanel&gt;
/// </code>
/// </remarks>
public partial class CollapsiblePanel : UserControl
{
    /// <summary>
    /// Defines the <see cref="HeaderText"/> property.
    /// </summary>
    public static readonly StyledProperty<string> HeaderTextProperty = ...;

    /// <summary>
    /// Gets or sets the header text displayed in the panel header.
    /// </summary>
    /// <value>The header text. Default is "Header".</value>
    public string HeaderText
    {
        get => GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }
}
```

## README Documentation

### README Structure

```markdown
# {Project/Feature Name}

{Brief description of purpose and value}

## Features

- Feature 1: {Description}
- Feature 2: {Description}
- Feature 3: {Description}

## Prerequisites

- .NET 8.0 SDK
- MySQL 5.7 Server
- Visual Studio 2022 or VS Code

## Installation

```bash
# Step 1: Clone repository
git clone {repo-url}

# Step 2: Restore dependencies
dotnet restore

# Step 3: Configure database
# Edit Config/appsettings.json

# Step 4: Run application
dotnet run
```

## Configuration

{Configuration instructions}

## Usage

{Usage examples}

## Architecture

{Architecture overview}

## Contributing

{Contribution guidelines}

## License

{License information}
```

### README Examples

**Feature README**:
```markdown
# Inventory Search Feature

Real-time inventory search functionality with filtering, sorting, and export capabilities.

## Overview

This feature provides operators with fast, flexible inventory search across all manufacturing locations with real-time updates and comprehensive filtering options.

## User Stories

- US1: As an operator, I want to search inventory by part number
- US2: As an operator, I want to filter by location
- US3: As an operator, I want to export search results

## Technical Implementation

### Components

- `InventorySearchViewModel`: Manages search state and commands
- `InventorySearchView`: AXAML UI with data grid and filters
- `InventoryService.SearchInventoryAsync()`: Database operation

### Database

- Stored Procedure: `usp_SearchInventory`
- Parameters: SearchText, LocationCode, IncludeInactive
- Returns: DataTable with inventory records

### Configuration

```json
{
  "MTM": {
    "DefaultLocations": ["FLOOR", "RECEIVING", "SHIPPING"]
  }
}
```

## Testing

### Manual Validation

1. Search by part number - should return matching records
2. Filter by location - should show only that location's inventory
3. Export results - should generate CSV file

### Success Criteria

- Search completes in < 2 seconds
- Results accurate for all filters
- Export includes all filtered data
```

## Technical Guide Documentation

### Guide Structure

```markdown
# {Guide Topic}

{Introduction and purpose}

## Overview

{High-level explanation}

## Core Concepts

### Concept 1

{Explanation with examples}

### Concept 2

{Explanation with examples}

## Implementation Patterns

### Pattern 1

{Code examples and best practices}

## Common Scenarios

### Scenario 1

{Example implementation}

## Troubleshooting

### Issue 1

**Problem**: {Description}
**Solution**: {Resolution steps}

## References

- {Link to related documentation}
- {Link to external resources}
```

## Validation Checklist

After generating documentation, verify:

- [ ] All public APIs have XML comments
- [ ] Parameter descriptions are clear and accurate
- [ ] Return value descriptions specify possible outcomes
- [ ] Exception documentation lists all thrown exceptions
- [ ] Remarks section provides useful context
- [ ] Code examples are valid and compile
- [ ] Markdown follows consistent formatting
- [ ] Links are valid and accessible
- [ ] README includes all required sections
- [ ] No sensitive information exposed

## Success Criteria

✅ **Success** when:
- XML comments complete for all public members
- Documentation is clear and accurate
- Code examples are valid
- README provides complete overview
- Technical guides are comprehensive
- No documentation warnings on build

## Next Steps

After generating documentation:
1. Build project to verify XML comment warnings
2. Review documentation for accuracy
3. Test code examples
4. Update related documentation files
5. Commit documentation with descriptive message
