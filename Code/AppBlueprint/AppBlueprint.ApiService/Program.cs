using AppBlueprint.Infrastructure;
using AppBlueprint.Presentation.ApiModule.Extensions;
using AppBlueprint.Presentation.ApiModule.Middleware;
using NSwag;
using NSwag.Generation.Processors.Security;


namespace AppBlueprint.ApiService;

internal static class Program // Make class static
{
    public static async Task Main(string[] args) // Make Main static
    {
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

        // Configure Kestrel to use port 80 for Railway compatibility
        builder.WebHost.ConfigureKestrel(options =>
        {
            // Always listen on HTTP port 80 (Railway requirement)
            options.ListenAnyIP(80);
            
            // Only configure HTTPS in Development mode
            // In production (Railway), TLS is handled at the edge/load balancer
            if (builder.Environment.IsDevelopment())
            {
                // In development, also listen on 443 for HTTPS
                options.ListenAnyIP(443, listenOptions =>
                {
                    listenOptions.UseHttps();
                });
                Console.WriteLine("[API] Development mode - HTTP (80) and HTTPS (443) enabled");
            }
            else
            {
                Console.WriteLine("[API] Production mode - HTTP only (80), HTTPS handled by Railway edge");
            }
        });

        // Add services to the container.
        builder.Services.AddProblemDetails();

        builder.Services.AddAppBlueprintServices();

        // Add CORS support for development
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Add JWT authentication
        builder.Services.AddJwtAuthentication(builder.Configuration);

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

        builder.Services.AddControllers(); var app = builder.Build();

        app.UseCustomMiddlewares();

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();

            await MigrationExtensions.ApplyMigrationsAsync(app);
            await MigrationExtensions.ApplyDatabaseSeedingAsync(app);
        }

        // Add TenantMiddleware AFTER OpenAPI/Swagger middleware to allow access to documentation
        app.UseMiddleware<TenantMiddleware>();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();
        
        // Add CORS middleware (must be before authentication)
        app.UseCors();
        
        // Re-enabled authentication and authorization middleware
        app.UseAuthentication();
        app.UseAuthorization();

        // Redirect root path to Swagger UI for easier access from Aspire dashboard
        app.MapGet("/", () => Results.Redirect("/swagger"));

        app.MapDefaultEndpoints();
        app.MapControllers();

        await app.RunAsync();
    }
}
