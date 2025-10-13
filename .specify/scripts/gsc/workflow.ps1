<#
.SYNOPSIS
    GSC Workflow Command - Orchestrate end-to-end feature development

.DESCRIPTION
    Provides workflow orchestration for the .specify feature development process:
    - Start new feature workflows (7-phase process)
    - Advance through phases (Specification → Planning → Tasks → Implementation → Testing → Validation → Review)
    - Track progress across phases
    - Automatic checkpoint creation at phase transitions
    - Constitutional validation gates between phases

    The 7-phase workflow:
    1. Specification - Define requirements and acceptance criteria
    2. Planning - Create technical implementation plan
    3. Tasks - Generate detailed task breakdown
    4. Implementation - Execute development following plan
    5. Testing - Validate functionality and quality
    6. Validation - Constitutional compliance and cross-platform checks
    7. Review - Final validation before completion
    Provides workflow orchestration for the .specify feature development process:
    - Start new feature workflows (7-phase process)
    - Advance through phases (Specification → Planning → Tasks → Implementation → Testing → Validation → Review)
    - Track progress across phases
    - Automatic checkpoint creation at phase transitions
    - Constitutional validation gates between phases

    The 7-phase workflow:
    1. Specification - Define requirements and acceptance criteria
    2. Planning - Create technical implementation plan
    3. Tasks - Generate detailed task breakdown
    4. Implementation - Execute development following plan
    5. Testing - Validate functionality and quality
    6. Validation - Constitutional compliance and cross-platform checks
    7. Review - Final validation before completion

.PARAMETER Action
    Workflow action to perform:
    - start: Begin new feature workflow
    - next: Advance to next phase
    - status: Show current workflow state
    - complete: Mark workflow as complete
    Workflow action to perform:
    - start: Begin new feature workflow
    - next: Advance to next phase
    - status: Show current workflow state
    - complete: Mark workflow as complete

.PARAMETER FeatureName
    Name of the feature (required for 'start' action)

.EXAMPLE
    gsc workflow start inventory-transfer
    Start new workflow for inventory-transfer feature
    Name of the feature (required for 'start' action)

.EXAMPLE
    gsc workflow start inventory-transfer
    Start new workflow for inventory-transfer feature

.EXAMPLE
    gsc workflow next
    Advance to next phase in current workflow

.EXAMPLE
    gsc workflow status
    Show current workflow status

.EXAMPLE
    Advance to next phase in current workflow

.EXAMPLE
    gsc workflow status
    Show current workflow status

.EXAMPLE
    gsc workflow complete
    Mark current workflow as complete
    Mark current workflow as complete

.NOTES
    Version: 1.0.0
    Phase 6 Implementation
    Phase 6 Implementation
#>

param(
    [Parameter(Mandatory=$true, Position=0)]
    [ValidateSet('start', 'next', 'status', 'complete')]
    [string]$Action,

    [Parameter(Position=1)]
    [string]$FeatureName
)

# Import common utilities
$commonPath = Join-Path $PSScriptRoot "common"
. (Join-Path $commonPath "output.ps1")
. (Join-Path $commonPath "state.ps1")

# Determine repository root
$repoRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))

#region Helper Functions

