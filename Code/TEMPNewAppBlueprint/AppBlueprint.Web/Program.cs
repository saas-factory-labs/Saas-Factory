using AppBlueprint.Web;
using AppBlueprint.Web.Components;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace AppBlueprint.Web;

internal sealed class Program
{
    public static async Task Main(string[] args)
    {
        // Add instrumentation for telemetry tracking
        using var activitySource = new ActivitySource("AppBlueprint.Web");
        using var meter = new Meter("AppBlueprint.Web", "1.0.0");
        var pageViewCounter = meter.CreateCounter<long>("appblueprint.web.pageviews", "PageView", "The number of page views");

        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire components.
        builder.AddServiceDefaults();

        // Add PostgreSQL connection for health checks
        var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
        if (!string.IsNullOrEmpty(connectionString))
        {
            // Add PostgreSQL health check
            var healthTags = new[] { "database", "postgres" };
            builder.Services.AddHealthChecks()
                .AddCheck("postgres-config", () => 
                {
                    try
                    {
                        // This is a simple check - in a real app, you might use an actual connection test
                        if (!string.IsNullOrEmpty(connectionString))
                            return HealthCheckResult.Healthy("PostgreSQL configuration is available");
                        else
                            return HealthCheckResult.Degraded("PostgreSQL connection string is empty");
                    }
                    catch (Exception ex)
                    {
                        return HealthCheckResult.Unhealthy($"PostgreSQL error: {ex.Message}");
                    }
                }, healthTags);
        }

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddOutputCache();

        builder.Services.AddHttpClient<WeatherApiClient>(client =>
            {
                // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
                // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
                client.BaseAddress = new("https+http://apiservice");
            });

        var app = builder.Build();

        // Add telemetry middleware
        app.Use(async (context, next) =>
        {
            // Track page views with OpenTelemetry
            using var activity = activitySource.StartActivity("PageView");
            activity?.SetTag("page.path", context.Request.Path);
            activity?.SetTag("user.agent", context.Request.Headers.UserAgent.ToString());
            
            // Count the page view
            pageViewCounter.Add(1);
            
            await next();
        });

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        
        app.UseStaticFiles();
        app.UseAntiforgery();
        
        app.UseOutputCache();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.MapDefaultEndpoints();

        await app.RunAsync();
    }
}
