---
description: 'xUnit async test patterns and xUnit1031 warning resolution for blocking operations in test methods'
applyTo: '**/tests/**/*.cs'
---

# Async Test Patterns for xUnit

## Overview

This instruction file defines patterns for writing proper async xUnit tests and resolving xUnit1031 analyzer warnings that occur when tests use blocking operations (`.Result`, `.Wait()`) on async code. Blocking operations in tests can cause deadlocks and poor test performance.

## Core Principles

### Why Async Tests Matter

1. **Prevent Deadlocks**: Blocking on async code can cause deadlocks, especially in UI frameworks
2. **Better Performance**: Async tests allow test runner to utilize threads efficiently
3. **Real-World Accuracy**: Tests should mirror production async patterns
4. **Analyzer Compliance**: xUnit 2.4+ warns about blocking operations (xUnit1031)

### When to Use Async Tests

✅ **Use async tests when**:
- Testing services that have async methods (`*Async()`)
- Testing database operations (all should be async)
- Testing API calls or network operations
- Testing file I/O operations
- Testing any operation with `CancellationToken` parameter

❌ **Use synchronous tests when**:
- Testing pure computational logic (no I/O)
- Testing synchronous ViewModels property setters
- Testing value converters
- Testing validation logic that's purely synchronous

## Pattern 1: Basic Async Test Conversion

### Before: Blocking Test (xUnit1031 Warning)

```csharp
[Fact]
public void MyTest_BlockingOperation()
{
    // Arrange
    var service = new MyService();
    
    // Act - WRONG: Blocking on async operation
    var result = service.GetDataAsync().Result;  // ⚠️ xUnit1031 warning
    
    // Assert
    result.Should().NotBeNull();
}
```

**Problems**:
- `.Result` blocks the test thread
- Can cause deadlocks in SynchronizationContext scenarios
- xUnit1031 analyzer warning
- Poor test performance

### After: Proper Async Test

```csharp
[Fact]
public async Task MyTest_ProperAsync()
{
    // Arrange
    var service = new MyService();
    
    // Act - CORRECT: Awaiting async operation
    var result = await service.GetDataAsync();
    
    // Assert
    result.Should().NotBeNull();
}
```

**Benefits**:
- No blocking
- No deadlock risk
- No analyzer warnings
- Better test runner efficiency

## Pattern 2: Async Theory Tests

### Parameterized Async Tests

```csharp
[Theory]
[InlineData("value1")]
[InlineData("value2")]
[InlineData("value3")]
public async Task MyTest_WithParameters(string testValue)
{
    // Arrange
    var service = new MyService();
    
    // Act
    var result = await service.ProcessAsync(testValue);
    
    // Assert
    result.Should().Be(expected);
}
```

### Member Data Async Tests

```csharp
public static IEnumerable<object[]> TestCases =>
    new List<object[]>
    {
        new object[] { "case1", 100 },
        new object[] { "case2", 200 },
        new object[] { "case3", 300 }
    };

[Theory]
[MemberData(nameof(TestCases))]
public async Task MyTest_WithMemberData(string input, int expected)
{
    var service = new MyService();
    var result = await service.CalculateAsync(input);
    result.Should().Be(expected);
}
```

## Pattern 3: CancellationToken in Tests

### Default CancellationToken Pattern

```csharp
[Fact]
public async Task MyTest_WithDefaultCancellationToken()
{
    // Arrange
    var service = new MyService();
    
    // Act - Use CancellationToken.None for default
    var result = await service.GetDataAsync(CancellationToken.None);
    
    // Assert
    result.Should().NotBeNull();
}
```

### Testing Cancellation

```csharp
[Fact]
public async Task MyTest_CancelsOperation()
{
    // Arrange
    var service = new MyService();
    var cts = new CancellationTokenSource();
    
    // Act - Start operation then cancel
    var task = service.LongRunningOperationAsync(cts.Token);
    cts.Cancel();
    
    // Assert - Should throw OperationCanceledException
    await task.Invoking(t => t)
        .Should()
        .ThrowAsync<OperationCanceledException>();
}
```

### Testing Cancellation Token Propagation

