/speckit.specify Reverse-spec the existing dependency architecture of the MTM_WIP_Application_Avalonia codebase and produce Spec Kit artifacts we can use for planning and future implementation.

Context:
- Tech: C# .NET 8, Dependency Injection (Microsoft.Extensions.DependencyInjection), MVVM Community Toolkit, Services, Models, DTOs, Converters, Behaviors
- Repository: Dorotel/MTM_WIP_Application_Avalonia
- Goal: Read-only analysis of the architectural dependencies to create high-quality, structured specifications that describe services, models, DI patterns, business logic, and architectural rules.
- Timeline: 1-2 weeks
- Priorities: Migration planning, clean architecture establishment, dependency mapping, service contracts documentation
- Automation: Joyride scripts available for automated model switching and workflow orchestration (see .github/joyride/scripts/)

Clarified Decisions:
1. Scope: Services, Models/DTOs, Converters, Behaviors, Extensions, Core utilities, Configuration management
2. Service interfaces: Document interface contracts AND implementation details
3. DI registration: Document ServiceCollection configuration and lifetime scopes
4. Dependencies: Map all constructor injection dependencies and service usage
5. Business logic: Capture method signatures, responsibilities, and orchestration patterns
6. Data flow: Track data transformations from database through services to ViewModels
7. Configuration: Document appsettings.json structure and configuration binding
8. Architectural patterns: Identify repository patterns, service layer patterns, helper utilities

Deliverables (create these files):
1) Top-level feature spec
   - Path: specs/000-dependency-capture/spec.md
   - Contents:
     - Feature overview: "Capture current dependency architecture of MTM WIP Application"
     - Objectives & non-goals
     - High-level architecture diagram (textual)
     - Dependency injection patterns and conventions
     - Service layer patterns and boundaries
     - Assumptions and open questions
     - Acceptance criteria for "Dependency capture complete"

2) Dependency inventory
   - Path: specs/000-dependency-capture/dependencies/dependency-inventory.md
   - Table with columns:
     - Component Name
     - Type (Service/Model/Converter/Behavior/Extension/Core)
     - Primary file path
     - Interface (if applicable)
     - Dependencies (constructor parameters)
     - DI Lifetime (Singleton/Scoped/Transient)
     - Registered in (ServiceCollectionExtensions/Startup)
     - Consumers (who uses this)
     - Purpose summary
     - Notes/quirks/edge cases
   - Include:
     - All Services/ directory components
     - All Models/ directory types
     - All Converters/ directory classes
     - All Behaviors/ directory classes
     - All Extensions/ directory classes
     - Core/ directory utilities
     - Configuration management classes

3) Per-component specifications (one folder per major component)
   - Path (for each component): specs/000-dependency-capture/dependencies/{ComponentName}/spec.md
   - Use the "Dependency Spec Template" below for each file.
   - Populate from code: interface contracts, method signatures, dependencies, DI registration, usage patterns
   - For service layer components:
     - Document interface contracts (public methods, properties, events)
     - Map all constructor dependencies
     - Document DI lifetime and registration location
     - Identify all consumers (ViewModels, other services)
     - Capture business logic responsibilities (NO code examples needed)
   - For models and DTOs:
     - Document properties and their purposes
     - Identify validation rules
     - Map transformations (where created/consumed)
   - For converters and behaviors:
     - Document conversion logic purpose
     - Identify XAML usage patterns
     - Map dependencies if any

4) Global architecture guide
   - Path: specs/000-dependency-capture/dependencies/global-architecture.md
   - Structure:
     - **Dependency Injection Patterns**:
       - ServiceCollectionExtensions registration patterns
       - Lifetime scope conventions (when to use Singleton/Scoped/Transient)
       - Constructor injection best practices
       - Service locator anti-patterns (if any)
     - **Service Layer Architecture**:
       - Service boundaries and responsibilities
       - Inter-service communication patterns
       - Data access patterns (repository pattern usage)
       - Business logic organization
     - **Configuration Management**:
       - appsettings.json structure
       - Configuration binding patterns
       - Environment-specific configuration
     - **Common Patterns**:
       - Error handling patterns
       - Logging patterns (ILogger<T> usage)
       - Async/await conventions
       - Result/error return patterns (ServiceResult<T>)
     - **Data Flow**:
       - Database → Service → ViewModel flow
       - Model transformations and mapping
       - DTO usage patterns

