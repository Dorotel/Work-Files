/speckit.specify Reverse-spec the existing UI of the MTM_WIP_Application_Avalonia codebase and produce Spec Kit artifacts we can use for planning and future implementation.

Context:
- Tech: Avalonia UI (.axaml views), C# code-behind (.axaml.cs), MVVM Community Toolkit ViewModels, ResourceDictionaries, Styles, DataTemplates.
- Repository: Dorotel/MTM_WIP_Application_Avalonia
- Goal: Read-only analysis of the UI to create high-quality, structured specifications that describe the current screens, flows, bindings, commands, and UX rules.
- Timeline: 2 weeks
- Priorities: Migration planning, onboarding, documentation, compliance, testing
- Automation: Joyride scripts available for automated model switching and workflow orchestration (see .github/joyride/scripts/)

Clarified Decisions (see specs/ui-capture-clarifications.md for full rationale):
1. Scope: Top-level screens PLUS standalone reusable components (dialogs, overlays, controls from Controls/ directory)
2. Composite shells: Hybrid approach - high-level shell spec with links to region specs when complexity warrants
3. ViewModel recording: Document primary runtime ViewModel; note when alternates (design-time, DI variants) exist
4. MVVM Toolkit references: Document BOTH generated member names (e.g., MyProperty) AND backing definitions (e.g., [ObservableProperty] _myProperty)
5. Navigation depth: Comprehensive - include explicit navigation calls PLUS contextual triggers (tabs, quick buttons, auto-redirects)
6. Global UX granularity: Tiered - curated summary of actively-used resources with appendices for exhaustive listings
7. Accessibility: Tiered - baseline coverage everywhere (focus order, AutomationProperties), deeper dives where concerns detected
8. Audit checklist: Hybrid - static core sections with dynamic subsections populated from actual findings

Deliverables (create these files):
1) Top-level feature spec
   - Path: specs/000-ui-capture/spec.md
   - Contents:
     - Feature overview: “Capture current UI of MTM WIP Application (Avalonia)”
     - Objectives & non-goals
     - High-level user journeys and navigation map
     - Global UX rules (themeing, windowing, dialogs, errors, keyboard, accessibility)
     - Assumptions and open questions
     - Acceptance criteria for “UI capture complete”

2) UI inventory
   - Path: specs/000-ui-capture/ui/ui-inventory.md
   - Table with columns:
     - View Name
     - Primary .axaml path
     - Code-behind (.axaml.cs) path
     - ViewModel type (primary runtime ViewModel; note alternates if present)
     - Key DataContext/bindings
     - Commands and routed events
     - Navigation entry points (explicit calls + contextual triggers: tabs, quick buttons, auto-redirects)
     - Dependent Styles/Resources (ResourceDictionaries, DataTemplates)
     - Notes/quirks/edge cases
   - Include:
     - All Views/ directory screens
     - Standalone reusable components: Controls/CollapsiblePanel, Controls/CustomDataGrid, Controls/SessionHistoryPanel
     - Significant dialogs and overlays that function as distinct UI surfaces
   - Include a brief description per row of the screen purpose.

3) Per-view specifications (one folder per view/screen)
   - Path (for each view): specs/000-ui-capture/ui/{ViewName}/spec.md
   - Use the "UI Spec Template" below for each file.
   - Populate from code: layout structure, bindings, commands, validation, state transitions, and acceptance criteria.
   - For composite shells (e.g., MainWindow.axaml):
     - Create high-level shell spec documenting overall structure and regions
     - Link to separate nested specs at specs/000-ui-capture/ui/{ViewName}/{SubviewName}/spec.md when region complexity warrants detailed documentation
     - Use consistent judgment: separate specs for regions with 5+ distinct controls or complex state management
   - For MVVM Community Toolkit patterns:
     - Document BOTH generated property names (e.g., UserName) AND backing fields (e.g., [ObservableProperty] private string _userName)
     - Document BOTH generated command names (e.g., SaveCommand) AND backing methods (e.g., [RelayCommand] private async Task SaveAsync())

