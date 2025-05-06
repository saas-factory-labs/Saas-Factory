using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using AppBlueprint.Infrastructure.DatabaseContexts;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Presentation.ApiModule.Extensions;

public static class ApplicationBuilderExtensions
{
    private static readonly ILogger Logger = LoggerFactory
        .Create(builder => builder.AddConsole())
        .CreateLogger(typeof(ApplicationBuilderExtensions));

    private static readonly Action<ILogger, string, Exception?> LogMode =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(LogMode)),
            "Configuration mode: {Mode}");

    private static readonly Action<ILogger, string, Exception?> LogConnectionString =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(2, nameof(LogConnectionString)),
            "Connection String: {ConnectionString}");

    private static void AddDbContext(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        // Get connection string from IConfiguration (set by ApiService/Program.cs)
        string? databaseConnectionString = configuration.GetConnectionString("DefaultConnection");

        Console.WriteLine($"[AddDbContext] Database Connection String from IConfiguration: {databaseConnectionString}"); // Added logging

        // Throw if connection string is null or empty
        ArgumentException.ThrowIfNullOrEmpty(databaseConnectionString, nameof(databaseConnectionString));

        LogConnectionString(Logger, databaseConnectionString, null); // Existing logging

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
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        
        // Add API versioning services
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        })
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
                
                // Don't validate the audience as Supabase doesn't use it by default
                ValidateAudience = false,
                
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

        // Pass IConfiguration to AddDbContext
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
