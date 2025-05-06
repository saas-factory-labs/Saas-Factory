#!/usr/bin/env pwsh
# validate-telemetry-config.ps1
# Validates that telemetry is properly configured across all services

Write-Host "Validating Telemetry Configuration" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Gray

# 1. Check environment variables
Write-Host "`nChecking environment variables:" -ForegroundColor Magenta
$otlpEndpoint = $env:OTEL_EXPORTER_OTLP_ENDPOINT
$dashboardEndpoint = $env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL
$dashboardPort = $env:ASPIRE_DASHBOARD_PORT
$otlpProtocol = $env:OTEL_EXPORTER_OTLP_PROTOCOL
$otlpHeaders = $env:OTEL_EXPORTER_OTLP_HEADERS
$allowUnsecured = $env:ASPIRE_ALLOW_UNSECURED_TRANSPORT

Write-Host "  OTEL_EXPORTER_OTLP_ENDPOINT: $otlpEndpoint"
Write-Host "  DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $dashboardEndpoint"
Write-Host "  ASPIRE_DASHBOARD_PORT: $dashboardPort"
Write-Host "  OTEL_EXPORTER_OTLP_PROTOCOL: $otlpProtocol"
Write-Host "  OTEL_EXPORTER_OTLP_HEADERS: $otlpHeaders"
Write-Host "  ASPIRE_ALLOW_UNSECURED_TRANSPORT: $allowUnsecured"

if ([string]::IsNullOrEmpty($otlpEndpoint)) {
    Write-Host "  ❌ OTEL_EXPORTER_OTLP_ENDPOINT is not set" -ForegroundColor Red
} else {
    Write-Host "  ✅ OTEL_EXPORTER_OTLP_ENDPOINT is set" -ForegroundColor Green
}

if ([string]::IsNullOrEmpty($dashboardEndpoint)) {
    Write-Host "  ❌ DOTNET_DASHBOARD_OTLP_ENDPOINT_URL is not set" -ForegroundColor Red
} else {
    Write-Host "  ✅ DOTNET_DASHBOARD_OTLP_ENDPOINT_URL is set" -ForegroundColor Green
}

if ([string]::IsNullOrEmpty($dashboardPort)) {
    Write-Host "  ⚠️ ASPIRE_DASHBOARD_PORT is not set, will use default 15068" -ForegroundColor Yellow
} else {
    Write-Host "  ✅ ASPIRE_DASHBOARD_PORT is set" -ForegroundColor Green
}

if ([string]::IsNullOrEmpty($otlpProtocol)) {
    Write-Host "  ❌ OTEL_EXPORTER_OTLP_PROTOCOL is not set" -ForegroundColor Red
} else {
    Write-Host "  ✅ OTEL_EXPORTER_OTLP_PROTOCOL is set" -ForegroundColor Green
}

# 2. Test dashboard connectivity
Write-Host "`nTesting Aspire dashboard connectivity:" -ForegroundColor Magenta
$dashboardPort = if ([string]::IsNullOrEmpty($dashboardPort)) { "15068" } else { $dashboardPort }
$dashboardUrl = "http://localhost:$dashboardPort"

try {
    $dashboardResponse = Invoke-WebRequest -Uri $dashboardUrl -Method Head -TimeoutSec 5 -ErrorAction SilentlyContinue
    Write-Host "  ✅ Dashboard is accessible at $dashboardUrl (Status: $($dashboardResponse.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Dashboard is not accessible at $dashboardUrl" -ForegroundColor Red
    Write-Host "     Error: $($_.Exception.Message)" -ForegroundColor Red
}

# 3. Test OTLP endpoint connectivity
Write-Host "`nTesting OTLP endpoint connectivity:" -ForegroundColor Magenta
$otlpUrl = if ([string]::IsNullOrEmpty($otlpEndpoint)) { "http://localhost:18889" } else { $otlpEndpoint }

try {
    $otlpResponse = Invoke-WebRequest -Uri "$otlpUrl/v1/metrics" -Method Head -TimeoutSec 5 -ErrorAction SilentlyContinue
    Write-Host "  ✅ OTLP endpoint is accessible at $otlpUrl (Status: $($otlpResponse.StatusCode))" -ForegroundColor Green
} catch [System.Net.WebException] {
    # A 400 Bad Request is actually expected, as we're just testing connectivity
    if ($_.Exception.Response -and $_.Exception.Response.StatusCode -eq 400) {
        Write-Host "  ✅ OTLP endpoint is accessible at $otlpUrl (Status: 400 Bad Request - expected)" -ForegroundColor Green
    } else {
        Write-Host "  ❌ OTLP endpoint is not accessible at $otlpUrl" -ForegroundColor Red
        Write-Host "     Error: $($_.Exception.Message)" -ForegroundColor Red
    }
} catch {
    Write-Host "  ❌ OTLP endpoint is not accessible at $otlpUrl" -ForegroundColor Red
    Write-Host "     Error: $($_.Exception.Message)" -ForegroundColor Red
}

# 4. Check running processes
Write-Host "`nChecking running AppBlueprint processes:" -ForegroundColor Magenta
$appProcesses = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue

if ($appProcesses) {
    Write-Host "  Found $($appProcesses.Count) AppBlueprint processes running:"
    foreach ($proc in $appProcesses) {
        Write-Host "  - $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Green
    }
} else {
    Write-Host "  ❌ No AppBlueprint processes are running" -ForegroundColor Red
}

# 5. Check port usage
Write-Host "`nChecking port usage:" -ForegroundColor Magenta
$dashboardPortInUse = Get-NetTCPConnection -LocalPort $dashboardPort -ErrorAction SilentlyContinue
$otlpPort = [Uri]$otlpUrl | Select-Object -ExpandProperty Port
$otlpPortInUse = Get-NetTCPConnection -LocalPort $otlpPort -ErrorAction SilentlyContinue

if ($dashboardPortInUse) {
    $processId = $dashboardPortInUse | Select-Object -First 1 -ExpandProperty OwningProcess
    $processName = (Get-Process -Id $processId -ErrorAction SilentlyContinue).ProcessName
    Write-Host "  ✅ Dashboard port $dashboardPort is in use by process $processName (PID: $processId)" -ForegroundColor Green
} else {
    Write-Host "  ❌ Dashboard port $dashboardPort is not in use" -ForegroundColor Red
}

if ($otlpPortInUse) {
    $processId = $otlpPortInUse | Select-Object -First 1 -ExpandProperty OwningProcess
    $processName = (Get-Process -Id $processId -ErrorAction SilentlyContinue).ProcessName
    Write-Host "  ✅ OTLP port $otlpPort is in use by process $processName (PID: $processId)" -ForegroundColor Green
} else {
    Write-Host "  ❌ OTLP port $otlpPort is not in use" -ForegroundColor Red
}

Write-Host "`nValidation complete!" -ForegroundColor Cyan
Write-Host "`nNext steps:" -ForegroundColor White
Write-Host "1. If validation failed, run './configure-aspire-telemetry.ps1' to configure telemetry" -ForegroundColor White
Write-Host "2. Access the Aspire dashboard at: http://localhost:$dashboardPort" -ForegroundColor White
Write-Host "3. Check the Resources tab in the dashboard to see if telemetry is being collected" -ForegroundColor White
