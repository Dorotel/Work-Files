````prompt
---
description: Implement the GSC (GitHub Copilot Spec Commands) system with intelligent progress detection and phase-by-phase execution
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

---

## Overview

This prompt implements the **GSC Command System** as defined in `.specify/docs/gsc-implementation-plan.md`. It intelligently detects current implementation progress and continues from the appropriate phase, ensuring incremental development with validation gates.

**GSC System**: A unified command-line interface providing 7 core commands (`create`, `validate`, `status`, `rollback`, `memory`, `workflow`, `help`) that automate the `.specify` workflow with constitutional compliance enforcement and checkpoint safety mechanisms.

---

## Phase 0: Progress Assessment & Context Loading

### 0.1 Load Implementation Plan

**Action**: Read the complete implementation plan to understand all phases and deliverables.

```
Read: .specify/docs/gsc-implementation-plan.md
```

**Extract**:
- Phase definitions (1-8)
- Deliverables per phase (files, functions, features)
- Success criteria for each phase
- Dependencies between phases

### 0.2 Detect Current Implementation State

**Scan for GSC components** in order of implementation phases:

#### Phase 1-2: Foundation & Memory System

Check for existence of:
- [ ] `.specify/scripts/gsc.ps1` (entry point)
- [ ] `.specify/scripts/gsc/create.ps1`
- [ ] `.specify/scripts/gsc/memory.ps1`
- [ ] `.specify/scripts/gsc/help.ps1`
- [ ] `.specify/scripts/gsc/common/state.ps1`
- [ ] `.specify/scripts/gsc/common/output.ps1`

#### Phase 3: State Management & Rollback

Check for:
- [ ] `.specify/scripts/gsc/rollback.ps1`
- [ ] `.specify/state/` directory structure
- [ ] Checkpoint creation functions
- [ ] Checkpoint restoration functions

#### Phase 4: Validation System

Check for:
- [ ] `.specify/scripts/gsc/validate.ps1`
- [ ] `.specify/scripts/gsc/common/constitution.ps1`
- [ ] Constitutional compliance functions (Test-CodeQuality, Test-TestingStandards, Test-UXConsistency, Test-Performance)

#### Phase 5: Status & Progress Tracking

Check for:
- [ ] `.specify/scripts/gsc/status.ps1`
- [ ] Progress tracking integration
- [ ] Metrics collection functions

#### Phase 6: Workflow Orchestration

Check for:
- [ ] `.specify/scripts/gsc/workflow.ps1`
- [ ] Phase transition logic
- [ ] Workflow state management

#### Phase 7-8: Documentation & Polish

Check for:
- [ ] `docs/gsc-enhancement-system.md`
- [ ] `docs/gsc-interactive-help.html`
- [ ] Updated AGENTS.md with GSC references
- [ ] Updated constitution.md (warning status changed to ✅)

### 0.3 Determine Starting Phase

**Logic**:

```
IF no GSC files exist:
    START_PHASE = "Phase 1: Foundation"
    MESSAGE = "🚀 Starting GSC implementation from scratch"

ELSE IF gsc.ps1 exists BUT rollback.ps1 doesn't exist:
    START_PHASE = "Phase 3: State Management"
    MESSAGE = "📦 Foundation complete. Beginning state management system"

ELSE IF rollback.ps1 exists BUT validate.ps1 doesn't exist:
    START_PHASE = "Phase 4: Validation"
    MESSAGE = "✅ State management complete. Beginning validation system"

ELSE IF validate.ps1 exists BUT status.ps1 doesn't exist:
    START_PHASE = "Phase 5: Status Tracking"
    MESSAGE = "🔍 Validation complete. Beginning status tracking"

ELSE IF status.ps1 exists BUT workflow.ps1 doesn't exist:
    START_PHASE = "Phase 6: Workflow Orchestration"
    MESSAGE = "📊 Status tracking complete. Beginning workflow orchestration"

ELSE IF workflow.ps1 exists BUT docs/gsc-enhancement-system.md doesn't exist:
    START_PHASE = "Phase 7-8: Documentation"
    MESSAGE = "📝 Core implementation complete. Beginning documentation"

ELSE IF all components exist:
    START_PHASE = "Phase 9: Verification & Testing"
    MESSAGE = "✨ GSC system fully implemented. Running verification tests"

ELSE:
    START_PHASE = "Unknown - Manual assessment required"
    MESSAGE = "⚠️ Partial implementation detected. Analyzing..."
```

### 0.4 Display Progress Assessment

**Output**:

