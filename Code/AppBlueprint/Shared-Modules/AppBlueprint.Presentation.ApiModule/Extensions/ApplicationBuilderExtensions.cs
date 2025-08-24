using System.Text;
using AppBlueprint.Application.Services.DataExport;
using AppBlueprint.Infrastructure.Services.DataExport;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Application.Interfaces.UnitOfWork;
using Asp.Versioning;
using Asp.Versioning.Routing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AppBlueprint.Presentation.ApiModule.Extensions;

public static class ApplicationBuilderExtensions
{
    private static readonly ILogger Logger = LoggerFactory
        .Create(builder => builder.AddConsole())
        .CreateLogger(typeof(ApplicationBuilderExtensions)); private static void AddDbContext(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        // Get connection string from IConfiguration - try Aspire database name first, then fallbacks
        string? databaseConnectionString = configuration.GetConnectionString("appblueprintdb") ??
                                         configuration.GetConnectionString("postgres-server") ??
                                         configuration.GetConnectionString("DefaultConnection");

        Console.WriteLine($"[AddDbContext] Database Connection String from IConfiguration: {databaseConnectionString}"); // Added logging

        // Throw if connection string is null or empty
        ArgumentException.ThrowIfNullOrEmpty(databaseConnectionString, nameof(databaseConnectionString));

        Logger.LogInformation("Connection String: {ConnectionString}", databaseConnectionString); // Existing logging

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

    private static void ConfigureJwtAuthentication(IServiceCollection services)
    {
        // Configure JWT Bearer authentication for Supabase
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Get the JWT secret from environment variables
            string supabaseJwtSecret = Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET") ?? "your-supabase-jwt-secret";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                // Validate the token issuer (Supabase)
                ValidateIssuer = true,
                ValidIssuer = "supabase",

                // Validate the audience for security (Supabase uses "authenticated" for authenticated users)
                ValidateAudience = true,
                ValidAudience = "authenticated",

                // Validate the token's signature
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(supabaseJwtSecret)),

                // Validate the token's lifetime
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Don't allow any clock skew
            };

            // Validate the token as soon as it's received
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // Set to true in production

            // Handle events
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Append("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        // Add authorization services
        services.AddAuthorization();
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

        AddDbContext(services, configuration);
        ConfigureApiVersioning(services);
        AddCors(services);
        ConfigureJwtAuthentication(services);

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
}
