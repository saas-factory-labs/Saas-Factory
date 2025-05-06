#!/usr/bin/env pwsh
# setup-external-otlp.ps1
# This script sets up the environment variables for external OTLP collection (e.g., Grafana Cloud)

Write-Host "Setting up OpenTelemetry for EXTERNAL collection" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Gray

# Set external OTLP endpoint (Grafana Cloud)
$grafanaEndpoint = "https://otlp-gateway-prod-us-central-0.grafana.net/otlp"
$authToken = $env:OTEL_EXPORTER_OTLP_HEADERS

if ([string]::IsNullOrEmpty($authToken)) {
    $authToken = Read-Host "Enter your Grafana Cloud OTLP auth token (or press Enter to skip)"
    if ([string]::IsNullOrEmpty($authToken)) {
        Write-Host "No auth token provided. Only setting endpoint, not headers." -ForegroundColor Yellow
    } else {
        $authToken = "Authorization=Basic $authToken"
    }
}

Write-Host "Setting OTEL_EXPORTER_OTLP_ENDPOINT to $grafanaEndpoint" -ForegroundColor Green
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $grafanaEndpoint, "User")

Write-Host "Setting OTEL_EXPORTER_OTLP_PROTOCOL to http/protobuf" -ForegroundColor Green
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf", "User")

if (-not [string]::IsNullOrEmpty($authToken)) {
    Write-Host "Setting OTEL_EXPORTER_OTLP_HEADERS for external authentication" -ForegroundColor Green
    [System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", $authToken, "User")
}

# Also set for current process
$env:OTEL_EXPORTER_OTLP_ENDPOINT = $grafanaEndpoint
$env:OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
if (-not [string]::IsNullOrEmpty($authToken)) {
    $env:OTEL_EXPORTER_OTLP_HEADERS = $authToken
}

Write-Host "`nOpenTelemetry environment variables have been set for EXTERNAL collection." -ForegroundColor Cyan
Write-Host "The changes have been applied to both the current session and user environment variables." -ForegroundColor Yellow

Write-Host "`nTo run the Aspire AppHost:" -ForegroundColor Magenta
Write-Host "  cd Code\AppBlueprint"
Write-Host "  dotnet run --project AppBlueprint.AppHost"

Write-Host "`nTo test the OTLP configuration:" -ForegroundColor Magenta
Write-Host "  cd Code\AppBlueprint"
Write-Host "  .\otlp-test.ps1"

Write-Host "`nNote: When using external collection, telemetry won't appear in the Aspire dashboard." -ForegroundColor Yellow