function Start-FeatureWorkflow {
    <#
    .SYNOPSIS
        Start new feature workflow
    #>
    param([string]$FeatureName)

    Write-GscHeader -Title "Starting Workflow" -SubTitle $FeatureName

    # Validate feature name
    if (-not $FeatureName) {
        Write-GscError "Feature name is required"
        Write-Host "Usage: gsc workflow start <feature-name>" -ForegroundColor Gray
        exit 1
    }

    # Check for existing workflow
    $currentState = Get-CurrentState
    if ($currentState -and $currentState.currentFeature) {
        Write-GscWarning "Active workflow already exists: $($currentState.currentFeature)"
        Write-Host ""
        Write-Host "Options:" -ForegroundColor Gray
        Write-Host "  1. Complete current workflow: " -NoNewline -ForegroundColor Gray
        Write-Host "gsc workflow complete" -ForegroundColor Cyan
        Write-Host "  2. Archive current state manually and start new workflow" -ForegroundColor Gray
        Write-Host ""
        exit 1
    }

    # Phase 1: Create feature structure
    Write-GscSectionHeader "Phase 1: Creating Feature Structure"

    # Use existing Create.ps1 command
    $createScript = Join-Path $PSScriptRoot "Create.ps1"
    & $createScript feature $FeatureName

    if ($LASTEXITCODE -ne 0) {
        Write-GscError "Failed to create feature structure"
        exit 1
    }

    Write-Host ""

    # Create initial checkpoint
    Write-Host "📦 Creating initial checkpoint..." -ForegroundColor Cyan
    $rollbackScript = Join-Path $PSScriptRoot "Rollback.ps1"
    & $rollbackScript checkpoint "Workflow started - Feature structure created"

    # Create initial checkpoint
    Write-Host "📦 Creating initial checkpoint..." -ForegroundColor Cyan
    $rollbackScript = Join-Path $PSScriptRoot "Rollback.ps1"
    & $rollbackScript checkpoint "Workflow started - Feature structure created"

    Write-Host ""

    # Initialize workflow state
    $state = @{
        currentFeature = $FeatureName
        currentPhase = "Specification"
        currentTask = 1
        totalTasks = 7
        startTimestamp = Get-Date -Format "M/d/yyyy h:mm:ss tt"
        lastUpdateTimestamp = Get-Date -Format "M/d/yyyy h:mm:ss tt"
        modifiedFiles = @()
        checkpointsCount = 1
        constitutionalCompliance = $null
    }

    Set-CurrentState $state

    # Display next steps
    Write-GscSectionHeader "Workflow Initialized"

    Write-Host "  Feature: " -NoNewline -ForegroundColor Gray
    Write-Host $FeatureName -ForegroundColor Cyan

    Write-Host "  Phase:   " -NoNewline -ForegroundColor Gray
    Write-Host "1/7 - Specification" -ForegroundColor Yellow

    Write-Host ""
    Write-Host "📝 Next Steps:" -ForegroundColor Cyan
    Write-Host "  1. Edit spec.md in: " -NoNewline -ForegroundColor Gray
    Write-Host ".specify\specs\$FeatureName\" -ForegroundColor White
    Write-Host "  2. Define user stories and acceptance criteria" -ForegroundColor Gray
    Write-Host "  3. When complete, run: " -NoNewline -ForegroundColor Gray
    Write-Host "gsc workflow next" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "💡 Tip: View spec template guidance:" -ForegroundColor Gray
    Write-Host "   gsc memory get spec-template" -ForegroundColor Cyan
    Write-Host "💡 Tip: View spec template guidance:" -ForegroundColor Gray
    Write-Host "   gsc memory get spec-template" -ForegroundColor Cyan
    Write-Host ""

    exit 0

    exit 0
}

