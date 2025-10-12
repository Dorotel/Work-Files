<#
.SYNOPSIS
    State management utilities for GSC workflow

.DESCRIPTION
    Provides JSON-based state persistence for tracking workflow progress
#>

function Get-StateDirectory {
    <#
    .SYNOPSIS
        Get the .specify/state/ directory path
    #>
    # From .specify/scripts/gsc/common → .specify/scripts/gsc → .specify/scripts → .specify
    $specifyRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
    return Join-Path $specifyRoot "state"
}

function Initialize-StateDirectory {
    <#
    .SYNOPSIS
        Create state directory structure if it doesn't exist
    #>
    $stateDir = Get-StateDirectory

    if (-not (Test-Path $stateDir)) {
        New-Item -Path $stateDir -ItemType Directory -Force | Out-Null
    }

    # Create subdirectories
    $checkpointsDir = Join-Path $stateDir "checkpoints"
    if (-not (Test-Path $checkpointsDir)) {
        New-Item -Path $checkpointsDir -ItemType Directory -Force | Out-Null
    }

    # Create history log if it doesn't exist
    $historyLog = Join-Path $stateDir "history.log"
    if (-not (Test-Path $historyLog)) {
        "# GSC State Change History" | Out-File -FilePath $historyLog -Encoding UTF8
        "# Created: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" | Out-File -FilePath $historyLog -Append -Encoding UTF8
        "" | Out-File -FilePath $historyLog -Append -Encoding UTF8
    }

    return $stateDir
}

function Get-CurrentState {
    <#
    .SYNOPSIS
        Read current workflow state from JSON file

    .OUTPUTS
        PSCustomObject with current state, or $null if no active state
    #>
    $stateDir = Initialize-StateDirectory
    $stateFile = Join-Path $stateDir "current-state.json"

    if (-not (Test-Path $stateFile)) {
        return $null
    }

    try {
        $json = Get-Content -Path $stateFile -Raw -Encoding UTF8
        return $json | ConvertFrom-Json
    }
    catch {
        Write-Warning "Failed to read state file: $_"
        return $null
    }
}

function Set-CurrentState {
    <#
    .SYNOPSIS
        Update workflow state in JSON file

    .PARAMETER State
        State object to persist

    .PARAMETER Append
        If true, merge with existing state instead of replacing
    #>
    param(
        [Parameter(Mandatory=$true)]
        [PSCustomObject]$State,

        [Parameter(Mandatory=$false)]
        [switch]$Append
    )

    $stateDir = Initialize-StateDirectory
    $stateFile = Join-Path $stateDir "current-state.json"

    if ($Append -and (Test-Path $stateFile)) {
        $existingState = Get-CurrentState

        # Merge states
        foreach ($property in $State.PSObject.Properties) {
            $existingState | Add-Member -MemberType NoteProperty -Name $property.Name -Value $property.Value -Force
        }

        $State = $existingState
    }

    # Add/update timestamp
    $State | Add-Member -MemberType NoteProperty -Name "lastUpdateTimestamp" -Value (Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ") -Force

    # Write to file
    try {
        $json = $State | ConvertTo-Json -Depth 10
        $json | Out-File -FilePath $stateFile -Encoding UTF8 -Force

        # Log state change
        Add-StateHistory -Action "StateUpdated" -Details "State updated for feature: $($State.currentFeature)"

        return $true
    }
    catch {
        Write-Warning "Failed to write state file: $_"
        return $false
    }
}

function Clear-CurrentState {
    <#
    .SYNOPSIS
        Clear current workflow state (workflow complete or abandoned)
    #>
    $stateDir = Get-StateDirectory
    $stateFile = Join-Path $stateDir "current-state.json"

    if (Test-Path $stateFile) {
        $state = Get-CurrentState
        Add-StateHistory -Action "StateCleared" -Details "Cleared state for feature: $($state.currentFeature)"
        Remove-Item -Path $stateFile -Force
    }
}

function Add-StateHistory {
    <#
    .SYNOPSIS
        Append entry to state change history log

    .PARAMETER Action
        Action performed (e.g., "StateUpdated", "CheckpointCreated")

    .PARAMETER Details
        Additional details about the action
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$Action,

        [Parameter(Mandatory=$false)]
        [string]$Details = ""
    )

    $stateDir = Initialize-StateDirectory
    $historyLog = Join-Path $stateDir "history.log"

    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $entry = "[$timestamp] $Action"
    if ($Details) {
        $entry += " - $Details"
    }

    $entry | Out-File -FilePath $historyLog -Append -Encoding UTF8
}

function New-EmptyState {
    <#
    .SYNOPSIS
        Create a new empty state object with default values

    .PARAMETER FeatureName
        Name of the feature being developed
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$FeatureName
    )

    return [PSCustomObject]@{
        currentFeature = $FeatureName
        currentPhase = "Specification"
        currentTask = 0
        totalTasks = 0
        startTimestamp = Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ"
        lastUpdateTimestamp = Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ"
        modifiedFiles = @()
        checkpointsCount = 0
        constitutionalCompliance = @{
            codeQuality = $false
            testingStandards = $false
            uxConsistency = $false
            performance = $false
        }
    }
}