```
═══════════════════════════════════════════════════════════════
       GSC COMMAND SYSTEM IMPLEMENTATION PROGRESS
═══════════════════════════════════════════════════════════════

📋 Current State Analysis:

Phase 1-2: Foundation & Memory System
   ├─ [X/✓/ ] gsc.ps1 entry point
   ├─ [X/✓/ ] gsc/create.ps1
   ├─ [X/✓/ ] gsc/memory.ps1
   ├─ [X/✓/ ] gsc/help.ps1
   └─ [X/✓/ ] gsc/common/ utilities

Phase 3: State Management
   ├─ [X/✓/ ] gsc/rollback.ps1
   ├─ [X/✓/ ] .specify/state/ structure
   └─ [X/✓/ ] Checkpoint functions

Phase 4: Validation System
   ├─ [X/✓/ ] gsc/validate.ps1
   ├─ [X/✓/ ] Constitution compliance checks
   └─ [X/✓/ ] Validation reporting

Phase 5: Status Tracking
   ├─ [X/✓/ ] gsc/status.ps1
   └─ [X/✓/ ] Progress metrics

Phase 6: Workflow Orchestration
   ├─ [X/✓/ ] gsc/workflow.ps1
   └─ [X/✓/ ] Phase transitions

Phase 7-8: Documentation
   ├─ [X/✓/ ] docs/gsc-enhancement-system.md
   ├─ [X/✓/ ] docs/gsc-interactive-help.html
   └─ [X/✓/ ] AGENTS.md updates

═══════════════════════════════════════════════════════════════
🎯 NEXT ACTION: {START_PHASE}
═══════════════════════════════════════════════════════════════
```

Legend:
- `[X]` = Complete
- `[✓]` = Partial (file exists but may need enhancement)
- `[ ]` = Not started

### 0.5 User Confirmation

**Ask user**:

```
{MESSAGE}

Current assessment shows implementation should continue with: {START_PHASE}

Options:
1. Continue with recommended phase
2. Start from a specific phase (specify which)
3. Show detailed analysis of current implementation
4. Skip to verification testing (if core complete)

How would you like to proceed? (1/2/3/4)
```

Wait for user response before proceeding.

---

## Phase 1-2: Foundation & Memory System Implementation

**Goal**: Create GSC entry point, command routing, memory system integration, and help system.

**Execute only if**: Phase 1-2 not complete (gsc.ps1 or memory.ps1 or help.ps1 missing)

### 1.1 Create GSC Entry Point

**File**: `.specify/scripts/gsc.ps1`

**Implementation**:

```powershell
<#
.SYNOPSIS
    GSC (GitHub Copilot Spec Commands) - Unified command interface for .specify workflow

.DESCRIPTION
    Main entry point for GSC command system. Routes commands to appropriate handlers.

.PARAMETER Command
    The GSC command to execute: create, validate, status, rollback, memory, workflow, help

.PARAMETER Arguments
    Arguments to pass to the command handler

.EXAMPLE
    .\gsc.ps1 help
    .\gsc.ps1 workflow start my-feature
    .\gsc.ps1 validate constitution

.NOTES
    Version: 1.0.0
    Author: MTM Development Team
#>

param(
    [Parameter(Mandatory=$false, Position=0)]
    [ValidateSet('create', 'validate', 'status', 'rollback', 'memory', 'workflow', 'help', '')]
    [string]$Command = 'help',
    
    [Parameter(ValueFromRemainingArguments=$true)]
    [string[]]$Arguments
)

# Set error action preference
$ErrorActionPreference = 'Stop'

# Get GSC root directory
$gscRoot = $PSScriptRoot
$gscCommandsDir = Join-Path $gscRoot "gsc"

# Validate command directory exists
if (-not (Test-Path $gscCommandsDir)) {
    Write-Error "GSC commands directory not found: $gscCommandsDir"
    Write-Host "Please ensure GSC system is fully installed." -ForegroundColor Yellow
    exit 1
}

# Route to command handler
$commandScript = Join-Path $gscCommandsDir "$Command.ps1"

if (-not (Test-Path $commandScript)) {
    Write-Error "Command not found: $Command"
    Write-Host "Available commands: create, validate, status, rollback, memory, workflow, help" -ForegroundColor Yellow
    Write-Host "Run 'gsc help' for more information" -ForegroundColor Cyan
    exit 1
}

# Execute command with arguments
try {
    & $commandScript @Arguments
}
catch {
    Write-Error "Error executing command '$Command': $_"
    Write-Host "`nFor help, run: gsc help $Command" -ForegroundColor Cyan
    exit 1
}
```

**Validation**: Test that `gsc.ps1 help` runs without errors.

### 1.2 Create Common Utilities

#### 1.2.1 Output Formatting Utilities

**File**: `.specify/scripts/gsc/common/output.ps1`

**Functions**:
- `Write-GscHeader` - Formatted headers with borders
- `Write-GscSuccess` - Green success messages with ✅
- `Write-GscError` - Red error messages with ❌
- `Write-GscWarning` - Yellow warnings with ⚠️
- `Write-GscInfo` - Cyan info messages with ℹ️
- `Write-GscProgress` - Progress bars with percentage

**Implementation**: Follow patterns from implementation plan Phase 1 section.

#### 1.2.2 State Management Utilities

**File**: `.specify/scripts/gsc/common/state.ps1`

**Functions**:
- `Get-CurrentState` - Read current workflow state
- `Set-CurrentState` - Update workflow state
- `Get-StateDirectory` - Get `.specify/state/` path
- `Initialize-StateDirectory` - Create state directory structure

**Implementation**: Create JSON-based state persistence.

### 1.3 Implement Memory Command

**File**: `.specify/scripts/gsc/memory.ps1`

**Operations**: get, post, search, list

**Implementation**: Follow Phase 2 section of implementation plan (lines ~75-160).

**Key Functions**:
- `Get-Memory` - Read memory file content
- `Set-Memory` - Write/append to memory file
- `Search-Memory` - Full-text search across memory files
- `Get-MemoryList` - List all available memory files

**Validation**:
- `gsc memory get constitution` returns constitution content
- `gsc memory list` shows all .md files in `.specify/memory/`
- `gsc memory search "MVVM"` returns relevant matches

### 1.4 Implement Help Command

**File**: `.specify/scripts/gsc/help.ps1`

**Topics**: General help, command-specific help, constitutional guidance

**Implementation**: Follow Phase 7 help system section (lines ~650-750).

**Help Content**:
- General overview of GSC
- Command reference with examples
- Constitutional principles summary
- Links to documentation files

**Validation**: `gsc help validate` displays validation command help.

### 1.5 Implement Create Command (Basic)

**File**: `.specify/scripts/gsc/create.ps1`

**Operations**: feature, spec, tasks

**Integration**: Wrap existing `.specify/scripts/powershell/create-new-feature.ps1`

**Implementation**: Route `gsc create feature <name>` to existing PowerShell script.

### 1.6 Phase 1-2 Validation Gate

**Checklist**:
- [ ] `gsc help` displays general help
- [ ] `gsc help memory` displays memory command help
- [ ] `gsc memory list` shows memory files
- [ ] `gsc memory get constitution` displays constitution
- [ ] `gsc memory search "test"` returns search results
- [ ] `gsc create feature test-feature` creates feature structure
- [ ] All scripts have proper error handling
- [ ] All scripts use common output utilities

**Output**: Display validation results. If all pass, proceed to Phase 3.

---

## Phase 3: State Management & Rollback Implementation

**Goal**: Implement checkpoint system for safe experimentation with rollback capabilities.

**Execute only if**: Phase 1-2 complete AND Phase 3 not complete (rollback.ps1 missing)

### 3.1 Create State Directory Structure

**Action**: Create `.specify/state/` directory hierarchy:

```
.specify/state/
├── checkpoints/          # Checkpoint storage
├── current-state.json    # Active workflow state
└── history.log           # State change history
```

### 3.2 Implement Rollback Command

**File**: `.specify/scripts/gsc/rollback.ps1`

**Operations**: checkpoint, list, restore, full-reset

**Implementation**: Follow Phase 3 section of implementation plan (lines ~175-290).

**Key Functions**:
- `New-Checkpoint` - Create checkpoint with file snapshots
- `Restore-Checkpoint` - Restore files from checkpoint
- `Get-Checkpoints` - List all available checkpoints
- `Remove-AllCheckpoints` - Full reset (with confirmation)

**Critical Features**:
- Snapshot modified files in checkpoint directory
- Store metadata (timestamp, description, modified files list)
- Restore files atomically (all or nothing)
- Confirmation prompt for full-reset operation

### 3.3 Checkpoint Metadata Schema

**Format**: JSON file in each checkpoint directory

```json
{
    "id": "checkpoint-20251010-143022",
    "timestamp": "2025-10-10T14:30:22Z",
    "description": "after viewmodel generation",
    "feature": "inventory-transfer",
    "phase": "Implementation",
    "modifiedFiles": [
        "ViewModels/InventoryTransferViewModel.cs",
        "Views/InventoryTransferView.axaml",
        ".specify/state/current-state.json"
    ],
    "fileCount": 3,
    "sizeBytes": 45230
}
```

### 3.4 Phase 3 Validation Gate

**Checklist**:
- [ ] `gsc rollback checkpoint "test"` creates checkpoint
- [ ] `.specify/state/checkpoints/checkpoint-*/` directory created
- [ ] Metadata JSON file contains all required fields
- [ ] Modified files are copied to checkpoint/files/
- [ ] `gsc rollback list` displays all checkpoints in table format
- [ ] `gsc rollback restore <id>` successfully restores files
- [ ] `gsc rollback full-reset` prompts for confirmation
- [ ] File restoration is atomic (all files or none)

**Output**: Display validation results. If all pass, proceed to Phase 4.

---

## Phase 4: Validation System Implementation

**Goal**: Automate constitutional compliance checking with multi-level validation.

**Execute only if**: Phase 3 complete AND Phase 4 not complete (validate.ps1 missing)

### 4.1 Implement Validate Command

**File**: `.specify/scripts/gsc/validate.ps1`

**Targets**: constitution, code, tests, cross-platform, full

**Implementation**: Follow Phase 4 section of implementation plan (lines ~310-480).

### 4.2 Create Constitution Compliance Module

**File**: `.specify/scripts/gsc/common/constitution.ps1`

**Functions** (one per constitutional principle):

#### 4.2.1 Principle I: Code Quality Excellence

**Function**: `Test-CodeQuality`

**Checks**:
- [ ] Nullable reference types enabled in .csproj
- [ ] No ReactiveUI patterns (ReactiveObject, ReactiveCommand, RaiseAndSetIfChanged)
- [ ] ViewModels use [ObservableProperty] attribute
- [ ] No direct exception throwing (use Services.ErrorHandling)
- [ ] Dependency injection used in constructors

**Returns**: `@{ Passed = $true/$false; Violations = @(...) }`

#### 4.2.2 Principle II: Testing Standards

**Function**: `Test-TestingStandards`

**Checks**:
- [ ] tests/ directory exists
- [ ] Test coverage ≥ 80% (if coverage tooling available)
- [ ] Critical paths have ≥ 95% coverage
- [ ] Test naming conventions followed

#### 4.2.3 Principle III: UX Consistency

**Function**: `Test-UXConsistency`

**Checks**:
- [ ] No hardcoded colors in AXAML (must use DynamicResource)
- [ ] All UserControls have x:DataType attribute
- [ ] Material Design icons used consistently
- [ ] Theme V2 resources referenced correctly

#### 4.2.4 Principle IV: Performance Requirements

**Function**: `Test-Performance`

**Checks**:
- [ ] No synchronous database calls (.Result, .Wait())
- [ ] Connection pooling configured in appsettings.json
- [ ] Async/await patterns used for I/O operations
- [ ] No blocking UI thread operations

### 4.3 Validation Reporting Format

**Output Structure**:

```
═══════════════════════════════════════════════════════════════
       CONSTITUTIONAL COMPLIANCE REPORT
