# GSC Quick Start Guide

**For Developers Using the .specify Workflow**

This guide helps you get started with GSC (GitHub Copilot Spec Commands) - the intelligent automation framework for `.specify` feature development.

---

## Installation (Once GSC is Implemented)

### Prerequisites

- PowerShell 5.1+ or PowerShell Core 7+
- MTM WIP Application repository cloned
- `.specify/` directory present in repository

### Setup

No installation required! GSC is part of the repository.

```powershell
# Verify GSC is available
cd c:\path\to\MTM_WIP_Application_Avalonia
.\..specify\scripts\gsc.ps1 help
```

### Optional: Add to PATH

For convenience, add GSC to your PATH:

```powershell
# Add to PowerShell profile
$profileContent = @"
function gsc {
    param([Parameter(ValueFromRemainingArguments=`$true)][string[]]`$args)
    & "c:\path\to\MTM_WIP_Application_Avalonia\.specify\scripts\gsc.ps1" @args
}
"@

Add-Content $PROFILE $profileContent
```

After restart, you can use:

```powershell
gsc help  # instead of .\.specify\scripts\gsc.ps1 help
```

---

## Basic Workflow: Creating a New Feature

### Step 1: Start Workflow

```powershell
gsc workflow start my-feature-name

# Example:
gsc workflow start inventory-transfer
```

**What it does:**
- Creates `.specify/specs/inventory-transfer/` directory
- Generates `spec.md` from template
- Creates initial checkpoint
- Sets workflow state to "Specification" phase

**Output:**
```
🚀 Starting workflow for feature: inventory-transfer

📁 Phase 1: Creating feature structure...
   ✅ Feature directory created
   ✅ Spec generated: .specify\specs\inventory-transfer\spec.md

💾 Checkpoint created: feature-structure-created

📝 Phase 2: Generating specification...
   ✅ Spec created: .specify\specs\inventory-transfer\spec.md
   📝 Please fill out the specification and run: gsc workflow next
```

### Step 2: Fill Out Specification

Edit `.specify/specs/inventory-transfer/spec.md`:

```markdown
# Feature Specification: Inventory Transfer

## User Story

As a **warehouse operator**  
I want **to transfer inventory between locations**  
So that **I can move parts to where they're needed**

## Acceptance Criteria

- [ ] Can select source location
- [ ] Can select destination location
- [ ] Can enter quantity to transfer
- [ ] Validates sufficient inventory at source
- [ ] Updates both locations atomically
- [ ] Logs transaction in audit trail
```

### Step 3: Check Status

```powershell
gsc status
```

**Output:**
```
╔═══════════════════════════════════════════════════════════════╗
║       FEATURE DEVELOPMENT STATUS REPORT                      ║
╚═══════════════════════════════════════════════════════════════╝

📋 Feature: inventory-transfer
🔄 Phase: Specification (Task 1 of 7)
📊 Progress: [██░░░░░░░░░░░░░░░░░░] 14%

✅ CONSTITUTIONAL COMPLIANCE:
   ⚠️  Principle I: Code Quality Excellence (Not yet implemented)
   ⚠️  Principle II: Testing Standards (No tests)
   ⚠️  Principle III: UX Consistency (No UI yet)
   ⚠️  Principle IV: Performance (Not yet implemented)

💾 CHECKPOINTS:
   1 saved checkpoints available

📝 NEXT STEPS:
   • Complete specification in spec.md
   • Run: gsc workflow next
```

### Step 4: Advance Workflow

```powershell
gsc workflow next
```

**What it does:**
- Validates specification is complete
- Generates `plan.md` from spec
- Creates checkpoint "plan-generated"
- Advances to "Planning" phase

**Output:**
```
📋 Validating specification...
   ✅ Specification is complete

📋 Phase 3: Generating implementation plan...
   ✅ Plan generated: .specify\specs\inventory-transfer\plan.md

💾 Checkpoint created: plan-generated

📝 Review plan.md and run: gsc workflow next
```