function Advance-Workflow {
    <#
    .SYNOPSIS
        Advance workflow to next phase
    #>

    $state = Get-CurrentState

    if (-not $state -or -not $state.currentFeature) {
        Write-GscError "No active workflow"
        Write-Host ""
        Write-Host "Start a workflow with: " -NoNewline -ForegroundColor Gray
        Write-Host "gsc workflow start <feature-name>" -ForegroundColor Cyan
        Write-Host ""
        exit 1
    }

    Write-GscHeader -Title "Advancing Workflow" -SubTitle "$($state.currentFeature) - $($state.currentPhase)"

    # Determine next phase based on current phase
    $phaseOrder = @(
        "Specification",
        "Planning",
        "Tasks",
        "Implementation",
        "Testing",
        "Validation",
        "Review"
    )

    $currentIndex = $phaseOrder.IndexOf($state.currentPhase)

    if ($currentIndex -eq -1) {
        Write-GscError "Unknown current phase: $($state.currentPhase)"
        exit 1
    }

    if ($currentIndex -eq 6) {
        Write-GscWarning "Already at final phase (Review)"
        Write-Host ""
        Write-Host "Complete the workflow with: " -NoNewline -ForegroundColor Gray
        Write-Host "gsc workflow complete" -ForegroundColor Cyan
        Write-Host ""
        exit 0
    }

    $nextPhase = $phaseOrder[$currentIndex + 1]

    # Phase-specific validation and actions
    switch ($state.currentPhase) {
        "Specification" {
            Write-GscSectionHeader "Validating Specification"

            $specPath = Join-Path $repoRoot ".specify\specs\$($state.currentFeature)\spec.md"

            if (-not (Test-Path $specPath)) {
                Write-GscError "spec.md not found"
                exit 1
            }

            $specContent = Get-Content $specPath -Raw

            # Basic validation - check for key sections
            $requiredSections = @(
                "## User Story",
                "## Acceptance Criteria",
                "## Success Criteria"
            )

            $missingSection = $false
            foreach ($section in $requiredSections) {
                if ($specContent -notmatch [regex]::Escape($section)) {
                    Write-Host "  ⚠️  Missing section: $section" -ForegroundColor Yellow
                    $missingSection = $true
                }
            }

            if ($missingSection) {
                Write-Host ""
                Write-Host "Please complete the specification before advancing." -ForegroundColor Yellow
                Write-Host "Edit: " -NoNewline -ForegroundColor Gray
                Write-Host $specPath -ForegroundColor Cyan
                Write-Host ""
                exit 1
            }

            Write-Host "  ✅ Specification complete" -ForegroundColor Green
            Write-Host ""

            # Create checkpoint
            Write-Host "📦 Creating checkpoint..." -ForegroundColor Cyan
            $rollbackScript = Join-Path $PSScriptRoot "Rollback.ps1"
            & $rollbackScript checkpoint "Specification complete - Advancing to Planning"
            Write-Host ""

            # Advance to Planning
            Write-GscSectionHeader "Phase 2: Planning"
            Write-Host "  Creating plan.md template..." -ForegroundColor Gray

            $planPath = Join-Path $repoRoot ".specify\specs\$($state.currentFeature)\plan.md"
            $planTemplate = @"
# Implementation Plan: $($state.currentFeature)

**Date**: $(Get-Date -Format "yyyy-MM-dd")
**Phase**: Planning
**Status**: In Progress

---

## Architecture Overview

[Describe high-level architecture and component interactions]

## Technical Decisions

### Decision 1: [Title]
- **Context**: [Why this decision is needed]
- **Decision**: [What was decided]
- **Rationale**: [Why this is the best choice]
- **Consequences**: [Implications and tradeoffs]

## Implementation Approach

### Component 1: [Name]
- **Purpose**: [What this component does]
- **Dependencies**: [Other components it depends on]
- **Implementation**: [How it will be built]

## Dependency Analysis

- [ ] Dependency 1
- [ ] Dependency 2

## Risks and Mitigations

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| [Risk 1] | High | Medium | [How to address] |

## Success Criteria

- [ ] Criterion 1
- [ ] Criterion 2

---

**Next Phase**: Tasks Generation
"@

            Set-Content $planPath $planTemplate -Encoding UTF8
            Write-Host "  ✅ plan.md created" -ForegroundColor Green
            Write-Host ""
        }

        "Planning" {
            Write-GscSectionHeader "Validating Plan"

            $planPath = Join-Path $repoRoot ".specify\specs\$($state.currentFeature)\plan.md"

            if (-not (Test-Path $planPath)) {
                Write-GscError "plan.md not found"
                exit 1
            }

            Write-Host "  ✅ Plan exists" -ForegroundColor Green
            Write-Host ""

            # Create checkpoint
            Write-Host "📦 Creating checkpoint..." -ForegroundColor Cyan
            $rollbackScript = Join-Path $PSScriptRoot "Rollback.ps1"
            & $rollbackScript checkpoint "Planning complete - Advancing to Tasks"
            Write-Host ""

            # Advance to Tasks
            Write-GscSectionHeader "Phase 3: Tasks"
            Write-Host "  Creating tasks.md template..." -ForegroundColor Gray

            $tasksPath = Join-Path $repoRoot ".specify\specs\$($state.currentFeature)\tasks.md"
            $tasksTemplate = @"
# Task Breakdown: $($state.currentFeature)

**Date**: $(Get-Date -Format "yyyy-MM-dd")
**Phase**: Tasks
**Status**: In Progress

---

## Task Organization

### Phase 1: Foundation
- [ ] Task 1.1: [Description]
- [ ] Task 1.2: [Description]

### Phase 2: Implementation
- [ ] Task 2.1: [Description]
- [ ] Task 2.2: [Description]

### Phase 3: Testing
- [ ] Task 3.1: [Description]
- [ ] Task 3.2: [Description]

## Task Dependencies

``````mermaid
graph TD
    A[Task 1.1] --> B[Task 2.1]
    A --> C[Task 2.2]
    B --> D[Task 3.1]
    C --> D
``````

## Parallel Execution Opportunities

Tasks that can be executed in parallel:
- Task 1.1 and Task 1.2
- Task 2.1 and Task 2.2

---

**Next Phase**: Implementation
"@

            Set-Content $tasksPath $tasksTemplate -Encoding UTF8
            Write-Host "  ✅ tasks.md created" -ForegroundColor Green
            Write-Host ""
        }

        "Tasks" {
            Write-GscSectionHeader "Validating Tasks"

            $tasksPath = Join-Path $repoRoot ".specify\specs\$($state.currentFeature)\tasks.md"

            if (-not (Test-Path $tasksPath)) {
                Write-GscError "tasks.md not found"
                exit 1
            }

            Write-Host "  ✅ Tasks defined" -ForegroundColor Green
            Write-Host ""

            # Create checkpoint
            Write-Host "📦 Creating checkpoint..." -ForegroundColor Cyan
            $rollbackScript = Join-Path $PSScriptRoot "Rollback.ps1"
            & $rollbackScript checkpoint "Tasks complete - Beginning Implementation"
            Write-Host ""

            Write-GscSectionHeader "Phase 4: Implementation"
            Write-Host "  Begin development according to tasks.md" -ForegroundColor Gray
            Write-Host "  Create checkpoints after major milestones" -ForegroundColor Gray
            Write-Host ""
        }

        "Implementation" {
            Write-GscSectionHeader "Validating Implementation"

            Write-Host "  Running code quality checks..." -ForegroundColor Gray

            # Run validation
            $validateScript = Join-Path $PSScriptRoot "Validate.ps1"
            $validationOutput = & $validateScript code 2>&1

            # Check if validation passed (exit code 0)
            if ($LASTEXITCODE -ne 0) {
                Write-Host ""
                Write-GscWarning "Code quality checks found issues"
                Write-Host ""
                Write-Host "Fix violations and run validation again:" -ForegroundColor Gray
                Write-Host "  gsc validate code" -ForegroundColor Cyan
                Write-Host ""
                Write-Host "Then advance workflow:" -ForegroundColor Gray
                Write-Host "  gsc workflow next" -ForegroundColor Cyan
                Write-Host ""
                exit 1
            }

            Write-Host "  ✅ Code quality validated" -ForegroundColor Green
            Write-Host ""

            # Create checkpoint
            Write-Host "📦 Creating checkpoint..." -ForegroundColor Cyan
            $rollbackScript = Join-Path $PSScriptRoot "Rollback.ps1"
            & $rollbackScript checkpoint "Implementation complete - Advancing to Testing"
            Write-Host ""

            Write-GscSectionHeader "Phase 5: Testing"
            Write-Host "  Write and execute tests" -ForegroundColor Gray
            Write-Host "  Verify 80% minimum coverage" -ForegroundColor Gray
            Write-Host ""
        }

        "Testing" {
            Write-GscSectionHeader "Validating Tests"

            Write-Host "  ℹ️  Manual test validation required" -ForegroundColor Cyan
            Write-Host ""
            Write-Host "Confirm test completion:" -ForegroundColor Yellow
            Write-Host "  - Have you written tests for new code?" -ForegroundColor Gray
            Write-Host "  - Have you executed all test scenarios?" -ForegroundColor Gray
            Write-Host "  - Have you documented test results?" -ForegroundColor Gray
            Write-Host ""
            Write-Host "Press Enter to confirm tests are complete, or Ctrl+C to cancel:" -ForegroundColor Yellow
            Read-Host

            Write-Host "  ✅ Tests confirmed complete" -ForegroundColor Green
            Write-Host ""

            # Create checkpoint
            Write-Host "📦 Creating checkpoint..." -ForegroundColor Cyan
            $rollbackScript = Join-Path $PSScriptRoot "Rollback.ps1"
            & $rollbackScript checkpoint "Testing complete - Advancing to Validation"
            Write-Host ""

            Write-GscSectionHeader "Phase 6: Validation"
            Write-Host "  Running full validation suite..." -ForegroundColor Gray

            # Run full validation
            $validateScript = Join-Path $PSScriptRoot "Validate.ps1"
            & $validateScript all

            Write-Host ""
        }

        "Validation" {
            Write-GscSectionHeader "Final Validation"

            Write-Host "  Running constitutional compliance check..." -ForegroundColor Gray

            # Run constitution validation
            $validateScript = Join-Path $PSScriptRoot "Validate.ps1"
            $validationOutput = & $validateScript constitution 2>&1

            if ($LASTEXITCODE -ne 0) {
                Write-Host ""
                Write-GscWarning "Constitutional compliance issues found"
                Write-Host ""
                Write-Host "Fix violations and run validation again:" -ForegroundColor Gray
                Write-Host "  gsc validate constitution" -ForegroundColor Cyan
                Write-Host ""
                exit 1
            }

            Write-Host "  ✅ Constitutional compliance validated" -ForegroundColor Green
            Write-Host ""

            # Create checkpoint
            Write-Host "📦 Creating checkpoint..." -ForegroundColor Cyan
            $rollbackScript = Join-Path $PSScriptRoot "Rollback.ps1"
            & $rollbackScript checkpoint "Validation complete - Ready for Review"
            Write-Host ""

            Write-GscSectionHeader "Phase 7: Review"
            Write-Host "  Feature ready for code review" -ForegroundColor Gray
            Write-Host "  Create pull request" -ForegroundColor Gray
            Write-Host ""
        }
    }

    # Update state to next phase
    $state.currentPhase = $nextPhase
    $state.currentTask = $currentIndex + 2
    $state.lastUpdateTimestamp = Get-Date -Format "M/d/yyyy h:mm:ss tt"

    Set-CurrentState $state

    # Display current status
    Write-GscSectionHeader "Workflow Status"

    Write-Host "  Feature:      " -NoNewline -ForegroundColor Gray
    Write-Host $state.currentFeature -ForegroundColor Cyan

    Write-Host "  Current Phase:" -NoNewline -ForegroundColor Gray
    Write-Host " $($state.currentTask)/7 - $nextPhase" -ForegroundColor Yellow

    # Progress bar
    $percentage = [math]::Round(($state.currentTask / $state.totalTasks) * 100)
    $barWidth = 40
    $filled = [math]::Floor(($percentage / 100) * $barWidth)
    $empty = $barWidth - $filled

    Write-Host "  Progress:     " -NoNewline -ForegroundColor Gray
    Write-Host "[" -NoNewline
    Write-Host ("█" * $filled) -NoNewline -ForegroundColor Green
    Write-Host ("░" * $empty) -NoNewline -ForegroundColor DarkGray
    Write-Host "] $percentage%" -ForegroundColor White

    Write-Host ""

    # Phase-specific next steps
    $nextSteps = Get-PhaseNextSteps -Phase $nextPhase

    Write-Host "📝 Next Steps:" -ForegroundColor Cyan
    foreach ($step in $nextSteps) {
        Write-Host "  • $step" -ForegroundColor Gray
    }

    Write-Host ""

    if ($nextPhase -eq "Review") {
        Write-Host "When ready to complete:" -ForegroundColor Gray
        Write-Host "  gsc workflow complete" -ForegroundColor Cyan
    } else {
        Write-Host "When complete:" -ForegroundColor Gray
        Write-Host "  gsc workflow next" -ForegroundColor Cyan
    }

    Write-Host ""

    exit 0
}

