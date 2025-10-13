using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Xunit;

namespace MTM_Template_Tests.TestHelpers;

/// <summary>
/// Shared database fixture for integration tests requiring database access.
/// Ensures database is available and properly configured before tests run.
/// Implements IAsyncLifetime for async setup/cleanup.
/// </summary>
/// <remarks>
/// Uses collection fixture pattern to share database connection across test classes.
/// Tests requiring database access should use: [Collection("DatabaseTests")]
/// 
/// Connection: localhost:3306/mtm_template_dev (MAMP MySQL 5.7)
/// Test Users: UserId=42 (testuser), UserId=99 (integrationtest)
/// </remarks>
public class DatabaseFixture : IAsyncLifetime
{
    /// <summary>
    /// Database connection string for local MAMP MySQL
    /// </summary>
    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    /// Indicates whether database is available and tests can run
    /// </summary>
    public bool IsAvailable { get; private set; }

    /// <summary>
    /// Error message if database is not available
    /// </summary>
    public string? UnavailableReason { get; private set; }

    /// <summary>
    /// Test user ID (testuser)
    /// </summary>
    public const int TestUserId = 42;

    /// <summary>
    /// Integration test user ID (integrationtest)
    /// </summary>
    public const int IntegrationTestUserId = 99;

    public async Task InitializeAsync()
    {
        // Build connection string
        ConnectionString = BuildConnectionString();

        // Test database availability
        try
        {
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync(CancellationToken.None);

            // Verify test users exist
            await VerifyTestUsersExist(connection);

            IsAvailable = true;
            UnavailableReason = null;
        }
        catch (MySqlException ex)
        {
            IsAvailable = false;
            UnavailableReason = $"MySQL connection failed: {ex.Message}. Ensure MAMP MySQL is running.";
        }
        catch (Exception ex)
        {
            IsAvailable = false;
            UnavailableReason = $"Database initialization failed: {ex.Message}";
        }
    }

    public Task DisposeAsync()
    {
        // No cleanup needed - fixture is shared across all tests
        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates a new database connection for test use.
    /// Connection should be disposed by caller.
    /// </summary>
    public MySqlConnection CreateConnection()
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException($"Database not available: {UnavailableReason}");
        }

        return new MySqlConnection(ConnectionString);
    }

    /// <summary>
    /// Begins a transaction for test isolation.
    /// Transaction will be rolled back in test cleanup to avoid affecting other tests.
    /// </summary>
    public async Task<(MySqlConnection connection, MySqlTransaction transaction)> BeginTransactionAsync()
    {
        var connection = CreateConnection();
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        return (connection, transaction);
    }

    private string BuildConnectionString()
    {
        // Read from environment or use defaults
        var host = Environment.GetEnvironmentVariable("MTM_DATABASE_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("MTM_DATABASE_PORT") ?? "3306";
        var database = Environment.GetEnvironmentVariable("MTM_DATABASE_NAME") ?? "mtm_template_dev";
        var user = Environment.GetEnvironmentVariable("MTM_DATABASE_USER") ?? "root";
        var password = Environment.GetEnvironmentVariable("MTM_DATABASE_PASSWORD") ?? "root";

        return $"Server={host};Port={port};Database={database};Uid={user};Pwd={password};" +
               "SslMode=none;AllowPublicKeyRetrieval=true;ConnectionTimeout=5;";
    }

    private async Task VerifyTestUsersExist(MySqlConnection connection)
    {
        const string query = "SELECT COUNT(*) FROM Users WHERE UserId IN (42, 99)";
        await using var command = new MySqlCommand(query, connection);

        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
        if (count < 2)
        {
            throw new InvalidOperationException(
                "Test users not found. Run migration: config/migrations/001_initial_schema.sql");
        }
    }
}

/// <summary>
/// Collection definition for database tests.
/// Apply [Collection("DatabaseTests")] to test classes that need database access.
/// </summary>
[CollectionDefinition("DatabaseTests")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionFixture<>] and all the
    // ICollectionFixture<> interfaces.
}
