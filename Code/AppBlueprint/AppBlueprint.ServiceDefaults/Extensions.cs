using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using OpenTelemetry.Exporter;

namespace Microsoft.Extensions.Hosting
{
    public static class Extensions
    {
        private static readonly string[] LiveTags = { "live" };

        public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            ConfigureDefaultOpenTelemetrySettings();

            builder.ConfigureOpenTelemetry();
            builder.AddDefaultHealthChecks();

            builder.Services.AddServiceDiscovery();
            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                http.AddStandardResilienceHandler();
                http.AddServiceDiscovery();
            });

            return builder;
        }

        private static void ConfigureDefaultOpenTelemetrySettings()
        {
            string? dashboard = Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");
            if (!string.IsNullOrEmpty(dashboard))
            {
                Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", dashboard);
            }
            else
            {
                string? endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
                if (string.IsNullOrEmpty(endpoint))
                {
                    Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889");
                }
            }

            string? proto = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
            if (string.IsNullOrEmpty(proto))
            {
                Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
            }

            if (Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS") == null)
            {
                Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", string.Empty);
            }
        }

        public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            string serviceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? builder.Environment.ApplicationName;
            var resources = ResourceBuilder.CreateDefault().AddService(serviceName, "1.0.0");

            builder.Logging.AddOpenTelemetry(log =>
            {
                log.SetResourceBuilder(resources);
                log.IncludeScopes = true;
                log.IncludeFormattedMessage = true;

                string endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")!;
                bool grpc = string.Equals(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL"), "grpc", StringComparison.OrdinalIgnoreCase);
                log.AddOtlpExporter(opt => { opt.Endpoint = new Uri(endpoint); opt.Protocol = grpc ? OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf; });
            });

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(serviceName))
                .WithMetrics(metrics =>
                {
                    metrics.AddMeter(serviceName);
                    metrics.AddMeter("Microsoft.AspNetCore.Hosting");
                    metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                    metrics.AddAspNetCoreInstrumentation();
                    metrics.AddHttpClientInstrumentation();
                    metrics.AddRuntimeInstrumentation();

                    string endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")!;
                    bool grpc = string.Equals(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL"), "grpc", StringComparison.OrdinalIgnoreCase);
                    metrics.AddOtlpExporter(opt => { opt.Endpoint = new Uri(endpoint); opt.Protocol = grpc ? OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf; });
                })
                .WithTracing(tracing =>
                {
                    tracing.AddSource(serviceName);
                    tracing.AddSource("Microsoft.AspNetCore");
                    tracing.AddAspNetCoreInstrumentation(options => options.RecordException = true);
                    tracing.AddHttpClientInstrumentation(options => { options.RecordException = true; options.FilterHttpRequestMessage = _ => true; });

                    string endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")!;
                    bool grpc = string.Equals(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL"), "grpc", StringComparison.OrdinalIgnoreCase);
                    tracing.AddOtlpExporter(opt => { opt.Endpoint = new Uri(endpoint); opt.Protocol = grpc ? OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf; });
                });

            return builder;
        }

        public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));
            builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), LiveTags);
            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));
            if (app.Environment.IsDevelopment())
            {
                app.MapHealthChecks("/health");
                app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
            }
            return app;
        }
    }
}

// using System;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Diagnostics.HealthChecks;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
// using OpenTelemetry.Exporter;
// using OpenTelemetry.Logs;
// using OpenTelemetry.Metrics;
// using OpenTelemetry.Resources;
// using OpenTelemetry.Trace;

// namespace Microsoft.Extensions.Hosting
// {
//     public static class Extensions
//     {
//         public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
//         {
//             ArgumentNullException.ThrowIfNull(builder, nameof(builder));

//             ConfigureDefaultOpenTelemetrySettings();

//             builder.ConfigureOpenTelemetry();
//             builder.AddDefaultHealthChecks();

//             builder.Services.AddServiceDiscovery();
//             builder.Services.ConfigureHttpClientDefaults(http =>
//             {
//                 http.AddStandardResilienceHandler();
//                 http.AddServiceDiscovery();
//             });

//             return builder;
//         }

//         private static void ConfigureDefaultOpenTelemetrySettings()
//         {
//             string? dashboard = Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");
//             if (!string.IsNullOrEmpty(dashboard))
//             {
//                 Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", dashboard);
//             }
//             else
//             {
//                 string? endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
//                 if (string.IsNullOrEmpty(endpoint))
//                 {
//                     Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889");
//                 }
//             }

//             string? proto = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
//             if (string.IsNullOrEmpty(proto))
//             {
//                 Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
//             }

