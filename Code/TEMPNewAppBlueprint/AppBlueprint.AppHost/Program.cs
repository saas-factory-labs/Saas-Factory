using AppBlueprint.AppHost;
using System;
using System.Threading.Tasks;

namespace AppBlueprint.AppHost;

internal sealed class Program
{
    public static async Task Main(string[] args)
    {
        // Setup OpenTelemetry environment variables
        ConfigureOpenTelemetry();

        var builder = DistributedApplication.CreateBuilder(args);

        var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

        // Add PostgreSQL database
        var postgres = builder.AddPostgres("postgres")
            .WithEnvironment("POSTGRES_PASSWORD", postgresPassword)
            .WithEnvironment("POSTGRES_USER", "postgres")
            .WithEnvironment("POSTGRES_DB", "appblueprint");

        // Add API service with OpenTelemetry and PostgreSQL reference
        var apiService = builder.AddProject<Projects.AppBlueprint_ApiService>("apiservice")
            .WithReference(postgres, "PostgreSQL")
            .WithTelemetryDefaults("AppBlueprint-Api");

        // Add Web frontend with API reference and OpenTelemetry
        builder.AddProject<Projects.AppBlueprint_Web>("webfrontend")
            .WithExternalHttpEndpoints()
            .WithReference(apiService)
            .WithReference(postgres, "PostgreSQL")
            .WithTelemetryDefaults("AppBlueprint-Web");

        await builder.Build().RunAsync();
    }

    private static void ConfigureOpenTelemetry()
    {
        // Set environment variables for OpenTelemetry
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
        Environment.SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");
        
        // Check for Aspire dashboard OTLP endpoint first
        string? dashboardEndpoint = Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");
        
        // If dashboard endpoint is set, use it for OTEL exporter
        if (!string.IsNullOrEmpty(dashboardEndpoint))
        {
            // Ensure we're using http:// for local connections
            if (dashboardEndpoint.Contains("localhost") && dashboardEndpoint.StartsWith("https://"))
            {
                dashboardEndpoint = dashboardEndpoint.Replace("https://", "http://");
            }
            
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", dashboardEndpoint);
            Console.WriteLine($"Using Aspire dashboard OTLP endpoint: {dashboardEndpoint}");
        }
        else
        {
            // Default OTLP endpoint for Aspire dashboard collector
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889");
            Console.WriteLine($"Using default OTLP endpoint: http://localhost:18889");
        }
        
        // Disable TLS certificate validation for local development
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_CERTIFICATE", "");
    }
}
