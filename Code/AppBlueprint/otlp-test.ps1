#!/usr/bin/env pwsh
# otlp-test.ps1
# This script tests if the OpenTelemetry OTLP endpoint is properly configured and accessible

Write-Host "OpenTelemetry OTLP Endpoint Test" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Gray

# Function to test HTTP endpoint
function Test-HttpEndpoint {
    param (
        [string]$Url,
        [string]$Method = "HEAD",
        [int]$TimeoutSec = 5,
        [string]$ContentType,
        [string]$Body
    )

    try {
        $params = @{
            Uri = $Url
            Method = $Method
            TimeoutSec = $TimeoutSec
            ErrorAction = "SilentlyContinue"
        }

        if ($ContentType) {
            $params.Add("ContentType", $ContentType)
        }

        if ($Body) {
            $params.Add("Body", $Body)
        }

        $response = Invoke-WebRequest @params
        return @{
            Success = $true
            StatusCode = $response.StatusCode
            StatusDescription = $response.StatusDescription
        }
    }
    catch [System.Net.WebException] {
        $ex = $_.Exception
        if ($ex.Response) {
            $response = $ex.Response
            return @{
                Success = $false
                StatusCode = $response.StatusCode.value__
                StatusDescription = $response.StatusDescription
                Error = $ex.Message
            }
        }
        else {
            return @{
                Success = $false
                Error = $ex.Message
            }
        }
    }
    catch {
        return @{
            Success = $false
            Error = $_.Exception.Message
        }
    }
}

# Check if the environment variables are set
$otlpEndpoint = [System.Environment]::GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
$dashboardEndpoint = [System.Environment]::GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL")

if (-not $otlpEndpoint) {
    if ($dashboardEndpoint) {
        # Use Aspire dashboard endpoint if available
        $otlpEndpoint = $dashboardEndpoint
        Write-Host "Using DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $otlpEndpoint" -ForegroundColor Yellow
        [System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $otlpEndpoint)
    } else {
        # Default to standard Aspire OTLP endpoint
        $otlpEndpoint = "http://localhost:18889"
        Write-Host "OTEL_EXPORTER_OTLP_ENDPOINT not set. Using Aspire dashboard default: $otlpEndpoint" -ForegroundColor Yellow
        [System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $otlpEndpoint)
    }
}
else {
    Write-Host "OTEL_EXPORTER_OTLP_ENDPOINT is set to: $otlpEndpoint" -ForegroundColor Green
}

# Test the OTLP metrics endpoint
Write-Host "`nTesting OTLP metrics endpoint..." -ForegroundColor Cyan
$metricsUrl = "$otlpEndpoint/v1/metrics"
$metricsResult = Test-HttpEndpoint -Url $metricsUrl -Method "POST" -ContentType "application/json" -Body "{}"

if ($metricsResult.Success) {
    Write-Host "✅ Metrics endpoint is accessible! Status: $($metricsResult.StatusCode)" -ForegroundColor Green
} else {
    Write-Host "❌ Metrics endpoint is not accessible: $($metricsResult.Error)" -ForegroundColor Red
    
    # Try HTTP if HTTPS fails
    if ($otlpEndpoint -like "https://*") {
        $httpEndpoint = $otlpEndpoint -replace "^https://", "http://"
        Write-Host "Testing fallback HTTP endpoint: $httpEndpoint/v1/metrics" -ForegroundColor Yellow
        $httpResult = Test-HttpEndpoint -Url "$httpEndpoint/v1/metrics" -Method "POST" -ContentType "application/json" -Body "{}"
        
        if ($httpResult.Success) {
            Write-Host "✅ HTTP fallback endpoint is accessible! Status: $($httpResult.StatusCode)" -ForegroundColor Green
            Write-Host "Consider using HTTP instead of HTTPS for your OTLP endpoint" -ForegroundColor Yellow
            $otlpEndpoint = $httpEndpoint
            [System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $httpEndpoint)
        } else {
            Write-Host "❌ HTTP fallback endpoint is also not accessible: $($httpResult.Error)" -ForegroundColor Red
        }
    }
}

# Test the OTLP traces endpoint
Write-Host "`nTesting OTLP traces endpoint..." -ForegroundColor Cyan
$tracesUrl = "$otlpEndpoint/v1/traces"
$tracesResult = Test-HttpEndpoint -Url $tracesUrl -Method "POST" -ContentType "application/json" -Body "{}"

