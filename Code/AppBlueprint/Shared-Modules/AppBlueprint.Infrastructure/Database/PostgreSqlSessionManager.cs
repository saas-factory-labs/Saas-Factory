using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AppBlueprint.Infrastructure.Database;

/// <summary>
/// Manages PostgreSQL session variables safely without raw SQL.
/// Provides strongly-typed methods for setting RLS session variables.
/// </summary>
public sealed class PostgreSqlSessionManager
{
    private readonly DbContext _dbContext;

    public PostgreSqlSessionManager(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Sets the current tenant ID session variable for Row-Level Security.
    /// </summary>
    /// <param name="tenantId">The tenant ID to set.</param>
    public async Task SetTenantIdAsync(string tenantId)
    {
        ArgumentException.ThrowIfNullOrEmpty(tenantId);

        // Use parameterized connection to set session variable
        NpgsqlConnection? connection = _dbContext.Database.GetDbConnection() as NpgsqlConnection ?? throw new InvalidOperationException("Database connection is not a PostgreSQL connection");

        // Ensure connection is open
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        // Create parameterized command
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = "SELECT set_config('app.current_tenant_id', $1, FALSE)";
        command.Parameters.AddWithValue(tenantId);

        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Sets the admin flag session variable for Row-Level Security.
    /// This allows admin users to bypass tenant filtering for SELECT operations.
    /// </summary>
    /// <param name="isAdmin">Whether the current user is an admin.</param>
    public async Task SetAdminFlagAsync(bool isAdmin)
    {
        NpgsqlConnection? connection = _dbContext.Database.GetDbConnection() as NpgsqlConnection ?? throw new InvalidOperationException("Database connection is not a PostgreSQL connection");
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = "SELECT set_config('app.is_admin', $1, FALSE)";
        command.Parameters.AddWithValue(isAdmin ? "true" : "false");

        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Clears all RLS session variables by resetting them to empty strings.
    /// Should be called in finally blocks to prevent session variable leakage.
    /// </summary>
    public async Task ClearSessionVariablesAsync()
    {
        if (_dbContext.Database.GetDbConnection() is not NpgsqlConnection connection)
            return; // Not a PostgreSQL connection, nothing to clear

        if (connection.State != System.Data.ConnectionState.Open)
            return; // Connection closed, session variables already cleared

        try
        {
            await using NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"
                SELECT set_config('app.current_tenant_id', '', FALSE);
                SELECT set_config('app.is_admin', 'false', FALSE);
            ";

            await command.ExecuteNonQueryAsync();
        }
        catch
        {
            // Ignore errors during cleanup
        }
    }

    /// <summary>
    /// Sets multiple session variables atomically in a single command.
    /// Useful for admin tenant access where both tenant ID and admin flag need to be set.
    /// </summary>
    /// <param name="tenantId">The tenant ID to set.</param>
    /// <param name="isAdmin">Whether the current user is an admin.</param>
    public async Task SetSessionVariablesAsync(string tenantId, bool isAdmin)
    {
        ArgumentException.ThrowIfNullOrEmpty(tenantId);

        NpgsqlConnection? connection = _dbContext.Database.GetDbConnection() as NpgsqlConnection ?? throw new InvalidOperationException("Database connection is not a PostgreSQL connection");
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        // Set both variables in a single command for atomicity
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT set_config('app.current_tenant_id', $1, FALSE);
            SELECT set_config('app.is_admin', $2, FALSE);
        ";
        command.Parameters.AddWithValue(tenantId);
        command.Parameters.AddWithValue(isAdmin ? "true" : "false");

        await command.ExecuteNonQueryAsync();
    }
}