5) Checklist and gaps
   - Path: specs/000-dependency-capture/dependencies/dependency-audit-checklist.md
   - Structure:
     - **Static Core Sections**:
       - Services inventory completion
       - Models/DTOs documentation status
       - DI registration mapping coverage
       - Interface contract documentation
       - Dependency graph completeness
     - **Dynamic Subsections** (populated from findings):
       - Discovered services and their completion status
       - Missing interface definitions
       - Circular dependency issues
       - Service locator anti-pattern usage
       - Unregistered dependencies
       - Missing or inconsistent error handling
   - "Gaps & ambiguities" section listing unknowns or missing code references.
   - Progress metrics: X of Y components documented, completion percentage per section

Scope & process:
- Search patterns: 
  - Services: Services/**/*.cs
  - Models: Models/**/*.cs
  - Converters: Converters/**/*.cs
  - Behaviors: Behaviors/**/*.cs
  - Extensions: Extensions/**/*.cs
  - Core: Core/**/*.cs
  - Configuration: Config/**/*.json, **/appsettings*.json
- For each component:
  - Identify interface contracts and implementations
  - Map constructor dependencies
  - Find DI registration in ServiceCollectionExtensions.cs
  - Document DI lifetime (Singleton/Scoped/Transient)
  - Identify all consumers (search for usages)
  - Capture method signatures and responsibilities (NO code examples)
  - Document error handling patterns
  - Document async patterns
  - Map data transformations
  - Identify configuration dependencies
- Do not modify code—read and extract only.
- NO code examples in specifications - describe responsibilities and contracts only.

Output quality rules:
- Be precise. Quote interface names, method signatures, file paths, and DI registrations.
- Use present tense and neutral tone.
- Acceptance criteria must be verifiable and specific (Given/When/Then or checklist).
- Keep each per-component spec self-contained and link back to dependency-inventory.md.
- NO code examples - implementation will be guided by spec kit implementation phase.
- Focus on contracts, responsibilities, and architectural patterns.

Dependency Spec Template (use for each component's spec.md):
---
Title: {ComponentName} – Dependency Specification
Source:
- File: {path/to/Component.cs}
- Interface: {path/to/IComponent.cs} (if applicable)
- Registration: {ServiceCollectionExtensions line number or startup location}
- Type: {Service/Model/Converter/Behavior/Extension/Core}

1. Purpose
   - What problem this component solves and for whom.
   - Business domain context.

2. Interface Contract (for services/components with interfaces)
   - Interface name: {IComponentName}
   - Public methods:
     - {MethodName}({parameters}) : {ReturnType} - {brief description of responsibility}
   - Public properties:
     - {PropertyName} : {Type} - {brief description}
   - Events (if any):
     - {EventName} : {EventArgsType} - {when raised}

3. Dependencies
   - Constructor parameters:
     - {ILogger<T>} - Logging
     - {IOtherService} - {reason for dependency}
   - Configuration dependencies:
     - {IConfiguration section path} - {what configuration is consumed}
   - External dependencies:
     - {Database connections, file system, external APIs}

4. DI Registration
   - Lifetime: {Singleton/Scoped/Transient}
   - Registration location: {ServiceCollectionExtensions.ConfigureServices() line X}
   - Rationale for lifetime choice: {why this lifetime}

5. Responsibilities & Business Logic
   - Primary responsibilities: {what this component does}
   - Business rules enforced: {validation, authorization, workflow rules}
   - Orchestration patterns: {how it coordinates with other services}
   - Error handling approach: {how errors are handled and propagated}
   - Async patterns: {which operations are async and why}

6. Data Flow & Transformations
   - Input: {what data comes in, from where}
   - Processing: {high-level description of transformations}
   - Output: {what data goes out, to where}
   - Models/DTOs used: {list of models this component works with}

7. Consumers & Usage
   - Direct consumers:
     - {ViewModelName} - {how it's used}
     - {ServiceName} - {how it's used}
   - Usage patterns: {common usage scenarios}
   - Initialization requirements: {any setup needed before use}

8. Configuration
   - Configuration sections used: {appsettings.json paths}
   - Configuration binding: {how configuration is loaded}
   - Environment-specific settings: {Development/Production differences}

9. Error Handling & Logging
   - Error handling strategy: {exceptions, Result<T> pattern, etc.}
   - Logging approach: {what gets logged at what levels}
   - Error propagation: {how errors flow to consumers}

10. Performance Considerations
   - Async operations: {which methods are async}
   - Caching (if applicable): {what is cached and why}
   - Resource management: {IDisposable, connection pooling, etc.}
   - Performance patterns: {batch processing, lazy loading, etc.}

11. Acceptance Criteria
   - [ ] Interface contract documented with all public methods
   - [ ] All dependencies identified and documented
   - [ ] DI registration location and lifetime documented
   - [ ] All consumers identified
   - [ ] Business logic responsibilities clearly described
   - [ ] Data flow and transformations mapped
   - [ ] Error handling patterns documented
   - [ ] Configuration dependencies listed

Open Questions
- {Any uncertainties needing clarification}
---

Success criteria:
- All services, models, converters, behaviors, and extensions discovered are listed in dependency-inventory.md.
- Each major component has a corresponding spec.md with contracts, dependencies, responsibilities, and patterns.
- global-architecture.md captures DI patterns, service layer architecture, and common conventions.
- dependency-audit-checklist.md indicates remaining gaps and architectural concerns.

Please start by:
1) Detecting all dependency components and building specs/000-dependency-capture/dependencies/dependency-inventory.md.
2) Generating specs per component following the template (NO code examples).
3) Producing the global-architecture.md guide.
4) Producing the dependency-audit-checklist.md with any unresolved questions.

