# MTM Template Application TODO

> **🤖 AI Agent Instructions**: When this file is referenced in chat without additional context, the agent should:
> 1. Select the next logical TODO item (prioritize by 🔴 High → 🟡 Medium → 🟢 Low)
> 2. Implement the selected TODO completely
> 3. Update this file by checking off completed items and moving them to "Recently Completed"
> 4. Update the "Last Updated" timestamp

Comprehensive tracking of application enhancements, incomplete features, technical debt, and optimization opportunities.

**Last Updated**: October 12, 2025  
**Application Version**: 1.0.0 (Feature 005 - Visual ERP Integration)  
**Framework**: .NET 9.0 + Avalonia UI 11.3+ + MVVM CommunityToolkit 8.3+

---

## 🔴 High Priority - Feature Completion

### 1. Configuration Error Dialog (Feature 009 - In Progress)
**Files**: `ViewModels/Settings/SettingsViewModel.cs`, `Views/MainWindow.axaml.cs`

**Incomplete Items:**
- [ ] **Line 47 (MainWindow.axaml.cs)**: Show ConfigurationErrorDialog with error details
- [ ] **Line 177 (SettingsViewModel.cs)**: Show success notification after save
- [ ] **Line 183 (SettingsViewModel.cs)**: Show error dialog to user on save failure
- [ ] **Line 298 (SettingsViewModel.cs)**: Show success notification after reset
- [ ] **Line 303 (SettingsViewModel.cs)**: Show error dialog on reset failure

**Action Items:**
- [ ] Create `Services/UI/NotificationService.cs` for toast notifications
- [ ] Implement success notification pattern (non-blocking, auto-dismiss)
- [ ] Implement error dialog pattern (blocking, requires acknowledgment)
- [ ] Wire up ConfigurationErrorDialog to MainWindow error event
- [ ] Add telemetry for configuration error frequency

**Related Spec**: `docs/features/009-configuration-error-dialog/`  
**Estimated Effort**: 3-4 hours

---

### 2. Settings Management UI (Feature 007 - Partially Complete)
**Files**: `ViewModels/Settings/SettingsViewModel.cs`

**Incomplete Items:**
- [ ] **Line 193**: Open file picker dialog for configuration import
- [ ] **Line 220**: Implement conflict resolution for duplicate settings (currently just overwrites)

**Action Items:**
- [ ] Implement file picker integration using `StorageProvider` API
- [ ] Create conflict resolution dialog with options: Overwrite, Keep Existing, Merge
- [ ] Add JSON schema validation before import
- [ ] Add import preview UI showing changes before applying
- [ ] Add undo/redo for settings changes

**Estimated Effort**: 4-6 hours

---

### 3. Debug Terminal Enhancements (Feature 003 - 95% Complete)
**Files**: `ViewModels/DebugTerminalViewModel.cs`

**Incomplete Items:**
- [ ] **Line 376**: Add confirmation dialog for Clear Errors action per CL-008
- [ ] **Line 422**: Add actual connection test when IMySqlClient has TestConnectionAsync method
- [ ] **Line 1235**: Implement actual clipboard interaction (currently in code-behind)

**Action Items:**
- [ ] Create reusable confirmation dialog component
- [ ] Add IMySqlClient.TestConnectionAsync() method
- [ ] Refactor clipboard logic to use proper ViewModel command pattern
- [ ] Add clipboard success/failure feedback

**Estimated Effort**: 2-3 hours

---

### 4. Navigation Service Implementation
**Files**: `Services/Navigation/NavigationService.cs`

**Incomplete Items:**
- [ ] **Line 74**: Actual navigation logic (currently placeholder)
- [ ] **Line 109**: Actual navigation logic for back navigation
- [ ] **Line 144**: Actual navigation logic for forward navigation

**Action Items:**
- [ ] Implement ViewModel switching logic
- [ ] Add transition animations between views
- [ ] Implement view caching for frequently accessed views
- [ ] Add navigation state persistence
- [ ] Wire up to Avalonia routing system

**Estimated Effort**: 6-8 hours

---

### 5. Error Handling Services (Feature 010 - Not Started)
**Files**: `Extensions/ServiceCollectionExtensions.cs`

**Incomplete Item:**
- [ ] **Line 428**: Implement error handling services (T134-T137 from spec)