//             if (Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS") == null)
//             {
//                 Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", string.Empty);
//             }
//         }

//         public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
//         {
//             ArgumentNullException.ThrowIfNull(builder, nameof(builder));

//             string serviceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? builder.Environment.ApplicationName;
//             var resources = ResourceBuilder.CreateDefault().AddService(serviceName, "1.0.0");

//             builder.Logging.AddOpenTelemetry(log =>
//             {
//                 log.SetResourceBuilder(resources);
//                 log.IncludeScopes = true;
//                 log.IncludeFormattedMessage = true;

//                 string endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")!;
//                 bool grpc = string.Equals(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL"), "grpc", StringComparison.OrdinalIgnoreCase);
//                 log.AddOtlpExporter(opt => { opt.Endpoint = new Uri(endpoint); opt.Protocol = grpc ? OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf; });
//             });

//             builder.Services.AddOpenTelemetry()
//                 .ConfigureResource(r => r.AddService(serviceName))
//                 .WithMetrics(metrics =>
//                 {
//                     metrics.AddMeter(serviceName);
//                     metrics.AddMeter("Microsoft.AspNetCore.Hosting");
//                     metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
//                     metrics.AddAspNetCoreInstrumentation();
//                     metrics.AddHttpClientInstrumentation();
//                     metrics.AddRuntimeInstrumentation();

//                     string endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")!;
//                     bool grpc = string.Equals(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL"), "grpc", StringComparison.OrdinalIgnoreCase);
//                     metrics.AddOtlpExporter(opt => { opt.Endpoint = new Uri(endpoint); opt.Protocol = grpc ? OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf; });
//                 })
//                 .WithTracing(tracing =>
//                 {
//                     tracing.AddSource(serviceName);
//                     tracing.AddSource("Microsoft.AspNetCore");
//                     tracing.AddAspNetCoreInstrumentation(options => options.RecordException = true);
//                     tracing.AddHttpClientInstrumentation(options => { options.RecordException = true; options.FilterHttpRequestMessage = _ => true; });

//                     string endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")!;
//                     bool grpc = string.Equals(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL"), "grpc", StringComparison.OrdinalIgnoreCase);
//                     tracing.AddOtlpExporter(opt => { opt.Endpoint = new Uri(endpoint); opt.Protocol = grpc ? OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf; });
//                 });

//             return builder;
//         }

//         public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
//         {
//             ArgumentNullException.ThrowIfNull(builder, nameof(builder));
//             builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "live" });
//             return builder;
//         }

//         public static WebApplication MapDefaultEndpoints(this WebApplication app)
//         {
//             ArgumentNullException.ThrowIfNull(app, nameof(app));
//             if (app.Environment.IsDevelopment())
//             {
//                 app.MapHealthChecks("/health");
//                 app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
//             }
//             return app;
//         }
//     }
// }

// // using Microsoft.AspNetCore.Builder;
// // using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// // using Microsoft.Extensions.DependencyInjection;
// // using Microsoft.Extensions.Diagnostics.HealthChecks;
// // using Microsoft.Extensions.Logging;
// // using OpenTelemetry;
// // using OpenTelemetry.Exporter;
// // using OpenTelemetry.Logs;
// // using OpenTelemetry.Metrics;
// // using OpenTelemetry.Resources;
// // using OpenTelemetry.Trace;
// // using System;

// // namespace Microsoft.Extensions.Hosting
// // {
// //     public static class Extensions
// //     {        public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
// //         {
// //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// //             // Set default OpenTelemetry configuration
// //             ConfigureDefaultOpenTelemetrySettings();

// //             // Add standard services and middleware
// //             builder.ConfigureOpenTelemetry();
// //             builder.AddDefaultHealthChecks();
// //             builder.Services.AddServiceDiscovery();
// //             builder.Services.ConfigureHttpClientDefaults(http =>
// //             {
// //                 http.AddStandardResilienceHandler();
// //                 http.AddServiceDiscovery();
// //             });

// //             return builder;
// //         }

// //         private static void ConfigureDefaultOpenTelemetrySettings()
// //         {
// //             // Check for Aspire dashboard OTLP endpoint first and prioritize it
// //             // string? dashboardEndpoint = Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");

// //             string dashboardEndpoint = "https://localhost:21250";
            
// //             // If dashboard endpoint is set, use it for OTEL exporter
// //             if (!string.IsNullOrEmpty(dashboardEndpoint))
// //             {
// //                 Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", dashboardEndpoint);
// //                 Console.WriteLine($"ServiceDefaults: Using Aspire dashboard OTLP endpoint: {dashboardEndpoint}");
// //             }
// //             else
// //             {
// //                 // Fallback to standard OTLP endpoint or set default
// //                 string? endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
// //                 if (string.IsNullOrEmpty(endpoint))
// //                 {
// //                     Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889");
// //                     Console.WriteLine("ServiceDefaults: Setting default OTLP endpoint: http://localhost:18889");
// //                 }
// //             }

