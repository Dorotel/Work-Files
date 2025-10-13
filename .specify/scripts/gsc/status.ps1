<#
.SYNOPSIS
    GSC Status Command - Show current progress and state

.DESCRIPTION
    Displays comprehensive workflow status including:
    - Current feature and phase
    - File changes (modified, staged, untracked)
    - Task progress and milestones
    - Recent checkpoint history
    - Constitutional compliance summary
    - Phase-specific next actions

.PARAMETER Detailed
    Show detailed file listings and extended information

.PARAMETER Quick
    Show condensed summary only

.EXAMPLE
    gsc status
    Show standard status display

.EXAMPLE
    gsc status -Detailed
    Show detailed status with full file listings

.EXAMPLE
    gsc status -Quick
    Show condensed summary

.NOTES
    Version: 2.0.0
    Phase 5 Implementation: Complete
#>

param(
    [switch]$Detailed,
    [switch]$Quick
)

# Import common utilities
$commonPath = Join-Path $PSScriptRoot "common"
. (Join-Path $commonPath "output.ps1")
. (Join-Path $commonPath "state.ps1")

# Determine repository root (go up from .specify/scripts)
$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

#region Helper Functions

function Get-GitFileStatus {
    <#
    .SYNOPSIS
        Get categorized git file status
    #>
    try {
        Push-Location $repoRoot
        $gitStatus = git status --porcelain 2>$null

        if ($LASTEXITCODE -ne 0) {
            return @{
                Modified = @()
                Staged = @()
                Untracked = @()
                Total = 0
            }
        }

        $modified = @()
        $staged = @()
        $untracked = @()

        foreach ($line in $gitStatus) {
            if ($line.Length -lt 4) { continue }

            $status = $line.Substring(0, 2)
            $file = $line.Substring(3).Trim()

            # Staged files (first character indicates staged status)
            if ($status[0] -match '[MADRC]') {
                $staged += $file
            }

            # Modified files (second character indicates working tree status)
            if ($status[1] -match '[MD]') {
                $modified += $file
            }

            # Untracked files
            if ($status -eq '??') {
                $untracked += $file
            }
        }

        return @{
            Modified = $modified
            Staged = $staged
            Untracked = $untracked
            Total = $modified.Count + $staged.Count + $untracked.Count
        }
    }
    finally {
        Pop-Location
    }
}

function Get-RecentCheckpoints {
    <#
    .SYNOPSIS
        Get recent checkpoint information
    #>
    param([int]$Count = 3)

    # Go up from gsc/ to .specify/, then to state/checkpoints
    $checkpointsDir = Join-Path (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) "state\checkpoints"

    if (-not (Test-Path $checkpointsDir)) {
        return @()
    }

    $checkpoints = Get-ChildItem -Path $checkpointsDir -Directory |
        Where-Object { $_.Name -match '^checkpoint-\d{8}-\d{6}$' } |
        Sort-Object Name -Descending |
        Select-Object -First $Count

    $result = @()
    foreach ($cp in $checkpoints) {
        $metaFile = Join-Path $cp.FullName "metadata.json"
        if (Test-Path $metaFile) {
            $meta = Get-Content $metaFile -Raw | ConvertFrom-Json
            $result += @{
                Id = $cp.Name
                Description = $meta.description
                Created = $meta.timestamp
                FileCount = $meta.fileCount
                Feature = $meta.feature
            }
        }
    }

    return $result
}

function Get-FileTypeBreakdown {
    <#
    .SYNOPSIS
        Categorize files by type
    #>
    param([string[]]$Files)

    $breakdown = @{
        CSharp = 0
        AXAML = 0
        PowerShell = 0
        Docs = 0
        Config = 0
        Other = 0
    }

    foreach ($file in $Files) {
        $ext = [System.IO.Path]::GetExtension($file).ToLower()
        switch ($ext) {
            '.cs' { $breakdown.CSharp++ }
            '.axaml' { $breakdown.AXAML++ }
            '.ps1' { $breakdown.PowerShell++ }
            { $_ -in '.md', '.txt' } { $breakdown.Docs++ }
            { $_ -in '.json', '.xml', '.config' } { $breakdown.Config++ }
            default { $breakdown.Other++ }
        }
    }

    return $breakdown
}