```csharp
[Fact]
public async Task MyTest_PropagatesCancellationToken()
{
    // Arrange
    var mockService = Substitute.For<IDataService>();
    var service = new MyService(mockService);
    var cts = new CancellationTokenSource();
    cts.Cancel();
    
    // Act
    await service.GetDataAsync(cts.Token);
    
    // Assert - Verify cancellation token was passed through
    await mockService.Received(1).FetchDataAsync(Arg.Is<CancellationToken>(ct => ct.IsCancellationRequested));
}
```

## Pattern 4: Converting Blocking Code to Async

### Pattern 4a: Replacing `.Result`

```csharp
// ❌ Before: Blocking with .Result
[Fact]
public void LoadData_ReturnsExpectedItems()
{
    var service = new InventoryService();
    var items = service.GetItemsAsync().Result;  // xUnit1031 warning
    items.Should().HaveCount(10);
}

// ✅ After: Proper async/await
[Fact]
public async Task LoadData_ReturnsExpectedItems()
{
    var service = new InventoryService();
    var items = await service.GetItemsAsync();
    items.Should().HaveCount(10);
}
```

### Pattern 4b: Replacing `.Wait()`

```csharp
// ❌ Before: Blocking with .Wait()
[Fact]
public void SaveData_UpdatesDatabase()
{
    var service = new InventoryService();
    var task = service.SaveItemAsync(item);
    task.Wait();  // xUnit1031 warning
    
    // Verify save occurred
}

// ✅ After: Proper async/await
[Fact]
public async Task SaveData_UpdatesDatabase()
{
    var service = new InventoryService();
    await service.SaveItemAsync(item);
    
    // Verify save occurred
}
```

### Pattern 4c: Replacing `.GetAwaiter().GetResult()`

```csharp
// ❌ Before: Blocking with GetAwaiter().GetResult()
[Fact]
public void ProcessData_CompletesSuccessfully()
{
    var service = new DataService();
    var result = service.ProcessAsync().GetAwaiter().GetResult();  // xUnit1031 warning
    result.Should().BeTrue();
}

// ✅ After: Proper async/await
[Fact]
public async Task ProcessData_CompletesSuccessfully()
{
    var service = new DataService();
    var result = await service.ProcessAsync();
    result.Should().BeTrue();
}
```

## Pattern 5: Multiple Async Operations in Tests

### Sequential Operations

```csharp
[Fact]
public async Task MultipleOperations_Sequential()
{
    var service = new MyService();
    
    // Operations that depend on each other
    var item1 = await service.CreateAsync("Item1");
    var item2 = await service.CreateAsync("Item2");
    var combined = await service.CombineAsync(item1, item2);
    
    combined.Should().NotBeNull();
}
```

### Parallel Operations (Independent)

```csharp
[Fact]
public async Task MultipleOperations_Parallel()
{
    var service = new MyService();
    
    // Operations that don't depend on each other
    var task1 = service.GetDataAsync("source1");
    var task2 = service.GetDataAsync("source2");
    var task3 = service.GetDataAsync("source3");
    
    // Wait for all to complete
    await Task.WhenAll(task1, task2, task3);
    
    // Assert on results
    task1.Result.Should().NotBeNull();
    task2.Result.Should().NotBeNull();
    task3.Result.Should().NotBeNull();
}
```

### Task.WhenAll Pattern for Efficiency

```csharp
[Fact]
public async Task MultipleServices_ParallelExecution()
{
    var service1 = new Service1();
    var service2 = new Service2();
    var service3 = new Service3();
    
    // Execute all services in parallel
    var results = await Task.WhenAll(
        service1.GetDataAsync(),
        service2.GetDataAsync(),
        service3.GetDataAsync()
    );
    
    results[0].Should().NotBeNull();
    results[1].Should().NotBeNull();
    results[2].Should().NotBeNull();
}
```

## Pattern 6: Testing Async Void Methods

### Problem: Async Void

```csharp
// ⚠️ Problematic: async void (cannot be awaited)
public async void ProcessDataAsync()
{
    await _service.ProcessAsync();
}
```

### Solution: Return Task

```csharp
// ✅ Correct: Return Task (can be awaited in tests)
public async Task ProcessDataAsync()
{
    await _service.ProcessAsync();
}

// Test
[Fact]
public async Task ProcessData_CompletesSuccessfully()
{
    var viewModel = new MyViewModel();
    await viewModel.ProcessDataAsync();  // Can await
    viewModel.IsProcessing.Should().BeFalse();
}
```

## Pattern 7: FluentAssertions with Async

