# Documentation Coverage Checklist: MTM Avalonia Template

**Purpose**: Track completion of instruction file documentation for all implemented features in the MTM Avalonia Template application.  
**Created**: October 13, 2025  
**Related**: `.copilot-scripts/missing-documentation-report.json`  
**Coverage**: 0% (0/168 files documented)

**Goal**: Achieve 100% documentation coverage by creating instruction files that mirror the codebase structure and document all patterns, best practices, and implementation guidelines.

---

## 🔴 Critical Priority - Core Services (6 categories, 30 files)

These are foundational services that the entire application depends on. Documentation is essential for maintaining and extending the application.

### Services/Boot (10 files)

- [ ] DOC-001 Create `.github/instructions/Services/Boot/boot-orchestrator.instructions.md`
  - **Files Covered**: BootOrchestrator.cs, BootStage0Service.cs, BootStage1Service.cs, BootStage2Service.cs, IBootOrchestrator.cs, etc.
  - **Key Topics**: 3-stage boot sequence, service initialization order, startup performance targets, stage completion criteria
  - **Priority**: Critical - Boot sequence is the application entry point
  - **Estimated Effort**: 4-6 hours (complex with performance requirements)

### Services/Configuration (6 files)

- [ ] DOC-002 Create `.github/instructions/Services/Configuration/configuration-service.instructions.md`
  - **Files Covered**: ConfigurationService.cs, IConfigurationService.cs, ConfigurationChangedEventArgs.cs, etc.
  - **Key Topics**: Layered configuration precedence, environment variables, user preferences persistence, change notifications
  - **Priority**: Critical - Configuration affects all services
  - **Estimated Effort**: 3-4 hours (well-defined patterns from Feature 002)

### Services/Secrets (5 files)

- [ ] DOC-003 Create `.github/instructions/Services/Secrets/secrets-management.instructions.md`
  - **Files Covered**: ISecretsService.cs, WindowsSecretsService.cs, AndroidSecretsService.cs, SecretsServiceFactory.cs, etc.
  - **Key Topics**: OS-native credential storage, platform-specific implementations, security best practices, credential recovery
  - **Priority**: Critical - Security-sensitive service
  - **Estimated Effort**: 3-4 hours (security considerations require detail)

### Services/Visual (2 files)

- [ ] DOC-004 Create `.github/instructions/Services/Visual/visual-api-integration.instructions.md`
  - **Files Covered**: VisualApiClient.cs, IVisualApiClient.cs
  - **Key Topics**: HTTP client configuration, retry policies with Polly, contract testing, offline sync, API whitelist enforcement
  - **Priority**: Critical - Core integration point with Visual ERP
  - **Estimated Effort**: 4-5 hours (complex integration patterns)

### Services/Navigation (3 files)

- [ ] DOC-005 Create `.github/instructions/Services/Navigation/navigation-service.instructions.md`
  - **Files Covered**: NavigationService.cs, INavigationService.cs, NavigationEventArgs.cs
  - **Key Topics**: ViewModel navigation patterns, view caching, history management, transition animations
  - **Priority**: Critical - Affects all UI navigation
  - **Estimated Effort**: 3-4 hours

### Services/ErrorHandling (4 files)

- [ ] DOC-006 Create `.github/instructions/Services/ErrorHandling/error-handling.instructions.md`
  - **Files Covered**: ErrorCategorizer.cs, ErrorRecoveryStrategy.cs, IErrorHandler.cs, etc.
  - **Key Topics**: Error categorization (database, network, UI, business), retry strategies, graceful degradation, user notifications
  - **Priority**: Critical - Affects application reliability
  - **Estimated Effort**: 3-4 hours

---

## 🟡 High Priority - ViewModels & UI Layer (12 categories, 35 files)

ViewModels drive the UI and contain most of the user-facing logic. Documentation ensures consistent MVVM patterns.

### ViewModels/viewmodel-patterns (Base Patterns)