function Get-QuickComplianceCheck {
    <#
    .SYNOPSIS
        Run quick constitutional compliance check
    #>
    try {
        # Import constitution functions
        . (Join-Path $commonPath "constitution.ps1")

        # Get file lists
        $csFiles = Get-ChildItem -Path $repoRoot -Filter "*.cs" -Recurse -File |
            Where-Object { $_.FullName -notmatch '\\obj\\|\\bin\\' } |
            Select-Object -ExpandProperty FullName

        $axamlFiles = Get-ChildItem -Path $repoRoot -Filter "*.axaml" -Recurse -File |
            Where-Object { $_.FullName -notmatch '\\obj\\|\\bin\\' } |
            Select-Object -ExpandProperty FullName

        # Quick scan (first 50 files of each type for speed)
        $csSample = $csFiles | Select-Object -First 50
        $axamlSample = $axamlFiles | Select-Object -First 50

        $codeViolations = 0
        $uxViolations = 0

        # Sample code quality check
        foreach ($file in $csSample) {
            $content = Get-Content $file -Raw -ErrorAction SilentlyContinue
            if ($content) {
                if ($content -match 'ReactiveCommand|ReactiveObject') { $codeViolations++ }
                if ($content -match '\.Result\s|\.Wait\(') { $codeViolations++ }
            }
        }

        # Sample UX consistency check
        foreach ($file in $axamlSample) {
            $content = Get-Content $file -Raw -ErrorAction SilentlyContinue
            if ($content) {
                if ($content -match 'Background="#[0-9A-Fa-f]{6}"') { $uxViolations++ }
            }
        }

        $totalScanned = $csSample.Count + $axamlSample.Count
        $totalViolations = $codeViolations + $uxViolations

        $score = if ($totalScanned -gt 0) {
            [math]::Round((1 - ($totalViolations / ($totalScanned * 2))) * 100)
        } else {
            100
        }

        return @{
            Score = $score
            Violations = $totalViolations
            FilesScanned = $totalScanned
            Status = if ($score -ge 80) { "Pass" } else { "Needs Attention" }
        }
    }
    catch {
        return @{
            Score = 0
            Violations = 0
            FilesScanned = 0
            Status = "Unable to check"
        }
    }
}

function Get-PhaseGuidance {
    <#
    .SYNOPSIS
        Get phase-specific guidance and next actions
    #>
    param([string]$Phase)

    $guidance = @{
        'specification' = @{
            Title = "📝 Specification Phase"
            Description = "Define feature requirements and acceptance criteria"
            Actions = @(
                "Review and refine spec.md",
                "Run: gsc memory get constitution",
                "Validate requirements against constitutional principles",
                "Create checkpoint before advancing"
            )
            NextPhase = "planning"
        }
        'planning' = @{
            Title = "🗺️  Planning Phase"
            Description = "Create technical implementation plan"
            Actions = @(
                "Review plan.md and architecture decisions",
                "Identify dependencies and risks",
                "Create checkpoint before implementation",
                "Run: gsc validate constitution"
            )
            NextPhase = "implementation"
        }
        'implementation' = @{
            Title = "⚙️  Implementation Phase"
            Description = "Execute development following plan"
            Actions = @(
                "Follow task breakdown from tasks.md",
                "Create checkpoints after major milestones",
                "Run: gsc validate code",
                "Test incrementally as you build"
            )
            NextPhase = "testing"
        }
        'testing' = @{
            Title = "🧪 Testing Phase"
            Description = "Validate functionality and quality"
            Actions = @(
                "Execute test scenarios",
                "Run: gsc validate all",
                "Verify cross-platform compatibility",
                "Document test results"
            )
            NextPhase = "review"
        }
        'review' = @{
            Title = "👁️  Review Phase"
            Description = "Final validation before completion"
            Actions = @(
                "Code review for constitutional compliance",
                "Performance validation",
                "Documentation completeness check",
                "Create final checkpoint"
            )
            NextPhase = "complete"
        }
        'complete' = @{
            Title = "✅ Feature Complete"
            Description = "Ready for merge"
            Actions = @(
                "Final validation: gsc validate all",
                "Merge to main branch",
                "Update documentation",
                "Archive workflow state"
            )
            NextPhase = $null
        }
    }

    if ($guidance.ContainsKey($Phase.ToLower())) {
        return $guidance[$Phase.ToLower()]
    }

    return @{
        Title = "Unknown Phase"
        Description = "Phase not recognized"
        Actions = @("Run: gsc workflow start")
        NextPhase = $null
    }
}

