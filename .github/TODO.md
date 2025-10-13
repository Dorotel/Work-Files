# .github Directory TODO

> **🤖 AI Agent Instructions**: When this file is referenced in chat without additional context, the agent should:
> 1. Select the next logical TODO item (prioritize by 🔴 High → 🟡 Medium → 🟢 Low)
> 2. Implement the selected TODO completely
> 3. Update this file by checking off completed items and moving them to "Recently Completed"
> 4. Update the "Last Updated" timestamp

Comprehensive tracking of GitHub Copilot configuration, prompts, chatmodes, instructions, and workflow automation improvements.

**Last Updated**: October 12, 2025  
**Repository**: MTM_Avalonia_Template  
**Purpose**: AI-assisted development infrastructure

---

## 🔴 High Priority - Critical Gaps

### 1. Missing Critical Instructions Files
**Impact**: AI agents lack guidance for key development patterns

**Missing Instructions:**
- [ ] `database-ui-integration-memory.instructions.md` - Patterns for binding database results to UI (already exists as user instruction, needs consolidation)
- [ ] `gsc-integration-memory.instructions.md` - GSC command patterns (already exists as user instruction, needs consolidation)
- [ ] `debugging-memory.instructions.md` - Debugging workflows (already exists as user instruction, needs consolidation)
- [ ] `visual-api-integration.instructions.md` - Visual ERP API integration patterns
- [ ] `android-development.instructions.md` - Android-specific Avalonia patterns
- [ ] `performance-profiling.instructions.md` - How to profile and optimize

**Action Items:**
- [ ] Consolidate user memory files into repository instructions
- [ ] Create Visual API integration instruction file
- [ ] Create Android development instruction file
- [ ] Create performance profiling instruction file
- [ ] Add cross-references between related instruction files

**Estimated Effort**: 6-8 hours

---

### 2. Incomplete Prompts
**Impact**: Some prompts are stubs or outdated

**Prompts Needing Updates:**
- [ ] `prompts/implement-gsc-system.prompt.md` - GSC system implementation (stub)
- [ ] `prompts/mamp-database-sync.prompt.md` - Database schema sync (needs actual sync logic)
- [ ] `prompts/memory-merger.prompt.md` - Merge memory files (needs actual merge strategy)
- [ ] `prompts/reimagine.prompt.md` - Feature reimagining (needs clearer workflow)

**Prompts Needing Creation:**
- [ ] `prompts/create-android-view.prompt.md` - Android-specific view scaffolding
- [ ] `prompts/create-integration-test.prompt.md` - Integration test generation
- [ ] `prompts/optimize-performance.prompt.md` - Performance optimization workflow
- [ ] `prompts/implement-accessibility.prompt.md` - Accessibility enhancement workflow

**Estimated Effort**: 4-6 hours

---

### 3. Chatmode Enhancements
**Impact**: Chatmodes need more context-specific instructions

**Existing Chatmodes Needing Enhancement:**
- [ ] `chatmodes/mtm-architect.chatmode.md` - Add architecture decision record template
- [ ] `chatmodes/mtm-code-reviewer.chatmode.md` - Add automated code review checklist
- [ ] `chatmodes/mtm-debugger.chatmode.md` - Add systematic debugging workflow
- [ ] `chatmodes/mtm-manufacturing-expert.chatmode.md` - Add manufacturing domain glossary
- [ ] `chatmodes/mtm-specify-integrator.chatmode.md` - Add .specify workflow automation

**New Chatmodes Needed:**
- [ ] `chatmodes/mtm-performance-engineer.chatmode.md` - Performance analysis and optimization
- [ ] `chatmodes/mtm-test-engineer.chatmode.md` - Test creation and debugging
- [ ] `chatmodes/mtm-accessibility-expert.chatmode.md` - Accessibility compliance
- [ ] `chatmodes/mtm-security-auditor.chatmode.md` - Security vulnerability scanning

**Estimated Effort**: 6-8 hours

---

## 🟡 Medium Priority - Infrastructure Improvements

### 4. MAMP Database Sync Automation
**Files**: `mamp-database/` directory

**Current State**: Schema files manually updated

**Action Items:**
- [ ] Create PowerShell script to sync schema from MAMP to JSON files
- [ ] Add validation to ensure JSON matches actual database
- [ ] Add automated schema diff generation
- [ ] Create migration script generator from schema changes
- [ ] Add pre-commit hook to validate schema consistency

**Estimated Effort**: 4-6 hours

---

### 5. Memory File Consolidation
**Files**: `memory/` directory vs user memory files

**Issue**: Duplicate memory files exist in workspace vs .github

