<#
.SYNOPSIS
    GSC Help Command - Interactive help system

.DESCRIPTION
    Provides comprehensive help for all GSC commands and constitutional guidance

.PARAMETER Topic
    Help topic: general, command name, or 'constitution'

.EXAMPLE
    gsc help
    gsc help validate
    gsc help constitution

.NOTES
    Version: 1.0.0
#>

param(
    [Parameter(Mandatory=$false, Position=0)]
    [string]$Topic = ""
)

# Import common utilities
$commonPath = Join-Path $PSScriptRoot "common"
. (Join-Path $commonPath "output.ps1")

function Show-GeneralHelp {
    Write-GscHeader -Title "GSC Command System Help" -SubTitle "GitHub Copilot Spec Commands v1.0.0"

    Write-Host "GSC is an intelligent development assistant for the .specify workflow." -ForegroundColor Gray
    Write-Host "It enforces constitutional compliance and automates feature development." -ForegroundColor Gray
    Write-Host ""

    Write-Host "AVAILABLE COMMANDS:" -ForegroundColor Yellow
    Write-Host ""

    Write-Host "  create" -ForegroundColor Cyan -NoNewline
    Write-Host "      Create new features, specs, or tasks"
    Write-Host "              " -NoNewline
    Write-Host "Usage: gsc create feature <name>" -ForegroundColor Gray
    Write-Host ""

    Write-Host "  validate" -ForegroundColor Cyan -NoNewline
    Write-Host "    Validate constitutional compliance"
    Write-Host "              " -NoNewline
    Write-Host "Usage: gsc validate constitution" -ForegroundColor Gray
    Write-Host ""

    Write-Host "  status" -ForegroundColor Cyan -NoNewline
    Write-Host "      Show current progress and state"
    Write-Host "              " -NoNewline
    Write-Host "Usage: gsc status" -ForegroundColor Gray
    Write-Host ""

    Write-Host "  rollback" -ForegroundColor Cyan -NoNewline
    Write-Host "    Manage checkpoints and rollback changes"
    Write-Host "              " -NoNewline
    Write-Host "Usage: gsc rollback checkpoint <description>" -ForegroundColor Gray
    Write-Host ""

    Write-Host "  memory" -ForegroundColor Cyan -NoNewline
    Write-Host "      Access constitutional memory system"
    Write-Host "              " -NoNewline
    Write-Host "Usage: gsc memory list" -ForegroundColor Gray
    Write-Host ""

    Write-Host "  workflow" -ForegroundColor Cyan -NoNewline
    Write-Host "    Orchestrate end-to-end feature development"
    Write-Host "              " -NoNewline
    Write-Host "Usage: gsc workflow start <feature-name>" -ForegroundColor Gray
    Write-Host ""

    Write-Host "  housekeeping" -ForegroundColor Cyan -NoNewline
    Write-Host " Documentation inventory and cleanup"
    Write-Host "              " -NoNewline
    Write-Host "Usage: gsc housekeeping inventory" -ForegroundColor Gray
    Write-Host ""

    Write-Host "  help" -ForegroundColor Cyan -NoNewline
    Write-Host "        Display help information"
    Write-Host "              " -NoNewline
    Write-Host "Usage: gsc help <command>" -ForegroundColor Gray
    Write-Host ""

    Write-Host "GETTING STARTED:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  1. View constitutional guidance:" -ForegroundColor White
    Write-Host "     gsc memory get constitution" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  2. Start a new feature:" -ForegroundColor White
    Write-Host "     gsc workflow start my-feature" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  3. Check progress:" -ForegroundColor White
    Write-Host "     gsc status" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  4. Validate compliance:" -ForegroundColor White
    Write-Host "     gsc validate constitution" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "DOCUMENTATION:" -ForegroundColor Yellow
    Write-Host "  .specify/docs/gsc-quickstart.md" -ForegroundColor Gray
    Write-Host "  .specify/docs/gsc-enhancement-system.md" -ForegroundColor Gray
    Write-Host "  .specify/docs/gsc-interactive-help.html" -ForegroundColor Gray
    Write-Host ""

    Write-Host "For detailed help on a specific command:" -ForegroundColor Gray
    Write-Host "  gsc help <command>" -ForegroundColor Cyan
    Write-Host ""
}