// //             // Ensure protocol is set
// //             string? protocol = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
// //             if (string.IsNullOrEmpty(protocol))
// //             {
// //                 Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
// //                 Console.WriteLine("ServiceDefaults: Setting default OTLP protocol: http/protobuf");
// //             }
            
// //             // Clear headers if not set
// //             string? headers = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");
// //             if (headers == null)
// //             {
// //                 Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", string.Empty);
// //             }
// //         }

// //         public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
// //         {
// //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));
// //             string serviceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? builder.Environment.ApplicationName;

// //             var resources = ResourceBuilder.CreateDefault()
// //                 .AddService(serviceName: serviceName, serviceVersion: "1.0.0");

// //             builder.Logging.AddOpenTelemetry(log =>
// //             {
// //                 log.SetResourceBuilder(resources);
// //                 log.IncludeScopes = true;
// //                 log.IncludeFormattedMessage = true;
// //             });

// //             builder.Services.AddOpenTelemetry()
// //                 .ConfigureResource(r => r.AddService(serviceName))
// //                 .WithMetrics(metrics =>
// //                 {
// //                     metrics.AddMeter(serviceName);
// //                     metrics.AddMeter("Microsoft.AspNetCore.Hosting");
// //                     metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
// //                     metrics.AddAspNetCoreInstrumentation();
// //                     metrics.AddHttpClientInstrumentation();
// //                     metrics.AddRuntimeInstrumentation();
// //                 })
// //                 .WithTracing(tracing =>
// //                 {
// //                     tracing.AddSource(serviceName);
// //                     tracing.AddSource("Microsoft.AspNetCore");
// //                     tracing.AddAspNetCoreInstrumentation(options =>
// //                     {
// //                         options.RecordException = true;
// //                         options.EnrichWithHttpRequest = (activity, req) =>
// //                         {
// //                             activity.SetTag("http.host", req.Host);
// //                             activity.SetTag("http.user_agent", req.Headers.UserAgent);
// //                         };
// //                     });
// //                     tracing.AddHttpClientInstrumentation(options =>
// //                     {
// //                         options.RecordException = true;
// //                         options.FilterHttpRequestMessage = _ => true;
// //                     });
// //                 });

// //             builder.AddOpenTelemetryExporters();
// //             return builder;
// //         }        private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
// //         {
// //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));
// //             try
// //             {
// //                 string? endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
// //                 if (string.IsNullOrWhiteSpace(endpoint))
// //                 {
// //                     // Default to Aspire dashboard endpoint if available
// //                     // string? dashboardEndpoint = Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");
// //                     string dashboardEndpoint = "https://localhost:21250";
// //                     endpoint = !string.IsNullOrWhiteSpace(dashboardEndpoint) 
// //                         ? dashboardEndpoint 
// //                         : "http://localhost:18889";
// //                 }

// //                 Console.WriteLine($"Configuring OpenTelemetry with endpoint: {endpoint}");

// //                 string? proto = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
// //                 var protocol = string.Equals(proto, "grpc", StringComparison.OrdinalIgnoreCase)
// //                     ? OtlpExportProtocol.Grpc
// //                     : OtlpExportProtocol.HttpProtobuf;

// //                 builder.Services.ConfigureOpenTelemetryMeterProvider(m =>
// //                 {
// //                     m.AddConsoleExporter();
// //                     m.AddOtlpExporter(opt =>
// //                     {
// //                         opt.Endpoint = new Uri(endpoint);
// //                         opt.Protocol = protocol;
// //                     });
// //                 });

// //                 builder.Services.ConfigureOpenTelemetryTracerProvider(t =>
// //                 {
// //                     t.AddConsoleExporter();
// //                     t.AddOtlpExporter(opt =>
// //                     {
// //                         opt.Endpoint = new Uri(endpoint);
// //                         opt.Protocol = protocol;
// //                     });
// //                 });

// //                 builder.Logging.AddOpenTelemetry(log =>
// //                 {
// //                     log.AddOtlpExporter(opt =>
// //                     {
// //                         opt.Endpoint = new Uri(endpoint);
// //                         opt.Protocol = protocol;
// //                     });
// //                 });

