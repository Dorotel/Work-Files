---
description: Execute the implementation plan by processing and executing all tasks defined in tasks.md
---

# SpecKit Implement

Execute the implementation plan by processing and executing all tasks defined in tasks.md.

This prompt is compatible with both Visual Studio and VS Code GitHub Copilot Chat.

## Usage

**In VS Code**: Type `/speckit.implement` in Copilot Chat

**In Visual Studio**: Reference this file directly:
```
#file:.github/prompts/speckit.implement.prompt.md
Execute implementation for current feature
```

Or describe the intent:
```
Execute the speckit implementation workflow - load tasks.md, check prerequisites, 
and implement all tasks phase by phase following the .specify methodology
```

---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Outline

1. **Setup and Prerequisites Check**:

   **GSC-Enhanced Approach** (Prefer when GSC available):
   - Use `gsc validate constitution -IncludeTasks` to verify environment and load task information
   - Parse JSON output for FEATURE_DIR, AVAILABLE_DOCS, and task metadata
   
   **Fallback Approach** (Legacy compatibility):
   - If GSC is not available or command fails, run `.specify/scripts/powershell/check-prerequisites.ps1 -Json -RequireTasks -IncludeTasks` from repo root
   - Parse FEATURE_DIR and AVAILABLE_DOCS list
   
   **Common Path Resolution**:
   - All paths must be absolute
   - For single quotes in args like "I'm Groot", use escape syntax: e.g 'I'\''m Groot' (or double-quote if possible: "I'm Groot")
   
   **Phase Transition Support** (if GSC available):
   - After completing a major phase (Setup, Tests, Core, Integration, Polish), consider using: `gsc workflow next`
   - This may automatically update workflow state and suggest next steps
   - Not required - manual progression still fully supported

2. **Check checklists status** (if FEATURE_DIR/checklists/ exists):
   - Scan all checklist files in the checklists/ directory
   - For each checklist, count:
     * Total items: All lines matching `- [ ]` or `- [X]` or `- [x]`
     * Completed items: Lines matching `- [X]` or `- [x]`
     * Incomplete items: Lines matching `- [ ]`
   - Create a status table:
     ```
     | Checklist | Total | Completed | Incomplete | Status |
     |-----------|-------|-----------|------------|--------|
     | ux.md     | 12    | 12        | 0          | ✓ PASS |
     | test.md   | 8     | 5         | 3          | ✗ FAIL |
     | security.md | 6   | 6         | 0          | ✓ PASS |
     ```
   - Calculate overall status:
     * **PASS**: All checklists have 0 incomplete items
     * **FAIL**: One or more checklists have incomplete items
   
   - **If any checklist is incomplete**:
     * Display the table with incomplete item counts
     * **STOP** and ask: "Some checklists are incomplete. Do you want to proceed with implementation anyway? (yes/no)"
     * Wait for user response before continuing
     * If user says "no" or "wait" or "stop", halt execution
     * If user says "yes" or "proceed" or "continue", proceed to step 3
   
   - **If all checklists are complete**:
     * Display the table showing all checklists passed
     * Automatically proceed to step 3

3. Load and analyze the implementation context:
   - **REQUIRED**: Read tasks.md for the complete task list and execution plan
   - **REQUIRED**: Read plan.md for tech stack, architecture, and file structure
   - **IF EXISTS**: Read data-model.md for entities and relationships
   - **IF EXISTS**: Read contracts/ for API specifications and test requirements
   - **IF EXISTS**: Read research.md for technical decisions and constraints
   - **IF EXISTS**: Read quickstart.md for integration scenarios

4. Parse tasks.md structure and extract:
   - **Task phases**: Setup, Tests, Core, Integration, Polish
   - **Task dependencies**: Sequential vs parallel execution rules
   - **Task details**: ID, description, file paths, parallel markers [P]
   - **Execution flow**: Order and dependency requirements

5. Execute implementation following the task plan:
   - **Phase-by-phase execution**: Complete each phase before moving to the next
   - **Respect dependencies**: Run sequential tasks in order, parallel tasks [P] can run together  
   - **Follow TDD approach**: Execute test tasks before their corresponding implementation tasks
   - **File-based coordination**: Tasks affecting the same files must run sequentially
   - **Validation checkpoints**: Verify each phase completion before proceeding

6. Implementation execution rules:
   - **Setup first**: Initialize project structure, dependencies, configuration
   - **Tests before code**: If you need to write tests for contracts, entities, and integration scenarios
   - **Core development**: Implement models, services, CLI commands, endpoints
   - **Integration work**: Database connections, middleware, logging, external services
   - **Polish and validation**: Unit tests, performance optimization, documentation

7. Progress tracking and error handling:
   - Report progress after each completed task
   - Halt execution if any non-parallel task fails
   - For parallel tasks [P], continue with successful tasks, report failed ones
   - Provide clear error messages with context for debugging
   - Suggest next steps if implementation cannot proceed
   - **IMPORTANT**: For completed tasks, make sure to mark the task off as [X] in the tasks file
   
   **Checkpoint Creation per Task** (if GSC available):
   - After successfully completing each task, create a checkpoint: `gsc rollback checkpoint "task-<ID>-complete"`
   - Example: After completing T001, create checkpoint: `gsc rollback checkpoint "task-T001-complete"`
   - This enables granular rollback if a specific task needs revision
   - Checkpoint creation failures should be logged but not block implementation progress

8. Completion validation:
   - Verify all required tasks are completed
   - Check that implemented features match the original specification
   - Validate that tests pass and coverage meets requirements
   - Confirm the implementation follows the technical plan
   - Report final status with summary of completed work
   
   **Optional Cleanup Preview** (if GSC available):
   - After completion, consider running: `gsc housekeeping prune --DryRun`
   - This shows what documentation could be cleaned up without making changes
   - Helps identify obsolete or redundant documentation from implementation process
   - Use without `--DryRun` to actually perform cleanup if desired

---

## Visual Studio Integration Notes

When using this prompt in Visual Studio:

1. **Reference the file directly** using `#file:` syntax for better context
2. **Use `run_command_in_terminal` tool** for PowerShell commands
3. **Use standard file manipulation tools** (edit_file, create_file, get_file)
4. **Progress tracking** happens through task completion markers in tasks.md

## VS Code Integration Notes

When using this prompt in VS Code with the full agent mode:

1. **Type `/speckit.implement`** to trigger the workflow
2. **Agent tools available**: edit, search, new, runCommands, runTasks
3. **Automatic tool selection** based on task requirements

---

## Notes

This command assumes a complete task breakdown exists in tasks.md. If tasks are incomplete or missing, suggest running the tasks generation workflow first to regenerate the task list.

**Constitutional Compliance**: This workflow follows all four constitutional principles:
- **Principle I**: Code quality through MVVM patterns, DI, and error handling
- **Principle II**: Testing standards with 80%+ coverage targets
- **Principle III**: UX consistency through zero-regression validation
- **Principle IV**: Performance requirements maintained throughout