function Show-CreateHelp {
    Write-GscHeader -Title "GSC Create Command" -SubTitle "Create new features, specs, or tasks"

    Write-Host "DESCRIPTION:" -ForegroundColor Yellow
    Write-Host "  Creates new feature structures, specification files, or task breakdowns" -ForegroundColor Gray
    Write-Host "  following constitutional templates and best practices." -ForegroundColor Gray
    Write-Host ""

    Write-Host "USAGE:" -ForegroundColor Yellow
    Write-Host "  gsc create feature <feature-name>" -ForegroundColor Cyan
    Write-Host "  gsc create spec <feature-name>" -ForegroundColor Cyan
    Write-Host "  gsc create tasks <feature-name>" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "EXAMPLES:" -ForegroundColor Yellow
    Write-Host "  # Create new feature with full structure" -ForegroundColor Gray
    Write-Host "  gsc create feature inventory-export" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # Create only specification file" -ForegroundColor Gray
    Write-Host "  gsc create spec inventory-export" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "WHAT IT CREATES:" -ForegroundColor Yellow
    Write-Host "  specs/<feature-name>/" -ForegroundColor White
    Write-Host "    ├── spec.md           (Feature specification)" -ForegroundColor Gray
    Write-Host "    ├── plan.md           (Implementation plan)" -ForegroundColor Gray
    Write-Host "    ├── tasks.md          (Task breakdown)" -ForegroundColor Gray
    Write-Host "    └── validation.md     (Validation checklist)" -ForegroundColor Gray
    Write-Host ""

    Write-Host "CONSTITUTIONAL COMPLIANCE:" -ForegroundColor Yellow
    Write-Host "  ✓ Uses approved templates from .specify/templates/" -ForegroundColor Green
    Write-Host "  ✓ Enforces naming conventions" -ForegroundColor Green
    Write-Host "  ✓ Includes success criteria patterns" -ForegroundColor Green
    Write-Host "  ✓ Auto-generates validation checklists" -ForegroundColor Green
    Write-Host ""
}

function Show-ValidateHelp {
    Write-GscHeader -Title "GSC Validate Command" -SubTitle "Validate constitutional compliance"

    Write-Host "DESCRIPTION:" -ForegroundColor Yellow
    Write-Host "  Validates code, tests, and implementation against constitutional principles." -ForegroundColor Gray
    Write-Host "  Detects violations and provides actionable recommendations." -ForegroundColor Gray
    Write-Host ""

    Write-Host "USAGE:" -ForegroundColor Yellow
    Write-Host "  gsc validate constitution    " -ForegroundColor Cyan -NoNewline
    Write-Host "# All 4 principles" -ForegroundColor Gray
    Write-Host "  gsc validate code            " -ForegroundColor Cyan -NoNewline
    Write-Host "# Code quality only" -ForegroundColor Gray
    Write-Host "  gsc validate tests           " -ForegroundColor Cyan -NoNewline
    Write-Host "# Testing standards only" -ForegroundColor Gray
    Write-Host "  gsc validate ux              " -ForegroundColor Cyan -NoNewline
    Write-Host "# UX consistency only" -ForegroundColor Gray
    Write-Host "  gsc validate performance     " -ForegroundColor Cyan -NoNewline
    Write-Host "# Performance requirements only" -ForegroundColor Gray
    Write-Host ""

    Write-Host "PRINCIPLES VALIDATED:" -ForegroundColor Yellow
    Write-Host "  I.   Code Quality Excellence" -ForegroundColor White
    Write-Host "       • No ReactiveUI patterns" -ForegroundColor Gray
    Write-Host "       • MVVM Community Toolkit used" -ForegroundColor Gray
    Write-Host "       • Centralized error handling" -ForegroundColor Gray
    Write-Host "       • Nullable reference types enabled" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  II.  Testing Standards" -ForegroundColor White
    Write-Host "       • Test coverage ≥ 80%" -ForegroundColor Gray
    Write-Host "       • Critical paths ≥ 95%" -ForegroundColor Gray
    Write-Host "       • Test naming conventions" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  III. UX Consistency" -ForegroundColor White
    Write-Host "       • No hardcoded colors (use Theme V2)" -ForegroundColor Gray
    Write-Host "       • x:DataType attributes present" -ForegroundColor Gray
    Write-Host "       • Material Design icons used" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  IV.  Performance Requirements" -ForegroundColor White
    Write-Host "       • No synchronous database calls" -ForegroundColor Gray
    Write-Host "       • Connection pooling configured" -ForegroundColor Gray
    Write-Host "       • Async/await patterns followed" -ForegroundColor Gray
    Write-Host ""

    Write-Host "EXAMPLES:" -ForegroundColor Yellow
    Write-Host "  gsc validate constitution" -ForegroundColor Cyan
    Write-Host "  gsc validate code" -ForegroundColor Cyan
    Write-Host ""
}

