#!/usr/bin/env pwsh
# fix-all-telemetry-components.ps1
# This script fixes telemetry configuration across all microservices

Write-Host "Fixing Telemetry Configuration Across All Components" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Gray

# 1. First, stop all running processes
Write-Host "`nStopping all AppBlueprint processes..." -ForegroundColor Magenta
$appProcesses = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue

if ($appProcesses) {
    foreach ($proc in $appProcesses) {
        Write-Host "  Stopping $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Yellow
        Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    }
    # Give processes time to fully stop
    Start-Sleep -Seconds 3
}

# 2. Set correct environment variables system-wide
Write-Host "`nSetting consistent OTLP environment variables..." -ForegroundColor Magenta
$aspireOtlpEndpoint = "http://localhost:18889"
$dashboardPort = "15068"

# Set at user level first
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $aspireOtlpEndpoint, "User")
[System.Environment]::SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", $aspireOtlpEndpoint, "User")
[System.Environment]::SetEnvironmentVariable("ASPIRE_DASHBOARD_PORT", $dashboardPort, "User")
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf", "User")
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "", "User")
[System.Environment]::SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true", "User")

# Also for current process
$env:OTEL_EXPORTER_OTLP_ENDPOINT = $aspireOtlpEndpoint
$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = $aspireOtlpEndpoint
$env:ASPIRE_DASHBOARD_PORT = $dashboardPort
$env:OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
$env:OTEL_EXPORTER_OTLP_HEADERS = ""
$env:ASPIRE_ALLOW_UNSECURED_TRANSPORT = "true"

Write-Host "  ✅ Environment variables set system-wide" -ForegroundColor Green

# 3. Rebuild the solution
Write-Host "`nRebuilding the solution..." -ForegroundColor Magenta
try {
    Push-Location "C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"
    dotnet clean
    dotnet build
    Write-Host "  ✅ Solution rebuilt successfully" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Error rebuilding solution: $_" -ForegroundColor Red
    exit 1
} finally {
    Pop-Location
}

# 4. Start AppHost with correct telemetry configuration
Write-Host "`nStarting AppHost with correct telemetry configuration..." -ForegroundColor Magenta

$startupScript = @"
`$env:OTEL_EXPORTER_OTLP_ENDPOINT = '$aspireOtlpEndpoint'
`$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = '$aspireOtlpEndpoint'
`$env:ASPIRE_DASHBOARD_PORT = '$dashboardPort'
`$env:OTEL_EXPORTER_OTLP_PROTOCOL = 'http/protobuf'
`$env:OTEL_EXPORTER_OTLP_HEADERS = ''
`$env:ASPIRE_ALLOW_UNSECURED_TRANSPORT = 'true'

Write-Host "Starting AppHost with following variables:" -ForegroundColor Green
Write-Host "OTEL_EXPORTER_OTLP_ENDPOINT: `$env:OTEL_EXPORTER_OTLP_ENDPOINT"
Write-Host "OTEL_EXPORTER_OTLP_PROTOCOL: `$env:OTEL_EXPORTER_OTLP_PROTOCOL"
Write-Host "OTEL_EXPORTER_OTLP_HEADERS: '`$env:OTEL_EXPORTER_OTLP_HEADERS'"
Write-Host "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: `$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"
Write-Host "ASPIRE_DASHBOARD_PORT: `$env:ASPIRE_DASHBOARD_PORT"

Set-Location "C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"
dotnet run --project AppBlueprint.AppHost
"@

$startupScriptPath = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), "start-apphost-with-telemetry.ps1")
Set-Content -Path $startupScriptPath -Value $startupScript

# Start AppHost in a new PowerShell window with explicit environment variables
Start-Process "pwsh.exe" -ArgumentList "-NoExit", "-File", $startupScriptPath

Write-Host "  ✅ Started AppHost with correct telemetry configuration" -ForegroundColor Green
Write-Host "  📊 Aspire dashboard should be available at: http://localhost:15068" -ForegroundColor Green

# 5. Wait and test endpoints
Write-Host "`nWaiting for services to start (20 seconds)..." -ForegroundColor Magenta
Start-Sleep -Seconds 20

Write-Host "`nTesting telemetry endpoints..." -ForegroundColor Magenta

# Test Aspire dashboard
try {
    $dashboardResponse = Invoke-WebRequest -Uri "http://localhost:15068" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "  ✅ Aspire dashboard is accessible: HTTP $($dashboardResponse.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Aspire dashboard is not accessible: $_" -ForegroundColor Red
}

# Test OTLP metrics endpoint
try {
    $otlpResponse = Invoke-WebRequest -Uri "http://localhost:18889/v1/metrics" -TimeoutSec 5 -Method Post -ErrorAction Stop
    Write-Host "  ✅ OTLP metrics endpoint is responding: HTTP $($otlpResponse.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "  ❌ OTLP metrics endpoint not responding: $_" -ForegroundColor Red
}

# Test Web service endpoint
try {
    $webResponse = Invoke-WebRequest -Uri "http://localhost:5153" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "  ✅ Web service is accessible: HTTP $($webResponse.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Web service is not accessible: $_" -ForegroundColor Red
}

# Test API service endpoint
try {
    $apiResponse = Invoke-WebRequest -Uri "http://localhost:5153/api/health" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "  ✅ API service is accessible: HTTP $($apiResponse.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "  ❌ API service is not accessible: $_" -ForegroundColor Red
}

Write-Host "`nFix complete!" -ForegroundColor Cyan
Write-Host "If the Aspire dashboard is not showing telemetry data:" -ForegroundColor White
Write-Host "1. Open browser at: http://localhost:15068" -ForegroundColor White
Write-Host "2. Generate some traffic by visiting: http://localhost:5153" -ForegroundColor White
Write-Host "3. Check the 'Telemetry' tab in the Aspire dashboard" -ForegroundColor White
