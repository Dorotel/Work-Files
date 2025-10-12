<#
.SYNOPSIS
    GSC Rollback Command - Manage checkpoints and rollback changes

.DESCRIPTION
    Creates checkpoints for safe experimentation and provides rollback
    capabilities with atomic file restoration.

.PARAMETER Action
    Action to perform: checkpoint, list, restore, full-reset

.PARAMETER Description
    Description for checkpoint creation or checkpoint ID for restore

.EXAMPLE
    gsc rollback checkpoint "before refactoring"
    gsc rollback list
    gsc rollback restore checkpoint-20251010-143022
    gsc rollback full-reset

.NOTES
    Version: 1.0.0
    Phase 3: Complete implementation
#>

param(
    [Parameter(Mandatory=$false, Position=0)]
    [ValidateSet('checkpoint', 'list', 'restore', 'full-reset', '')]
    [string]$Action = '',

    [Parameter(Mandatory=$false, Position=1)]
    [string]$Description = ''
)

# Import common utilities
$commonPath = Join-Path $PSScriptRoot "common"
. (Join-Path $commonPath "output.ps1")
. (Join-Path $commonPath "state.ps1")

function New-Checkpoint {
    <#
    .SYNOPSIS
        Create a new checkpoint with file snapshots
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$Description
    )

    Write-GscHeader -Title "Creating Checkpoint" -SubTitle $Description

    # Initialize state directory
    $stateDir = Initialize-StateDirectory
    $checkpointsDir = Join-Path $stateDir "checkpoints"

    # Generate checkpoint ID with timestamp
    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $checkpointId = "checkpoint-$timestamp"
    $checkpointDir = Join-Path $checkpointsDir $checkpointId

    # Create checkpoint directory structure
    New-Item -Path $checkpointDir -ItemType Directory -Force | Out-Null
    $filesDir = Join-Path $checkpointDir "files"
    New-Item -Path $filesDir -ItemType Directory -Force | Out-Null

    # Get current state
    $currentState = Get-CurrentState

    # Get repository root (from .specify/scripts/gsc → .specify/scripts → .specify → repo root)
    $repoRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))

    # Get modified files from git status
    $modifiedFiles = @()
    $totalSize = 0

    try {
        Push-Location $repoRoot

        # Get all modified, added, and untracked files
        $gitStatus = git status --short 2>$null

        if ($LASTEXITCODE -eq 0 -and $gitStatus) {
            $modifiedFiles = @(foreach ($statusLine in $gitStatus) {
                # Parse git status output (format: XY filename)
                $line = $statusLine.Trim()
                if ($line) {
                    # Extract filename (skip first 3 characters which are status codes)
                    $filename = $line.Substring(3).Trim()

                    # Resolve full path
                    $fullPath = Join-Path $repoRoot $filename

                    if (Test-Path $fullPath -PathType Leaf) {
                        $fileInfo = Get-Item $fullPath
                        $totalSize += $fileInfo.Length

                        # Calculate relative path for checkpoint storage
                        $relativePath = $filename

                        # Copy file to checkpoint with directory structure
                        $targetPath = Join-Path $filesDir $relativePath
                        $targetDir = Split-Path $targetPath -Parent

                        if (-not (Test-Path $targetDir)) {
                            New-Item -Path $targetDir -ItemType Directory -Force | Out-Null
                        }

                        Copy-Item -Path $fullPath -Destination $targetPath -Force

                        Write-Host "  📄 " -ForegroundColor Cyan -NoNewline
                        Write-Host $relativePath -ForegroundColor Gray

                        # Output to array
                        $relativePath
                    }
                }
            })
        }

        Pop-Location
    }
    catch {
        Pop-Location
        Write-GscError "Failed to detect modified files: $_"
        return
    }

    # If no git changes, check state for modified files list
    if ($modifiedFiles.Count -eq 0 -and $currentState -and $currentState.modifiedFiles) {
        $modifiedFiles = $currentState.modifiedFiles

        foreach ($relPath in $modifiedFiles) {
            $fullPath = Join-Path $repoRoot $relPath

            if (Test-Path $fullPath -PathType Leaf) {
                $fileInfo = Get-Item $fullPath
                $totalSize += $fileInfo.Length

                $targetPath = Join-Path $filesDir $relPath
                $targetDir = Split-Path $targetPath -Parent

                if (-not (Test-Path $targetDir)) {
                    New-Item -Path $targetDir -ItemType Directory -Force | Out-Null
                }

                Copy-Item -Path $fullPath -Destination $targetPath -Force

                Write-Host "  📄 " -ForegroundColor Cyan -NoNewline
                Write-Host $relPath -ForegroundColor Gray
            }
        }
    }

    # Create metadata
    $metadata = @{
        id = $checkpointId
        timestamp = Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ"
        description = $Description
        feature = if ($currentState) { $currentState.currentFeature } else { "none" }
        phase = if ($currentState) { $currentState.currentPhase } else { "none" }
        modifiedFiles = $modifiedFiles
        fileCount = $modifiedFiles.Count
        sizeBytes = $totalSize
        stateSnapshot = $currentState
    }

    # Save metadata
    $metadataPath = Join-Path $checkpointDir "metadata.json"
    $metadata | ConvertTo-Json -Depth 10 | Out-File -FilePath $metadataPath -Encoding UTF8

    # Update checkpoint count in state
    if ($currentState) {
        $currentState.checkpointsCount = ($currentState.checkpointsCount ?? 0) + 1
        Set-CurrentState -State $currentState
    }

    # Log checkpoint creation
    Add-StateHistory -Action "CheckpointCreated" -Details "$checkpointId - $Description"

    Write-Host ""
    Write-GscSuccess "Checkpoint created: $checkpointId"
    Write-Host ""
    Write-Host "Files captured: " -ForegroundColor Gray -NoNewline
    Write-Host $modifiedFiles.Count -ForegroundColor White
    Write-Host "Total size: " -ForegroundColor Gray -NoNewline
    Write-Host "$([math]::Round($totalSize / 1KB, 2)) KB" -ForegroundColor White
    Write-Host "Location: " -ForegroundColor Gray -NoNewline
    Write-Host $checkpointDir -ForegroundColor Cyan
    Write-Host ""

    exit 0
}

