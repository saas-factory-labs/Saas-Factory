using Amazon.Runtime;
using Amazon.S3;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Application.Services.DataExport;
using AppBlueprint.Infrastructure.Authentication;
using AppBlueprint.Infrastructure.Configuration;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Resend;
using Stripe;

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

        services.AddDatabaseContexts(configuration);
        services.AddRepositories();
        
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

        // Validate connection string with helpful error message
        ConfigurationValidator.ValidateDatabaseConnectionString(
            connectionString, 
            "appblueprintdb", 
            "postgres-server", 
            "DefaultConnection");

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
    /// Looks for STRIPE_API_KEY environment variable or ConnectionStrings:StripeApiKey in configuration.
    /// </summary>
    private static IServiceCollection AddStripeService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string? stripeApiKey = Environment.GetEnvironmentVariable("STRIPE_API_KEY") ??
                          configuration.GetConnectionString("StripeApiKey");

        if (!string.IsNullOrEmpty(stripeApiKey))
        {
            StripeConfiguration.ApiKey = stripeApiKey;
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
    /// Requires ObjectStorage:AccessKeyId, ObjectStorage:SecretAccessKey, ObjectStorage:EndpointUrl, ObjectStorage:BucketName.
    /// </summary>
    private static IServiceCollection AddCloudflareR2Service(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string? accessKeyId = Environment.GetEnvironmentVariable("CLOUDFLARE_R2_ACCESS_KEY_ID") ??
                         configuration["ObjectStorage:AccessKeyId"];
        string? secretAccessKey = Environment.GetEnvironmentVariable("CLOUDFLARE_R2_SECRET_ACCESS_KEY") ??
                             configuration["ObjectStorage:SecretAccessKey"];
        string? endpointUrl = Environment.GetEnvironmentVariable("CLOUDFLARE_R2_ENDPOINT_URL") ??
                         configuration["ObjectStorage:EndpointUrl"];
        string? bucketName = Environment.GetEnvironmentVariable("CLOUDFLARE_R2_BUCKET_NAME") ??
                        configuration["ObjectStorage:BucketName"];

        if (!string.IsNullOrEmpty(accessKeyId) &&
            !string.IsNullOrEmpty(secretAccessKey) &&
            !string.IsNullOrEmpty(endpointUrl) &&
            !string.IsNullOrEmpty(bucketName))
        {
            services.AddSingleton<IAmazonS3>(sp =>
            {
                var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);
                return new AmazonS3Client(credentials, new AmazonS3Config
                {
                    ServiceURL = endpointUrl,
                    ForcePathStyle = true // Required for R2 compatibility
                });
            });

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
    /// Looks for RESEND_API_KEY environment variable or Resend:ApiKey in configuration.
    /// </summary>
    private static IServiceCollection AddResendEmailService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string? resendApiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY") ??
                          configuration["Resend:ApiKey"];

        if (!string.IsNullOrEmpty(resendApiKey))
        {
            string resendApiBaseUrl = Environment.GetEnvironmentVariable("RESEND_BASE_URL") ??
                                      configuration["Resend:BaseUrl"] ??
                                      "https://api.resend.com";
            services.AddHttpClient<IResend, ResendClient>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(resendApiBaseUrl);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {resendApiKey}");
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
                tags: new[] { "db", "postgresql" });
        }

        // Redis health check (if configured)
        string? redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ??
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
