using AppBlueprint.Application.Services.DataExport;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Presentation.ApiModule.Extensions;

public static class ApplicationBuilderExtensions
{
    private static readonly ILogger Logger = LoggerFactory
        .Create(builder => builder.AddConsole())
        .CreateLogger(typeof(ApplicationBuilderExtensions)); private static void AddDbContext(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        // Priority 1: Check for environment variable for cloud database connection
        string? databaseConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
        
        // Priority 2: Fall back to IConfiguration connection strings
        if (string.IsNullOrEmpty(databaseConnectionString))
        {
            databaseConnectionString = configuration.GetConnectionString("appblueprintdb") ??
                                     configuration.GetConnectionString("postgres-server") ??
                                     configuration.GetConnectionString("DefaultConnection");
        }

        var connectionSource = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") != null ? "Environment Variable" : "Configuration";
        
        Console.WriteLine($"[AddDbContext] Database Connection String Source: {connectionSource}");
        
        // Log detailed connection information
        LogDatabaseConnectionInfo(databaseConnectionString);

        // Throw if connection string is null or empty
        ArgumentException.ThrowIfNullOrEmpty(databaseConnectionString, nameof(databaseConnectionString));

        Logger.LogInformation("Connection String configured from: {Source}", connectionSource);

        // Properly configure the ApplicationDbContext with Entity Framework Core
        serviceCollection.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            // Use the connection string from IConfiguration
            options.UseNpgsql(databaseConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(60);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });

            // Configure warnings to suppress the pending model changes warning in development
            options.ConfigureWarnings(warnings =>
            {
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning);

                // Also log the shadow property warnings instead of throwing
                warnings.Log(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ShadowPropertyCreated);
            });
        });

        // Register B2BDbContext with the same connection string
        serviceCollection.AddDbContext<B2BDbContext>((serviceProvider, options) =>
        {
            // Use the connection string from IConfiguration
            options.UseNpgsql(databaseConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(60);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });

            // Configure warnings to suppress the pending model changes warning in development
            options.ConfigureWarnings(warnings =>
            {
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning);

                // Also log the shadow property warnings instead of throwing
                warnings.Log(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ShadowPropertyCreated);
            });
        });
    }

    private static void LogDatabaseConnectionInfo(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("[AddDbContext] Connection string is NULL");
            return;
        }

        try
        {
            var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
            
            Console.WriteLine("[AddDbContext] === Database Connection Details ===");
            Console.WriteLine($"[AddDbContext] Host: {builder.Host ?? "not specified"}");
            Console.WriteLine($"[AddDbContext] Port: {builder.Port}");
            Console.WriteLine($"[AddDbContext] Database: {builder.Database ?? "not specified"}");
            Console.WriteLine($"[AddDbContext] Username: {builder.Username ?? "not specified"}");
            Console.WriteLine($"[AddDbContext] SSL Mode: {builder.SslMode}");
            
            var isLocal = builder.Host?.Contains("localhost") == true || 
                         builder.Host?.Contains("127.0.0.1") == true ||
                         builder.Host?.Contains("postgres-server") == true;
            Console.WriteLine($"[AddDbContext] Connection Type: {(isLocal ? "LOCAL" : "CLOUD/REMOTE")}");
            Console.WriteLine("[AddDbContext] ==========================================");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AddDbContext] Failed to parse connection details: {ex.Message}");
            Console.WriteLine($"[AddDbContext] Masked Connection String: {MaskConnectionString(connectionString)}");
        }
    }

    private static void AddCors(IServiceCollection serviceCollection)
    {
        serviceCollection.AddCors(policy => policy.AddDefaultPolicy(builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }));
    }
    private static void ConfigureApiVersioning(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        // Add API versioning services with URL path support
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);

            // Configure to read API version from URL path
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader("apiVersion"),
                new HeaderApiVersionReader("X-Version")
            );
        })
        .AddMvc() // This registers the apiVersion route constraint
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    public static IServiceCollection AddAppBlueprintServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Get IConfiguration from the service provider
        var serviceProvider = services.BuildServiceProvider(); // Build temporary provider to get config
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();
        services.AddAntiforgery();
        services.AddHttpContextAccessor();

        services.AddScoped<IDataExportService, AppBlueprint.Infrastructure.Services.DataExport.DataExportService>();
        services.AddScoped<IDataExportRepository, DataExportRepository>();       
        services.AddScoped<IUnitOfWork, AppBlueprint.Infrastructure.UnitOfWork.Implementation.UnitOfWork>();
        
        // Register TodoRepository
        services.AddScoped<AppBlueprint.TodoAppKernel.Repositories.ITodoRepository, TodoRepository>();

        AddDbContext(services, configuration);
        ConfigureApiVersioning(services);
        AddCors(services);

        return services;
    }

    public static WebApplication ConfigureApplication(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    private static string MaskConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "NULL";
        
        // Mask password in connection string for logging
        var parts = connectionString.Split(';');
        var masked = new List<string>();
        foreach (var part in parts)
        {
            if (part.Contains("Password=", StringComparison.OrdinalIgnoreCase) || 
                part.Contains("Pwd=", StringComparison.OrdinalIgnoreCase))
            {
                var key = part.Split('=')[0];
                masked.Add($"{key}=***");
            }
            else
            {
                masked.Add(part);
            }
        }
        return string.Join(";", masked);
    }
}
