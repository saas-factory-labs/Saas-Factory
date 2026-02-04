using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Configuration;
using AppBlueprint.Infrastructure.DatabaseContexts.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Configuration;

/// <summary>
/// Provides methods for configuring and registering DbContext instances based on application requirements.
/// Supports Baseline, B2C, and B2B contexts with flexible configuration.
/// </summary>
public static class DbContextConfigurator
{
    /// <summary>
    /// Registers the appropriate DbContext based on configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddConfiguredDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Bind and validate DatabaseContext configuration
        services.Configure<DatabaseContextOptions>(configuration.GetSection(DatabaseContextOptions.SectionName));
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DatabaseContextOptions>>().Value;
            options.Validate();
            return options;
        });

        // Get configuration
        var dbContextOptions = configuration
            .GetSection(DatabaseContextOptions.SectionName)
            .Get<DatabaseContextOptions>() ?? new DatabaseContextOptions();

        dbContextOptions.Validate();

        // Get connection string with priority: Environment Variable > Configuration
        string? connectionString = GetConnectionString(configuration, dbContextOptions);

        // Validate connection string
        ConfigurationValidator.ValidateDatabaseConnectionString(
            connectionString,
            "appblueprintdb",
            "postgres-server",
            dbContextOptions.ConnectionStringName);

        var connectionSource = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") != null
            ? "Environment Variable"
            : $"Configuration ({dbContextOptions.ConnectionStringName})";

        Console.WriteLine($"[DbContextConfigurator] Database Connection Source: {connectionSource}");
        Console.WriteLine($"[DbContextConfigurator] Context Type: {dbContextOptions.ContextType}");
        Console.WriteLine($"[DbContextConfigurator] Baseline Only: {dbContextOptions.BaselineOnly}");
        Console.WriteLine($"[DbContextConfigurator] Hybrid Mode: {dbContextOptions.EnableHybridMode}");

        // Register DbContext based on configuration
        if (dbContextOptions.BaselineOnly)
        {
            RegisterBaselineDbContext(services, connectionString, dbContextOptions);
        }
        else
        {
            switch (dbContextOptions.ContextType)
            {
                case DatabaseContextType.Baseline:
                    RegisterBaselineDbContext(services, connectionString, dbContextOptions);
                    break;

                case DatabaseContextType.B2C:
                    RegisterB2CDbContext(services, connectionString, dbContextOptions);
                    break;

                case DatabaseContextType.B2B:
                    RegisterB2BDbContext(services, connectionString, dbContextOptions);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported DatabaseContextType: {dbContextOptions.ContextType}");
            }
        }

        // If hybrid mode is enabled, register all contexts
        if (dbContextOptions.EnableHybridMode)
        {
            Console.WriteLine("[DbContextConfigurator] Hybrid Mode: Registering all DbContext types");
            RegisterAllDbContexts(services, connectionString, dbContextOptions);
        }

        return services;
    }

    private static string? GetConnectionString(IConfiguration configuration, DatabaseContextOptions options)
    {
        // Priority 1: Environment variable
        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");

        // Priority 2: Configuration using specified connection string name
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = configuration.GetConnectionString(options.ConnectionStringName);
        }

        // Priority 3: Fallback to common connection string names
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = configuration.GetConnectionString("appblueprintdb") ??
                             configuration.GetConnectionString("postgres-server") ??
                             configuration.GetConnectionString("DefaultConnection");
        }

        return connectionString;
    }

    private static void RegisterBaselineDbContext(
        IServiceCollection services,
        string connectionString,
        DatabaseContextOptions options)
    {
        services.AddDbContext<Baseline.BaselineDbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(serviceProvider, dbOptions, connectionString, options);
        });

        Console.WriteLine("[DbContextConfigurator] Registered: BaselineDbContext");
    }

    private static void RegisterB2CDbContext(
        IServiceCollection services,
        string connectionString,
        DatabaseContextOptions options)
    {
        // Register B2CDbContext (includes Baseline entities)
        services.AddDbContext<B2C.B2CdbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(serviceProvider, dbOptions, connectionString, options);
        });

        // Register ApplicationDbContext as scoped (required by repositories and services)
        services.AddDbContext<ApplicationDbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(serviceProvider, dbOptions, connectionString, options);
        });

        // Register ApplicationDbContext factory (required by SignupService for non-scoped scenarios)
        // Note: Scoped interceptors cannot be added during factory configuration
        // Custom factory to explicitly pass null for ITenantContextAccessor (cannot resolve scoped service from singleton factory)
        services.AddSingleton<IDbContextFactory<ApplicationDbContext>>(serviceProvider =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            ConfigureNpgsqlOptions(serviceProvider, optionsBuilder, connectionString, options, isFactory: true);
            var dbContextOptions = optionsBuilder.Options;
            
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<ApplicationDbContext>();
            
            return new ApplicationDbContextFactory(dbContextOptions, configuration, logger);
        });

        Console.WriteLine("[DbContextConfigurator] Registered: B2CdbContext, ApplicationDbContext");
    }

    private static void RegisterB2BDbContext(
        IServiceCollection services,
        string connectionString,
        DatabaseContextOptions options)
    {
        // Register B2BDbContext (includes Baseline entities)
        services.AddDbContext<B2B.B2BDbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(serviceProvider, dbOptions, connectionString, options);
        });

        Console.WriteLine("[DbContextConfigurator] Registered: B2BDbContext");
    }

    private static void RegisterAllDbContexts(
        IServiceCollection services,
        string connectionString,
        DatabaseContextOptions options)
    {
        // Register Baseline
        services.AddDbContext<Baseline.BaselineDbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(serviceProvider, dbOptions, connectionString, options);
        });

        // Register B2C
        services.AddDbContext<B2C.B2CdbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(serviceProvider, dbOptions, connectionString, options);
        });

        // Register ApplicationDbContext as scoped (required by repositories and services)
        services.AddDbContext<ApplicationDbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(serviceProvider, dbOptions, connectionString, options);
        });

        // Register ApplicationDbContext factory (required by SignupService for non-scoped scenarios)
        // Note: Cannot use both AddDbContext and AddDbContextFactory - factory is Singleton, DbContext is Scoped
        // Note: Scoped interceptors cannot be added during factory configuration
        // Custom factory to explicitly pass null for ITenantContextAccessor (cannot resolve scoped service from singleton factory)
        services.AddSingleton<IDbContextFactory<ApplicationDbContext>>(serviceProvider =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            ConfigureNpgsqlOptions(serviceProvider, optionsBuilder, connectionString, options, isFactory: true);
            var dbContextOptions = optionsBuilder.Options;
            
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<ApplicationDbContext>();
            
            return new ApplicationDbContextFactory(dbContextOptions, configuration, logger);
        });

        // Register B2B
        services.AddDbContext<B2B.B2BDbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(serviceProvider, dbOptions, connectionString, options);
        });

        Console.WriteLine("[DbContextConfigurator] Hybrid Mode: All contexts registered");
    }

    private static void ConfigureNpgsqlOptions(
        IServiceProvider serviceProvider,
        DbContextOptionsBuilder dbOptions,
        string connectionString,
        DatabaseContextOptions options,
        bool isFactory = false)
    {
        // Create NpgsqlDataSource with EnableDynamicJson for JSONB support (required since Npgsql 8.0)
        // This is required for Dictionary<string, string> and other dynamic JSON types
        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        dbOptions.UseNpgsql(dataSource, npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(options.CommandTimeout);
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: options.MaxRetryCount,
                maxRetryDelay: TimeSpan.FromSeconds(options.MaxRetryDelaySeconds),
                errorCodesToAdd: null);
        });

        // Register TenantConnectionInterceptor for RLS session variable configuration
        // This interceptor sets app.current_tenant_id on every database connection
        // for PostgreSQL Row-Level Security (defense-in-depth Layer 2)
        // Note: Cannot add scoped interceptors to DbContextFactory (which uses root provider)
        // For factory contexts, interceptor must be added at context creation time
        if (!isFactory)
        {
            var tenantInterceptor = serviceProvider.GetService<TenantConnectionInterceptor>();
            if (tenantInterceptor is not null)
            {
                dbOptions.AddInterceptors(tenantInterceptor);
            }
            
            var tenantSecurityInterceptor = serviceProvider.GetService<TenantSecurityInterceptor>();
            if (tenantSecurityInterceptor is not null)
            {
                dbOptions.AddInterceptors(tenantSecurityInterceptor);
            }

            var tenantRlsInterceptor = serviceProvider.GetService<TenantRlsInterceptor>();
            if (tenantRlsInterceptor is not null)
            {
                dbOptions.AddInterceptors(tenantRlsInterceptor);
            }
        }

        dbOptions.ConfigureWarnings(warnings =>
        {
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId
                .PendingModelChangesWarning);
            warnings.Log(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ShadowPropertyCreated);
        });
    }
}
