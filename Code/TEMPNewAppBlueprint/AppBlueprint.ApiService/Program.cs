using AppBlueprint.ApiService.Data;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace AppBlueprint.ApiService;

internal sealed class Program
{
    public static async Task Main(string[] args)
    {
        // Add instrumentation for telemetry tracking
        using var activitySource = new ActivitySource("AppBlueprint.ApiService");
        using var meter = new Meter("AppBlueprint.ApiService", "1.0.0");
        var requestCounter = meter.CreateCounter<long>("appblueprint.api.requests", "Request", "The number of requests received");

        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire components
        builder.AddServiceDefaults();

        // Get PostgreSQL connection string from configuration
        var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
        Console.WriteLine($"PostgreSQL connection string: {connectionString}");

        // Add services to the container.
        builder.Services.AddProblemDetails();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add DbContext using the PostgreSQL provider
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        // Add PostgreSQL health check
        // Create a static readonly array for health check tags to avoid CA1861 warning
        var healthTags = new[] { "database", "postgres" };
        builder.Services.AddHealthChecks()
            .AddCheck("postgres-connection", () => 
            {
                try
                {
                    using var dbContext = new ApplicationDbContext(
                        new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseNpgsql(connectionString)
                            .Options);
                            
                    if (dbContext.Database.CanConnect())
                        return HealthCheckResult.Healthy("PostgreSQL connection is healthy");
                    else
                        return HealthCheckResult.Unhealthy("Cannot connect to PostgreSQL");
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return HealthCheckResult.Unhealthy($"PostgreSQL error: {ex.Message}");
                }
            }, healthTags);

        var app = builder.Build();

        // Ensure database is created and migrations are applied
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);
        }

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();

        // Add Swagger middleware in development environment
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Add telemetry middleware
        app.Use(async (context, next) =>
        {
            // Track requests with OpenTelemetry
            using var activity = activitySource.StartActivity("HandleRequest");
            activity?.SetTag("http.path", context.Request.Path);
            activity?.SetTag("http.method", context.Request.Method);
            
            // Count the request
            requestCounter.Add(1);
            
            await next().ConfigureAwait(false);
        });

        app.MapGet("/weatherforecast", async (ApplicationDbContext db) =>
        {
            return await db.WeatherForecasts.ToListAsync().ConfigureAwait(false);
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

        // Add health check endpoints
        app.MapDefaultEndpoints();

        // Run the application with ConfigureAwait to prevent awaiter context issues
        await app.RunAsync().ConfigureAwait(false);
    }
}