═══════════════════════════════════════════════════════════════

✅ PASS: Principle I - Code Quality Excellence
   ✓ Nullable reference types enabled
   ✓ No ReactiveUI patterns found
   ✓ MVVM Community Toolkit patterns used
   ✓ Centralized error handling

❌ FAIL: Principle III - UX Consistency
   ⚠️  Hardcoded colors found in:
       - Views/InventoryView.axaml (line 45: Background="#FFFFFF")
       - Views/TransferView.axaml (line 78: Foreground="#000000")
   ⚠️  Missing x:DataType in:
       - Views/InventoryView.axaml
       - Views/StatusView.axaml

✅ PASS: Principle II - Testing Standards
✅ PASS: Principle IV - Performance Requirements

═══════════════════════════════════════════════════════════════
OVERALL RESULT: ❌ FAIL (2 principles passed, 2 failed)
═══════════════════════════════════════════════════════════════

📝 Recommendations:
   1. Replace hardcoded colors with Theme V2 DynamicResource
   2. Add x:DataType attributes to UserControl elements
   3. Re-run validation: gsc validate constitution
```

### 4.4 Phase 4 Validation Gate

**Checklist**:
- [ ] `gsc validate constitution` runs all 4 principle checks
- [ ] `gsc validate code` runs only code quality check
- [ ] Validation detects ReactiveUI patterns correctly
- [ ] Validation detects hardcoded colors in AXAML
- [ ] Validation detects missing x:DataType attributes
- [ ] Validation detects synchronous database calls
- [ ] Violations include file names and line numbers (where applicable)
- [ ] Validation report is clearly formatted with colors

**Output**: Display validation results. If all pass, proceed to Phase 5.

---

## Phase 5: Status & Progress Tracking Implementation

**Goal**: Provide comprehensive visibility into feature development progress.

**Execute only if**: Phase 4 complete AND Phase 5 not complete (status.ps1 missing)

### 5.1 Implement Status Command

**File**: `.specify/scripts/gsc/status.ps1`

**Implementation**: Follow Phase 5 section of implementation plan (lines ~500-590).

### 5.2 Status Report Components

**Sections to display**:

1. **Feature Information**
   - Current feature name
   - Current phase
   - Task progress (X of Y tasks complete)

2. **Progress Visualization**
   - Percentage complete
   - ASCII progress bar: `[████████░░░░░░░░░░░░] 40%`

3. **Constitutional Compliance Status**
   - Live results from `Test-ConstitutionalCompliance`
   - Green checkmarks for passed principles
   - Yellow warnings for failed principles

4. **Test Coverage Metrics**
   - Current coverage percentage
   - Target coverage (80% minimum, 95% critical paths)
   - Color-coded status (green ≥80%, red <80%)

5. **Checkpoints Available**
   - Count of saved checkpoints
   - Most recent checkpoint timestamp

6. **Blockers & Warnings**
   - List of current issues preventing progress
   - Dependencies not met

7. **Next Steps**
   - Recommended actions based on current phase
   - Commands to run next

### 5.3 State Integration

**Read from**: `.specify/state/current-state.json`

**State Schema**:

```json
{
    "currentFeature": "inventory-transfer",
    "currentPhase": "Implementation",
    "currentTask": 5,
    "totalTasks": 12,
    "startTimestamp": "2025-10-10T10:00:00Z",
    "lastUpdateTimestamp": "2025-10-10T14:30:22Z",
    "modifiedFiles": ["file1.cs", "file2.axaml"],
    "checkpointsCount": 3
}
```

### 5.4 Phase 5 Validation Gate

**Checklist**:
- [ ] `gsc status` displays when no feature is active
- [ ] `gsc status` shows all 7 sections when feature is active
- [ ] Progress bar renders correctly at 0%, 50%, 100%
- [ ] Constitutional compliance shows live results
- [ ] Test coverage displays correctly (if available)
- [ ] Checkpoint count is accurate
- [ ] Next steps provide actionable recommendations
- [ ] All output uses color-coded formatting

**Output**: Display validation results. If all pass, proceed to Phase 6.

---

## Phase 6: Workflow Orchestration Implementation

**Goal**: Automate end-to-end feature development workflow with guided phase transitions.

**Execute only if**: Phase 5 complete AND Phase 6 not complete (workflow.ps1 missing)

### 6.1 Implement Workflow Command

**File**: `.specify/scripts/gsc/workflow.ps1`

**Actions**: start, next, complete, list

**Implementation**: Follow Phase 6 section of implementation plan (lines ~600-780).

### 6.2 Workflow Phases

**Phase Sequence**:

1. **Specification** - Generate spec.md from template
2. **Planning** - Create plan.md from spec
3. **Task Breakdown** - Generate tasks.md from plan
4. **Implementation** - Execute tasks
5. **Testing** - Verify test coverage
6. **Validation** - Run constitutional compliance
7. **Review** - Cross-platform verification

### 6.3 Phase Transition Logic

**Function**: `Continue-Workflow`

**Logic**:
- Read current phase from state
- Validate current phase is complete
- Create checkpoint before advancing
- Execute next phase initialization
- Update state with new phase
- Display next steps

**Validation Gates**:
- Specification phase requires complete spec.md
- Planning phase requires reviewed plan.md
- Implementation phase requires passing validation
- Testing phase requires ≥80% coverage

### 6.4 Workflow State Persistence

**Update**: `.specify/state/current-state.json` after each phase transition

**Fields to update**:
- `currentPhase`
- `currentTask`
- `lastUpdateTimestamp`
- `modifiedFiles`

### 6.5 Phase 6 Validation Gate

**Checklist**:
- [ ] `gsc workflow start test-feature` creates feature structure
- [ ] State JSON created with initial values
- [ ] `gsc workflow next` validates current phase before advancing
- [ ] Checkpoint created automatically before phase transition
- [ ] State updated correctly after phase transition
- [ ] `gsc workflow next` displays appropriate next steps
- [ ] `gsc workflow complete` displays final checklist
- [ ] `gsc workflow list` shows all active workflows
- [ ] Validation gates prevent premature advancement

**Output**: Display validation results. If all pass, proceed to Phase 7-8.

---

## Phase 7-8: Documentation & Polish Implementation

**Goal**: Create comprehensive documentation and finalize system polish.

**Execute only if**: Phase 6 complete AND Phase 7-8 not complete (docs incomplete)

### 7.1 Create Enhancement System Documentation

**File**: `docs/gsc-enhancement-system.md`

**Sections**:
1. **Architecture Overview** - Command structure, state management, file organization
2. **Command Reference** - Detailed documentation for all 7 commands
3. **Workflow Examples** - Common scenarios with command sequences
4. **Best Practices** - Tips for effective GSC usage
5. **Troubleshooting** - Common issues and solutions
6. **Extension Guide** - How to add new commands

**Length**: ~3000-4000 words with code examples

### 7.2 Create Interactive Help HTML

**File**: `docs/gsc-interactive-help.html`

**Features**:
- Searchable command reference
- Visual workflow diagrams (SVG or Mermaid)
- Interactive examples (copy-to-clipboard buttons)
- Constitutional principles reference
- Navigation sidebar
- Responsive design for mobile/desktop

**Technologies**: HTML5, CSS3, JavaScript (no frameworks required)

### 7.3 Update AGENTS.md

**Changes**:
1. Update GSC automation helpers section (lines ~68-73):
   - Change status from "New" to "✅ Implemented"
   - Add usage examples for each command

2. Update tool descriptions section:
   - Add GSC command system to tool inventory
   - Reference new documentation files

3. Update development workflow section:
   - Add GSC workflow examples
   - Reference gsc-quickstart.md

**Validation**: Search AGENTS.md for "GSC" to ensure all references are updated.

### 7.4 Update Constitution

**File**: `.specify/memory/constitution.md`

**Changes**:
1. Update warning status (line ~24):
   ```markdown
   - ✅ **Command templates** - GSC command system fully implemented; 
     see `.specify/scripts/gsc/` and documentation in `docs/gsc-enhancement-system.md`
   ```

2. Add GSC reference to enforcement mechanisms section:
   ```markdown
   ### Enforcement Mechanisms
   
   - **GSC Validation Command**: `gsc validate constitution` automatically checks 
     all constitutional principles before feature completion
   - **Workflow Gates**: `gsc workflow next` validates compliance before phase transitions
   - **Checkpoint System**: `gsc rollback` enables safe experimentation without 
     violating constitutional standards
   ```

### 7.5 Phase 7-8 Validation Gate

**Checklist**:
- [ ] `docs/gsc-enhancement-system.md` exists and is complete
- [ ] `docs/gsc-interactive-help.html` exists and opens in browser
- [ ] AGENTS.md updated with GSC implementation status
- [ ] Constitution.md warning changed to ✅ status
- [ ] All documentation has proper headers and formatting
- [ ] All code examples in docs are tested and correct
- [ ] Interactive help HTML is responsive and functional

**Output**: Display validation results. If all pass, proceed to Phase 9.

---

## Phase 9: Verification & Testing

**Goal**: Comprehensive testing of the complete GSC system.

**Execute only if**: All previous phases complete.

### 9.1 Functional Testing

**Test Suite**:

#### Test 1: Entry Point
```powershell
# Should display help
.\..specify\scripts\gsc.ps1
.\..specify\scripts\gsc.ps1 help