function Show-MemoryHelp {
    Write-GscHeader -Title "GSC Memory Command" -SubTitle "Access constitutional memory system"

    Write-Host "DESCRIPTION:" -ForegroundColor Yellow
    Write-Host "  Provides access to .specify/memory/ constitutional guidance documents." -ForegroundColor Gray
    Write-Host "  Use this to reference best practices and lessons learned." -ForegroundColor Gray
    Write-Host ""

    Write-Host "USAGE:" -ForegroundColor Yellow
    Write-Host "  gsc memory list                  " -ForegroundColor Cyan -NoNewline
    Write-Host "# List all memory files" -ForegroundColor Gray
    Write-Host "  gsc memory get <name>            " -ForegroundColor Cyan -NoNewline
    Write-Host "# Display file content" -ForegroundColor Gray
    Write-Host "  gsc memory search <term>         " -ForegroundColor Cyan -NoNewline
    Write-Host "# Search all files" -ForegroundColor Gray
    Write-Host "  gsc memory post <name> <content> " -ForegroundColor Cyan -NoNewline
    Write-Host "# Append to file" -ForegroundColor Gray
    Write-Host ""

    Write-Host "EXAMPLES:" -ForegroundColor Yellow
    Write-Host "  # View constitutional principles" -ForegroundColor Gray
    Write-Host "  gsc memory get constitution" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # Search for MVVM patterns" -ForegroundColor Gray
    Write-Host "  gsc memory search MVVM" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # List all available guidance" -ForegroundColor Gray
    Write-Host "  gsc memory list" -ForegroundColor Cyan
    Write-Host ""
}

function Show-RollbackHelp {
    Write-GscHeader -Title "GSC Rollback Command" -SubTitle "Manage checkpoints and rollback changes"

    Write-Host "DESCRIPTION:" -ForegroundColor Yellow
    Write-Host "  Creates checkpoints for safe experimentation and provides rollback" -ForegroundColor Gray
    Write-Host "  capabilities to restore previous states." -ForegroundColor Gray
    Write-Host ""

    Write-Host "USAGE:" -ForegroundColor Yellow
    Write-Host "  gsc rollback checkpoint <description>" -ForegroundColor Cyan
    Write-Host "  gsc rollback list" -ForegroundColor Cyan
    Write-Host "  gsc rollback restore <id>" -ForegroundColor Cyan
    Write-Host "  gsc rollback full-reset" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "EXAMPLES:" -ForegroundColor Yellow
    Write-Host "  # Create checkpoint before risky change" -ForegroundColor Gray
    Write-Host "  gsc rollback checkpoint \"before refactoring\"" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # List all checkpoints" -ForegroundColor Gray
    Write-Host "  gsc rollback list" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # Restore specific checkpoint" -ForegroundColor Gray
    Write-Host "  gsc rollback restore checkpoint-20251010-143022" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "CHECKPOINT CONTENTS:" -ForegroundColor Yellow
    Write-Host "  • Snapshot of all modified files" -ForegroundColor Gray
    Write-Host "  • Metadata (timestamp, description, file list)" -ForegroundColor Gray
    Write-Host "  • Current workflow state" -ForegroundColor Gray
    Write-Host ""
}

