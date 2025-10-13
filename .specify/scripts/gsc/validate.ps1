<#
.SYNOPSIS
    GSC Validate Command - Validate constitutional compliance

.DESCRIPTION
    Validates code, tests, and implementation against constitutional principles.
    Executes automated checks for all 4 principles and generates compliance report.

.PARAMETER Target
    Validation target:
    - all: Run all validation checks (default)
    - constitution: Run all validation checks (alias for 'all')
    - code: Code Quality Excellence only
    - tests: Testing Standards only
    - ux: UX Consistency only
    - performance: Performance Requirements only

.PARAMETER Detailed
    Show detailed violation reports (file, line, issue)

.PARAMETER Summary
    Show summary only (no detailed violations)

.EXAMPLE
    gsc validate
    gsc validate constitution
    gsc validate all --detailed
    gsc validate code
    gsc validate ux --summary

.NOTES
    Version: 2.0.0
    Phase 4 Implementation
#>

param(
    [Parameter(Mandatory=$false, Position=0)]
    [ValidateSet('all', 'code', 'tests', 'ux', 'performance', 'constitution', '')]
    [string]$Target = 'all',

    [Parameter(Mandatory=$false)]
    [switch]$Detailed,

    [Parameter(Mandatory=$false)]
    [switch]$Summary
)

# Import common utilities
$commonPath = Join-Path $PSScriptRoot "common"
. (Join-Path $commonPath "output.ps1")
. (Join-Path $commonPath "state.ps1")
. (Join-Path $commonPath "constitution.ps1")

# Determine what to run
$runAll = ($Target -eq 'all' -or $Target -eq '' -or $Target -eq 'constitution')
$runCode = $runAll -or ($Target -eq 'code')
$runTests = $runAll -or ($Target -eq 'tests')
$runUX = $runAll -or ($Target -eq 'ux')
$runPerformance = $runAll -or ($Target -eq 'performance')

# Show detailed by default unless --summary specified
$showDetailed = (-not $Summary) -or $Detailed

Write-GscHeader -Title "Constitutional Compliance Validation" -SubTitle "Target: $Target"

$results = @()
$repoRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))

# Run selected validations
if ($runCode) {
    Write-GscSection "Principle I: Code Quality Excellence"
    $codeResult = Test-CodeQuality -RepositoryRoot $repoRoot
    $results += $codeResult

    Write-Host "Files scanned: " -ForegroundColor Gray -NoNewline
    Write-Host $codeResult.FilesScanned -ForegroundColor White
    Write-Host "Violations: " -ForegroundColor Gray -NoNewline
    Write-Host $codeResult.ViolationCount -ForegroundColor $(if ($codeResult.ViolationCount -eq 0) { "Green" } else { "Red" })
    Write-Host "Compliance score: " -ForegroundColor Gray -NoNewline

    $scoreColor = switch ($codeResult.Status) {
        "Pass" { "Green" }
        "Warning" { "Yellow" }
        "Fail" { "Red" }
    }
    Write-Host "$($codeResult.ComplianceScore)% - $($codeResult.Status)" -ForegroundColor $scoreColor
    Write-Host ""

    if ($showDetailed -and $codeResult.Violations.Count -gt 0) {
        Write-Host "Violations:" -ForegroundColor Yellow
        foreach ($violation in $codeResult.Violations) {
            $severityColor = switch ($violation.Severity) {
                "High" { "Red" }
                "Medium" { "Yellow" }
                "Low" { "Gray" }
            }
            Write-Host "  [$($violation.Severity)] " -ForegroundColor $severityColor -NoNewline
            Write-Host "$($violation.File):$($violation.Line)" -ForegroundColor Cyan
            Write-Host "    $($violation.Issue)" -ForegroundColor Gray
            Write-Host "    $($violation.Pattern)" -ForegroundColor DarkGray
        }
        Write-Host ""
    }
}

