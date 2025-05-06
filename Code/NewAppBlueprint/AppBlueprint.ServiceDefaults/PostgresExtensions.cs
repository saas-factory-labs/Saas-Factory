using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.ServiceDefaults;

public static class PostgresExtensions
{
    /// <summary>
    /// Adds a PostgreSQL DbContext with OpenTelemetry instrumentation.
    /// </summary>
    public static IServiceCollection AddPostgreSqlDbContext<TContext>(
        this IServiceCollection services,
        string connectionString,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));
        }

        // Add the DbContext using the PostgreSQL provider
        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptionsAction =>
            {
                // Configure retries for better reliability
                npgsqlOptionsAction.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
            
            // Let the options action configure additional settings if provided
            optionsAction?.Invoke(options);
        });

        return services;
    }
}