function Show-WorkflowStatus {
    <#
    .SYNOPSIS
        Show current workflow status (delegates to Status.ps1)
    #>

    $statusScript = Join-Path $PSScriptRoot "Status.ps1"
    & $statusScript
}

function Complete-FeatureWorkflow {
    <#
    .SYNOPSIS
        Complete current workflow
    #>

    $state = Get-CurrentState

    if (-not $state -or -not $state.currentFeature) {
        Write-GscError "No active workflow"
        exit 1
    }

    Write-GscHeader -Title "Completing Workflow" -SubTitle $state.currentFeature

    # Final validation
    Write-GscSectionHeader "Final Validation"

    Write-Host "  Running constitutional compliance check..." -ForegroundColor Gray
    $validateScript = Join-Path $PSScriptRoot "Validate.ps1"
    $validationOutput = & $validateScript constitution 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-GscWarning "Final validation failed"
        Write-Host ""
        Write-Host "Fix violations before completing workflow:" -ForegroundColor Gray
        Write-Host "  gsc validate constitution" -ForegroundColor Cyan
        Write-Host ""
        exit 1
    }

    Write-Host "  ✅ Final validation passed" -ForegroundColor Green
    Write-Host ""

    # Create final checkpoint
    Write-Host "📦 Creating final checkpoint..." -ForegroundColor Cyan
    $rollbackScript = Join-Path $PSScriptRoot "Rollback.ps1"
    & $rollbackScript checkpoint "Workflow complete - Feature ready for merge"
    Write-Host ""

    # Display completion summary
    Write-GscSectionHeader "Workflow Complete"

    Write-Host "  Feature:    " -NoNewline -ForegroundColor Gray
    Write-Host $state.currentFeature -ForegroundColor Green

    Write-Host "  Started:    " -NoNewline -ForegroundColor Gray
    Write-Host $state.startTimestamp -ForegroundColor White

    Write-Host "  Completed:  " -NoNewline -ForegroundColor Gray
    Write-Host (Get-Date -Format "M/d/yyyy h:mm:ss tt") -ForegroundColor White

    $duration = (Get-Date) - [DateTime]::Parse($state.startTimestamp)
    Write-Host "  Duration:   " -NoNewline -ForegroundColor Gray
    Write-Host "$([math]::Round($duration.TotalHours, 1)) hours" -ForegroundColor White

    Write-Host ""

    Write-Host "📋 Checklist:" -ForegroundColor Cyan
    Write-Host "  ✅ Specification complete" -ForegroundColor Green
    Write-Host "  ✅ Planning complete" -ForegroundColor Green
    Write-Host "  ✅ Tasks defined" -ForegroundColor Green
    Write-Host "  ✅ Implementation complete" -ForegroundColor Green
    Write-Host "  ✅ Tests written and executed" -ForegroundColor Green
    Write-Host "  ✅ Constitutional compliance validated" -ForegroundColor Green
    Write-Host "  ✅ Ready for code review" -ForegroundColor Green

    Write-Host ""

    Write-Host "📝 Next Steps:" -ForegroundColor Cyan
    Write-Host "  1. Create pull request" -ForegroundColor White
    Write-Host "  2. Request code review" -ForegroundColor White
    Write-Host "  3. Address review feedback" -ForegroundColor White
    Write-Host "  4. Merge to master" -ForegroundColor White

    Write-Host ""

    # Archive workflow state
    $completedDir = Join-Path (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) "state\completed"
    if (-not (Test-Path $completedDir)) {
        New-Item -ItemType Directory -Path $completedDir -Force | Out-Null
    }

    $archivePath = Join-Path $completedDir "$($state.currentFeature)-$(Get-Date -Format 'yyyyMMdd-HHmmss').json"
    $state | ConvertTo-Json -Depth 10 | Set-Content $archivePath -Encoding UTF8

    Write-Host "  📦 Workflow archived: $archivePath" -ForegroundColor Gray
    Write-Host ""

    # Clear current state
    $statePath = Join-Path (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) "state\current-state.json"
    Remove-Item $statePath -Force -ErrorAction SilentlyContinue

    Write-Host "✨ Workflow complete!" -ForegroundColor Green
    Write-Host ""

    exit 0
}

