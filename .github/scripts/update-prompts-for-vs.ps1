# Update all prompt files to be compatible with Visual Studio
# Removes VS Code-specific frontmatter (mode and tools)

$promptsDir = Join-Path $PSScriptRoot "..\prompts"
$files = @(
    'ai-prompt-engineering-safety-review.prompt.md',
    'create-stored-procedure.prompt.md',
    'database-operation.prompt.md',
    'debug-issue.prompt.md',
    'editorconfig.prompt.md',
    'generate-docs.prompt.md',
    'github-copilot-starter.prompt.md',
    'prompt-builder.prompt.md',
    'refactor-code.prompt.md',
    'reimagine.prompt.md',
    'setup-custom-control.prompt.md',
    'setup-service.prompt.md',
    'setup-view.prompt.md',
    'setup-viewmodel.prompt.md',
    'write-tests.prompt.md'
)

foreach ($file in $files) {
    $filePath = Join-Path $promptsDir $file
    
    if (Test-Path $filePath) {
        Write-Host "Processing: $file" -ForegroundColor Cyan
        
        $content = Get-Content $filePath -Raw
        
        # Remove mode and tools lines from frontmatter, keep only description
        $pattern = "(?ms)^---\s*\r?\nmode:.*?\r?\ntools:.*?\]\s*\r?\ndescription:"
        $replacement = "---`r`ndescription:"
        
        $newContent = $content -replace $pattern, $replacement
        
        if ($newContent -ne $content) {
            Set-Content -Path $filePath -Value $newContent -NoNewline
            Write-Host "✓ Updated: $file" -ForegroundColor Green
        } else {
            Write-Host "⊘ No changes needed: $file" -ForegroundColor Yellow
        }
    } else {
        Write-Host "✗ Not found: $file" -ForegroundColor Red
    }
}

Write-Host "`n✓ All prompt files updated for Visual Studio compatibility" -ForegroundColor Green
