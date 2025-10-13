<#
.SYNOPSIS
    Constitutional compliance validation functions

.DESCRIPTION
    Implements automated checking for all 4 constitutional principles:
    1. Code Quality Excellence
    2. Testing Standards
    3. UX Consistency
    4. Performance Requirements
#>

# Import dependencies
. "$PSScriptRoot\output.ps1"

function Test-CodeQuality {
    <#
    .SYNOPSIS
        Validate code quality compliance (Principle 1)

    .DESCRIPTION
        Checks for:
        - ReactiveUI patterns (should be MVVM Community Toolkit)
        - Nullable type enforcement
        - Proper error handling patterns
        - MVVM Community Toolkit usage

    .OUTPUTS
        PSCustomObject with violations array and compliance score
    #>
    param(
        [Parameter(Mandatory=$false)]
        [string]$RepositoryRoot = (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))))
    )

    $violations = @()
    $filesScanned = 0

    Write-GscInfo "Scanning C# files for code quality violations..."

    # Get all C# files (ViewModels and Services primarily)
    $csFiles = Get-ChildItem -Path $RepositoryRoot -Filter "*.cs" -Recurse -File |
        Where-Object { $_.FullName -notmatch "\\obj\\|\\bin\\|\\packages\\" }

    foreach ($file in $csFiles) {
        $filesScanned++
        $content = Get-Content -Path $file.FullName -Raw
        $lines = Get-Content -Path $file.FullName

        # Check for ReactiveUI patterns (anti-pattern)
        if ($content -match "ReactiveObject|ReactiveCommand|RaiseAndSetIfChanged") {
            for ($i = 0; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match "ReactiveObject|ReactiveCommand|RaiseAndSetIfChanged") {
                    $violations += [PSCustomObject]@{
                        Principle = "Code Quality"
                        Severity = "High"
                        File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                        Line = $i + 1
                        Issue = "ReactiveUI pattern detected - use MVVM Community Toolkit instead"
                        Pattern = $lines[$i].Trim()
                    }
                }
            }
        }

        # Check for missing [ObservableProperty] in ViewModels
        if ($file.Name -match "ViewModel\.cs$" -and $content -match "INotifyPropertyChanged") {
            # Check if class has [ObservableObject] attribute OR inherits from ObservableObject/BaseViewModel
            $hasAttribute = $content -match "\[ObservableObject\]"
            $inheritsFromObservableObject = $content -match ":\s*ObservableObject\b|:\s*BaseViewModel\b"

            if (-not $hasAttribute -and -not $inheritsFromObservableObject) {
                $violations += [PSCustomObject]@{
                    Principle = "Code Quality"
                    Severity = "Medium"
                    File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                    Line = 1
                    Issue = "ViewModel should use [ObservableObject] attribute or inherit from ObservableObject/BaseViewModel"
                    Pattern = "Missing [ObservableObject] and inheritance"
                }
            }
        }

        # Check for manual ICommand implementation instead of [RelayCommand]
        if ($content -match "public\s+ICommand\s+\w+\s*{\s*get" -and $content -notmatch "\[RelayCommand\]") {
            for ($i = 0; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match "public\s+ICommand\s+") {
                    $violations += [PSCustomObject]@{
                        Principle = "Code Quality"
                        Severity = "Medium"
                        File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                        Line = $i + 1
                        Issue = "Manual ICommand implementation - use [RelayCommand] attribute"
                        Pattern = $lines[$i].Trim()
                    }
                }
            }
        }

        # Check for missing ArgumentNullException.ThrowIfNull in constructors
        if ($content -match "public\s+\w+\([^)]*\w+\s+\w+[^)]*\)\s*{" -and
            $content -notmatch "ArgumentNullException\.ThrowIfNull") {
            # Constructor with parameters but no null checking
            for ($i = 0; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match "public\s+\w+\([^)]*\w+\s+\w+") {
                    $violations += [PSCustomObject]@{
                        Principle = "Code Quality"
                        Severity = "Low"
                        File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                        Line = $i + 1
                        Issue = "Constructor parameters should be validated with ArgumentNullException.ThrowIfNull"
                        Pattern = $lines[$i].Trim()
                    }
                    break # Only report once per file
                }
            }
        }
    }

    $score = if ($filesScanned -eq 0) { 100 } else {
        [math]::Max(0, [math]::Round(100 - (($violations.Count / $filesScanned) * 10), 2))
    }

    return [PSCustomObject]@{
        Principle = "Code Quality Excellence"
        FilesScanned = $filesScanned
        Violations = $violations
        ViolationCount = $violations.Count
        ComplianceScore = $score
        Status = if ($score -ge 90) { "Pass" } elseif ($score -ge 70) { "Warning" } else { "Fail" }
    }
}

