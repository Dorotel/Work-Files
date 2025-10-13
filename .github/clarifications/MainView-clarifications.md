# Clarification Questions: MainView

**Status**: [ ] Initial / [ ] Partial / [x] Complete
**Date Generated**: October 10, 2025
**Last Updated**: October 10, 2025
**Based On**: Phase 1 Discovery Results

---

## Discovery Summary

**MainView** is the primary application shell managing:

- **3 main tabs**: Inventory, Remove, Transfer (with normal + advanced modes)
- **4 overlay systems**: Suggestion, Success, NewQuickButton, ThemeDropdown
- **Window management**: Multi-monitor support, positioning, sizing
- **Tab switching coordination**: Input clearing, focus management, progress overlays
- **Theme integration**: Theme V2 system, ThemeQuickSwitcher, ThemeDropdownOverlay

**Current Architecture**:

- **AXAML**: 205 lines, uses Theme V2, StyleSystem classes, proper x:DataType
- **Code-Behind**: 947+ lines with extensive business logic (window management, tab coordination, overlay management)
- **ViewModel**: Comprehensive MVVM Community Toolkit implementation with 6 child ViewModels

**Key Concerns Identified**:

1. **Excessive Code-Behind Logic**: 947+ lines of business logic in View (window positioning, tab switching, overlay coordination)
2. **Static State Management**: `IsTabSwitchInProgress` static flag for cross-component coordination
3. **Manual Focus Management**: Complex manual focus triggering and field searching
4. **View-Level Window Management**: Extensive window positioning/sizing logic in View layer
5. **Mixed Responsibilities**: View handles business logic (input clearing, validation triggers) that belongs in ViewModel

---

## Category A: UI/UX Requirements

### Layout & Structure

**Q1**: The MainView uses a complex grid layout with multiple overlay panels. Should the redesign:

**Context**: Currently there are 4 separate overlay panels (Suggestion, Success, NewQuickButton, ThemeDropdown) each positioned in the grid. This creates complexity in managing which overlay is visible and preventing multiple overlays from showing simultaneously.

**Plain Language**: Think of overlays like popup windows - right now we have 4 different popup systems. Should we keep them separate or combine them into one unified popup system?

**Agent Recommendation**: **Consolidate overlays into a single modal/dialog system** ✅

- **Why**: Simpler to manage (one visibility toggle vs four), prevents overlay conflicts automatically, reduces code complexity
- **Trade-off**: Requires refactoring existing overlay code but results in more maintainable system

**Options**:

- [ ] Keep the current overlay-based architecture (4 separate overlay panels)
  - *Pro*: No code changes needed
  - *Con*: Complex management, potential visibility conflicts
- [ANSWER] **Consolidate overlays into a single modal/dialog system with content switching** ✅ RECOMMENDED
  - *Pro*: Simplified management, no conflicts, cleaner code
  - *Con*: Requires refactoring work upfront
- [ ] Move overlay management to a dedicated OverlayManagerService
  - *Pro*: Keeps current structure but centralizes control
  - *Con*: Adds service layer complexity
- [ ] Other (please specify): _____________________

**Q2**: The tab system uses ContentControl for dynamic content (Inventory, Remove, Transfer). Should this:

**Context**: Right now tabs display their content using ContentControl which allows swapping different views in/out dynamically. This is like having one picture frame and swapping different pictures into it as needed.

**Plain Language**: When you click on different tabs (Inventory, Remove, Transfer), how should the application load the screen content?

**Agent Recommendation**: **Remain as ContentControl with view caching** ✅

- **Why**: Current pattern works well, preserves user input when switching tabs, good performance
- **Trade-off**: Slightly more memory usage but better user experience (no data loss)

**Options**:

- [ANSWER] **Remain as ContentControl with view caching (current pattern)** ✅ RECOMMENDED
  - *Pro*: Preserves user input, fast tab switching, proven pattern
  - *Con*: Uses more memory (keeps all tab screens loaded)
- [ ] Switch to TabItem.Content with direct view binding
  - *Pro*: Simpler XAML structure
  - *Con*: May lose user input when switching tabs
- [ ] Use a navigation region pattern with view injection
  - *Pro*: More flexible for future features
  - *Con*: More complex architecture, harder to understand
