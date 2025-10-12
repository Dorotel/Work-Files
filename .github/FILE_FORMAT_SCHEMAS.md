# File Format Schemas for GitHub Copilot Configuration

This document defines the YAML frontmatter schemas and file structure patterns for all configuration files in the `.github/` directory.

## Instruction Files

**Purpose**: Define coding standards, patterns, and principles that GitHub Copilot follows automatically

**Location**: `.github/instructions/*.instructions.md`

### Frontmatter Schema

```yaml
---
description: 'Brief description of what this instruction file covers (required)'
applyTo: '**/*.{ext}' # File pattern for when these instructions apply (required)
---
```

**Fields**:

- `description`: Single-line string describing the instruction scope
- `applyTo`: Glob pattern for file matching (e.g., `**/*.cs` for all C# files, `**/*.axaml` for all AXAML files)

### File Structure

```markdown
---
description: 'Instruction file description'
applyTo: '**/*.ext'
---

<!-- Based on patterns from: https://github.com/github/awesome-copilot -->

# Title of Instruction Set

## Overview
Brief introduction to the instruction scope

## Core Principles
High-level principles (not code examples)

## Naming Conventions
Pattern guidelines

## Project Structure
Organization patterns

## Error Handling
Exception management patterns

## Testing Approach
Test strategy guidelines

## Performance Considerations
Optimization principles
```

### Example: C# Instructions

```yaml
---
description: 'C# and .NET 8 development guidelines'
applyTo: '**/*.cs'
---
```

---

## Prompt Files

**Purpose**: Reusable command templates that scaffold code or perform specific tasks

**Location**: `.github/prompts/*.prompt.md`

### Frontmatter Schema

```yaml
---
mode: 'agent' # or 'chat' (required)
tools: ['codebase', 'editFiles', 'search'] # Array of tool names (required)
description: 'What this prompt does' (required)
---
```

**Fields**:

- `mode`: Execution mode
  - `agent`: Multi-step autonomous workflows (file creation, complex scaffolding)
  - `chat`: Interactive assistance (guidance, suggestions, explanations)
- `tools`: Array of tool names Copilot can use
  - `codebase`: Read and analyze code
  - `editFiles`: Create/modify files
  - `search`: Search codebase
  - `fetch`: Retrieve external resources
  - `run`: Execute commands
- `description`: Single-line string describing the prompt purpose

### File Structure

```markdown
---
mode: 'agent'
tools: ['codebase', 'editFiles', 'search']
description: 'Brief description of prompt action'
---

<!-- Based on patterns from: https://github.com/github/awesome-copilot -->

# Prompt Title

## Goal
Clear statement of what this prompt accomplishes

## Prerequisites
Required context or setup before execution

## Execution Steps
1. Step-by-step instructions
2. With specific actions
3. And validation criteria

## Validation
How to verify success

## Common Pitfalls
Known issues and solutions
```

### Example: Setup ViewModel Prompt

```yaml
---
mode: 'agent'
tools: ['codebase', 'editFiles', 'search']
description: 'Generate new ViewModel with MVVM Community Toolkit patterns'
---
```

---

## Chatmode Files

**Purpose**: Specialized personas that provide domain-specific or context-aware assistance

**Location**: `.github/chatmodes/*.chatmode.md`

### Frontmatter Schema

```yaml
---
description: 'Persona and specialization description' (required)
tools: ['codebase', 'search', 'fetch'] # Array of tool names (required)
---
```

**Fields**:

- `description`: Single-line string describing the chatmode persona and capabilities
- `tools`: Array of tool names the persona can use (same as prompt tools)

### File Structure

```markdown
---
description: 'Specialized persona description'
tools: ['codebase', 'search', 'fetch']
---

<!-- Based on patterns from: https://github.com/github/awesome-copilot -->

# Chatmode Title

You are [persona description with expertise and focus areas].

## Capabilities
- What this chatmode excels at
- Specific domain knowledge
- Interaction patterns

## Domain Knowledge
- Key concepts and terminology
- Technology stack familiarity
- Project-specific context

## Interaction Style
- How to engage with users
- Question-asking patterns
- Guidance approach

## References
- Instruction files to consult
- Memory files to reference
- External documentation links
```

### Example: MTM Architect Chatmode

```yaml
---
description: 'MTM architecture planning and service design expert'
tools: ['codebase', 'search', 'fetch']
---
```

---

## Memory Files

**Purpose**: Capture lessons learned and persistent knowledge from development experience

**Location**: `.github/memory/*.md`

### Frontmatter Schema

Memory files do NOT require frontmatter. They are referenced directly in `copilot-instructions.md`.

### File Structure

```markdown
# Memory Category Title

Brief introduction to the memory domain

## Pattern/Lesson Title

### Context
When and why this pattern/lesson was discovered

### Problem
What issue was encountered

### Solution
How it was resolved

### Lesson Learned
Key takeaway for future reference

### Related
- Links to instruction files
- Links to other memory entries
- External documentation
```

### Example: Avalonia UI Patterns Memory

```markdown
# Avalonia UI Patterns Memory

Lessons learned from Avalonia 11.3+ development

## Container Height Constraints Issue

### Context
Creating expandable Notes field in manufacturing form grid

### Problem
Fixed height constraints from base styles prevented expansion

[... continued]
```

---

## High-Level Principles Pattern (from awesome-copilot)

### Principle: Focus on "What" and "Why", Not "How"

Instruction files should provide:

- ✅ **High-level principles**: "Use async/await for all database operations"
- ✅ **Pattern guidelines**: "Follow repository pattern for data access"
- ✅ **Architectural guidance**: "Separate ViewModels from Views"
- ❌ **NOT code examples**: Avoid showing exact implementation code
- ❌ **NOT step-by-step code**: Let Copilot generate the implementation

### Rationale

- Copilot generates better code when given principles vs. examples
- Code examples can become outdated or conflict with project evolution
- Principles adapt to different contexts and requirements
- Examples constrain Copilot's creativity and problem-solving

### Exception: When Code Examples ARE Appropriate

- Configuration patterns (JSON/YAML structure that rarely changes)
- Command-line syntax (precise terminal commands)
- File naming conventions (exact patterns like `*.instructions.md`)

---

## File Naming Conventions

- **Instruction files**: `{topic}.instructions.md` (e.g., `csharp-dotnet8.instructions.md`)
- **Prompt files**: `{action}-{target}.prompt.md` (e.g., `setup-viewmodel.prompt.md`)
- **Chatmode files**: `{persona}-{focus}.chatmode.md` (e.g., `mtm-architect.chatmode.md`)
- **Memory files**: `{domain}-patterns.md` (e.g., `avalonia-ui-patterns.md`)

---

## Attribution Requirements

All files that adapt patterns from awesome-copilot must include attribution:

```markdown
<!-- Based on patterns from: https://github.com/github/awesome-copilot -->
```

Or for specific file adaptations:

```markdown
<!-- Based on: https://github.com/github/awesome-copilot/blob/main/instructions/csharp.instructions.md -->
```

---

## Validation Checklist

Before committing a configuration file, verify:

- [ ] YAML frontmatter is valid and includes all required fields
- [ ] Attribution comment is present (if using awesome-copilot patterns)
- [ ] File follows the structural pattern for its type
- [ ] Description is clear and single-line
- [ ] Glob patterns in `applyTo` are correct
- [ ] Tool names in `tools` array are valid
- [ ] Content focuses on principles, not implementation details
- [ ] Markdown formatting is consistent
