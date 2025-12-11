using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using AppBlueprint.TodoAppKernel.Infrastructure;
using AppBlueprint.TodoAppKernel.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.TodoAppKernel.Infrastructure;

/// <summary>
/// Extension methods for registering TodoAppKernel services
/// Follows the same pattern as the Dating app integration example
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds TodoAppKernel services including TodoDbContext and TodoRepository
    /// </summary>
    public static IServiceCollection AddTodoAppKernel(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Get connection string (priority: environment variable -> configuration)
        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = configuration.GetConnectionString("appblueprintdb") ??
                             configuration.GetConnectionString("postgres-server") ??
                             configuration.GetConnectionString("DefaultConnection");
        }

        ArgumentException.ThrowIfNullOrEmpty(connectionString, nameof(connectionString));

        // Register TodoDbContext (inherits from B2BDbContext)
        services.AddDbContext<TodoDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(60);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });

            options.ConfigureWarnings(warnings =>
            {
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning);
                warnings.Log(CoreEventId.ShadowPropertyCreated);
            });
        });

        // Register TodoRepository
        services.AddScoped<ITodoRepository, TodoRepository>();

        return services;
    }
}
