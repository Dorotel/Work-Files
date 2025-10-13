# Joyride Scripts TODO

> **🤖 AI Agent Instructions**: When this file is referenced in chat without additional context, the agent should:
> 1. Select the next logical TODO item (prioritize by 🔴 High → 🟡 Medium → 🟢 Low)
> 2. Implement the selected TODO completely
> 3. Update this file by checking off completed items and moving them to "Recently Completed"
> 4. Update the "Last Updated" timestamp

Comprehensive tracking of Joyride ClojureScript automation scripts, enhancements, and workflow improvements.

**Last Updated**: October 12, 2025  
**Language**: ClojureScript (SCI - Small Clojure Interpreter)  
**Runtime**: VS Code Extension Host via Joyride extension  
**Purpose**: REPL-driven VS Code automation and workflow optimization

---

## 🔴 High Priority - Script Improvements

### 1. Script Documentation Gaps
**Impact**: Scripts are hard to understand and use without documentation

**Current State**: README.md and QUICKSTART.md exist but may be outdated

**Action Items:**
- [ ] Audit README.md for accuracy and completeness
- [ ] Update QUICKSTART.md with current script examples
- [ ] Add docstrings to all public functions in all scripts
- [ ] Create usage examples for each script
- [ ] Add troubleshooting section to README
- [ ] Document script dependencies and prerequisites
- [ ] Add expected input/output examples

**Scripts Needing Documentation:**
- [ ] `batch_processor.cljs` - What batches are processed? How?
- [ ] `dependency_analyzer.cljs` - What dependencies? Output format?
- [ ] `dependency_batch_processor.cljs` - Use cases?
- [ ] `dependency_progress_tracker.cljs` - Integration with other scripts?
- [ ] `dependency_spec_generator.cljs` - Spec format? Output location?
- [ ] `model_switcher.cljs` - What models? When to switch?
- [ ] `progress_tracker.cljs` - Progress of what? UI integration?
- [ ] `reverse_dependency_workflow.cljs` - Workflow steps?
- [ ] `reverse_spec_workflow.cljs` - Reverse engineering specs?
- [ ] `spec_generator.cljs` - Input sources? Templates?
- [ ] `view_analyzer.cljs` - What does it analyze? Output format?

**Estimated Effort**: 4-6 hours

---

### 2. Error Handling Improvements
**Impact**: Scripts may fail silently or with cryptic errors

**Action Items:**
- [ ] Add try-catch blocks to all main entry points
- [ ] Implement user-friendly error messages
- [ ] Add error logging to file for debugging
- [ ] Create error notification mechanism (VS Code notifications)
- [ ] Add validation for input parameters
- [ ] Add pre-flight checks (e.g., file existence, workspace open)
- [ ] Document common errors and solutions

**Estimated Effort**: 3-4 hours

---

### 3. Script Integration and Workflow Automation
**Impact**: Scripts run in isolation, could be chained for workflows

**Potential Workflows:**
- [ ] **View Analysis → Dependency Detection → Spec Generation** workflow
- [ ] **Model Discovery → Batch Processing → Progress Tracking** workflow
- [ ] **Reverse Engineering** workflow (View → Dependencies → Specs)
- [ ] **Specification Automation** workflow (Templates → Generation → Validation)

**Action Items:**
- [ ] Create workflow orchestration script (`workflows.cljs`)
- [ ] Add workflow definitions in EDN format
- [ ] Implement workflow state management
- [ ] Add workflow progress UI (webview or status bar)
- [ ] Create common workflow templates
- [ ] Add workflow error recovery and rollback

**Estimated Effort**: 8-10 hours

---

## 🟡 Medium Priority - Feature Enhancements

### 4. Progress Tracking Enhancements
**Files**: `progress_tracker.cljs`, `dependency_progress_tracker.cljs`

**Current Limitations:**
- Progress data format unclear
- No persistence across VS Code sessions
- No visualization beyond console output

**Action Items:**
- [ ] Define progress data schema (EDN or JSON)
- [ ] Implement progress persistence to workspace `.joyride/state/`
- [ ] Create progress visualization (status bar item with percentage)
- [ ] Add progress history tracking
- [ ] Create progress dashboard (webview with charts)
- [ ] Add progress export functionality (CSV, JSON)
- [ ] Add progress notifications at milestones (25%, 50%, 75%, 100%)

**Estimated Effort**: 6-8 hours

---

### 5. Dependency Analysis Improvements
**Files**: `dependency_analyzer.cljs`, `dependency_batch_processor.cljs`, `reverse_dependency_workflow.cljs`

