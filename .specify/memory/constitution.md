<!--
=============================================================================
SYNC IMPACT REPORT - Constitution Update
=============================================================================
Version Change: 1.0.0 → 1.0.1 (PATCH)
Date: 2025-10-10

CHANGE SUMMARY:
- Type: PATCH - Documentation clarification and tooling integration
- Reason: Document GSC Enhancement System as compliance enforcement tooling

SECTIONS MODIFIED:
- Governance → Compliance Enforcement: Added GSC validation system documentation
- Development Workflow → Feature Development Process: Added GSC-enhanced workflow with fallback

PRINCIPLES UNCHANGED:
- Principle I: Code Quality Excellence (No changes)
- Principle II: Comprehensive Testing Standards (No changes)
- Principle III: User Experience Consistency (No changes)
- Principle IV: Performance Requirements (No changes)

GSC INTEGRATION DOCUMENTED:
- GSC validates all 4 constitutional principles automatically
- Provides safety mechanisms (checkpoint/rollback system)
- Enables progress visibility (status reporting with metrics)
- Offers workflow orchestration (guided feature development)
- Integrates memory system access (constitution and lessons learned)

GSC PROMPT ALIGNMENT STATUS:
✅ speckit.analyze.prompt.md - GSC-aligned (Phase 9.3 complete)
✅ speckit.checklist.prompt.md - GSC-aligned (Phase 9.3 complete)
✅ speckit.clarify.prompt.md - GSC-aligned (Phase 9.3 complete)
✅ speckit.constitution.prompt.md - GSC-aligned (Phase 9.3 complete)
✅ speckit.specify.prompt.md - GSC-aligned (Phase 9.3 complete)
✅ speckit.plan.prompt.md - GSC-aligned (Phase 9.3 complete)
✅ speckit.tasks.prompt.md - GSC-aligned (Phase 9.3 complete)
✅ speckit.implement.prompt.md - GSC-aligned (Phase 9.3 complete)
Status: 8/8 prompts GSC-aligned (100% Phase 9.3 completion)

TEMPLATES STATUS:
✅ plan-template.md - No changes required (constitutional alignment maintained)
✅ spec-template.md - No changes required (requirements sections unchanged)
✅ tasks-template.md - No changes required (task organization unchanged)
✅ All 8 speckit prompts - Now document GSC integration alongside legacy workflows

FOLLOW-UP ITEMS:
- ✅ No structural changes to constitutional principles
- ✅ GSC documented as recommended tooling, not required
- ✅ Backward compatibility maintained (fallback to PowerShell scripts preserved)
- ✅ Phase 9.3 complete - all prompts GSC-integrated

NEXT STEPS:
- Update AGENTS.md to reference GSC system (optional enhancement)
- No code changes required (clarification only, not redefinition)
- No migration plan needed (additive documentation only)
=============================================================================
-->

# MTM WIP Application (Avalonia) Constitution

## Core Principles

### I. Code Quality Excellence

The MTM WIP Application MUST maintain the highest standards of code quality to ensure reliability, maintainability, and long-term project sustainability.

**Non-Negotiable Requirements:**

- **Nullable Reference Types**: MUST be enabled across all project files (`<Nullable>enable</Nullable>` in .csproj). All code MUST handle nullability explicitly with proper null checks and null-forgiving operators only when provably safe.

- **MVVM Community Toolkit Patterns**: MUST use `CommunityToolkit.Mvvm` version 8.3.2+ exclusively for all MVVM implementations. Specifically:
  - Use `[ObservableProperty]` for bindable properties (never manual `INotifyPropertyChanged`)
  - Use `[RelayCommand]` for command implementations (never `ReactiveCommand` or manual `ICommand`)
  - Inherit from `ObservableObject` for ViewModels
  - Use `IAsyncRelayCommand` for async operations

- **Centralized Error Handling**: MUST use `Services.ErrorHandling.HandleErrorAsync()` for all error scenarios. Direct exception throwing or swallowing exceptions is prohibited except in:
  - Constructor argument validation (`ArgumentNullException.ThrowIfNull()`)
  - Critical infrastructure failures that cannot be recovered

- **Comprehensive Dependency Injection**: MUST use `Microsoft.Extensions.DependencyInjection` for all service instantiation. Manual `new` instantiation prohibited for:
  - Services (business logic layer)
  - ViewModels (presentation layer)
  - Database connections
  - Logger instances