#endregion

#region Main Status Display

Write-GscHeader -Title "Workflow Status" -SubTitle "Current Development Progress"

$state = Get-CurrentState

# Handle no active workflow
if (-not $state -or -not $state.currentFeature) {
    Write-GscInfo "No active feature workflow"
    Write-Host ""

    # Show git status even without active workflow
    Write-GscSectionHeader "Repository Status"
    $gitStatus = Get-GitFileStatus

    if ($gitStatus.Total -gt 0) {
        Write-Host "  Modified:  " -ForegroundColor Gray -NoNewline
        Write-Host $gitStatus.Modified.Count -ForegroundColor Yellow
        Write-Host "  Staged:    " -ForegroundColor Gray -NoNewline
        Write-Host $gitStatus.Staged.Count -ForegroundColor Green
        Write-Host "  Untracked: " -ForegroundColor Gray -NoNewline
        Write-Host $gitStatus.Untracked.Count -ForegroundColor Cyan
        Write-Host ""
    } else {
        Write-Host "  No uncommitted changes" -ForegroundColor Green
        Write-Host ""
    }

    # Show recent checkpoints
    Write-GscSectionHeader "Recent Checkpoints"
    $recentCheckpoints = Get-RecentCheckpoints -Count 3

    if ($recentCheckpoints.Count -gt 0) {
        foreach ($cp in $recentCheckpoints) {
            Write-Host "  🔖 " -NoNewline -ForegroundColor Cyan
            Write-Host $cp.Id -NoNewline -ForegroundColor White
            Write-Host " ($($cp.FileCount) files)" -ForegroundColor Gray
        }
        Write-Host ""
        Write-Host "  Restore: " -NoNewline -ForegroundColor Gray
        Write-Host "gsc rollback restore <checkpoint-id>" -ForegroundColor Cyan
    } else {
        Write-Host "  No checkpoints found" -ForegroundColor Gray
    }
    Write-Host ""

    Write-Host "To start a new feature:" -ForegroundColor Gray
    Write-Host "  gsc workflow start <feature-name>" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Or create a feature:" -ForegroundColor Gray
    Write-Host "  gsc create feature <feature-name>" -ForegroundColor Cyan
    Write-Host ""
    exit 0
}

# Feature Overview Section
Write-GscSectionHeader "Feature Overview"

Write-Host "  Feature:     " -ForegroundColor Gray -NoNewline
Write-Host $state.currentFeature -ForegroundColor Cyan

Write-Host "  Phase:       " -ForegroundColor Gray -NoNewline
$phaseColor = switch ($state.currentPhase.ToLower()) {
    'specification' { 'Blue' }
    'planning' { 'Magenta' }
    'implementation' { 'Yellow' }
    'testing' { 'Cyan' }
    'review' { 'Green' }
    'complete' { 'Green' }
    default { 'White' }
}
Write-Host $state.currentPhase -ForegroundColor $phaseColor

if ($state.startTimestamp) {
    Write-Host "  Started:     " -ForegroundColor Gray -NoNewline
    Write-Host $state.startTimestamp
}

if ($state.lastUpdateTimestamp) {
    Write-Host "  Last Update: " -ForegroundColor Gray -NoNewline
    Write-Host $state.lastUpdateTimestamp
}

Write-Host ""

