<#
.SYNOPSIS
    GSC Memory Command - Access constitutional memory system

.DESCRIPTION
    Provides read/write access to .specify/memory/ constitutional guidance files

.PARAMETER Operation
    Operation to perform: get, post, search, list

.PARAMETER Target
    Target memory file (without .md extension) for get/post operations

.PARAMETER Content
    Content to append (for post operation)

.EXAMPLE
    gsc memory list
    gsc memory get constitution
    gsc memory search "MVVM"
    gsc memory post custom-memory "New guidance entry"

.NOTES
    Version: 1.0.0
#>

param(
    [Parameter(Mandatory=$false, Position=0)]
    [ValidateSet('get', 'post', 'search', 'list', '')]
    [string]$Operation = 'list',

    [Parameter(Mandatory=$false, Position=1)]
    [string]$Target = "",

    [Parameter(Mandatory=$false, Position=2)]
    [string]$Content = ""
)

# Import common utilities
$commonPath = Join-Path $PSScriptRoot "common"
. (Join-Path $commonPath "output.ps1")
. (Join-Path $commonPath "state.ps1")

# Get memory directory
$specifyRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$memoryDir = Join-Path $specifyRoot "memory"

function Get-Memory {
    param([string]$FileName)

    $filePath = Join-Path $memoryDir "$FileName.md"

    if (-not (Test-Path $filePath)) {
        Write-GscError "Memory file not found: $FileName"
        Write-Host ""
        Write-Host "Available memory files:" -ForegroundColor Yellow
        Get-MemoryList
        return
    }

    Write-GscHeader -Title "Memory: $FileName" -SubTitle "Constitutional Guidance"

    $content = Get-Content -Path $filePath -Raw -Encoding UTF8
    Write-Host $content

    Write-Host ""
    Write-GscInfo "File location: $filePath"
}

function Set-Memory {
    param(
        [string]$FileName,
        [string]$Content
    )

    if (-not $Content) {
        Write-GscError "Content is required for post operation"
        Write-Host ""
        Write-Host "Usage: gsc memory post <filename> <content>" -ForegroundColor Yellow
        return
    }

    $filePath = Join-Path $memoryDir "$FileName.md"

    # Create file if it doesn't exist
    if (-not (Test-Path $filePath)) {
        Write-GscWarning "Creating new memory file: $FileName"
        "# $FileName`n`n" | Out-File -FilePath $filePath -Encoding UTF8
    }

    # Append content with timestamp
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $entry = "`n---`n`n**[$timestamp]**`n`n$Content`n"

    $entry | Out-File -FilePath $filePath -Append -Encoding UTF8

    Write-GscSuccess "Content added to memory: $FileName"
    Write-GscInfo "File location: $filePath"
}

function Search-Memory {
    param([string]$SearchTerm)

    if (-not $SearchTerm) {
        Write-GscError "Search term is required"
        Write-Host ""
        Write-Host "Usage: gsc memory search <term>" -ForegroundColor Yellow
        return
    }

    Write-GscHeader -Title "Memory Search Results" -SubTitle "Searching for: '$SearchTerm'"

    $memoryFiles = Get-ChildItem -Path $memoryDir -Filter "*.md" -File
    $resultsFound = $false

    foreach ($file in $memoryFiles) {
        $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8

        # Case-insensitive search
        if ($content -match "(?i)$SearchTerm") {
            $resultsFound = $true
            $fileName = $file.BaseName

            Write-Host ""
            Write-Host "📄 $fileName" -ForegroundColor Cyan
            Write-Host "   $($file.FullName)" -ForegroundColor Gray
            Write-Host ""

            # Extract matching lines with context
            $lines = $content -split "`n"
            for ($i = 0; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match "(?i)$SearchTerm") {
                    # Show line with context (2 lines before and after)
                    $startLine = [Math]::Max(0, $i - 2)
                    $endLine = [Math]::Min($lines.Count - 1, $i + 2)

                    Write-Host "   Line $($i + 1):" -ForegroundColor Yellow
                    for ($j = $startLine; $j -le $endLine; $j++) {
                        if ($j -eq $i) {
                            Write-Host "   → " -ForegroundColor Green -NoNewline
                            Write-Host $lines[$j] -ForegroundColor White
                        } else {
                            Write-Host "     " -NoNewline
                            Write-Host $lines[$j] -ForegroundColor Gray
                        }
                    }
                    Write-Host ""
                }
            }
        }
    }

    if (-not $resultsFound) {
        Write-GscWarning "No results found for: '$SearchTerm'"
    }

    Write-Host ""
}

function Get-MemoryList {
    $memoryFiles = Get-ChildItem -Path $memoryDir -Filter "*.md" -File | Sort-Object Name

    if ($memoryFiles.Count -eq 0) {
        Write-GscWarning "No memory files found in: $memoryDir"
        return
    }

    Write-GscHeader -Title "Available Memory Files" -SubTitle "$($memoryFiles.Count) constitutional guidance documents"

    foreach ($file in $memoryFiles) {
        $fileName = $file.BaseName
        $fileSize = [math]::Round($file.Length / 1KB, 2)
        $lastModified = $file.LastWriteTime.ToString("yyyy-MM-dd HH:mm")

        Write-Host "  📄 " -ForegroundColor Cyan -NoNewline
        Write-Host $fileName.PadRight(30) -ForegroundColor White -NoNewline
        Write-Host " │ " -ForegroundColor DarkGray -NoNewline
        Write-Host "$fileSize KB".PadRight(12) -ForegroundColor Gray -NoNewline
        Write-Host " │ " -ForegroundColor DarkGray -NoNewline
        Write-Host $lastModified -ForegroundColor Gray
    }

    Write-Host ""
    Write-GscInfo "Use 'gsc memory get <name>' to view content"
    Write-GscInfo "Use 'gsc memory search <term>' to search all files"
}

# Main execution
switch ($Operation) {
    'get' {
        if (-not $Target) {
            Write-GscError "Target memory file is required"
            Write-Host ""
            Write-Host "Usage: gsc memory get <filename>" -ForegroundColor Yellow
            Write-Host ""
            Get-MemoryList
        } else {
            Get-Memory -FileName $Target
        }
    }
    'post' {
        if (-not $Target) {
            Write-GscError "Target memory file is required"
            Write-Host ""
            Write-Host "Usage: gsc memory post <filename> <content>" -ForegroundColor Yellow
        } else {
            Set-Memory -FileName $Target -Content $Content
        }
    }
    'search' {
        if (-not $Target) {
            Write-GscError "Search term is required"
            Write-Host ""
            Write-Host "Usage: gsc memory search <term>" -ForegroundColor Yellow
        } else {
            Search-Memory -SearchTerm $Target
        }
    }
    'list' {
        Get-MemoryList
    }
    default {
        Get-MemoryList
    }
}