- [ ] DOC-007 Create `.github/instructions/ViewModels/viewmodel-patterns.instructions.md`
  - **General Pattern File**: Applies to ALL ViewModels
  - **Key Topics**: ObservableObject patterns, command patterns with [RelayCommand], validation, error handling, async operations
  - **Priority**: High - Foundation for all ViewModels
  - **Estimated Effort**: 4-5 hours (comprehensive guide)

### ViewModels/MainViewModel.cs (1 file)

- [ ] DOC-008 Document MainViewModel patterns in `viewmodel-patterns.instructions.md`
  - **Key Topics**: Application shell coordination, tab management, navigation integration
  - **Priority**: High - Primary application ViewModel

### ViewModels/DebugTerminalViewModel.cs (1 file)

- [ ] DOC-009 Document DebugTerminalViewModel patterns in `viewmodel-patterns.instructions.md`
  - **Key Topics**: Diagnostics data binding, performance metrics, command patterns for debug actions
  - **Priority**: High - Complex diagnostic features

### ViewModels/SplashViewModel.cs (1 file)

- [ ] DOC-010 Document SplashViewModel patterns in `viewmodel-patterns.instructions.md`
  - **Key Topics**: Boot progress tracking, stage notifications, timeout handling
  - **Priority**: High - Boot sequence UI

### ViewModels/Settings (2 files)

- [ ] DOC-011 Create `.github/instructions/ViewModels/Settings/settings-viewmodel.instructions.md`
  - **Files Covered**: SettingsViewModel.cs, SettingsTabViewModel.cs
  - **Key Topics**: Configuration management UI, import/export, validation, user notifications
  - **Priority**: High - Settings management patterns
  - **Estimated Effort**: 2-3 hours

### ViewModels/Configuration (1 file)

- [ ] DOC-012 Document ConfigurationViewModel patterns in `viewmodel-patterns.instructions.md`
  - **Key Topics**: Configuration display, real-time updates, change tracking
  - **Priority**: High

### Views/view-patterns (All Views)

- [ ] DOC-013 Create `.github/instructions/Views/view-patterns.instructions.md`
  - **General Pattern File**: Applies to ALL Views (28 files)
  - **Key Topics**: AXAML structure, CompiledBinding with x:DataType, design-time data, Theme V2 integration
  - **Priority**: High - Foundation for all views
  - **Estimated Effort**: 4-5 hours

### Views/Settings (18 files)

- [ ] DOC-014 Create `.github/instructions/Views/Settings/settings-view-patterns.instructions.md`
  - **Files Covered**: All Settings view AXAML/CS files
  - **Key Topics**: Settings UI patterns, tab navigation, form validation display
  - **Priority**: High - Complex settings UI
  - **Estimated Effort**: 2-3 hours

### Extensions/dependency-injection (1 file)

- [ ] DOC-015 Create `.github/instructions/Extensions/dependency-injection.instructions.md`
  - **Files Covered**: ServiceCollectionExtensions.cs
  - **Key Topics**: Service registration patterns, lifetime management (Singleton, Transient, Scoped), factory patterns
  - **Priority**: High - DI configuration affects entire app
  - **Estimated Effort**: 3-4 hours

---

## 🟢 Medium Priority - Supporting Services (5 categories, 30 files)

These services support core functionality but are not critical path dependencies.

### Services/Diagnostics (13 files)

- [ ] DOC-016 Create `.github/instructions/Services/Diagnostics/diagnostics-service.instructions.md`
  - **Files Covered**: DiagnosticsService.cs, DiagnosticsServiceExtensions.cs, HealthCheckService.cs, etc.
  - **Key Topics**: Performance monitoring, health checks, connection pool metrics, export functionality
  - **Priority**: Medium - Non-critical but valuable
  - **Estimated Effort**: 3-4 hours

### Services/Cache (7 files)

- [ ] DOC-017 Create `.github/instructions/Services/Cache/cache-service.instructions.md`
  - **Files Covered**: CacheService.cs, LZ4CompressionService.cs, ICacheService.cs, etc.
  - **Key Topics**: LZ4 compression, cache invalidation, offline-first data access, performance targets
  - **Priority**: Medium
  - **Estimated Effort**: 2-3 hours