// //                 Console.WriteLine("Successfully configured OpenTelemetry exporters");
// //             }
// //             catch (Exception ex)
// //             {
// //                 Console.WriteLine($"Error configuring OpenTelemetry: {ex.Message}");
// //             }
// //             return builder;
// //         }

// //         public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
// //         {
// //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));
// //             builder.Services.AddHealthChecks()
// //                 .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "live" });
// //             return builder;
// //         }

// //         public static WebApplication MapDefaultEndpoints(this WebApplication app)
// //         {
// //             ArgumentNullException.ThrowIfNull(app, nameof(app));
// //             if (app.Environment.IsDevelopment())
// //             {
// //                 app.MapHealthChecks("/health");
// //                 app.MapHealthChecks("/alive", new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
// //             }
// //             return app;
// //         }
// //     }
// // }

// // // using Microsoft.AspNetCore.Builder;
// // // using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// // // using Microsoft.Extensions.DependencyInjection;
// // // using Microsoft.Extensions.Diagnostics.HealthChecks;
// // // using Microsoft.Extensions.Logging;
// // // using OpenTelemetry;
// // // using OpenTelemetry.Exporter;
// // // using OpenTelemetry.Logs;
// // // using OpenTelemetry.Metrics;
// // // using OpenTelemetry.Resources;
// // // using OpenTelemetry.Trace;
// // // using System;

// // // namespace Microsoft.Extensions.Hosting
// // // {
// // //     public static class Extensions
// // //     {
// // //         public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
// // //         {
// // //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// // //             string? otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
// // //             if (string.IsNullOrEmpty(otlpEndpoint))
// // //             {
// // //                 Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4318");
// // //             }

// // //             string? otlpProtocol = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
// // //             if (string.IsNullOrEmpty(otlpProtocol))
// // //             {
// // //                 Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
// // //             }

// // //             builder.ConfigureOpenTelemetry();
// // //             builder.AddDefaultHealthChecks();

// // //             builder.Services.AddServiceDiscovery();
// // //             builder.Services.ConfigureHttpClientDefaults(http =>
// // //             {
// // //                 http.AddStandardResilienceHandler();
// // //                 http.AddServiceDiscovery();
// // //             });

// // //             return builder;
// // //         }

// // //         public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
// // //         {
// // //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// // //             string serviceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? builder.Environment.ApplicationName;

// // //             var resourceBuilder = ResourceBuilder.CreateDefault()
// // //                 .AddService(serviceName: serviceName, serviceVersion: "1.0.0");

// // //             builder.Logging.AddOpenTelemetry(logging =>
// // //             {
// // //                 logging.SetResourceBuilder(resourceBuilder);
// // //                 logging.IncludeFormattedMessage = true;
// // //                 logging.IncludeScopes = true;
// // //             });

// // //             builder.Services.AddOpenTelemetry()
// // //                 .ConfigureResource(r => r.AddService(serviceName))
// // //                 .WithMetrics(metrics =>
// // //                 {
// // //                     metrics.AddMeter(serviceName);
// // //                     metrics.AddMeter("Microsoft.AspNetCore.Hosting");
// // //                     metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
// // //                     metrics.AddAspNetCoreInstrumentation();
// // //                     metrics.AddHttpClientInstrumentation();
// // //                     metrics.AddRuntimeInstrumentation();
// // //                 })
// // //                 .WithTracing(tracing =>
// // //                 {
// // //                     tracing.AddSource(serviceName);
// // //                     tracing.AddSource("Microsoft.AspNetCore");
// // //                     tracing.AddAspNetCoreInstrumentation(options =>
// // //                     {
// // //                         options.RecordException = true;
// // //                         options.EnrichWithHttpRequest = (activity, request) =>
// // //                         {
// // //                             activity.SetTag("http.request.headers.host", request.Host);
// // //                             activity.SetTag("http.request.headers.user_agent", request.Headers.UserAgent);
// // //                         };
// // //                     });
// // //                     tracing.AddHttpClientInstrumentation(options =>
// // //                     {
// // //                         options.RecordException = true;
// // //                         options.FilterHttpRequestMessage = _ => true;
// // //                     });
// // //                 });

// // //             builder.AddOpenTelemetryExporters();
// // //             return builder;
// // //         }

// // //         private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
// // //         {
// // //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// // //             try
// // //             {
// // //                 string? otlpEndpoint = builder.Configuration["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"]
// // //                     ?? builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
// // //                     ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
// // //                 if (string.IsNullOrWhiteSpace(otlpEndpoint))
// // //                 {
// // //                     otlpEndpoint = "http://localhost:4318";
// // //                 }

// // //                 Console.WriteLine($"Configuring OpenTelemetry with endpoint: {otlpEndpoint}");

