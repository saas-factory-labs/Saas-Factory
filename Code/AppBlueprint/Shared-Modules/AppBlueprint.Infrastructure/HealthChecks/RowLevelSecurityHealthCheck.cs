using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AppBlueprint.Infrastructure.HealthChecks;

/// <summary>
/// Health check that verifies PostgreSQL Row-Level Security (RLS) is enabled on all tenant-scoped tables.
/// CRITICAL: The application must NOT start if RLS is not properly configured to prevent tenant data leakage.
/// </summary>
public sealed class RowLevelSecurityHealthCheck : IHealthCheck
{
    private readonly string _connectionString;
    private readonly ILogger<RowLevelSecurityHealthCheck> _logger;

    // Tables that MUST have RLS enabled for tenant isolation
    private static readonly string[] RequiredRlsTables =
    [
        "Users",
        "Teams",
        "Organizations",
        "ContactPersons",
        "EmailAddresses",
        "PhoneNumbers",
        "Addresses",
        "Todos"
    ];

    // Tables that should have RLS if they exist (optional)
    private static readonly string[] OptionalRlsTables =
    [
        "AuditLogs",
        "ApiLogs"
    ];

    public RowLevelSecurityHealthCheck(string connectionString, ILogger<RowLevelSecurityHealthCheck> logger)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        ArgumentNullException.ThrowIfNull(logger);

        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            // Step 1: Verify required functions exist
            bool functionsExist = await VerifyRlsFunctionsExistAsync(connection, cancellationToken);
            if (!functionsExist)
            {
                string errorMessage = "Row-Level Security functions (set_current_tenant, get_current_tenant) are missing. Run SetupRowLevelSecurity.sql.";
                _logger.LogCritical("[RLS Health Check] {ErrorMessage}", errorMessage);
                return HealthCheckResult.Unhealthy(errorMessage);
            }

            // Step 2: Check RLS status on required tables
            var rlsStatus = await GetRlsStatusAsync(connection, cancellationToken);

            var missingRls = new List<string>();
            var missingTables = new List<string>();

            foreach (string table in RequiredRlsTables)
            {
                if (!rlsStatus.TryGetValue(table, out bool hasRls))
                {
                    // Table doesn't exist yet - might be before migrations
                    missingTables.Add(table);
                }
                else if (!hasRls)
                {
                    // Table exists but RLS not enabled - CRITICAL security issue
                    missingRls.Add(table);
                }
            }

            // Step 3: Verify policies exist on tables with RLS enabled
            var missingPolicies = await GetTablesWithoutPoliciesAsync(connection, RequiredRlsTables, cancellationToken);

            // Step 4: Determine health status
            if (missingRls.Count > 0)
            {
                string errorMessage = $"CRITICAL: Row-Level Security NOT enabled on tables: {string.Join(", ", missingRls)}. " +
                                    "Run SetupRowLevelSecurity.sql immediately to prevent tenant data leakage.";
                _logger.LogCritical("[RLS Health Check] {ErrorMessage}", errorMessage);
                return HealthCheckResult.Unhealthy(errorMessage);
            }

            if (missingPolicies.Count > 0)
            {
                string errorMessage = $"CRITICAL: RLS policies missing on tables: {string.Join(", ", missingPolicies)}. " +
                                    "Tables have RLS enabled but no policies defined. Run SetupRowLevelSecurity.sql.";
                _logger.LogCritical("[RLS Health Check] {ErrorMessage}", errorMessage);
                return HealthCheckResult.Unhealthy(errorMessage);
            }

            if (missingTables.Count > 0)
            {
                // Tables don't exist yet - might be initial setup
                // This is degraded status, not unhealthy
                string warningMessage = $"Tables not yet created: {string.Join(", ", missingTables)}. " +
                                      "RLS will be checked after migrations are applied.";
                _logger.LogWarning("[RLS Health Check] {WarningMessage}", warningMessage);
                return HealthCheckResult.Degraded(warningMessage);
            }

            // All checks passed
            var enabledTables = rlsStatus.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
            string successMessage = $"Row-Level Security enabled and configured on {enabledTables.Count} tables: {string.Join(", ", enabledTables)}";
            _logger.LogInformation("[RLS Health Check] ✅ {SuccessMessage}", successMessage);

            return HealthCheckResult.Healthy(successMessage);
        }
        catch (NpgsqlException ex)
        {
            string errorMessage = $"Database error verifying Row-Level Security: {ex.Message}";
            _logger.LogError(ex, "[RLS Health Check] {ErrorMessage}", errorMessage);
            return HealthCheckResult.Unhealthy(errorMessage, ex);
        }
        catch (InvalidOperationException ex)
        {
            string errorMessage = $"Invalid operation during RLS verification: {ex.Message}";
            _logger.LogError(ex, "[RLS Health Check] {ErrorMessage}", errorMessage);
            return HealthCheckResult.Unhealthy(errorMessage, ex);
        }
        catch (TimeoutException ex)
        {
            string errorMessage = $"Timeout verifying Row-Level Security: {ex.Message}";
            _logger.LogError(ex, "[RLS Health Check] {ErrorMessage}", errorMessage);
            return HealthCheckResult.Degraded(errorMessage, ex);
        }
    }

    private static async Task<bool> VerifyRlsFunctionsExistAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT COUNT(*) 
            FROM pg_proc 
            WHERE proname IN ('set_current_tenant', 'get_current_tenant')";

        await using var command = new NpgsqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync(cancellationToken);

        // Should return 2 (both functions exist)
        return Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture) == 2;
    }

    private static async Task<Dictionary<string, bool>> GetRlsStatusAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT tablename, rowsecurity 
            FROM pg_tables 
            WHERE schemaname = 'public'";

        var rlsStatus = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        await using var command = new NpgsqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            string tableName = reader.GetString(0);
            bool rlsEnabled = reader.GetBoolean(1);
            rlsStatus[tableName] = rlsEnabled;
        }

        return rlsStatus;
    }

    private static async Task<List<string>> GetTablesWithoutPoliciesAsync(
        NpgsqlConnection connection,
        string[] requiredTables,
        CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT DISTINCT tablename 
            FROM pg_policies 
            WHERE schemaname = 'public' 
                AND policyname = 'tenant_isolation_policy'";

        var tablesWithPolicies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        await using var command = new NpgsqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            tablesWithPolicies.Add(reader.GetString(0));
        }

        // Find tables that should have policies but don't
        var missingPolicies = new List<string>();
        var rlsStatus = await GetRlsStatusAsync(connection, cancellationToken);

        foreach (string table in requiredTables)
        {
            // Only check if table exists and has RLS enabled
            if (rlsStatus.TryGetValue(table, out bool hasRls) && hasRls)
            {
                if (!tablesWithPolicies.Contains(table))
                {
                    missingPolicies.Add(table);
                }
            }
        }

        return missingPolicies;
    }
}