4) Global UX & style guide extraction
   - Path: specs/000-ui-capture/ui/global-ux.md
   - Structure:
     - **Curated Summary Section**: Document actively-referenced resources:
       - Theme systems (Resources/ThemesV2/ - 17 theme files)
       - Common styles and style classes
       - Frequently-used DataTemplates
       - Material icons patterns (Material.Icons.Avalonia)
       - Typography and spacing conventions
       - Color token usage
     - **Appendices Section**: Exhaustive reference listings:
       - Complete ResourceDictionary file inventory
       - All defined style keys
       - All DataTemplate keys
       - Unused/dormant resources (for potential cleanup)
   - Note conventions:
     - Command naming patterns (MVVM Toolkit generated names)
     - Validation patterns
     - Async/long-running operation indicators
     - Error display patterns

5) Checklist and gaps
   - Path: specs/000-ui-capture/ui/ui-audit-checklist.md
   - Structure:
     - **Static Core Sections** (predefined categories):
       - Views inventory completion
       - ViewModels documentation status
       - Navigation mapping coverage
       - Accessibility baseline checks
       - Resource documentation status
     - **Dynamic Subsections** (populated from findings):
       - Discovered views and their completion status
       - Identified accessibility concerns requiring deeper analysis
       - Missing or incomplete ViewModels
       - Unmapped navigation paths
       - Resource references without definitions
   - "Gaps & ambiguities" section listing unknowns or missing code references.
   - Progress metrics: X of Y views documented, completion percentage per section

