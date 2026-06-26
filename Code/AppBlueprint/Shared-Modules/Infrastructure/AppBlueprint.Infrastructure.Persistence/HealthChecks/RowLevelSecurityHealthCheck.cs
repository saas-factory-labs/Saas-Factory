using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AppBlueprint.Infrastructure.Persistence.HealthChecks;

public sealed class RowLevelSecurityHealthCheck : IHealthCheck
{
    private readonly string _connectionString;
    private readonly ILogger<RowLevelSecurityHealthCheck> _logger;

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

            if (!await RlsFunctionsExistAsync(connection, cancellationToken))
            {
                const string msg = "RLS setup functions are missing — EnableRowLevelSecurity migration has not been applied.";
                _logger.LogCritical("[RLS] {Message}", msg);
                return HealthCheckResult.Unhealthy(msg);
            }

            List<string> violations = await RlsStartupValidator.CollectViolationsAsync(connection, cancellationToken);

            if (violations.Count == 0)
            {
                _logger.LogInformation("[RLS] Row-Level Security healthy on all tenant-scoped tables.");
                return HealthCheckResult.Healthy("RLS enabled and configured on all tenant-scoped tables.");
            }

            string detail = string.Join("; ", violations);
            _logger.LogCritical("[RLS] {Detail}", detail);
            return HealthCheckResult.Unhealthy($"RLS misconfigured — {detail}");
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "[RLS] Database error during health check.");
            return HealthCheckResult.Unhealthy($"Database error: {ex.Message}", ex);
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "[RLS] Timeout during RLS health check.");
            return HealthCheckResult.Degraded($"Timeout: {ex.Message}", ex);
        }
    }

    private static async Task<bool> RlsFunctionsExistAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        const string query = """
            SELECT COUNT(*) FROM pg_proc
            WHERE proname IN ('set_current_tenant', 'get_current_tenant')
            """;

        await using var command = new NpgsqlCommand(query, connection);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture) == 2;
    }
}