# File Status Section
if (-not $Quick) {
    Write-GscSectionHeader "File Changes"

    $gitStatus = Get-GitFileStatus

    if ($gitStatus.Total -eq 0) {
        Write-Host "  No uncommitted changes" -ForegroundColor Green
        Write-Host ""
    } else {
        # Summary counts
        Write-Host "  Modified:  " -ForegroundColor Gray -NoNewline
        Write-Host $gitStatus.Modified.Count -ForegroundColor Yellow

        Write-Host "  Staged:    " -ForegroundColor Gray -NoNewline
        Write-Host $gitStatus.Staged.Count -ForegroundColor Green

        Write-Host "  Untracked: " -ForegroundColor Gray -NoNewline
        Write-Host $gitStatus.Untracked.Count -ForegroundColor Cyan

        Write-Host "  Total:     " -ForegroundColor Gray -NoNewline
        Write-Host $gitStatus.Total -ForegroundColor White
        Write-Host ""

        # File type breakdown
        $allFiles = $gitStatus.Modified + $gitStatus.Staged + $gitStatus.Untracked
        $breakdown = Get-FileTypeBreakdown -Files $allFiles

        $hasFiles = $false
        if ($breakdown.CSharp -gt 0) {
            Write-Host "    C#:         " -ForegroundColor Gray -NoNewline
            Write-Host $breakdown.CSharp -ForegroundColor White
            $hasFiles = $true
        }
        if ($breakdown.AXAML -gt 0) {
            Write-Host "    AXAML:      " -ForegroundColor Gray -NoNewline
            Write-Host $breakdown.AXAML -ForegroundColor White
            $hasFiles = $true
        }
        if ($breakdown.PowerShell -gt 0) {
            Write-Host "    PowerShell: " -ForegroundColor Gray -NoNewline
            Write-Host $breakdown.PowerShell -ForegroundColor White
            $hasFiles = $true
        }
        if ($breakdown.Docs -gt 0) {
            Write-Host "    Docs:       " -ForegroundColor Gray -NoNewline
            Write-Host $breakdown.Docs -ForegroundColor White
            $hasFiles = $true
        }
        if ($breakdown.Config -gt 0) {
            Write-Host "    Config:     " -ForegroundColor Gray -NoNewline
            Write-Host $breakdown.Config -ForegroundColor White
            $hasFiles = $true
        }
        if ($breakdown.Other -gt 0) {
            Write-Host "    Other:      " -ForegroundColor Gray -NoNewline
            Write-Host $breakdown.Other -ForegroundColor White
            $hasFiles = $true
        }

        if ($hasFiles) {
            Write-Host ""
        }

        # Detailed file listing if requested
        if ($Detailed) {
            if ($gitStatus.Modified.Count -gt 0) {
                Write-Host "  Modified files:" -ForegroundColor Yellow
                foreach ($file in ($gitStatus.Modified | Select-Object -First 10)) {
                    Write-Host "    M  $file" -ForegroundColor Gray
                }
                if ($gitStatus.Modified.Count -gt 10) {
                    Write-Host "    ... and $($gitStatus.Modified.Count - 10) more" -ForegroundColor DarkGray
                }
                Write-Host ""
            }

            if ($gitStatus.Staged.Count -gt 0) {
                Write-Host "  Staged files:" -ForegroundColor Green
                foreach ($file in ($gitStatus.Staged | Select-Object -First 10)) {
                    Write-Host "    A  $file" -ForegroundColor Gray
                }
                if ($gitStatus.Staged.Count -gt 10) {
                    Write-Host "    ... and $($gitStatus.Staged.Count - 10) more" -ForegroundColor DarkGray
                }
                Write-Host ""
            }

            if ($gitStatus.Untracked.Count -gt 0) {
                Write-Host "  Untracked files:" -ForegroundColor Cyan
                foreach ($file in ($gitStatus.Untracked | Select-Object -First 10)) {
                    Write-Host "    ?  $file" -ForegroundColor Gray
                }
                if ($gitStatus.Untracked.Count -gt 10) {
                    Write-Host "    ... and $($gitStatus.Untracked.Count - 10) more" -ForegroundColor DarkGray
                }
                Write-Host ""
            }
        }
    }
}

# Progress Section
if (-not $Quick -and $state.totalTasks -gt 0) {
    Write-GscSectionHeader "Task Progress"

    Write-Host "  Current Task: " -ForegroundColor Gray -NoNewline
    Write-Host "$($state.currentTask) of $($state.totalTasks)" -ForegroundColor White

    $percentage = [math]::Round(($state.currentTask / $state.totalTasks) * 100)
    Write-Host "  Completion:   " -ForegroundColor Gray -NoNewline
    Write-Host "$percentage%" -ForegroundColor $(if ($percentage -ge 80) { 'Green' } elseif ($percentage -ge 50) { 'Yellow' } else { 'Red' })

    # Progress bar
    $barWidth = 40
    $filled = [math]::Floor(($percentage / 100) * $barWidth)
    $empty = $barWidth - $filled

    Write-Host "  [" -NoNewline -ForegroundColor Gray
    Write-Host ("█" * $filled) -NoNewline -ForegroundColor Green
    Write-Host ("░" * $empty) -NoNewline -ForegroundColor DarkGray
    Write-Host "]" -ForegroundColor Gray
    Write-Host ""
}

