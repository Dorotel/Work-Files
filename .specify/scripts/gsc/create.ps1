<#
.SYNOPSIS
    GSC Create Command - Create new features, specs, or tasks

.DESCRIPTION
    Creates feature structures, specification files, or task breakdowns
    using constitutional templates

.PARAMETER Type
    Type to create: feature, spec, tasks

.PARAMETER Name
    Name of the feature/spec/tasks to create

.EXAMPLE
    gsc create feature inventory-export
    gsc create spec inventory-export
    gsc create tasks inventory-export

.NOTES
    Version: 1.0.0
    This command wraps existing PowerShell scripts with GSC interface
#>

param(
    [Parameter(Mandatory=$false, Position=0)]
    [ValidateSet('feature', 'spec', 'tasks', '')]
    [string]$Type = '',

    [Parameter(Mandatory=$false, Position=1)]
    [string]$Name = '',

    [Parameter(ValueFromRemainingArguments=$true)]
    [string[]]$AdditionalArgs
)

# Import common utilities
$commonPath = Join-Path $PSScriptRoot "common"
. (Join-Path $commonPath "output.ps1")
. (Join-Path $commonPath "state.ps1")

# Get PowerShell scripts directory
$powershellScriptsDir = Join-Path (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)) "scripts\powershell"

function Invoke-CreateFeature {
    param([string]$FeatureName)

    if (-not $FeatureName) {
        Write-GscError "Feature name is required"
        Write-Host ""
        Write-Host "Usage: gsc create feature <feature-name>" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Example:" -ForegroundColor Gray
        Write-Host "  gsc create feature inventory-export" -ForegroundColor Cyan
        Write-Host ""
        return
    }

    Write-GscHeader -Title "Creating New Feature" -SubTitle "Feature: $FeatureName"

    # Call existing PowerShell script
    $createScript = Join-Path $powershellScriptsDir "create-new-feature.ps1"

    if (-not (Test-Path $createScript)) {
        Write-GscError "Create feature script not found: $createScript"
        return
    }

    Write-GscInfo "Executing feature creation..."
    Write-Host ""

    try {
        # Execute the existing script
        & $createScript $FeatureName

        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-GscSuccess "Feature structure created successfully!"
            Write-Host ""

            # Initialize state for this feature
            $newState = New-EmptyState -FeatureName $FeatureName
            Set-CurrentState -State $newState

            Write-GscInfo "Next steps:"
            Write-Host "  1. Review and edit: " -ForegroundColor Gray -NoNewline
            Write-Host "specs/$FeatureName/spec.md" -ForegroundColor Cyan
            Write-Host "  2. Check progress: " -ForegroundColor Gray -NoNewline
            Write-Host "gsc status" -ForegroundColor Cyan
            Write-Host "  3. Advance workflow: " -ForegroundColor Gray -NoNewline
            Write-Host "gsc workflow next" -ForegroundColor Cyan
            Write-Host ""
        } else {
            Write-GscError "Feature creation failed"
        }
    }
    catch {
        Write-GscError "Error creating feature: $_"
        Write-Host ""
        Write-Host $_.Exception.Message -ForegroundColor Red
        Write-Host ""
    }
}

function Invoke-CreateSpec {
    param([string]$FeatureName)

    if (-not $FeatureName) {
        Write-GscError "Feature name is required"
        Write-Host ""
        Write-Host "Usage: gsc create spec <feature-name>" -ForegroundColor Yellow
        return
    }

    Write-GscInfo "Creating specification for: $FeatureName"
    Write-Host ""

    # Get template path
    $specifyRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
    $templatePath = Join-Path $specifyRoot "templates\spec.md"

    if (-not (Test-Path $templatePath)) {
        Write-GscError "Spec template not found: $templatePath"
        return
    }

    # Create specs directory
    $specsDir = Join-Path (Split-Path -Parent $specifyRoot) "specs"
    $featureDir = Join-Path $specsDir $FeatureName

    if (-not (Test-Path $featureDir)) {
        New-Item -Path $featureDir -ItemType Directory -Force | Out-Null
    }

    # Copy template
    $specPath = Join-Path $featureDir "spec.md"
    Copy-Item -Path $templatePath -Destination $specPath -Force

    Write-GscSuccess "Specification created: $specPath"
    Write-Host ""
    Write-GscInfo "Edit the file and complete the specification sections"
    Write-Host ""
}

function Invoke-CreateTasks {
    param([string]$FeatureName)

    if (-not $FeatureName) {
        Write-GscError "Feature name is required"
        Write-Host ""
        Write-Host "Usage: gsc create tasks <feature-name>" -ForegroundColor Yellow
        return
    }

    Write-GscInfo "Creating task breakdown for: $FeatureName"
    Write-Host ""

    # Get template path
    $specifyRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
    $templatePath = Join-Path $specifyRoot "templates\tasks.md"

    if (-not (Test-Path $templatePath)) {
        Write-GscError "Tasks template not found: $templatePath"
        return
    }

    # Create specs directory
    $specsDir = Join-Path (Split-Path -Parent $specifyRoot) "specs"
    $featureDir = Join-Path $specsDir $FeatureName

    if (-not (Test-Path $featureDir)) {
        New-Item -Path $featureDir -ItemType Directory -Force | Out-Null
    }

    # Copy template
    $tasksPath = Join-Path $featureDir "tasks.md"
    Copy-Item -Path $templatePath -Destination $tasksPath -Force

    Write-GscSuccess "Task breakdown created: $tasksPath"
    Write-Host ""
    Write-GscInfo "Edit the file and define your implementation tasks"
    Write-Host ""
}

function Show-CreateUsage {
    Write-GscHeader -Title "GSC Create Command" -SubTitle "Create new features, specs, or tasks"

    Write-Host "USAGE:" -ForegroundColor Yellow
    Write-Host "  gsc create feature <feature-name>" -ForegroundColor Cyan
    Write-Host "  gsc create spec <feature-name>" -ForegroundColor Cyan
    Write-Host "  gsc create tasks <feature-name>" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "EXAMPLES:" -ForegroundColor Yellow
    Write-Host "  gsc create feature inventory-export" -ForegroundColor Cyan
    Write-Host "  gsc create spec data-validation" -ForegroundColor Cyan
    Write-Host "  gsc create tasks performance-optimization" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "For detailed help: " -ForegroundColor Gray -NoNewline
    Write-Host "gsc help create" -ForegroundColor Cyan
    Write-Host ""
}

# Main execution
if (-not $Type) {
    Show-CreateUsage
    exit 0
}

switch ($Type) {
    'feature' {
        Invoke-CreateFeature -FeatureName $Name
    }
    'spec' {
        Invoke-CreateSpec -FeatureName $Name
    }
    'tasks' {
        Invoke-CreateTasks -FeatureName $Name
    }
    default {
        Show-CreateUsage
    }
}
