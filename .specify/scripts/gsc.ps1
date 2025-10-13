<#
.SYNOPSIS
    GSC (GitHub Copilot Spec Commands) - Unified command interface for .specify workflow

.DESCRIPTION
    Main entry point for GSC command system. Routes commands to appropriate handlers.

    For complex commands with switches, you can call command scripts directly:
        .\gsc\validate.ps1 all -Summary
        .\gsc\rollback.ps1 restore checkpoint-name

    Or use the wrapper for simple commands:
        .\gsc.ps1 help
        .\gsc.ps1 validate
        .\gsc.ps1 status

.EXAMPLE
    .\gsc.ps1 help
    .\gsc.ps1 validate
    .\gsc\validate.ps1 all -Summary

.NOTES
    Version: 1.1.0
    Author: MTM Development Team
    Date: October 10, 2025
#>

# Get first argument as command
if ($args.Count -eq 0) {
    $Command = 'help'
    $commandArgs = @()
} else {
    $Command = $args[0]
    $commandArgs = $args[1..($args.Count - 1)]
}

# Set error action preference
$ErrorActionPreference = 'Stop'

# Get GSC root directory
$gscRoot = $PSScriptRoot
$gscCommandsDir = Join-Path $gscRoot "gsc"

# Display banner on first run
if ($Command -eq 'help' -and $commandArgs.Count -eq 0) {
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "   GSC - GitHub Copilot Spec Commands v1.0.0" -ForegroundColor White
    Write-Host "   Intelligent Development Assistant for .specify Workflow" -ForegroundColor Gray
    Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
}

# Validate command directory exists
if (-not (Test-Path $gscCommandsDir)) {
    Write-Host "❌ " -ForegroundColor Red -NoNewline
    Write-Host "GSC commands directory not found: $gscCommandsDir"
    Write-Host "Please ensure GSC system is fully installed." -ForegroundColor Yellow
    exit 1
}

# Route to command handler
$commandScript = Join-Path $gscCommandsDir "$Command.ps1"

if (-not (Test-Path $commandScript)) {
    Write-Host "❌ " -ForegroundColor Red -NoNewline
    Write-Host "Command not found: $Command"
    Write-Host ""
    Write-Host "Available commands:" -ForegroundColor Yellow
    Write-Host "  create       - Create new features, specs, or tasks" -ForegroundColor Cyan
    Write-Host "  validate     - Validate constitutional compliance" -ForegroundColor Cyan
    Write-Host "  status       - Show current progress and state" -ForegroundColor Cyan
    Write-Host "  rollback     - Manage checkpoints and rollback changes" -ForegroundColor Cyan
    Write-Host "  memory       - Access constitutional memory system" -ForegroundColor Cyan
    Write-Host "  workflow     - Orchestrate end-to-end feature development" -ForegroundColor Cyan
    Write-Host "  housekeeping - Documentation inventory and cleanup" -ForegroundColor Cyan
    Write-Host "  help         - Display help information" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Run 'gsc help' for more information" -ForegroundColor Gray
    exit 1
}

# Execute command with arguments
try {
    if ($commandArgs.Count -gt 0) {
        & $commandScript @commandArgs
    } else {
        & $commandScript
    }
    exit $LASTEXITCODE
}
catch {
    Write-Host ""
    Write-Host "❌ " -ForegroundColor Red -NoNewline
    Write-Host "Error executing command '$Command': $_"
    Write-Host ""
    Write-Host "For help, run: " -ForegroundColor Cyan -NoNewline
    Write-Host "gsc help $Command" -ForegroundColor White
    Write-Host ""
    exit 1
}