# Should route to command
.\..specify\scripts\gsc.ps1 memory list
```

#### Test 2: Memory System
```powershell
# Should list memory files
gsc memory list

# Should display constitution
gsc memory get constitution

# Should search and return results
gsc memory search "MVVM"

# Should append to memory (if implemented)
gsc memory post test-memory "Test entry"
```

#### Test 3: Rollback System
```powershell
# Create test file
echo "test content" > test-file.txt

# Create checkpoint
gsc rollback checkpoint "test checkpoint"

# Modify file
echo "modified content" > test-file.txt

# List checkpoints
gsc rollback list

# Restore checkpoint
gsc rollback restore checkpoint-<id>

# Verify file restored
cat test-file.txt  # Should show "test content"
```

#### Test 4: Validation System
```powershell
# Should run all checks
gsc validate constitution

# Should run specific check
gsc validate code

# Should detect violations (test with sample violations)
```

#### Test 5: Status System
```powershell
# Should show "no active feature" message
gsc status

# After starting workflow, should show status
gsc workflow start test-feature
gsc status
```

#### Test 6: Workflow System
```powershell
# Should create feature structure
gsc workflow start test-feature-workflow

# Should show current phase
gsc status

# Should advance phase
gsc workflow next

# Should prevent advancement if validation fails
gsc workflow next  # (with invalid spec)
```

#### Test 7: Help System
```powershell
# Should display general help
gsc help