function Show-WorkflowHelp {
    Write-GscHeader -Title "GSC Workflow Command" -SubTitle "Orchestrate end-to-end feature development"

    Write-Host "DESCRIPTION:" -ForegroundColor Yellow
    Write-Host "  Automates the 7-phase .specify workflow with validation gates and" -ForegroundColor Gray
    Write-Host "  checkpoint creation at each phase transition." -ForegroundColor Gray
    Write-Host ""

    Write-Host "USAGE:" -ForegroundColor Yellow
    Write-Host "  gsc workflow start <feature-name>" -ForegroundColor Cyan
    Write-Host "  gsc workflow next" -ForegroundColor Cyan
    Write-Host "  gsc workflow complete" -ForegroundColor Cyan
    Write-Host "  gsc workflow list" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "WORKFLOW PHASES:" -ForegroundColor Yellow
    Write-Host "  1. Specification    → Generate spec.md" -ForegroundColor White
    Write-Host "  2. Planning         → Create plan.md" -ForegroundColor White
    Write-Host "  3. Task Breakdown   → Generate tasks.md" -ForegroundColor White
    Write-Host "  4. Implementation   → Execute tasks" -ForegroundColor White
    Write-Host "  5. Testing          → Verify coverage" -ForegroundColor White
    Write-Host "  6. Validation       → Check compliance" -ForegroundColor White
    Write-Host "  7. Review           → Cross-platform verification" -ForegroundColor White
    Write-Host ""

    Write-Host "EXAMPLES:" -ForegroundColor Yellow
    Write-Host "  # Start new feature workflow" -ForegroundColor Gray
    Write-Host "  gsc workflow start inventory-export" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # Advance to next phase (with validation)" -ForegroundColor Gray
    Write-Host "  gsc workflow next" -ForegroundColor Cyan
    Write-Host ""
}

function Show-HousekeepingHelp {
    Write-GscHeader -Title "GSC Housekeeping Command" -SubTitle "Documentation inventory and cleanup"

    Write-Host "DESCRIPTION:" -ForegroundColor Yellow
    Write-Host "  Manages documentation lifecycle: inventory, prune obsolete docs," -ForegroundColor Gray
    Write-Host "  archive files, and purge old checkpoints." -ForegroundColor Gray
    Write-Host ""

    Write-Host "USAGE:" -ForegroundColor Yellow
    Write-Host "  gsc housekeeping inventory              " -ForegroundColor Cyan -NoNewline
    Write-Host "# Scan and report" -ForegroundColor Gray
    Write-Host "  gsc housekeeping prune                  " -ForegroundColor Cyan -NoNewline
    Write-Host "# Archive obsolete docs" -ForegroundColor Gray
    Write-Host "  gsc housekeeping archive                " -ForegroundColor Cyan -NoNewline
    Write-Host "# Full backup archive" -ForegroundColor Gray
    Write-Host "  gsc housekeeping purge-checkpoints      " -ForegroundColor Cyan -NoNewline
    Write-Host "# Remove old checkpoints" -ForegroundColor Gray
    Write-Host ""

    Write-Host "SAFE BY DEFAULT:" -ForegroundColor Yellow
    Write-Host "  All destructive operations default to DRY-RUN mode." -ForegroundColor White
    Write-Host "  Use --DryRun:`$false to execute changes." -ForegroundColor White
    Write-Host ""

    Write-Host "EXAMPLES:" -ForegroundColor Yellow
    Write-Host "  # Create documentation inventory" -ForegroundColor Gray
    Write-Host "  gsc housekeeping inventory" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # Preview what would be pruned (dry-run)" -ForegroundColor Gray
    Write-Host "  gsc housekeeping prune" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # Actually prune obsolete documentation" -ForegroundColor Gray
    Write-Host "  gsc housekeeping prune --DryRun:`$false" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # Purge checkpoints older than 14 days" -ForegroundColor Gray
    Write-Host "  gsc housekeeping purge-checkpoints --DryRun:`$false" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # Purge ALL checkpoints" -ForegroundColor Gray
    Write-Host "  gsc housekeeping purge-checkpoints --All --DryRun:`$false" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "OPERATIONS:" -ForegroundColor Yellow
    Write-Host "  inventory            Scan all documentation and checkpoints" -ForegroundColor White
    Write-Host "                       Creates: .specify\state\reports\docs-inventory.json" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  prune                Archive documentation not referenced in active workflows" -ForegroundColor White
    Write-Host "                       Safe: archives to .specify\archive\<timestamp>\" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  archive              Full backup of all documentation" -ForegroundColor White
    Write-Host "                       Archives both required and candidate files" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  purge-checkpoints    Remove old checkpoints (14-day retention default)" -ForegroundColor White
    Write-Host "                       Use --All to purge all checkpoints" -ForegroundColor Gray
    Write-Host ""

    Write-Host "RETENTION POLICY:" -ForegroundColor Yellow
    Write-Host "  Checkpoints:  14 days (configurable)" -ForegroundColor White
    Write-Host "  Archives:     Permanent (manual cleanup)" -ForegroundColor White
    Write-Host ""
}

