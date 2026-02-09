using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Configuration;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using AppBlueprint.Infrastructure.DatabaseContexts.B2C;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;
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

        // Get configuration from appsettings.json first
        var dbContextOptions = configuration
            .GetSection(DatabaseContextOptions.SectionName)
            .Get<DatabaseContextOptions>() ?? new DatabaseContextOptions();

        // Override with environment variables
        ApplyEnvironmentVariableOverrides(dbContextOptions);
        dbContextOptions.Validate();

        // Get connection string with priority: Environment Variable > Configuration
        string? connectionString = GetConnectionString(configuration, dbContextOptions);

        // DIAGNOSTIC: Check if password present after GetConnectionString
        bool hasPasswordBeforeValidation = HasPassword(connectionString);
        Console.WriteLine($"[DbContextConfigurator] DIAGNOSTIC - After GetConnectionString - Has Password: {hasPasswordBeforeValidation}");
        if (!hasPasswordBeforeValidation)
        {
            Console.WriteLine($"[DbContextConfigurator] ERROR - Connection string from GetConnectionString does NOT contain password!");
            Console.WriteLine($"[DbContextConfigurator] Connection string preview: {(connectionString != null ? string.Concat(connectionString.AsSpan(0, Math.Min(50, connectionString.Length)), "...") : "NULL")}");
        }

        // Validate connection string
        ConfigurationValidator.ValidateDatabaseConnectionString(
            connectionString,
            "appblueprintdb",
            "postgres-server",
            dbContextOptions.ConnectionStringName);

        // After validation, ensure connectionString is not null (validation throws if null, but compiler needs explicit check)
        if (connectionString is null)
        {
            throw new InvalidOperationException("Connection string validation failed unexpectedly.");
        }

        var connectionSource = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING") != null
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

    /// <summary>
    /// Applies environment variable overrides to the DatabaseContextOptions.
    /// </summary>
    private static void ApplyEnvironmentVariableOverrides(DatabaseContextOptions options)
    {
        // Override with flat UPPERCASE environment variables (following UPPERCASE_UNDERSCORE standard)
        const string Prefix = "DATABASECONTEXT_";

        // Check TYPE first (new standard), then CONTEXTTYPE (legacy)
        string? contextType = Environment.GetEnvironmentVariable($"{Prefix}TYPE")
                           ?? Environment.GetEnvironmentVariable($"{Prefix}CONTEXTTYPE");
        if (!string.IsNullOrWhiteSpace(contextType) && Enum.TryParse<DatabaseContextType>(contextType, ignoreCase: true, out DatabaseContextType parsedType))
            options.ContextType = parsedType;

        string? enableHybridMode = Environment.GetEnvironmentVariable($"{Prefix}ENABLEHYBRIDMODE");
        if (!string.IsNullOrWhiteSpace(enableHybridMode) && bool.TryParse(enableHybridMode, out bool hybridMode))
            options.EnableHybridMode = hybridMode;

        string? baselineOnly = Environment.GetEnvironmentVariable($"{Prefix}BASELINEONLY");
        if (!string.IsNullOrWhiteSpace(baselineOnly) && bool.TryParse(baselineOnly, out bool baseline))
            options.BaselineOnly = baseline;

        string? commandTimeout = Environment.GetEnvironmentVariable($"{Prefix}COMMANDTIMEOUT");
        if (!string.IsNullOrWhiteSpace(commandTimeout) && int.TryParse(commandTimeout, out int timeout))
            options.CommandTimeout = timeout;

        string? maxRetryCount = Environment.GetEnvironmentVariable($"{Prefix}MAXRETRYCOUNT");
        if (!string.IsNullOrWhiteSpace(maxRetryCount) && int.TryParse(maxRetryCount, out int retryCount))
            options.MaxRetryCount = retryCount;
    }

    /// <summary>
    /// Checks if a connection string contains a password in either key-value or URI format.
    /// </summary>
    /// <param name="connectionString">The connection string to check.</param>
    /// <returns>True if password is present, false otherwise.</returns>
    private static bool HasPassword(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;
        
        // Check key-value format: Password=...
        if (connectionString.Contains("Password=", StringComparison.OrdinalIgnoreCase))
            return true;
        
        // Check PostgreSQL URI format: postgresql://username:password@host:port/database
        if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
            connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            int schemeEnd = connectionString.IndexOf("://", StringComparison.Ordinal);
            int atIndex = connectionString.IndexOf('@', schemeEnd + 3);
            int colonIndex = connectionString.IndexOf(':', schemeEnd + 3);
            
            // Password exists if there's a colon between :// and @
            return colonIndex > schemeEnd && colonIndex < atIndex && atIndex > 0;
        }
        
        return false;
    }

    /// <summary>
    /// Normalizes a PostgreSQL connection string from URI format to key-value format.
    /// If already in key-value format, returns the string unchanged.
    /// 
    /// ROOT CAUSE OF CONNECTION FAILURE:
    /// NpgsqlDataSourceBuilder (used in ConfigureNpgsqlOptions below) CANNOT parse PostgreSQL URI format.
    /// When attempting to create a data source with URI format, Npgsql throws:
    ///   System.ArgumentException: "Format of the initialization string does not conform to specification starting at index 0"
    ///     at Npgsql.NpgsqlConnectionStringBuilder.GetProperty(String keyword)
    ///     at Npgsql.NpgsqlDataSourceBuilder..ctor(String connectionString)
    /// 
    /// Railway and many cloud providers use PostgreSQL URI format by default:
    ///   postgresql://user:password@host:port/database
    /// 
    /// But Npgsql's NpgsqlDataSourceBuilder requires key-value format:
    ///   Host=host;Port=port;Username=user;Password=password;Database=database
    /// 
    /// This method performs the necessary conversion to prevent the ArgumentException.
    /// </summary>
    /// <param name="connectionString">The connection string to normalize.</param>
    /// <returns>Connection string in key-value format.</returns>
    private static string NormalizeConnectionString(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        
        // If already in key-value format, return as-is
        if (!connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) &&
            !connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }
        
        try
        {
            // Parse URI format: postgresql://username:password@host:port/database?params
            var uri = new Uri(connectionString);
            
            string host = uri.Host;
            int port = uri.Port > 0 ? uri.Port : 5432;
            string database = uri.AbsolutePath.TrimStart('/');
            
            // Extract username and password from UserInfo (username:password)
            string? username = null;
            string? password = null;
            
            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                int colonIndex = uri.UserInfo.IndexOf(':', StringComparison.Ordinal);
                if (colonIndex >= 0)
                {
                    username = Uri.UnescapeDataString(uri.UserInfo.AsSpan(0, colonIndex).ToString());
                    password = Uri.UnescapeDataString(uri.UserInfo.AsSpan(colonIndex + 1).ToString());
                }
                else
                {
                    username = Uri.UnescapeDataString(uri.UserInfo);
                }
            }
            
            // Build key-value connection string
            var builder = new System.Text.StringBuilder();
            builder.Append(System.Globalization.CultureInfo.InvariantCulture, $"Host={host};");
            builder.Append(System.Globalization.CultureInfo.InvariantCulture, $"Port={port};");
            builder.Append(System.Globalization.CultureInfo.InvariantCulture, $"Database={database};");
            
            if (!string.IsNullOrEmpty(username))
            {
                builder.Append(System.Globalization.CultureInfo.InvariantCulture, $"Username={username};");
            }
            
            if (!string.IsNullOrEmpty(password))
            {
                builder.Append(System.Globalization.CultureInfo.InvariantCulture, $"Password={password};");
            }
            
            // Add query parameters if present
            if (!string.IsNullOrEmpty(uri.Query))
            {
                // Parse query string and add parameters
                string query = uri.Query.TrimStart('?');
                string[] parameters = query.Split('&', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (string param in parameters)
                {
                    int equalsIndex = param.IndexOf('=', StringComparison.Ordinal);
                    if (equalsIndex > 0)
                    {
                        string key = Uri.UnescapeDataString(param.AsSpan(0, equalsIndex).ToString());
                        string value = Uri.UnescapeDataString(param.AsSpan(equalsIndex + 1).ToString());
                        
                        // Convert common URI query parameters to Npgsql format
                        if (key.Equals("sslmode", StringComparison.OrdinalIgnoreCase))
                        {
                            builder.Append(System.Globalization.CultureInfo.InvariantCulture, $"SslMode={value};");
                        }
                        else
                        {
                            builder.Append(System.Globalization.CultureInfo.InvariantCulture, $"{key}={value};");
                        }
                    }
                }
            }
            
            string result = builder.ToString().TrimEnd(';');
            Console.WriteLine($"[DbContextConfigurator] Converted URI format to key-value format (password: {HasPassword(result)})");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DbContextConfigurator] ERROR: Failed to parse PostgreSQL URI: {ex.Message}");
            throw new InvalidOperationException($"Failed to parse PostgreSQL connection string URI format: {ex.Message}", ex);
        }
    }

    private static string? GetConnectionString(IConfiguration configuration, DatabaseContextOptions options)
    {
        // Priority 1: Environment variable
        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING");

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
        services.AddDbContext<BaselineDbContext>((serviceProvider, dbOptions) =>
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
        services.AddDbContext<B2CdbContext>((serviceProvider, dbOptions) =>
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
        services.AddDbContext<B2BDbContext>((serviceProvider, dbOptions) =>
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
        services.AddDbContext<BaselineDbContext>((serviceProvider, dbOptions) =>
        {
            ConfigureNpgsqlOptions(serviceProvider, dbOptions, connectionString, options);
        });

        // Register B2C
        services.AddDbContext<B2CdbContext>((serviceProvider, dbOptions) =>
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
        services.AddDbContext<B2BDbContext>((serviceProvider, dbOptions) =>
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
        // Normalize connection string to key-value format if it's in URI format
        string normalizedConnectionString = NormalizeConnectionString(connectionString);
        
        // Create NpgsqlDataSource with EnableDynamicJson for JSONB support (required since Npgsql 8.0)
        // This is required for Dictionary<string, string> and other dynamic JSON types
        var dataSourceBuilder = new Npgsql.NpgsqlDataSourceBuilder(normalizedConnectionString);

        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        dataSourceBuilder.ConfigureJsonOptions(jsonOptions);
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

            var piiInterceptor = serviceProvider.GetService<PiiSaveChangesInterceptor>();
            if (piiInterceptor is not null)
            {
                dbOptions.AddInterceptors(piiInterceptor);
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