if ($tracesResult.Success) {
    Write-Host "✅ Traces endpoint is accessible! Status: $($tracesResult.StatusCode)" -ForegroundColor Green
} else {
    Write-Host "❌ Traces endpoint is not accessible: $($tracesResult.Error)" -ForegroundColor Red
}

# Test the OTLP health endpoint
Write-Host "`nTesting OTLP health endpoint..." -ForegroundColor Cyan
$healthUrl = "$otlpEndpoint/health"
$healthResult = Test-HttpEndpoint -Url $healthUrl -Method "GET"

if ($healthResult.Success) {
    Write-Host "✅ OTLP health endpoint is accessible! Status: $($healthResult.StatusCode)" -ForegroundColor Green
} else {
    Write-Host "❌ OTLP health endpoint is not accessible: $($healthResult.Error)" -ForegroundColor Red
}

# Check for the Aspire dashboard
$dashboardUrl = "http://localhost:15068"
Write-Host "`nChecking Aspire dashboard..." -ForegroundColor Cyan
$dashboardResult = Test-HttpEndpoint -Url $dashboardUrl

if ($dashboardResult.Success) {
    Write-Host "✅ Aspire dashboard is accessible! Status: $($dashboardResult.StatusCode)" -ForegroundColor Green
} else {
    Write-Host "❌ Aspire dashboard is not accessible: $($dashboardResult.Error)" -ForegroundColor Red
    Write-Host "  Make sure the Aspire application host is running." -ForegroundColor Yellow
}

# Check if any AppBlueprint processes are running
Write-Host "`nChecking for running AppBlueprint processes..." -ForegroundColor Cyan
$processes = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue
if ($processes) {
    Write-Host "Found running AppBlueprint processes:" -ForegroundColor Green
    foreach ($proc in $processes) {
        Write-Host "  - $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor White
    }
} else {
    Write-Host "❌ No AppBlueprint processes found." -ForegroundColor Red
    Write-Host "  Start the AppHost project to run the application." -ForegroundColor Yellow
}

# Display OpenTelemetry package versions
Write-Host "`nChecking OpenTelemetry package versions..." -ForegroundColor Cyan
$packagesPath = Join-Path $PSScriptRoot "Directory.Packages.props"
if (Test-Path $packagesPath) {
    $packages = Get-Content $packagesPath -Raw
    
    $otelPackages = @(
        "OpenTelemetry",
        "OpenTelemetry.Api",
        "OpenTelemetry.Exporter.Console",
        "OpenTelemetry.Exporter.OpenTelemetryProtocol",
        "OpenTelemetry.Extensions.Hosting",
        "OpenTelemetry.Instrumentation.AspNetCore",
        "OpenTelemetry.Instrumentation.Http",
        "OpenTelemetry.Instrumentation.Runtime"
    )
    
    foreach ($package in $otelPackages) {
        if ($packages -match [regex]::Escape($package) + '"\s+Version="([^"]+)"') {
            Write-Host "  $($package): $($Matches[1])" -ForegroundColor Green
        } else {
            Write-Host "  $($package): Not found" -ForegroundColor Red
        }
    }
} else {
    Write-Host "  Directory.Packages.props not found!" -ForegroundColor Red
}

# Provide troubleshooting tips
Write-Host "`nTroubleshooting Tips:" -ForegroundColor Magenta
Write-Host "1. Make sure the Aspire dashboard is running (dotnet run --project AppBlueprint.AppHost)"
Write-Host "2. Check that OTEL_EXPORTER_OTLP_ENDPOINT is correctly set and accessible"
Write-Host "3. Verify that all services are including the ServiceDefaults project"
Write-Host "4. Check firewall settings if endpoints are not accessible"
Write-Host "5. Try manually starting the services to verify they're sending telemetry"

Write-Host "`nTo fix missing telemetry data:" -ForegroundColor Cyan
Write-Host "1. Modify Extensions.cs in AppBlueprint.ServiceDefaults to use the provided OpenTelemetry config"
Write-Host "2. Make sure all projects reference the ServiceDefaults project"
Write-Host "3. Rebuild and restart the application"