# Should display command help
gsc help validate
gsc help rollback
gsc help workflow

# Should display constitutional guidance
gsc help constitution
```

### 9.2 Integration Testing

**Test Scenarios**:

1. **Complete Feature Workflow**
   - Start workflow
   - Create checkpoints at key points
   - Validate at each phase
   - Check status throughout
   - Complete workflow successfully

2. **Error Recovery**
   - Start feature
   - Make mistake
   - Create checkpoint before fix
   - Validate failure detection
   - Rollback if needed
   - Fix and re-validate

3. **Cross-Command Integration**
   - Use `gsc memory` to get constitutional guidance
   - Use `gsc validate` to check compliance
   - Use `gsc rollback checkpoint` before risky changes
   - Use `gsc status` to track progress
   - Use `gsc workflow next` to advance phases

### 9.3 Performance Testing

**Metrics**:
- Command response time (should be <2 seconds for all commands)
- Checkpoint creation time (should be <5 seconds for typical feature)
- Validation time (should be <10 seconds for full constitution check)
- Memory search time (should be <1 second)

### 9.4 Documentation Testing

**Validation**:
- [ ] All examples in gsc-quickstart.md execute correctly
- [ ] All examples in gsc-enhancement-system.md execute correctly
- [ ] Interactive help HTML loads without errors
- [ ] All links in documentation are valid
- [ ] Code syntax highlighting works

### 9.5 Final Validation Gate

**Comprehensive Checklist**:

✅ **Core Functionality**
- [ ] All 7 commands work correctly
- [ ] Command routing is reliable
- [ ] Error handling is comprehensive
- [ ] Output formatting is consistent

✅ **State Management**
- [ ] Checkpoints save correctly
- [ ] Checkpoints restore correctly
- [ ] State persistence works across sessions
- [ ] History logging is accurate

✅ **Validation System**
- [ ] All 4 principles check correctly
- [ ] Violations are detected accurately
- [ ] Reports are clear and actionable
- [ ] False positives are minimal

✅ **Workflow Orchestration**
- [ ] All 7 phases transition correctly
- [ ] Validation gates work properly
- [ ] Checkpoints auto-created
- [ ] State updates accurately

✅ **Documentation**
- [ ] All docs are complete
- [ ] All examples work
- [ ] Links are valid
- [ ] HTML help is functional

✅ **User Experience**
- [ ] Commands are intuitive
- [ ] Error messages are helpful
- [ ] Output is readable
- [ ] Colors enhance clarity

---

## Phase 10: Completion Report

### 10.1 Generate Implementation Summary

**Output**:

```
═══════════════════════════════════════════════════════════════
     GSC COMMAND SYSTEM IMPLEMENTATION COMPLETE! 🎉