function Get-PhaseNextSteps {
    <#
    .SYNOPSIS
        Get phase-specific next steps
    #>
    param([string]$Phase)

    $nextSteps = @{
        'Specification' = @(
            "Edit spec.md in .specify\specs\<feature>\"
            "Define user stories and acceptance criteria"
            "Document success criteria"
            "Run: gsc workflow next"
        )
        'Planning' = @(
            "Edit plan.md in .specify\specs\<feature>\"
            "Define architecture and technical decisions"
            "Identify dependencies and risks"
            "Run: gsc workflow next"
        )
        'Tasks' = @(
            "Edit tasks.md in .specify\specs\<feature>\"
            "Break down work into concrete tasks"
            "Identify task dependencies"
            "Run: gsc workflow next"
        )
        'Implementation' = @(
            "Execute tasks according to tasks.md"
            "Create checkpoints after milestones: gsc rollback checkpoint"
            "Test incrementally as you build"
            "Run: gsc workflow next when implementation complete"
        )
        'Testing' = @(
            "Write and execute test scenarios"
            "Document test results"
            "Verify minimum 80% coverage"
            "Run: gsc workflow next when testing complete"
        )
        'Validation' = @(
            "Run: gsc validate all"
            "Fix any constitutional violations"
            "Test on all supported platforms"
            "Run: gsc workflow next when validated"
        )
        'Review' = @(
            "Review all implementation files"
            "Verify constitutional compliance"
            "Create pull request"
            "Run: gsc workflow complete when ready"
        )
    }

    if ($nextSteps.ContainsKey($Phase)) {
        return $nextSteps[$Phase]
    }

    return @("Unknown phase")
}

#endregion

#region Main Workflow Logic

switch ($Action) {
    'start' {
        Start-FeatureWorkflow -FeatureName $FeatureName
    }
    'next' {
        Advance-Workflow
    }
    'status' {
        Show-WorkflowStatus
    }
    'complete' {
        Complete-FeatureWorkflow
    }
}

#endregion