// // //                 string? protocolStr = builder.Configuration["OTEL_EXPORTER_OTLP_PROTOCOL"]
// // //                     ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
// // //                 var protocol = string.Equals(protocolStr, "grpc", StringComparison.OrdinalIgnoreCase)
// // //                     ? OtlpExportProtocol.Grpc
// // //                     : OtlpExportProtocol.HttpProtobuf;

// // //                 builder.Services.ConfigureOpenTelemetryMeterProvider(options =>
// // //                 {
// // //                     options.AddConsoleExporter();
// // //                     options.AddOtlpExporter(otlpOptions =>
// // //                     {
// // //                         otlpOptions.Endpoint = new Uri(otlpEndpoint);
// // //                         otlpOptions.Protocol = protocol;
// // //                     });
// // //                 });

// // //                 builder.Services.ConfigureOpenTelemetryTracerProvider(options =>
// // //                 {
// // //                     options.AddConsoleExporter();
// // //                     options.AddOtlpExporter(otlpOptions =>
// // //                     {
// // //                         otlpOptions.Endpoint = new Uri(otlpEndpoint);
// // //                         otlpOptions.Protocol = protocol;
// // //                     });
// // //                 });

// // //                 builder.Logging.AddOpenTelemetry(logging =>
// // //                 {
// // //                     logging.AddOtlpExporter(otlpOptions =>
// // //                     {
// // //                         otlpOptions.Endpoint = new Uri(otlpEndpoint);
// // //                         otlpOptions.Protocol = protocol;
// // //                     });
// // //                 });

// // //                 Console.WriteLine("Successfully configured OpenTelemetry exporters");
// // //             }
// // //             catch (Exception ex)
// // //             {
// // //                 Console.WriteLine($"Error configuring OpenTelemetry: {ex.Message}");
// // //             }

// // //             return builder;
// // //         }

// // //         public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
// // //         {
// // //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// // //             builder.Services.AddHealthChecks()
// // //                 .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "live" });

// // //             return builder;
// // //         }

// // //         public static WebApplication MapDefaultEndpoints(this WebApplication app)
// // //         {
// // //             ArgumentNullException.ThrowIfNull(app, nameof(app));

// // //             if (app.Environment.IsDevelopment())
// // //             {
// // //                 app.MapHealthChecks("/health");
// // //                 app.MapHealthChecks("/alive", new HealthCheckOptions
// // //                 {
// // //                     Predicate = r => r.Tags.Contains("live")
// // //                 });
// // //             }

// // //             return app;
// // //         }
// // //     }
// // // }

// // // // using Microsoft.AspNetCore.Builder;
// // // // using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// // // // using Microsoft.Extensions.DependencyInjection;
// // // // using Microsoft.Extensions.Diagnostics.HealthChecks;
// // // // using Microsoft.Extensions.Logging;
// // // // using OpenTelemetry;
// // // // using OpenTelemetry.Logs;
// // // // using OpenTelemetry.Metrics;
// // // // using OpenTelemetry.Resources;
// // // // using OpenTelemetry.Trace;
// // // // using System;

// // // // namespace Microsoft.Extensions.Hosting
// // // // {
// // // //     // Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// // // //     public static class Extensions
// // // //     {
// // // //         public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
// // // //         {
// // // //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// // // //             // Default OTLP endpoint for Aspire dashboard collector
// // // //             string? otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
// // // //             if (string.IsNullOrEmpty(otlpEndpoint))
// // // //                 Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4318");

// // // //             // Default OTLP protocol if not set
// // // //             string? otlpProtocol = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
// // // //             if (string.IsNullOrEmpty(otlpProtocol))
// // // //                 Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");

// // // //             builder.ConfigureOpenTelemetry();
// // // //             builder.AddDefaultHealthChecks();

// // // //             builder.Services.AddServiceDiscovery();
// // // //             builder.Services.ConfigureHttpClientDefaults(http =>
// // // //             {
// // // //                 http.AddStandardResilienceHandler();
// // // //                 http.AddServiceDiscovery();
// // // //             });

// // // //             return builder;
// // // //         }

// // // //         public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
// // // //         {
// // // //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// // // //             string serviceName = builder.Configuration["OTEL_SERVICE_NAME"]
// // // //                                  ?? builder.Environment.ApplicationName;

// // // //             var resourceBuilder = ResourceBuilder.CreateDefault()
// // // //                 .AddService(serviceName: serviceName, serviceVersion: "1.0.0");