- [ ] Other (please specify): _____________________

**Q3**: The StyleSystem classes (MainContainer, HeaderPanel, etc.) are used throughout. Should we:

**Context**: StyleSystem classes are pre-defined styling rules that ensure consistent look across the application (like using pre-set themes in PowerPoint). They control colors, spacing, borders, etc.

**Plain Language**: Should we continue using the existing design templates or create custom styles just for this main screen?

**Agent Recommendation**: **Continue using StyleSystem classes as primary styling mechanism** ✅

- **Why**: Maintains consistency across entire app, easier to update globally, proven working system
- **Trade-off**: Less flexibility for unique MainView styling but consistency is more valuable

**Options**:

- [ANSWER] **Continue using StyleSystem classes as primary styling mechanism** ✅ RECOMMENDED
  - *Pro*: Consistent design, easier maintenance, automatic theme updates
  - *Con*: Limited customization for MainView-specific needs
- [ ] Migrate some elements to inline styles for flexibility
  - *Pro*: More control over individual element styling
  - *Con*: Loses consistency, harder to maintain
- [ ] Create component-specific style classes for MainView
  - *Pro*: Balance between consistency and customization
  - *Con*: Adds complexity, potential style conflicts
- [ ] Other (please specify): _____________________

### Window Management

**Q4**: The View currently handles all window positioning/sizing logic (947 lines of code-behind). Should this:

**Context**: Right now, the main screen itself contains 947 lines of programming code that handles where the window appears on screen, how big it is, and what happens when you have multiple monitors. This is like having the receptionist also doing accounting work - it works but isn't the right separation of responsibilities.

**Plain Language**: Who should be responsible for managing where the application window appears on screen and how large it is?

**Agent Recommendation**: **Move to WindowManagementService with ViewModel coordination** ✅

- **Why**: **CRITICAL REFACTORING** - Separates concerns (screen handles display, service handles logic), makes code testable, reduces code-behind from 947 to ~200 lines
- **Trade-off**: Requires creating new service but dramatically improves maintainability

**Technical Impact**: This addresses the #1 architectural issue - excessive business logic in View layer

**Options**:

- [ANSWER] **RECOMMENDED: Move to WindowManagementService with ViewModel coordination** ✅
  - *Pro*: Proper separation of concerns, testable, maintainable, reusable
  - *Con*: Requires significant refactoring work (~2-3 hours)
- [ ] Stay in View code-behind (keep current pattern)
  - *Pro*: No work required
  - *Con*: **Not recommended** - violates MTM patterns, hard to test/maintain
- [ ] Split between Service (business logic) and View (UI manipulation only)
  - *Pro*: Partial improvement
  - *Con*: Still mixed responsibilities, incomplete solution
- [ ] Other (please specify): _____________________

**Q5**: Multi-monitor support includes automatic centering and bounds checking. Requirements:

**Context**: The application currently automatically positions itself nicely when you have multiple monitors, making sure the window doesn't get stuck between screens or positioned off-screen.

**Plain Language**: If you have multiple computer monitors, should the app continue to automatically position itself smartly across your screens?

**Agent Recommendation**: **Preserve all current multi-monitor functionality** ✅

- **Why**: Multi-monitor setups are common in manufacturing environments, current functionality works well, users expect it
- **Trade-off**: None - this is working functionality we should keep

**Options**:

- [ANSWER] **Preserve all current multi-monitor functionality** ✅ RECOMMENDED
  - *Pro*: Works well now, meets user needs, prevents window positioning issues
  - *Con*: None - this is essential functionality
- [ ] Simplify to single-monitor support (reduce complexity)
  - *Pro*: Less code to maintain
  - *Con*: **Not recommended** - breaks existing functionality users rely on
- [ ] Enhance with monitor selection preferences
  - *Pro*: More user control
  - *Con*: Adds complexity without clear user benefit (current auto-positioning works)
- [ ] Other (please specify): _____________________

---

## Category B: ViewModel & Data Binding

### Child ViewModel Management

**Q6**: MainViewViewModel manages 6 child ViewModels (Inventory, Remove, Transfer, Advanced×2, QuickButtons). Should this:

**Context**: The main screen coordinator manages 6 sub-coordinators (one for each tab and features). Think of it like a manager who has 6 department heads reporting to them.

**Plain Language**: How should the main screen load and manage the different tab screens (Inventory, Remove, Transfer)?

**Agent Recommendation**: **Keep all child ViewModels as injected dependencies (current pattern)** ✅

- **Why**: Simple, predictable, all tabs ready immediately, follows dependency injection best practices
- **Trade-off**: Uses more memory upfront but provides better user experience (no loading delays)

**Options**:

- [ANSWER] **Keep all child ViewModels as injected dependencies (current pattern)** ✅ RECOMMENDED
  - *Pro*: Fast tab switching, simple architecture, all features ready
  - *Con*: Higher initial memory usage (but negligible on modern systems)
- [ ] Use lazy initialization for advanced ViewModels (load on demand)
  - *Pro*: Lower initial memory usage
  - *Con*: Delay when first accessing advanced mode, more complex code
- [ ] Create a ViewModelFactory for dynamic child creation
  - *Pro*: Most flexible for future changes
  - *Con*: Adds architectural complexity without clear benefit
- [ ] Other (please specify): _____________________

**Q7**: View caching (`_cachedInventoryTabView`, etc.) prevents recreation but adds state management complexity. Should we:

**Context**: Currently the application "remembers" what you were typing in each tab when you switch away. It's like keeping 3 notebooks open on your desk instead of closing and reopening them each time.

**Plain Language**: When you switch between tabs, should the app remember what you had typed on the previous tab?

**Agent Recommendation**: **Use ViewModel-based state persistence instead of view caching** ✅

- **Why**: Better architecture - data storage belongs in ViewModel not View, easier to test, same user experience
- **Trade-off**: Requires refactoring but results in cleaner, more maintainable code

**Options**:

- [ ] Keep view caching (preserve state across tab switches)
  - *Pro*: Currently working, no changes needed
  - *Con*: View manages data (architectural issue), harder to test
- [ ] Remove caching and recreate views on demand (simpler but loses state)
  - *Pro*: Simpler code
  - *Con*: **Not recommended** - users lose their input when switching tabs
- [ANSWER] **Use ViewModel-based state persistence instead of view caching** ✅ RECOMMENDED
  - *Pro*: Proper architecture, testable, maintains user experience
  - *Con*: Requires refactoring work but worth it
- [ ] Other (please specify): _____________________

### Mode Switching (Normal ↔ Advanced)

**Q8**: Inventory and Remove tabs have normal/advanced mode switching. Should this:

**Context**: Some tabs have a "simple" view and an "advanced" view with more features. Currently the main coordinator decides which view to show.

**Plain Language**: Who should control whether you're seeing the simple version or advanced version of a screen?

**Agent Recommendation**: **Create separate ViewModels for normal/advanced (already exists, make primary)** ✅

- **Why**: Best separation of concerns - each screen type manages itself, easier to maintain, ViewModels already exist
- **Trade-off**: None - this improves architecture and ViewModels already exist

**Options**:

- [ ] Stay in MainViewViewModel with content switching (current)
  - *Pro*: Centralized control
  - *Con*: Main coordinator shouldn't manage child screen details
- [ ] Move to child ViewModels with mode property
  - *Pro*: Better separation than current
  - *Con*: Single ViewModel handling two different screen types
- [ANSWER] **Create separate ViewModels for normal/advanced (already exists, make primary)** ✅ RECOMMENDED
  - *Pro*: Best separation, ViewModels already exist, each manages own complexity
  - *Con*: None - this is the correct architectural pattern
- [ ] Other (please specify): _____________________

**Q9**: Advanced mode switching triggers input clearing and progress overlays. Should this:

**Context**: When switching from simple to advanced mode (or vice versa), the screen needs to clear old input and show a brief "loading" message.

**Plain Language**: When switching between simple and advanced modes, what code should handle clearing the form and showing loading indicators?

**Agent Recommendation**: **Move input clearing logic to ViewModel/Service** ✅

- **Why**: Business logic belongs in ViewModel/Service layers, not View layer, makes code testable
- **Trade-off**: Requires refactoring but results in proper architecture

**Options**:

- [ANSWER] **RECOMMENDED: Move input clearing logic to ViewModel/Service** ✅
  - *Pro*: Proper architecture, testable, reusable logic
  - *Con*: Requires refactoring work upfront
- [ ] Keep in View event handlers (current pattern)
  - *Pro*: No changes needed
  - *Con*: **Not recommended** - violates MTM patterns, hard to test
- [ ] Use command pattern with ViewModel orchestration
  - *Pro*: Better than current, uses commands
  - *Con*: Partial solution - still better to fully move to Service
- [ ] Other (please specify): _____________________

---

## Category C: Service & State Management

### Tab Switching Coordination

**Q10**: The `IsTabSwitchInProgress` static flag prevents SuggestionOverlay during tab switches. Should this:

**Context**: There's a shared "flag" that multiple parts of the application check to know if a tab switch is happening. It's like a "Do Not Disturb" sign that's currently sitting in a shared location where anyone can see it - this can cause threading issues in software.

**Plain Language**: How should different parts of the app know when you're switching between tabs?

**Agent Recommendation**: **Move to TabSwitchCoordinationService with proper state management** ✅

- **Why**: **CRITICAL REFACTORING** - Static flags cause thread-safety issues, proper service provides centralized coordination, testable
- **Trade-off**: Requires creating new service but fixes architectural and potential bug issues

**Technical Impact**: This fixes thread-safety concerns and provides proper state management

**Options**:

- [ANSWER] **RECOMMENDED: Move to TabSwitchCoordinationService with proper state management** ✅
  - *Pro*: Thread-safe, testable, proper architecture, centralized coordination
  - *Con*: Requires creating new service (~1-2 hours work)
- [ ] Change to instance-level property in ViewModel
  - *Pro*: Better than static, easier to test
  - *Con*: Still not ideal - doesn't solve cross-component coordination
- [ ] Remove flag and use event-based coordination
  - *Pro*: More loosely coupled
  - *Con*: Complex event management, harder to debug
- [ ] Keep as static flag (current pattern)
  - *Pro*: No work required
  - *Con*: **Not recommended** - thread-safety issues, hard to test

**Q11**: Tab switching triggers multiple actions (input clearing, focus management, progress overlay). Architecture preference:

**Context**: When you switch tabs, many things need to happen: clear the form, move the cursor, show loading. Right now this is scattered across different event handlers like having assembly instructions spread across multiple pages.

**Plain Language**: How should the application organize all the tasks that need to happen when you switch between tabs?

**Agent Recommendation**: **Orchestrate through TabSwitchService with pipeline pattern** ✅

- **Why**: Centralized coordination, easy to add/remove steps, testable, clear execution order
- **Trade-off**: New service but provides clear, maintainable architecture

**Options**:

- [ANSWER] **RECOMMENDED: Orchestrate through TabSwitchService with pipeline pattern** ✅
  - *Pro*: Clear responsibility, easy to maintain/test, ordered execution
  - *Con*: Requires new service but architectural benefit is substantial
- [ ] Keep distributed across View event handlers (current)
  - *Pro*: No changes needed
  - *Con*: **Not recommended** - hard to maintain, scattered logic, difficult to debug
- [ ] Centralize in MainViewViewModel with command coordination
  - *Pro*: Some centralization
  - *Con*: ViewModel shouldn't orchestrate View-level concerns
- [ ] Other (please specify): _____________________

### Overlay Management

**Q12**: Four overlay systems (Suggestion, Success, NewQuickButton, ThemeDropdown) are managed in View code-behind. Should this:

**Context**: The application has 4 different popup/overlay systems currently controlled directly by the screen code. It's like having 4 different notification systems each with their own management.

**Plain Language**: Who should control showing and hiding popup messages and dialogs in the application?

**Agent Recommendation**: **Create OverlayManagementService with ViewModel coordination** ✅

- **Why**: **CRITICAL REFACTORING** - Centralizes overlay logic, prevents conflicts, testable, reduces code-behind
- **Trade-off**: Requires new service but dramatically simplifies overlay management

**Options**:

- [ANSWER] **RECOMMENDED: Create OverlayManagementService with ViewModel coordination** ✅
  - *Pro*: Centralized control, prevents conflicts, testable, reusable
  - *Con*: Requires creating new service (~2 hours work)
