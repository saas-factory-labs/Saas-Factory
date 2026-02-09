using Amazon.Runtime;
using Amazon.S3;
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Interfaces.PII;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using AppBlueprint.Application.Options;
using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.Configuration;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Configuration;
using AppBlueprint.Infrastructure.DatabaseContexts.Interceptors;
using AppBlueprint.Infrastructure.DependencyInjection;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Infrastructure.Services;
using AppBlueprint.Infrastructure.Services.PII;
using AppBlueprint.Infrastructure.Services.Search;
using AppBlueprint.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Resend;
using B2BDbContext = AppBlueprint.Infrastructure.DatabaseContexts.B2B.B2BDbContext;
using BaselineDbContext = AppBlueprint.Infrastructure.DatabaseContexts.Baseline.BaselineDbContext;

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
        // Priority 1: Environment variable
        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING");

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
        services.AddScoped<TenantSecurityInterceptor>();
        services.AddScoped<TenantRlsInterceptor>();

        // User context and admin access
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<IAdminTenantAccessService, AdminTenantAccessService>();

        // Signup database context provider (EF Core)
        services.AddScoped<Application.Services.ISignupDbContextProvider, SignupDbContextProvider>();

        return services;
    }

    /// <summary>
    /// Registers Unit of Work pattern implementation from AppBlueprint.Infrastructure.
    /// </summary>
    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWorkImplementation>();
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
        services.AddStripeService();
        services.AddCloudflareR2Service();
        services.AddResendEmailService(configuration);

        return services;
    }

    /// <summary>
    /// Registers Stripe payment service if API key is configured.
    /// Uses StripeOptions from IOptions pattern.
    /// </summary>
    private static IServiceCollection AddStripeService(
        this IServiceCollection services)
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
        this IServiceCollection services)
    {
        // Get R2 options to check if configured
        IServiceProvider tempProvider = services.BuildServiceProvider();
        CloudflareR2Options? r2Options = tempProvider.GetService<IOptions<CloudflareR2Options>>()?.Value;

        if (r2Options is not null &&
            !string.IsNullOrWhiteSpace(r2Options.AccessKeyId) &&
            !string.IsNullOrWhiteSpace(r2Options.SecretAccessKey) &&
            !string.IsNullOrWhiteSpace(r2Options.EndpointUrl) &&
            (!string.IsNullOrWhiteSpace(r2Options.PrivateBucketName) || !string.IsNullOrWhiteSpace(r2Options.PublicBucketName)))
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

            // Register new file storage services
            services.AddScoped<IFileStorageService, R2FileStorageService>();
            services.AddScoped<IFileValidationService, FileValidationService>();
            services.AddScoped<IFileMetadataRepository, FileMetadataRepository>();

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
    /// Supports multiple environment variable naming conventions:
    /// 1. RESEND_APIKEY (standard UPPERCASE format)
    /// 2. RESEND_API_KEY (legacy with underscores)
    /// </summary>
    private static IServiceCollection AddResendEmailService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Try multiple environment variable naming conventions (in priority order)
        string? apiKey = Environment.GetEnvironmentVariable("RESEND_APIKEY")
                      ?? Environment.GetEnvironmentVariable("RESEND_API_KEY")
                      ?? configuration["Resend:ApiKey"];

        string? fromEmail = Environment.GetEnvironmentVariable("RESEND_FROMEMAIL")
                         ?? Environment.GetEnvironmentVariable("RESEND_FROM_EMAIL")
                         ?? configuration["Resend:FromEmail"];

        string? fromName = Environment.GetEnvironmentVariable("RESEND_FROMNAME")
                        ?? Environment.GetEnvironmentVariable("RESEND_FROM_NAME")
                        ?? configuration["Resend:FromName"];

        if (!string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(fromEmail))
        {
            var resendOptions = new ResendEmailOptions
            {
                ApiKey = apiKey,
                FromEmail = fromEmail,
                FromName = fromName,
                BaseUrl = configuration["Resend:BaseUrl"] ?? "https://api.resend.com",
                TimeoutSeconds = int.TryParse(configuration["Resend:TimeoutSeconds"], out int timeout) ? timeout : 30
            };

            services.AddSingleton<IOptions<ResendEmailOptions>>(new OptionsWrapper<ResendEmailOptions>(resendOptions));

            // Configure Resend according to official documentation
            services.AddOptions();
            services.AddHttpClient<ResendClient>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(resendOptions.BaseUrl, UriKind.Absolute);
                    client.Timeout = TimeSpan.FromSeconds(resendOptions.TimeoutSeconds);
                });
            services.Configure<ResendClientOptions>(o =>
            {
                o.ApiToken = resendOptions.ApiKey;
            });
            services.AddTransient<IResend, ResendClient>();

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
            
            string? connString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING") ??
                               config.GetConnectionString("appblueprintdb") ??
                               config.GetConnectionString("postgres-server");
            
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
            catch (Exception ex)
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
                var tablesWithoutRls = new List<string>();
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
            catch (Exception ex)
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
    /// Adds SignalR with tenant-aware authentication and authorization.
    /// Configures MessagePack protocol for efficient binary serialization.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppBlueprintSignalR(this IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            // Enable detailed errors in development only
            options.EnableDetailedErrors = false; // Set to true via environment variable in dev

            // Configure client timeouts
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            options.HandshakeTimeout = TimeSpan.FromSeconds(15);
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);

            // Maximum message size (1 MB default)
            options.MaximumReceiveMessageSize = 1024 * 1024;
        })
        .AddMessagePackProtocol(); // Binary protocol for better performance

        Console.WriteLine("[AppBlueprint.Infrastructure] SignalR registered with tenant-aware authentication");

        return services;
    }

    /// <summary>
    /// Adds PostgreSQL full-text search service for a specific entity type.
    /// Automatically respects tenant isolation for ITenantScoped entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to enable search for (must have SearchVector column)</typeparam>
    /// <typeparam name="TDbContext">The DbContext containing the entity</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// // Register search for Users (using B2BDbContext)
    /// services.AddPostgreSqlFullTextSearch&lt;UserEntity, B2BDbContext&gt;();
    /// 
    /// // Register search for Tenants (using BaselineDbContext)
    /// services.AddPostgreSqlFullTextSearch&lt;TenantEntity, BaselineDbContext&gt;();
    /// </example>
    public static IServiceCollection AddPostgreSqlFullTextSearch<TEntity, TDbContext>(this IServiceCollection services)
        where TEntity : class
        where TDbContext : DbContext
    {
        services.AddScoped<ISearchService<TEntity>, PostgreSqlSearchService<TEntity, TDbContext>>();

        Console.WriteLine($"[AppBlueprint.Infrastructure] PostgreSQL full-text search registered for {typeof(TEntity).Name}");

        return services;
    }

    /// <summary>
    /// Registers PII detection engine and scanners.
    /// </summary>
    private static IServiceCollection AddPIIServices(this IServiceCollection services)
    {
        services.AddScoped<IPIIScanner, RegexPIIScanner>();
        services.AddScoped<IPIIScanner, NerPIIScannerPlaceholder>();
        services.AddScoped<IPIIScanner, LlmPIIScannerPlaceholder>();
        services.AddScoped<IPIIEngine, PIIEngine>();
        services.AddScoped<PIITaggingService>();

        // Register the EF Core interceptor
        services.AddScoped<PiiSaveChangesInterceptor>();

        Console.WriteLine("[AppBlueprint.Infrastructure] PII services registered (Regex, NER/LLM Placeholders, Interceptor)");

        return services;
    }
}