**Action Items:**
- [ ] Consolidate `avalonia-ui-memory.instructions.md` into `memory/avalonia-ui-patterns.md`
- [ ] Consolidate `database-ui-integration-memory.instructions.md` into `memory/database-patterns.md`
- [ ] Consolidate `debugging-memory.instructions.md` into instructions or memory
- [ ] Remove duplicate user memory files after consolidation
- [ ] Update copilot-instructions.md references

**Estimated Effort**: 2-3 hours

---

### 6. Prompt Template Standardization
**Issue**: Prompts have inconsistent structure

**Standardization Needs:**
- [ ] Create prompt template with sections: Purpose, Inputs, Outputs, Examples, Error Handling
- [ ] Audit all 30+ prompts for consistency
- [ ] Add validation rules to each prompt
- [ ] Add success criteria to each prompt
- [ ] Add related prompts cross-references

**Estimated Effort**: 4-6 hours

---

### 7. GitHub Actions Workflows
**Files**: `workflows/` directory

**Current State**: Workflows directory exists but unclear what's in it

**Needs Investigation:**
- [ ] Audit existing workflows
- [ ] Add CI/CD workflow for test execution
- [ ] Add workflow for automated schema validation
- [ ] Add workflow for prompt validation
- [ ] Add workflow for instruction file link checking
- [ ] Add workflow for changelog generation

**Estimated Effort**: 6-8 hours

---

## 🟢 Low Priority - Enhancements

### 8. Joyride Scripts Enhancement
**Files**: `joyride/scripts/` directory

**Current Scripts:**
- ✅ `batch_processor.cljs` - Batch processing utility
- ✅ `dependency_analyzer.cljs` - Dependency analysis
- ✅ `dependency_batch_processor.cljs` - Batch dependency processing
- ✅ `dependency_progress_tracker.cljs` - Progress tracking
- ✅ `dependency_spec_generator.cljs` - Spec generation from dependencies
- ✅ `model_switcher.cljs` - Model switching utility
- ✅ `progress_tracker.cljs` - General progress tracking
- ✅ `reverse_dependency_workflow.cljs` - Reverse dependency workflow
- ✅ `reverse_spec_workflow.cljs` - Reverse spec workflow
- ✅ `spec_generator.cljs` - Spec generation
- ✅ `view_analyzer.cljs` - View analysis

**Enhancement Opportunities:**
- [ ] Add script for automated TODO extraction from code
- [ ] Add script for generating dependency graphs
- [ ] Add script for code complexity analysis
- [ ] Add script for automated documentation generation
- [ ] Add script for test coverage analysis
- [ ] Add comprehensive documentation for each script

**Estimated Effort**: 8-10 hours

---

### 9. Awesome Copilot Prompts Integration
**Files**: `prompts/awesome-copilot-prompts/` directory

**Action Items:**
- [ ] Audit awesome-copilot-prompts for applicable patterns
- [ ] Adapt generic prompts to MTM-specific use cases
- [ ] Add MTM domain context to adapted prompts
- [ ] Create index of available awesome-copilot prompts
- [ ] Add usage examples for each adapted prompt

**Estimated Effort**: 4-6 hours

---

### 10. Instruction File Cross-Referencing
**Issue**: Instruction files don't reference each other

**Action Items:**
- [ ] Add "Related Instructions" section to each instruction file
- [ ] Add "See Also" links between related patterns
- [ ] Create instruction file dependency graph
- [ ] Add "Prerequisites" section to advanced instructions
- [ ] Create instruction file index/table of contents

**Estimated Effort**: 3-4 hours

---

### 11. Prompt Discovery and Search
**Issue**: 30+ prompts are hard to discover

**Action Items:**
- [ ] Create `prompts/INDEX.md` with categorized prompt list
- [ ] Add tags/keywords to each prompt file
- [ ] Create search script for finding relevant prompts
- [ ] Add "Frequently Used" section to INDEX
- [ ] Add prompt usage examples to copilot-instructions.md

**Estimated Effort**: 2-3 hours

---

### 12. Chatmode Activation Patterns
**Issue**: Chatmodes not consistently activated

**Action Items:**
- [ ] Document when to use each chatmode
- [ ] Add chatmode suggestion triggers in copilot-instructions.md
- [ ] Create chatmode switching guide
- [ ] Add chatmode chaining patterns (architect → specify → implement)
- [ ] Add chatmode effectiveness metrics

**Estimated Effort**: 2-3 hours

---

## 🔧 Technical Debt

### 13. Instruction File Maintenance

**Outdated Content:**
- [ ] Audit all instruction files for outdated framework versions
- [ ] Update Avalonia UI instructions for 11.3+ features
- [ ] Update MVVM Community Toolkit instructions for 8.3+ patterns
- [ ] Update MySQL instructions for connection pooling improvements
- [ ] Update testing instructions for xUnit 2.8+ patterns

