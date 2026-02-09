using AppBlueprint.Infrastructure.DatabaseContexts;
using FluentRegex;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using B2BDbContext = AppBlueprint.Infrastructure.DatabaseContexts.B2B.B2BDbContext;
using BaselineDbContext = AppBlueprint.Infrastructure.DatabaseContexts.Baseline.BaselineDbContext;

namespace AppBlueprint.Infrastructure;

public static class MigrationExtensions
{
    private static readonly ILogger Logger = LoggerFactory
        .Create(builder => builder.AddConsole())
        .CreateLogger(typeof(MigrationExtensions));

    private const int MaxRetryAttempts = 10;
    private const int InitialRetryDelayMs = 1000;

    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using IServiceScope scope = app.ApplicationServices.CreateScope();

        await using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await using B2BDbContext b2bDbContext =
            scope.ServiceProvider.GetRequiredService<B2BDbContext>();

        await using BaselineDbContext baselineDbContext =
            scope.ServiceProvider.GetRequiredService<BaselineDbContext>();

        try
        {
            // Get connection string without logging it (security)
            string? connectionString = dbContext.Database.GetConnectionString();
            Logger.LogInformation("Attempting to apply database migrations...");

            // Try to ensure the database exists first with retry logic
            await RetryWithExponentialBackoffAsync(
                async () => await EnsureDatabaseExistsAsync(connectionString),
                "database creation check");

            // Check if the database exists before trying to migrate with retry logic
            Logger.LogInformation("Checking database connection with retry...");
            bool canConnect = await RetryWithExponentialBackoffAsync(
                async () => await dbContext.Database.CanConnectAsync(),
                "database connection check");

            if (canConnect)
            {
                Logger.LogInformation("Successfully connected to database. Applying ApplicationDbContext migrations...");
                await dbContext.Database.MigrateAsync();
                Logger.LogInformation("ApplicationDbContext migrations applied successfully.");

                Logger.LogInformation("Applying B2BDbContext migrations...");
                await b2bDbContext.Database.MigrateAsync();
                Logger.LogInformation("B2BDbContext migrations applied successfully.");

                Logger.LogInformation("Applying BaselineDbContext migrations...");
                await baselineDbContext.Database.MigrateAsync();
                Logger.LogInformation("BaselineDbContext migrations applied successfully.");
            }
            else
            {
                Logger.LogWarning("Cannot connect to the database. Migrations were not applied.");
            }
        }
        catch (NpgsqlException ex)
        {
            // Log the database-specific error but don't crash the application
            Logger.LogError(ex, "A database error occurred while applying migrations: {Message}", ex.Message);
            Logger.LogWarning("Application will continue without applying migrations.");
        }
        catch (InvalidOperationException ex)
        {
            // Log EF Core operation errors but don't crash the application
            Logger.LogError(ex, "An Entity Framework error occurred while applying migrations: {Message}", ex.Message);
            Logger.LogWarning("Application will continue without applying migrations.");
        }
        catch (TimeoutException ex)
        {
            // Log timeout errors but don't crash the application
            Logger.LogError(ex, "A timeout occurred while applying migrations: {Message}", ex.Message);
            Logger.LogWarning("Application will continue without applying migrations.");
        }
    }

    private static async Task<T> RetryWithExponentialBackoffAsync<T>(Func<Task<T>> operation, string operationName)
    {
        ArgumentNullException.ThrowIfNull(operation);

        int retryCount = 0;
        int delayMs = InitialRetryDelayMs;

        while (true)
        {
            try
            {
                return await operation();
            }
            catch (NpgsqlException ex) when (retryCount < MaxRetryAttempts && IsTransientError(ex))
            {
                retryCount++;
                Logger.LogWarning(ex,
                    "Transient error during {OperationName} (attempt {RetryCount}/{MaxRetries}). Retrying in {DelayMs}ms...",
                    operationName, retryCount, MaxRetryAttempts, delayMs);

                await Task.Delay(delayMs);
                delayMs = Math.Min(delayMs * 2, 10000); // Cap at 10 seconds
            }
            catch (Exception ex) when (retryCount < MaxRetryAttempts)
            {
                retryCount++;
                Logger.LogWarning(ex,
                    "Error during {OperationName} (attempt {RetryCount}/{MaxRetries}). Retrying in {DelayMs}ms...",
                    operationName, retryCount, MaxRetryAttempts, delayMs);

                await Task.Delay(delayMs);
                delayMs = Math.Min(delayMs * 2, 10000);
            }
        }
    }

    private static async Task RetryWithExponentialBackoffAsync(Func<Task> operation, string operationName)
    {
        ArgumentNullException.ThrowIfNull(operation);

        await RetryWithExponentialBackoffAsync(async () =>
        {
            await operation();
            return true;
        }, operationName);
    }

    private static bool IsTransientError(NpgsqlException ex)
    {
        ArgumentNullException.ThrowIfNull(ex);

        // Common PostgreSQL transient error codes
        // 08000 - connection_exception
        // 08003 - connection_does_not_exist
        // 08006 - connection_failure
        // 53300 - too_many_connections
        // 57P03 - cannot_connect_now
        return ex.SqlState is "08000" or "08003" or "08006" or "53300" or "57P03"
               || ex.Message.Contains("server is starting up", StringComparison.OrdinalIgnoreCase)
               || ex.Message.Contains("authentication", StringComparison.OrdinalIgnoreCase)
               || ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task EnsureDatabaseExistsAsync(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            Logger.LogWarning("Connection string is null or empty. Cannot ensure database exists.");
            return;
        }

        try
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var databaseName = builder.Database;

            // Validate database name to prevent SQL injection
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new InvalidOperationException("Database name cannot be null or empty.");
            }

            // PostgreSQL identifier validation: alphanumeric, underscore, max 63 chars
            if (!System.Text.RegularExpressions.Regex.IsMatch(databaseName, Pattern.With
                .StartOfLine
                .Set(Pattern.With.Letter.Literal("_"))
                .Set(Pattern.With.Letter.Digit.Literal("_")).Repeat.Times(0, 62)
                .EndOfLine
                .ToString()))
            {
                throw new InvalidOperationException($"Invalid database name: '{databaseName}'. Database names must start with a letter or underscore and contain only alphanumeric characters and underscores (max 63 chars).");
            }

            // Switch to postgres database to check if our database exists
            builder.Database = "postgres";
            var postgresConnectionString = builder.ToString();

            Logger.LogInformation("Checking if database '{DatabaseName}' exists...", databaseName);

            await using var connection = new NpgsqlConnection(postgresConnectionString);
            await connection.OpenAsync();

            // Check if database exists
            await using var checkCommand = new NpgsqlCommand(
                $"SELECT 1 FROM pg_database WHERE datname = @databaseName", connection);
            checkCommand.Parameters.AddWithValue("databaseName", databaseName);

            var exists = await checkCommand.ExecuteScalarAsync();

            if (exists is null)
            {
                Logger.LogInformation("Database '{DatabaseName}' does not exist. Creating it...", databaseName);

                // Create the database - databaseName is validated above
#pragma warning disable CA2100 // Database name is validated against injection attacks
                await using var createCommand = new NpgsqlCommand(
                    $"CREATE DATABASE \"{databaseName}\"", connection);
                await createCommand.ExecuteNonQueryAsync();
#pragma warning restore CA2100

                Logger.LogInformation("Database '{DatabaseName}' created successfully.", databaseName);
            }
            else
            {
                Logger.LogInformation("Database '{DatabaseName}' already exists.", databaseName);
            }

            await connection.CloseAsync();
        }
        catch (NpgsqlException ex)
        {
            Logger.LogError(ex, "Failed to ensure database exists: {Message}", ex.Message);
            throw;
        }
    }

    // NOTE: Database seeding has been moved to SeedTest project
    // This method is left as a placeholder for future database initialization tasks
    public static Task ApplyDatabaseSeedingAsync(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // Database seeding is now handled separately via the SeedTest project
        // For production scenarios, consider implementing database seeding through:
        // - Migration-based seed data
        // - Startup initialization tasks
        // - Dedicated seeding services

        Logger.LogInformation("Database seeding method called - no seeding configured (use SeedTest project for development seeding).");
        return Task.CompletedTask;
    }
}
