using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace AppBlueprint.Infrastructure;

public static class MigrationExtensions
{
    private static readonly ILogger Logger = LoggerFactory
        .Create(builder => builder.AddConsole())
        .CreateLogger(typeof(MigrationExtensions));
        
    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        await using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            // Get connection string for logging
            var connectionString = dbContext.Database.GetConnectionString();
            Logger.LogInformation("Attempting to apply migrations with connection: {ConnectionString}", 
                connectionString?.Replace(";Password=", ";Password=*****"));
            
            // Ensure we can connect to the database
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    // Try to connect with explicit timeout
                    NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString);
                    sqlConnection.Open();
                    sqlConnection.Close();
                    Logger.LogInformation("Successfully established database connection on attempt {Attempt}", attempt);
                    break;
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("Failed to connect to database on attempt {Attempt}: {Message}", attempt, ex.Message);
                    if (attempt == 3)
                    {
                        throw;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }
            }

            // Check if the database exists before trying to migrate
            Logger.LogInformation("Checking database connection...");
            if (await dbContext.Database.CanConnectAsync())
            {
                Logger.LogInformation("Successfully connected to database. Applying database migrations...");
                await dbContext.Database.MigrateAsync();
                Logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                Logger.LogWarning("Cannot connect to the database. Migrations were not applied.");
            }
        }
        catch (Exception ex)
        {
            // Log the error but don't crash the application
            Logger.LogError(ex, "An error occurred while applying migrations: {Message}", ex.Message);
            Logger.LogWarning("Application will continue without applying migrations.");
        }
    }

    public static async Task ApplyDatabaseSeedingAsync(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using IServiceScope scope = app.ApplicationServices.CreateScope();

        await using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            var dataSeeder = new DataSeeder(dbContext);
            await dataSeeder.SeedDatabaseAsync();
            Logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            // Log the error but don't crash the application
            Logger.LogError(ex, "An error occurred while seeding the database: {Message}", ex.Message);
            Logger.LogWarning("Application will continue without seeding the database.");
        }
    }
}