## Model Optimization & Automation

### Recommended Models by Phase

**Phase 1-2: Discovery & Per-Component Analysis**
- **Model**: GPT-5-Codex
- **Rationale**: Superior architectural analysis, dependency graph construction, DI pattern recognition
- **Est. Premium Requests**: ~70 (Phase 1: ~15, Phase 2: ~55)
- **Duration**: 3-4 hours total

**Phase 3-4: Global Synthesis & Audit**
- **Model**: Claude Sonnet 4.5
- **Rationale**: Sophisticated architectural documentation, pattern synthesis, structured output
- **Est. Premium Requests**: ~15 (Phase 3: ~10, Phase 4: ~5)
- **Duration**: 1-1.5 hours total

**Total Estimated**: ~85 premium requests, 4.5-5.5 hours active work

### Automated Workflow (Recommended)

**Using Joyride Scripts** (see .github/joyride/scripts/README.md):

#### How to Use Joyride Automation

**Step 1: Start Joyride Evaluation**
- The Joyride scripts are already loaded in VS Code's classpath
- You can directly require and use the namespaces in your code evaluations
- Use the `joyride_evaluate_code` tool with `awaitResult: true` for operations that need results

**Step 2: Execute Workflow**
```clojure
(require '[reverse-dependency-workflow :as rdw])

;; Show overview first (optional)
(rdw/show-workflow-overview)

;; Execute complete workflow with automatic model switching
(rdw/execute-workflow)
```

**Important Notes**:
- ✅ Scripts are pre-loaded - just `require` the namespace directly
- ✅ Use `awaitResult: true` when evaluating code that returns promises or needs user interaction
- ✅ If any script has issues, the AI agent will fix and update them automatically
- ✅ All namespaces are available: `reverse-dependency-workflow`, `dependency-analyzer`, `dependency-spec-generator`, `dependency-batch-processor`, `dependency-progress-tracker`, `model-switcher`

#### Complete Workflow Automation

#### Phase 2 Automation (NEW - Recommended for 50+ components)

For **Phase 2 (Per-Component Analysis)**, use the automated utilities by requiring the namespaces:

```clojure
;; Require all automation utilities (namespaces are pre-loaded)
(require '[dependency-analyzer :as da])
(require '[dependency-spec-generator :as dsg])
(require '[dependency-batch-processor :as dbp])
(require '[dependency-progress-tracker :as dpt])

;; OPTION A: Complete Phase 2 automation (recommended)
;; Step 1: Analyze all dependencies and generate analysis-report.md
;; Use awaitResult: true when evaluating these
(da/save-analysis-report)

;; Step 2: Batch generate all spec files with error handling
(dbp/smart-batch-processor :generate-specs 15)

;; Step 3: Track progress and identify incomplete specs
(dpt/show-progress-dashboard)

;; Step 4: Work through incomplete specs
(dpt/show-incomplete-components)

;; OPTION B: Step-by-step with manual checkpoints
;; Analyze single component
(da/quick-component-summary "DatabaseService")

;; Generate specs for specific components
(dsg/generate-specs-for-components ["DatabaseService" "InventoryService"])

;; Check progress
(dpt/save-progress-report)
```