**Enhancement Opportunities:**
- [ ] Add dependency graph visualization (webview with d3.js or similar)
- [ ] Add circular dependency detection
- [ ] Add unused dependency detection
- [ ] Add dependency version conflict detection
- [ ] Export dependency analysis to Markdown report
- [ ] Integrate with VS Code dependency lens
- [ ] Add dependency update suggestions

**Estimated Effort**: 8-10 hours

---

### 6. Spec Generator Enhancements
**Files**: `spec_generator.cljs`, `dependency_spec_generator.cljs`, `reverse_spec_workflow.cljs`

**Current Gaps:**
- Template system unclear
- Output format inconsistent?
- No validation of generated specs

**Action Items:**
- [ ] Document spec template format
- [ ] Create template library (common spec patterns)
- [ ] Add spec validation before writing to file
- [ ] Add spec preview before generation
- [ ] Implement spec diff when updating existing specs
- [ ] Add custom template support (user-defined templates)
- [ ] Create spec generation wizard (interactive prompts)

**Estimated Effort**: 6-8 hours

---

### 7. View Analyzer Enhancements
**Files**: `view_analyzer.cljs`

**Potential Enhancements:**
- [ ] Analyze AXAML binding expressions
- [ ] Detect unused ViewModel properties
- [ ] Detect missing ViewModel properties (bindings with no backing property)
- [ ] Analyze control naming consistency
- [ ] Detect accessibility issues (missing AutomationProperties)
- [ ] Generate view analysis report (Markdown)
- [ ] Add quick-fix suggestions for common issues

**Estimated Effort**: 6-8 hours

---

### 8. Model Switcher Improvements
**Files**: `model_switcher.cljs`

**Current Unknowns:**
- What models are being switched?
- Use cases unclear

**Investigation Needed:**
- [ ] Document model switching use cases
- [ ] Add model discovery (scan project for models)
- [ ] Add model comparison (show differences between models)
- [ ] Create model migration tool (transform data between model versions)
- [ ] Add model validation (ensure model integrity after switch)
- [ ] Add rollback functionality (revert to previous model)

**Estimated Effort**: 4-6 hours (after investigation)

---

## 🟢 Low Priority - New Script Ideas

### 9. TODO Extraction and Management Script
**Purpose**: Extract TODO comments from codebase and manage them

**Features:**
- [ ] Scan all source files for TODO, FIXME, HACK, NOTE comments
- [ ] Extract TODO text, file path, line number, author (from git blame)
- [ ] Categorize TODOs by priority (infer from keywords)
- [ ] Generate TODO.md file automatically
- [ ] Create TODO dashboard (webview)
- [ ] Add TODO completion tracking
- [ ] Integrate with GitHub Issues (create issues from TODOs)

**Estimated Effort**: 8-10 hours

---

### 10. Code Metrics and Complexity Analysis Script
**Purpose**: Analyze code quality and identify refactoring candidates

**Features:**
- [ ] Calculate cyclomatic complexity per method
- [ ] Identify long methods (>50 lines)
- [ ] Identify large classes (>500 lines)
- [ ] Detect code duplication
- [ ] Calculate test coverage gaps
- [ ] Generate code quality report
- [ ] Create refactoring suggestions

**Estimated Effort**: 10-12 hours

---

### 11. Documentation Generator Script
**Purpose**: Generate documentation from code

**Features:**
- [ ] Extract XML documentation from C# files
- [ ] Generate API reference documentation
- [ ] Create class hierarchy diagrams
- [ ] Generate sequence diagrams from code flow
- [ ] Create markdown documentation from code comments
- [ ] Add cross-references between related code elements

**Estimated Effort**: 12-16 hours

---

### 12. Test Generator Script
**Purpose**: Generate test scaffolding from source code

**Features:**
- [ ] Analyze C# class and generate test class skeleton
- [ ] Generate test methods for all public methods
- [ ] Add test data builders for complex types
- [ ] Generate mock setup for dependencies
- [ ] Add FluentAssertions assertion templates
- [ ] Support xUnit, NUnit, MSTest patterns

**Estimated Effort**: 10-12 hours

---

### 13. Git Workflow Automation Script
**Purpose**: Automate common git workflows

**Features:**
- [ ] Create feature branch with naming convention
- [ ] Generate commit message from staged changes
- [ ] Automate PR creation with template
- [ ] Generate changelog from git commits
- [ ] Automate version bumping
- [ ] Create release notes from merged PRs

**Estimated Effort**: 8-10 hours

---

### 14. Configuration Validator Script
**Purpose**: Validate appsettings.json and configuration files

**Features:**
- [ ] Validate JSON schema
- [ ] Check for missing required settings
- [ ] Validate connection strings
- [ ] Check for sensitive data in config files
- [ ] Compare config across environments
- [ ] Generate config documentation

**Estimated Effort**: 6-8 hours

---

## 🔧 Technical Debt

