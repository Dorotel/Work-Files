---
description: "Task list for feature 005 - Manufacturing Application Modernization (All-in-One Mega-Feature)"
---

# Tasks: 005 - Manufacturing Application Modernization

**Input**: Design documents from `/specs/005-migrate-infor-visual/`
**Prerequisites**: plan.md, spec.md, data-model.md, contracts/

---

## 📑 Table of Contents

### Task Organization
- [Dependencies & Execution Order](#dependencies--execution-order) - Phase dependencies and critical path
- [Task Format & Conventions](#format-id-p-story-description) - Task naming and parallelization markers

### Implementation Phases

#### Foundation Phases (1-4)
- [Phase 1: Custom Controls Library](#phase-1-custom-controls-library-foundation-) - 10 reusable controls (T001-T033)
- [Phase 2: Settings Management UI](#phase-2-settings-management-ui) - Settings window with 8 categories (T034-T076)
- [Phase 3: Debug Terminal Modernization](#phase-3-debug-terminal-modernization) - SplitView navigation redesign (T077-T104)
- [Phase 4: Configuration Error Dialog](#phase-4-configuration-error-dialog) - Error handling modal (T105-T116)

#### VISUAL Integration Phases (5-11)
- [Phase 5: VISUAL API Setup](#phase-5-visual-api-toolkit-setup-preparation-) - API Toolkit integration prep (T117-T128)
- [Phase 6: VISUAL Foundation](#phase-6-visual-erp-foundation-architecture-) - Core services and infrastructure (T129-T151)
- [Phase 7: US5 Part Lookup](#phase-7-us5-part-information-lookup-mvp-) - Part lookup UI and logic (T152-T165)
- [Phase 8: US6 Work Orders](#phase-8-us6-work-order-operations) - Work order management (T166-T183)
- [Phase 9: US7 Inventory](#phase-9-us7-inventory-transaction-recording) - Inventory transactions (T184-T202)
- [Phase 10: US8 Shipments](#phase-10-us8-shipment-confirmation) - Shipment confirmation (T203-T220)
- [Phase 11: US9 Offline & Performance](#phase-11-us9-offline-operation--performance-monitoring) - Offline mode and monitoring (T221-T243)

#### Final Phase
- [Phase 12: Polish & Documentation](#phase-12-polish--release-preparation) - Integration testing and docs (T244-T261)

### Testing Strategy
- Test-First Workflow - TDD approach for all phases
- Coverage Targets - 80%+ for critical paths
- Test Categories - Unit, Integration, Contract, Performance

### Reference
- [Related Files](#related-files) - Links to spec.md, plan.md, data-model.md
- [Task Progress Tracking](#task-progress-tracking) - Completion checklist

---

## Dependencies & Execution Order

**Tests**: Test-first workflow is mandatory (see plan.md constitution check). Each phase lists tests that MUST be implemented before feature code.

**Organization**: Tasks are grouped by phase with clear dependencies. Phases 1-4 are NEW (Custom Controls, Settings, Debug Terminal, Config Dialog). Phases 5-11 are VISUAL integration (existing content renumbered).

## Dependencies & Execution Order

1. **Setup (Phase 5 - VISUAL)** → prerequisite for foundation.
2. **Foundation (Phase 6 - VISUAL)** → blocks all VISUAL user stories until complete.
3. **User Stories**:
   - **US5 Part Lookup (P1, Phase 7)** starts immediately after foundation and delivers MVP.
   - **US6 Work Orders (P1, Phase 8)** can start after foundation; integrates with US5 but remains independently testable.
   - **US7 Inventory (P2, Phase 9)** depends on US5/US6 repositories and transaction scaffolding.
   - **US8 Shipments (P2, Phase 10)** depends on US6 (navigation patterns) and US7 (transaction persistence helpers).
   - **US9 Offline (P3, Phase 11)** depends on all prior stories for UI hooks and repositories.
4. **Polish (Phase 12)** follows completion of desired user stories and final integration validation.

**Story Dependencies Graph**:
- Foundation → US5 → US6 → US7 → US8 → US9 → Polish
- US7 also requires US6's repository extensions; US8 builds on US7's transaction persistence.ies**:

1. **Phase 1 (Custom Controls)** - FOUNDATION for all subsequent phases. No dependencies.
2. **Phase 2 (Settings UI)** - Depends on Phase 1 (uses SettingsCategory, SettingRow, ActionButtonGroup controls).
3. **Phase 3 (Debug Terminal)** - Depends on Phase 1 (uses StatusCard, MetricDisplay, ErrorListPanel, BootTimelineChart, NavigationMenuItem, ActionButtonGroup controls). Independent of Phase 2.
4. **Phase 4 (Config Dialog)** - Depends on Phase 1 (uses ConfigurationErrorDialog, ActionButtonGroup controls) and Phase 2 (integrates with Settings window).
5. **Phase 5 (VISUAL Setup)** - Independent of Phases 1-4 (but Phases 1-4 should be complete before starting VISUAL work).
6. **Phase 6 (VISUAL Foundation)** - Depends on Phase 5, blocks all VISUAL user stories.
7. **Phase 7 (US5 Part Lookup)** - Depends on Phase 6, delivers MVP.
8. **Phase 8 (US6 Work Orders)** - Depends on Phase 6, independent of Phase 7 but integrates with US5 navigation.
9. **Phase 9 (US7 Inventory)** - Depends on Phases 7 & 8 (uses repositories and transaction patterns).
10. **Phase 10 (US8 Shipments)** - Depends on Phases 8 & 9 (uses repository extensions and transaction persistence).
11. **Phase 11 (US9 Offline/Performance)** - Depends on all prior phases (adds monitoring to all VISUAL operations).
12. **Phase 12 (Polish)** - Depends on completion of desired user stories, final integration validation.

**Critical Path**:
```
Phase 1 (Custom Controls)
├─→ Phase 2 (Settings UI)
│   └─→ Phase 4 (Config Dialog)
├─→ Phase 3 (Debug Terminal) [independent of Phase 2]
└─→ Phase 5 (VISUAL Setup)
    └─→ Phase 6 (VISUAL Foundation)
        └─→ Phase 7 (US5 Part Lookup) [MVP]
            ├─→ Phase 8 (US6 Work Orders)
            │   ├─→ Phase 9 (US7 Inventory)
            │   │   └─→ Phase 10 (US8 Shipments)
            │   │       └─→ Phase 11 (US9 Offline/Performance)
            │   │           └─→ Phase 12 (Polish)
```

**Minimum Viable Product (MVP)**: Phase 1 → Phase 6 → Phase 7 (Custom Controls → VISUAL Foundation → Part Lookup)

**Recommended Delivery Sequence**:
1. Complete Phases 1-4 first (Custom Controls, Settings, Debug Terminal, Config Dialog) - Immediate value, independent of VISUAL
2. Then Phases 5-7 (VISUAL Setup/Foundation/Part Lookup) - MVP VISUAL integration
3. Then Phases 8-11 (Work Orders, Inventory, Shipments, Offline) - Full VISUAL integration
4. Finally Phase 12 (Polish) - Documentation and release preparationplan.md constitution check). Each phase lists tests that MUST be implemented before feature code.

**Organization**: Tasks are grouped by phase with clear dependencies. Phases 1-4 are NEW (Custom Controls, Settings, Debug Terminal, Config Dialog). Phases 5-11 are VISUAL integration (existing content renumbered).

## Format: `[ID] [P?] [Story] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- **[Phase]**: Which phase the task belongs to (Phase 1-11)
- Include exact file paths in descriptions for quick navigation

---

## Phase 1: Custom Controls Library (Foundation 🎯)

**Purpose**: Extract 10 reusable Avalonia custom controls from existing UIs to eliminate XAML duplication (60%+ reduction) and provide foundation for all subsequent phases. Addresses Constitution Principle XI and TODO from constitution.md lines 34-37.

**Independent Test**: Extract StatusCard control → document in catalog → use in test view → verify properties bind → success = renders correctly with full documentation.

### Tests for Phase 1 (write first)

- [x] T001 [P] [Phase1] Create `tests/unit/Controls/StatusCardTests.cs` with tests for Title/Status/IconSource/Background property changes, null handling, and rendering
- [x] T002 [P] [Phase1] Create `tests/unit/Controls/MetricDisplayTests.cs` with tests for Value/Label/Format/Trend/Color properties, numeric formatting, and trend indicators
- [x] T003 [P] [Phase1] Create `tests/unit/Controls/ErrorListPanelTests.cs` with tests for error collection binding, severity icon mapping, timestamp formatting
- [x] T004 [P] [Phase1] Create `tests/unit/Controls/ConnectionHealthBadgeTests.cs` with tests for Status enum (Healthy/Degraded/Offline), color mapping, LastChecked display
- [x] T005 [P] [Phase1] Create `tests/unit/Controls/BootTimelineChartTests.cs` with tests for stage duration visualization, target comparison, performance indication
- [x] T006 [P] [Phase1] Create `tests/unit/Controls/SettingsCategoryTests.cs` with tests for CategoryName/Icon/Children collection binding, expand/collapse behavior
- [x] T007 [P] [Phase1] Create `tests/unit/Controls/SettingRowTests.cs` with tests for SettingKey/SettingValue/SettingType/Validation properties, inline validation display
- [x] T008 [P] [Phase1] Create `tests/unit/Controls/NavigationMenuItemTests.cs` with tests for MenuText/Icon/IsSelected/Command properties, selection state visual
- [x] T009 [P] [Phase1] Create `tests/unit/Controls/ConfigurationErrorDialogTests.cs` with tests for ErrorMessage/RecoveryOptions/result handling, button commands
- [x] T010 [P] [Phase1] Create `tests/unit/Controls/ActionButtonGroupTests.cs` with tests for Buttons collection, consistent spacing, enable/disable states

### Implementation for Phase 1

- [x] T011 [Phase1] Create `MTM_Template_Application/Controls/` directory and add to project structure
- [x] T012 [P] [Phase1] Extract StatusCard pattern from `DebugTerminalWindow.axaml` → Create `Controls/StatusCard.axaml` and `.cs` with StyledProperty for Title, Status, IconSource, Background. Use TemplatedControl base class with ControlTheme in axaml.
- [x] T013 [P] [Phase1] Extract MetricDisplay pattern (20+ occurrences) → Create `Controls/MetricDisplay.axaml` and `.cs` with StyledProperty for Value, Label, Format, Trend, Color
- [x] T014 [P] [Phase1] Extract ErrorListPanel pattern → Create `Controls/ErrorListPanel.axaml` and `.cs` with ItemsControl for error collection, DataTemplate for severity icons
- [x] T015 [P] [Phase1] Extract ConnectionHealthBadge pattern → Create `Controls/ConnectionHealthBadge.axaml` and `.cs` with StyledProperty for Status enum, LastChecked DateTime
- [x] T016 [P] [Phase1] Create BootTimelineChart control → `Controls/BootTimelineChart.axaml` and `.cs` with ItemsControl for stage entries, visual comparison with targets
- [x] T017 [P] [Phase1] Create SettingsCategory control → `Controls/SettingsCategory.axaml` and `.cs` with StyledProperty for CategoryName, Icon, Children ObservableCollection
- [x] T018 [P] [Phase1] Create SettingRow control → `Controls/SettingRow.axaml` and `.cs` with StyledProperty for SettingKey, SettingValue, SettingType, Validation, supports String/Int/Decimal/Boolean/Enum types
- [x] T019 [P] [Phase1] Create NavigationMenuItem control → `Controls/NavigationMenuItem.axaml` and `.cs` with StyledProperty for MenuText, Icon, IsSelected, Command, visual selection indicator
- [x] T020 [P] [Phase1] Create ConfigurationErrorDialog control → `Controls/ConfigurationErrorDialog.axaml` and `.cs` with StyledProperty for ErrorMessage, RecoveryOptions, result handling via DialogResult
- [x] T021 [P] [Phase1] Create ActionButtonGroup control → `Controls/ActionButtonGroup.axaml` and `.cs` with StyledProperty for Buttons collection, ItemsControl with consistent spacing
- [x] T022 [Phase1] Create `docs/UI-CUSTOM-CONTROLS-CATALOG.md` with template structure: Introduction, Quick Reference Table, Detailed Controls sections
- [x] T023 [P] [Phase1] Document StatusCard in catalog: Name, Purpose, Properties table (Title/Status/IconSource/Background with types/defaults), Events, Usage Example, Screenshot placeholder
- [x] T024 [P] [Phase1] Document MetricDisplay in catalog: Properties table (Value/Label/Format/Trend/Color), numeric formatting examples, Usage Example
- [x] T025 [P] [Phase1] Document ErrorListPanel in catalog: Properties table (Errors collection), severity mapping, Usage Example
- [x] T026 [P] [Phase1] Document ConnectionHealthBadge in catalog: Properties table (Status/LastChecked), Status enum values, color mapping
- [x] T027 [P] [Phase1] Document BootTimelineChart in catalog: Properties table (Stages collection), target comparison visual, Usage Example
- [x] T028 [P] [Phase1] Document SettingsCategory in catalog: Properties table (CategoryName/Icon/Children), expand/collapse behavior
- [x] T029 [P] [Phase1] Document SettingRow in catalog: Properties table (SettingKey/SettingValue/SettingType/Validation), supported types, inline validation
- [x] T030 [P] [Phase1] Document NavigationMenuItem in catalog: Properties table (MenuText/Icon/IsSelected/Command), selection visual
- [x] T031 [P] [Phase1] Document ConfigurationErrorDialog in catalog: Properties table (ErrorMessage/RecoveryOptions), recovery button behavior
- [x] T032 [P] [Phase1] Document ActionButtonGroup in catalog: Properties table (Buttons collection), spacing rules, Usage Example
- [x] T033 [Phase1] Run `dotnet test --filter "Category=Unit&FullyQualifiedName~Controls"` to verify all 10 controls have 80%+ test coverage (58 tests passing, 86.5% coverage)

**Checkpoint**: Custom Controls Library complete with documentation. Foundation ready for Phases 2-5.

---

## Phase 2: Settings Management UI

**Purpose**: Build comprehensive settings window with tabbed navigation for 8 categories exposing all 60+ IConfigurationService settings. Enables non-technical users to configure application without editing files.

**Independent Test**: Open Settings window → change Theme setting → click Save → restart application → verify theme persisted.

### Tests for Phase 2 (write first)

- [x] T034 [P] [Phase2] Create `tests/unit/ViewModels/Settings/SettingsViewModelTests.cs` with tests for tab navigation, Save/Cancel commands, validation state management
- [x] T035 [P] [Phase2] Create `tests/unit/ViewModels/Settings/GeneralSettingsViewModelTests.cs` with tests for theme/language/startup settings, validation (6 tests passing)
- [x] T036 [P] [Phase2] Create `tests/unit/ViewModels/Settings/DatabaseSettingsViewModelTests.cs` with tests for connection string validation using FluentValidation patterns (5 tests passing)
- [x] T037 [P] [Phase2] Create `tests/unit/ViewModels/Settings/VisualSettingsViewModelTests.cs` with tests for API endpoint URL validation, credential masking (5 tests passing)
- [x] T038 [P] [Phase2] Create `tests/unit/ViewModels/Settings/LoggingSettingsViewModelTests.cs` with tests for log level enum, file path validation (5 tests passing)
- [x] T039 [P] [Phase2] Create `tests/unit/ViewModels/Settings/UiSettingsViewModelTests.cs` with tests for theme/font/layout settings (5 tests passing)
- [x] T040 [P] [Phase2] Create `tests/unit/ViewModels/Settings/CacheSettingsViewModelTests.cs` with tests for TTL numeric validation, size limits (6 tests passing)
- [x] T041 [P] [Phase2] Create `tests/unit/ViewModels/Settings/PerformanceSettingsViewModelTests.cs` with tests for budget threshold validation (positive integers only) (5 tests passing)
- [x] T042 [P] [Phase2] Create `tests/unit/ViewModels/Settings/DeveloperSettingsViewModelTests.cs` with tests for debug mode toggle, trace levels (4 tests passing)
- [x] T043 [P] [Phase2] Create `tests/integration/SettingsPersistenceTests.cs` with tests for UserPreferences table persistence, import/export JSON (4 tests: 2 passing, 2 skipped pending test user)

### Implementation for Phase 2

- [x] T044 [Phase2] Create `MTM_Template_Application/ViewModels/Settings/` directory
- [x] T045 [Phase2] Create `MTM_Template_Application/Views/Settings/` directory
- [x] T046 [P] [Phase2] Create `ViewModels/Settings/SettingsViewModel.cs` with [ObservableProperty] for category ViewModels, [RelayCommand] for Save/Cancel/Export/Import, uses IConfigurationService
- [x] T047 [P] [Phase2] Create `ViewModels/Settings/GeneralSettingsViewModel.cs` with theme/language/startup properties, FluentValidation rules
- [x] T048 [P] [Phase2] Create `ViewModels/Settings/DatabaseSettingsViewModel.cs` with connection string properties, FluentValidation for connection string format
- [x] T049 [P] [Phase2] Create `ViewModels/Settings/VisualSettingsViewModel.cs` with API endpoint/credentials properties, URL validation
- [x] T050 [P] [Phase2] Create `ViewModels/Settings/LoggingSettingsViewModel.cs` with log level/file path properties, file path validation
- [x] T051 [P] [Phase2] Create `ViewModels/Settings/UiSettingsViewModel.cs` with theme/font/layout properties
- [x] T052 [P] [Phase2] Create `ViewModels/Settings/CacheSettingsViewModel.cs` with TTL/size properties, numeric range validation
- [x] T053 [P] [Phase2] Create `ViewModels/Settings/PerformanceSettingsViewModel.cs` with budget/threshold properties, positive integer validation
- [x] T054 [P] [Phase2] Create `ViewModels/Settings/DeveloperSettingsViewModel.cs` with debug/trace properties
- [x] T055 [Phase2] Create `Views/Settings/SettingsWindow.axaml` with TabControl for 8 categories, uses SettingRow controls from Phase 1, x:DataType="vm:SettingsViewModel", CompiledBinding only
- [x] T056 [P] [Phase2] Create `Views/Settings/GeneralSettingsView.axaml` with SettingRow controls for theme/language/startup, CompiledBinding
- [x] T057 [P] [Phase2] Create `Views/Settings/DatabaseSettingsView.axaml` with SettingRow for connection strings, inline validation display
- [x] T058 [P] [Phase2] Create `Views/Settings/VisualSettingsView.axaml` with SettingRow for API endpoint/credentials, URL validation feedback
- [x] T059 [P] [Phase2] Create `Views/Settings/LoggingSettingsView.axaml` with SettingRow for levels/paths, file path validation
- [x] T060 [P] [Phase2] Create `Views/Settings/UiSettingsView.axaml` with SettingRow for theme/fonts/layout
- [x] T061 [P] [Phase2] Create `Views/Settings/CacheSettingsView.axaml` with SettingRow for TTL/size, numeric validation
- [x] T062 [P] [Phase2] Create `Views/Settings/PerformanceSettingsView.axaml` with SettingRow for budgets/thresholds, positive integer validation
- [x] T063 [P] [Phase2] Create `Views/Settings/DeveloperSettingsView.axaml` with SettingRow for debug/trace settings
- [x] T064 [Phase2] Register Settings window and ViewModels in DI (`Program.cs`), add menu command to MainWindow for opening Settings
- [X] T065 [Phase2] Implement Save command to persist all changes to UserPreferences table via IConfigurationService with audit trail (user, timestamp)
- [X] T066 [Phase2] Implement Cancel command to discard staged changes and revert to previous values
- [X] T067 [Phase2] Implement Export command to serialize settings to JSON with filtered sensitive values (passwords, tokens, keys show as "***FILTERED***")
- [X] T068 [Phase2] Implement Import command to deserialize JSON, validate, and apply settings with conflict resolution (prompt user for overwrites)
- [X] T069 [Phase2] Add real-time validation feedback with inline error messages for invalid setting values (connection strings, URLs, file paths, numeric ranges)
- [X] T070 [Phase2] Run `dotnet test --filter "Category=Unit&FullyQualifiedName~Settings"` and verify Settings UI loads within 500ms (performance test)

**Checkpoint**: Settings Management UI complete. Users can configure all 60+ settings without file editing.

---

## Phase 3: Debug Terminal Modernization

**Purpose**: Complete rewrite of DebugTerminalWindow with SplitView navigation, feature-based organization, and pending TODO completion (Copy to Clipboard, IsMonitoring, Environment Variables). Uses custom controls from Phase 1.

**Independent Test**: Open Debug Terminal → click "Feature 001: Boot" → view Boot Timeline chart → copy data to clipboard → success = navigation works, all sections functional.

### Tests for Phase 3 (write first)

- [ ] T071 [P] [Phase3] Create `tests/integration/DebugTerminalNavigationTests.cs` with tests for SplitView navigation, feature section loading, collapsible side panel
- [ ] T072 [P] [Phase3] Create `tests/unit/ViewModels/DebugTerminalViewModelTests.cs` with tests for CopyToClipboardCommand, IsMonitoring toggle, Environment Variables filtering
- [ ] T073 [P] [Phase3] Create `tests/performance/DebugTerminalLoadPerformanceTests.cs` with test verifying window load <500ms

### Implementation for Phase 3

- [ ] T074 [Phase3] Backup existing `Views/DebugTerminalWindow.axaml` to `Views/DebugTerminalWindow.axaml.old` for reference during rewrite
- [ ] T075 [Phase3] Rewrite `Views/DebugTerminalWindow.axaml` with SplitView root, collapsible side panel (IsPaneOpen bindable), hamburger menu button, x:DataType="vm:DebugTerminalViewModel", CompiledBinding
- [ ] T076 [P] [Phase3] Add NavigationMenuItem controls (from Phase 1) to SplitView pane for feature sections: "Feature 001: Boot", "Feature 002: Config", "Feature 003: Diagnostics", "Feature 005: VISUAL"
- [ ] T077 [P] [Phase3] Create "Feature 001: Boot" content section in DebugTerminalWindow using BootTimelineChart control (from Phase 1), bind to BootTimeline collection
- [ ] T078 [P] [Phase3] Create "Feature 002: Config" content section using SettingsCategory/SettingRow controls (from Phase 1), display current configuration values
- [ ] T079 [P] [Phase3] Create "Feature 003: Diagnostics" content section using StatusCard/MetricDisplay/ErrorListPanel controls (from Phase 1), bind to performance snapshots
- [ ] T080 [P] [Phase3] Create "Feature 005: VISUAL" content section placeholder (will be populated in Phase 5) using StatusCard/ConnectionHealthBadge controls
- [ ] T081 [Phase3] Update `ViewModels/DebugTerminalViewModel.cs` with [ObservableProperty] for SelectedFeature, IsPaneOpen, [RelayCommand] for feature navigation
- [ ] T082 [Phase3] Implement CopyToClipboardCommand in DebugTerminalViewModel with actual clipboard integration (System.Windows.Clipboard or Avalonia.Input.Platform), JSON serialization of current section data
- [ ] T083 [Phase3] Implement IsMonitoring toggle in DebugTerminalViewModel with [ObservableProperty], start/stop performance snapshot collection every 5 seconds when enabled (addresses Feature 003 TODO)
- [ ] T084 [Phase3] Implement Environment Variables display section with filtered sensitive variables (PASSWORD, TOKEN, SECRET, KEY showing as "***FILTERED***") (addresses Feature 003 TODO)
- [ ] T085 [Phase3] Add "Copy to Clipboard", "IsMonitoring" toggle, "Export Diagnostics" buttons to each feature section using ActionButtonGroup control (from Phase 1)
- [ ] T086 [Phase3] Wire navigation commands to change SelectedFeature property and update content area (use ContentControl with DataTemplate selector for feature sections)
- [ ] T087 [Phase3] Run `dotnet test --filter "Category=Performance&FullyQualifiedName~DebugTerminal"` to verify <500ms load time

**Checkpoint**: Debug Terminal modernized with feature-based navigation, all pending TODOs complete.

---

## Phase 4: Configuration Error Dialog

**Purpose**: Create ConfigurationErrorDialog modal for graceful configuration error handling during startup/runtime. Integrates with MainWindow error paths (addresses Feature 002 TODO from line 1234).

**Independent Test**: Cause configuration error (invalid database connection) → verify dialog appears with recovery suggestions → click "Edit Settings" → Settings window opens to Database tab.

### Tests for Phase 4 (write first)

- [ ] T088 [P] [Phase4] Create `tests/unit/ViewModels/ConfigurationErrorDialogViewModelTests.cs` with tests for ErrorMessage/RecoveryOptions properties, button command execution
- [ ] T089 [P] [Phase4] Create `tests/integration/ConfigurationErrorDialogIntegrationTests.cs` with tests for MainWindow error path integration, Settings window opening

### Implementation for Phase 4

- [ ] T090 [Phase4] ConfigurationErrorDialog control already created in Phase 1  - verify it exists in `Controls/ConfigurationErrorDialog.axaml`
- [ ] T091 [Phase4] Create `ViewModels/ConfigurationErrorDialogViewModel.cs` with [ObservableProperty] for ErrorMessage, ErrorCategory, AffectedSettingKey, Timestamp, [RelayCommand] for EditSettings/Retry/Exit
- [ ] T092 [Phase4] Update `Views/MainWindow.axaml` error paths (search for "TODO: Feature 002" around line 1234) to show ConfigurationErrorDialog on critical configuration errors
- [ ] T093 [Phase4] Implement EditSettingsCommand to open SettingsWindow (from Phase 2) to relevant tab based on ErrorCategory, highlight AffectedSettingKey
- [ ] T094 [Phase4] Implement RetryCommand to re-attempt configuration operation (e.g., database connection test), close dialog if successful
- [ ] T095 [Phase4] Implement ExitCommand to gracefully shut down application with confirmation prompt
- [ ] T096 [Phase4] Register ConfigurationErrorDialogViewModel in DI, add to MainViewModel error handling paths
- [ ] T097 [Phase4] Run `dotnet test --filter "FullyQualifiedName~ConfigurationErrorDialog"` to verify integration with MainWindow

**Checkpoint**: Configuration Error Dialog complete and integrated with MainWindow error paths.

---

## Phase 5: VISUAL Integration - Setup (Shared Infrastructure)

## Phase 5: VISUAL Integration - Setup (Shared Infrastructure)

**Purpose**: Prepare solution-level prerequisites required before implementing VISUAL integration.

- [ ] T098 [Phase5-Setup] Add Windows-only references to the Infor VISUAL API Toolkit assemblies (9 DLLs) in `MTM_Template_Application/MTM_Template_Application.csproj` and `MTM_Template_Application.Desktop/MTM_Template_Application.Desktop.csproj`. Use `<Reference Include="[DllName]">` with `HintPath` pointing to `..\..\docs\Visual Files\ReferenceFiles\[DllName].dll` for each: LsaCore, LsaShared, VmfgFinancials, VmfgInventory, VmfgPurchasing, VmfgSales, VmfgShared, VmfgShopFloor, VmfgTrace. Set `<Private>True</Private>` and `<CopyLocal>True</CopyLocal>` for each reference. **Note**: DLLs are already available locally in repository - no external installation required.
- [ ] T099 [Phase5-Setup] Introduce a `VisualIntegration` configuration section in `MTM_Template_Application/appsettings.json` (performance thresholds, cache TTL overrides, degradation settings) and update `Services/Configuration/ConfigurationService.cs` to expose strongly typed options.

---

## Phase 6: VISUAL Integration - Foundation (Blocking Prerequisites)

**Purpose**: Core infrastructure required by all VISUAL features. No user-story work may start until these tasks are complete.

- [ ] T100 [P] [Phase6-Foundation] Add Visual domain models (`Part`, `WorkOrder`, `InventoryTransaction`, `CustomerOrder`, `ShipmentStatus`, nested types, `VisualPerformanceMetrics`) under `MTM_Template_Application/Models/Visual/`, matching `data-model.md` nullability rules.
- [ ] T101 [P] [Phase6-Foundation] Create `LocalTransactionRecord` model and `LocalTransactionType` enum with JSON serialization helpers in `Models/Visual/LocalTransactionRecord.cs` to represent MAMP MySQL audit records.
- [ ] T102 [Phase6-Foundation] Register `VisualIntegrationOptions` POCOs and bind them inside `Services/Configuration/ConfigurationService.cs`, exposing accessors for response thresholds, cache TTL, and degradation limits.
- [ ] T103 [Phase6-Foundation] Define `IVisualApiService` plus filter DTOs (`PartSearchFilter`, `WorkOrderFilter`, `CustomerOrderFilter`) in `Services/Visual/IVisualApiService.cs` aligned with research.md abstractions.
- [ ] T104 [Phase6-Foundation] Scaffold `Services/Visual/VisualApiService.cs` with dependency injection, Polly retry policies, whitelist validation, structured logging hooks, and method stubs for all planned operations (throw `NotImplementedException` until stories fill them in).
- [ ] T105 [Phase6-Foundation] Implement `Services/Cache/VisualCacheService.cs` to wrap `CacheService`, enforce TTL policies (24h parts, 7d others), and provide helpers for cache keys defined in `data-model.md`.
- [ ] T106 [Phase6-Foundation] Create `Services/DataLayer/LocalTransactionRepository.cs` with parameterized CRUD scaffolding targeting `LocalTransactionRecords`, including serialization helpers and cancellation support.
- [ ] T107 [Phase6-Foundation] Document the `LocalTransactionRecords` table in `.github/mamp-database/schema-tables.json` and append the migration reference to `.github/mamp-database/migrations-history.json`.
- [ ] T108 [Phase6-Foundation] Add `Services/Visual/VisualServiceExtensions.cs` plus register Visual services/repositories/options in DI, updating `MTM_Template_Application.Desktop/Program.cs` to call the extension.

**Checkpoint**: Foundation ready — user stories can now proceed in priority order.

---

## Phase 7: User Story 5 - Part Lookup (Priority: P1) 🎯 MVP

**Goal**: Enable operators to scan or enter a part number and retrieve VISUAL part details (description, on-hand inventory, location, specifications) within 3 seconds, with cached fallback when offline.

**Independent Test**: Scan part “12345-ABC” (or use sample from quickstart) → part details load within 3 seconds online, within 1 second offline with “Last updated” timestamp.

### Tests for User Story 5 (Phase 7) (write first)

- [ ] T109 [P] [US5] Extend `tests/contract/VisualApiContractTests.cs` with CT-PART-001/002 contract tests covering `GetPartByIdAsync` and `SearchPartsAsync` against the VISUAL API Toolkit abstraction.
- [ ] T110 [P] [US5] Add `tests/integration/VisualPartLookupIntegrationTests.cs` validating cache-first lookup, live fallback, and offline banner behavior using `VisualCacheService`.
- [ ] T111 [P] [US5] Create `tests/unit/ViewModels/PartLookupViewModelTests.cs` to cover command enablement, barcode search workflow, and error handling.

### Implementation for User Story 9 (Phase 11) (Phase 7)

- [ ] T112 [P] [US5] Implement part operations in `Services/Visual/VisualApiService.cs` and add `VisualPartsRepository.cs` to translate toolkit data, integrate caching, and enforce whitelist.
- [ ] T113 [US5] Add `ViewModels/PartLookupViewModel.cs` using CommunityToolkit source generators, injecting repositories, cache service, and configuration options.
- [ ] T114 [US5] Create `Views/PartLookupView.axaml` (+ code-behind) with compiled bindings, search input, results grid, cached indicators, and barcode scan affordances following theme guidelines.
- [ ] T115 [US5] Update `Views/MainView.axaml` and `ViewModels/MainViewModel.cs` to host the part lookup experience (replace placeholder dashboard content) and surface offline/cached banners.
- [ ] T116 [US5] Register Part Lookup view + view model in DI/navigation (`Program.cs`, `Services/Navigation`, and `ViewLocator.cs`) so it loads as the default manufacturing workspace.

**Checkpoint**: Part lookup flow fully functional and independently demoable.

---

## Phase 8: User Story 6 - Work Order Operations (Priority: P1)

**Goal**: Allow supervisors to list/filter work orders, drill into material/labor details, and persist status updates to local MySQL within 2 seconds.

**Independent Test**: Filter open work orders → open detail for “WO-2024-001” → change status to Closed → confirm record stored locally even if VISUAL is offline.

### Tests for User Story 6 (Phase 8) (write first)

- [ ] T117 [P] [US6] Add CT-WO-001 contract coverage to `tests/contract/VisualApiContractTests.cs` for list/detail queries with material requirements and operation steps.
- [ ] T118 [P] [US6] Create `tests/integration/VisualWorkOrderWorkflowTests.cs` validating status updates persist via `LocalTransactionRepository` and cache refresh logic.
- [ ] T119 [P] [US6] Add `tests/unit/ViewModels/WorkOrderViewModelTests.cs` (list + detail) to verify filtering, status transitions, and warning flows.

### Implementation for User Story 6 (Phase 8)

- [ ] T120 [P] [US6] Implement `GetWorkOrdersAsync`/`GetWorkOrderByIdAsync` in `VisualApiService` and add `VisualWorkOrderRepository.cs` with filter support and cache keys.
- [ ] T121 [US6] Extend `LocalTransactionRepository` with `SaveWorkOrderStatusAsync` and query helpers, ensuring JSON payloads mirror `LocalTransactionType.WorkOrderStatusUpdate` schema.
- [ ] T122 [US6] Add `WorkOrderListViewModel.cs` and `WorkOrderDetailViewModel.cs` with CommunityToolkit commands, validation warnings, and offline hints.
- [ ] T123 [US6] Create `Views/WorkOrderListView.axaml` and `Views/WorkOrderDetailView.axaml` with compiled bindings, filters (status/date), and material breakdowns.
- [ ] T124 [US6] Integrate work order navigation into `MainView.axaml` (e.g., tab, pivot, or navigation rail) and wire commands in `MainViewModel`.
- [ ] T125 [US6] Instrument status updates with structured logging and performance metrics (response time) in `VisualApiService`/`LocalTransactionRepository`.

**Checkpoint**: Work order management complete; Part lookup + Work orders independently shippable (MVP+).

---

## Phase 9: User Story 7 - Inventory Transactions (Priority: P2)

**Goal**: Allow inventory managers to record receipts/issues/transfers, validate against cached VISUAL balances, and persist locally within 5 seconds.

**Independent Test**: Record receipt for part “12345-ABC” (qty 100) → warning when exceeding cached on-hand → transaction saved to MySQL with audit trail.

### Tests for User Story 7 (Phase 9) (write first)

- [ ] T126 [P] [US7] Add inventory contract coverage (CT-INV series) to `tests/contract/VisualApiContractTests.cs` for balance/history endpoints.
- [ ] T127 [P] [US7] Create `tests/integration/InventoryTransactionPersistenceTests.cs` verifying receipts/issues/transfers save via `LocalTransactionRepository` and warnings surface.
- [ ] T128 [P] [US7] Add `tests/unit/ViewModels/InventoryTransactionViewModelTests.cs` covering validation, warning banners, and command enablement.

### Implementation for User Story 7 (Phase 9)

- [ ] T129 [P] [US7] Implement inventory operations in `VisualApiService` and add `VisualInventoryRepository.cs` with cache integration and balance lookups.
- [ ] T130 [US7] Extend `LocalTransactionRepository` with receipt/issue/transfer persistence methods and query APIs for exports.
- [ ] T131 [US7] Implement `Services/Validation/TransactionValidator.cs` to compare requests against cached VISUAL data and emit non-blocking warnings (FR-013).
- [ ] T132 [US7] Add `ViewModels/InventoryTransactionViewModel.cs` handling receipts/issues/transfers with offline fallbacks.
- [ ] T133 [US7] Create `Views/InventoryTransactionView.axaml` with compiled bindings, validation prompts, and cached indicators.
- [ ] T134 [US7] Integrate the inventory module UI into `MainView.axaml` navigation and update export options for transaction history.

**Checkpoint**: Inventory transaction recording complete; US1–US3 can be validated end-to-end.

---

## Phase 10: User Story 8 - Shipment Confirmation (Priority: P2)

**Goal**: Let warehouse staff confirm shipments (including partials), capture tracking info, and persist data locally within 3 seconds, with export support.

**Independent Test**: Select sales order “SO-2024-100” → record partial shipment (50/100) with tracking number → confirmation saved locally and export shows entry.

### Tests for User Story 8 (Phase 10) (write first)

- [ ] T135 [P] [US8] Add shipment contract coverage (CT-ORD series) to `tests/contract/VisualApiContractTests.cs` for ready-to-ship queries.
- [ ] T136 [P] [US8] Create `tests/integration/ShipmentConfirmationTests.cs` verifying partial shipments persist and exports include tracking metadata.
- [ ] T137 [P] [US8] Add `tests/unit/ViewModels/ShipmentConfirmationViewModelTests.cs` for partial shipments, validation, and export triggers.

### Implementation for User Story 8 (Phase 10)

- [ ] T138 [P] [US8] Implement order/shipment queries in `VisualApiService` and add `VisualShipmentRepository.cs` to surface ready-to-ship lists.
- [ ] T139 [US8] Extend `LocalTransactionRepository` with shipment confirmation persistence and export query helpers (`LocalTransactionType.ShipmentConfirmation`).
- [ ] T140 [US8] Implement `ViewModels/ShipmentConfirmationViewModel.cs` with CommunityToolkit commands, partial shipment handling, and export orchestration (FR-012).
- [ ] T141 [US8] Create `Views/ShipmentConfirmationView.axaml` with compiled bindings, shipment form, and export controls.
- [ ] T142 [US8] Add shipment navigation entry to `MainView.axaml`, wire commands in `MainViewModel`, and update exports toolbar/menu.

**Checkpoint**: Shipment confirmation workflow complete; US1–US4 independently testable.

---

## Phase 11: User Story 9 - Offline Operation & Performance Monitoring (Priority: P3)

**Goal**: Provide reliable offline experience with cached data, automatic refresh after reconnection, and a Debug Terminal performance panel with degradation controls.

**Independent Test**: Disconnect VISUAL → cache banner appears instantly → operations served from cache → reconnect → auto-refresh within 30 seconds → Debug Terminal shows metrics and allows manual refresh.

### Tests for User Story 9 (Phase 11) (write first)

- [ ] T143 [P] [US9] Add `tests/integration/VisualOfflineModeTests.cs` simulating API outages, ensuring degradation mode triggers after 5 failures and auto-refresh resumes.
- [ ] T144 [P] [US9] Create `tests/unit/Services/VisualPerformanceMonitorTests.cs` covering metrics capture, degradation detection, and cache-hit tracking.
- [ ] T145 [P] [US9] Extend `tests/unit/ViewModels/DebugTerminalViewModelTests.cs` to verify VISUAL performance panel data binding and manual actions (force refresh, clear cache, export diagnostics).

### Implementation for User Story 9 (Phase 11)

- [ ] T146 [P] [US9] Implement `Services/Diagnostics/VisualPerformanceMonitor.cs` to wrap VISUAL calls, emit structured logs, and expose observable metrics collections.
- [ ] T147 [P] [US9] Implement `Services/Diagnostics/VisualDegradationDetector.cs` to monitor failures, trigger degradation mode, and raise events consumed by UI/services.
- [ ] T148 [US9] Update `VisualCacheService` and repositories to respect degradation state, surface offline banners, and queue refresh requests.
- [ ] T149 [US9] Update `ViewModels/PartLookupViewModel`, `WorkOrderViewModel`, `InventoryTransactionViewModel`, and `ShipmentConfirmationViewModel` to display offline/cached indicators and retry affordances.
- [ ] T150 [US9] Add VISUAL performance panel to `Views/DebugTerminalWindow.axaml` and enhance `ViewModels/DebugTerminalViewModel.cs` with metrics, charts, and manual actions (FR-021).
- [ ] T151 [US9] Implement auto-refresh scheduler (≤30s after reconnection) within `VisualCacheService` and surface notifications via status banners.

**Checkpoint**: Offline-ready experience delivered; all user stories complete.

---

## Phase 12: Polish & Cross-Cutting Concerns

**Purpose**: Consolidate documentation, migrations, and quality gates across the feature.

- [ ] T152 [P] [Polish] Add SQL migration `config/migrations/005_visual_local_transactions.sql` aligning with updated schema documentation and sample payloads.
- [ ] T153 [P] [Polish] Update `docs/DEBUG-TERMINAL-IMPLEMENTATION.md` and `docs/USER-GUIDE-DEBUG-TERMINAL.md` with VISUAL performance panel usage, offline workflow guidance, and troubleshooting tips.
- [ ] T154 [P] [Polish] Refresh `specs/005-migrate-infor-visual/quickstart.md` and `.github/copilot-instructions.md` with toolkit setup, navigation updates, and VISUAL read-only reminders.
- [ ] T155 [P] [Polish] Update `.github/mamp-database/schema-tables.json` examples and `docs/TROUBLESHOOTING-CATALOG.md` with VISUAL offline/degradation scenarios.
- [ ] T156 [Polish] Run `.specify/scripts/powershell/validate-implementation.ps1 -FeatureId "005-migrate-infor-visual"` and execute full `dotnet test` (include performance/category filters) capturing results for release notes.
- [ ] T157 [Polish] Prepare release notes & update `docs/FEATURES-CHECKLIST.md` with completion status for Feature 005, including success metrics instrumentation summary.

**Checkpoint**: Feature implementation ready for final review and deployment.

---

## Dependencies & Execution Order

1. **Setup (Phase 1)** → prerequisite for foundation.
2. **Foundation (Phase 2)** → blocks all user stories until complete.
3. **User Stories**:
   - **US1 (P1)** starts immediately after foundation and delivers MVP.
   - **US2 (P1)** can start after foundation; integrates with US1 but remains independently testable.
   - **US3 (P2)** depends on US1/US2 repositories and transaction scaffolding.
   - **US4 (P2)** depends on US2 (navigation patterns) and US3 (transaction persistence helpers).
   - **US5 (P3)** depends on all prior stories for UI hooks and repositories.
4. **Polish (Phase 8)** follows completion of desired user stories and final integration validation.

**Story Dependencies Graph**:
- Foundation → US1 → US2 → US3 → US4 → US5 → Polish
- US3 also requires US2’s repository extensions; US4 builds on US3’s transaction persistence.

---

## Parallel Execution Examples

### Phase 7: User Story 5 - Part Lookup
- **Parallel Tests**: T109, T110, T111 can be authored simultaneously (different files).
- **Parallel Implementation**: T112 (service/repository) and T114 (view) can progress in parallel once T113 (view model contract) is defined.

### Phase 8: User Story 6 - Work Orders
- **Parallel Tests**: T117, T118, T119.
- **Parallel Implementation**: T120 (service) and T121 (repository persistence) can proceed together; T122/T123 require both to finish.

### Phase 9: User Story 7 - Inventory
- **Parallel Tests**: T126, T127, T128.
- **Parallel Implementation**: T129 (service) and T131 (validator) can be built concurrently, feeding into T132/T133.

### Phase 10: User Story 8 - Shipments
- **Parallel Tests**: T135, T136, T137.
- **Parallel Implementation**: T138 (service) and T139 (repository extension) unblock T140/T141.

### Phase 11: User Story 9 - Offline/Performance
- **Parallel Tests**: T143, T144, T145.
- **Parallel Implementation**: T146 (performance monitor) and T147 (degradation detector) can progress concurrently; T150 (UI) waits for both.

---

## Implementation Strategy

1. **MVP First**: Complete Setup → Foundation → US5 Part Lookup (Phase 7). Verify part lookup end-to-end before continuing.
2. **Incremental Delivery**: Add US6 for work orders (still shippable), then US7 for inventory, US8 for shipments, and finally US9 for offline/performance extras.
3. **Quality Gates**: Each story completes with tests first, structured logging, and DI registration. Run validation script + targeted tests at every checkpoint.
4. **Documentation & Telemetry**: Update docs and success metrics in Polish phase to capture operational expectations (response times, cached usage, degradation controls).

---

**Total Tasks**: 157 (increased from 60 with addition of Phases 1-4)
- Phase 1 (Custom Controls): 33 tasks (T001–T033)
- Phase 2 (Settings UI): 37 tasks (T034–T070)
- Phase 3 (Debug Terminal): 17 tasks (T071–T087)
- Phase 4 (Config Dialog): 10 tasks (T088–T097)
- Phase 5 (VISUAL Setup): 2 tasks (T098–T099)
- Phase 6 (VISUAL Foundation): 9 tasks (T100–T108)
- Phase 7 (US5 Part Lookup): 8 tasks (T109–T116)
- Phase 8 (US6 Work Orders): 9 tasks (T117–T125)
- Phase 9 (US7 Inventory): 9 tasks (T126–T134)
- Phase 10 (US8 Shipments): 8 tasks (T135–T142)
- Phase 11 (US9 Offline/Performance): 9 tasks (T143–T151)
- Phase 12 (Polish): 6 tasks (T152–T157)

**Parallel Opportunities Identified**: Tests and implementation pairs per phase as detailed in phase sections above.

**Independent Test Criteria**: Documented per phase, ensuring each phase can be validated standalone.

**Suggested MVP Scope**: Complete through Phase 7 (US5 Part Lookup) after Phases 1-6 for earliest deployable value with Custom Controls Library, Settings UI, modernized Debug Terminal, and basic VISUAL part lookup integration.
