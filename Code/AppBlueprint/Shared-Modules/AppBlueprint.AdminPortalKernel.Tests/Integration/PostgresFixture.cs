using Npgsql;
using Testcontainers.PostgreSql;

namespace AppBlueprint.AdminPortalKernel.Tests.Integration;

/// <summary>
/// Shared PostgreSQL test container with the baseline "Users"/"Tenants" schema subset
/// that every AppBlueprint-based app database exposes (column names and types copied
/// from BaselineDbContextModelSnapshot.cs). One container per test session; the
/// Testcontainers reaper removes it after the process exits.
/// </summary>
internal static class PostgresFixture
{
    private static readonly SemaphoreSlim InitLock = new(1, 1);
    private static PostgreSqlContainer? _container;

    private const string BaselineSchemaSql = """
        CREATE TABLE IF NOT EXISTS "Tenants" (
            "Id" character varying(40) PRIMARY KEY,
            "Country" character varying(100) NULL,
            "CreatedAt" timestamp with time zone NOT NULL,
            "CustomerEntityId" character varying(1024) NULL,
            "CustomerId" character varying(1024) NULL,
            "Description" character varying(500) NULL,
            "Email" character varying(100) NULL,
            "IsActive" boolean NOT NULL,
            "IsPrimary" boolean NOT NULL,
            "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
            "LastUpdatedAt" timestamp with time zone NULL,
            "Name" character varying(100) NOT NULL,
            "Phone" character varying(20) NULL,
            "TenantType" integer NOT NULL,
            "VatNumber" character varying(50) NULL
        );

        CREATE TABLE IF NOT EXISTS "Users" (
            "Id" character varying(40) PRIMARY KEY,
            "CreatedAt" timestamp with time zone NOT NULL,
            "Email" character varying(100) NOT NULL,
            "ExternalAuthId" character varying(1024) NULL,
            "FirstName" character varying(100) NOT NULL,
            "IsActive" boolean NOT NULL,
            "IsSoftDeleted" boolean NOT NULL DEFAULT FALSE,
            "LastLogin" timestamp with time zone NOT NULL,
            "LastName" character varying(100) NOT NULL,
            "LastUpdatedAt" timestamp with time zone NULL,
            "TenantId" character varying(40) NOT NULL,
            "UserName" character varying(100) NOT NULL
        );
        """;

    /// <summary>
    /// Connection string to a separate "dm_audit_tests" database in the same container,
    /// mirroring production where the audit log lives in DeploymentManager's own database
    /// rather than in any app database.
    /// </summary>
    public static async Task<string> GetAuditConnectionStringAsync()
    {
        string appDbConnectionString = await GetConnectionStringAsync();

        await InitLock.WaitAsync();
        try
        {
            if (!_auditDatabaseCreated)
            {
                await using var connection = new NpgsqlConnection(appDbConnectionString);
                await connection.OpenAsync();
                await using var command = new NpgsqlCommand("CREATE DATABASE dm_audit_tests", connection);
                await command.ExecuteNonQueryAsync();
                _auditDatabaseCreated = true;
            }
        }
        finally
        {
            InitLock.Release();
        }

        var builder = new NpgsqlConnectionStringBuilder(appDbConnectionString) { Database = "dm_audit_tests" };
        return builder.ConnectionString;
    }

    private static bool _auditDatabaseCreated;

    public static async Task<string> GetConnectionStringAsync()
    {
        if (_container is not null)
        {
            return _container.GetConnectionString();
        }

        await InitLock.WaitAsync();
        try
        {
            if (_container is null)
            {
                PostgreSqlContainer container = new PostgreSqlBuilder()
                    .WithImage("postgres:17-alpine")
                    .Build();

                await container.StartAsync();

                await using (var connection = new NpgsqlConnection(container.GetConnectionString()))
                {
                    await connection.OpenAsync();
                    await using var command = new NpgsqlCommand(BaselineSchemaSql, connection);
                    await command.ExecuteNonQueryAsync();
                }

                _container = container;
            }

            return _container.GetConnectionString();
        }
        finally
        {
            InitLock.Release();
        }
    }

    /// <summary>Inserts a tenant row directly via SQL (the kernel context is read-only by design).</summary>
    public static async Task InsertTenantAsync(
        string connectionString,
        string id,
        string name,
        int tenantType = 0,
        bool isActive = true,
        bool isSoftDeleted = false,
        string? email = null,
        string? country = null,
        string? vatNumber = null)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            INSERT INTO "Tenants" ("Id", "Name", "TenantType", "IsActive", "IsPrimary", "IsSoftDeleted", "CreatedAt", "Email", "Country", "VatNumber")
            VALUES ($1, $2, $3, $4, FALSE, $5, NOW() AT TIME ZONE 'UTC', $6, $7, $8)
            """,
            connection);
        command.Parameters.AddWithValue(id);
        command.Parameters.AddWithValue(name);
        command.Parameters.AddWithValue(tenantType);
        command.Parameters.AddWithValue(isActive);
        command.Parameters.AddWithValue(isSoftDeleted);
        command.Parameters.AddWithValue((object?)email ?? DBNull.Value);
        command.Parameters.AddWithValue((object?)country ?? DBNull.Value);
        command.Parameters.AddWithValue((object?)vatNumber ?? DBNull.Value);
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>Inserts a user row directly via SQL.</summary>
    public static async Task InsertUserAsync(
        string connectionString,
        string id,
        string email,
        string tenantId,
        string firstName = "Test",
        string lastName = "User",
        string? userName = null,
        bool isActive = true,
        bool isSoftDeleted = false,
        DateTime? createdAt = null)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            INSERT INTO "Users" ("Id", "Email", "TenantId", "FirstName", "LastName", "UserName", "IsActive", "IsSoftDeleted", "CreatedAt", "LastLogin")
            VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, NOW())
            """,
            connection);
        command.Parameters.AddWithValue(id);
        command.Parameters.AddWithValue(email);
        command.Parameters.AddWithValue(tenantId);
        command.Parameters.AddWithValue(firstName);
        command.Parameters.AddWithValue(lastName);
        command.Parameters.AddWithValue(userName ?? email);
        command.Parameters.AddWithValue(isActive);
        command.Parameters.AddWithValue(isSoftDeleted);
        command.Parameters.AddWithValue(createdAt ?? DateTime.UtcNow);
        await command.ExecuteNonQueryAsync();
    }
}