// // // //             // OpenTelemetry logging
// // // //             builder.Logging.AddOpenTelemetry(logging =>
// // // //             {
// // // //                 logging.SetResourceBuilder(resourceBuilder);
// // // //                 logging.IncludeFormattedMessage = true;
// // // //                 logging.IncludeScopes = true;
// // // //             });

// // // //             // Metrics and tracing instrumentation
// // // //             builder.Services.AddOpenTelemetry()
// // // //                 .ConfigureResource(r => r.AddService(serviceName))
// // // //                 .WithMetrics(metrics =>
// // // //                 {
// // // //                     metrics.AddMeter(serviceName);
// // // //                     metrics.AddMeter("Microsoft.AspNetCore.Hosting");
// // // //                     metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
// // // //                     metrics.AddAspNetCoreInstrumentation();
// // // //                     metrics.AddHttpClientInstrumentation();
// // // //                     metrics.AddRuntimeInstrumentation();
// // // //                 })
// // // //                 .WithTracing(tracing =>
// // // //                 {
// // // //                     tracing.AddSource(serviceName);
// // // //                     tracing.AddSource("Microsoft.AspNetCore");
// // // //                     tracing.AddAspNetCoreInstrumentation(options =>
// // // //                     {
// // // //                         options.RecordException = true;
// // // //                         options.EnrichWithHttpRequest = (activity, request) =>
// // // //                         {
// // // //                             activity.SetTag("http.request.headers.host", request.Host);
// // // //                             activity.SetTag("http.request.headers.user_agent", request.Headers.UserAgent);
// // // //                         };
// // // //                     });
// // // //                     tracing.AddHttpClientInstrumentation(options =>
// // // //                     {
// // // //                         options.RecordException = true;
// // // //                         options.FilterHttpRequestMessage = _ => true;
// // // //                     });
// // // //                 });

// // // //             builder.AddOpenTelemetryExporters();
// // // //             return builder;
// // // //         }

// // // //         private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
// // // //         {
// // // //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// // // //             try
// // // //             {
// // // //                 // Determine OTLP endpoint (dashboard collector)
// // // //                 string? otlpEndpoint = builder.Configuration["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"]
// // // //                                        ?? builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
// // // //                                        ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
// // // //                 if (string.IsNullOrWhiteSpace(otlpEndpoint))
// // // //                     otlpEndpoint = "http://localhost:4318";

// // // //                 Console.WriteLine($"Configuring OpenTelemetry with endpoint: {otlpEndpoint}");

// // // //                 // Determine protocol from env-var
// // // //                 string? protocolStr = builder.Configuration["OTEL_EXPORTER_OTLP_PROTOCOL"]
// // // //                                       ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
// // // //                 var protocol = string.Equals(protocolStr, "grpc", StringComparison.OrdinalIgnoreCase);
                  

// // // //                 // Metrics exporter
// // // //                 builder.Services.ConfigureOpenTelemetryMeterProvider(options =>
// // // //                 {
// // // //                     options.AddConsoleExporter();
// // // //                     options.AddOtlpExporter(otlpOptions =>
// // // //                     {
// // // //                         otlpOptions.Endpoint = new Uri(otlpEndpoint);
// // // //                         otlpOptions.Protocol = protocol;
// // // //                     });
// // // //                 });

// // // //                 // Tracing exporter
// // // //                 builder.Services.ConfigureOpenTelemetryTracerProvider(options =>
// // // //                 {
// // // //                     options.AddConsoleExporter();
// // // //                     options.AddOtlpExporter(otlpOptions =>
// // // //                     {
// // // //                         otlpOptions.Endpoint = new Uri(otlpEndpoint);
// // // //                         otlpOptions.Protocol = protocol;
// // // //                     });
// // // //                 });

// // // //                 // Logging exporter
// // // //                 builder.Logging.AddOpenTelemetry(logging =>
// // // //                 {
// // // //                     logging.AddOtlpExporter(otlpOptions =>
// // // //                     {
// // // //                         otlpOptions.Endpoint = new Uri(otlpEndpoint);
// // // //                         otlpOptions.Protocol = protocol;
// // // //                     });
// // // //                 });

// // // //                 Console.WriteLine("Successfully configured OpenTelemetry exporters");
// // // //             }
// // // //             catch (Exception ex)
// // // //             {
// // // //                 Console.WriteLine($"Error configuring OpenTelemetry: {ex.Message}");
// // // //             }

// // // //             return builder;
// // // //         }

// // // //         public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
// // // //         {
// // // //             ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// // // //             builder.Services.AddHealthChecks()
// // // //                 .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "live" });

// // // //             return builder;
// // // //         }

