<#
.SYNOPSIS
    GSC Template Functions - Centralized template generation for workflow phases

.DESCRIPTION
    Provides template generation functions for spec.md, plan.md, tasks.md, and validation.md
    Used by workflow.ps1 and create.ps1 to maintain consistent template structures

.NOTES
    Version: 1.0.0
    Created: 2025-10-10
    Templates extracted from workflow.ps1 to eliminate duplication
#>

function Get-PlanTemplate {
    <#
    .SYNOPSIS
        Generate plan.md template content

    .PARAMETER FeatureName
        Name of the feature for the plan

    .EXAMPLE
        $content = Get-PlanTemplate -FeatureName "inventory-export"
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$FeatureName
    )

    return @"
# Implementation Plan: $FeatureName

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
}

function Get-TasksTemplate {
    <#
    .SYNOPSIS
        Generate tasks.md template content

    .PARAMETER FeatureName
        Name of the feature for the task breakdown

    .EXAMPLE
        $content = Get-TasksTemplate -FeatureName "inventory-export"
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$FeatureName
    )

    return @"
# Task Breakdown: $FeatureName

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
}

function Get-SpecTemplate {
    <#
    .SYNOPSIS
        Generate spec.md template content

    .PARAMETER FeatureName
        Name of the feature for the specification

    .EXAMPLE
        $content = Get-SpecTemplate -FeatureName "inventory-export"

    .NOTES
        This template aligns with constitutional principles and includes
        user stories, acceptance criteria, success criteria, and research sections
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$FeatureName
    )

    return @"
# Feature Specification: $FeatureName

**Date**: $(Get-Date -Format "yyyy-MM-dd")
**Phase**: Specification
**Status**: In Progress

---

## User Stories

### Story 1: [Title]
**As a** [user role]
**I want** [capability]
**So that** [benefit]

### Story 2: [Title]
**As a** [user role]
**I want** [capability]
**So that** [benefit]

---

## Acceptance Criteria

### Story 1
- [ ] Criterion 1.1: [Specific testable requirement]
- [ ] Criterion 1.2: [Specific testable requirement]

### Story 2
- [ ] Criterion 2.1: [Specific testable requirement]
- [ ] Criterion 2.2: [Specific testable requirement]

---

## Success Criteria

**SC-001: [Measurable Outcome]**
- **Target**: [Specific target metric or behavior]
- **Validation**: [How to measure/verify]
- **Pass**: [Success threshold]

**SC-002: [Measurable Outcome]**
- **Target**: [Specific target metric or behavior]
- **Validation**: [How to measure/verify]
- **Pass**: [Success threshold]

---

## Research Findings

### Technology Research
- **Topic**: [Technology/Library investigated]
- **Findings**: [What was learned]
- **Decision**: [How this impacts implementation]

### Pattern Research
- **Pattern**: [Design pattern or approach]
- **Applicability**: [How it fits this feature]
- **References**: [Documentation, examples, similar implementations]

---

## Constitutional Alignment

### Principle I: Code Quality Excellence
- [ ] MVVM Community Toolkit patterns planned
- [ ] Nullable reference types considered
- [ ] Error handling approach defined

### Principle II: Testing Standards
- [ ] Test coverage strategy defined (≥80% target)
- [ ] Critical path tests identified (≥95% target)
- [ ] Cross-platform testing planned

### Principle III: UX Consistency
- [ ] Avalonia UI standards planned
- [ ] Theme V2 integration planned
- [ ] Material Design icons specified

### Principle IV: Performance Requirements
- [ ] Database query timeout considered (≤30s)
- [ ] UI responsiveness planned (<100ms)
- [ ] Connection pooling configured

---

**Next Phase**: Planning
"@
}

function Get-ValidationTemplate {
    <#
    .SYNOPSIS
        Generate validation.md template content

    .PARAMETER FeatureName
        Name of the feature for validation

    .EXAMPLE
        $content = Get-ValidationTemplate -FeatureName "inventory-export"

    .NOTES
        Validation template for Testing and Validation phases
        Includes success criteria verification and cross-platform testing
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$FeatureName
    )

    return @"
# Validation Checklist: $FeatureName

**Date**: $(Get-Date -Format "yyyy-MM-dd")
**Phase**: Validation
**Status**: In Progress

---

## Success Criteria Verification

### SC-001: [Criterion Name]
- **Status**: ⬜ Pass / ⬜ Fail
- **Validation Method**: [How verified]
- **Results**: [Actual outcome]
- **Notes**: [Any observations]

### SC-002: [Criterion Name]
- **Status**: ⬜ Pass / ⬜ Fail
- **Validation Method**: [How verified]
- **Results**: [Actual outcome]
- **Notes**: [Any observations]

---

## Constitutional Compliance

### ✅ Principle I: Code Quality Excellence
- [ ] MVVM Community Toolkit patterns used correctly
- [ ] `[ObservableProperty]` for bindable properties
- [ ] `[RelayCommand]` for command implementations
- [ ] ArgumentNullException.ThrowIfNull in constructors
- [ ] Nullable reference types handled

### ✅ Principle II: Testing Standards
- [ ] Test coverage ≥80% achieved
- [ ] Critical paths ≥95% coverage achieved
- [ ] Cross-platform testing completed (Windows, macOS, Linux)
- [ ] Manufacturing domain validation passed

### ✅ Principle III: UX Consistency
- [ ] Avalonia UI standards followed
- [ ] No hardcoded colors (Theme V2 used)
- [ ] Material Design icons used
- [ ] x:DataType attributes present
- [ ] Theme switching tested

### ✅ Principle IV: Performance Requirements
- [ ] Database queries <30s
- [ ] UI interactions <100ms
- [ ] Connection pooling validated (5-100 connections)
- [ ] No memory leaks detected
- [ ] Cross-platform performance parity verified

---

## Cross-Platform Testing

### Windows
- **Version**: [Windows 10/11]
- **Status**: ⬜ Pass / ⬜ Fail
- **Notes**: [Observations]

### macOS
- **Version**: [Intel/Apple Silicon]
- **Status**: ⬜ Pass / ⬜ Fail
- **Notes**: [Observations]

### Linux
- **Version**: [Ubuntu LTS version]
- **Status**: ⬜ Pass / ⬜ Fail
- **Notes**: [Observations]

---

## Manufacturing Domain Validation

### Operations Validation
- [ ] Operation codes validated (90=Move, 100=Receive, 110=Ship)
- [ ] Location codes validated (FLOOR, RECEIVING, SHIPPING)
- [ ] Transaction types correct (IN, OUT, TRANSFER)

### Session Management
- [ ] 8+ hour session stability verified
- [ ] Session timeout (60 min) tested
- [ ] Auto-save functionality validated

### Inventory Accuracy
- [ ] Quantity tracking accurate
- [ ] Part validation working
- [ ] Transaction audit trail complete

---

## Build Verification

- [ ] Project compiles without errors
- [ ] No AVLN2000 binding errors
- [ ] No new compiler warnings introduced
- [ ] NuGet packages up-to-date

---

## Code Review Checklist

- [ ] Code follows MVVM patterns
- [ ] Proper dependency injection used
- [ ] Error handling comprehensive
- [ ] XML documentation complete
- [ ] No ReactiveUI patterns present
- [ ] Security best practices followed

---

## Final Approval

**Reviewer**: [Name]
**Date**: [Date]
**Status**: ⬜ Approved / ⬜ Rejected / ⬜ Needs Changes

**Comments**:
[Reviewer feedback]

---

**Next Phase**: Review → Archive
"@
}
