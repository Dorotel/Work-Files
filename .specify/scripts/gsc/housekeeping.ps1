<#
.SYNOPSIS
    GSC Housekeeping Command - Documentation inventory and cleanup

.DESCRIPTION
    Manages documentation lifecycle: inventory, prune obsolete docs, archive, and purge checkpoints

.PARAMETER Action
    Operation: 'inventory', 'prune', 'archive', 'purge-checkpoints'

.PARAMETER Scope
    Scope of operation (default: 'all')

.PARAMETER All
    Include all items (for purge-checkpoints)

.PARAMETER DryRun
    Perform dry-run without making changes (default: $true)

.EXAMPLE
    gsc housekeeping inventory
    gsc housekeeping prune --DryRun:$false
    gsc housekeeping purge-checkpoints --All

.NOTES
    Version: 1.0.0
    Safe by default - all destructive actions require --DryRun:$false
#>

param(
    [Parameter(Mandatory=$true, Position=0)]
    [ValidateSet('inventory', 'prune', 'archive', 'purge-checkpoints')]
    [string]$Action,

    [Parameter(Position=1)]
    [string]$Scope = 'all',

    [switch]$All,
    [switch]$DryRun = $true
)

# Import common utilities
$commonPath = Join-Path $PSScriptRoot "common"
. (Join-Path $commonPath "output.ps1")
. (Join-Path $commonPath "state.ps1")

