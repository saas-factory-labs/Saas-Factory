using System;
using System.Threading.Tasks;
using AppBlueprint.Infrastructure;
using AppBlueprint.Presentation.ApiModule.Extensions;
using AppBlueprint.Presentation.ApiModule.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
using NSwag.Generation.Processors.Security;


namespace AppBlueprint.ApiService;

public static class Program // Make class static
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

        // Add services to the container.
        builder.Services.AddProblemDetails();

        builder.Services.AddAppBlueprintServices();

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

        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseCustomMiddlewares();
        app.UseMiddleware<TenantMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();

            await MigrationExtensions.ApplyMigrationsAsync(app).ConfigureAwait(false);
            await MigrationExtensions.ApplyDatabaseSeedingAsync(app).ConfigureAwait(false);
        }

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();

        app.MapDefaultEndpoints();
        app.MapControllers();

        app.Run();
    }
}


