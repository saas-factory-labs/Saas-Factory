using AppBlueprint.Infrastructure.Core.Abstractions;
using AppBlueprint.Infrastructure.Core.Configuration;
using Pulumi;
using Pulumi.Cloudflare;
using Pulumi.Cloudflare.Inputs;

namespace AppBlueprint.Infrastructure.Core.Providers.Cloudflare;

public sealed partial class CloudflareProvider
{
    // Neon Postgres is managed manually in the portal (Step 7) — IaC provisions no database. When an
    // app opts into Hyperdrive, we front that externally-managed database with a HyperdriveConfig so
    // Workers get pooled, low-latency connections. The origin is parsed from the manually-set
    // connection-string secret; every derived field inherits the secret's secrecy.

    private const string PostgresScheme = "postgres";
    private const int DefaultPostgresPort = 5432;

    private static void ProvisionDatabase(AppInfrastructureConfig config, string accountId, AppDeploymentOutputs outputs)
    {
        if (config.Database is null || !config.Database.UseHyperdrive)
        {
            return;
        }

        Output<string> connectionString = ResolveConnectionStringSecret();
        Output<PostgresOrigin> origin = connectionString.Apply(ParsePostgresConnectionString);

        string name = ResourceName(config.AppId, "hyperdrive");
        var hyperdrive = new HyperdriveConfig(name, new HyperdriveConfigArgs
        {
            AccountId = accountId,
            Name = name,
            Origin = new HyperdriveConfigOriginArgs
            {
                Scheme = PostgresScheme,
                Host = origin.Apply(o => o.Host),
                Port = origin.Apply(o => o.Port),
                Database = origin.Apply(o => o.Database),
                User = origin.Apply(o => o.User),
                Password = origin.Apply(o => o.Password)
            }
        });

        outputs.Set("database:hyperdriveId", hyperdrive.Id);
        outputs.Set(AppDeploymentOutputs.DatabaseConnectionStringKey, connectionString);
    }

    /// <summary>
    /// Resolves the externally-managed Postgres connection string as a Pulumi secret, from the
    /// <c>neon:connectionString</c> secret config or the <c>NEON_DATABASE_URL</c> /
    /// <c>DATABASE_CONNECTIONSTRING</c> environment variables.
    /// </summary>
    private static Output<string> ResolveConnectionStringSecret()
    {
        Output<string>? fromConfig = new Pulumi.Config("neon").GetSecret("connectionString");
        if (fromConfig is not null)
        {
            return fromConfig;
        }

        string? fromEnv = Environment.GetEnvironmentVariable("NEON_DATABASE_URL")
            ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING");
        if (!string.IsNullOrWhiteSpace(fromEnv))
        {
            return Output.CreateSecret(fromEnv);
        }

        throw new InvalidOperationException(
            "Database connection string not found. Set the 'neon:connectionString' Pulumi secret " +
            "or the NEON_DATABASE_URL / DATABASE_CONNECTIONSTRING environment variable.");
    }

    /// <summary>Parses a Postgres URI (e.g. <c>postgres://user:pass@host/db</c>) into origin fields.</summary>
    private static PostgresOrigin ParsePostgresConnectionString(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        Uri uri;
        try
        {
            uri = new Uri(connectionString);
        }
        catch (UriFormatException ex)
        {
            throw new InvalidOperationException(
                "The database connection string is not a valid Postgres URI " +
                "(expected postgres://user:password@host/database).", ex);
        }

        string[] userInfo = uri.UserInfo.Split(':', 2);
        string user = Uri.UnescapeDataString(userInfo[0]);
        string password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
        int port = uri.Port > 0 ? uri.Port : DefaultPostgresPort;
        string database = uri.AbsolutePath.Trim('/');

        if (string.IsNullOrWhiteSpace(uri.Host) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(database))
        {
            throw new InvalidOperationException(
                "The database connection string must include host, user and database.");
        }

        return new PostgresOrigin(uri.Host, port, database, user, password);
    }

    private sealed record PostgresOrigin(string Host, int Port, string Database, string User, string Password);
}