function Get-Checkpoints {
    <#
    .SYNOPSIS
        List all available checkpoints
    #>

    Write-GscHeader -Title "Available Checkpoints" -SubTitle "Checkpoint History"

    $stateDir = Initialize-StateDirectory
    $checkpointsDir = Join-Path $stateDir "checkpoints"

    if (-not (Test-Path $checkpointsDir)) {
        Write-GscInfo "No checkpoints found"
        Write-Host ""
        Write-Host "Create your first checkpoint:" -ForegroundColor Gray
        Write-Host "  gsc rollback checkpoint " -NoNewline -ForegroundColor Cyan
        Write-Host '"description"' -ForegroundColor White
        Write-Host ""
        return
    }

    $checkpointDirs = Get-ChildItem -Path $checkpointsDir -Directory | Sort-Object Name -Descending

    if ($checkpointDirs.Count -eq 0) {
        Write-GscInfo "No checkpoints found"
        Write-Host ""
        return
    }

    Write-Host "Found $($checkpointDirs.Count) checkpoint(s):" -ForegroundColor Gray
    Write-Host ""

    foreach ($cpDir in $checkpointDirs) {
        $metadataPath = Join-Path $cpDir.FullName "metadata.json"

        if (Test-Path $metadataPath) {
            $metadata = Get-Content -Path $metadataPath -Raw | ConvertFrom-Json

            Write-Host "  🔖 " -ForegroundColor Cyan -NoNewline
            Write-Host $metadata.id -ForegroundColor White
            Write-Host "     Description: " -ForegroundColor Gray -NoNewline
            Write-Host $metadata.description -ForegroundColor White
            Write-Host "     Created: " -ForegroundColor Gray -NoNewline
            Write-Host $metadata.timestamp
            Write-Host "     Feature: " -ForegroundColor Gray -NoNewline
            Write-Host $metadata.feature -ForegroundColor Yellow
            Write-Host "     Files: " -ForegroundColor Gray -NoNewline
            Write-Host "$($metadata.fileCount) files" -ForegroundColor White -NoNewline
            Write-Host " ($([math]::Round($metadata.sizeBytes / 1KB, 2)) KB)" -ForegroundColor Gray
            Write-Host ""
        }
    }

    Write-Host "To restore a checkpoint:" -ForegroundColor Gray
    Write-Host "  gsc rollback restore <checkpoint-id>" -ForegroundColor Cyan
    Write-Host ""

    exit 0
}

