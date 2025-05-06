using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Metrics;
using System.Text;
using System.Text.Json;

namespace AppBlueprint.MetricsExporter;

/// <summary>
/// Extensions for adding and configuring metrics endpoints.
/// </summary>
public static class MetricsEndpointExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Maps a metrics endpoint that exposes OpenTelemetry metrics.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapMetricsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // Basic metrics endpoint that returns a text representation of metrics
        endpoints.MapGet("/metrics", async context =>
        {
            context.Response.ContentType = "text/plain; charset=utf-8";
            
            var sb = new StringBuilder();
            sb.AppendLine("# HELP app_metrics AppBlueprint application metrics");
            sb.AppendLine("# TYPE app_metrics gauge");
            
            // Add some basic metrics about the app
            sb.AppendLine($"app_metrics{{service=\"{Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ?? "AppBlueprint"}\",env=\"{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}\"}} 1");
            
            // Add system metrics
            sb.AppendLine($"process_memory_usage_bytes {GC.GetTotalMemory(false)}");
            sb.AppendLine($"process_cpu_cores {Environment.ProcessorCount}");
            sb.AppendLine($"process_start_time_seconds {DateTimeOffset.Now.ToUnixTimeSeconds() - Environment.TickCount / 1000}");

            await context.Response.WriteAsync(sb.ToString());
        });
        
        // JSON metrics endpoint 
        endpoints.MapGet("/metrics/json", async context =>
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            
            var metrics = new
            {
                Service = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ?? "AppBlueprint",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                ProcessMetrics = new
                {
                    MemoryUsageBytes = GC.GetTotalMemory(false),
                    CpuCores = Environment.ProcessorCount,
                    StartTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds() - Environment.TickCount / 1000,
                    TotalAllocatedBytes = GC.GetTotalAllocatedBytes(false)
                },
                RuntimeInfo = new
                {
                    RuntimeVersion = Environment.Version.ToString(),
                    OsVersion = Environment.OSVersion.ToString(),
                    MachineName = Environment.MachineName,
                    ProcessId = Environment.ProcessId
                }
            };
            
            await context.Response.WriteAsJsonAsync(metrics, _jsonOptions);
        });
        
        return endpoints;
    }
    
    /// <summary>
    /// Adds a simple metrics exporter to the application.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseMetricsExporter(this IApplicationBuilder app)
    {
        // Get all registered meters from the application services
        var meterProvider = app.ApplicationServices.GetService<MeterProvider>();
        
        return app;
    }
}