- [ ] Move to MainViewViewModel with overlay state properties
  - *Pro*: Removes from View
  - *Con*: ViewModel shouldn't manage View presentation details
- [ ] Keep in View code-behind (current pattern)
  - *Pro*: No work required
  - *Con*: **Not recommended** - violates MTM patterns, hard to test/maintain
- [ ] Other (please specify): _____________________

**Q13**: Overlay visibility is toggled via public methods (`ShowSuggestionOverlay()`, etc.). Should this:

**Context**: Currently the screen has public methods that other code can call to show/hide overlays. Think of it like having light switches that anyone can flip from anywhere.

**Plain Language**: How should other parts of the application request to show or hide popup messages?

**Agent Recommendation**: **Use ViewModel properties with bindings (IsOverlayVisible)** ✅

- **Why**: Proper MVVM pattern - data binding, automatic UI updates, testable, declarative
- **Trade-off**: None - this is the correct MVVM approach

**Options**:

- [ANSWER] **RECOMMENDED: Use ViewModel properties with bindings (IsOverlayVisible)** ✅
  - *Pro*: Proper MVVM, automatic UI sync, testable, clean architecture
  - *Con*: None - this is how MVVM should work
- [ ] Keep public methods for direct View control (current)
  - *Pro*: Direct control, no refactoring
  - *Con*: **Not recommended** - violates MVVM, imperative approach, hard to test
- [ ] Use messaging/events for overlay coordination
  - *Pro*: Loosely coupled
  - *Con*: More complex, harder to track, unnecessary when bindings suffice
- [ ] Other (please specify): _____________________

### Focus Management

**Q14**: Manual focus triggering with field name searching (`TriggerLostFocusOnField()`, etc.). Should this:

**Context**: Currently the View searches through screen elements by name to move the cursor/focus. It's like searching through a filing cabinet for a specific folder instead of having a direct reference to it.

**Plain Language**: How should the application move the typing cursor to different fields on screen?

**Agent Recommendation**: **Use FocusManagementService exclusively (already injected in ViewModel)** ✅

- **Why**: Service already exists and is injected, eliminates brittle field name searching, testable, centralized
- **Trade-off**: None - service already available, just use it

**Options**:

- [ANSWER] **RECOMMENDED: Use FocusManagementService exclusively (already injected in ViewModel)** ✅
  - *Pro*: Service exists, removes brittle code, testable, maintainable
  - *Con*: None - service is ready to use
- [ ] Keep manual field searching in View (current)
  - *Pro*: No changes needed
  - *Con*: **Not recommended** - brittle (breaks if field names change), not testable
- [ ] Hybrid: Service for strategy, View for execution
  - *Pro*: Partial improvement
  - *Con*: Unnecessarily complex when service can handle everything
- [ ] Other (please specify): _____________________
  NOTE ON THIS QUESTION: Currently when an overlay is active, the focus goes from the overlay back to the parent (seems to be a focus management issue).  when an overlay is active no interaction should occur with the parent elements.

---

## Category D: Manufacturing Domain Logic

### Input Clearing Strategy

**Q15**: Input clearing happens at multiple trigger points (tab switch, advanced mode, etc.). Should this:

**Context**: When certain actions happen (switching tabs, changing modes), form fields need to be cleared. Currently this logic is scattered in different places like having the same instruction written on multiple pages.

**Plain Language**: When should the application clear form fields, and what code should handle this?

**Agent Recommendation**: **Centralize in ClearInputService with context-aware clearing** ✅

- **Why**: Single responsibility, reusable logic, context-aware (knows which fields to clear when), testable
- **Trade-off**: Requires new service but eliminates scattered, duplicated logic

**Options**:

- [ANSWER] **RECOMMENDED: Centralize in ClearInputService with context-aware clearing** ✅
  - *Pro*: Single source of truth, context-aware, testable, maintainable
  - *Con*: Requires creating new service (~1 hour work)
- [ ] Keep distributed across event handlers (current)
  - *Pro*: No work required
  - *Con*: **Not recommended** - duplicated logic, hard to maintain, error-prone
- [ ] Move to child ViewModel ClearInputs() methods
  - *Pro*: Partial improvement
  - *Con*: Still scattered across multiple ViewModels, no central coordination