# Checkpoint History Section
if (-not $Quick) {
    Write-GscSectionHeader "Recent Checkpoints"

    $recentCheckpoints = Get-RecentCheckpoints -Count 3

    if ($recentCheckpoints.Count -eq 0) {
        Write-Host "  No checkpoints yet" -ForegroundColor Gray
        Write-Host "  Create one: " -NoNewline -ForegroundColor Gray
        Write-Host "gsc rollback checkpoint '<description>'" -ForegroundColor Cyan
    } else {
        foreach ($cp in $recentCheckpoints) {
            Write-Host "  🔖 " -NoNewline -ForegroundColor Cyan
            Write-Host $cp.Id -NoNewline -ForegroundColor White
            Write-Host " ($($cp.FileCount) files)" -ForegroundColor Gray

            if ($cp.Description) {
                Write-Host "     $($cp.Description)" -ForegroundColor DarkGray
            }
        }
        Write-Host ""
        Write-Host "  View all: " -NoNewline -ForegroundColor Gray
        Write-Host "gsc rollback list" -ForegroundColor Cyan
        Write-Host "  Restore:  " -NoNewline -ForegroundColor Gray
        Write-Host "gsc rollback restore <checkpoint-id>" -ForegroundColor Cyan
    }
    Write-Host ""
}

# Constitutional Compliance Section
if (-not $Quick) {
    Write-GscSectionHeader "Constitutional Compliance (Quick Check)"

    Write-Host "  Checking..." -ForegroundColor Gray
    $compliance = Get-QuickComplianceCheck

    Write-Host "`r  Score:         " -ForegroundColor Gray -NoNewline
    $scoreColor = if ($compliance.Score -ge 80) { 'Green' } elseif ($compliance.Score -ge 60) { 'Yellow' } else { 'Red' }
    Write-Host "$($compliance.Score)% - $($compliance.Status)" -ForegroundColor $scoreColor

    Write-Host "  Files Sampled: " -ForegroundColor Gray -NoNewline
    Write-Host $compliance.FilesScanned -ForegroundColor White

    if ($compliance.Violations -gt 0) {
        Write-Host "  Violations:    " -ForegroundColor Gray -NoNewline
        Write-Host $compliance.Violations -ForegroundColor Yellow
    }

    Write-Host ""
    Write-Host "  Full check: " -NoNewline -ForegroundColor Gray
    Write-Host "gsc validate all" -ForegroundColor Cyan
    Write-Host ""
}

# Phase Guidance Section
Write-GscSectionHeader "Next Actions"

$guidance = Get-PhaseGuidance -Phase $state.currentPhase

Write-Host "  $($guidance.Title)" -ForegroundColor White
Write-Host "  $($guidance.Description)" -ForegroundColor Gray
Write-Host ""

foreach ($action in $guidance.Actions) {
    Write-Host "  • $action" -ForegroundColor Gray
}

Write-Host ""

if ($guidance.NextPhase) {
    Write-Host "  Next phase: " -NoNewline -ForegroundColor Gray
    Write-Host $guidance.NextPhase -ForegroundColor Cyan
    Write-Host "  Advance:    " -NoNewline -ForegroundColor Gray
    Write-Host "gsc workflow next" -ForegroundColor Cyan
}

Write-Host ""

# Quick mode summary
if ($Quick) {
    $gitStatus = Get-GitFileStatus
    Write-Host "Summary: " -NoNewline -ForegroundColor Gray
    Write-Host "$($gitStatus.Total) files changed" -NoNewline -ForegroundColor White
    if ($state.totalTasks -gt 0) {
        Write-Host " | " -NoNewline -ForegroundColor DarkGray
        Write-Host "$($state.currentTask)/$($state.totalTasks) tasks" -NoNewline -ForegroundColor White
    }
    Write-Host " | " -NoNewline -ForegroundColor DarkGray
    Write-Host $state.currentPhase -ForegroundColor $phaseColor
    Write-Host ""
}

#endregion

exit 0
