using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace AppBlueprint.Infrastructure;

public static class MigrationExtensions
{
    private static readonly ILogger Logger = LoggerFactory
        .Create(builder => builder.AddConsole())
        .CreateLogger(typeof(MigrationExtensions));

    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using IServiceScope scope = app.ApplicationServices.CreateScope();

        await using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await using B2BDbContext b2bDbContext =
            scope.ServiceProvider.GetRequiredService<B2BDbContext>();

        try
        {
            // Get connection string for logging
            var connectionString = dbContext.Database.GetConnectionString();
            Logger.LogInformation("Attempting to apply migrations with connection: {ConnectionString}",
                connectionString?.Replace(";Password=", ";Password=*****", StringComparison.Ordinal));

            // Try to ensure the database exists first
            await EnsureDatabaseExistsAsync(connectionString);

            // Check if the database exists before trying to migrate
            Logger.LogInformation("Checking database connection...");
            if (await dbContext.Database.CanConnectAsync())
            {
                Logger.LogInformation("Successfully connected to database. Applying ApplicationDbContext migrations...");
                await dbContext.Database.MigrateAsync();
                Logger.LogInformation("ApplicationDbContext migrations applied successfully.");

                Logger.LogInformation("Applying B2BDbContext migrations...");
                await b2bDbContext.Database.MigrateAsync();
                Logger.LogInformation("B2BDbContext migrations applied successfully.");
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
                
                // Create the database
                await using var createCommand = new NpgsqlCommand(
                    $"CREATE DATABASE \"{databaseName}\"", connection);
                await createCommand.ExecuteNonQueryAsync();
                
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