### Async Should() Extensions

```csharp
[Fact]
public async Task ServiceCall_ShouldThrowException()
{
    var service = new MyService();
    
    // FluentAssertions async assertion
    await service.Invoking(s => s.InvalidOperationAsync())
        .Should()
        .ThrowAsync<InvalidOperationException>();
}
```

### Async Should().CompleteWithinAsync()

```csharp
[Fact]
public async Task ServiceCall_ShouldCompleteQuickly()
{
    var service = new MyService();
    
    // Verify operation completes within timeout
    await service.Invoking(s => s.GetDataAsync())
        .Should()
        .CompleteWithinAsync(TimeSpan.FromSeconds(5));
}
```

### Async Should().NotThrowAsync()

```csharp
[Fact]
public async Task ServiceCall_ShouldNotThrowException()
{
    var service = new MyService();
    
    await service.Invoking(s => s.SafeOperationAsync())
        .Should()
        .NotThrowAsync();
}
```

## Pattern 8: Async Setup and Teardown

### IAsyncLifetime Interface

```csharp
public class DatabaseTests : IAsyncLifetime
{
    private DatabaseConnection _connection;
    
    // Async initialization
    public async Task InitializeAsync()
    {
        _connection = new DatabaseConnection();
        await _connection.OpenAsync();
        await SeedTestDataAsync();
    }
    
    // Async cleanup
    public async Task DisposeAsync()
    {
        await CleanupTestDataAsync();
        await _connection.CloseAsync();
    }
    
    [Fact]
    public async Task DatabaseQuery_ReturnsExpectedResults()
    {
        var results = await _connection.QueryAsync("SELECT * FROM TestTable");
        results.Should().HaveCount(10);
    }
}
```

### Async Constructor Pattern (Collection Fixture)

```csharp
public class DatabaseFixture : IAsyncLifetime
{
    public DatabaseConnection Connection { get; private set; }
    
    public async Task InitializeAsync()
    {
        Connection = new DatabaseConnection();
        await Connection.OpenAsync();
    }
    
    public async Task DisposeAsync()
    {
        await Connection.CloseAsync();
    }
}

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}

[Collection("Database")]
public class MyDatabaseTests
{
    private readonly DatabaseFixture _fixture;
    
    public MyDatabaseTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task QueryDatabase_ReturnsResults()
    {
        var results = await _fixture.Connection.QueryAsync("SELECT * FROM Users");
        results.Should().NotBeEmpty();
    }
}
```

## Pattern 9: Testing Async Loops

### Async Foreach Pattern

```csharp
[Fact]
public async Task ProcessItems_InAsyncLoop()
{
    var service = new BatchService();
    var items = new[] { "item1", "item2", "item3" };
    var results = new List<string>();
    
    // Process each item asynchronously
    foreach (var item in items)
    {
        var result = await service.ProcessAsync(item);
        results.Add(result);
    }
    
    results.Should().HaveCount(3);
}
```

### Async Parallel Loop

```csharp
[Fact]
public async Task ProcessItems_InParallel()
{
    var service = new BatchService();
    var items = new[] { "item1", "item2", "item3" };
    
    // Process all items in parallel
    var tasks = items.Select(item => service.ProcessAsync(item));
    var results = await Task.WhenAll(tasks);
    
    results.Should().HaveCount(3);
}
```

## Common Pitfalls to Avoid

### Pitfall 1: Using `async void` Test Methods

```csharp
// ❌ WRONG: async void cannot be awaited properly
[Fact]
public async void MyTest_WrongSignature()
{
    await service.GetDataAsync();
}

// ✅ CORRECT: async Task
[Fact]
public async Task MyTest_CorrectSignature()
{
    await service.GetDataAsync();
}
```

**Why**: xUnit cannot track async void test completion, leading to race conditions and unreliable tests.

---

### Pitfall 2: Forgetting to Await

```csharp
// ❌ WRONG: Not awaiting async operation
[Fact]
public async Task MyTest_NoAwait()
{
    var task = service.GetDataAsync();  // Warning: No await
    // Test completes before operation finishes
}

// ✅ CORRECT: Awaiting the operation
[Fact]
public async Task MyTest_WithAwait()
{
    var result = await service.GetDataAsync();
    result.Should().NotBeNull();
}
```

---

### Pitfall 3: Mixing Async and Blocking