- [ ] Other (please specify): _____________________
NOTE ON THIS QUESTION:  WITH THE AMOUNT OF NEW SERVICES THIS SEEMS TO BEING WANTING TO IMPLEMENT MAKE SURE THEY ARE ORGANIZED AND DOCUMENTED PROPERLY.  USING FOLDERS AND SUBFOLDERS CAN HELP WITH THIS.


**Q16**: The `ClearAllTabInputsImmediate()` method prevents SuggestionOverlay triggers. Is this:

**Context**: There's a special "clear all fields immediately" method that bypasses normal clearing to prevent suggestion popups from appearing during the clear operation.

**Plain Language**: The current way of clearing fields has a special bypass to prevent popup suggestions - is this necessary or just a workaround?

**Agent Recommendation**: **Should be replaced with proper event/state coordination** ✅

- **Why**: "Immediate" bypass indicates architectural issue - proper state management eliminates need for workarounds
- **Trade-off**: None - fixing architecture removes need for special cases

**Options**:

- [ ] Critical behavior that must be preserved
  - *Evaluation*: Likely not critical - symptom of architectural issue
- [ ] Workaround that could be eliminated with better architecture
  - *Evaluation*: Most likely scenario - indicates state management gaps
- [ANSWER] **Should be replaced with proper event/state coordination** ✅ RECOMMENDED
  - *Pro*: Fixes root cause, eliminates workaround, cleaner architecture
  - *Con*: Requires refactoring but results in better system

### Progress Overlay Integration

**Q17**: Progress overlays show during tab switching and advanced mode transitions. Should this:

**Context**: When switching tabs or modes, a brief loading indicator appears. These operations are very fast (200-500 milliseconds), so the loading indicator flashes quickly.

**Plain Language**: Should the application show a "loading" message when switching between tabs or modes (these switches are very quick)?

**Agent Recommendation**: **Continue showing progress for these short operations** ✅

- **Why**: Provides user feedback that system is responding, prevents perceived "freeze", good UX even for short operations
- **Trade-off**: None - brief indicators improve perceived responsiveness

**Options**:

- [ANSWER] **Continue showing progress for these short operations (< 500ms)** ✅ RECOMMENDED
  - *Pro*: User feedback, prevents perceived freeze, good UX practice
  - *Con*: None - quick flash is better than no feedback
- [ ] Remove progress overlays for quick operations (simplify UX)
  - *Pro*: Simpler code
  - *Con*: **Not recommended** - users may perceive application as "frozen"
- [ ] Make progress overlay timing configurable
  - *Pro*: Flexibility
  - *Con*: Adds complexity without clear benefit, current timing works
- [ ] Other (please specify): _____________________

---

## Category E: Testing & Validation

### Success Criteria

**Q18**: For MainView re-implementation, success criteria should include:

**Context**: We need measurable goals to know when the refactoring is complete and successful.

**Plain Language**: How will we know the refactoring was successful? What should we measure?

**Agent Recommendation**: **All of the above** ✅

- **Why**: Comprehensive success requires multiple dimensions - code quality (< 200 lines), architecture (proper patterns), UX (no regressions), performance (better responsiveness)
- **Trade-off**: None - all criteria are achievable and necessary

**Options**:

- [ ] Reduce code-behind to < 200 lines (from 947+)
  - *Why*: Addresses the primary architectural issue
- [ ] Move all business logic to ViewModel/Services
  - *Why*: Proper separation of concerns following MTM patterns
- [ ] Maintain exact same user experience (no functional changes)
  - *Why*: Users shouldn't notice changes except improved responsiveness
- [ ] Improve tab switching performance (reduce delays)
  - *Why*: Better architecture naturally improves performance
- [ANSWER] **All of the above** ✅ RECOMMENDED
  - *Why*: All criteria are necessary for complete success

**Q19**: Edge cases to preserve:

**Context**: There are several complex behaviors that currently work correctly and must continue working after refactoring.

**Plain Language**: What specific features must continue working exactly as they do now?

**Agent Recommendation**: **All of the above** ✅

- **Why**: All listed edge cases are working functionality that users depend on
- **Trade-off**: None - these are must-preserve features

