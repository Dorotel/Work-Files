<#
.SYNOPSIS
    Output formatting utilities for GSC commands

.DESCRIPTION
    Provides consistent, color-coded output formatting across all GSC commands
#>

function Write-GscHeader {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Title,

        [Parameter(Mandatory=$false)]
        [string]$SubTitle = ""
    )

    $border = "═" * 63

    Write-Host ""
    Write-Host $border -ForegroundColor Cyan
    Write-Host "   $Title" -ForegroundColor White
    if ($SubTitle) {
        Write-Host "   $SubTitle" -ForegroundColor Gray
    }
    Write-Host $border -ForegroundColor Cyan
    Write-Host ""
}

function Write-GscSuccess {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Message
    )

    Write-Host "✅ " -ForegroundColor Green -NoNewline
    Write-Host $Message -ForegroundColor White
}

function Write-GscError {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Message
    )

    Write-Host "❌ " -ForegroundColor Red -NoNewline
    Write-Host $Message -ForegroundColor White
}

function Write-GscWarning {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Message
    )

    Write-Host "⚠️  " -ForegroundColor Yellow -NoNewline
    Write-Host $Message -ForegroundColor White
}

function Write-GscInfo {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Message
    )

    Write-Host "ℹ️  " -ForegroundColor Cyan -NoNewline
    Write-Host $Message -ForegroundColor White
}

function Write-GscProgress {
    param(
        [Parameter(Mandatory=$true)]
        [int]$Current,

        [Parameter(Mandatory=$true)]
        [int]$Total,

        [Parameter(Mandatory=$false)]
        [string]$Activity = "Progress"
    )

    $percentage = [math]::Round(($Current / $Total) * 100, 0)
    $barLength = 40
    $filledLength = [math]::Round(($percentage / 100) * $barLength)

    $bar = "█" * $filledLength + "░" * ($barLength - $filledLength)

    Write-Host ""
    Write-Host "$Activity : " -ForegroundColor Gray -NoNewline
    Write-Host "[$bar] " -ForegroundColor Cyan -NoNewline
    Write-Host "$percentage%" -ForegroundColor White -NoNewline
    Write-Host " ($Current/$Total)" -ForegroundColor Gray
    Write-Host ""
}

function Write-GscSection {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Title
    )

    Write-Host ""
    Write-Host "──────────────────────────────────────────────────────────────" -ForegroundColor DarkGray
    Write-Host " $Title" -ForegroundColor White
    Write-Host "──────────────────────────────────────────────────────────────" -ForegroundColor DarkGray
    Write-Host ""
}

function Write-GscSectionHeader {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Title
    )

    Write-Host $Title -ForegroundColor White
    Write-Host ("─" * $Title.Length) -ForegroundColor DarkGray
}

function Write-GscTable {
    param(
        [Parameter(Mandatory=$true)]
        [array]$Data,

        [Parameter(Mandatory=$true)]
        [string[]]$Headers
    )

    # Calculate column widths
    $columnWidths = @()
    foreach ($header in $Headers) {
        $maxWidth = $header.Length
        foreach ($row in $Data) {
            $value = $row.$header
            if ($value -and $value.ToString().Length -gt $maxWidth) {
                $maxWidth = $value.ToString().Length
            }
        }
        $columnWidths += $maxWidth + 2
    }

    # Print headers
    Write-Host ""
    for ($i = 0; $i -lt $Headers.Count; $i++) {
        Write-Host $Headers[$i].PadRight($columnWidths[$i]) -ForegroundColor Cyan -NoNewline
    }
    Write-Host ""

    # Print separator
    $separator = ""
    foreach ($width in $columnWidths) {
        $separator += "─" * $width
    }
    Write-Host $separator -ForegroundColor DarkGray

    # Print data rows
    foreach ($row in $Data) {
        for ($i = 0; $i -lt $Headers.Count; $i++) {
            $value = $row.($Headers[$i])
            if (-not $value) { $value = "" }
            Write-Host $value.ToString().PadRight($columnWidths[$i]) -ForegroundColor White -NoNewline
        }
        Write-Host ""
    }
    Write-Host ""
}

function Write-GscBanner {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Message,

        [Parameter(Mandatory=$false)]
        [string]$Icon = "🎉"
    )

    $border = "═" * 63

    Write-Host ""
    Write-Host $border -ForegroundColor Green
    Write-Host "   $Icon $Message" -ForegroundColor White
    Write-Host $border -ForegroundColor Green
    Write-Host ""
}