### Step 5: Continue Through Phases

Repeat `gsc workflow next` after completing each phase:

1. **Specification** → Fill out spec.md
2. **Planning** → Review plan.md
3. **Task Breakdown** → Review tasks.md
4. **Implementation** → Write code
5. **Testing** → Write tests, verify 80% coverage
6. **Validation** → Constitutional compliance check
7. **Review** → Cross-platform testing

### Step 6: Complete Workflow

```powershell
gsc workflow complete
```

**Output:**
```
🎉 Feature workflow complete: inventory-transfer

📋 Final Checklist:
   ✅ Specification complete
   ✅ Implementation plan generated
   ✅ Tasks executed
   ✅ Implementation complete
   ✅ Tests written and coverage verified
   ✅ Cross-platform validation complete
   ✅ Code review ready

📝 Next Steps:
   1. Create pull request
   2. Request code review
   3. Address review feedback
   4. Merge to master
```

---

## Essential Commands

### Checkpoints (Save Your Work)

```powershell
# Create checkpoint before risky changes
gsc rollback checkpoint "before refactoring viewmodel"

# List all checkpoints
gsc rollback list

# Restore checkpoint if something breaks
gsc rollback restore checkpoint-20251010-143022
```

### Validation (Check Compliance)

```powershell
# Full constitutional compliance check
gsc validate constitution

# Code quality only
gsc validate code

# Test coverage only
gsc validate tests
```

### Memory Access (Constitutional Guidance)

```powershell
# Display constitution
gsc memory get constitution

# Search for guidance
gsc memory search "MVVM patterns"

# List all memory files
gsc memory list
```

### Help System

```powershell
# General help
gsc help

# Command-specific help
gsc help validate
gsc help rollback
gsc help workflow

# Constitutional guidance
gsc help constitution
```

---

## Common Scenarios

### Scenario 1: "I broke something and need to undo"

```powershell
# List checkpoints
gsc rollback list

# Restore previous checkpoint
gsc rollback restore checkpoint-20251010-140500

# Verify restoration
gsc status
```

### Scenario 2: "I want to know if my code violates constitutional principles"

```powershell
# Run full validation
gsc validate constitution

# Example output:
# ❌ FAIL Principle: Code Quality Excellence
#    ⚠️  ReactiveUI patterns found in: ViewModels\InventoryViewModel.cs
#    ⚠️  ViewModel InventoryViewModel.cs not using [ObservableProperty]
```

### Scenario 3: "I'm stuck and need guidance"

```powershell
# Search memory for relevant patterns
gsc memory search "database operations"

# Get constitutional guidance
gsc help constitution

# Check workflow status
gsc status
```

### Scenario 4: "I want to experiment without breaking things"

```powershell
# Create checkpoint before experiment
gsc rollback checkpoint "before experimenting with new approach"

# Try your experiment...

# If it works, create another checkpoint
gsc rollback checkpoint "after successful experiment"

# If it fails, restore previous checkpoint
gsc rollback restore checkpoint-20251010-143500
```

### Scenario 5: "I need to see my progress"

```powershell
gsc status

# Shows:
# - Current phase
# - Progress percentage
# - Constitutional compliance status
# - Test coverage
# - Next steps
```

---

## Best Practices

### ✅ DO

- **Create checkpoints before risky changes** - Free undo capability
- **Run `gsc validate` before committing** - Catch issues early
- **Check `gsc status` regularly** - Know where you are
- **Use `gsc workflow` for new features** - Guided development
- **Search memory when stuck** - Constitutional guidance available

### ❌ DON'T

- **Don't skip validation** - Constitutional violations block PRs
- **Don't work without checkpoints** - You'll wish you had them
- **Don't ignore workflow phases** - They enforce quality gates
- **Don't assume compliance** - Always validate
- **Don't forget to complete workflow** - Marks feature ready for PR

---

## Troubleshooting

### "GSC command not found"

