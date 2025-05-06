using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.Hosting
{
    // Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
    // This project should be referenced by each service project in your solution.
    public static class Extensions
    {
        public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            // Set default OpenTelemetry configuration
            ConfigureDefaultOpenTelemetrySettings();

            builder.ConfigureOpenTelemetry();

            builder.AddDefaultHealthChecks();

            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                http.AddServiceDiscovery();
            });

            return builder;
        }

        private static void ConfigureDefaultOpenTelemetrySettings()
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
                if (dashboardEndpoint.Contains("localhost", StringComparison.OrdinalIgnoreCase) && 
                    dashboardEndpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    dashboardEndpoint = dashboardEndpoint.Replace("https://", "http://", StringComparison.OrdinalIgnoreCase);
                    Console.WriteLine($"ServiceDefaults: Converted HTTPS to HTTP for localhost OTLP endpoint: {dashboardEndpoint}");
                }
                
                Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", dashboardEndpoint);
                Console.WriteLine($"ServiceDefaults: Using Aspire dashboard OTLP endpoint: {dashboardEndpoint}");
                
                // Explicitly disable TLS certificate validation for local development
                Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_CERTIFICATE", "");
                Environment.SetEnvironmentVariable("OTEL_INSECURE", "true");
            }
            else
            {
                // Default OTLP endpoint for Aspire dashboard collector
                Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889");
                Console.WriteLine("ServiceDefaults: Setting default OTLP endpoint: http://localhost:18889");
            }
            
            // Add explicit environment variables to help with connection issues
            Environment.SetEnvironmentVariable("OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY_COUNT", "5");
            Environment.SetEnvironmentVariable("OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY_DELAY", "1000");

            // Enable HTTP/2 unencrypted support for local development
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            string serviceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? builder.Environment.ApplicationName;
            
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: "1.0.0");

            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.SetResourceBuilder(resourceBuilder);
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(serviceName))
                .WithMetrics(metrics =>
                {
                    metrics.AddMeter(serviceName);
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddSource(serviceName);
                    tracing.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddSource("Npgsql");
                });

            builder.AddOpenTelemetryExporters();

            return builder;
        }

        private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            
            try
            {
                string? endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    // Default to Aspire dashboard endpoint if available
                    string? dashboardEndpoint = Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");
                    endpoint = !string.IsNullOrWhiteSpace(dashboardEndpoint) 
                        ? dashboardEndpoint 
                        : "http://localhost:18889";
                }

                Console.WriteLine($"Configuring OpenTelemetry with endpoint: {endpoint}");

                // Configure metrics exporter
                builder.Services.ConfigureOpenTelemetryMeterProvider(metrics =>
                {
                    metrics.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(endpoint);
                    });
                });

                // Configure tracing exporter
                builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
                {
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(endpoint);
                    });
                });

                // Configure logging exporter
                builder.Logging.AddOpenTelemetry(logging =>
                {
                    logging.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(endpoint);
                    });
                });

                Console.WriteLine("Successfully configured OpenTelemetry exporters");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring OpenTelemetry: {ex.Message}");
            }
            
            return builder;
        }

        public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            
            builder.Services.AddHealthChecks()
                // Add a default liveness check to ensure app is responsive
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app);
            
            // Adding health checks endpoints
            if (app.Environment.IsDevelopment())
            {
                app.MapHealthChecks("/health");
                app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
            }
            
            return app;
        }
    }
}