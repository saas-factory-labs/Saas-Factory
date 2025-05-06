// using Aspire.Hosting;

// namespace AppBlueprint.AppHost;

// /// <summary>
// /// Extensions for configuring OpenTelemetry in Aspire resources
// /// </summary>
// public static class TelemetryExtensions
// {
//     /// <summary>
//     /// Configure all OpenTelemetry environment variables for a resource
//     /// </summary>
//     public static IResourceBuilder<T> WithTelemetryDefaults<T>(
//         this IResourceBuilder<T> builder, 
//         string serviceName) where T : IResourceWithEnvironment
//     {
//         // Get all telemetry environment variables
//         string otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") ?? "http://localhost:18889";
//         string otlpProtocol = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL") ?? "http/protobuf";
//         string otlpHeaders = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS") ?? "";
//         string dashboardEndpoint = "https://localhost:21250"; // " "  Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL") ?? otlpEndpoint;
        
//         // Set all telemetry environment variables
//         return builder
//             .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", otlpEndpoint)
//             .WithEnvironment("OTEL_EXPORTER_OTLP_PROTOCOL", otlpProtocol)
//             .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", otlpHeaders)
//             .WithEnvironment("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", dashboardEndpoint)
//             .WithEnvironment("OTEL_SERVICE_NAME", serviceName)
//             .WithEnvironment("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");
//     }
// }