```powershell
# Use full path
.\.specify\scripts\gsc.ps1 help

# Or add to PATH (see Installation section)
```

### "Validation fails but I don't understand why"

```powershell
# Get detailed validation report
gsc validate constitution

# Search memory for guidance
gsc memory search "error message text"

# Get help on validation
gsc help validate
```

### "Workflow won't advance to next phase"

```powershell
# Check status for blockers
gsc status

# Review current phase requirements
gsc help workflow

# Validate current phase is complete
gsc validate
```

### "I want to start over completely"

```powershell
# ⚠️ WARNING: This resets ALL changes
gsc rollback full-reset

# Confirmation required - type 'y' to confirm
```

---

## Advanced Usage

### Creating Custom Checkpoints

```powershell
# Create named checkpoints at key milestones
gsc rollback checkpoint "viewmodel-complete"
gsc rollback checkpoint "service-layer-done"
gsc rollback checkpoint "before-ui-redesign"
```

### Validating Specific Principles

```powershell
# Check only code quality
gsc validate code

# Check only UX consistency
gsc validate ux

# Check only performance
gsc validate performance
```

### Memory System Advanced Usage

```powershell
# Get specific memory file
gsc memory get mvvm-patterns

# Append to memory (when you discover patterns)
gsc memory post lessons-learned "MVVM Community Toolkit tip: Always use [ObservableProperty] instead of manual INotifyPropertyChanged"

# Search with regex
gsc memory search "\[ObservableProperty\]"
```

---

## Tips & Tricks

### Tip 1: Alias GSC for Speed

```powershell
# In PowerShell profile
function g { gsc @args }

# Now you can use:
g status
g validate
g rollback checkpoint "quick save"
```

### Tip 2: Create Checkpoint Aliases

```powershell
function gsave { gsc rollback checkpoint $args[0] }
function gundo { gsc rollback restore $args[0] }

# Usage:
gsave "before refactor"
gundo checkpoint-20251010-143022
```

### Tip 3: Combine with Git

```powershell
# Create checkpoint before committing
gsc rollback checkpoint "before git commit"
git add .
git commit -m "Implement inventory transfer"

# If you need to undo commit AND code changes
git reset HEAD~1
gsc rollback restore checkpoint-20251010-143022
```

---

## Learning Path

### Week 1: Basics
- Learn `gsc help`, `gsc status`
- Practice `gsc workflow start`
- Create your first checkpoint

### Week 2: Validation
- Run `gsc validate` regularly
- Fix constitutional violations
- Understand all 4 principles

### Week 3: Advanced
- Use `gsc memory` for guidance
- Create strategic checkpoints
- Master workflow phases

### Week 4: Mastery
- Teach GSC to new developers
- Create custom aliases
- Contribute improvements

---

## Getting Help

### In-Command Help

```powershell
gsc help                 # General help
gsc help <command>       # Command-specific help
gsc help constitution    # Constitutional guidance
```

### Memory System

```powershell
gsc memory search "<your question>"
gsc memory get constitution
```

### Team Resources

- **Implementation Plan**: `.specify/docs/gsc-implementation-plan.md`
- **Executive Summary**: `.specify/docs/gsc-summary.md`
- **Constitution**: `.specify/memory/constitution.md`

### Contact

- **Questions**: Ask in team chat or #mtm-development
- **Issues**: Report bugs in GitHub issues
- **Improvements**: Submit PRs for new features

---

## Summary: Your GSC Cheat Sheet

```powershell
# Start new feature
gsc workflow start <feature-name>

# Check progress
gsc status

# Save your work
gsc rollback checkpoint "<description>"

# Validate compliance
gsc validate constitution

# Advance workflow
gsc workflow next

# Complete feature
gsc workflow complete

# Get help
gsc help
```

---

**Welcome to GSC! You're now ready to develop features with intelligent automation and constitutional compliance.**

*Questions? Run `gsc help` or contact your team lead.*
