using Amazon.Runtime;
using Amazon.S3;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Application.Options;
using AppBlueprint.Application.Services.DataExport;
using AppBlueprint.Infrastructure.Authentication;
using AppBlueprint.Infrastructure.Configuration;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;
using AppBlueprint.Infrastructure.DatabaseContexts.Configuration;
using AppBlueprint.Infrastructure.DatabaseContexts.Interceptors;
using AppBlueprint.Infrastructure.HealthChecks;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;
using Stripe;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint Infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string DefaultResendApiBaseUrl = "https://api.resend.com";
    
    // Static readonly arrays for health check tags (CA1861)
    private static readonly string[] PostgreSqlHealthCheckTags = new[] { "db", "postgresql" };
    private static readonly string[] RlsHealthCheckTags = new[] { "db", "security", "rls", "critical" };
    private static readonly string[] RedisHealthCheckTags = new[] { "cache", "redis" };

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
    /// Uses environment variable DATABASE_CONNECTION_STRING or falls back to configuration.
    /// This method is maintained for backward compatibility.
    /// </summary>
    [Obsolete("Use DbContextConfigurator.AddConfiguredDbContext() instead.")]
    private static IServiceCollection AddDatabaseContextsLegacy(
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

        // Validate connection string with helpful error message
        ConfigurationValidator.ValidateDatabaseConnectionString(
            connectionString, 
            "appblueprintdb", 
            "postgres-server", 
            "DefaultConnection");

        // Register ApplicationDbContext factory only (required by SignupService)
        // Note: Cannot use both AddDbContext and AddDbContextFactory - factory is Singleton, DbContext is Scoped
        services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, options) =>
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

        // Register BaselineDbContext
        services.AddDbContext<BaselineDbContext>((serviceProvider, options) =>
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
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IDataExportRepository, DataExportRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        // Add more repositories as they are implemented

        return services;
    }

    /// <summary>
    /// Registers tenant-scoped services for multi-tenant isolation and admin access.
    /// </summary>
    private static IServiceCollection AddTenantServices(this IServiceCollection services)
    {
        // Multi-tenant isolation
        services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();
        services.AddScoped<TenantConnectionInterceptor>();
        
        // User context and admin access
        services.AddScoped<Application.Services.ICurrentUserService, CurrentUserService>();
        services.AddScoped<Application.Services.ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<Application.Services.IAdminTenantAccessService, AdminTenantAccessService>();
        
        // Signup database context provider (EF Core)
        services.AddScoped<Application.Services.ISignupDbContextProvider, SignupDbContextProvider>();
        
        return services;
    }

    /// <summary>
    /// Registers Unit of Work pattern implementation from AppBlueprint.Infrastructure.
    /// </summary>
    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork.Implementation.UnitOfWorkImplementation>();
        return services;
    }

    /// <summary>
    /// Registers external service integrations (Stripe, Cloudflare R2, Resend).
    /// Services are only registered if their configuration is present.
    /// </summary>
    private static IServiceCollection AddExternalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddStripeService(configuration);
        services.AddCloudflareR2Service(configuration);
        services.AddResendEmailService(configuration);

        return services;
    }

    /// <summary>
    /// Registers Stripe payment service if API key is configured.
    /// Uses StripeOptions from IOptions pattern.
    /// </summary>
    private static IServiceCollection AddStripeService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get Stripe options to check if configured
        IServiceProvider tempProvider = services.BuildServiceProvider();
        StripeOptions? stripeOptions = tempProvider.GetService<IOptions<StripeOptions>>()?.Value;
        
        if (stripeOptions is not null && !string.IsNullOrWhiteSpace(stripeOptions.ApiKey))
        {
            services.AddScoped<StripeSubscriptionService>();
            Console.WriteLine("[AppBlueprint.Infrastructure] Stripe service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure] Stripe not configured (optional)");
        }

        return services;
    }

    /// <summary>
    /// Registers Cloudflare R2 object storage service if credentials are configured.
    /// Uses CloudflareR2Options from IOptions pattern.
    /// </summary>
    private static IServiceCollection AddCloudflareR2Service(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get R2 options to check if configured
        IServiceProvider tempProvider = services.BuildServiceProvider();
        CloudflareR2Options? r2Options = tempProvider.GetService<IOptions<CloudflareR2Options>>()?.Value;
        
        if (r2Options is not null && 
            !string.IsNullOrWhiteSpace(r2Options.AccessKeyId) &&
            !string.IsNullOrWhiteSpace(r2Options.SecretAccessKey) &&
            !string.IsNullOrWhiteSpace(r2Options.EndpointUrl) &&
            !string.IsNullOrWhiteSpace(r2Options.BucketName))
        {
            services.AddSingleton<IAmazonS3>(sp =>
            {
                CloudflareR2Options options = sp.GetRequiredService<IOptions<CloudflareR2Options>>().Value;
                var credentials = new BasicAWSCredentials(options.AccessKeyId, options.SecretAccessKey);
                return new AmazonS3Client(credentials, new AmazonS3Config
                {
                    ServiceURL = options.EndpointUrl,
                    ForcePathStyle = true // Required for R2 compatibility
                });
            });
            
            services.AddSingleton<ObjectStorageService>();
            Console.WriteLine("[AppBlueprint.Infrastructure] Cloudflare R2 storage service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure] Cloudflare R2 not configured (optional)");
        }

        return services;
    }

    /// <summary>
    /// Registers Resend email service if API key is configured.
    /// Uses ResendEmailOptions from IOptions pattern.
    /// </summary>
    private static IServiceCollection AddResendEmailService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get Resend options to check if configured
        IServiceProvider tempProvider = services.BuildServiceProvider();
        ResendEmailOptions? resendOptions = tempProvider.GetService<IOptions<ResendEmailOptions>>()?.Value;
        
        if (resendOptions is not null && !string.IsNullOrWhiteSpace(resendOptions.ApiKey))
        {
            services.AddHttpClient<IResend, ResendClient>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(resendOptions.BaseUrl, UriKind.Absolute);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {resendOptions.ApiKey}");
                    client.Timeout = TimeSpan.FromSeconds(resendOptions.TimeoutSeconds);
                });
            services.AddScoped<TransactionEmailService>();
            Console.WriteLine("[AppBlueprint.Infrastructure] Resend email service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure] Resend not configured (optional)");
        }

        return services;
    }

    /// <summary>
    /// Registers health check services for database, Redis, and external endpoints.
    /// Uses environment variables or configuration for connection strings.
    /// CRITICAL: Includes Row-Level Security validation to prevent application startup without RLS.
    /// </summary>
    private static IServiceCollection AddHealthChecksServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var healthChecksBuilder = services.AddHealthChecks();

        // Database health check
        string? dbConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??
                                configuration.GetConnectionString("appblueprintdb");

        if (!string.IsNullOrEmpty(dbConnectionString))
        {
            healthChecksBuilder.AddNpgSql(
                dbConnectionString,
                name: "postgresql",
                tags: PostgreSqlHealthCheckTags);

            // CRITICAL: Row-Level Security validation
            // Application MUST NOT start if RLS is not properly configured
            // This prevents tenant data leakage if RLS policies are missing
            healthChecksBuilder.AddCheck(
                "row-level-security",
                new RowLevelSecurityHealthCheck(
                    dbConnectionString,
                    services.BuildServiceProvider().GetRequiredService<ILogger<RowLevelSecurityHealthCheck>>()),
                failureStatus: HealthStatus.Unhealthy,
                tags: RlsHealthCheckTags);
        }

        // Redis health check (if configured)
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
}