// // // //         public static WebApplication MapDefaultEndpoints(this WebApplication app)
// // // //         {
// // // //             ArgumentNullException.ThrowIfNull(app, nameof(app));

// // // //             if (app.Environment.IsDevelopment())
// // // //             {
// // // //                 app.MapHealthChecks("/health");
// // // //                 app.MapHealthChecks("/alive", new HealthCheckOptions
// // // //                 {
// // // //                     Predicate = r => r.Tags.Contains("live")
// // // //                 });
// // // //             }

// // // //             return app;
// // // //         }
// // // //     }
// // // // }


// // // // // using Microsoft.AspNetCore.Builder;
// // // // // using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// // // // // using Microsoft.Extensions.DependencyInjection;
// // // // // using Microsoft.Extensions.Diagnostics.HealthChecks;
// // // // // using Microsoft.Extensions.Logging;
// // // // // using OpenTelemetry;
// // // // // using OpenTelemetry.Logs;
// // // // // using OpenTelemetry.Metrics;
// // // // // using OpenTelemetry.Resources;
// // // // // using OpenTelemetry.Trace;
// // // // // using System;
// // // // // using System.Diagnostics;

// // // // // namespace Microsoft.Extensions.Hosting;

// // // // // // Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// // // // // public static class Extensions
// // // // // {
// // // // //     public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
// // // // //     {
// // // // //         ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// // // // //         // Set default OTLP endpoint if not set
// // // // //         string? otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
// // // // //         if (string.IsNullOrEmpty(otlpEndpoint))
// // // // //         {
// // // // //             Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4318");
// // // // //         }

// // // // //         builder.ConfigureOpenTelemetry();

// // // // //         builder.AddDefaultHealthChecks();

// // // // //         builder.Services.AddServiceDiscovery();

// // // // //         builder.Services.ConfigureHttpClientDefaults(http =>
// // // // //         {
// // // // //             // Turn on resilience by default
// // // // //             http.AddStandardResilienceHandler();

// // // // //             // Turn on service discovery by default
// // // // //             http.AddServiceDiscovery();
// // // // //         });

// // // // //         return builder;
// // // // //     }

// // // // //     public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
// // // // //     {
// // // // //         ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// // // // //         // Get service name from configuration or use application name
// // // // //         string serviceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? 
// // // // //                              builder.Environment.ApplicationName;

// // // // //         // Create a resource builder with appropriate service information
// // // // //         var resourceBuilder = ResourceBuilder.CreateDefault()
// // // // //             .AddService(serviceName: serviceName, serviceVersion: "1.0.0");

// // // // //         // Add OpenTelemetry logging
// // // // //         builder.Logging.AddOpenTelemetry(logging =>
// // // // //         {
// // // // //             logging.SetResourceBuilder(resourceBuilder);
// // // // //             logging.IncludeFormattedMessage = true;
// // // // //             logging.IncludeScopes = true;
// // // // //         });

// // // // //         // Configure OpenTelemetry with metrics and tracing
// // // // //         builder.Services.AddOpenTelemetry()
// // // // //             .ConfigureResource(r => r.AddService(serviceName))
// // // // //             .WithMetrics(metrics =>
// // // // //             {
// // // // //                 metrics.AddMeter(serviceName);
// // // // //                 metrics.AddMeter("Microsoft.AspNetCore.Hosting");
// // // // //                 metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
// // // // //                 metrics.AddAspNetCoreInstrumentation();
// // // // //                 metrics.AddHttpClientInstrumentation();
// // // // //                 metrics.AddRuntimeInstrumentation();
// // // // //             })
// // // // //             .WithTracing(tracing =>
// // // // //             {
// // // // //                 // Add sources that should be traced
// // // // //                 tracing.AddSource(serviceName);
// // // // //                 tracing.AddSource("Microsoft.AspNetCore");
                
// // // // //                 // Add instrumentation
// // // // //                 tracing.AddAspNetCoreInstrumentation(options => 
// // // // //                 {
// // // // //                     options.RecordException = true;
// // // // //                     options.EnrichWithHttpRequest = (activity, request) => 
// // // // //                     {
// // // // //                         activity.SetTag("http.request.headers.host", request.Host);
// // // // //                         activity.SetTag("http.request.headers.user_agent", request.Headers.UserAgent);
// // // // //                     };
// // // // //                 });
// // // // //                 tracing.AddHttpClientInstrumentation(options =>
// // // // //                 {
// // // // //                     options.RecordException = true;
// // // // //                     options.FilterHttpRequestMessage = (request) => true;
// // // // //                 });
// // // // //             });

// // // // //         // Add exporters
// // // // //         builder.AddOpenTelemetryExporters();

