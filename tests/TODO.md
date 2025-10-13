# Test Project TODO

> **🤖 AI Agent Instructions**: When this file is referenced in chat without additional context, the agent should:
> 1. Select the next logical TODO item (prioritize by 🔴 High → 🟡 Medium → 🟢 Low)
> 2. Implement the selected TODO completely
> 3. Update this file by checking off completed items and moving them to "Recently Completed"
> 4. Update the "Last Updated" timestamp

Comprehensive tracking of test improvements, skipped tests, technical debt, and enhancement opportunities.

**Last Updated**: October 12, 2025  
**Current Test Status**: 858 passing, 2 skipped, 0 failing  
**Test Categories**: Unit (753), Integration (90), Contract (15), Performance (2)

---

## 🔴 High Priority - Blocking Issues

### 1. Database Integration Test Setup
**Impact**: 2 tests skipped, database testing incomplete  
**Files**: `tests/integration/SettingsPersistenceTests.cs`

**Skipped Tests:**
- `LoadUserPreferencesAsync_ShouldLoadFromDatabase` (Line 41)
- `SaveUserPreferenceAsync_ShouldPersistToDatabase` (Line 59)

**Action Items:**
- [ ] Create `tests/TestHelpers/DatabaseSeeder.cs` for test data management
- [ ] Implement test user fixtures with predictable UserIds
- [ ] Add database transaction rollback pattern for test isolation
- [ ] Create `tests/TestHelpers/DatabaseFixture.cs` implementing `IAsyncLifetime`
- [ ] Update tests to use `[Collection("DatabaseTests")]` with shared fixture
- [ ] Create SQL seed scripts in `tests/TestData/seed_users.sql`
- [ ] Add environment variable check (skip tests if DB not available)
- [ ] Document database test setup in `tests/README.md`

**Estimated Effort**: 4-6 hours

---

## 🟡 Medium Priority - Code Quality

### 2. Missing Test Coverage Areas

#### Visual API Integration
- [ ] Visual API authentication flow end-to-end
- [ ] Visual API rate limiting behavior
- [ ] Visual API circuit breaker state transitions
- [ ] Visual API whitelist violation scenarios

#### Boot Sequence Edge Cases
- [ ] Stage timeout handling
- [ ] Stage failure recovery and rollback
- [ ] Stage dependency failures (e.g., database down during Stage 1)
- [ ] Boot sequence interruption/cancellation

#### Theme System
- [ ] Theme switching with active UI elements
- [ ] Theme persistence across application restarts
- [ ] Custom theme loading from user directory

#### Configuration Service
- [ ] Environment variable precedence with all 3 layers
- [ ] Configuration change event throttling
- [ ] Concurrent configuration updates

#### Debug Terminal
- [ ] Performance snapshot memory limit enforcement (100 snapshots)
- [ ] Error history limit enforcement
- [ ] Export functionality with large datasets

---

### 5. Test Infrastructure Improvements

#### Test Data Builders
- [ ] Create builders for: User, ConfigurationSetting, FeatureFlag, BootMetrics
- [ ] Create builders for: CacheEntry, DiagnosticSnapshot, ErrorLogEntry
- [ ] Add randomization support for property values

#### Custom FluentAssertions Extensions
- [ ] Create assertions for: BootMetrics, DiagnosticSnapshot
- [ ] Add domain-specific assertions: `MeetPerformanceTargets()`, `BeWithinMemoryBudget()`

#### Test Documentation
- [ ] Create `tests/README.md` - Test organization and running strategies
- [ ] Create `tests/CONTRIBUTING.md` - Test authoring guidelines
- [ ] Create `tests/TROUBLESHOOTING.md` - Common failures and fixes
- [ ] Document mocking strategy guide

---

### 6. Performance Test Enhancements

- [ ] Add boot sequence performance regression tests
- [ ] Add memory leak detection tests (run for extended periods)
- [ ] Add UI responsiveness tests (measure render times)
- [ ] Add database query performance tests
- [ ] Add Visual API call latency tests
- [ ] Add concurrent operation stress tests

**Target**: Reduce full suite execution from 20s to <10s

---

### 7. Contract Test Enhancements

- [ ] Add Visual API contract tests (verify actual API responses)
- [ ] Add configuration schema contract tests
- [ ] Add log message format contract tests
- [ ] Add event message contract tests (MessageBus envelopes)
- [ ] Add cache entry schema contract tests

---

### 8. Test Execution Optimization

**Current**: Full test suite runs in ~20 seconds

**Optimization Opportunities:**
- [ ] Profile test execution to identify slowest tests
- [ ] Parallelize independent integration tests
- [ ] Reduce artificial delays where safe (`Task.Delay(100)`)
- [ ] Use test collections to control parallelization
- [ ] Add `[assembly: CollectionBehavior(MaxParallelThreads = 4)]`
- [ ] Split tests into Fast/Slow categories for PR validation

**Target**: <10 seconds for full suite

---

### 9. Test Flakiness Investigation

