#!/usr/bin/env pwsh
# set-aspire-local-telemetry.ps1
# This script replaces any external telemetry endpoints with the local Aspire endpoint

Write-Host "Replacing Grafana endpoint with Local Aspire Endpoint" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Gray

# Define the local Aspire OTLP endpoint
$aspireOtlpEndpoint = "http://localhost:18889"
$dashboardPort = "15068"

# Current configuration
Write-Host "`nCurrent configuration:" -ForegroundColor Magenta
Write-Host "OTEL_EXPORTER_OTLP_ENDPOINT: $env:OTEL_EXPORTER_OTLP_ENDPOINT" -ForegroundColor White
Write-Host "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL" -ForegroundColor White
Write-Host "OTEL_EXPORTER_OTLP_HEADERS: $env:OTEL_EXPORTER_OTLP_HEADERS" -ForegroundColor White
Write-Host "ASPIRE_DASHBOARD_PORT: $env:ASPIRE_DASHBOARD_PORT" -ForegroundColor White

# 1. Clear any existing Grafana configuration at both User and Process level
Write-Host "`nClearing existing OTLP configuration..." -ForegroundColor Yellow

# First set to null to fully clear the variables
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $null, "User")
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", $null, "User") 
[System.Environment]::SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", $null, "User")

# 2. Set the environment variables to the local Aspire endpoint
Write-Host "`nSetting OTLP endpoint to Aspire local endpoint: $aspireOtlpEndpoint" -ForegroundColor Green
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $aspireOtlpEndpoint, "User")
[System.Environment]::SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", $aspireOtlpEndpoint, "User")
[System.Environment]::SetEnvironmentVariable("ASPIRE_DASHBOARD_PORT", $dashboardPort, "User")
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf", "User")

# Also set for current process
$env:OTEL_EXPORTER_OTLP_ENDPOINT = $aspireOtlpEndpoint
$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = $aspireOtlpEndpoint
$env:ASPIRE_DASHBOARD_PORT = $dashboardPort
$env:OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
$env:OTEL_EXPORTER_OTLP_HEADERS = ""

# Verify changes
Write-Host "`nVerified configuration:" -ForegroundColor Magenta
Write-Host "OTEL_EXPORTER_OTLP_ENDPOINT: $env:OTEL_EXPORTER_OTLP_ENDPOINT" -ForegroundColor White
Write-Host "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL" -ForegroundColor White
Write-Host "OTEL_EXPORTER_OTLP_HEADERS: $env:OTEL_EXPORTER_OTLP_HEADERS" -ForegroundColor White
Write-Host "ASPIRE_DASHBOARD_PORT: $env:ASPIRE_DASHBOARD_PORT" -ForegroundColor White

Write-Host "`nNextt steps:" -ForegroundColor Cyan
Write-Host "1. Restart your PowerShell session for the changes to take full effect" -ForegroundColor White
Write-Host "2. Restart the AppHost: dotnet run --project AppBlueprint.AppHost" -ForegroundColor White
Write-Host "3. Run full-aspire-test.ps1 to verify telemetry is working" -ForegroundColor White