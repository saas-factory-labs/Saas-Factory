#!/usr/bin/env pwsh
# generate-telemetry-traffic.ps1
# Generates test traffic to populate telemetry data

Write-Host "Generating Test Traffic for Telemetry" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Gray

# Get services to test
$apiServiceUrl = "https://localhost:5002"
$webServiceUrl = "https://localhost:5001"
$appGatewayUrl = "https://localhost:5000"
$dashboardUrl = "http://localhost:$(if ($env:ASPIRE_DASHBOARD_PORT) { $env:ASPIRE_DASHBOARD_PORT } else { "15068" })"

# Function to test an endpoint
function Test-Endpoint {
    param (
        [string]$ServiceName,
        [string]$Url,
        [string]$Endpoint,
        [int]$Requests = 5
    )
    
    $fullUrl = "$Url$Endpoint"
    Write-Host "`nTesting $ServiceName at $fullUrl" -ForegroundColor Magenta
    
    for ($i = 1; $i -le $Requests; $i++) {
        Write-Host "  Request $i of $Requests..." -NoNewline
        
        try {
            $response = Invoke-WebRequest -Uri $fullUrl -SkipCertificateCheck -TimeoutSec 5 -ErrorAction SilentlyContinue
            Write-Host " ✅ Status: $($response.StatusCode)" -ForegroundColor Green
        } catch {
            if ($_.Exception.Response) {
                Write-Host " ⚠️ Status: $($_.Exception.Response.StatusCode)" -ForegroundColor Yellow
            } else {
                Write-Host " ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
            }
        }
        
        # Add a small delay between requests
        Start-Sleep -Milliseconds 500
    }
}

# Test API service endpoints
Test-Endpoint -ServiceName "API Service" -Url $apiServiceUrl -Endpoint "/health"
Test-Endpoint -ServiceName "API Service" -Url $apiServiceUrl -Endpoint "/api/health"
Test-Endpoint -ServiceName "API Service" -Url $apiServiceUrl -Endpoint "/api/values"
Test-Endpoint -ServiceName "API Service" -Url $apiServiceUrl -Endpoint "/swagger"

# Test Web service endpoints
Test-Endpoint -ServiceName "Web Service" -Url $webServiceUrl -Endpoint "/"
Test-Endpoint -ServiceName "Web Service" -Url $webServiceUrl -Endpoint "/health"
Test-Endpoint -ServiceName "Web Service" -Url $webServiceUrl -Endpoint "/api"

# Test AppGateway endpoints
Test-Endpoint -ServiceName "AppGateway" -Url $appGatewayUrl -Endpoint "/health"
Test-Endpoint -ServiceName "AppGateway" -Url $appGatewayUrl -Endpoint "/api/health"

# Test Aspire dashboard
Write-Host "`nTesting Aspire Dashboard" -ForegroundColor Magenta
try {
    $response = Invoke-WebRequest -Uri $dashboardUrl -TimeoutSec 5 -ErrorAction SilentlyContinue
    Write-Host "  ✅ Dashboard is accessible at $dashboardUrl (Status: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Dashboard is not accessible at $dashboardUrl" -ForegroundColor Red
    Write-Host "     Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nTest traffic generation complete!" -ForegroundColor Cyan
Write-Host "Now check the Aspire dashboard at $dashboardUrl to see the telemetry data." -ForegroundColor White
Write-Host "It may take a few moments for the telemetry data to appear in the dashboard." -ForegroundColor White