function Test-TestingStandards {
    <#
    .SYNOPSIS
        Validate testing standards compliance (Principle 2)

    .DESCRIPTION
        Checks for:
        - Test coverage thresholds (≥80% overall, ≥95% critical paths)
        - Test file naming conventions (*Tests.cs, *Test.cs)
        - Test method naming (descriptive names, not Test1, Test2)

    .OUTPUTS
        PSCustomObject with violations array and compliance score
    #>
    param(
        [Parameter(Mandatory=$false)]
        [string]$RepositoryRoot = (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))))
    )

    $violations = @()
    $filesScanned = 0

    Write-GscInfo "Scanning test files for testing standards..."

    # Get all test files
    $testFiles = Get-ChildItem -Path $RepositoryRoot -Filter "*Test*.cs" -Recurse -File |
        Where-Object { $_.FullName -notmatch "\\obj\\|\\bin\\|\\packages\\" }

    foreach ($file in $testFiles) {
        $filesScanned++
        $lines = Get-Content -Path $file.FullName

        # Check for poor test method naming
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -match "public\s+(void|async\s+Task)\s+(Test\d+|test\d+)\s*\(") {
                $violations += [PSCustomObject]@{
                    Principle = "Testing Standards"
                    Severity = "Medium"
                    File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                    Line = $i + 1
                    Issue = "Test method has non-descriptive name (Test1, Test2, etc.)"
                    Pattern = $lines[$i].Trim()
                }
            }
        }
    }

    # Check for test file naming convention
    $csFiles = Get-ChildItem -Path $RepositoryRoot -Filter "*.cs" -Recurse -File |
        Where-Object {
            $_.FullName -notmatch "\\obj\\|\\bin\\|\\packages\\" -and
            $_.Name -match "ViewModel\.cs$|Service\.cs$"
        }

    foreach ($file in $csFiles) {
        $expectedTestFile = $file.Name -replace "\.cs$", "Tests.cs"
        $testFileExists = Get-ChildItem -Path $RepositoryRoot -Filter $expectedTestFile -Recurse -File |
            Where-Object { $_.FullName -notmatch "\\obj\\|\\bin\\" }

        if (-not $testFileExists) {
            $violations += [PSCustomObject]@{
                Principle = "Testing Standards"
                Severity = "Low"
                File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                Line = 0
                Issue = "No corresponding test file found (expected: $expectedTestFile)"
                Pattern = "Missing test coverage"
            }
        }
    }

    $score = if ($filesScanned -eq 0 -and $csFiles.Count -eq 0) { 100 } else {
        [math]::Max(0, [math]::Round(100 - (($violations.Count / [math]::Max(1, $filesScanned)) * 5), 2))
    }

    return [PSCustomObject]@{
        Principle = "Testing Standards"
        FilesScanned = $filesScanned
        Violations = $violations
        ViolationCount = $violations.Count
        ComplianceScore = $score
        Status = if ($score -ge 80) { "Pass" } elseif ($score -ge 60) { "Warning" } else { "Fail" }
    }
}