if ($runTests) {
    Write-GscSection "Principle II: Testing Standards"
    $testResult = Test-TestingStandards -RepositoryRoot $repoRoot
    $results += $testResult

    Write-Host "Files scanned: " -ForegroundColor Gray -NoNewline
    Write-Host $testResult.FilesScanned -ForegroundColor White
    Write-Host "Violations: " -ForegroundColor Gray -NoNewline
    Write-Host $testResult.ViolationCount -ForegroundColor $(if ($testResult.ViolationCount -eq 0) { "Green" } else { "Red" })
    Write-Host "Compliance score: " -ForegroundColor Gray -NoNewline

    $scoreColor = switch ($testResult.Status) {
        "Pass" { "Green" }
        "Warning" { "Yellow" }
        "Fail" { "Red" }
    }
    Write-Host "$($testResult.ComplianceScore)% - $($testResult.Status)" -ForegroundColor $scoreColor
    Write-Host ""

    if ($showDetailed -and $testResult.Violations.Count -gt 0) {
        Write-Host "Violations:" -ForegroundColor Yellow
        foreach ($violation in $testResult.Violations | Select-Object -First 10) {
            $severityColor = switch ($violation.Severity) {
                "High" { "Red" }
                "Medium" { "Yellow" }
                "Low" { "Gray" }
            }
            Write-Host "  [$($violation.Severity)] " -ForegroundColor $severityColor -NoNewline
            Write-Host "$($violation.File):$($violation.Line)" -ForegroundColor Cyan
            Write-Host "    $($violation.Issue)" -ForegroundColor Gray
        }
        if ($testResult.Violations.Count -gt 10) {
            Write-Host "  ... and $($testResult.Violations.Count - 10) more" -ForegroundColor DarkGray
        }
        Write-Host ""
    }
}

if ($runUX) {
    Write-GscSection "Principle III: UX Consistency"
    $uxResult = Test-UXConsistency -RepositoryRoot $repoRoot
    $results += $uxResult

    Write-Host "Files scanned: " -ForegroundColor Gray -NoNewline
    Write-Host $uxResult.FilesScanned -ForegroundColor White
    Write-Host "Violations: " -ForegroundColor Gray -NoNewline
    Write-Host $uxResult.ViolationCount -ForegroundColor $(if ($uxResult.ViolationCount -eq 0) { "Green" } else { "Red" })
    Write-Host "Compliance score: " -ForegroundColor Gray -NoNewline

    $scoreColor = switch ($uxResult.Status) {
        "Pass" { "Green" }
        "Warning" { "Yellow" }
        "Fail" { "Red" }
    }
    Write-Host "$($uxResult.ComplianceScore)% - $($uxResult.Status)" -ForegroundColor $scoreColor
    Write-Host ""

    if ($showDetailed -and $uxResult.Violations.Count -gt 0) {
        Write-Host "Violations:" -ForegroundColor Yellow
        foreach ($violation in $uxResult.Violations | Select-Object -First 10) {
            $severityColor = switch ($violation.Severity) {
                "High" { "Red" }
                "Medium" { "Yellow" }
                "Low" { "Gray" }
            }
            Write-Host "  [$($violation.Severity)] " -ForegroundColor $severityColor -NoNewline
            Write-Host "$($violation.File):$($violation.Line)" -ForegroundColor Cyan
            Write-Host "    $($violation.Issue)" -ForegroundColor Gray
            if ($violation.Pattern.Length -le 80) {
                Write-Host "    $($violation.Pattern)" -ForegroundColor DarkGray
            }
        }
        if ($uxResult.Violations.Count -gt 10) {
            Write-Host "  ... and $($uxResult.Violations.Count - 10) more" -ForegroundColor DarkGray
        }
        Write-Host ""
    }
}

