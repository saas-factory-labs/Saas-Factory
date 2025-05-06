# OpenTelemetry in AppBlueprint

This document explains how OpenTelemetry is configured and used in the AppBlueprint project.

## Overview

AppBlueprint uses OpenTelemetry to collect and export telemetry data (metrics, traces, and logs) to 
various backends. When running in development mode, telemetry is sent to the Aspire dashboard.

## Key Components

1. **Aspire Dashboard**: Acts as an OpenTelemetry collector when running the application in development mode
2. **ServiceDefaults**: Contains the OpenTelemetry configuration used by all services
3. **Environment Variables**: Control where telemetry data is sent

## Environment Variables

The following environment variables control OpenTelemetry behavior:

- `DOTNET_DASHBOARD_OTLP_ENDPOINT_URL`: The Aspire dashboard's OTLP endpoint URL (default: http://localhost:18889)
- `OTEL_EXPORTER_OTLP_ENDPOINT`: The OpenTelemetry collector endpoint URL (should match the dashboard endpoint in development)
- `OTEL_EXPORTER_OTLP_PROTOCOL`: The protocol used by the OTLP exporter (default: http/protobuf)
- `OTEL_EXPORTER_OTLP_HEADERS`: Optional headers for the OTLP exporter (default: empty)
- `ASPIRE_DASHBOARD_PORT`: The port for the Aspire dashboard (default: 15068)
- `ASPIRE_ALLOW_UNSECURED_TRANSPORT`: Whether to allow HTTP for local development (default: true)

## Troubleshooting Guide

If telemetry data is not appearing in the Aspire dashboard, follow these steps:

1. **Check environment variables**:
   ```powershell
   # Check if environment variables are set correctly
   Write-Host "OTEL_EXPORTER_OTLP_ENDPOINT: $env:OTEL_EXPORTER_OTLP_ENDPOINT"
   Write-Host "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"
   Write-Host "ASPIRE_DASHBOARD_PORT: $env:ASPIRE_DASHBOARD_PORT"
   ```

2. **Verify telemetry endpoints are running**:
   ```powershell
   # Check if Aspire dashboard is accessible
   Invoke-WebRequest -Uri "http://localhost:15068" -Method Head
   
   # Check if OTLP endpoint is accessible (will return 400 which is expected)
   try { Invoke-WebRequest -Uri "http://localhost:18889/v1/metrics" -Method Head }
   catch { $_.Exception.Response.StatusCode }
   ```

3. **Restart the AppHost with correct environment variables**:
   ```powershell
   # Run the fix script
   .\fix-all-telemetry-issues.ps1
   ```

4. **Generate test traffic**:
   ```powershell
   # Generate traffic to populate telemetry data
   .\generate-telemetry-traffic.ps1
   ```

## Fixing Telemetry Issues

The following scripts are available to fix telemetry issues:

- `configure-aspire-telemetry.ps1`: Sets all required environment variables and restarts the AppHost
- `fix-all-telemetry-issues.ps1`: Comprehensive fix script that handles all telemetry components
- `validate-telemetry-config.ps1`: Validates the current telemetry configuration
- `generate-telemetry-traffic.ps1`: Generates test traffic to populate telemetry data

## Implementation Details

### ServiceDefaults

The `ServiceDefaults` extension in `AppBlueprint.ServiceDefaults` configures OpenTelemetry for all services. It sets up:

- Metrics exporters for runtime, ASP.NET Core, HTTP client, and process metrics
- Trace exporters for ASP.NET Core, HTTP client, and Entity Framework Core
- Logger exporters for sending logs to the OTLP endpoint

### Individual Services

Each service (Web, API, AppGateway) configures telemetry in its `Program.cs` file by:

1. Getting telemetry endpoint information from environment variables
2. Setting any missing variables with appropriate defaults
3. Calling `builder.AddServiceDefaults()` to configure OpenTelemetry

### AppHost

The AppHost configures environment variables for all child services and explicitly passes them as environment variables when launching each service.

## Best Practices

1. Always run services with the Aspire AppHost in development
2. Use the provided scripts to configure and troubleshoot telemetry
3. Check the Aspire dashboard's Resource tab to verify telemetry data
4. Always set `ASPIRE_ALLOW_UNSECURED_TRANSPORT` to `true` in development
- `OTEL_EXPORTER_OTLP_PROTOCOL`: The protocol used to send telemetry data (default: http/protobuf)
- `OTEL_SERVICE_NAME`: The name of the service (set per service in the AppHost configuration)
- `ASPIRE_DASHBOARD_PORT`: The port where the Aspire dashboard UI is served (default: 15068)

## How to Set Up

1. Run the provided setup script to set the environment variables:
   ```powershell
   .\setup-aspire-otlp.ps1
   ```

2. Start the Aspire AppHost project:
   ```powershell
   dotnet run --project AppBlueprint.AppHost
   ```

3. Test the OpenTelemetry configuration:
   ```powershell
   .\aspire-otlp-test.ps1
   ```

4. View telemetry data in the Aspire dashboard at http://localhost:15068

## Troubleshooting

If telemetry data is not showing up in the Aspire dashboard:

1. Make sure the environment variables are correctly set
2. Verify that all services include the ServiceDefaults project
3. Confirm that the Aspire dashboard is running
4. Check that the OTLP endpoints are accessible
5. Ensure the correct protocol is being used (http/protobuf is recommended)

## External Collectors

To send telemetry to an external collector like Grafana Cloud or Jaeger:

1. Update the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable to point to your collector
2. Set any required headers using `OTEL_EXPORTER_OTLP_HEADERS`
3. Update the protocol if needed (some collectors require gRPC instead of HTTP)

## Extending Telemetry

To add custom metrics or traces:

1. Inject the appropriate meter or tracer using DI:
   ```csharp
   private readonly Meter _meter;
   
   public MyService(IMeterFactory meterFactory)
   {
       _meter = meterFactory.Create("MyService");
   }
   ```

2. Create and record metrics or traces:
   ```csharp
   var counter = _meter.CreateCounter<long>("my_counter", "count", "Description");
   counter.Add(1);
   ```

For more information, see the [OpenTelemetry .NET documentation](https://opentelemetry.io/docs/instrumentation/net/).