function Restore-Checkpoint {
    <#
    .SYNOPSIS
        Restore files from a checkpoint
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$CheckpointId
    )

    Write-GscHeader -Title "Restoring Checkpoint" -SubTitle $CheckpointId

    $stateDir = Initialize-StateDirectory
    $checkpointsDir = Join-Path $stateDir "checkpoints"
    $checkpointDir = Join-Path $checkpointsDir $CheckpointId

    if (-not (Test-Path $checkpointDir)) {
        Write-GscError "Checkpoint not found: $CheckpointId"
        Write-Host ""
        Write-Host "Available checkpoints:" -ForegroundColor Yellow
        Get-Checkpoints
        return
    }

    # Load metadata
    $metadataPath = Join-Path $checkpointDir "metadata.json"
    if (-not (Test-Path $metadataPath)) {
        Write-GscError "Checkpoint metadata not found"
        return
    }

    $metadata = Get-Content -Path $metadataPath -Raw | ConvertFrom-Json

    Write-Host "Description: " -ForegroundColor Gray -NoNewline
    Write-Host $metadata.description -ForegroundColor White
    Write-Host "Files to restore: " -ForegroundColor Gray -NoNewline
    Write-Host $metadata.fileCount -ForegroundColor White
    Write-Host ""

    # Confirm restoration
    Write-Host "⚠️  This will overwrite current files with checkpoint versions." -ForegroundColor Yellow
    Write-Host "Do you want to continue? (y/n): " -ForegroundColor Yellow -NoNewline
    $response = Read-Host

    if ($response -ne 'y' -and $response -ne 'yes') {
        Write-GscInfo "Restoration cancelled"
        return
    }

    Write-Host ""
    Write-GscInfo "Restoring files..."
    Write-Host ""

    # Get repository root (from .specify/scripts/gsc → .specify/scripts → .specify → repo root)
    $repoRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
    $filesDir = Join-Path $checkpointDir "files"

    $restoredCount = 0
    $errorCount = 0

    # Restore each file
    foreach ($relPath in $metadata.modifiedFiles) {
        $sourcePath = Join-Path $filesDir $relPath
        $targetPath = Join-Path $repoRoot $relPath

        if (Test-Path $sourcePath) {
            try {
                # Ensure target directory exists
                $targetDir = Split-Path $targetPath -Parent
                if (-not (Test-Path $targetDir)) {
                    New-Item -Path $targetDir -ItemType Directory -Force | Out-Null
                }

                Copy-Item -Path $sourcePath -Destination $targetPath -Force
                Write-Host "  ✅ " -ForegroundColor Green -NoNewline
                Write-Host $relPath -ForegroundColor Gray
                $restoredCount++
            }
            catch {
                Write-Host "  ❌ " -ForegroundColor Red -NoNewline
                Write-Host "$relPath - Error: $_" -ForegroundColor Red
                $errorCount++
            }
        }
        else {
            Write-Host "  ⚠️  " -ForegroundColor Yellow -NoNewline
            Write-Host "$relPath - File not found in checkpoint" -ForegroundColor Yellow
            $errorCount++
        }
    }

    # Restore state if available
    if ($metadata.stateSnapshot) {
        Set-CurrentState -State $metadata.stateSnapshot
        Write-Host ""
        Write-GscSuccess "Workflow state restored"
    }

    # Log restoration
    Add-StateHistory -Action "CheckpointRestored" -Details "$CheckpointId - $restoredCount files restored"

    Write-Host ""
    Write-GscSuccess "Checkpoint restoration complete!"
    Write-Host ""
    Write-Host "Files restored: " -ForegroundColor Gray -NoNewline
    Write-Host $restoredCount -ForegroundColor Green
    if ($errorCount -gt 0) {
        Write-Host "Errors: " -ForegroundColor Gray -NoNewline
        Write-Host $errorCount -ForegroundColor Red
    }
    Write-Host ""

    exit 0
}