**Options**:

- [ ] Multi-monitor window positioning (already working)
  - *Why*: Users with multiple monitors depend on this
- [ ] Tab switch input clearing (prevents SuggestionOverlay)
  - *Why*: Prevents confusing popup behavior during transitions
- [ ] Overlay stacking/visibility coordination
  - *Why*: Prevents multiple overlays showing simultaneously
- [ ] Cached view state preservation across mode switches
  - *Why*: Users expect their input to persist when switching modes
- [ANSWER] **All of the above** ✅ RECOMMENDED
  - *Why*: All are essential, working features

**Q20**: Cross-platform testing priorities:

**Context**: The application can run on Windows, macOS, and Linux. We need to decide testing priorities.

**Plain Language**: Which computer operating systems should we test on, and how thoroughly?

**Agent Recommendation**: **Windows (primary) - extensive testing** ✅

- **Why**: MTM manufacturing environments primarily use Windows, focus resources where most users are
- **Trade-off**: Less testing on other platforms but maximizes value for user base

**Options**:

- [ANSWER] **Windows (primary) - extensive testing** ✅ RECOMMENDED
  - *Pro*: Focuses on primary user base, efficient resource use
  - *Secondary*: macOS/Linux - basic functionality testing (smoke tests only)
- [ ] macOS - basic functionality testing
  - *Evaluation*: Include as secondary testing, not primary
- [ ] Linux - basic functionality testing
  - *Evaluation*: Include as secondary testing, not primary
- [ ] All platforms equally
  - *Pro*: Comprehensive coverage
  - *Con*: Inefficient - most users on Windows

NOTE ON THIS QUESTION: CURRENTLY WINDOWS IS THE ONLY SUPPORTED OPERATING SYSTEM.

---

## Category F: Documentation & Architecture

### Architectural Decisions

**Q21**: The primary architectural concern is **excessive code-behind logic**. Preferred solution approach:

**Context**: The main screen has 947 lines of programming code in the "code-behind" file (the logic file associated with the screen). Best practice is under 200 lines. We need to decide how aggressively to refactor.

**Plain Language**: How much of the 947 lines of screen code should we move to proper locations (services, ViewModels)?

**Agent Recommendation**: **Aggressive refactoring - move 80%+ logic to ViewModel/Services** ✅

- **Why**: **CRITICAL DECISION** - Fixes architectural debt properly, brings code to MTM standards, worth the investment
- **Trade-off**: Most work upfront (~3-5 days) but results in maintainable, testable, standards-compliant code

**Impact**: This is the primary decision that determines all other refactoring work

**Options**:

- [ANSWER] **RECOMMENDED: Aggressive refactoring - move 80%+ logic to ViewModel/Services** ✅
  - *Pro*: Fixes architecture properly, MTM compliant, maintainable long-term
  - *Con*: Most work (~3-5 days) but worth the investment
  - *Result*: Code-behind < 200 lines, proper separation of concerns
- [ ] Conservative refactoring - move only critical logic (50-60%)
  - *Pro*: Less work (~2-3 days)
  - *Con*: Still architectural debt remaining, will need future refactoring
- [ ] Minimal refactoring - clean up but keep structure (20-30%)
  - *Pro*: Minimal work (~1 day)
  - *Con*: **Not recommended** - doesn't solve architectural issues
- [ ] No refactoring - focus only on AXAML improvements
  - *Pro*: No work
  - *Con*: **Not recommended** - misses opportunity to fix critical issues

**Q22**: Service creation priorities for refactoring:

**Context**: Four new services need to be created to properly handle responsibilities currently in the View. We need to prioritize the order.

**Plain Language**: Which new support systems should we build first?

**Agent Recommendation Priority Order**:

1. **WindowManagementService** (Priority 1) - Multi-monitor positioning, window sizing
2. **TabSwitchCoordinationService** (Priority 2) - Orchestrates tab switching pipeline
3. **OverlayManagementService** (Priority 3) - Manages 4 overlay systems
4. **ClearInputService** (Priority 4) - Context-aware form clearing

**Rationale**:

- **Why this order**: Window management has most code (250+ lines), tab switching is most complex coordination, overlays are most visible to users, input clearing is simplest
- **Dependencies**: Window and tab services are independent; overlay service depends on tab coordination; clear input uses tab context

**Order by priority (1=highest)**:

- WindowManagementService: **1** ← Most code, foundation service
- TabSwitchCoordinationService: **2** ← Complex coordination, affects multiple features
- OverlayManagementService: **3** ← Depends on tab coordination
- ClearInputService: **4** ← Depends on tab context, simplest to implement

**Q23**: Known issues to address:

**Context**: There are several architectural issues and workarounds currently in the code that should be fixed during refactoring.

**Plain Language**: What specific problems should we fix while refactoring?

**Agent Recommendation**: **All of the above** ✅

- **Why**: All listed issues are architectural problems that refactoring will naturally address
- **Trade-off**: None - fixing these is part of proper refactoring

**Options**:

- [ ] Static `IsTabSwitchInProgress` flag (thread-safety concerns)
  - *Why fix*: Thread-safety issue, will be replaced by TabSwitchCoordinationService
- [ ] Manual focus field searching (brittle, maintenance burden)
  - *Why fix*: Replaced by FocusManagementService (already exists)
- [ ] View caching adds state management complexity
  - *Why fix*: Replaced by ViewModel-based state persistence
- [ ] Progress overlays for short operations (< 500ms) may feel sluggish
  - *Why keep*: Actually good UX - provides user feedback
- [ANSWER] **All of the above (except last - keep progress overlays)** ✅ RECOMMENDED

**Q24**: Code documentation requirements:

**Context**: Refactored code needs documentation so future developers can understand and maintain it.

**Plain Language**: What kind of documentation should we write for the refactored code?

**Agent Recommendation**: **All of the above** ✅

- **Why**: Comprehensive documentation is essential for maintainability, especially after major refactoring
- **Trade-off**: None - documentation is part of professional development

**Options**:

- [ ] XML comments for all new Services/ViewModels
  - *Why*: Standard C# documentation, shows in IntelliSense
- [ ] Inline comments for complex refactored logic
  - *Why*: Helps understand non-obvious code decisions
- [ ] Architecture Decision Record (ADR) for major refactoring decisions
  - *Why*: Documents WHY decisions were made, invaluable for future maintenance
- [ANSWER] **All of the above** ✅ RECOMMENDED
  - *Why*: Complete documentation package ensures long-term maintainability

---

## User Responses

**Instructions**: Please answer as many questions as possible. Mark questions:

- **[ANSWER]**: Your response
- **[SKIP]**: Revisit later
- **[DEFAULT]**: Use MTM standard patterns

---

## Response Summary

**Answered**: 0/24 questions
**Categories Complete**: 0/6
**Pending**: All questions

**Critical Questions** (must answer to proceed):

- Q4: Window management architecture
- Q10: Tab switch coordination approach
- Q12: Overlay management architecture
- Q18: Success criteria definition
- Q21: Refactoring scope preference

**Next Steps**:

1. Answer at least the 5 critical questions above
2. Provide answers for preferred architectural directions
3. Indicate any edge cases or constraints that must be preserved
4. Reply "proceed" when ready, or "partial" to continue with assumptions

---

## Assumptions (If Proceeding with Partial Answers)

**Default Assumptions Based on MTM Standards**:

1. **Window Management**: Move to WindowManagementService (Q4)
2. **Tab Coordination**: Create TabSwitchCoordinationService (Q10)
3. **Overlay Management**: Create OverlayManagementService (Q12)
4. **Success Criteria**: Reduce code-behind to < 200 lines, maintain UX (Q18)
5. **Refactoring Scope**: Aggressive refactoring - move 80%+ to ViewModel/Services (Q21)
6. **View Caching**: Keep ViewModel-based state persistence (Q7)
7. **Focus Management**: Use FocusManagementService exclusively (Q14)
8. **Input Clearing**: Centralize in ClearInputService (Q15)
9. **Progress Overlays**: Keep for mode switches (200-500ms operations) (Q17)
10. **Cross-Platform**: Windows primary, macOS/Linux basic testing (Q20)

---

**AGENT STATUS**: Phase 2 complete - awaiting user responses before proceeding to Phase 3 (Anti-Pattern Detection).
