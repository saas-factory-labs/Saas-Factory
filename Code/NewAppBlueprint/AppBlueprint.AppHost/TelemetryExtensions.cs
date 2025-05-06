using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace AppBlueprint.AppHost;

internal static class TelemetryExtensions
{
    /// <summary>
    /// Adds default telemetry configuration to a resource.
    /// </summary>
    public static T WithTelemetryDefaults<T>(this T builder, string serviceName) where T : IResourceBuilder<ProjectResource>
    {
        // Configure standard OTLP settings
        builder.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", GetOtlpEndpoint());
        builder.WithEnvironment("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
        builder.WithEnvironment("OTEL_SERVICE_NAME", serviceName);
        builder.WithEnvironment("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");
        
        // Configure resource attributes
        builder.WithEnvironment("OTEL_RESOURCE_ATTRIBUTES", 
            $"service.name={serviceName},service.namespace=AppBlueprint,service.version=1.0.0,deployment.environment=development");
        
        return builder;
    }
    
    /// <summary>
    /// Gets the OTLP endpoint from environment variables or uses a default.
    /// </summary>
    private static string GetOtlpEndpoint()
    {
        // Check for Aspire dashboard OTLP endpoint first
        string? dashboardEndpoint = Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");
        
        if (!string.IsNullOrEmpty(dashboardEndpoint))
        {
            // Ensure we're using http:// for local connections
            if (dashboardEndpoint.Contains("localhost", StringComparison.OrdinalIgnoreCase) && 
                dashboardEndpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                dashboardEndpoint = dashboardEndpoint.Replace("https://", "http://", StringComparison.OrdinalIgnoreCase);
                Console.WriteLine($"TelemetryExtensions: Converted HTTPS to HTTP for localhost OTLP endpoint: {dashboardEndpoint}");
            }
            
            return dashboardEndpoint;
        }
        
        // Default to the standard Aspire dashboard collector endpoint
        return "http://localhost:18889";
    }
}
