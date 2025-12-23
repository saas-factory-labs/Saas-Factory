using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            ConfigureNpgsqlOptions(dbOptions, connectionString, options);
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
            ConfigureNpgsqlOptions(dbOptions, connectionString, options);
        });

        // Also register ApplicationDbContext which extends B2CDbContext
        services.AddDbContext<ApplicationDbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(dbOptions, connectionString, options);
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
            ConfigureNpgsqlOptions(dbOptions, connectionString, options);
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
            ConfigureNpgsqlOptions(dbOptions, connectionString, options);
        });

        // Register B2C
        services.AddDbContext<B2C.B2CdbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(dbOptions, connectionString, options);
        });

        services.AddDbContext<ApplicationDbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(dbOptions, connectionString, options);
        });

        // Register B2B
        services.AddDbContext<B2B.B2BDbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(dbOptions, connectionString, options);
        });

        Console.WriteLine("[DbContextConfigurator] Hybrid Mode: All contexts registered");
    }

    private static void ConfigureNpgsqlOptions(
        DbContextOptionsBuilder dbOptions,
        string connectionString,
        DatabaseContextOptions options)
    {
        dbOptions.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(options.CommandTimeout);
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: options.MaxRetryCount,
                maxRetryDelay: TimeSpan.FromSeconds(options.MaxRetryDelaySeconds),
                errorCodesToAdd: null);
        });

        dbOptions.ConfigureWarnings(warnings =>
        {
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId
                .PendingModelChangesWarning);
            warnings.Log(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ShadowPropertyCreated);
        });
    }
}