**Planned Services:**
- [ ] Global exception handler middleware
- [ ] Error categorization service (database, network, UI, business logic)
- [ ] Error recovery strategies (retry, fallback, graceful degradation)
- [ ] Error reporting/telemetry service

**Estimated Effort**: 8-10 hours

---

## 🟡 Medium Priority - Enhancements

### 6. Diagnostics Service Extensions
**Files**: `Services/Diagnostics/DiagnosticsServiceExtensions.cs`

**Incomplete Items:**
- [ ] **Line 116**: Query actual MySql.Data connection pool metrics (placeholder)
- [ ] **Line 204**: Implement cache clearing logic
- [ ] **Line 238**: Implement application restart logic

**Action Items:**
- [ ] Research MySql.Data connection pool API for metrics
- [ ] Implement safe cache clearing (with backup/restore option)
- [ ] Implement graceful application restart (save state, close cleanly, relaunch)
- [ ] Add pre-restart confirmation dialog

**Estimated Effort**: 4-5 hours

---

### 7. Visual ERP Configuration Management
**Files**: `Extensions/ServiceCollectionExtensions.cs`

**Incomplete Item:**
- [ ] **Line 223**: Load Visual ERP settings from configuration service (currently hardcoded)

**Action Items:**
- [ ] Create Visual ERP configuration section in appsettings.json
- [ ] Add Visual ERP settings UI in Settings window
- [ ] Implement Visual API credential storage using SecretsService
- [ ] Add Visual API endpoint validation
- [ ] Add Visual API connection test button in settings

**Estimated Effort**: 3-4 hours

---

### 8. Feature Flag Enhancement
**Files**: `Services/Configuration/FeatureFlagEvaluator.cs`

**Incomplete Item:**
- [ ] **Line 240**: Placeholder implementation for deterministic rollout

**Current Issue**: Hash-based user distribution not fully deterministic

**Action Items:**
- [ ] Implement proper MurmurHash3 algorithm for consistent hashing
- [ ] Add user cohort assignment (e.g., internal users, beta testers, production)
- [ ] Add feature flag override UI for development/testing
- [ ] Add feature flag metrics (how many users enabled per flag)
- [ ] Add feature flag documentation generator

**Estimated Effort**: 3-4 hours

---

### 9. Platform-Specific Optimizations

#### Windows-Specific
- [ ] Implement Windows notification integration (toast notifications)
- [ ] Add Windows jump list support for frequent actions
- [ ] Optimize for high-DPI displays

#### Android-Specific
- [ ] Implement Android back button handling
- [ ] Add Android notifications for background operations
- [ ] Optimize for different screen sizes and orientations
- [ ] Implement Android-specific theme (Material Design integration)

**Estimated Effort**: 8-12 hours (per platform)

---

### 10. Performance Monitoring Enhancements
**Files**: `Services/Diagnostics/PerformanceMonitoringService.cs`

**Enhancement Opportunities:**
- [ ] Add CPU usage per-thread metrics
- [ ] Add network I/O metrics
- [ ] Add disk I/O metrics
- [ ] Add UI frame rate metrics
- [ ] Add memory allocation rate metrics
- [ ] Implement performance regression detection

**Estimated Effort**: 6-8 hours

---

## 🟢 Low Priority - Polish & UX

### 11. Localization/Internationalization
**Files**: `Services/Localization/LocalizationService.cs`

**Enhancement Opportunities:**
- [ ] Add language selection UI in Settings
- [ ] Add runtime language switching (without restart)
- [ ] Add missing translations for all UI strings
- [ ] Add plural form handling (e.g., "1 item" vs "2 items")
- [ ] Add date/time formatting per locale
- [ ] Add number formatting per locale (decimals, currency)

**Supported Languages**: Currently English only

**Target Languages**: Spanish, French, German, Chinese

**Estimated Effort**: 12-16 hours (full implementation)

---

### 12. Theme Customization
**Files**: `Services/Theme/ThemeService.cs`, `Services/Theme/OSDarkModeMonitor.cs`

**Enhancement Opportunities:**
- [ ] Add custom theme creation UI
- [ ] Add theme import/export
- [ ] Add per-control theme overrides
- [ ] Add theme preview before applying
- [ ] Add scheduled theme switching (e.g., dark mode at night)
- [ ] Add theme marketplace/gallery