### Services/Logging (5 files)

- [ ] DOC-018 Create `.github/instructions/Services/Logging/logging-patterns.instructions.md`
  - **Files Covered**: SerilogConfiguration.cs, LoggingService.cs, etc.
  - **Key Topics**: Structured logging with Serilog, log levels, sensitive data filtering, file rotation
  - **Priority**: Medium
  - **Estimated Effort**: 2-3 hours

### Models/domain-models (31 files across 12 subdirectories)

- [ ] DOC-019 Create `.github/instructions/Models/Boot/boot-models.instructions.md`
  - **Files Covered**: BootStageResult.cs, BootMetrics.cs, BootTimelineEntry.cs
  - **Estimated Effort**: 1 hour

- [ ] DOC-020 Create `.github/instructions/Models/Cache/cache-models.instructions.md`
  - **Files Covered**: CacheEntry.cs, CacheMetadata.cs
  - **Estimated Effort**: 1 hour

- [ ] DOC-021 Create `.github/instructions/Models/Configuration/configuration-models.instructions.md`
  - **Files Covered**: ConfigurationSource.cs, EnvironmentConfig.cs, etc.
  - **Estimated Effort**: 1 hour

- [ ] DOC-022 Create `.github/instructions/Models/Diagnostics/diagnostics-models.instructions.md`
  - **Files Covered**: DiagnosticSnapshot.cs, ErrorLogEntry.cs, PerformanceMetrics.cs, etc. (10 files)
  - **Estimated Effort**: 2 hours

- [ ] DOC-023 Create `.github/instructions/Models/ErrorHandling/error-models.instructions.md`
  - **Files Covered**: ErrorCategory.cs, ErrorSeverity.cs, etc.
  - **Estimated Effort**: 1 hour

- [ ] DOC-024 Create general Models documentation: `.github/instructions/Models/domain-models.instructions.md`
  - **Key Topics**: DTOs, validation, immutability patterns, nullable reference types
  - **Priority**: Medium - Models are data contracts
  - **Estimated Effort**: 2-3 hours (covers all model patterns)

### Controls/custom-controls (20 files)

- [ ] DOC-025 Create `.github/instructions/Controls/custom-controls.instructions.md`
  - **Files Covered**: All 10 custom controls (ActionButtonGroup, BootTimelineChart, ConfigurationErrorDialog, etc.)
  - **Key Topics**: UserControl vs TemplatedControl, styled properties, Avalonia behaviors, reusability patterns
  - **Priority**: Medium - Complex UI components
  - **Estimated Effort**: 3-4 hours

### ViewModels/Settings (covered above in High Priority)

---

## 🔵 Low Priority - Utilities & Small Categories (4 categories, 5 files)

These are smaller categories with few files that can be combined into single documentation files.

### Converters (3 files)

- [ ] DOC-026 Update `.github/instructions/Converters/avalonia-converter-patterns.instructions.md` (already exists)
  - **Files Covered**: BootStageToColorConverter.cs, ErrorSeverityToIconConverter.cs, MemoryUsageToColorConverter.cs
  - **Key Topics**: IValueConverter implementation, type safety, performance, reusability
  - **Priority**: Low - Small category
  - **Estimated Effort**: 1-2 hours (update existing file)

### Behaviors (1 file)

- [ ] DOC-027 Create `.github/instructions/Behaviors/avalonia-behaviors.instructions.md`
  - **Files Covered**: ProgressAnimationBehavior.cs
  - **Key Topics**: Attached behaviors, event handling, reusable UI logic
  - **Priority**: Low - Single file
  - **Estimated Effort**: 1 hour

### Services/Theme (4 files)

- [ ] DOC-028 Update `.github/instructions/Views/Themes.instructions.md` (already exists)
  - **Files Covered**: ThemeService.cs, IThemeService.cs, ThemeManager.cs, etc.
  - **Key Topics**: Theme V2 system, dynamic resource management, theme switching
  - **Priority**: Low - Theme system already documented
  - **Estimated Effort**: 1-2 hours (update with service patterns)