function Show-ConstitutionHelp {
    Write-GscHeader -Title "Constitutional Principles" -SubTitle "Core development standards"

    Write-Host "The MTM WIP Application follows four constitutional principles:" -ForegroundColor Gray
    Write-Host ""

    Write-Host "PRINCIPLE I: CODE QUALITY EXCELLENCE" -ForegroundColor Yellow
    Write-Host "  • Use MVVM Community Toolkit (not ReactiveUI)" -ForegroundColor White
    Write-Host "  • Enable nullable reference types" -ForegroundColor White
    Write-Host "  • Centralized error handling via Services.ErrorHandling" -ForegroundColor White
    Write-Host "  • Dependency injection for all services" -ForegroundColor White
    Write-Host ""

    Write-Host "PRINCIPLE II: TESTING STANDARDS" -ForegroundColor Yellow
    Write-Host "  • Minimum 80% test coverage" -ForegroundColor White
    Write-Host "  • Critical paths require 95% coverage" -ForegroundColor White
    Write-Host "  • Follow test naming conventions" -ForegroundColor White
    Write-Host "  • Integration tests for database operations" -ForegroundColor White
    Write-Host ""

    Write-Host "PRINCIPLE III: UX CONSISTENCY" -ForegroundColor Yellow
    Write-Host "  • Use Theme V2 dynamic resources (no hardcoded colors)" -ForegroundColor White
    Write-Host "  • x:DataType attributes on all UserControls" -ForegroundColor White
    Write-Host "  • Material Design icons exclusively" -ForegroundColor White
    Write-Host "  • Cross-platform layout compatibility" -ForegroundColor White
    Write-Host ""

    Write-Host "PRINCIPLE IV: PERFORMANCE REQUIREMENTS" -ForegroundColor Yellow
    Write-Host "  • Async/await for all I/O operations" -ForegroundColor White
    Write-Host "  • Connection pooling configured (MySQL)" -ForegroundColor White
    Write-Host "  • No blocking UI thread operations" -ForegroundColor White
    Write-Host "  • Sub-100ms UI response time" -ForegroundColor White
    Write-Host ""

    Write-Host "ENFORCEMENT:" -ForegroundColor Yellow
    Write-Host "  Run: " -NoNewline
    Write-Host "gsc validate constitution" -ForegroundColor Cyan -NoNewline
    Write-Host " to check compliance"
    Write-Host ""

    Write-Host "For complete details:" -ForegroundColor Gray
    Write-Host "  gsc memory get constitution" -ForegroundColor Cyan
    Write-Host ""
}

# Main execution
switch ($Topic.ToLower()) {
    'create' { Show-CreateHelp }
    'validate' { Show-ValidateHelp }
    'memory' { Show-MemoryHelp }
    'rollback' { Show-RollbackHelp }
    'workflow' { Show-WorkflowHelp }
    'status' {
        Write-GscHeader -Title "GSC Status Command" -SubTitle "Show current progress and state"
        Write-Host "Displays real-time workflow progress, constitutional compliance," -ForegroundColor Gray
        Write-Host "test coverage, and next recommended actions." -ForegroundColor Gray
        Write-Host ""
        Write-Host "USAGE:" -ForegroundColor Yellow
        Write-Host "  gsc status" -ForegroundColor Cyan
        Write-Host ""
    }
    'constitution' { Show-ConstitutionHelp }
    '' { Show-GeneralHelp }
    default {
        Write-GscError "Unknown help topic: $Topic"
        Write-Host ""
        Show-GeneralHelp
    }
}