**Evaluation Guidelines for AI Agents**:
- Use `joyride_evaluate_code` tool with `awaitResult: true` for all operations above
- Scripts are pre-loaded in classpath - just require the namespace
- Show the code in a chat code block before evaluating so user can see what's happening
- If errors occur, analyze and fix the scripts automatically

#### New Joyride Scripts Available

**dependency_analyzer.cljs** - Automated dependency analysis
- `analyze-all-components` - Discover and analyze all services/models/converters/behaviors
- `analyze-component` - Deep analysis of interface contracts, dependencies, DI registration
- `save-analysis-report` - Generate comprehensive analysis-report.md

**dependency_spec_generator.cljs** - Automated spec.md generation
- `generate-all-specs` - Create all spec files at once
- `batch-generate-specs` - Memory-efficient batch generation
- `generate-specs-for-components` - Generate specific components only
- `regenerate-spec` - Recreate single spec file

**dependency_batch_processor.cljs** - Efficient batch processing
- `smart-batch-processor` - Adaptive batch sizing for large collections
- `batch-analyze-components` - Batch analysis with progress reporting
- `batch-generate-specs-with-errors` - Error recovery and retry
- `parallel-analyze-batch` - Parallel processing with concurrency limits
- `interactive-batch-menu` - Interactive UI for batch operations

**dependency_progress_tracker.cljs** - Progress tracking and reporting
- `show-progress-dashboard` - Interactive progress UI with actions
- `save-progress-report` - Generate detailed progress report
- `show-incomplete-components` - Quick-pick for incomplete specs
- `estimate-remaining-work` - Calculate hours to completion

**Benefits**:
- ✅ Automatic model switching at optimal transition points
- ✅ Human checkpoints between phases for review
- ✅ Progress tracking and premium request estimation
- ✅ Resume capability at any phase
- ✅ **NEW**: Automated dependency analysis and spec generation for Phase 2
- ✅ **NEW**: Batch processing with error recovery for 50+ components
- ✅ **NEW**: Progress tracking with completion percentages
- ✅ **NEW**: Interactive dashboards and quick-pick menus
- ✅ **NEW**: No code examples - focus on contracts and architecture

**Workflow includes**:
1. Phase overview with time/cost estimates
2. Auto-switch to GPT-5-Codex for Phase 1-2
3. **NEW**: Automated dependency analysis and spec generation (Phase 2)
4. Checkpoint: Review Phase 2 output, approve model switch
5. Auto-switch to Claude Sonnet 4.5 for Phase 3-4
6. Final completion summary with progress metrics

### Manual Execution (Alternative)

If not using Joyride automation:

**Phase 1-2: Manually select GPT-5-Codex**
1. Open Copilot Chat model selector
2. Choose "GPT-5-Codex"
3. Execute Phase 1-2 tasks
4. **STOP WORK** - Do not proceed to Phase 3 yet

**Checkpoint**: Review Phase 2 outputs
- Validate dependency-inventory.md completeness
- Spot-check 3-5 component specs for accuracy
- Verify interface contract documentation
- Approve model switch to maximize documentation quality

**Phase 3-4: Manually select Claude Sonnet 4.5**
1. Open Copilot Chat model selector
2. Choose "Claude Sonnet 4.5"
3. Execute Phase 3-4 tasks
4. Complete workflow

### Premium Request Optimization

**To maximize efficiency**:
- ✅ Batch similar tasks (analyze 10-15 components at once in Phase 2)
- ✅ Use explicit file references (#file:...) to minimize context loading
- ✅ Complete full phases before stopping to maintain context
- ✅ Review checkpoint outputs carefully - rework costs premium requests
- ✅ Focus on contracts and architecture - NO code examples needed

**Model switching rationale**:
- GPT-5-Codex: "Delivers higher-quality code on complex engineering tasks... without lengthy instructions"
- Claude Sonnet 4.5: "Complex problem-solving challenges, sophisticated reasoning" with agent mode

See `.github/joyride/scripts/README.md` for detailed automation usage.
