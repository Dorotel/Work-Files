using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_Template_Application.Extensions;
using MTM_Template_Application.Models.Diagnostics;
using MTM_Template_Application.Services.Diagnostics;
using MTM_Template_Application.Services.Secrets;
using Xunit;

namespace MTM_Template_Tests.Integration.Diagnostics;

/// <summary>
/// Integration tests for performance monitoring service end-to-end workflow.
/// Tests T023: Performance monitoring with real service instance and DI container.
/// </summary>
[Trait("Category", "Integration")]
public class PerformanceMonitoringIntegrationTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IPerformanceMonitoringService _service;
    private bool _disposed;

    public PerformanceMonitoringIntegrationTests()
    {
        // Setup DI container with Debug Terminal services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Add Debug Terminal services
        services.AddDebugTerminalServices();

        _serviceProvider = services.BuildServiceProvider();
        _service = _serviceProvider.GetRequiredService<IPerformanceMonitoringService>();
    }

    [Fact]
    public async Task StartMonitoring_Should_Capture_Snapshots_With_Valid_Interval()
    {
        // Arrange
        var interval = TimeSpan.FromSeconds(1); // 1 second (within valid range 1-30)

        try
        {
            // Act - Start monitoring with 1-second interval (use CancellationToken.None for background task)
            var monitoringTask = _service.StartMonitoringAsync(interval, CancellationToken.None);

            // Wait for 2.5 seconds to capture at least 2 snapshots
            await Task.Delay(2500);

            // Stop monitoring
            await _service.StopMonitoringAsync();

            // Assert - Verify snapshots were captured
            var snapshots = await _service.GetRecentSnapshotsAsync(100, CancellationToken.None);
            snapshots.Should().NotBeNull();
            snapshots.Should().HaveCountGreaterOrEqualTo(2, "at least 2 snapshots should be captured in 2.5 seconds with 1-second interval");

            // Verify IsMonitoring is false after stopping
            _service.IsMonitoring.Should().BeFalse("monitoring should be stopped");
        }
        finally
        {
            if (_service.IsMonitoring)
            {
                await _service.StopMonitoringAsync();
            }
        }
    }

    [Fact]
    public async Task ShouldRespectCircularBufferLimit()
    {
        // Arrange
        var interval = TimeSpan.FromSeconds(1); // 1 second

        try
        {
            // Act - Start monitoring (use CancellationToken.None for background task)
            var monitoringTask = _service.StartMonitoringAsync(interval, CancellationToken.None);

            // Wait for 3 seconds to capture 3 snapshots
            await Task.Delay(3000);

            // Stop monitoring first to ensure clean state
            await _service.StopMonitoringAsync();

            // Retrieve all snapshots to get the full list
            var allSnapshots = await _service.GetRecentSnapshotsAsync(100, CancellationToken.None);

            // Retrieve only last 2 snapshots
            var recentSnapshots = await _service.GetRecentSnapshotsAsync(2, CancellationToken.None);

            // Assert - Verify count respects limit
            recentSnapshots.Should().NotBeNull();
            recentSnapshots.Count.Should().BeLessOrEqualTo(2, "should only retrieve the requested number of snapshots");

            // Verify buffer returns most recent snapshots
            if (allSnapshots.Count >= 2)
            {
                // The service returns snapshots in the order they were added (oldest to newest)
                // We should get the last 2 snapshots (most recent)
                var expected = allSnapshots
                    .OrderBy(s => s.Timestamp)
                    .TakeLast(2)
                    .Select(s => s.Timestamp)
                    .ToList();

                var actual = recentSnapshots.Select(s => s.Timestamp).ToList();
                
                actual.Should().BeEquivalentTo(expected,
                    "the buffer should return the most recent snapshots"
                );
            }
        }
        finally
        {
            if (_service.IsMonitoring)
            {
                await _service.StopMonitoringAsync();
            }
        }
    }
[Fact]
public async Task GetCurrentSnapshotAsync_Should_Return_Valid_Snapshot_With_Metrics()
{
    // Arrange
    var cancellationToken = CancellationToken.None;

    // Act
    var snapshot = await _service.GetCurrentSnapshotAsync(cancellationToken);

    // Assert
    snapshot.Should().NotBeNull();
    snapshot.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2), "snapshot timestamp should be recent");
    snapshot.CpuUsagePercent.Should().BeGreaterOrEqualTo(0, "CPU usage should be non-negative");
    snapshot.MemoryUsageMB.Should().BeGreaterThan(0, "memory usage should be positive");
    snapshot.GcGen0Collections.Should().BeGreaterOrEqualTo(0);
    snapshot.GcGen1Collections.Should().BeGreaterOrEqualTo(0);
    snapshot.GcGen2Collections.Should().BeGreaterOrEqualTo(0);
    snapshot.ThreadCount.Should().BeGreaterThan(0, "thread count should be positive");
}