// // // // //         return builder;
// // // // //     }    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
// // // // //     {
// // // // //         ArgumentNullException.ThrowIfNull(builder, nameof(builder));

// // // // //         try
// // // // //         {
// // // // //             // First check for DOTNET_DASHBOARD_OTLP_ENDPOINT_URL (Aspire standard)
// // // // //             string? otlpEndpoint = builder.Configuration["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"];
            
// // // // //             // Fall back to OTEL_EXPORTER_OTLP_ENDPOINT if the Aspire-specific one isn't set
// // // // //             if (string.IsNullOrWhiteSpace(otlpEndpoint))
// // // // //             {
// // // // //                 otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? 
// // // // //                                Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
// // // // //             }
            
// // // // //             // Default as last resort
// // // // //             if (string.IsNullOrWhiteSpace(otlpEndpoint))
// // // // //             {
// // // // //                 otlpEndpoint = "http://localhost:18889";
// // // // //             }

// // // // //             Console.WriteLine($"Configuring OpenTelemetry with endpoint: {otlpEndpoint}");

// // // // //             // Determine protocol (prefer gRPC for Aspire)
// // // // //             var protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
// // // // //             string? protocolStr = builder.Configuration["OTEL_EXPORTER_OTLP_PROTOCOL"] ?? 
// // // // //                                  Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
            
// // // // //             if (!string.IsNullOrWhiteSpace(protocolStr))
// // // // //             {
// // // // //                 if (protocolStr.Equals("http/protobuf", StringComparison.OrdinalIgnoreCase))
// // // // //                 {
// // // // //                     protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
// // // // //                 }
// // // // //             }

// // // // //             // Configure metrics with OTLP exporter
// // // // //             builder.Services.ConfigureOpenTelemetryMeterProvider(options => 
// // // // //             {
// // // // //                 options.AddConsoleExporter(); // Add console exporter for debugging
// // // // //                 options.AddOtlpExporter(otlpOptions => 
// // // // //                 {
// // // // //                     otlpOptions.Endpoint = new Uri(otlpEndpoint);
// // // // //                     otlpOptions.Protocol = protocol;
// // // // //                 });
// // // // //             });
            
// // // // //             // Configure tracing with OTLP exporter
// // // // //             builder.Services.ConfigureOpenTelemetryTracerProvider(options => 
// // // // //             {
// // // // //                 options.AddConsoleExporter(); // Add console exporter for debugging
// // // // //                 options.AddOtlpExporter(otlpOptions => 
// // // // //                 {
// // // // //                     otlpOptions.Endpoint = new Uri(otlpEndpoint);
// // // // //                     otlpOptions.Protocol = protocol;
// // // // //                 });
// // // // //             });
            
// // // // //             // Configure logging with OTLP exporter
// // // // //             builder.Logging.AddOpenTelemetry(logging =>
// // // // //             {
// // // // //                 logging.AddOtlpExporter(otlpOptions =>
// // // // //                 {
// // // // //                     otlpOptions.Endpoint = new Uri(otlpEndpoint);
// // // // //                     otlpOptions.Protocol = protocol;
// // // // //                 });
// // // // //             });
            
// // // // //             Console.WriteLine("Successfully configured OpenTelemetry exporters");
// // // // //         }
// // // // //         catch (Exception ex)
// // // // //         {            
// // // // //             Console.WriteLine($"Error configuring OpenTelemetry: {ex.Message}");
// // // // //         }
        
// // // // //         return builder;
// // // // //     }

// // // // //     public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
// // // // //     {
// // // // //         ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        
// // // // //         builder.Services.AddHealthChecks()
// // // // //             // Add a default liveness check to ensure app is responsive
// // // // //             .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

// // // // //         return builder;
// // // // //     }

// // // // //     public static WebApplication MapDefaultEndpoints(this WebApplication app)
// // // // //     {
// // // // //         ArgumentNullException.ThrowIfNull(app, nameof(app));
        
// // // // //         // Adding health checks endpoints to applications in non-development environments has security implications.
// // // // //         // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
// // // // //         if (app.Environment.IsDevelopment())
// // // // //         {
// // // // //             // All health checks must pass for app to be considered ready to accept traffic after starting
// // // // //             app.MapHealthChecks("/health");

// // // // //             // Only health checks tagged with the "live" tag must pass for app to be considered alive
// // // // //             app.MapHealthChecks("/alive", new HealthCheckOptions
// // // // //             {
// // // // //                 Predicate = r => r.Tags.Contains("live")
// // // // //             });
// // // // //         }

// // // // //         return app;
// // // // //     }
// // // // // }
