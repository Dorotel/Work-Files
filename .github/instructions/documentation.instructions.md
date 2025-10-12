---
description: 'Documentation standards for code comments, XML docs, and markdown files'
applyTo: '**/*.cs,**/*.md,**/*.axaml'
---

<!-- Based on patterns from: https://github.com/github/awesome-copilot -->

# Documentation Standards

## Overview

This file defines documentation standards for code comments, XML documentation, README files, and markdown documentation for the MTM WIP Application.

## Core Principles

### Documentation Serves Readers
- Write for developers who will maintain the code
- Explain "why", not "what"
- Keep documentation up-to-date with code changes
- Use clear, concise language

### Code is Self-Documenting
- Use descriptive names for classes, methods, and variables
- Code structure should reveal intent
- Comments explain non-obvious decisions
- Avoid stating the obvious

## XML Documentation Comments

### Required XML Comments
- All public classes, interfaces, and enums
- All public methods, properties, and events
- Complex internal methods when logic is non-obvious

### XML Comment Tags

#### Summary Tag
```
/// <summary>
/// Retrieves inventory items for the specified location code.
/// </summary>
```

#### Parameter Tags
```
/// <param name="locationCode">The location code to filter inventory items.</param>
/// <param name="includeInactive">Whether to include inactive items in results.</param>
```

#### Returns Tag
```
/// <returns>A list of inventory items matching the specified criteria.</returns>
```

#### Exception Tags
```
/// <exception cref="ArgumentNullException">Thrown when locationCode is null.</exception>
/// <exception cref="InvalidOperationException">Thrown when database connection fails.</exception>
```

#### Remarks Tag
```
/// <remarks>
/// This method queries the database using stored procedure usp_GetInventory.
/// Results are cached for 5 minutes to improve performance.
/// </remarks>
```

### Complete Example
```
/// <summary>
/// Saves inventory changes to the database.
/// </summary>
/// <param name="inventoryItem">The inventory item to save.</param>
/// <param name="cancellationToken">Cancellation token for async operation.</param>
/// <returns>A ServiceResult indicating success or failure.</returns>
/// <exception cref="ArgumentNullException">Thrown when inventoryItem is null.</exception>
/// <remarks>
/// This method validates the inventory item before saving and logs all operations.
/// If validation fails, no database operation is performed.
/// </remarks>
public async Task<ServiceResult> SaveInventoryAsync(
    InventoryItem inventoryItem,
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

## Code Comments

### When to Write Comments
- Explain non-obvious design decisions
- Document complex algorithms
- Clarify business rules
- Explain workarounds for known issues
- Provide context for future maintainers

### When NOT to Write Comments
- Don't state the obvious: `// Increment counter` → `counter++;`
- Don't comment bad code - refactor it instead
- Don't leave commented-out code - use version control
- Don't write comments that will become outdated

### Comment Style
```
// Single-line comments for brief explanations

// Multi-line comments for longer explanations that need
// multiple lines to fully describe the context and reasoning
// behind a particular implementation choice.

/* Block comments for very long explanations or when temporarily
   disabling code during debugging. Prefer // comments for production code. */
```

### TODO Comments
```
// TODO: Implement retry logic for transient database failures
// TODO: Add validation for date range inputs
// HACK: Workaround for Avalonia binding issue - remove when fixed in v11.4
// FIXME: Memory leak when loading large datasets - needs optimization
```

## README Documentation

### README.md Structure
1. **Project Title and Description**: Brief overview of application purpose
2. **Features**: Key capabilities and functionality
3. **Prerequisites**: Required software and tools
4. **Installation**: Step-by-step setup instructions
5. **Configuration**: How to configure the application
6. **Usage**: How to run and use the application
7. **Architecture**: High-level architectural overview
8. **Contributing**: Guidelines for contributors (if applicable)
9. **License**: License information

### README Example Sections

#### Installation Section
````markdown
## Installation