### Services/Localization (4 files)

- [ ] DOC-029 Create `.github/instructions/Services/Localization/localization-patterns.instructions.md`
  - **Files Covered**: LocalizationService.cs, ResourceManager.cs, etc.
  - **Key Topics**: i18n resource management, language switching, fallback patterns
  - **Priority**: Low - Feature not actively used yet
  - **Estimated Effort**: 1-2 hours

---

## 📊 Progress Tracking

### Overall Statistics
- **Total Files**: 168 source files
- **Documented Files**: 0
- **Documentation Coverage**: 0%
- **Target Coverage**: 100%

### Priority Breakdown
- **Critical Priority**: 6 items (30 files) - **0% Complete**
- **High Priority**: 9 items (35 files) - **0% Complete**
- **Medium Priority**: 10 items (98 files) - **0% Complete**
- **Low Priority**: 4 items (5 files) - **0% Complete**

### Estimated Total Effort
- **Critical**: 20-27 hours
- **High**: 18-23 hours
- **Medium**: 12-17 hours
- **Low**: 4-7 hours
- **Total**: 54-74 hours (approximately 7-10 full days)

---

## 🎯 Completion Strategy

### Phase 1: Critical Services (Week 1)
Focus on core services that affect entire application:
1. Boot orchestrator patterns
2. Configuration service patterns
3. Secrets management patterns
4. Visual API integration patterns
5. Navigation service patterns
6. Error handling patterns

**Target**: 6 documentation files completed

### Phase 2: ViewModels & Views (Week 2)
Document MVVM patterns and UI layer:
1. ViewModel base patterns (applies to all)
2. View patterns (applies to all)
3. Settings-specific patterns
4. Extension/DI patterns

**Target**: 4 documentation files completed (covers 35+ files)

### Phase 3: Supporting Services (Week 3)
Document remaining services and models:
1. Diagnostics service patterns
2. Cache service patterns
3. Logging patterns
4. Model patterns (all subdirectories)
5. Custom controls patterns

**Target**: 6 documentation files completed

### Phase 4: Utilities & Polish (Week 4)
Complete remaining documentation:
1. Update Converters documentation
2. Behaviors patterns
3. Theme service patterns
4. Localization patterns
5. Review and cross-link all documentation

**Target**: 4 documentation files updated/created

---

## ✅ Quality Checklist

For each documentation file created, ensure:

- [ ] **Frontmatter Complete**: description, applyTo, priority fields
- [ ] **Real Examples**: Code examples from actual codebase (not placeholders)
- [ ] **Pattern Documentation**: Common patterns with rationale
- [ ] **Anti-Patterns**: What NOT to do with clear explanations
- [ ] **Testing Guidelines**: Unit, integration, contract test patterns
- [ ] **Performance Targets**: Specific metrics where applicable
- [ ] **Cross-References**: Links to related instruction files
- [ ] **Common Pitfalls**: Real issues encountered during development
- [ ] **Maintenance Info**: Last updated date, status, TODOs

---

## 📝 Notes

- **Template Files Created**: 17 instruction file templates were automatically generated by the restructure script
- **Templates Need Content**: All template files currently contain placeholders and must be filled with actual patterns from the codebase
- **Continuous Updates**: As new patterns emerge, documentation should be updated immediately
- **Version Control**: Commit documentation updates alongside code changes

---

## 🔗 Related Resources

- **Missing Documentation Report**: `.copilot-scripts/missing-documentation-report.json`
- **Restructure Script**: `.copilot-scripts/restructure-instruction-files.ps1`
- **Scanner Script**: `.copilot-scripts/find-missing-documentation.ps1`
- **Existing Documentation**: `.github/instructions/` (restructured to mirror codebase)
- **AGENTS.md**: References to instruction files for AI agent context

---

**Last Updated**: October 13, 2025  
**Next Review**: After each documentation file completion  
**Completion Target**: November 15, 2025
