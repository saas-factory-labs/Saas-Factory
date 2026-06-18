using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Options;
using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.Compliance.Extensions;
using AppBlueprint.Infrastructure.DependencyInjection;
using AppBlueprint.Infrastructure.Email.Extensions;
using AppBlueprint.Infrastructure.Payments.Extensions;
using AppBlueprint.Infrastructure.Persistence.Configuration;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Configuration;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Interceptors;
using AppBlueprint.Infrastructure.Persistence.Extensions;
using AppBlueprint.Infrastructure.Persistence.Repositories;
using AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;
using AppBlueprint.Infrastructure.Services;
using AppBlueprint.Infrastructure.Storage.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using B2BDbContext = AppBlueprint.Infrastructure.Persistence.DatabaseContexts.B2B.B2BDbContext;
using BaselineDbContext = AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.BaselineDbContext;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint Infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    // Static readonly arrays for health check tags (CA1861)
    private static readonly string[] PostgreSqlHealthCheckTags = new[] { "db", "postgresql" };
    private static readonly string[] RlsHealthCheckTags = new[] { "db", "security", "rls", "critical" };
    private static readonly string[] RedisHealthCheckTags = new[] { "cache", "redis" };

    /// <summary>
    /// Adds AppBlueprint Infrastructure services including database contexts, repositories,
    /// external service integrations, and health checks.
    /// Simplified overload for prototype / consuming-app integration — no environment parameter needed.
    /// Also registers configuration options (calls AddAppBlueprintConfiguration internally).
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

        // Ensure configuration options are registered
        services.TryAddAppBlueprintConfiguration(configuration);

        services.AddConfiguredDbContext(configuration);
        services.AddRepositories();
        services.AddTenantServices();
        services.AddUnitOfWork();
        services.AddExternalServices(configuration);
        services.AddHealthChecksServices(configuration);
        services.AddNotificationServices();
        services.AddPIIServices();

        return services;
    }

    /// <summary>
    /// Adds AppBlueprint Infrastructure services including database contexts, repositories, 
    /// external service integrations, and health checks.
    /// Uses the new flexible DbContext configuration based on DatabaseContextOptions.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration (used as fallback if environment variables not set).</param>
    /// <param name="environment">The hosting environment (required for authentication setup).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppBlueprintInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        // Use new flexible DbContext configuration
        services.AddConfiguredDbContext(configuration);

        services.AddRepositories();
        services.AddTenantServices();

        // Only add web authentication for non-API environments (Blazor Server)
        // API services handle authentication differently (JWT Bearer)
        // Check if this is being called from a Web project by looking for Blazor services
        // For API services, authentication is configured separately in their Program.cs
        // services.AddWebAuthentication(configuration, environment); // Commented out - add explicitly in Web project

        services.AddUnitOfWork();
        services.AddExternalServices(configuration);
        services.AddHealthChecksServices(configuration);
        services.AddNotificationServices();
        services.AddPIIServices();

        return services;
    }

    /// <summary>
    /// Adds AppBlueprint Infrastructure services using legacy database context registration.
    /// This method is maintained for backward compatibility.
    /// Consider migrating to AddAppBlueprintInfrastructure() with DatabaseContextOptions configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration (used as fallback if environment variables not set).</param>
    /// <param name="environment">The hosting environment (required for authentication setup).</param>
    /// <returns>The service collection for chaining.</returns>
    [Obsolete("Use AddAppBlueprintInfrastructure() with DatabaseContextOptions configuration instead. This method will be removed in a future version.")]
    public static IServiceCollection AddAppBlueprintInfrastructureLegacy(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        services.AddDatabaseContextsLegacy(configuration);
        services.AddRepositories();
        services.AddTenantServices();
        services.AddUnitOfWork();
        services.AddExternalServices(configuration);
        services.AddHealthChecksServices(configuration);

        return services;
    }

    /// <summary>
    /// Registers database contexts (ApplicationDbContext and B2BDbContext) - LEGACY METHOD.
    /// Uses environment variable DATABASE_CONNECTIONSTRING or falls back to configuration.
    /// This method is maintained for backward compatibility.
    /// </summary>
    [Obsolete("Use DbContextConfigurator.AddConfiguredDbContext() instead.")]
    private static IServiceCollection AddDatabaseContextsLegacy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Priority 1: Environment variable (our standard name, then Railway's default DATABASE_URL fallback)
        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING");
        if (connectionString is null)
        {
            connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
        }

        // Priority 2: Configuration fallback
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = configuration.GetConnectionString("appblueprintdb") ??
                             configuration.GetConnectionString("postgres-server") ??
                             configuration.GetConnectionString("DefaultConnection");
        }

        var connectionSource = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING") != null
            ? "Environment Variable"
            : "Configuration";

        Console.WriteLine($"[AppBlueprint.Infrastructure] Database Connection Source: {connectionSource}");

        // Validate connection string with helpful error message
        ConfigurationValidator.ValidateDatabaseConnectionString(
            connectionString,
            "appblueprintdb",
            "postgres-server",
            "DefaultConnection");

        // Create NpgsqlDataSource with EnableDynamicJson for JSONB support (required since Npgsql 8.0)
        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson(); // Required for Dictionary<string, string> JSONB columns
        var dataSource = dataSourceBuilder.Build();

        // Register ApplicationDbContext factory only (required by SignupService)
        // Note: Cannot use both AddDbContext and AddDbContextFactory - factory is Singleton, DbContext is Scoped
        services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(dataSource, npgsqlOptions =>
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

        // Register BaselineDbContext
        services.AddDbContext<BaselineDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(dataSource, npgsqlOptions =>
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
            options.UseNpgsql(dataSource, npgsqlOptions =>
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
    /// Registers repository implementations (delegates to AppBlueprint.Infrastructure.Persistence).
    /// </summary>
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services.AddAppBlueprintRepositories();
    }

    /// <summary>
    /// Registers tenant-scoped services (delegates to AppBlueprint.Infrastructure.Persistence).
    /// </summary>
    private static IServiceCollection AddTenantServices(this IServiceCollection services)
    {
        return services.AddAppBlueprintTenantServices();
    }

    /// <summary>
    /// Registers Unit of Work pattern implementation (delegates to AppBlueprint.Infrastructure.Persistence).
    /// </summary>
    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        return services.AddAppBlueprintUnitOfWork();
    }

    /// <summary>
    /// Registers external service integrations (Stripe, Cloudflare R2, Resend).
    /// Services are only registered if their configuration is present.
    /// </summary>
    private static IServiceCollection AddExternalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddStripeService();
        services.AddCloudflareR2Service();
        services.AddResendEmailService(configuration);

        return services;
    }

    /// <summary>
    /// Normalizes a PostgreSQL connection string from URI format to key-value format.
    /// Helper method for health checks to ensure Npgsql compatibility.
    /// </summary>
    private static string NormalizeConnectionStringForHealthCheck(string connectionString)
    {
        // If already in key-value format, return as-is
        if (!connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        var uri = new Uri(connectionString);
        var builder = new System.Text.StringBuilder();

        builder.Append($"Host={uri.Host};");
        builder.Append($"Port={(uri.Port > 0 ? uri.Port : 5432)};");
        builder.Append($"Database={uri.AbsolutePath.TrimStart('/')};");

        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            int colonIndex = uri.UserInfo.IndexOf(':', StringComparison.Ordinal);
            if (colonIndex >= 0)
            {
                builder.Append($"Username={Uri.UnescapeDataString(uri.UserInfo.Substring(0, colonIndex))};");
                builder.Append($"Password={Uri.UnescapeDataString(uri.UserInfo.Substring(colonIndex + 1))};");
            }
            else
            {
                builder.Append($"Username={Uri.UnescapeDataString(uri.UserInfo)};");
            }
        }

        return builder.ToString().TrimEnd(';');
    }

    /// <summary>
    /// Registers health check services for database, Redis, and external endpoints.
    /// Uses environment variables or configuration for connection strings.
    /// CRITICAL: Includes Row-Level Security validation to prevent application startup without RLS.
    /// NOTE: Health checks will fetch connection strings at RUNTIME (not registration time).
    /// </summary>
    private static IServiceCollection AddHealthChecksServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var healthChecksBuilder = services.AddHealthChecks();

        // PostgreSQL health check - uses IConfiguration to get connection string at runtime
        healthChecksBuilder.AddCheck("postgresql", () =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            string? connString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING");
            if (connString is null)
            {
                connString = Environment.GetEnvironmentVariable("DATABASE_URL");
            }
            if (connString is null)
            {
                connString = config.GetConnectionString("appblueprintdb");
            }
            if (connString is null)
            {
                connString = config.GetConnectionString("postgres-server");
            }

            if (string.IsNullOrEmpty(connString))
            {
                return HealthCheckResult.Unhealthy(
                    "DATABASE_CONNECTIONSTRING not found. Ensure environment variable or ConnectionStrings:appblueprintdb is set.");
            }

            try
            {
                // Normalize connection string to handle both URI and key-value formats
                string normalizedConnString = NormalizeConnectionStringForHealthCheck(connString);

                using var connection = new Npgsql.NpgsqlConnection(normalizedConnString);
                connection.Open();
                connection.Close();
                return HealthCheckResult.Healthy($"Connected successfully");
            }
            catch (Npgsql.NpgsqlException ex)
            {
                return HealthCheckResult.Unhealthy($"PostgreSQL connection failed: {ex.Message}", ex);
            }
            catch (InvalidOperationException ex)
            {
                return HealthCheckResult.Unhealthy($"PostgreSQL connection failed: {ex.Message}", ex);
            }
            catch (ArgumentException ex)
            {
                return HealthCheckResult.Unhealthy($"PostgreSQL connection failed: {ex.Message}", ex);
            }
        }, tags: PostgreSqlHealthCheckTags);

        // CRITICAL: Row-Level Security validation
        // Application MUST NOT start if RLS is not properly configured
        // This prevents tenant data leakage if RLS policies are missing
        healthChecksBuilder.AddCheck("row-level-security", () =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            string? connString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING") ??
                               config.GetConnectionString("appblueprintdb") ??
                               config.GetConnectionString("postgres-server");

            if (string.IsNullOrEmpty(connString))
            {
                return HealthCheckResult.Unhealthy(
                    "DATABASE_CONNECTIONSTRING not found for RLS validation.");
            }

            try
            {
                // Normalize connection string to handle both URI and key-value formats
                string normalizedConnString = NormalizeConnectionStringForHealthCheck(connString);

                using var connection = new Npgsql.NpgsqlConnection(normalizedConnString);
                connection.Open();

                // Check if RLS is enabled on tenant-scoped tables
                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = @"
                    SELECT tablename 
                    FROM pg_tables 
                    WHERE schemaname = 'public' 
                    AND tablename IN ('Users', 'Teams', 'TodoItems')
                    AND tablename NOT IN (
                        SELECT tablename FROM pg_tables t
                        JOIN pg_class c ON c.relname = t.tablename
                        WHERE c.relrowsecurity = true
                    );";

                var reader = checkCommand.ExecuteReader();
                List<string> tablesWithoutRls = [];
                while (reader.Read())
                {
                    tablesWithoutRls.Add(reader.GetString(0));
                }
                reader.Close();
                connection.Close();

                if (tablesWithoutRls.Count > 0)
                {
                    return HealthCheckResult.Unhealthy(
                        $"RLS NOT ENABLED on tables: {string.Join(", ", tablesWithoutRls)}. " +
                        "Run SetupRowLevelSecurity.sql to enable RLS policies.");
                }

                return HealthCheckResult.Healthy("RLS enabled on all tenant-scoped tables");
            }
            catch (Npgsql.NpgsqlException ex)
            {
                return HealthCheckResult.Unhealthy($"RLS validation failed: {ex.Message}", ex);
            }
            catch (InvalidOperationException ex)
            {
                return HealthCheckResult.Unhealthy($"RLS validation failed: {ex.Message}", ex);
            }
            catch (ArgumentException ex)
            {
                return HealthCheckResult.Unhealthy($"RLS validation failed: {ex.Message}", ex);
            }
        }, tags: RlsHealthCheckTags);

        // Redis health check (if configured at startup)
        string? redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ??
                                   configuration.GetConnectionString("redis");

        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            healthChecksBuilder.AddRedis(
                redisConnectionString,
                name: "redis",
                tags: RedisHealthCheckTags);
        }

        return services;
    }

    /// <summary>
    /// Registers PII detection engine and scanners (delegates to AppBlueprint.Infrastructure.PII)
    /// plus the EF Core PII interceptor owned by this project.
    /// </summary>
    private static IServiceCollection AddPIIServices(this IServiceCollection services)
    {
        services.AddPIIDetection();

        // Register the EF Core interceptor
        services.AddScoped<PiiSaveChangesInterceptor>();

        return services;
    }
}