1. Clone the repository:
```bash
git clone https://github.com/org/MTM_WIP_Application_Avalonia.git
cd MTM_WIP_Application_Avalonia
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Configure database connection in `Config/appsettings.json`

4. Run the application:
```bash
dotnet run
```
````

#### Configuration Section
```markdown
## Configuration

Edit `Config/appsettings.json` to configure:
- Database connection string
- Logging settings
- Manufacturing domain settings (valid operations, locations)
- Session timeout and preferences
```

## Feature Specifications

### Specification Structure (.specify workflow)
- **spec.md**: User stories, acceptance criteria, success criteria
- **research.md**: Technical decisions and research findings
- **plan.md**: Implementation plan and architecture
- **tasks.md**: Task breakdown with dependencies
- **quickstart.md**: Setup and usage guide for feature

### User Story Format
```markdown
## User Story: [Title]

**As a** [user role]  
**I want** [capability]  
**So that** [benefit]

### Acceptance Criteria
- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

### Success Criteria
- SC-001: [Measurable outcome with target metric]
- SC-002: [Another measurable outcome]
```

## Inline Documentation

### AXAML Comments
```xml
<!-- Main content area with card-based layout -->
<Border Classes="Card" Padding="24">
    
    <!-- Form grid with fixed and expandable rows -->
    <Grid RowDefinitions="80,12,80,12,*">
        
        <!-- Part Number field -->
        <Border Grid.Row="0" Classes="ManufacturingField">
            ...
        </Border>
        
        <!-- Notes field - expandable -->
        <Border Grid.Row="4" Classes="ManufacturingField Notes">
            ...
        </Border>
        
    </Grid>
</Border>
```

### C# Region Usage (Sparingly)
```
#region Lifecycle Methods
// Constructor and initialization
#endregion

#region Command Handlers
// All RelayCommand method implementations
#endregion

#region Helper Methods
// Private helper methods
#endregion
```

Use regions sparingly - good file organization reduces need for regions.

## API Documentation

### Service Documentation
```
/// <summary>
/// Provides inventory management operations including create, read, update, and delete.
/// </summary>
/// <remarks>
/// This service interacts with the database through stored procedures and implements
/// retry logic for transient failures. All operations are logged for audit purposes.
/// </remarks>
public interface IInventoryService
{
    /// <summary>
    /// Retrieves all inventory items for the specified location.
    /// </summary>
    Task<List<InventoryItem>> GetInventoryByLocationAsync(string locationCode);
}
```

## Change Documentation

### CHANGELOG.md
- Track changes by version
- Categories: Added, Changed, Deprecated, Removed, Fixed, Security
- Date each release
- Follow semantic versioning

### Example
```markdown
# Changelog

## [5.0.0] - 2025-10-10

### Added
- Comprehensive GitHub Copilot configuration system
- 9 instruction files for code generation patterns
- 10 reusable prompts for component scaffolding

### Changed
- Updated MVVM patterns to use Community Toolkit 8.3.2
- Migrated from ReactiveUI to MVVM Community Toolkit

### Fixed
- Avalonia binding errors in DataGrid columns
- Memory leaks in session management
```

## Architecture Documentation

### Architecture Decision Records (ADR)
Document significant decisions:
- Context: What is the issue we're facing?
- Decision: What decision did we make?
- Rationale: Why did we make this decision?
- Consequences: What are the implications?
- Alternatives: What other options were considered?

### System Architecture Diagram
- Include high-level architecture diagrams
- Show component interactions
- Document data flow
- Explain technology choices

## Maintenance

### Keeping Documentation Current
- Update documentation in same commit as code changes
- Review documentation during code reviews
- Remove outdated documentation promptly
- Use automated tools to check for broken links

### Documentation Debt
- Track missing or outdated documentation
- Prioritize documentation for public APIs
- Schedule regular documentation review sessions
- Treat documentation gaps as technical debt
