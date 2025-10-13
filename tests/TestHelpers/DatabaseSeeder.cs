using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MTM_Template_Tests.TestHelpers;

/// <summary>
/// Helper class for seeding and cleaning up test data in the database.
/// Provides methods for creating predictable test data with automatic cleanup.
/// </summary>
public class DatabaseSeeder
{
    private readonly MySqlConnection _connection;
    private readonly MySqlTransaction? _transaction;
    private readonly List<(string table, int id)> _createdRecords = new();

    public DatabaseSeeder(MySqlConnection connection, MySqlTransaction? transaction = null)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _transaction = transaction;
    }

    #region User Preferences

    /// <summary>
    /// Seeds a user preference for testing.
    /// Automatically tracked for cleanup.
    /// </summary>
    public async Task<int> SeedUserPreferenceAsync(
        int userId,
        string preferenceKey,
        string preferenceValue,
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO UserPreferences (UserId, PreferenceKey, PreferenceValue, Category, LastUpdated)
            VALUES (@UserId, @PreferenceKey, @PreferenceValue, @Category, NOW())
            ON DUPLICATE KEY UPDATE
                PreferenceValue = @PreferenceValue,
                Category = @Category,
                LastUpdated = NOW();
            
            SELECT LAST_INSERT_ID();";

        await using var command = new MySqlCommand(sql, _connection, _transaction);
        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@PreferenceKey", preferenceKey);
        command.Parameters.AddWithValue("@PreferenceValue", preferenceValue);
        command.Parameters.AddWithValue("@Category", category ?? (object)DBNull.Value);

        var preferenceId = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));

        // Track for cleanup (if not using transaction)
        if (_transaction == null)
        {
            _createdRecords.Add(("UserPreferences", preferenceId));
        }

        return preferenceId;
    }

    /// <summary>
    /// Seeds multiple user preferences for testing.
    /// </summary>
    public async Task SeedUserPreferencesAsync(
        int userId,
        Dictionary<string, string> preferences,
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        foreach (var kvp in preferences)
        {
            await SeedUserPreferenceAsync(userId, kvp.Key, kvp.Value, category, cancellationToken);
        }
    }

    /// <summary>
    /// Gets a user preference value from the database.
    /// </summary>
    public async Task<string?> GetUserPreferenceAsync(
        int userId,
        string preferenceKey,
        CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT PreferenceValue FROM UserPreferences WHERE UserId = @UserId AND PreferenceKey = @PreferenceKey";

        await using var command = new MySqlCommand(sql, _connection, _transaction);
        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@PreferenceKey", preferenceKey);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result?.ToString();
    }

    #endregion

    #region Feature Flags

    /// <summary>
    /// Seeds a feature flag for testing.
    /// Automatically tracked for cleanup.
    /// </summary>
    public async Task<int> SeedFeatureFlagAsync(
        string flagName,
        bool isEnabled,
        string? environment = null,
        int rolloutPercentage = 100,
        string? appVersion = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO FeatureFlags (FlagName, IsEnabled, Environment, RolloutPercentage, AppVersion, UpdatedAt)
            VALUES (@FlagName, @IsEnabled, @Environment, @RolloutPercentage, @AppVersion, NOW())
            ON DUPLICATE KEY UPDATE
                IsEnabled = @IsEnabled,
                Environment = @Environment,
                RolloutPercentage = @RolloutPercentage,
                AppVersion = @AppVersion,
                UpdatedAt = NOW();
            
            SELECT LAST_INSERT_ID();";

        await using var command = new MySqlCommand(sql, _connection, _transaction);
        command.Parameters.AddWithValue("@FlagName", flagName);
        command.Parameters.AddWithValue("@IsEnabled", isEnabled);
        command.Parameters.AddWithValue("@Environment", environment ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@RolloutPercentage", rolloutPercentage);
        command.Parameters.AddWithValue("@AppVersion", appVersion ?? (object)DBNull.Value);

        var flagId = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));

        // Track for cleanup (if not using transaction)
        if (_transaction == null)
        {
            _createdRecords.Add(("FeatureFlags", flagId));
        }

        return flagId;
    }

    #endregion

    #region Cleanup

    /// <summary>
    /// Cleans up all seeded test data.
    /// Only needed when not using transactions (transactions auto-rollback).
    /// </summary>
    public async Task CleanupAsync(CancellationToken cancellationToken = default)
    {
        // Cleanup in reverse order of creation
        for (int i = _createdRecords.Count - 1; i >= 0; i--)
        {
            var (table, id) = _createdRecords[i];

            string sql = table switch
            {
                "UserPreferences" => "DELETE FROM UserPreferences WHERE PreferenceId = @Id",
                "FeatureFlags" => "DELETE FROM FeatureFlags WHERE FlagId = @Id",
                _ => throw new InvalidOperationException($"Unknown table: {table}")
            };

            await using var command = new MySqlCommand(sql, _connection, _transaction);
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        _createdRecords.Clear();
    }

    /// <summary>
    /// Deletes all user preferences for a specific user.
    /// Useful for test isolation.
    /// </summary>
    public async Task DeleteUserPreferencesAsync(int userId, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM UserPreferences WHERE UserId = @UserId";
        await using var command = new MySqlCommand(sql, _connection, _transaction);
        command.Parameters.AddWithValue("@UserId", userId);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a specific user preference.
    /// </summary>
    public async Task DeleteUserPreferenceAsync(
        int userId,
        string preferenceKey,
        CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM UserPreferences WHERE UserId = @UserId AND PreferenceKey = @PreferenceKey";
        await using var command = new MySqlCommand(sql, _connection, _transaction);
        command.Parameters.AddWithValue("@UserId", userId);
        command.Parameters.AddWithValue("@PreferenceKey", preferenceKey);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    #endregion
}