**Rationale**: Manufacturing applications demand exceptional reliability. Null reference exceptions, inconsistent patterns, and poor error handling directly translate to production line downtime costing thousands of dollars per minute.

---

### II. Comprehensive Testing Standards

All features MUST be validated through comprehensive testing to prevent manufacturing disruptions and ensure cross-platform reliability.

**Non-Negotiable Requirements:**

- **Minimum 80% Code Coverage**: All new code MUST achieve minimum 80% line coverage. Critical paths (inventory transactions, database operations, manufacturing workflows) MUST achieve 95%+ coverage.

- **Test-Driven Development (TDD)**: For complex business logic and critical manufacturing operations:
  1. Write failing tests that capture requirements
  2. Obtain stakeholder/team approval of test scenarios
  3. Implement minimal code to pass tests
  4. Refactor while maintaining green tests
  
- **Cross-Platform Feature Testing**: All features MUST be validated on:
  - Windows (primary development platform)
  - macOS (Intel and Apple Silicon)
  - Linux (Ubuntu LTS)
  - Feature parity required across platforms
  
- **Manufacturing Domain Validation**: All manufacturing operations MUST have test coverage:
  - Operation codes (90=Move, 100=Receive, 110=Ship, 120=Transfer)
  - Location validation (FLOOR, RECEIVING, SHIPPING, custom locations)
  - Transaction types (IN, OUT, TRANSFER)
  - Session management (8+ hour manufacturing shifts)
  - Inventory accuracy (quantity tracking, part validation)

**Test Organization**:

```plaintext
tests/
├── unit/              # Component isolation testing (80% coverage minimum)
├── integration/       # Service interaction testing (cross-service)
├── contract/          # API/database contract testing
└── platform/          # Cross-platform compatibility testing
```

**Rationale**: Manufacturing environments cannot tolerate bugs. A failed inventory transaction can halt production lines. Cross-platform testing ensures operators on different terminals receive consistent, reliable functionality.

---

### III. User Experience Consistency

Operators working 8+ hour manufacturing shifts MUST have a consistent, intuitive, and responsive user interface across all platforms and sessions.

**Non-Negotiable Requirements:**

- **Avalonia UI 11.3.4+ Standards**: MUST use proper AXAML syntax with:
  - Compiled bindings where performance-critical (`x:CompileBindings="True"`)
  - Proper MVVM bindings (no code-behind logic except view-specific initialization)
  - ResourceDictionary usage for reusable styles
  - Theme V2 semantic token system (`Resources/ThemesV2/`)

- **Material Design Iconography**: MUST use Material Icons Avalonia (version 2.4.1+) for all UI icons. Custom icons prohibited unless Material Design lacks required icon. Icon consistency ensures intuitive recognition during rapid manufacturing operations.

- **Theme System Integration**: MUST support:
  - Light and Dark themes (Theme.Light.axaml, Theme.Dark.axaml)
  - Semantic token system (Tokens.axaml, Semantic.axaml)
  - Runtime theme switching without restart
  - High-contrast mode for accessibility

- **8+ Hour Session Responsiveness**: UI MUST remain responsive throughout extended manufacturing shifts:
  - No memory leaks accumulating over session duration
  - Consistent performance from session start to session end
  - Session timeout handling (60-minute inactivity) with graceful recovery
  - Auto-save mechanisms to prevent data loss during long operations

**UI Response Standards**:

- Button clicks: <100ms acknowledgment
- Data grid loading: <500ms for typical datasets (1000 rows)
- Form validation: Real-time (<50ms feedback)
- Navigation: <200ms between views

**Rationale**: Manufacturing operators perform repetitive tasks under time pressure. Inconsistent UI patterns cause errors. Slow response times frustrate operators and reduce throughput. Extended session stability prevents mid-shift disruptions.

---

### IV. Performance Requirements

The application MUST meet strict performance standards to support high-volume manufacturing operations without degradation.

**Non-Negotiable Requirements:**

- **Database Query Timeout**: 30-second maximum query execution time for all operations. Queries exceeding this limit MUST be:
  - Optimized with proper indexing
  - Refactored to use stored procedures
  - Paginated for large result sets
  - Reviewed for N+1 query patterns

- **MySQL Connection Pooling**: MUST maintain connection pool with:
  - Minimum: 5 connections (prevents exhaustion during idle periods)
  - Maximum: 100 connections (prevents database overload)
  - Connection lifetime: 5 minutes (prevents stale connections)
  - Retry policy: 3 attempts with exponential backoff