**Estimated Effort**: 8-10 hours

---

### 13. Accessibility Enhancements
**Current State**: Basic accessibility support via Avalonia defaults

**Enhancement Opportunities:**
- [ ] Add keyboard shortcuts for all major actions
- [ ] Add screen reader optimization (ARIA labels)
- [ ] Add high-contrast theme support
- [ ] Add font size scaling
- [ ] Add focus indicators for keyboard navigation
- [ ] Test with Windows Narrator, NVDA, JAWS

**Estimated Effort**: 6-8 hours

---

### 14. UI/UX Polish
**Areas Needing Improvement:**

#### Loading States
- [ ] Add skeleton screens for data loading
- [ ] Add progress indicators with percentage/ETA
- [ ] Add cancellation support for long operations

#### Error States
- [ ] Add inline validation feedback (before submit)
- [ ] Add error state illustrations
- [ ] Add recovery action suggestions ("Try Again", "Contact Support")

#### Empty States
- [ ] Add meaningful empty state messages
- [ ] Add getting started guidance for first-time users
- [ ] Add quick action buttons in empty states

#### Animations
- [ ] Add page transition animations
- [ ] Add micro-interactions (button press feedback, etc.)
- [ ] Add loading spinners with smooth transitions

**Estimated Effort**: 10-12 hours

---

## 🔧 Technical Debt

### 15. Code Quality Improvements

#### Null Safety
**Issue**: Some nullable annotations missing or inconsistent

**Files to Audit:**
- All Services/ files
- All ViewModels/ files
- All Models/ files

**Action Items:**
- [ ] Enable nullable reference types project-wide (already enabled)
- [ ] Audit all public APIs for proper nullable annotations
- [ ] Remove unnecessary null-forgiving operators (!)
- [ ] Add null validation at service boundaries

**Estimated Effort**: 4-6 hours

---

#### Async/Await Consistency
**Issue**: Some synchronous wrappers around async operations

**Action Items:**
- [ ] Audit all async methods for proper cancellation support
- [ ] Add CancellationToken to all async public methods
- [ ] Remove sync-over-async patterns (.Result, .Wait())
- [ ] Document async best practices in CONTRIBUTING.md

**Estimated Effort**: 3-4 hours

---

#### Dependency Injection Cleanup
**Issue**: Some services registered but rarely used

**Action Items:**
- [ ] Audit service registrations in ServiceCollectionExtensions
- [ ] Remove unused service registrations
- [ ] Document service lifetimes (Singleton, Transient, Scoped)
- [ ] Add service dependency graph visualization
- [ ] Profile DI container startup time

**Estimated Effort**: 2-3 hours

---

### 16. Performance Optimizations

#### Startup Performance
**Current**: Boot sequence completes in ~4-6 seconds

**Optimization Opportunities:**
- [ ] Lazy-load non-critical services
- [ ] Defer UI rendering until after Stage 2
- [ ] Optimize image asset loading (use WebP format)
- [ ] Reduce DI container reflection overhead
- [ ] Profile and optimize Stage 1 service initialization

**Target**: <3 seconds boot time

**Estimated Effort**: 6-8 hours

---

#### Memory Optimizations
**Current**: ~80-100MB memory usage after startup

**Optimization Opportunities:**
- [ ] Implement image caching with LRU eviction
- [ ] Reduce ObservableCollection allocations
- [ ] Use object pooling for frequently allocated objects
- [ ] Profile and fix memory leaks (if any)
- [ ] Optimize ViewModel memory footprint

**Target**: <70MB memory usage

**Estimated Effort**: 6-8 hours

---

#### UI Rendering Performance
**Current**: Generally 60 FPS, some drops during data loading

**Optimization Opportunities:**
- [ ] Implement virtualization for all large lists
- [ ] Use ItemsRepeater instead of ListBox where appropriate
- [ ] Optimize DataGrid rendering (fewer columns, simpler templates)
- [ ] Defer non-visible UI rendering
- [ ] Profile and optimize complex AXAML layouts

**Target**: Consistent 60 FPS

**Estimated Effort**: 4-6 hours

---

### 17. Code Organization

#### Service Layer Restructuring
**Issue**: Some services have mixed responsibilities

**Refactoring Opportunities:**
- [ ] Split DiagnosticsService into smaller services
- [ ] Extract common retry logic into RetryPolicy service
- [ ] Move validation logic from services to dedicated validators
- [ ] Create service interfaces for testability

