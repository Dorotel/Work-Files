# MTM_WIP_Application_Avalonia Development Guidelines

Auto-generated from all feature plans. Last updated: 2025-10-10

## Active Technologies
- .NET 8.0 + Avalonia UI 11.3.4 + MVVM Community Toolkit 8.3.2 + MySQL 5.7 (MySql.Data 9.4.0)
- GitHub Copilot VS Code Extension with comprehensive configuration system

## Core Instruction Files

GitHub Copilot automatically includes these instruction files for all code generation:

- #file:instructions/csharp-dotnet8.instructions.md
- #file:instructions/avalonia-ui.instructions.md
- #file:instructions/mvvm-community-toolkit.instructions.md
- #file:instructions/mysql-database.instructions.md
- #file:instructions/testing-standards.instructions.md
- #file:instructions/documentation.instructions.md
- #file:instructions/security-best-practices.instructions.md
- #file:instructions/performance-optimization.instructions.md
- #file:instructions/code-review-standards.instructions.md

## Memory Files

Persistent lessons learned from MTM development:

- #file:memory/avalonia-ui-patterns.md
- #file:memory/database-patterns.md
- #file:memory/mvvm-patterns.md
- #file:memory/testing-patterns.md

## Available Prompts

Use these prompts with `/` command prefix for rapid component scaffolding:

- `/setup-viewmodel` - Generate new ViewModel with MVVM Community Toolkit patterns
- `/setup-view` - Generate new Avalonia AXAML View with Theme V2 integration
- `/setup-service` - Generate new service with DI and logging
- `/setup-custom-control` - Generate Avalonia custom control
- `/database-operation` - Generate stored procedure execution code
- `/refactor-code` - Refactor code following MTM patterns
- `/generate-docs` - Generate XML comments and documentation
- `/debug-issue` - Debug workflow guidance
- `/write-tests` - Generate manual validation test scenarios
- `/create-stored-procedure` - Generate MySQL 5.7 stored procedure

## Available Chatmodes

Activate specialized chatmodes for context-aware assistance:

- **MTM Architect** - Architecture planning and service design
- **MTM Code Reviewer** - Pattern compliance checking
- **MTM Debugger** - Avalonia and MVVM troubleshooting
- **MTM Manufacturing Expert** - Manufacturing domain guidance
- **MTM Specify Integrator** - .specify workflow integration

## Project Structure
```
src/
  ViewModels/          # MVVM ViewModels with CommunityToolkit
  Views/               # Avalonia AXAML views
  Models/              # Data models and DTOs
  Services/            # Business logic services
  Controls/            # Custom Avalonia controls
  Converters/          # Value converters
  Behaviors/           # Avalonia behaviors
  Resources/Themes/    # Theme V2 system (17 theme files)
.github/
  instructions/        # Core instruction files (9 files)
  prompts/             # Reusable prompts (10 files)
  chatmodes/           # Specialized chatmodes (5 files)
  memory/              # Persistent lessons (4 files)
  workflows/           # CI/CD automation
specs/                 # Feature specifications (.specify workflow)
.specify/              # Specification system templates
```

## Commands

### Build and Run
```powershell
# Restore dependencies
dotnet restore

# Build application
dotnet build

# Run in development mode
dotnet run
```

### Database (MAMP MySQL 5.7)
```
Server: localhost:3306
Database: mtm_wip_application
Username: root
Password: root
Connection String: Server=localhost;Database=mtm_wip_application;SslMode=none;AllowPublicKeyRetrieval=true;
```

### Testing
```powershell
# Manual validation approach
# See testing-standards.instructions.md for success criteria patterns
```

## Code Style

- **C# .NET 8**: Follow csharp-dotnet8.instructions.md patterns
- **MVVM**: Use MVVM Community Toolkit 8.3.2 attributes ([ObservableObject], [ObservableProperty], [RelayCommand])
- **Avalonia UI**: Follow avalonia-ui.instructions.md with Theme V2 system
- **MySQL**: Use stored procedures with Helper_Database_StoredProcedure patterns
- **Testing**: Manual validation with success criteria (see testing-standards.instructions.md)

## .specify Workflow Integration

This project uses the .specify workflow for feature development with constitutional principles and comprehensive templates:

### Workflow Steps

1. **Specification**: `/speckit.specify` - Define feature requirements using structured templates
2. **Clarification**: `/speckit.clarify` - Resolve ambiguities through interactive Q&A
3. **Planning**: `/speckit.plan` - Create technical plan with architecture decisions
4. **Tasks**: `/speckit.tasks` - Generate detailed task breakdown with dependencies
5. **Implementation**: `/speckit.implement` - Execute tasks systematically with progress tracking

### Integration with GitHub Copilot Configuration

The .specify workflow leverages this GitHub Copilot configuration system:

- **Instruction Files**: Automatically applied during implementation to ensure pattern compliance
- **Memory Files**: Referenced during planning to incorporate lessons learned
- **Prompts**: Available for rapid component generation during implementation phase
- **Chatmodes**: Specialized assistance modes activate during specification, planning, and implementation
- **MTM Specify Integrator Chatmode**: Dedicated guidance for .specify workflow execution

### Constitutional Principles

The .specify workflow follows these constitutional principles:

1. **Requirements First**: All features begin with clear specifications in `specs/[feature]/spec.md`
2. **Research-Driven Planning**: Technical plans incorporate research from documentation, repositories, and existing patterns
3. **Task-Based Implementation**: Execution follows detailed task breakdowns with dependency tracking
4. **Progress Transparency**: Tasks marked complete as implemented, providing clear status visibility
5. **Validation Checkpoints**: Success criteria defined upfront, validation executed post-implementation

### Template System

Available templates in `.specify/templates/`:

- **spec.md**: User stories, acceptance criteria, success criteria, research findings
- **plan.md**: Architecture decisions, technical approach, dependency analysis
- **tasks.md**: Detailed task breakdown with phase organization and parallel execution markers
- **validation.md**: Comprehensive validation checklists with measurable success targets
- **migration.md**: Migration strategies, archive plans, rollback procedures

### Cross-References

- **Specification Phase**: Reference memory files for lessons learned, instruction files for current patterns
- **Planning Phase**: Leverage MTM Architect chatmode for architecture guidance
- **Implementation Phase**: Use component generation prompts (`/setup-viewmodel`, `/setup-view`, etc.)
- **Validation Phase**: Apply testing-standards.instructions.md and validation.md checklists
- **Review Phase**: Activate MTM Code Reviewer chatmode for pattern compliance checking

### Example: Feature Development Flow

```powershell
# 1. Specify feature (creates specs/[feature]/spec.md)
/speckit.specify "Add inventory export feature"

# 2. Clarify ambiguities (updates spec.md with Q&A)
/speckit.clarify

# 3. Plan implementation (creates specs/[feature]/plan.md)
/speckit.plan

# 4. Generate tasks (creates specs/[feature]/tasks.md)
/speckit.tasks

# 5. Implement systematically (follows tasks.md, marks complete)
/speckit.implement

# During implementation, GitHub Copilot automatically applies:
# - Instruction files for pattern compliance
# - Memory files for lessons learned
# - Component prompts for rapid scaffolding
# - Specialized chatmodes for domain assistance
```

See `.specify/templates/` for complete workflow documentation and constitutional principles.

## Recent Changes
- 001-setup-comprehensive-github: Comprehensive GitHub Copilot configuration system with 9 instruction files, 10 prompts, 5 chatmodes, 4 memory files, and CI/CD workflow

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