- **Sub-100ms UI Responsiveness**: All UI interactions MUST respond within 100ms:
  - Button clicks show immediate visual feedback
  - Text input appears without lag
  - Form validation provides real-time feedback
  - Navigation transitions feel instant

- **Cross-Platform Performance Parity**: Performance MUST be consistent across platforms:
  - Windows: Baseline reference platform
  - macOS: ±10% of Windows performance
  - Linux: ±10% of Windows performance
  - No platform should exhibit degraded user experience

**Performance Monitoring**:

- Database query logging with execution times
- UI responsiveness telemetry
- Memory usage tracking over session duration
- Connection pool utilization metrics

**Rationale**: Manufacturing operations require predictable performance. Database timeouts cause transaction failures. Slow UI reduces operator efficiency. Cross-platform consistency ensures seamless operator transitions between terminals.

---

## Technical Standards

### Technology Stack (Non-Negotiable)

- **.NET 8.0**: Single target framework (`<TargetFramework>net8.0</TargetFramework>`)
- **Avalonia UI 11.3.4+**: Cross-platform XAML framework
- **MVVM Community Toolkit 8.3.2+**: Source generator-based MVVM patterns
- **MySQL 9.4.0+**: Production database with 45+ stored procedures
- **Microsoft.Extensions 9.0.0+**: Dependency injection, logging, configuration
- **Material Icons Avalonia 2.4.1+**: Material Design iconography

### Architecture Patterns (Non-Negotiable)

- **MVVM Pattern**: Strict separation of Views, ViewModels, Models
- **Service Layer**: Business logic centralized in Services directory
- **Dependency Injection**: Constructor injection for all dependencies
- **Repository Pattern**: Database access abstracted through services
- **Event Aggregation**: Cross-component communication via events

### Security Standards

- **Connection String Encryption**: Database credentials MUST be encrypted in production
- **SQL Injection Prevention**: MUST use parameterized queries and stored procedures exclusively
- **Input Validation**: All user inputs MUST be validated at service layer before processing
- **Audit Logging**: All inventory transactions MUST be logged with user, timestamp, and operation details
- **Session Security**: Session tokens MUST be validated on every operation

### Database Standards

- **Stored Procedures**: Complex operations MUST use stored procedures (45+ existing procedures)
- **Transaction Management**: Multi-step operations MUST use database transactions
- **Connection Management**: MUST use `using` statements or connection pooling
- **Query Optimization**: All queries MUST have proper indexes and execution plans reviewed
- **Schema Versioning**: Database schema changes MUST be versioned and scripted

---

## Development Workflow

#### Feature Development Process

**GSC-Enhanced Approach** (Preferred when GSC available):

1. **Start Workflow**: `gsc workflow start <feature-name>` creates feature structure with constitutional guidance integrated
2. **Specification**: Generate spec with `gsc create spec` or manually in `.specify/specs/[###-feature-name]/spec.md` with constitutional alignment
3. **Planning**: `gsc workflow next` advances to planning phase with automatic validation gates for all 4 principles
4. **Test-First**: Write failing tests with safety checkpoint: `gsc rollback checkpoint "pre-implementation"` enables safe experimentation
5. **Implementation**: Develop with real-time validation: `gsc validate constitution` provides instant compliance feedback
6. **Testing**: Track coverage progress: `gsc status` displays compliance metrics for 80% minimum / 95% critical path requirements
7. **Review**: Generate automated constitutional compliance report for PR with `gsc validate constitution --IncludeTasks`
8. **Documentation**: Update with `gsc memory` integration for lessons learned capture

**Fallback Approach** (When GSC not available):

1. **Constitutional Review**: Review this constitution before starting feature work; ensure feature aligns with all 4 principles
2. **Specification**: Create `.specify/specs/[###-feature-name]/spec.md` with acceptance criteria mapped to constitutional requirements
3. **Planning**: Develop implementation plan in `plan.md` with explicit constitutional compliance checkpoints
4. **Test-First**: Write failing tests before implementation (TDD); validate tests cover constitutional requirements
5. **Implementation**: Develop feature following MVVM Community Toolkit patterns, Theme V2, async/await, parameterized queries
6. **Testing**: Execute tests, validate 80% minimum coverage (95% for critical paths); test on all platforms
7. **Review**: PR must pass code review checklist verifying constitutional compliance across all 4 principles
8. **Documentation**: Update relevant docs including lessons learned, patterns discovered, and constitutional alignment notes

