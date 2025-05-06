#!/usr/bin/env pwsh
# full-aspire-test.ps1
# A comprehensive test script for Aspire dashboard and services

Write-Host "Comprehensive Aspire Dashboard & OpenTelemetry Test" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Gray

# Function to test HTTP endpoint with custom options
function Test-HttpEndpoint {
    param (
        [string]$Name,
        [string]$Url,
        [string]$Method = "GET",
        [int]$TimeoutSec = 5,
        [string]$ContentType,
        [string]$Body,
        [switch]$IgnoreErrors
    )

    try {
        Write-Host "  Testing $Name at $Url..." -ForegroundColor Yellow -NoNewline
        
        $params = @{
            Uri = $Url
            Method = $Method
            TimeoutSec = $TimeoutSec
            ErrorAction = "Stop"
        }
        
        if ($ContentType) { $params.ContentType = $ContentType }
        if ($Body) { $params.Body = $Body }
        
        $response = Invoke-WebRequest @params
        
        Write-Host " ✅ Success ($($response.StatusCode))" -ForegroundColor Green
        return $true
    }
    catch {
        if ($IgnoreErrors -and $_.Exception.Response -and $_.Exception.Response.StatusCode.value__ -eq 400) {
            Write-Host " ⚠️ Expected 400 Error (OTLP endpoint requires specific payload)" -ForegroundColor Yellow
            return $true
        }
        else {
            Write-Host " ❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
            return $false
        }
    }
}

# Collect environment variables
$otlpEndpoint = $env:OTEL_EXPORTER_OTLP_ENDPOINT
if (-not $otlpEndpoint) {
    $otlpEndpoint = $env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL
    if (-not $otlpEndpoint) {
        $otlpEndpoint = "http://localhost:18889"
    }
}

$dashboardPort = $env:ASPIRE_DASHBOARD_PORT
if (-not $dashboardPort) {
    $dashboardPort = "15068"
}

$protocol = $env:OTEL_EXPORTER_OTLP_PROTOCOL
if (-not $protocol) {
    $protocol = "http/protobuf"
}

$headers = $env:OTEL_EXPORTER_OTLP_HEADERS
if (-not $headers) {
    $headers = ""
}

# Print configuration
Write-Host "`nConfiguration:" -ForegroundColor Magenta
Write-Host "  OTLP Endpoint: $otlpEndpoint" -ForegroundColor White
Write-Host "  Dashboard Port: $dashboardPort" -ForegroundColor White
Write-Host "  OTLP Protocol: $protocol" -ForegroundColor White
Write-Host "  OTLP Headers: $headers" -ForegroundColor White

# Check running services
Write-Host "`nChecking running AppBlueprint services:" -ForegroundColor Magenta
$appProcesses = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue
if ($appProcesses) {
    foreach ($proc in $appProcesses) {
        Write-Host "  • $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Green
    }
} else {
    Write-Host "  No AppBlueprint services are running!" -ForegroundColor Red
    Write-Host "  Please start the AppHost: dotnet run --project AppBlueprint.AppHost" -ForegroundColor Yellow
    exit 1
}

# Check Aspire dashboard
$dashboardUrl = "http://localhost:$dashboardPort"
Write-Host "`nChecking Aspire dashboard:" -ForegroundColor Magenta
$dashboardAccessible = Test-HttpEndpoint -Name "Aspire Dashboard" -Url $dashboardUrl

if (-not $dashboardAccessible) {
    Write-Host "  The Aspire dashboard is not accessible. Please ensure the AppHost is running correctly." -ForegroundColor Red
    exit 1
}

# Check OTLP endpoints (they should return 400 Bad Request when accessed without payload)
Write-Host "`nChecking OTLP endpoints:" -ForegroundColor Magenta
Test-HttpEndpoint -Name "OTLP Metrics Endpoint" -Url "$otlpEndpoint/v1/metrics" -IgnoreErrors
Test-HttpEndpoint -Name "OTLP Traces Endpoint" -Url "$otlpEndpoint/v1/traces" -IgnoreErrors

# Generate some traffic to service endpoints
Write-Host "`nGenerating traffic to service endpoints:" -ForegroundColor Magenta

# Get service URLs from env vars or use defaults
$apiUrl = $env:DOTNET_API_SERVICE_ENDPOINT_URL
if (-not $apiUrl) { $apiUrl = "http://localhost:8090" }

$webUrl = $env:DOTNET_WEB_SERVICE_ENDPOINT_URL
if (-not $webUrl) { $webUrl = "http://localhost:8092" }

$gatewayUrl = $env:DOTNET_GATEWAY_SERVICE_ENDPOINT_URL
if (-not $gatewayUrl) { $gatewayUrl = "http://localhost:8094" }

# Send requests to services that should generate telemetry
Test-HttpEndpoint -Name "Web UI" -Url $webUrl
Test-HttpEndpoint -Name "API Health Check" -Url "$apiUrl/health"
Test-HttpEndpoint -Name "Gateway Health Check" -Url "$gatewayUrl/health"

# Check the dashboard metrics page
Write-Host "`nChecking dashboard metrics page:" -ForegroundColor Magenta
Test-HttpEndpoint -Name "Dashboard Metrics Page" -Url "$dashboardUrl/metrics"

Write-Host "`nTest complete. If the dashboard is not showing telemetry:" -ForegroundColor Cyan
Write-Host "1. Check console output of the running AppHost for any errors" -ForegroundColor White
Write-Host "2. Verify all services are properly configured with ServiceDefaults" -ForegroundColor White
Write-Host "3. Allow a few minutes for telemetry data to appear in the dashboard" -ForegroundColor White
Write-Host "4. Try accessing the dashboard at $dashboardUrl directly in a browser" -ForegroundColor White
