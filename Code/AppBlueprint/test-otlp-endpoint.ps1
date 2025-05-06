#!/usr/bin/env pwsh
# test-otlp-endpoint.ps1
# This script tests if the OpenTelemetry endpoint is accessible and receiving data

Write-Host "Testing OpenTelemetry Endpoint" -ForegroundColor Cyan
Write-Host "============================" -ForegroundColor Gray

# Get the OTLP endpoint from environment
$otlpEndpoint = $env:OTEL_EXPORTER_OTLP_ENDPOINT
if (-not $otlpEndpoint) {
    $otlpEndpoint = $env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL
    if (-not $otlpEndpoint) {
        $otlpEndpoint = "http://localhost:18889"
    }
}

Write-Host "Using OTLP endpoint: $otlpEndpoint" -ForegroundColor Yellow

# Test the OTLP endpoint with a simple HEAD request to check if it's up
Write-Host "Testing OTLP endpoint availability..." -ForegroundColor Magenta
try {
    $response = Invoke-WebRequest -Uri $otlpEndpoint -Method HEAD -TimeoutSec 2 -ErrorAction SilentlyContinue
    Write-Host "✅ OTLP endpoint is accessible at $otlpEndpoint" -ForegroundColor Green
} catch {
    # Even if it returns an error, check if the server is responding
    if ($_.Exception.Response) {
        $statusCode = [int]$_.Exception.Response.StatusCode
        Write-Host "⚠️ OTLP endpoint returned status code: $statusCode" -ForegroundColor Yellow
        Write-Host "This is expected - the endpoint requires specific protocol and payload" -ForegroundColor Yellow
    } else {
        Write-Host "❌ OTLP endpoint is not accessible at $otlpEndpoint" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Check if the OTLP traces endpoint is accessible
Write-Host "`nTesting OTLP traces endpoint..." -ForegroundColor Magenta
$tracesEndpoint = "$otlpEndpoint/v1/traces"
try {
    $response = Invoke-WebRequest -Uri $tracesEndpoint -Method HEAD -TimeoutSec 2 -ErrorAction SilentlyContinue
    Write-Host "✅ OTLP traces endpoint is accessible at $tracesEndpoint" -ForegroundColor Green
} catch {
    # Even if it returns an error, check if the server is responding
    if ($_.Exception.Response) {
        $statusCode = [int]$_.Exception.Response.StatusCode
        Write-Host "⚠️ OTLP traces endpoint returned status code: $statusCode" -ForegroundColor Yellow
        if ($statusCode -eq 400) {
            Write-Host "This is expected - the endpoint requires a specific payload format" -ForegroundColor Yellow
        }
    } else {
        Write-Host "❌ OTLP traces endpoint is not accessible at $tracesEndpoint" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Check if the OTLP metrics endpoint is accessible
Write-Host "`nTesting OTLP metrics endpoint..." -ForegroundColor Magenta
$metricsEndpoint = "$otlpEndpoint/v1/metrics"
try {
    $response = Invoke-WebRequest -Uri $metricsEndpoint -Method HEAD -TimeoutSec 2 -ErrorAction SilentlyContinue
    Write-Host "✅ OTLP metrics endpoint is accessible at $metricsEndpoint" -ForegroundColor Green
} catch {
    # Even if it returns an error, check if the server is responding
    if ($_.Exception.Response) {
        $statusCode = [int]$_.Exception.Response.StatusCode
        Write-Host "⚠️ OTLP metrics endpoint returned status code: $statusCode" -ForegroundColor Yellow
        if ($statusCode -eq 400) {
            Write-Host "This is expected - the endpoint requires a specific payload format" -ForegroundColor Yellow
        }
    } else {
        Write-Host "❌ OTLP metrics endpoint is not accessible at $metricsEndpoint" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Check if the AppHost is running
Write-Host "`nChecking if AppHost is running..." -ForegroundColor Magenta
$appHostProcess = Get-Process -Name "AppBlueprint.AppHost" -ErrorAction SilentlyContinue
if ($appHostProcess) {
    Write-Host "✅ AppHost is running (PID: $($appHostProcess.Id))" -ForegroundColor Green
} else {
    Write-Host "❌ AppHost is not running" -ForegroundColor Red
}

# Check if other services are running
Write-Host "`nChecking if other AppBlueprint services are running..." -ForegroundColor Magenta
$otherServices = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue | Where-Object { $_.ProcessName -ne "AppBlueprint.AppHost" }
if ($otherServices) {
    Write-Host "✅ Found $(($otherServices | Measure-Object).Count) other AppBlueprint services running:" -ForegroundColor Green
    foreach ($service in $otherServices) {
        Write-Host "  • $($service.ProcessName) (PID: $($service.Id))" -ForegroundColor Green
    }
} else {
    Write-Host "❌ No other AppBlueprint services found running" -ForegroundColor Red
}

# Print summary
Write-Host "`nSummary:" -ForegroundColor Magenta
Write-Host "1. Make sure the Aspire dashboard is running (dotnet run --project AppBlueprint.AppHost)" -ForegroundColor White
Write-Host "2. Check that services are properly configured with correct OTLP endpoints" -ForegroundColor White
Write-Host "3. The Aspire dashboard should show telemetry data at http://localhost:15068" -ForegroundColor White
Write-Host "4. If you don't see telemetry data, try restarting all services and generating some traffic" -ForegroundColor White