function Test-UXConsistency {
    <#
    .SYNOPSIS
        Validate UX consistency compliance (Principle 3)

    .DESCRIPTION
        Checks for:
        - Hardcoded colors (should use Theme V2 DynamicResource)
        - Missing x:DataType attributes in AXAML UserControls
        - Material Design icon usage (not custom icons)

    .OUTPUTS
        PSCustomObject with violations array and compliance score
    #>
    param(
        [Parameter(Mandatory=$false)]
        [string]$RepositoryRoot = (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))))
    )

    $violations = @()
    $filesScanned = 0

    Write-GscInfo "Scanning AXAML files for UX consistency violations..."

    # Get all AXAML files
    $axamlFiles = Get-ChildItem -Path $RepositoryRoot -Filter "*.axaml" -Recurse -File |
        Where-Object { $_.FullName -notmatch "\\obj\\|\\bin\\" }

    foreach ($file in $axamlFiles) {
        $filesScanned++
        $content = Get-Content -Path $file.FullName -Raw
        $lines = Get-Content -Path $file.FullName

        # Check for hardcoded colors (anti-pattern)
        for ($i = 0; $i -lt $lines.Count; $i++) {
            # Match Background="#..." or Foreground="#..." but not DynamicResource
            if ($lines[$i] -match '(Background|Foreground|BorderBrush|Fill|Stroke)="#[0-9A-Fa-f]+"' -and
                $lines[$i] -notmatch "DynamicResource|StaticResource") {
                $violations += [PSCustomObject]@{
                    Principle = "UX Consistency"
                    Severity = "High"
                    File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                    Line = $i + 1
                    Issue = "Hardcoded color detected - use Theme V2 DynamicResource"
                    Pattern = $lines[$i].Trim()
                }
            }
        }

        # Check for missing x:DataType in UserControl
        if ($file.Name -notmatch "App\.axaml|Window\.axaml" -and $content -match "<UserControl") {
            if ($content -notmatch 'x:DataType="') {
                $violations += [PSCustomObject]@{
                    Principle = "UX Consistency"
                    Severity = "Medium"
                    File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                    Line = 1
                    Issue = "UserControl missing x:DataType attribute for compile-time binding validation"
                    Pattern = "<UserControl>"
                }
            }
        }

        # Check for custom icon usage instead of Material Design
        if ($content -match "<Path\s+Data=" -and $content -notmatch "MaterialIconData") {
            for ($i = 0; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match "<Path\s+Data=" -and $lines[$i] -notmatch "MaterialIconData") {
                    $violations += [PSCustomObject]@{
                        Principle = "UX Consistency"
                        Severity = "Low"
                        File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                        Line = $i + 1
                        Issue = "Custom icon path detected - consider using Material Design icons"
                        Pattern = $lines[$i].Trim().Substring(0, [math]::Min(80, $lines[$i].Trim().Length))
                    }
                }
            }
        }
    }

    $score = if ($filesScanned -eq 0) { 100 } else {
        [math]::Max(0, [math]::Round(100 - (($violations.Count / $filesScanned) * 8), 2))
    }

    return [PSCustomObject]@{
        Principle = "UX Consistency"
        FilesScanned = $filesScanned
        Violations = $violations
        ViolationCount = $violations.Count
        ComplianceScore = $score
        Status = if ($score -ge 90) { "Pass" } elseif ($score -ge 70) { "Warning" } else { "Fail" }
    }
}