**Estimated Effort**: 6-8 hours

---

#### ViewModel Cleanup
**Issue**: Some ViewModels have too many responsibilities

**Refactoring Opportunities:**
- [ ] Split DebugTerminalViewModel into feature-specific VMs
- [ ] Extract common ViewModel base functionality
- [ ] Create ViewModel helpers for common patterns (loading states, etc.)
- [ ] Reduce property count in large ViewModels

**Estimated Effort**: 8-10 hours

---

### 18. Documentation Gaps

**Missing Documentation:**
- [ ] API documentation for all public services
- [ ] Architecture decision records (ADRs)
- [ ] Component interaction diagrams
- [ ] Deployment guide for production
- [ ] Troubleshooting guide for common issues
- [ ] Performance tuning guide
- [ ] Security hardening guide

**Estimated Effort**: 12-16 hours

---

## 📊 Metrics & Monitoring

### Current Application Metrics
- **Boot Time**: Stage 0 (1s) + Stage 1 (3s) + Stage 2 (1s) = ~5s total
- **Memory Usage**: ~80-100MB after startup
- **UI Thread FPS**: 50-60 FPS typical, drops to 30-40 during heavy operations
- **Services Registered**: 45+ services in DI container
- **ViewModels**: 15+ ViewModels (Main, Splash, Debug Terminal, Settings, etc.)
- **Custom Controls**: 11 custom controls (StatusCard, MetricDisplay, BootTimelineChart, etc.)

### Target Metrics
- [ ] Boot Time: <3 seconds
- [ ] Memory Usage: <70MB
- [ ] UI Thread FPS: Consistent 60 FPS
- [ ] First Render Time: <500ms
- [ ] Time to Interactive: <2 seconds

---

## 🚀 Future Features (Not Yet Specified)

### Potential Enhancements
- [ ] Offline mode with data sync when reconnected
- [ ] Multi-user collaboration features
- [ ] Advanced reporting and analytics
- [ ] Integration with other ERP systems (beyond Visual)
- [ ] Mobile companion app (iOS)
- [ ] Web portal for remote access
- [ ] Plugin/extension system for third-party integrations

---

## ✅ Recently Completed (October 12, 2025)

### Feature 003: Debug Terminal Modernization
- [x] Performance monitoring service with real-time metrics
- [x] Diagnostic snapshot export functionality
- [x] Error history tracking and display
- [x] Connection pool statistics
- [x] Boot timeline visualization
- [x] All tests passing (858/858)

### Bug Fixes
- [x] Fixed Avalonia converter threading issues
- [x] Fixed Debug Terminal navigation control names
- [x] Fixed performance monitoring cancellation handling
- [x] Fixed SplitView DisplayMode (CompactInline → Inline)

---

## 📞 Contributing Guidelines

### Before Starting Work
1. Check this TODO for existing tracking
2. Review related specification in `docs/features/`
3. Create feature branch: `feature/XXX-description`
4. Update TODO with "In Progress" status

### During Development
1. Follow patterns in `.github/instructions/`
2. Add unit tests for new services
3. Add integration tests for UI components
4. Update this TODO as items are completed

### Before Submitting PR
1. Run full test suite (`dotnet test`)
2. Update documentation
3. Mark TODO items as complete
4. Add "Recently Completed" entry

---

## 📚 Quick Reference

### Key Files
- **DI Registration**: `Extensions/ServiceCollectionExtensions.cs`
- **Boot Sequence**: `Services/Boot/BootOrchestrator.cs`
- **Configuration**: `Services/Configuration/ConfigurationService.cs`
- **Diagnostics**: `Services/Diagnostics/DiagnosticsService.cs`
- **Main ViewModel**: `ViewModels/MainViewModel.cs`

### Useful Commands
```powershell
# Build application
dotnet build MTM_Template_Application.csproj

# Run application (Desktop)
dotnet run --project ../MTM_Template_Application.Desktop/

# Run application (Android)
dotnet build ../MTM_Template_Application.Android/ -f net9.0-android -t:Install

# Profile memory
dotnet-counters monitor --process-id <PID>
```

---

**Last Review**: October 12, 2025  
**Next Review**: Monthly or when completing major features  
**Maintainer**: Development Team