### 15. Code Quality Improvements

**ClojureScript Best Practices:**
- [ ] Add type hints for better performance
- [ ] Use spec for function contracts
- [ ] Implement proper namespacing
- [ ] Add docstrings to all public functions
- [ ] Use threading macros (-> ->>) for readability
- [ ] Implement error handling with ex-info

**Estimated Effort**: 4-6 hours

---

### 16. Testing Infrastructure

**Current Issue**: No tests for Joyride scripts

**Action Items:**
- [ ] Set up testing framework for ClojureScript (cljs.test)
- [ ] Write unit tests for pure functions
- [ ] Create integration tests for VS Code API calls
- [ ] Add test fixtures for common scenarios
- [ ] Set up continuous testing (watch mode)
- [ ] Add test coverage reporting

**Estimated Effort**: 8-10 hours

---

### 17. Script Organization

**Current Structure**: Flat directory with 11 scripts

**Improvement Opportunities:**
- [ ] Group scripts by category (analysis/, generation/, workflow/)
- [ ] Extract common utilities to `utils/` namespace
- [ ] Create `core/` namespace for shared functionality
- [ ] Implement plugin architecture for extensibility
- [ ] Create script registry for discovery

**Estimated Effort**: 4-6 hours

---

## 📊 Metrics & Goals

### Current Script Portfolio
- **Total Scripts**: 11
- **Documented Scripts**: 2 (README, QUICKSTART)
- **Scripts with Examples**: 0 (needs audit)
- **Scripts with Tests**: 0
- **Scripts with Error Handling**: Unknown (needs audit)

### Quality Goals
- [ ] 100% of scripts documented with usage examples
- [ ] 100% of scripts with error handling
- [ ] 80%+ test coverage for pure functions
- [ ] All scripts follow consistent naming conventions
- [ ] All scripts have performance benchmarks

---

## 🚀 Integration Opportunities

### VS Code Extension Integration
**Potential**:
- [ ] Register scripts as VS Code commands (Command Palette)
- [ ] Add keybindings for frequently used scripts
- [ ] Create status bar items for script execution
- [ ] Add context menu items (right-click actions)
- [ ] Create webview panels for interactive scripts

**Estimated Effort**: 6-8 hours

---

### GitHub Copilot Integration
**Potential**:
- [ ] Create Copilot prompts that invoke Joyride scripts
- [ ] Add script suggestions based on context
- [ ] Integrate script outputs into Copilot responses
- [ ] Create feedback loop (Copilot → Joyride → Copilot)

**Estimated Effort**: 6-8 hours

---

### Project-Specific Integration
**Potential**:
- [ ] Integrate with MTM project structure
- [ ] Add MTM-specific analysis scripts
- [ ] Create MTM workflow automation
- [ ] Add MTM code generation scripts

**Estimated Effort**: 10-12 hours

---

## 📚 Learning Resources

### Recommended Reading
- [ ] Joyride documentation: https://github.com/BetterThanTomorrow/joyride
- [ ] ClojureScript documentation: https://clojurescript.org/
- [ ] SCI (Small Clojure Interpreter): https://github.com/babashka/sci
- [ ] VS Code API documentation: https://code.visualstudio.com/api

### Example Projects
- [ ] Study Joyride examples repository
- [ ] Review Calva extension scripts (uses Joyride patterns)
- [ ] Explore community Joyride scripts

---

## ✅ Recently Completed (October 12, 2025)

- [x] Created comprehensive TODO tracking system
- [x] Identified 11 existing scripts
- [x] Documented enhancement opportunities
- [x] Established priority levels for improvements

---

## 📞 Quick Reference

### Running Scripts

```clojure
;; From Joyride REPL
(load-file ".github/joyride/scripts/view_analyzer.cljs")

;; From VS Code Command Palette
;; Joyride: Run Workspace Script → select script

;; From keybinding (if configured)
;; Ctrl+Alt+J (or custom binding)
```

### Script Template

```clojure
(ns my-script
  (:require ["vscode" :as vscode]
            [promesa.core :as p]))

(defn main
  "Entry point for script execution."
  []
  (p/let [result (do-something)]
    (vscode/window.showInformationMessage (str "Result: " result))))

;; Only run when invoked as script (not when loaded in REPL)
(when (= (joyride.core/invoked-script) joyride.core/*file*)
  (main))
```

### Debugging Scripts

```clojure
;; Add logging
(js/console.log "Debug message" data)

;; Use REPL for interactive development
;; Load script in REPL and test functions individually

;; Check Joyride output panel
;; View → Output → Select "Joyride" from dropdown
```

---

**Last Review**: October 12, 2025  
**Next Review**: Quarterly or when adding new scripts  
**Maintained By**: Development Team (ClojureScript enthusiasts)
