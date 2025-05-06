#!/usr/bin/env pwsh
# check-aspire-otlp.ps1
# This script tests if the Aspire OpenTelemetry OTLP endpoint is properly configured

Write-Host "Aspire OpenTelemetry Dashboard Endpoint Check" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Gray

# Check env vars from different sources
$envVars = @(
    @{Source = "Environment Variable"; Value = [System.Environment]::GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL")},
    @{Source = "Environment Variable"; Value = [System.Environment]::GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")},
    @{Source = "Process Environment Variable"; Value = $env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL},
    @{Source = "Process Environment Variable"; Value = $env:OTEL_EXPORTER_OTLP_ENDPOINT}
)

# Check if any env vars are set
$hasEndpoint = $false
Write-Host "`nChecking environment variables:" -ForegroundColor Magenta
foreach ($var in $envVars) {
    if ($var.Value) {
        Write-Host "✅ $($var.Source): $($var.Value)" -ForegroundColor Green
        $hasEndpoint = $true
    }
}

if (-not $hasEndpoint) {
    Write-Host "❌ No OTLP endpoints found in environment variables" -ForegroundColor Red
}

# Check launchSettings.json
$launchSettingsPath = Join-Path $PSScriptRoot "AppBlueprint.AppHost" "Properties" "launchSettings.json"

Write-Host "`nChecking launchSettings.json:" -ForegroundColor Magenta
if (Test-Path $launchSettingsPath) {
    try {
        $launchSettings = Get-Content $launchSettingsPath -Raw | ConvertFrom-Json
        $httpProfile = $launchSettings.profiles.http
        $httpsProfile = $launchSettings.profiles.https
        
        if ($httpProfile) {
            $httpEndpoint = $httpProfile.environmentVariables.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL
            $httpOtelEndpoint = $httpProfile.environmentVariables.OTEL_EXPORTER_OTLP_ENDPOINT
            
            if ($httpEndpoint) {
                Write-Host "✅ HTTP profile DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $httpEndpoint" -ForegroundColor Green
                
                if ($httpOtelEndpoint -eq $httpEndpoint) {
                    Write-Host "✅ HTTP profile OTEL_EXPORTER_OTLP_ENDPOINT matches dashboard endpoint" -ForegroundColor Green
                } else {
                    Write-Host "❌ HTTP profile OTEL_EXPORTER_OTLP_ENDPOINT ($httpOtelEndpoint) doesn't match dashboard endpoint" -ForegroundColor Red
                }
            } else {
                Write-Host "❌ HTTP profile missing DOTNET_DASHBOARD_OTLP_ENDPOINT_URL" -ForegroundColor Red
            }
        }
        
        if ($httpsProfile) {
            $httpsEndpoint = $httpsProfile.environmentVariables.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL
            $httpsOtelEndpoint = $httpsProfile.environmentVariables.OTEL_EXPORTER_OTLP_ENDPOINT
            
            if ($httpsEndpoint) {
                Write-Host "✅ HTTPS profile DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $httpsEndpoint" -ForegroundColor Green
                
                if ($httpsOtelEndpoint -eq $httpsEndpoint) {
                    Write-Host "✅ HTTPS profile OTEL_EXPORTER_OTLP_ENDPOINT matches dashboard endpoint" -ForegroundColor Green
                } else {
                    Write-Host "❌ HTTPS profile OTEL_EXPORTER_OTLP_ENDPOINT ($httpsOtelEndpoint) doesn't match dashboard endpoint" -ForegroundColor Red
                }
            } else {
                Write-Host "❌ HTTPS profile missing DOTNET_DASHBOARD_OTLP_ENDPOINT_URL" -ForegroundColor Red
            }
        }
    } catch {
        Write-Host "❌ Error parsing launchSettings.json: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "❌ launchSettings.json not found at $launchSettingsPath" -ForegroundColor Red
}

# Check running processes
Write-Host "`nChecking for running Aspire services:" -ForegroundColor Magenta
$aspireProcesses = Get-Process -Name "*dotnet*" | Where-Object { $_.CommandLine -like "*aspire*" -or $_.CommandLine -like "*AppBlueprint.AppHost*" } -ErrorAction SilentlyContinue

if ($aspireProcesses -and $aspireProcesses.Count -gt 0) {
    foreach ($proc in $aspireProcesses) {
        Write-Host "✅ Found Aspire process: $($proc.Id) - $($proc.CommandLine)" -ForegroundColor Green
    }
} else {
    Write-Host "❌ No Aspire processes found running" -ForegroundColor Red
}

# Add troubleshooting info
Write-Host "`nOTLP Endpoint Requirements for Aspire:" -ForegroundColor Yellow
Write-Host "1. DOTNET_DASHBOARD_OTLP_ENDPOINT_URL and OTEL_EXPORTER_OTLP_ENDPOINT should match"
Write-Host "2. The OTLP endpoint should be using port 18889 (default Aspire dashboard port)"
Write-Host "3. Protocol should be 'http/protobuf' for Aspire HTTP/protobuf collector"

Write-Host "`nQuick Fix:" -ForegroundColor Green
Write-Host "1. Run the setup-aspire-otlp.ps1 script to set the environment variables:"
Write-Host "   .\setup-aspire-otlp.ps1"
Write-Host "2. Restart your PowerShell session"
Write-Host "3. Restart the Aspire dashboard: dotnet run --project AppBlueprint.AppHost"
Write-Host "4. Check the dashboard for telemetry data"
