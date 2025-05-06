#!/usr/bin/env pwsh
# test-current-session-telemetry.ps1
# This script sets up the current PowerShell session with the correct OTLP variables
# and runs a test to verify telemetry collection

Write-Host "Testing OpenTelemetry with Current Session Variables" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Gray

# Set process-level environment variables (only affects current PowerShell session)
$env:OTEL_EXPORTER_OTLP_ENDPOINT = "http://localhost:18889"
$env:OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
$env:OTEL_EXPORTER_OTLP_HEADERS = ""
$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = "http://localhost:18889"
$env:ASPIRE_DASHBOARD_PORT = "15068"

# Display current session environment variables
Write-Host "`nCurrent session environment variables:" -ForegroundColor Magenta
Write-Host "  OTEL_EXPORTER_OTLP_ENDPOINT: $env:OTEL_EXPORTER_OTLP_ENDPOINT" -ForegroundColor Yellow
Write-Host "  OTEL_EXPORTER_OTLP_PROTOCOL: $env:OTEL_EXPORTER_OTLP_PROTOCOL" -ForegroundColor Yellow
Write-Host "  OTEL_EXPORTER_OTLP_HEADERS: '$env:OTEL_EXPORTER_OTLP_HEADERS'" -ForegroundColor Yellow
Write-Host "  DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL" -ForegroundColor Yellow
Write-Host "  ASPIRE_DASHBOARD_PORT: $env:ASPIRE_DASHBOARD_PORT" -ForegroundColor Yellow

# Check if AppHost is running
$appHost = Get-Process -Name "AppBlueprint.AppHost" -ErrorAction SilentlyContinue
if (-not $appHost) {
    # Try to start the AppHost with explicit environment variables
    Write-Host "`nAppHost not running, starting it with correct variables..." -ForegroundColor Magenta
    
    # Create a new process with the environment variables
    $startInfo = New-Object System.Diagnostics.ProcessStartInfo
    $startInfo.FileName = "dotnet"
    $startInfo.Arguments = "run --project AppBlueprint.AppHost"
    $startInfo.WorkingDirectory = $PSScriptRoot
    
    # Set environment variables
    $startInfo.EnvironmentVariables["OTEL_EXPORTER_OTLP_ENDPOINT"] = "http://localhost:18889"
    $startInfo.EnvironmentVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] = "http/protobuf"
    $startInfo.EnvironmentVariables["OTEL_EXPORTER_OTLP_HEADERS"] = ""
    $startInfo.EnvironmentVariables["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"] = "http://localhost:18889"
    $startInfo.EnvironmentVariables["ASPIRE_DASHBOARD_PORT"] = "15068"
    
    # Start process
    $startInfo.UseShellExecute = $false
    $process = [System.Diagnostics.Process]::Start($startInfo)
    
    Write-Host "  Started AppHost with PID: $($process.Id)" -ForegroundColor Green
    Write-Host "  Waiting for AppHost to initialize (15 seconds)..." -ForegroundColor Yellow
    Start-Sleep -Seconds 15
}
else {
    Write-Host "`nAppHost already running (PID: $($appHost.Id))" -ForegroundColor Yellow
    
    # We'll later restart it with correct environment variables
    Write-Host "  Stopping existing AppHost process..." -ForegroundColor Yellow
    Stop-Process -Id $appHost.Id -Force
    Start-Sleep -Seconds 2
    
    # Start new AppHost with correct environment variables
    Write-Host "  Starting new AppHost with correct variables..." -ForegroundColor Yellow
    
    # Create a new process with the environment variables
    $startInfo = New-Object System.Diagnostics.ProcessStartInfo
    $startInfo.FileName = "dotnet"
    $startInfo.Arguments = "run --project AppBlueprint.AppHost"
    $startInfo.WorkingDirectory = $PSScriptRoot
    
    # Set environment variables
    $startInfo.EnvironmentVariables["OTEL_EXPORTER_OTLP_ENDPOINT"] = "http://localhost:18889"
    $startInfo.EnvironmentVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] = "http/protobuf"
    $startInfo.EnvironmentVariables["OTEL_EXPORTER_OTLP_HEADERS"] = ""
    $startInfo.EnvironmentVariables["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"] = "http://localhost:18889"
    $startInfo.EnvironmentVariables["ASPIRE_DASHBOARD_PORT"] = "15068"
    
    # Start process
    $startInfo.UseShellExecute = $false
    $process = [System.Diagnostics.Process]::Start($startInfo)
    
    Write-Host "  Started new AppHost with PID: $($process.Id)" -ForegroundColor Green
    Write-Host "  Waiting for AppHost to initialize (15 seconds)..." -ForegroundColor Yellow
    Start-Sleep -Seconds 15
}

# Test Aspire dashboard
Write-Host "`nChecking Aspire dashboard..." -ForegroundColor Magenta
try {
    $response = Invoke-WebRequest -Uri "http://localhost:15068" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "  ✅ Aspire dashboard is accessible (Status: $($response.StatusCode))" -ForegroundColor Green
    
    # Check metrics page
    $metricsResponse = Invoke-WebRequest -Uri "http://localhost:15068/metrics" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "  ✅ Metrics page is accessible (Status: $($metricsResponse.StatusCode))" -ForegroundColor Green
    
    # Look for telemetry data
    if ($metricsResponse.Content -match "telemetry" -or $metricsResponse.Content -match "metric") {
        Write-Host "  ✅ Telemetry data appears to be present!" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️ Dashboard is accessible but may not have telemetry data yet" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  ❌ Failed to access Aspire dashboard: $($_.Exception.Message)" -ForegroundColor Red
}

# Test OTLP endpoints
Write-Host "`nChecking OTLP endpoints..." -ForegroundColor Magenta
try {
    $response = Invoke-WebRequest -Uri "http://localhost:18889/v1/metrics" -Method HEAD -TimeoutSec 2 -ErrorAction SilentlyContinue
    Write-Host "  ✅ OTLP metrics endpoint is responding" -ForegroundColor Green
} catch {
    if ($_.Exception.Response -and $_.Exception.Response.StatusCode.value__ -eq 400) {
        Write-Host "  ✅ OTLP metrics endpoint is responding (Expected 400 response)" -ForegroundColor Green
    } else {
        Write-Host "  ❌ OTLP metrics endpoint not responding: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nTest completed. If you don't see telemetry data in the dashboard:" -ForegroundColor Cyan
Write-Host "1. Open Process Explorer and check the environment variables of the AppHost process" -ForegroundColor White
Write-Host "2. Verify that OTEL_EXPORTER_OTLP_ENDPOINT is http://localhost:18889" -ForegroundColor White
Write-Host "3. Verify that OTEL_EXPORTER_OTLP_HEADERS is empty" -ForegroundColor White
Write-Host "4. Access the Aspire dashboard at http://localhost:15068" -ForegroundColor White
Write-Host "5. Check the console output of the AppHost for any errors related to OpenTelemetry" -ForegroundColor White