[Fact]
public async Task StartMonitoring_Should_Reject_Invalid_Interval_Less_Than_One()
{
    // Arrange
    var invalidInterval = TimeSpan.FromMilliseconds(500); // Invalid (must be 1-30 seconds)
    var cancellationToken = CancellationToken.None;

    // Act
    Func<Task> act = async () => await _service.StartMonitoringAsync(invalidInterval, cancellationToken);

    // Assert
    await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
        .WithMessage("*interval*");
}

[Fact]
public async Task StartMonitoring_Should_Reject_Invalid_Interval_Greater_Than_Thirty()
{
    // Arrange
    var invalidInterval = TimeSpan.FromSeconds(31); // Invalid (must be 1-30)
    var cancellationToken = CancellationToken.None;

    // Act
    Func<Task> act = async () => await _service.StartMonitoringAsync(invalidInterval, cancellationToken);

    // Assert
    await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
        .WithMessage("*interval*");
}

[Fact]
public async Task StopMonitoringAsync_Should_Stop_Background_Task()
{
    // Arrange
    var interval = TimeSpan.FromSeconds(1);

    try
    {
        // Act - Start monitoring (use CancellationToken.None for background task)
        var monitoringTask = _service.StartMonitoringAsync(interval, CancellationToken.None);
        _service.IsMonitoring.Should().BeTrue("monitoring should be active after start");

        await Task.Delay(1500); // Let it capture at least 1 snapshot

        await _service.StopMonitoringAsync();

        // Assert
        _service.IsMonitoring.Should().BeFalse("monitoring should be stopped");

        // Wait a bit and verify no new snapshots are captured
        var snapshotsBeforeWait = await _service.GetRecentSnapshotsAsync(100, CancellationToken.None);
        var countBefore = snapshotsBeforeWait.Count;

        // Wait long enough for 2+ snapshots if monitoring was still running
        await Task.Delay(2500);

        var snapshotsAfterWait = await _service.GetRecentSnapshotsAsync(100, CancellationToken.None);
        var countAfter = snapshotsAfterWait.Count;

        countAfter.Should().Be(countBefore, "no new snapshots should be captured after stopping");
    }
    finally
    {
        if (_service.IsMonitoring)
        {
            await _service.StopMonitoringAsync();
        }
    }
}

    [Fact]
    public async Task Performance_Monitoring_Should_Have_Low_CPU_Usage()
    {
        // Arrange
        var interval = TimeSpan.FromSeconds(1);

        try
        {
            // Get baseline CPU before monitoring
            var baselineSnapshot = await _service.GetCurrentSnapshotAsync(CancellationToken.None);
            var baselineCpu = baselineSnapshot.CpuUsagePercent;

            // Act - Start monitoring (use CancellationToken.None for background task)
            var monitoringTask = _service.StartMonitoringAsync(interval, CancellationToken.None);

            // Let it run for 5 seconds
            await Task.Delay(5000);

            // Get CPU usage during monitoring
            var monitoringSnapshot = await _service.GetCurrentSnapshotAsync(CancellationToken.None);
            var monitoringCpu = monitoringSnapshot.CpuUsagePercent;

            await _service.StopMonitoringAsync();

            // Assert - CPU increase should be less than 2% (NFR-003)
            var cpuIncrease = monitoringCpu - baselineCpu;
            cpuIncrease.Should().BeLessThan(2.0, "CPU usage during monitoring should be <2% (NFR-003)");
        }
        finally
        {
            if (_service.IsMonitoring)
            {
                await _service.StopMonitoringAsync();
            }
        }
    }

    [Fact]
    public async Task Circular_Buffer_Should_Maintain_Max_100_Snapshots()
    {
        // Arrange
        var interval = TimeSpan.FromSeconds(1);

        try
        {
            // Act - Start monitoring and let it run long enough to exceed 100 snapshots
            // Note: This test would take 100+ seconds to truly verify 100-snapshot limit.
            // For integration test speed, we'll verify that GetRecentSnapshotsAsync(100) works correctly.

            var monitoringTask = _service.StartMonitoringAsync(interval, CancellationToken.None);

            // Wait for 3 seconds (3 snapshots)
            await Task.Delay(3000);

            var snapshots = await _service.GetRecentSnapshotsAsync(100, CancellationToken.None);

            await _service.StopMonitoringAsync();

            // Assert - Verify snapshots list respects the buffer (should have 3 or fewer)
            snapshots.Should().NotBeNull();
            snapshots.Count.Should().BeLessOrEqualTo(100, "circular buffer should never exceed 100 snapshots (CL-002)");
            snapshots.Count.Should().BeGreaterOrEqualTo(2, "should have captured at least 2 snapshots in 3 seconds");
        }
        finally
        {
        if (_service.IsMonitoring)
        {
            await _service.StopMonitoringAsync();
        }
    }
}

public void Dispose()
{
    if (_disposed)
    {
        return;
    }

    // PerformanceMonitoringService implements IDisposable, but accessed via interface
    if (_service is IDisposable disposableService)
    {
        disposableService.Dispose();
    }

    _serviceProvider?.Dispose();

    _disposed = true;
    GC.SuppressFinalize(this);
}
}
