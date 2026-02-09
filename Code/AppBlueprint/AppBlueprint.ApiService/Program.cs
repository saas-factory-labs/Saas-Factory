using AppBlueprint.Infrastructure;
using AppBlueprint.Infrastructure.Extensions;
using AppBlueprint.Presentation.ApiModule.Extensions;
using AppBlueprint.Presentation.ApiModule.Middleware;
using AppBlueprint.ServiceDefaults;
using AppBlueprint.TodoAppKernel.Infrastructure;
using NSwag;
using NSwag.Generation.Processors.Security;


namespace AppBlueprint.ApiService;

internal static class Program // Make class static
{
    public static async Task Main(string[] args) // Make Main static
    {
        // DIAGNOSTIC: Check what ApiService receives from AppHost
        string? connString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING");
        bool hasPassword = HasPassword(connString);
        Console.WriteLine($"[ApiService] DATABASE_CONNECTIONSTRING received - Contains Password: {hasPassword}");
        if (!hasPassword)
        {
            Console.WriteLine("[ApiService] ERROR: ApiService did NOT receive password from AppHost!");
        }

        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire components.
        builder.AddServiceDefaults();

        // Explicitly add console logger and set minimum level
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Trace); // Set to Trace to capture everything

        builder.Host.UseDefaultServiceProvider(options =>
       {
           options.ValidateScopes = true;
           options.ValidateOnBuild = true;
       });

        // Port configuration handled by ASPNETCORE_URLS environment variable:
        // - Development: Set via launchSettings.json or appsettings (http://localhost:8081)
        // - Production: Set via Dockerfile ENV ASPNETCORE_URLS=http://+:80 (Railway handles SSL termination)
        // No ConfigureKestrel needed - avoids conflicts with environment variables

        // Add services to the container.
        builder.Services.AddProblemDetails();

        // Add HttpContextAccessor - required by Infrastructure layer
        builder.Services.AddHttpContextAccessor();

        // Register configuration options with validation
        builder.Services.AddAppBlueprintConfiguration(builder.Configuration, builder.Environment);

        builder.Services.AddAppBlueprintInfrastructure(builder.Configuration, builder.Environment);

        // Add Stripe webhook service
        builder.Services.AddStripeWebhookService(builder.Configuration);

        // Add TodoAppKernel module (includes TodoDbContext and TodoRepository)
        builder.Services.AddTodoAppKernel(builder.Configuration);

        // Add Presentation layer (includes Controllers, API Versioning, CORS, etc.)
        builder.Services.AddAppBlueprintPresentation(builder.Environment, builder.Configuration);

        // Add JWT authentication
        builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);

        // Add NSwag OpenAPI document generation
        builder.Services.AddOpenApiDocument(config =>
        {
            config.DocumentName = "v1";
            config.Title = "AppBlueprint API";

            // Add a security definition for Bearer authentication
            config.AddSecurity("BearerAuth", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\""
            });

            // Apply the security definition globally
            config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("BearerAuth"));
        });

        var app = builder.Build();

        app.UseCustomMiddlewares();

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();

            await MigrationExtensions.ApplyMigrationsAsync(app);
            await MigrationExtensions.ApplyDatabaseSeedingAsync(app);
        }

        // Add TenantMiddleware AFTER authentication middleware
        app.UseMiddleware<TenantMiddleware>();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();

        // Redirect root path to Swagger UI for easier access from Aspire dashboard
        app.MapGet("/", () => Results.Redirect("/swagger"));

        app.MapDefaultEndpoints();
        app.MapControllers();

        await app.RunAsync();
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
}