**Potentially Flaky Tests** (need monitoring):
- [ ] `DebugTerminalNavigationTests` - UI timing dependent
- [ ] `PerformanceMonitoringIntegrationTests` - snapshot timing
- [ ] `BootSequenceTests` - async initialization timing
- [ ] `ThemeTests` - resource loading timing

**Mitigation Actions:**
- [ ] Add retry logic to timing-dependent assertions
- [ ] Use `Avalonia.Threading.Dispatcher.UIThread.RunJobs()` instead of delays
- [ ] Add test result tracking to identify flaky tests

---

### 10. Mock Verification Gaps

**Issue**: Many tests mock dependencies but don't verify interactions

**Action Items:**
- [ ] Audit all tests using NSubstitute mocks
- [ ] Add `Received()` verification where appropriate
- [ ] Add `DidNotReceive()` verification for negative cases
- [ ] Document when to verify vs when it's not needed

---

## 📊 Test Metrics & Goals

### Current Coverage
- **Total Tests**: 860 (858 passing, 2 skipped)
- **Unit Tests**: 753 (87.6%)
- **Integration Tests**: 90 (10.5%)
- **Contract Tests**: 15 (1.7%)
- **Performance Tests**: 2 (0.2%)

### Coverage Goals
- [ ] Achieve 85%+ code coverage (line coverage)
- [ ] 90%+ coverage for Services layer
- [ ] 95%+ coverage for ViewModels
- [ ] 100% coverage for critical paths (boot, auth, data persistence)

### Test Distribution Goals
- [ ] Increase integration tests to 15% (target: 130 tests)
- [ ] Increase contract tests to 5% (target: 40 tests)
- [ ] Increase performance tests to 3% (target: 25 tests)

---

## 🔧 Technical Debt

### Test Code Duplication
- [ ] Extract common ViewModel test setup into `ViewModelTestBase<T>`
- [ ] Extract common Service test setup into `ServiceTestBase<T>`
- [ ] Create shared fixture classes for database, configuration, secrets
- [ ] Create test utility methods in `TestHelpers/TestUtilities.cs`

### Hardcoded Test Values
- [ ] Create `TestHelpers/TestConstants.cs` for magic numbers/strings
- [ ] Move all magic values to constants
- [ ] Group constants by domain (Boot, Config, Database, etc.)

### Test Organization
- [ ] Move Avalonia UI tests requiring rendering to integration/
- [ ] Move pure logic tests to unit/
- [ ] Create performance/ subdirectories for categories
- [ ] Standardize test file naming (all end with `Tests.cs`)

---

## 🚀 CI/CD Integration

### Proposed Tiered Test Strategy

**PR Validation (fast feedback - <5s):**
```powershell
dotnet test --filter "Category=Unit|Category=Critical"
```

**Merge to Main (comprehensive - <20s):**
```powershell
dotnet test --filter "Category!=Performance"
```

**Nightly Build (exhaustive - <5min):**
```powershell
dotnet test  # All tests including Performance
```

**Action Items:**
- [ ] Add `[Trait("Category", "Critical")]` to must-pass tests
- [ ] Create GitHub Actions workflow with tiered strategy
- [ ] Add test result publishing to PR comments
- [ ] Add coverage badge to README
- [ ] Set up test failure notifications

---

## ✅ Recently Completed (October 12, 2025)

- [x] Fixed CA1416 platform warnings (WindowsSecretsService in LogRedactionContractTests)
  - Created `tests/TestHelpers/WindowsOnlyFactAttribute.cs` for reusable platform-specific tests
  - Applied `[WindowsOnlyFact]` with platform guard to `SecretsService_NeverLogsCredentialValues` test
  - Zero build warnings achieved (was 5 CA1416 warnings)
  - Platform-specific test pattern established for future Android/iOS tests
- [x] Fixed 2 ConvertBack test failures (Color vs SolidColorBrush in test inputs)
- [x] Fixed xUnit1031 warnings (9 test methods converted to async Task pattern)
  - `tests/contract/FeatureFlagEvaluatorContractTests.cs` (2 methods)
  - `tests/contract/LogRedactionContractTests.cs` (7 methods)
  - Replaced all `.Result` and `.Wait()` with `await`
- [x] Fixed 22 Avalonia converter test assertions (Color vs SolidColorBrush)
- [x] Fixed 20 Avalonia converter threading issues
- [x] Fixed 4 Debug Terminal AXAML control name issues
- [x] Fixed 1 Performance Monitoring cancellation handling
- [x] Reduced test failures from 27 to 0
- [x] Success Rate: 100% (excluding intentionally skipped tests)

---

## 📞 Quick Reference

```powershell
# Run all tests
dotnet test

# Run by category
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
dotnet test --filter "Category=Contract"
dotnet test --filter "Category=Performance"

# Run specific test class
dotnet test --filter "FullyQualifiedName~BootOrchestratorTests"

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageOutputFormat=cobertura

# Watch mode
dotnet watch test --filter "Category=Unit"
```

---

**Last Review**: October 12, 2025  
**Next Review**: Monthly or when test count exceeds 1000