function Get-DocsInventory {
    <#
    .SYNOPSIS
        Scan all documentation and create comprehensive inventory
    #>

    Write-GscHeader -Title "Documentation Inventory" -SubTitle "Scanning all documentation files"

    $root = Resolve-Path "."
    $report = [ordered]@{
        Timestamp = (Get-Date -Format 'o')
        Required = @()
        Candidates = @()
        Checkpoints = @()
        TotalSize = 0
    }

    # Determine required files from active plan and tasks
    Write-Host "📋 Analyzing active workflows..." -ForegroundColor Cyan

    $planFiles = Get-ChildItem ".specify\specs\*\plan.md" -ErrorAction SilentlyContinue
    $tasksFiles = Get-ChildItem ".specify\specs\*\tasks.md" -ErrorAction SilentlyContinue

    $planContent = ($planFiles | ForEach-Object { Get-Content $_.FullName -Raw }) -join " "
    $tasksContent = ($tasksFiles | ForEach-Object { Get-Content $_.FullName -Raw }) -join " "

    # Documentation paths to inventory
    $docPaths = @(
        ".specify\docs\*.md",
        ".specify\docs\*.html",
        "AGENTS.md",
        ".github\prompts\*.md",
        ".specify\templates\*.md",
        ".specify\memory\*.md"
    )

    Write-Host "📁 Scanning documentation paths..." -ForegroundColor Cyan

    foreach ($glob in $docPaths) {
        $files = Get-ChildItem $glob -Recurse -ErrorAction SilentlyContinue

        foreach ($file in $files) {
            $relativePath = $file.FullName.Replace("$root\", "")
            $fileSize = $file.Length

            $report.TotalSize += $fileSize

            # Check if file is referenced in active plans/tasks
            $isRequired = $false
            if ($planContent -like "*$($file.Name)*" -or $tasksContent -like "*$($file.Name)*") {
                $isRequired = $true
            }

            # Core files always required
            $coreFiles = @(
                'AGENTS.md',
                'constitution.md',
                'lessons-learned.md',
                'gsc-enhancement-system.md',
                'gsc-interactive-help.html',
                'gsc-quickstart.md',
                'gsc-implementation-plan.md'
            )

            if ($coreFiles -contains $file.Name) {
                $isRequired = $true
            }

            $itemInfo = @{
                Path = $relativePath
                Name = $file.Name
                Size = $fileSize
                LastModified = $file.LastWriteTime
            }

            if ($isRequired) {
                $report.Required += $itemInfo
            } else {
                $report.Candidates += $itemInfo
            }
        }
    }

    # Inventory checkpoints
    Write-Host "💾 Scanning checkpoints..." -ForegroundColor Cyan

    if (Test-Path ".specify\state\checkpoints") {
        $checkpoints = Get-ChildItem ".specify\state\checkpoints" -Directory

        foreach ($checkpoint in $checkpoints) {
            $checkpointSize = (Get-ChildItem $checkpoint.FullName -Recurse | Measure-Object -Property Length -Sum).Sum

            $report.Checkpoints += @{
                Path = $checkpoint.FullName.Replace("$root\", "")
                Name = $checkpoint.Name
                Size = $checkpointSize
                LastModified = $checkpoint.LastWriteTime
            }

            $report.TotalSize += $checkpointSize
        }
    }

    # Save inventory report
    $reportDir = ".specify\state\reports"
    if (-not (Test-Path $reportDir)) {
        New-Item -ItemType Directory -Path $reportDir -Force | Out-Null
    }

    $reportPath = Join-Path $reportDir "docs-inventory.json"
    $report | ConvertTo-Json -Depth 10 | Set-Content $reportPath

    # Display summary
    Write-Host ""
    Write-Host "╔═══════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║       DOCUMENTATION INVENTORY REPORT                         ║" -ForegroundColor Cyan
    Write-Host "╚═══════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "📊 Summary:" -ForegroundColor Yellow
    Write-Host "   Required Files:     $($report.Required.Count)" -ForegroundColor Green
    Write-Host "   Candidate Files:    $($report.Candidates.Count)" -ForegroundColor Yellow
    Write-Host "   Checkpoints:        $($report.Checkpoints.Count)" -ForegroundColor Cyan
    Write-Host "   Total Size:         $([math]::Round($report.TotalSize / 1MB, 2)) MB" -ForegroundColor White
    Write-Host ""

    if ($report.Candidates.Count -gt 0) {
        Write-Host "📋 Candidate Files (Not Referenced in Active Plans):" -ForegroundColor Yellow
        foreach ($candidate in $report.Candidates) {
            $sizeMB = [math]::Round($candidate.Size / 1KB, 2)
            Write-Host "   • $($candidate.Name)" -ForegroundColor Gray -NoNewline
            Write-Host " ($sizeMB KB)" -ForegroundColor DarkGray
        }
        Write-Host ""
    }

    if ($report.Checkpoints.Count -gt 0) {
        Write-Host "💾 Checkpoints:" -ForegroundColor Cyan
        foreach ($checkpoint in $report.Checkpoints) {
            $sizeMB = [math]::Round($checkpoint.Size / 1MB, 2)
            $age = ((Get-Date) - $checkpoint.LastModified).Days
            Write-Host "   • $($checkpoint.Name)" -ForegroundColor Gray -NoNewline
            Write-Host " ($sizeMB MB, $age days old)" -ForegroundColor DarkGray
        }
        Write-Host ""
    }

    Write-Host "✅ Inventory saved: $reportPath" -ForegroundColor Green
    Write-Host ""
}

function Remove-ObsoleteDocs {
    <#
    .SYNOPSIS
        Archive candidate files that are not required
    #>

    param(
        [Parameter(Mandatory=$true)]
        [array]$Candidates,

        [switch]$DryRun
    )

    if ($Candidates.Count -eq 0) {
        Write-Host "✅ No obsolete files to prune" -ForegroundColor Green
        return
    }

    $archiveRoot = ".specify\archive\$(Get-Date -Format 'yyyyMMdd-HHmmss')"

    if ($DryRun) {
        Write-Host ""
        Write-Host "╔═══════════════════════════════════════════════════════════════╗" -ForegroundColor Yellow
        Write-Host "║       DRY-RUN MODE - No Changes Will Be Made                 ║" -ForegroundColor Yellow
        Write-Host "╚═══════════════════════════════════════════════════════════════╝" -ForegroundColor Yellow
        Write-Host ""

        Write-Host "📋 Would archive the following files:" -ForegroundColor Yellow
        foreach ($candidate in $Candidates) {
            Write-Host "   • $($candidate.Path)" -ForegroundColor Gray
        }

        Write-Host ""
        Write-Host "Archive destination: $archiveRoot" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "To execute: " -NoNewline
        Write-Host "gsc housekeeping prune --DryRun:`$false" -ForegroundColor Green
        Write-Host ""
        return
    }

    # Create archive directory
    New-Item -ItemType Directory -Path $archiveRoot -Force | Out-Null

    Write-Host ""
    Write-Host "📦 Archiving obsolete files..." -ForegroundColor Cyan
    Write-Host ""

    $archivedCount = 0
    foreach ($candidate in $Candidates) {
        $sourcePath = $candidate.Path

        if (Test-Path $sourcePath) {
            $relativeDest = Split-Path $sourcePath -Leaf
            $destPath = Join-Path $archiveRoot $relativeDest

            try {
                Move-Item $sourcePath $destPath -Force
                Write-Host "   ✅ Archived: $sourcePath" -ForegroundColor Green
                $archivedCount++
            } catch {
                Write-Host "   ⚠️  Failed: $sourcePath - $($_.Exception.Message)" -ForegroundColor Yellow
            }
        }
    }

    Write-Host ""
    Write-Host "✅ Archived $archivedCount files to: $archiveRoot" -ForegroundColor Green
    Write-Host ""
}

function Purge-Checkpoints {
    <#
    .SYNOPSIS
        Remove old checkpoints based on retention policy
    #>

    param(
        [switch]$All,
        [int]$KeepDays = 14,
        [switch]$DryRun
    )

    $checkpointDir = ".specify\state\checkpoints"

    if (-not (Test-Path $checkpointDir)) {
        Write-Host "✅ No checkpoints directory found" -ForegroundColor Green
        return
    }

    $cutoffDate = (Get-Date).AddDays(-$KeepDays)

    $targets = if ($All) {
        Get-ChildItem $checkpointDir -Directory
    } else {
        Get-ChildItem $checkpointDir -Directory | Where-Object { $_.LastWriteTime -lt $cutoffDate }
    }

    if ($targets.Count -eq 0) {
        Write-Host "✅ No checkpoints to purge" -ForegroundColor Green
        return
    }

    if ($DryRun) {
        Write-Host ""
        Write-Host "╔═══════════════════════════════════════════════════════════════╗" -ForegroundColor Yellow
        Write-Host "║       DRY-RUN MODE - No Changes Will Be Made                 ║" -ForegroundColor Yellow
        Write-Host "╚═══════════════════════════════════════════════════════════════╝" -ForegroundColor Yellow
        Write-Host ""

        Write-Host "💾 Would delete the following checkpoints:" -ForegroundColor Yellow
        foreach ($target in $targets) {
            $age = ((Get-Date) - $target.LastWriteTime).Days
            $size = (Get-ChildItem $target.FullName -Recurse | Measure-Object -Property Length -Sum).Sum
            $sizeMB = [math]::Round($size / 1MB, 2)
            Write-Host "   • $($target.Name) ($age days old, $sizeMB MB)" -ForegroundColor Gray
        }

        Write-Host ""
        Write-Host "Total to purge: $($targets.Count) checkpoints" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "To execute: " -NoNewline
        if ($All) {
            Write-Host "gsc housekeeping purge-checkpoints --All --DryRun:`$false" -ForegroundColor Green
        } else {
            Write-Host "gsc housekeeping purge-checkpoints --DryRun:`$false" -ForegroundColor Green
        }
        Write-Host ""
        return
    }

    Write-Host ""
    Write-Host "🗑️  Purging checkpoints..." -ForegroundColor Cyan
    Write-Host ""

    $purgedCount = 0
    $totalFreed = 0

    foreach ($target in $targets) {
        try {
            $size = (Get-ChildItem $target.FullName -Recurse | Measure-Object -Property Length -Sum).Sum
            Remove-Item $target.FullName -Recurse -Force
            Write-Host "   ✅ Purged: $($target.Name)" -ForegroundColor Green
            $purgedCount++
            $totalFreed += $size
        } catch {
            Write-Host "   ⚠️  Failed: $($target.Name) - $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }

    $freedMB = [math]::Round($totalFreed / 1MB, 2)

    Write-Host ""
    Write-Host "✅ Purged $purgedCount checkpoints, freed $freedMB MB" -ForegroundColor Green
    Write-Host ""
}

# Main execution
switch ($Action) {
    'inventory' {
        Get-DocsInventory
    }

    'prune' {
        # Load inventory
        $manifestPath = ".specify\state\reports\docs-inventory.json"

        if (-not (Test-Path $manifestPath)) {
            Write-Host "⚠️  No inventory found. Running inventory first..." -ForegroundColor Yellow
            Write-Host ""
            Get-DocsInventory
        }

        $inventory = Get-Content $manifestPath -Raw | ConvertFrom-Json
        Remove-ObsoleteDocs -Candidates $inventory.Candidates -DryRun:$DryRun
    }

    'archive' {
        # Load inventory
        $manifestPath = ".specify\state\reports\docs-inventory.json"

        if (-not (Test-Path $manifestPath)) {
            Write-Host "⚠️  No inventory found. Running inventory first..." -ForegroundColor Yellow
            Write-Host ""
            Get-DocsInventory
        }

        $inventory = Get-Content $manifestPath -Raw | ConvertFrom-Json

        # Archive both required and candidates (full backup)
        $allFiles = $inventory.Required + $inventory.Candidates
        Remove-ObsoleteDocs -Candidates $allFiles -DryRun:$DryRun
    }

    'purge-checkpoints' {
        Purge-Checkpoints -All:$All -DryRun:$DryRun
    }
}