═══════════════════════════════════════════════════════════════

✅ Phase 1-2: Foundation & Memory System
   ✓ GSC entry point: .specify/scripts/gsc.ps1
   ✓ Memory system: gsc/memory.ps1
   ✓ Help system: gsc/help.ps1
   ✓ Common utilities: gsc/common/

✅ Phase 3: State Management & Rollback
   ✓ Rollback system: gsc/rollback.ps1
   ✓ Checkpoint functionality complete
   ✓ State directory structure created

✅ Phase 4: Validation System
   ✓ Validation system: gsc/validate.ps1
   ✓ Constitutional compliance: gsc/common/constitution.ps1
   ✓ All 4 principles validated

✅ Phase 5: Status & Progress Tracking
   ✓ Status system: gsc/status.ps1
   ✓ Progress metrics implemented
   ✓ Real-time reporting functional

✅ Phase 6: Workflow Orchestration
   ✓ Workflow system: gsc/workflow.ps1
   ✓ 7-phase workflow automated
   ✓ Validation gates enforced

✅ Phase 7-8: Documentation & Polish
   ✓ Enhancement docs: docs/gsc-enhancement-system.md
   ✓ Interactive help: docs/gsc-interactive-help.html
   ✓ AGENTS.md updated
   ✓ Constitution.md updated

✅ Phase 9: Verification & Testing
   ✓ All functional tests passed
   ✓ Integration tests passed
   ✓ Documentation validated

═══════════════════════════════════════════════════════════════

📊 FINAL STATISTICS:

Files Created: {count}
Lines of Code: {count}
Commands Implemented: 7
Validation Checks: 4 principles
Documentation Pages: 4

═══════════════════════════════════════════════════════════════

🚀 NEXT STEPS:

1. Try the system:
   gsc help                     # See all commands
   gsc workflow start my-feature  # Start your first feature

2. Read documentation:
   docs/gsc-quickstart.md       # Quick start guide
   docs/gsc-enhancement-system.md  # Complete reference

3. Share with team:
   - Announce GSC availability
   - Provide training session
   - Collect feedback for improvements

4. Update constitution warning if not done automatically

═══════════════════════════════════════════════════════════════

✨ GSC Command System is ready for use! ✨

═══════════════════════════════════════════════════════════════
```

### 10.2 Create Changelog Entry

**File**: Add to repository CHANGELOG.md or create `.specify/CHANGELOG.md`

**Entry**:

```markdown
## [1.0.0] - 2025-10-10

### Added - GSC Command System

**GSC (GitHub Copilot Spec Commands)** - Unified command-line interface for .specify workflow

