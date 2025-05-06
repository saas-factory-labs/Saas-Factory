#!/usr/bin/env pwsh
# reset-all-otlp.ps1
# This script resets all OpenTelemetry environment variables to ensure a clean state

Write-Host "Resetting ALL OpenTelemetry Environment Variables" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Gray

# List of all OTLP-related environment variables to reset
$otlpVars = @(
    "OTEL_EXPORTER_OTLP_ENDPOINT",
    "OTEL_EXPORTER_OTLP_HEADERS",
    "OTEL_EXPORTER_OTLP_PROTOCOL",
    "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL",
    "ASPIRE_DASHBOARD_PORT",
    "OTEL_EXPORTER_OTLP_METRICS_ENDPOINT",
    "OTEL_EXPORTER_OTLP_TRACES_ENDPOINT",
    "OTEL_EXPORTER_OTLP_LOGS_ENDPOINT",
    "OTEL_RESOURCE_ATTRIBUTES",
    "OTEL_SERVICE_NAME",
    "OTEL_RESOURCE_ATTRIBUTES"
)

# Delete each variable from both User and Process environment
foreach ($var in $otlpVars) {
    # Reset User environment variable
    [System.Environment]::SetEnvironmentVariable($var, $null, "User")
    
    # Clear from current process
    if (Test-Path "env:$var") {
        Remove-Item "env:$var" -Force -ErrorAction SilentlyContinue
    }
    
    Write-Host "Cleared $var from both User and Process environment" -ForegroundColor Yellow
}

# Print all current environment variables that start with OTEL
Write-Host "`nCurrent environment variables (should be empty):" -ForegroundColor Magenta
Get-ChildItem -Path "env:OTEL_*" -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Host "  $($_.Name): $($_.Value)" -ForegroundColor Red
    Write-Host "  WARNING: Variable still exists in process environment!" -ForegroundColor Red
}

# Check User environment variables for any leftovers
$leftoverVars = [Environment]::GetEnvironmentVariables("User").Keys | Where-Object { $_ -like "OTEL_*" }
if ($leftoverVars) {
    Write-Host "`nWARNING: Still found OTEL variables in User environment:" -ForegroundColor Red
    foreach ($var in $leftoverVars) {
        $value = [Environment]::GetEnvironmentVariable($var, "User")
        Write-Host "  $var: $value" -ForegroundColor Red
    }
} else {
    Write-Host "`nNo OTEL environment variables found in User environment. Clean!" -ForegroundColor Green
}

# Check for Aspire dashboard variables
$aspireVars = [Environment]::GetEnvironmentVariables("User").Keys | Where-Object { $_ -like "*ASPIRE*" -or $_ -like "*DASHBOARD*" }
if ($aspireVars) {
    Write-Host "`nFound ASPIRE/DASHBOARD variables in User environment:" -ForegroundColor Yellow
    foreach ($var in $aspireVars) {
        $value = [Environment]::GetEnvironmentVariable($var, "User")
        Write-Host "  $var: $value" -ForegroundColor Yellow
    }
} else {
    Write-Host "`nNo ASPIRE/DASHBOARD environment variables found in User environment." -ForegroundColor Green
}

Write-Host "`nAll OpenTelemetry environment variables have been reset." -ForegroundColor Green
Write-Host "Now run setup-local-otlp.ps1 to set up for local Aspire collection." -ForegroundColor Magenta