### Code Review Requirements

All pull requests MUST pass:

- **Constitutional Compliance Check**: All four principles verified
- **Test Coverage Gate**: Minimum 80% line coverage (95% for critical paths)
- **Cross-Platform Validation**: Feature tested on Windows, macOS, Linux
- **Performance Benchmarks**: No degradation in database or UI responsiveness
- **Manufacturing Domain Validation**: Business rules verified for operations

### Branch Strategy

- **master**: Production-ready code only
- **Feature branches**: `[###-feature-name]` format (e.g., `001-inventory-transfer`)
- **Hotfix branches**: `hotfix/[description]` for critical production fixes

### Quality Gates

Before merging to master:

1. All tests pass on all platforms
2. Code coverage meets 80% minimum
3. No compiler warnings
4. Constitutional compliance verified
5. Performance benchmarks met
6. Documentation updated

---

## Governance

### Constitutional Authority

This Constitution supersedes all other development practices, guidelines, and instructions. When conflicts arise between this Constitution and other documentation, the Constitution takes precedence.

### Amendment Process

Constitutional amendments require:

1. **Proposal**: Written amendment with rationale submitted as pull request to `.specify/memory/constitution.md`
2. **Dual Approval**: Requires approval from:
   - Repository Owner
   - Lead Developer or designated Agent
3. **Review Period**: Minimum 5 business days for team review and feedback
4. **Version Update**: Constitution version MUST be incremented following semantic versioning:
   - **MAJOR**: Backward-incompatible changes (removing/redefining principles)
   - **MINOR**: New principles or materially expanded guidance
   - **PATCH**: Clarifications, wording improvements, non-semantic refinements
5. **Migration Plan**: If amendment impacts existing code, 30-day migration timeline with migration tasks
6. **Propagation**: Update all dependent templates, documentation, and instruction files

### Compliance Enforcement

- **GSC Validation System** (Recommended): Use `gsc validate constitution` for real-time compliance checking:
  - **Principle I (Code Quality)**: Validates nullable types, MVVM Community Toolkit patterns, centralized error handling, and dependency injection usage
  - **Principle II (Testing)**: Tracks 80% minimum coverage, 95% for critical paths, validates test organization and cross-platform testing
  - **Principle III (UX Consistency)**: Verifies Theme V2 usage, Material icons, x:DataType bindings, and 8+ hour session stability
  - **Principle IV (Performance)**: Monitors async operations, connection pooling configuration, query timeouts, and cross-platform performance parity
  - **Progress Tracking**: `gsc status` displays real-time compliance metrics, progress bars, and next steps
  - **Safety Mechanisms**: `gsc rollback checkpoint "<name>"` creates save points for safe experimentation; `gsc rollback restore <checkpoint>` undoes changes
  - **Memory Access**: `gsc memory get constitution` displays this constitution; `gsc memory search "<query>"` searches lessons learned
  - **Workflow Orchestration**: `gsc workflow start <feature>` guides through all 7 phases with automatic validation gates
  
- **CI/CD Integration**: Automated checks for constitutional compliance (test coverage, nullable types, patterns)
- **Code Review Checklists**: Review checklist includes constitutional principle verification (can use `gsc validate` output)
- **Quarterly Audits**: Comprehensive constitutional compliance audit every quarter
- **Exception Process**: Exceptions to constitutional requirements require:
  - Written justification documenting why simpler alternatives are insufficient
  - Repository Owner approval
  - Documented technical debt item with remediation plan

### Complexity Justification

Deviations from simplicity principles MUST be justified:

- **Additional Dependencies**: Why existing libraries insufficient?
- **Complex Patterns**: Why simpler patterns inadequate?
- **Performance Optimizations**: What performance problem necessitates complexity?
- **Architectural Deviations**: What constraint makes standard architecture impossible?

### Runtime Development Guidance

For detailed implementation guidance beyond constitutional principles, refer to:

- **AGENTS.md**: AI development automation tools and comprehensive instruction library
- **.github/instructions/**: 34+ specialized instruction files covering architecture, testing, patterns
- **README.md**: Quick start, setup, and operational documentation

---

**Version**: 1.0.1 | **Ratified**: October 9, 2025 | **Last Amended**: October 10, 2025