function Remove-AllCheckpoints {
    <#
    .SYNOPSIS
        Remove all checkpoints and reset state (with confirmation)
    #>

    Write-GscHeader -Title "Full Reset" -SubTitle "Remove All Checkpoints"

    $stateDir = Initialize-StateDirectory
    $checkpointsDir = Join-Path $stateDir "checkpoints"

    if (-not (Test-Path $checkpointsDir)) {
        Write-GscInfo "No checkpoints to remove"
        return
    }

    $checkpointDirs = Get-ChildItem -Path $checkpointsDir -Directory

    if ($checkpointDirs.Count -eq 0) {
        Write-GscInfo "No checkpoints to remove"
        return
    }

    Write-Host "⚠️  WARNING: This will permanently delete ALL checkpoints!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Checkpoints to delete: " -ForegroundColor Yellow -NoNewline
    Write-Host $checkpointDirs.Count
    Write-Host ""

    foreach ($cpDir in $checkpointDirs) {
        Write-Host "  • " -NoNewline
        Write-Host $cpDir.Name -ForegroundColor Gray
    }

    Write-Host ""
    Write-Host "Type 'DELETE' to confirm full reset: " -ForegroundColor Red -NoNewline
    $confirmation = Read-Host

    if ($confirmation -ne 'DELETE') {
        Write-GscInfo "Full reset cancelled"
        return
    }

    Write-Host ""
    Write-GscInfo "Removing all checkpoints..."

    try {
        Remove-Item -Path $checkpointsDir -Recurse -Force
        New-Item -Path $checkpointsDir -ItemType Directory -Force | Out-Null

        # Reset checkpoint count in state
        $currentState = Get-CurrentState
        if ($currentState) {
            $currentState.checkpointsCount = 0
            Set-CurrentState -State $currentState
        }

        Add-StateHistory -Action "FullReset" -Details "All checkpoints removed"

        Write-Host ""
        Write-GscSuccess "All checkpoints removed successfully"
        Write-Host ""
        exit 0
    }
    catch {
        Write-GscError "Failed to remove checkpoints: $_"
        Write-Host ""
        exit 1
    }
}

function Show-RollbackUsage {
    Write-GscHeader -Title "GSC Rollback Command" -SubTitle "Checkpoint & Rollback System"

    Write-Host "USAGE:" -ForegroundColor Yellow
    Write-Host "  gsc rollback checkpoint <description>" -ForegroundColor Cyan
    Write-Host "  gsc rollback list" -ForegroundColor Cyan
    Write-Host "  gsc rollback restore <checkpoint-id>" -ForegroundColor Cyan
    Write-Host "  gsc rollback full-reset" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "EXAMPLES:" -ForegroundColor Yellow
    Write-Host "  # Create checkpoint before risky change" -ForegroundColor Gray
    Write-Host "  gsc rollback checkpoint " -NoNewline -ForegroundColor Cyan
    Write-Host '"before refactoring"' -ForegroundColor White
    Write-Host ""
    Write-Host "  # List all checkpoints" -ForegroundColor Gray
    Write-Host "  gsc rollback list" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # Restore specific checkpoint" -ForegroundColor Gray
    Write-Host "  gsc rollback restore checkpoint-20251010-143022" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  # Remove all checkpoints (requires confirmation)" -ForegroundColor Gray
    Write-Host "  gsc rollback full-reset" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "CHECKPOINT STRUCTURE:" -ForegroundColor Yellow
    Write-Host "  .specify/state/checkpoints/" -ForegroundColor White
    Write-Host "    └── checkpoint-<timestamp>/" -ForegroundColor Gray
    Write-Host "        ├── metadata.json      (checkpoint info)" -ForegroundColor Gray
    Write-Host "        └── files/             (file snapshots)" -ForegroundColor Gray
    Write-Host ""
}

# Main execution
if (-not $Action) {
    Show-RollbackUsage
} else {
    switch ($Action) {
        'checkpoint' {
            if (-not $Description) {
                Write-GscError "Checkpoint description is required"
                Write-Host ""
                Write-Host "Usage: gsc rollback checkpoint " -NoNewline -ForegroundColor Yellow
                Write-Host '"description"' -ForegroundColor White
                Write-Host ""
                Write-Host "Example:" -ForegroundColor Gray
                Write-Host "  gsc rollback checkpoint " -NoNewline -ForegroundColor Cyan
                Write-Host '"before refactoring viewmodel"' -ForegroundColor White
                Write-Host ""
            }
            else {
                New-Checkpoint -Description $Description
            }
        }
        'list' {
            Get-Checkpoints
        }
        'restore' {
            if (-not $Description) {
                Write-GscError "Checkpoint ID is required"
                Write-Host ""
                Write-Host "Usage: gsc rollback restore <checkpoint-id>" -ForegroundColor Yellow
                Write-Host ""
                Get-Checkpoints
            }
            else {
                Restore-Checkpoint -CheckpointId $Description
            }
        }
        'full-reset' {
            Remove-AllCheckpoints
        }
    }
}
