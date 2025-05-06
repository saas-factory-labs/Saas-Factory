#!/usr/bin/env pwsh
# Set-LocalAspireEndpoint.ps1
# This script sets the OTLP endpoint to the local Aspire endpoint in both user and process environment

Write-Host "Setting OpenTelemetry endpoints to local Aspire" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Gray

# First, set the user environment variables
$localEndpoint = "http://localhost:18889"
Write-Host "Setting OTEL_EXPORTER_OTLP_ENDPOINT to $localEndpoint in user environment..." -ForegroundColor Yellow
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $localEndpoint, "User")

Write-Host "Clearing OTEL_EXPORTER_OTLP_HEADERS in user environment..." -ForegroundColor Yellow
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", $null, "User")

Write-Host "Setting OTEL_EXPORTER_OTLP_PROTOCOL to http/protobuf in user environment..." -ForegroundColor Yellow
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf", "User")

Write-Host "Setting DOTNET_DASHBOARD_OTLP_ENDPOINT_URL to $localEndpoint in user environment..." -ForegroundColor Yellow
[System.Environment]::SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", $localEndpoint, "User")

Write-Host "Setting ASPIRE_DASHBOARD_PORT to 15068 in user environment..." -ForegroundColor Yellow
[System.Environment]::SetEnvironmentVariable("ASPIRE_DASHBOARD_PORT", "15068", "User")

# Then, set the process environment variables for this session
Write-Host "`nSetting process environment variables for current session..." -ForegroundColor Magenta
$env:OTEL_EXPORTER_OTLP_ENDPOINT = $localEndpoint
$env:OTEL_EXPORTER_OTLP_HEADERS = $null
$env:OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = $localEndpoint
$env:ASPIRE_DASHBOARD_PORT = "15068"

# Clear specific Grafana-related variables if they exist
$grafanaVars = @(
    "OTEL_EXPORTER_OTLP_METRICS_ENDPOINT",
    "OTEL_EXPORTER_OTLP_TRACES_ENDPOINT",
    "OTEL_EXPORTER_OTLP_LOGS_ENDPOINT"
)

foreach ($var in $grafanaVars) {
    [System.Environment]::SetEnvironmentVariable($var, $null, "User")
    Remove-Item -Path "env:$var" -ErrorAction SilentlyContinue
    Write-Host "Cleared $var" -ForegroundColor Yellow
}

# Check current values
Write-Host "`nCurrent environment variables:" -ForegroundColor Magenta
Write-Host "User OTEL_EXPORTER_OTLP_ENDPOINT: $([System.Environment]::GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "User"))" -ForegroundColor White
Write-Host "User OTEL_EXPORTER_OTLP_HEADERS: $([System.Environment]::GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "User"))" -ForegroundColor White
Write-Host "User OTEL_EXPORTER_OTLP_PROTOCOL: $([System.Environment]::GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "User"))" -ForegroundColor White
Write-Host "Process OTEL_EXPORTER_OTLP_ENDPOINT: $env:OTEL_EXPORTER_OTLP_ENDPOINT" -ForegroundColor White
Write-Host "Process OTEL_EXPORTER_OTLP_HEADERS: $env:OTEL_EXPORTER_OTLP_HEADERS" -ForegroundColor White
Write-Host "Process OTEL_EXPORTER_OTLP_PROTOCOL: $env:OTEL_EXPORTER_OTLP_PROTOCOL" -ForegroundColor White

Write-Host "`nNow restart the AppHost to apply these changes:" -ForegroundColor Green
Write-Host "1. Stop any running AppHost instances" -ForegroundColor White
Write-Host "2. Run: dotnet run --project AppBlueprint.AppHost" -ForegroundColor White