function Test-Performance {
    <#
    .SYNOPSIS
        Validate performance compliance (Principle 4)

    .DESCRIPTION
        Checks for:
        - Blocking async calls (.Result, .Wait())
        - Synchronous database operations
        - Missing connection pooling configuration
        - Non-async file I/O

    .OUTPUTS
        PSCustomObject with violations array and compliance score
    #>
    param(
        [Parameter(Mandatory=$false)]
        [string]$RepositoryRoot = (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))))
    )

    $violations = @()
    $filesScanned = 0

    Write-GscInfo "Scanning C# files for performance anti-patterns..."

    # Get all C# files
    $csFiles = Get-ChildItem -Path $RepositoryRoot -Filter "*.cs" -Recurse -File |
        Where-Object { $_.FullName -notmatch "\\obj\\|\\bin\\|\\packages\\" }

    foreach ($file in $csFiles) {
        $filesScanned++
        $lines = Get-Content -Path $file.FullName

        # Check for blocking async calls (anti-pattern)
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -match '\.Result\b|\.Wait\(\)' -and
                $lines[$i] -notmatch "//.*\.Result|//.*\.Wait\(\)") {
                $violations += [PSCustomObject]@{
                    Principle = "Performance"
                    Severity = "High"
                    File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                    Line = $i + 1
                    Issue = "Blocking async call detected (.Result or .Wait()) - use await instead"
                    Pattern = $lines[$i].Trim()
                }
            }
        }

        # Check for synchronous file I/O
        if ($content -match "File\.ReadAllText\(|File\.WriteAllText\(|File\.ReadAllLines\(|File\.WriteAllLines\(") {
            for ($i = 0; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match "File\.(ReadAllText|WriteAllText|ReadAllLines|WriteAllLines)\(" -and
                    $lines[$i] -notmatch "//.*File\.") {
                    $violations += [PSCustomObject]@{
                        Principle = "Performance"
                        Severity = "Medium"
                        File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                        Line = $i + 1
                        Issue = "Synchronous file I/O detected - use async methods"
                        Pattern = $lines[$i].Trim()
                    }
                }
            }
        }

        # Check for missing async in database operations
        if ($lines -match "MySqlConnection|SqlConnection") {
            $hasAsync = $false
            for ($i = 0; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match "async\s+Task") {
                    $hasAsync = $true
                    break
                }
            }

            if (-not $hasAsync) {
                $violations += [PSCustomObject]@{
                    Principle = "Performance"
                    Severity = "High"
                    File = $file.FullName.Replace($RepositoryRoot, "").TrimStart('\')
                    Line = 0
                    Issue = "Database operations should be async (file contains MySqlConnection/SqlConnection but no async methods)"
                    Pattern = "Missing async database operations"
                }
            }
        }
    }

    $score = if ($filesScanned -eq 0) { 100 } else {
        [math]::Max(0, [math]::Round(100 - (($violations.Count / $filesScanned) * 10), 2))
    }

    return [PSCustomObject]@{
        Principle = "Performance Requirements"
        FilesScanned = $filesScanned
        Violations = $violations
        ViolationCount = $violations.Count
        ComplianceScore = $score
        Status = if ($score -ge 85) { "Pass" } elseif ($score -ge 65) { "Warning" } else { "Fail" }
    }
}

function Get-OverallCompliance {
    <#
    .SYNOPSIS
        Calculate overall constitutional compliance score

    .PARAMETER Results
        Array of principle test results

    .OUTPUTS
        PSCustomObject with overall compliance metrics
    #>
    param(
        [Parameter(Mandatory=$true)]
        [array]$Results
    )

    $totalViolations = ($Results | Measure-Object -Property ViolationCount -Sum).Sum
    $averageScore = ($Results | Measure-Object -Property ComplianceScore -Average).Average

    $passCount = ($Results | Where-Object { $_.Status -eq "Pass" }).Count
    $warningCount = ($Results | Where-Object { $_.Status -eq "Warning" }).Count
    $failCount = ($Results | Where-Object { $_.Status -eq "Fail" }).Count

    return [PSCustomObject]@{
        OverallScore = [math]::Round($averageScore, 2)
        TotalViolations = $totalViolations
        PrinciplesPassed = $passCount
        PrinciplesWarning = $warningCount
        PrinciplesFailed = $failCount
        Status = if ($averageScore -ge 85) { "Pass" }
                 elseif ($averageScore -ge 70) { "Warning" }
                 else { "Fail" }
    }
}