**Missing Examples:**
- [ ] Add more code examples to all instruction files
- [ ] Add anti-patterns section to each instruction file
- [ ] Add troubleshooting section to each instruction file
- [ ] Add performance considerations to each instruction file

**Estimated Effort**: 6-8 hours

---

### 14. Schema Validation Automation

**Current Issue**: No automated validation of JSON schema files

**Action Items:**
- [ ] Create JSON schema for `schema-tables.json`
- [ ] Create JSON schema for `functions.json`, `indexes.json`, etc.
- [ ] Add schema validation to pre-commit hook
- [ ] Add schema validation tests
- [ ] Document schema format in `mamp-database/README.md`

**Estimated Effort**: 3-4 hours

---

### 15. Documentation Organization

**Current Issues:**
- Multiple README files with overlapping content
- Unclear documentation hierarchy
- Some files in wrong locations

**Reorganization Needs:**
- [ ] Create clear documentation structure in `docs/`
- [ ] Move `.github/` documentation to `docs/development/`
- [ ] Create documentation index in main README
- [ ] Add navigation between documentation files
- [ ] Remove duplicate documentation

**Estimated Effort**: 4-6 hours

---

## 📊 Metrics & Quality

### Current State
- **Instruction Files**: 13 (needs consolidation with user memory files)
- **Prompts**: 30+ (some incomplete)
- **Chatmodes**: 6 (needs expansion)
- **Memory Files**: 4 core + 3 user (needs consolidation)
- **Joyride Scripts**: 11 (functional)

### Quality Goals
- [ ] 100% of instruction files have code examples
- [ ] 100% of prompts have validation rules
- [ ] 100% of chatmodes have activation triggers
- [ ] All instruction files cross-referenced
- [ ] All prompts discoverable via INDEX
- [ ] All scripts documented with usage examples

---

## 🚀 Future Enhancements

### 16. AI Agent Self-Improvement
**Vision**: AI agents that learn from their own mistakes

**Potential Features:**
- [ ] Automated pattern extraction from successful implementations
- [ ] Self-updating instruction files based on usage patterns
- [ ] A/B testing different prompt variations
- [ ] Success rate tracking per prompt/chatmode
- [ ] Automated prompt optimization

**Estimated Effort**: Research phase (20+ hours)

---

### 17. Multi-Agent Collaboration
**Vision**: Multiple specialized AI agents working together

**Potential Patterns:**
- [ ] Architect agent creates high-level design
- [ ] Specify agent creates detailed specifications
- [ ] Implement agent writes code
- [ ] Test agent creates tests
- [ ] Review agent performs code review

**Estimated Effort**: Research phase (20+ hours)

---

### 18. Context-Aware Prompt Selection
**Vision**: AI automatically suggests relevant prompts based on current file/context

**Potential Features:**
- [ ] File type detection → prompt suggestions
- [ ] Error message detection → debug prompt suggestions
- [ ] Code complexity detection → refactor prompt suggestions
- [ ] Missing tests detection → test prompt suggestions

**Estimated Effort**: Research phase (15+ hours)

---

## ✅ Recently Completed (October 12, 2025)

- [x] Created comprehensive TODO tracking system
- [x] Documented all existing prompts, chatmodes, and instructions
- [x] Identified gaps in AI agent infrastructure
- [x] Established priority levels for improvements

---

## 📞 Quick Reference

### Useful Commands

```powershell
# Find all instruction files
Get-ChildItem -Path .github/instructions/ -Filter *.md

# Find all prompts
Get-ChildItem -Path .github/prompts/ -Filter *.md

# Find all chatmodes
Get-ChildItem -Path .github/chatmodes/ -Filter *.md

# Search for specific instruction content
Get-ChildItem -Path .github/ -Recurse -Filter *.md | Select-String -Pattern "ObservableProperty"

# List Joyride scripts
Get-ChildItem -Path .github/joyride/scripts/ -Filter *.cljs
```

### File Organization

```
.github/
├── instructions/          # How to code (patterns, standards)
├── prompts/               # What to do (task templates)
├── chatmodes/             # Role-specific modes (architect, reviewer, etc.)
├── memory/                # Lessons learned (persistent knowledge)
├── mamp-database/         # Database schema definitions
├── joyride/               # ClojureScript automation scripts
└── workflows/             # GitHub Actions automation
```

---

**Last Review**: October 12, 2025  
**Next Review**: Monthly or when adding new AI infrastructure  
**Maintained By**: Development Team + AI Agents