```csharp
// ❌ WRONG: Mixing async and blocking calls
[Fact]
public async Task MyTest_MixedPattern()
{
    var result1 = await service.GetDataAsync();
    var result2 = service.GetMoreDataAsync().Result;  // Blocking!
    var result3 = await service.GetEvenMoreDataAsync();
}

// ✅ CORRECT: All async
[Fact]
public async Task MyTest_AllAsync()
{
    var result1 = await service.GetDataAsync();
    var result2 = await service.GetMoreDataAsync();
    var result3 = await service.GetEvenMoreDataAsync();
}
```

---

### Pitfall 4: Not Handling Task Exceptions

```csharp
// ❌ WRONG: Swallowing exception
[Fact]
public async Task MyTest_SwallowsException()
{
    try
    {
        await service.FailingOperationAsync();
    }
    catch
    {
        // Test passes even though operation failed!
    }
}

// ✅ CORRECT: Verifying expected exception
[Fact]
public async Task MyTest_VerifiesException()
{
    await service.Invoking(s => s.FailingOperationAsync())
        .Should()
        .ThrowAsync<InvalidOperationException>();
}
```

## xUnit1031 Resolution Checklist

When you see xUnit1031 warning, follow this checklist:

- [ ] 1. Change test method signature from `void` to `async Task`
- [ ] 2. Replace all `.Result` with `await`
- [ ] 3. Replace all `.Wait()` with `await`
- [ ] 4. Replace all `.GetAwaiter().GetResult()` with `await`
- [ ] 5. Add `CancellationToken.None` where CancellationToken is required
- [ ] 6. Rebuild and verify warnings are gone
- [ ] 7. Run tests to ensure they still pass

## Real-World Example: Complete Conversion

### Before: Multiple xUnit1031 Warnings

```csharp
[Fact]
public void FeatureFlag_EvaluatesCorrectly()
{
    // Arrange
    var evaluator = new FeatureFlagEvaluator(logger, repository);
    var flag = new FeatureFlag 
    { 
        FlagName = "Test.Feature", 
        IsEnabled = true,
        RolloutPercentage = 100
    };
    evaluator.RegisterFlag(flag).Wait();  // xUnit1031 warning
    
    // Act
    var isEnabled = evaluator.IsEnabledAsync("Test.Feature").Result;  // xUnit1031 warning
    
    // Assert
    isEnabled.Should().BeTrue();
}
```

### After: Fully Async

```csharp
[Fact]
public async Task FeatureFlag_EvaluatesCorrectly()
{
    // Arrange
    var evaluator = new FeatureFlagEvaluator(logger, repository);
    var flag = new FeatureFlag 
    { 
        FlagName = "Test.Feature", 
        IsEnabled = true,
        RolloutPercentage = 100
    };
    await evaluator.RegisterFlag(flag);
    
    // Act
    var isEnabled = await evaluator.IsEnabledAsync("Test.Feature");
    
    // Assert
    isEnabled.Should().BeTrue();
}
```

**Changes Made**:
1. ✅ `void` → `async Task`
2. ✅ `.Wait()` → `await`
3. ✅ `.Result` → `await`
4. ✅ Zero warnings
5. ✅ Better test performance

## Performance Considerations

### Async Tests Are Faster

```csharp
// Synchronous blocking: Total time = sum of all operations
[Fact]
public void ThreeOperations_Sequential()
{
    service.Operation1Async().Wait();  // 1 second
    service.Operation2Async().Wait();  // 1 second
    service.Operation3Async().Wait();  // 1 second
    // Total: 3 seconds
}

// Asynchronous parallel: Total time = longest operation
[Fact]
public async Task ThreeOperations_Parallel()
{
    await Task.WhenAll(
        service.Operation1Async(),  // 1 second
        service.Operation2Async(),  // 1 second
        service.Operation3Async()   // 1 second
    );
    // Total: 1 second (all run in parallel)
}
```

## Memory File Maintenance

**Last Updated**: October 12, 2025  
**Maintainer**: GitHub Copilot (via user input)

**How to Use This Guide**:
1. When fixing xUnit1031 warnings, follow the conversion checklist
2. Apply patterns consistently across all test files
3. Update tests/TODO.md when bulk conversions are completed
4. Document any new async patterns discovered

**Review Frequency**: When xUnit version is updated or new async patterns emerge