#### Core Commands
- `gsc create` - Feature/spec/task creation
- `gsc validate` - Constitutional compliance checking
- `gsc status` - Progress tracking and metrics
- `gsc rollback` - Checkpoint system for safe experimentation
- `gsc memory` - Memory system integration
- `gsc workflow` - End-to-end workflow orchestration
- `gsc help` - Interactive help system

#### Features
- Automated constitutional compliance validation (4 principles)
- Checkpoint system with file snapshots
- Real-time progress tracking with visual indicators
- 7-phase guided workflow with validation gates
- Memory system integration for constitutional guidance
- Comprehensive error handling and user feedback
- Cross-platform PowerShell implementation

#### Documentation
- Complete implementation plan (gsc-implementation-plan.md)
- Executive summary (gsc-summary.md)
- Quick start guide (gsc-quickstart.md)
- Enhancement system docs (gsc-enhancement-system.md)
- Interactive help HTML (gsc-interactive-help.html)

#### Impact
- 30-40% faster feature development
- 50%+ reduction in constitutional violations
- 100% checkpoint usage for safe experimentation
- 80%+ consistent test coverage achievement

See `.specify/docs/gsc-*.md` for complete documentation.
```

### 10.3 Final User Instructions

**Display**:

```
═══════════════════════════════════════════════════════════════

🎓 GETTING STARTED WITH GSC

For quick start, run:
   gsc help

For your first feature:
   gsc workflow start my-first-feature

For documentation:
   - Quick start: .specify/docs/gsc-quickstart.md
   - Complete guide: .specify/docs/gsc-enhancement-system.md
   - Interactive help: .specify/docs/gsc-interactive-help.html

For team onboarding:
   - Share gsc-quickstart.md with developers
   - Schedule training session (optional)
   - Set up team chat channel for GSC questions

═══════════════════════════════════════════════════════════════

Need help? Run: gsc help <command>

═══════════════════════════════════════════════════════════════
```

---

## Error Handling & Recovery

### Common Issues

#### Issue 1: "GSC command not found"

**Diagnosis**: gsc.ps1 not in expected location

**Resolution**:
1. Verify `.specify/scripts/gsc.ps1` exists
2. Check execution policy: `Get-ExecutionPolicy`
3. If needed: `Set-ExecutionPolicy -Scope CurrentUser RemoteSigned`
4. Run with full path: `.\.specify\scripts\gsc.ps1 help`

#### Issue 2: "Command handler not found"

**Diagnosis**: Individual command script missing (e.g., memory.ps1)

**Resolution**:
1. Check which phase is incomplete
2. Re-run implementation from that phase
3. Verify directory structure: `.specify/scripts/gsc/*.ps1`

#### Issue 3: Validation fails unexpectedly

**Diagnosis**: Constitutional compliance checks too strict or false positives

**Resolution**:
1. Review validation logic in `gsc/common/constitution.ps1`
2. Add exceptions for legitimate patterns
3. Update validation rules to be more precise
4. Document exceptions in comments

#### Issue 4: Checkpoint restoration fails

**Diagnosis**: File permissions or missing checkpoint files

**Resolution**:
1. Verify checkpoint metadata exists
2. Check file paths are absolute
3. Verify permissions on checkpoint directory
4. Check disk space for file copies

### Rollback Strategy

If GSC implementation encounters critical issues:

1. **Partial Implementation**:
   - Keep completed phases
   - Fix issues in current phase
   - Resume from fixed point

2. **Complete Rollback** (last resort):
   - Remove `.specify/scripts/gsc/` directory
   - Remove `.specify/state/` directory
   - Restore constitution warning to original state
   - Return to using PowerShell scripts directly

---

## Success Criteria

**Implementation is complete when**:

1. ✅ All 7 commands execute without errors
2. ✅ All validation gates pass
3. ✅ Documentation is complete and accurate
4. ✅ All examples in documentation work correctly
5. ✅ Constitution warning updated to ✅ status
6. ✅ AGENTS.md references GSC system
7. ✅ Interactive help HTML is functional
8. ✅ A complete feature workflow can be executed end-to-end
9. ✅ Constitutional compliance is automatically validated
10. ✅ Checkpoints can be created and restored successfully

**Quality Criteria**:
- All PowerShell scripts follow best practices
- Error messages are clear and actionable
- Output formatting is consistent across commands
- Performance is acceptable (<2s for most commands)
- Documentation is comprehensive and well-organized

---

## Notes

- This prompt is designed for incremental execution - it can be run multiple times
- Each phase has validation gates to ensure quality before proceeding
- User can choose to start from any phase if manual assessment needed
- All PowerShell code should follow MTM standards
- Constitutional principles are enforced throughout implementation
- State management enables resuming after interruptions

**Estimated Total Time**: 20-30 hours of AI-assisted implementation across all 9 phases

---

````
