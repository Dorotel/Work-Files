# Instruction Files Organization

**Last Updated**: 2025-10-13

This directory contains GitHub Copilot instruction files organized by category and application structure.

## 📂 Directory Structure

### 🎯 Framework/ (Core Language & UI)
Core patterns for C#, .NET 8, Avalonia UI, and MVVM Community Toolkit.

**Files**:
- `csharp-dotnet8.instructions.md` - C# language patterns, nullable reference types, async/await
- `avalonia-ui.instructions.md` - Avalonia UI 11.3+ patterns, AXAML, CompiledBinding
- `mvvm-community-toolkit.instructions.md` - ObservableObject, RelayCommand, source generators

**ApplyTo**: All `**/*.cs` and `**/*.axaml` files

---

### 🧪 Testing/ (Test Patterns)
Testing standards, async patterns, and platform-specific test approaches.

**Files**:
- `testing-standards.instructions.md` - Manual validation, success criteria, cross-platform testing
- `async-testing-patterns.instructions.md` - xUnit async patterns, xUnit1031 resolution
- `platform-specific-testing.instructions.md` - Platform-specific Fact attributes

**ApplyTo**: All `**/tests/**/*.cs` files

---

### ✅ Quality/ (Code Quality & Standards)
Code review standards, performance optimization, and security best practices.

**Files**:
- `code-review-standards.instructions.md` - Review checklists, approval criteria
- `performance-optimization.instructions.md` - Performance targets, optimization patterns
- `security-best-practices.instructions.md` - SQL injection prevention, credential security

**ApplyTo**: All `**/*.cs` files

---

### 📝 Development/ (Documentation & Process)
Documentation standards and markdown patterns.

**Files**:
- `documentation.instructions.md` - XML comments, code comments, documentation standards
- `markdown.instructions.md` - Markdown formatting for documentation

**ApplyTo**: All `**/*.md` and `**/*.cs` files (for XML docs)

---

### 🤖 Joyride/ (ClojureScript Automation)
Joyride user scripts and workspace automation (separate ecosystem from main app).

**Files**:
- `joyride-user-project.instructions.md` - User space VS Code automation
- `joyride-workspace-automation.instructions.md` - Workspace-specific automation

**ApplyTo**: `.joyride/**/*.*`

---

### 🏗️ Application Structure (Mirrors Codebase)

The following directories mirror the `MTM_Template_Application/` codebase structure:

- **Services/** - All service layer patterns (Boot, Configuration, Secrets, Visual, etc.)
- **ViewModels/** - MVVM ViewModel patterns and specific ViewModel implementations
- **Views/** - Avalonia view patterns, AXAML structure, Theme V2 integration
- **Models/** - Domain model patterns, DTOs, validation
- **Controls/** - Custom control development patterns
- **Converters/** - Value converter patterns
- **Behaviors/** - Avalonia behavior patterns
- **Extensions/** - Dependency injection and extension method patterns

Each subfolder contains instruction files specific to that component category.

---

## 🎯 Usage by GitHub Copilot

GitHub Copilot automatically references these instruction files based on:
1. **File path matching**: When editing a file, Copilot loads instructions matching `applyTo` patterns
2. **Explicit references**: Use `#file:instructions/path/to/file.instructions.md` in prompts
3. **Category applicability**: Framework, Testing, and Quality instructions apply broadly across the codebase

---

## 📊 Documentation Coverage

Run the documentation scanner to check coverage:

\\\powershell
pwsh .copilot-scripts/find-missing-documentation.ps1
\\\

Current coverage tracked in `.specify/checklists/documentation-coverage-checklist.md`.

---

## ✅ Maintenance

- **Update frequency**: Instruction files should be updated when new patterns emerge
- **Version control**: Commit instruction updates alongside code changes
- **Quality checks**: Ensure all instruction files have proper frontmatter (description, applyTo, priority)

---

## 🔗 Related Documentation

- **Copilot Instructions**: `.github/copilot-instructions.md` - Master instruction aggregation
- **AGENTS.md**: `AGENTS.md` - AI agent context and guidance
- **Documentation Checklist**: `.specify/checklists/documentation-coverage-checklist.md`
