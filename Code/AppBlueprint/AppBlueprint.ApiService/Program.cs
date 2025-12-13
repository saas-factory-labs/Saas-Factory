using AppBlueprint.Infrastructure;
using AppBlueprint.Infrastructure.Extensions;
using AppBlueprint.Presentation.ApiModule.Extensions;
using AppBlueprint.Presentation.ApiModule.Middleware;
using AppBlueprint.TodoAppKernel.Infrastructure;
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

        // Configure Kestrel ports based on environment
        builder.WebHost.ConfigureKestrel(options =>
        {
            if (builder.Environment.IsDevelopment())
            {
                // Development: Use non-privileged ports that don't require admin
                options.ListenAnyIP(8081); // HTTP on 8081
                options.ListenAnyIP(8082, listenOptions =>
                {
                    listenOptions.UseHttps();
                });
                Console.WriteLine("[API] Development mode - HTTP (8081) and HTTPS (8082) enabled");
            }
            else
            {
                // Production (Railway): Use port 80 as Railway expects
                // TLS is handled at the edge/load balancer
                options.ListenAnyIP(80);
                Console.WriteLine("[API] Production mode - HTTP (80) enabled, TLS handled by Railway");
            }
        });

        // Add services to the container.
        builder.Services.AddProblemDetails();

        // Add HttpContextAccessor - required by Infrastructure layer
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAppBlueprintInfrastructure(builder.Configuration, builder.Environment);
        
        // Add TodoAppKernel module (includes TodoDbContext and TodoRepository)
        builder.Services.AddTodoAppKernel(builder.Configuration);
        
        // Add Presentation layer (includes Controllers, API Versioning, CORS, etc.)
        builder.Services.AddAppBlueprintPresentation();

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
}
