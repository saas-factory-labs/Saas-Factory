#!/usr/bin/env pwsh
# setup-local-otlp.ps1
# This script sets up the environment variables for local Aspire OTLP collection

Write-Host "Setting up Aspire OpenTelemetry for LOCAL collection" -ForegroundColor Cyan
Write-Host "===================================================" -ForegroundColor Gray

# Clear any existing external OTLP headers completely
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", $null, "User")
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "", "User")

# Set up environment variables for the Aspire OTLP receiver
$aspireOtlpEndpoint = "http://localhost:18889"
$dashboardPort = "15068"

Write-Host "Setting ASPIRE_DASHBOARD_PORT to $dashboardPort" -ForegroundColor Green
[System.Environment]::SetEnvironmentVariable("ASPIRE_DASHBOARD_PORT", $dashboardPort, "User")

Write-Host "Setting DOTNET_DASHBOARD_OTLP_ENDPOINT_URL to $aspireOtlpEndpoint" -ForegroundColor Green
[System.Environment]::SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", $aspireOtlpEndpoint, "User")

Write-Host "Setting OTEL_EXPORTER_OTLP_ENDPOINT to $aspireOtlpEndpoint" -ForegroundColor Green
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $aspireOtlpEndpoint, "User")

Write-Host "Setting OTEL_EXPORTER_OTLP_PROTOCOL to http/protobuf" -ForegroundColor Green
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf", "User")

Write-Host "Clearing OTEL_EXPORTER_OTLP_HEADERS" -ForegroundColor Yellow
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "", "User")

# Also set for current process
$env:ASPIRE_DASHBOARD_PORT = $dashboardPort
$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = $aspireOtlpEndpoint
$env:OTEL_EXPORTER_OTLP_ENDPOINT = $aspireOtlpEndpoint
$env:OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
$env:OTEL_EXPORTER_OTLP_HEADERS = ""

# Clear any potentially conflicting variables
$conflictingVars = @(
    "OTEL_EXPORTER_OTLP_METRICS_ENDPOINT",
    "OTEL_EXPORTER_OTLP_TRACES_ENDPOINT",
    "OTEL_EXPORTER_OTLP_LOGS_ENDPOINT"
)

foreach ($var in $conflictingVars) {
    [System.Environment]::SetEnvironmentVariable($var, $null, "User")
    Remove-Item -Path "env:$var" -ErrorAction SilentlyContinue
    Write-Host "Cleared potential conflicting variable: $var" -ForegroundColor Yellow
}

Write-Host "`nOpenTelemetry environment variables have been set for LOCAL collection." -ForegroundColor Cyan
Write-Host "The changes have been applied to both the current session and user environment variables." -ForegroundColor Yellow

Write-Host "`nTo run the Aspire AppHost:" -ForegroundColor Magenta
Write-Host "  cd Code\AppBlueprint"
Write-Host "  dotnet run --project AppBlueprint.AppHost"

Write-Host "`nTo test the OTLP configuration:" -ForegroundColor Magenta
Write-Host "  cd Code\AppBlueprint"
Write-Host "  .\aspire-test-telemetry.ps1"

Write-Host "Setting DOTNET_DASHBOARD_OTLP_ENDPOINT_URL to $aspireOtlpEndpoint" -ForegroundColor Green
[System.Environment]::SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", $aspireOtlpEndpoint, "User")

Write-Host "Setting OTEL_EXPORTER_OTLP_ENDPOINT to $aspireOtlpEndpoint" -ForegroundColor Green
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $aspireOtlpEndpoint, "User")

Write-Host "Setting OTEL_EXPORTER_OTLP_PROTOCOL to http/protobuf" -ForegroundColor Green
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf", "User")

Write-Host "Clearing OTEL_EXPORTER_OTLP_HEADERS" -ForegroundColor Yellow
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "", "User")

# Also set for current process
$env:ASPIRE_DASHBOARD_PORT = $dashboardPort
$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = $aspireOtlpEndpoint
$env:OTEL_EXPORTER_OTLP_ENDPOINT = $aspireOtlpEndpoint
$env:OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
$env:OTEL_EXPORTER_OTLP_HEADERS = ""

Write-Host "`nOpenTelemetry environment variables have been set for LOCAL collection." -ForegroundColor Cyan
Write-Host "The changes have been applied to both the current session and user environment variables." -ForegroundColor Yellow

Write-Host "`nTo run the Aspire AppHost:" -ForegroundColor Magenta
Write-Host "  cd Code\AppBlueprint"
Write-Host "  dotnet run --project AppBlueprint.AppHost"

Write-Host "`nTo test the OTLP configuration:" -ForegroundColor Magenta
Write-Host "  cd Code\AppBlueprint"
Write-Host "  .\otlp-test.ps1"
