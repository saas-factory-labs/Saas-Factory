#!/usr/bin/env pwsh
# aspire-test-telemetry.ps1
# This script sends test telemetry data to the Aspire dashboard OTLP endpoint

Write-Host "Testing Aspire Dashboard OTLP Endpoint" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Gray

# Get the OTLP endpoint from environment
$otlpEndpoint = $env:OTEL_EXPORTER_OTLP_ENDPOINT
if (-not $otlpEndpoint) {
    $otlpEndpoint = $env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL
    if (-not $otlpEndpoint) {
        $otlpEndpoint = "http://localhost:18889"
    }
}

Write-Host "Using OTLP endpoint: $otlpEndpoint" -ForegroundColor Yellow

# First, verify the Aspire dashboard is running
$dashboardPort = $env:ASPIRE_DASHBOARD_PORT
if (-not $dashboardPort) {
    $dashboardPort = "15068"
}

$dashboardUrl = "http://localhost:$dashboardPort"
try {
    # Use Invoke-WebRequest with a timeout to check if the dashboard is accessible
    Write-Host "Checking Aspire dashboard at $dashboardUrl..." -ForegroundColor Yellow
    $response = Invoke-WebRequest -Uri $dashboardUrl -TimeoutSec 5 -ErrorAction Stop
    Write-Host "✅ Aspire dashboard is accessible at $dashboardUrl" -ForegroundColor Green
}
catch {
    # Try to check if there's a process listening on the dashboard port
    $portListening = netstat -an | Select-String -Pattern ":$dashboardPort"
    
    if ($portListening) {
        Write-Host "⚠️ Process is listening on port $dashboardPort but we can't access the dashboard." -ForegroundColor Yellow
        Write-Host "This could be a temporary issue. Continuing with the test..." -ForegroundColor Yellow
    }
    else {
        Write-Host "❌ Aspire dashboard is not accessible at $dashboardUrl" -ForegroundColor Red
        Write-Host "Please make sure the Aspire AppHost is running: dotnet run --project AppBlueprint.AppHost" -ForegroundColor Yellow
        exit 1
    }
}

# Second, let's check if any of the services are running and sending telemetry
$processNames = @("AppBlueprint.ApiService", "AppBlueprint.Web", "AppBlueprint.AppGateway")
$runningServices = Get-Process -Name $processNames -ErrorAction SilentlyContinue

if ($runningServices) {
    Write-Host "`nRunning AppBlueprint services:" -ForegroundColor Magenta
    foreach ($service in $runningServices) {
        Write-Host "  • $($service.ProcessName) (PID: $($service.Id))" -ForegroundColor Green
    }
}
else {
    Write-Host "`n❌ No AppBlueprint services found running!" -ForegroundColor Red
    Write-Host "Telemetry needs services to be running to generate data." -ForegroundColor Yellow
    Write-Host "Make sure the AppHost is running: dotnet run --project AppBlueprint.AppHost" -ForegroundColor Yellow
}

# Help generate some telemetry by making requests to services
Write-Host "`nGenerating test telemetry by making requests to services..." -ForegroundColor Magenta

# Function to test an HTTP endpoint
function Test-HttpEndpoint {
    param (
        [string]$Name,
        [string]$Url
    )

    try {
        Write-Host "  Testing $Name at $Url..." -ForegroundColor Yellow -NoNewline
        $response = Invoke-WebRequest -Uri $Url -Method GET -TimeoutSec 5 -ErrorAction Stop
        Write-Host " ✅ Success ($($response.StatusCode))" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host " ❌ Failed: $_" -ForegroundColor Red
        return $false
    }
}

# Get the service URLs from environment variables
$webUrl = $env:DOTNET_WEB_SERVICE_ENDPOINT_URL
if (-not $webUrl) {
    $webUrl = "http://localhost:8092"
}

$apiUrl = $env:DOTNET_API_SERVICE_ENDPOINT_URL
if (-not $apiUrl) {
    $apiUrl = "http://localhost:8090"
}

$gatewayUrl = $env:DOTNET_GATEWAY_SERVICE_ENDPOINT_URL
if (-not $gatewayUrl) {
    $gatewayUrl = "http://localhost:8094"
}

# Make test requests to generate telemetry
Test-HttpEndpoint -Name "Web UI" -Url $webUrl
Test-HttpEndpoint -Name "API" -Url "$apiUrl/health" 
Test-HttpEndpoint -Name "Gateway" -Url "$gatewayUrl/health"

Write-Host "`nTelemetry test complete. Check the Aspire dashboard at $dashboardUrl for data." -ForegroundColor Cyan
Write-Host "Note: It may take a few seconds for telemetry data to appear in the dashboard." -ForegroundColor Yellow
Write-Host "If you still don't see data, try the following:" -ForegroundColor Magenta
Write-Host "1. Verify OTEL_EXPORTER_OTLP_ENDPOINT is set to: $otlpEndpoint" -ForegroundColor White
Write-Host "2. Verify OTEL_EXPORTER_OTLP_HEADERS is empty (not set)" -ForegroundColor White
Write-Host "3. Verify OTEL_EXPORTER_OTLP_PROTOCOL is set to: http/protobuf" -ForegroundColor White
Write-Host "4. Restart all services and try again" -ForegroundColor White
