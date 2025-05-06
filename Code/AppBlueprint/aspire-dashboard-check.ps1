#!/usr/bin/env pwsh
# aspire-dashboard-check.ps1
# Simple script to check Aspire dashboard and OTLP

Write-Host "Checking Aspire Dashboard & OTLP" -ForegroundColor Cyan
Write-Host "===============================" -ForegroundColor Gray

$dashboardPort = $env:ASPIRE_DASHBOARD_PORT
if (-not $dashboardPort) {
    $dashboardPort = "15068"
}

$otlpEndpoint = $env:OTEL_EXPORTER_OTLP_ENDPOINT
if (-not $otlpEndpoint) {
    $otlpEndpoint = "http://localhost:18889"
}

Write-Host "`nConfiguration:" -ForegroundColor Yellow
Write-Host "Dashboard port: $dashboardPort" -ForegroundColor Gray
Write-Host "OTLP endpoint: $otlpEndpoint" -ForegroundColor Gray

Write-Host "`nChecking dashboard accessibility..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:$dashboardPort" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "✅ Dashboard is accessible (Status code: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "❌ Dashboard is not accessible: $_" -ForegroundColor Red
}

Write-Host "`nChecking OTLP endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$otlpEndpoint/v1/metrics" -TimeoutSec 2 -ErrorAction SilentlyContinue
    Write-Host "Unexpected success from OTLP endpoint (Status code: $($response.StatusCode))" -ForegroundColor Yellow
} catch {
    if ($_.Exception.Response.StatusCode.value__ -eq 400) {
        Write-Host "✅ OTLP endpoint is accessible (returned 400 Bad Request as expected)" -ForegroundColor Green
    } else {
        Write-Host "❌ OTLP endpoint is not accessible: $_" -ForegroundColor Red
    }
}

Write-Host "`nChecking running processes..." -ForegroundColor Yellow
$processes = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue
if ($processes) {
    Write-Host "Found running AppBlueprint processes:" -ForegroundColor Green
    foreach ($proc in $processes) {
        Write-Host "  • $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Gray
    }
} else {
    Write-Host "❌ No AppBlueprint processes found running" -ForegroundColor Red
}

Write-Host "`nIf everything looks good but telemetry is still not showing up:" -ForegroundColor Magenta
Write-Host "1. Give it a few minutes - telemetry data may take time to appear" -ForegroundColor White
Write-Host "2. Verify all services are properly configured with ServiceDefaults" -ForegroundColor White
Write-Host "3. Check OpenTelemetry configuration is consistent across all services" -ForegroundColor White
Write-Host "4. Try generating more traffic to the services to create more telemetry" -ForegroundColor White