Scope & process:
- Search patterns: 
  - Views: **.axaml**, **.xaml** (in Views/ directory)
  - Reusable components: Controls/CollapsiblePanel/, Controls/CustomDataGrid/, Controls/SessionHistoryPanel/
  - Code-behind: **.axaml.cs**
  - ViewModels: **ViewModel.cs** (ViewModels/ directory)
  - Styles & resources: **ResourceDictionary**, **Styles.xaml**, Resources/ThemesV2/** files, **DataTemplates**
- For each view:
  - Identify DataContext/ViewModel (primary runtime; note design-time or DI alternates if present)
  - Document bindings with BOTH generated names AND backing MVVM Toolkit attributes
  - Map commands (generated command names + backing [RelayCommand] methods)
  - Identify routed events, converters, validation
  - Map navigation flows comprehensively:
    - Explicit navigation: service method calls, button command handlers
    - Contextual triggers: tab selection changes, quick button clicks, automatic post-save redirects
  - Capture meaningful states (loading, empty, error) and transitions
  - Document layout hierarchy in structured text (no images needed)
  - List accessibility considerations:
    - Baseline: Focus order, AutomationProperties, accessible labels
    - Flag concerns for deeper analysis: missing keyboard shortcuts, insufficient contrast, missing screen-reader context
- Do not modify code—read and extract only.

Output quality rules:
- Be precise. Quote control names, binding paths, command names, and file paths.
- Use present tense and neutral tone.
- Acceptance criteria must be verifiable and specific (Given/When/Then or checklist).
- Keep each per-view spec self-contained and link back to ui-inventory.md.
- For MVVM Toolkit members, always document the full traceability chain.

UI Spec Template (use for each view’s spec.md):
---
Title: {ViewName} – UI Specification
Source:
- View: {path/to/View.axaml}
- Code-behind: {path/to/View.axaml.cs}
- ViewModel: {Namespace.ViewModelName}
- Related resources: [{paths to Styles/ResourceDictionaries/DataTemplates}]

1. Purpose
   - What problem this screen solves and for whom.

2. Visual Structure (textual outline)
   - Layout container(s): {Grid/StackPanel/DockPanel/etc.}
   - Regions and key controls (by name when available)
   - Notable styles or templates applied

3. Data & Bindings
   - DataContext: {ViewModel}
   - Bindings: 
     - {ControlName.Property} ←→ {ViewModel.Property} [{OneWay/TwoWay}]
   - Converters used and purpose

4. Commands & Interactions
   - Commands:
     - {ButtonName} -> {ViewModel.CommandName} (CanExecute conditions)
   - Routed events / behaviors
   - Keyboard shortcuts and focus behavior

5. Navigation & Flow
   - Entry: {how user navigates here}
   - Exits: {destinations}
   - Dialogs/Overlays/Transient UI elements

6. States & Validation
   - Loading/Empty/Error states and triggers
   - Validation rules (fields, messages, visual cues)

7. Accessibility
   - Focus order, automation properties, labels
   - Color/contrast notes, scaling behavior

8. Non-functional
   - Performance considerations (virtualization, heavy bindings)
   - Responsiveness/resizing behavior

9. Acceptance Criteria
   - [ ] Given {state}, when {action}, then {observable result}
   - [ ] Controls reflect ViewModel state and update as specified
   - [ ] Navigation triggers correct destinations under conditions
   - [ ] Validation appears under invalid inputs with messages {…}
   - [ ] Accessibility requirements satisfied as listed

Open Questions
- {Any uncertainties needing clarification}
---

Success criteria:
- All views discovered are listed in ui-inventory.md.
- Each view has a corresponding spec.md with concrete bindings, commands, flows, and acceptance criteria.
- global-ux.md captures shared resources and conventions.
- ui-audit-checklist.md indicates remaining gaps.

Please start by:
1) Detecting all Views and building specs/000-ui-capture/ui/ui-inventory.md.
2) Generating specs per view following the template.
3) Producing the global-ux.md summary.
4) Producing the ui-audit-checklist.md with any unresolved questions.

## Model Optimization & Automation

### Recommended Models by Phase

**Phase 1-2: Discovery & Per-View Analysis**
- **Model**: GPT-5-Codex
- **Rationale**: Superior code analysis, MVVM Toolkit pattern recognition, binding traceability
- **Est. Premium Requests**: ~65 (Phase 1: ~15, Phase 2: ~50)
- **Duration**: 3-3.5 hours total

**Phase 3-4: Global Synthesis & Audit**
- **Model**: Claude Sonnet 4.5
- **Rationale**: Sophisticated documentation synthesis, agent mode orchestration, structured output
- **Est. Premium Requests**: ~15 (Phase 3: ~10, Phase 4: ~5)
- **Duration**: 1-1.5 hours total

**Total Estimated**: ~80 premium requests, 4-5 hours active work

### Automated Workflow (Recommended)

**Using Joyride Scripts** (see .github/joyride/scripts/README.md):

#### How to Use Joyride Automation

**Step 1: Start Joyride Evaluation**
- The Joyride scripts are already loaded in VS Code's classpath
- You can directly require and use the namespaces in your code evaluations
- Use the `joyride_evaluate_code` tool with `awaitResult: true` for operations that need results

**Step 2: Execute Workflow**
```clojure
(require '[reverse-spec-workflow :as rsw])

;; Show overview first (optional)
(rsw/show-workflow-overview)

;; Execute complete workflow with automatic model switching
(rsw/execute-workflow)
```

**Important Notes**:
- ✅ Scripts are pre-loaded - just `require` the namespace directly
- ✅ Use `awaitResult: true` when evaluating code that returns promises or needs user interaction
- ✅ If any script has issues, the AI agent will fix and update them automatically
- ✅ All namespaces are available: `reverse-spec-workflow`, `view-analyzer`, `spec-generator`, `batch-processor`, `progress-tracker`, `model-switcher`

#### Complete Workflow Automation

#### Phase 2 Automation (NEW - Recommended for 30+ views)

For **Phase 2 (Per-View Analysis)**, use the automated utilities by requiring the namespaces:

```clojure
;; Require all automation utilities (namespaces are pre-loaded)
(require '[view-analyzer :as va])
(require '[spec-generator :as sg])
(require '[batch-processor :as bp])
(require '[progress-tracker :as pt])

;; OPTION A: Complete Phase 2 automation (recommended)
;; Step 1: Analyze all views and generate analysis-report.md
;; Use awaitResult: true when evaluating these
(va/save-analysis-report)

;; Step 2: Batch generate all spec files with error handling
(bp/smart-batch-processor :generate-specs 10)

;; Step 3: Track progress and identify incomplete specs
(pt/show-progress-dashboard)

;; Step 4: Work through incomplete specs
(pt/show-incomplete-views)

;; OPTION B: Step-by-step with manual checkpoints
;; Analyze single view
(va/quick-view-summary "MainWindow")

;; Generate specs for specific views
(sg/generate-specs-for-views ["MainWindow" "InventoryTabView"])

;; Check progress
(pt/save-progress-report)
```

**Evaluation Guidelines for AI Agents**:
- Use `joyride_evaluate_code` tool with `awaitResult: true` for all operations above
- Scripts are pre-loaded in classpath - just require the namespace
- Show the code in a chat code block before evaluating so user can see what's happening
- If errors occur, analyze and fix the scripts automatically

#### New Joyride Scripts Available

**view_analyzer.cljs** - Automated AXAML/ViewModel analysis
- `analyze-all-views` - Discover and analyze all .axaml files
- `analyze-view` - Deep analysis of bindings, commands, layout, MVVM Toolkit patterns
- `save-analysis-report` - Generate comprehensive analysis-report.md

**spec_generator.cljs** - Automated spec.md generation
- `generate-all-specs` - Create all spec files at once
- `batch-generate-specs` - Memory-efficient batch generation
- `generate-specs-for-views` - Generate specific views only
- `regenerate-spec` - Recreate single spec file

**batch_processor.cljs** - Efficient batch processing
- `smart-batch-processor` - Adaptive batch sizing for large collections
- `batch-analyze-views` - Batch analysis with progress reporting
- `batch-generate-specs-with-errors` - Error recovery and retry
- `parallel-analyze-batch` - Parallel processing with concurrency limits
- `interactive-batch-menu` - Interactive UI for batch operations

**progress_tracker.cljs** - Progress tracking and reporting
- `show-progress-dashboard` - Interactive progress UI with actions
- `save-progress-report` - Generate detailed progress report
- `show-incomplete-views` - Quick-pick for incomplete specs
- `estimate-remaining-work` - Calculate hours to completion

**Benefits**:
- ✅ Automatic model switching at optimal transition points
- ✅ Human checkpoints between phases for review
- ✅ Progress tracking and premium request estimation
- ✅ Resume capability at any phase
- ✅ **NEW**: Automated view analysis and spec generation for Phase 2
- ✅ **NEW**: Batch processing with error recovery for 30+ views
- ✅ **NEW**: Progress tracking with completion percentages
- ✅ **NEW**: Interactive dashboards and quick-pick menus

**Workflow includes**:
1. Phase overview with time/cost estimates
2. Auto-switch to GPT-5-Codex for Phase 1-2
3. **NEW**: Automated view analysis and spec generation (Phase 2)
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
- Validate ui-inventory.md completeness
- Spot-check 3-5 view specs for accuracy
- Verify MVVM Toolkit traceability
- Approve model switch to maximize documentation quality

**Phase 3-4: Manually select Claude Sonnet 4.5**
1. Open Copilot Chat model selector
2. Choose "Claude Sonnet 4.5"
3. Execute Phase 3-4 tasks
4. Complete workflow

### Premium Request Optimization

**To maximize efficiency**:
- ✅ Batch similar tasks (analyze 5-10 views at once in Phase 2)
- ✅ Use explicit file references (#file:...) to minimize context loading
- ✅ Complete full phases before stopping to maintain context
- ✅ Review checkpoint outputs carefully - rework costs premium requests

**Model switching rationale**:
- GPT-5-Codex: "Delivers higher-quality code on complex engineering tasks... without lengthy instructions"
- Claude Sonnet 4.5: "Complex problem-solving challenges, sophisticated reasoning" with agent mode

See `.github/joyride/scripts/README.md` for detailed automation usage.