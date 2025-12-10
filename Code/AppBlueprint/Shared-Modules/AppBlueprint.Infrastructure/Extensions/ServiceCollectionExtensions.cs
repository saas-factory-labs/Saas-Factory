using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Application.Services.DataExport;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.TodoAppKernel.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint Infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AppBlueprint Infrastructure services including database contexts, repositories, 
    /// external service integrations, and health checks.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration (used as fallback if environment variables not set).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppBlueprintInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDatabaseContexts(configuration);
        services.AddRepositories();
        services.AddAuthenticationServices();
        services.AddUnitOfWork();
        services.AddHealthChecksServices(configuration);

        return services;
    }

    /// <summary>
    /// Registers database contexts (ApplicationDbContext and B2BDbContext).
    /// Uses environment variable DATABASE_CONNECTION_STRING or falls back to configuration.
    /// </summary>
    private static IServiceCollection AddDatabaseContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Priority 1: Environment variable
        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

        // Priority 2: Configuration fallback
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = configuration.GetConnectionString("appblueprintdb") ??
                             configuration.GetConnectionString("postgres-server") ??
                             configuration.GetConnectionString("DefaultConnection");
        }

        var connectionSource = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") != null
            ? "Environment Variable"
            : "Configuration";

        Console.WriteLine($"[AppBlueprint.Infrastructure] Database Connection Source: {connectionSource}");

        ArgumentException.ThrowIfNullOrEmpty(connectionString, nameof(connectionString));

        // Register ApplicationDbContext
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
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
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId
                    .PendingModelChangesWarning);
                warnings.Log(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ShadowPropertyCreated);
            });
        });

        // Register B2BDbContext
        services.AddDbContext<B2BDbContext>((serviceProvider, options) =>
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
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId
                    .PendingModelChangesWarning);
                warnings.Log(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ShadowPropertyCreated);
            });
        });

        return services;
    }

    /// <summary>
    /// Registers repository implementations from AppBlueprint.Infrastructure.
    /// </summary>
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IDataExportRepository, DataExportRepository>();
        // Add more repositories as they are implemented

        return services;
    }

    /// <summary>
    /// Registers Unit of Work pattern implementation from AppBlueprint.Infrastructure.
    /// </summary>
    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork.Implementation.UnitOfWork>();
        return services;
    }

    /// <summary>
    /// Registers health check services for database, Redis, and external endpoints.
    /// Uses environment variables or configuration for connection strings.
    /// </summary>
    private static IServiceCollection AddHealthChecksServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var healthChecksBuilder = services.AddHealthChecks();

        // Database health check
        var dbConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??
                                configuration.GetConnectionString("appblueprintdb");

        if (!string.IsNullOrEmpty(dbConnectionString))
        {
            healthChecksBuilder.AddNpgSql(
                dbConnectionString,
                name: "postgresql",
                tags: new[] { "db", "postgresql" });
        }

        // Redis health check (if configured)
        var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ??
                                   configuration.GetConnectionString("redis");

        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            healthChecksBuilder.AddRedis(
                redisConnectionString,
                name: "redis",
                tags: new[] { "cache", "redis" });
        }

        return services;
    }
}
