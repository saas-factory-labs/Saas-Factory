using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AppBlueprint.Infrastructure.Persistence.HealthChecks;

public static class RlsStartupValidator
{
    private static readonly ILogger Logger = LoggerFactory
        .Create(b => b.AddConsole())
        .CreateLogger(typeof(RlsStartupValidator));

    // Dynamically discovers all public-schema tables that have a TenantId column
    // and verifies each has RLS enabled, forced, and both expected policies.
    // No hardcoded table list — automatically covers new tables from any DbContext.
    internal const string RlsStatusQuery = """
        SELECT
            c.relname                 AS table_name,
            c.relrowsecurity          AS rls_enabled,
            c.relforcerowsecurity     AS rls_forced,
            EXISTS (
                SELECT 1 FROM pg_policies p
                WHERE p.schemaname = 'public'
                  AND p.tablename  = c.relname
                  AND p.policyname = 'tenant_isolation_read_policy'
            ) AS has_read_policy,
            EXISTS (
                SELECT 1 FROM pg_policies p
                WHERE p.schemaname = 'public'
                  AND p.tablename  = c.relname
                  AND p.policyname = 'tenant_isolation_write_policy'
            ) AS has_write_policy
        FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        WHERE n.nspname  = 'public'
          AND c.relkind  = 'r'
          AND EXISTS (
              SELECT 1 FROM information_schema.columns col
              WHERE col.table_schema = 'public'
                AND col.table_name   = c.relname
                AND col.column_name  = 'TenantId'
          )
        ORDER BY c.relname
        """;

    public static async Task ValidateOrThrowAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        string connectionString = ResolveConnectionString(services);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        List<string> violations = await CollectViolationsAsync(connection, cancellationToken);

        if (violations.Count > 0)
        {
            string message =
                "Startup aborted — Row-Level Security is not properly configured on tenant-scoped tables.\n" +
                "Run migrations to apply the EnableRowLevelSecurity migration.\n" +
                string.Join("\n", violations.Select(v => $"  • {v}"));

            Logger.LogCritical("[RLS] {Message}", message);
            throw new InvalidOperationException(message);
        }

        Logger.LogInformation("[RLS] Row-Level Security validated on all tenant-scoped tables.");
    }

    internal static async Task<List<string>> CollectViolationsAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        var violations = new List<string>();

        await using var command = new NpgsqlCommand(RlsStatusQuery, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            string table = reader.GetString(0);
            bool rlsEnabled = reader.GetBoolean(1);
            bool rlsForced = reader.GetBoolean(2);
            bool hasReadPolicy = reader.GetBoolean(3);
            bool hasWritePolicy = reader.GetBoolean(4);

            if (!rlsEnabled)
                violations.Add($"{table}: RLS not enabled (ALTER TABLE ... ENABLE ROW LEVEL SECURITY)");
            else if (!rlsForced)
                violations.Add($"{table}: RLS not forced (ALTER TABLE ... FORCE ROW LEVEL SECURITY)");
            else if (!hasReadPolicy)
                violations.Add($"{table}: missing tenant_isolation_read_policy");
            else if (!hasWritePolicy)
                violations.Add($"{table}: missing tenant_isolation_write_policy");
        }

        return violations;
    }

    internal static string ResolveConnectionString(IServiceProvider services)
    {
        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING");

        if (string.IsNullOrEmpty(connectionString))
        {
            using var scope = services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            connectionString = config.GetConnectionString("appblueprintdb")
                ?? config.GetConnectionString("DefaultConnection");
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "[RLS] Cannot validate Row-Level Security: no database connection string found. " +
                "Set the DATABASE_CONNECTIONSTRING environment variable.");
        }

        return connectionString;
    }
}