if ($runPerformance) {
    Write-GscSection "Principle IV: Performance Requirements"
    $perfResult = Test-Performance -RepositoryRoot $repoRoot
    $results += $perfResult

    Write-Host "Files scanned: " -ForegroundColor Gray -NoNewline
    Write-Host $perfResult.FilesScanned -ForegroundColor White
    Write-Host "Violations: " -ForegroundColor Gray -NoNewline
    Write-Host $perfResult.ViolationCount -ForegroundColor $(if ($perfResult.ViolationCount -eq 0) { "Green" } else { "Red" })
    Write-Host "Compliance score: " -ForegroundColor Gray -NoNewline

    $scoreColor = switch ($perfResult.Status) {
        "Pass" { "Green" }
        "Warning" { "Yellow" }
        "Fail" { "Red" }
    }
    Write-Host "$($perfResult.ComplianceScore)% - $($perfResult.Status)" -ForegroundColor $scoreColor
    Write-Host ""

    if ($showDetailed -and $perfResult.Violations.Count -gt 0) {
        Write-Host "Violations:" -ForegroundColor Yellow
        foreach ($violation in $perfResult.Violations | Select-Object -First 10) {
            $severityColor = switch ($violation.Severity) {
                "High" { "Red" }
                "Medium" { "Yellow" }
                "Low" { "Gray" }
            }
            Write-Host "  [$($violation.Severity)] " -ForegroundColor $severityColor -NoNewline
            Write-Host "$($violation.File):$($violation.Line)" -ForegroundColor Cyan
            Write-Host "    $($violation.Issue)" -ForegroundColor Gray
            Write-Host "    $($violation.Pattern)" -ForegroundColor DarkGray
        }
        if ($perfResult.Violations.Count -gt 10) {
            Write-Host "  ... and $($perfResult.Violations.Count - 10) more" -ForegroundColor DarkGray
        }
        Write-Host ""
    }
}

# Overall compliance summary
if ($results.Count -gt 0) {
    Write-GscSection "Overall Compliance"

    $overall = Get-OverallCompliance -Results $results

    Write-Host "Overall score: " -ForegroundColor Gray -NoNewline
    $overallColor = switch ($overall.Status) {
        "Pass" { "Green" }
        "Warning" { "Yellow" }
        "Fail" { "Red" }
    }
    Write-Host "$($overall.OverallScore)% - $($overall.Status)" -ForegroundColor $overallColor
    Write-Host ""

    Write-Host "Principles passed: " -ForegroundColor Gray -NoNewline
    Write-Host "$($overall.PrinciplesPassed)/$($results.Count)" -ForegroundColor Green

    if ($overall.PrinciplesWarning -gt 0) {
        Write-Host "Principles with warnings: " -ForegroundColor Gray -NoNewline
        Write-Host $overall.PrinciplesWarning -ForegroundColor Yellow
    }

    if ($overall.PrinciplesFailed -gt 0) {
        Write-Host "Principles failed: " -ForegroundColor Gray -NoNewline
        Write-Host $overall.PrinciplesFailed -ForegroundColor Red
    }

    Write-Host ""
    Write-Host "Total violations: " -ForegroundColor Gray -NoNewline
    Write-Host $overall.TotalViolations -ForegroundColor $(if ($overall.TotalViolations -eq 0) { "Green" } else { "Red" })
    Write-Host ""

    # Update state with compliance info
    $currentState = Get-CurrentState
    if ($currentState) {
        $currentState.constitutionalCompliance = @{
            codeQuality = ($results | Where-Object { $_.Principle -eq "Code Quality Excellence" }).Status -eq "Pass"
            testingStandards = ($results | Where-Object { $_.Principle -eq "Testing Standards" }).Status -eq "Pass"
            uxConsistency = ($results | Where-Object { $_.Principle -eq "UX Consistency" }).Status -eq "Pass"
            performance = ($results | Where-Object { $_.Principle -eq "Performance Requirements" }).Status -eq "Pass"
            overallScore = $overall.OverallScore
            lastValidation = Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ"
        }
        Set-CurrentState -State $currentState | Out-Null
    }

    # Display next steps
    if ($overall.TotalViolations -gt 0) {
        Write-GscInfo "To fix violations:"
        Write-Host "  1. Review violation details above" -ForegroundColor Gray
        Write-Host "  2. Create a checkpoint before fixing: " -ForegroundColor Gray -NoNewline
        Write-Host "gsc rollback checkpoint 'before fixes'" -ForegroundColor Cyan
        Write-Host "  3. Fix violations following constitutional guidance" -ForegroundColor Gray
        Write-Host "  4. Re-run validation: " -ForegroundColor Gray -NoNewline
        Write-Host "gsc validate" -ForegroundColor Cyan
        Write-Host ""

        # Exit with code 1 to indicate violations found (standard practice for validation tools)
        exit 1
    } else {
        Write-GscSuccess "All constitutional principles satisfied! 🎉"
        exit 0
    }
}
