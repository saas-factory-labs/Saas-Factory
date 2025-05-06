#!/usr/bin/env pwsh
# restart-apphost-with-otlp.ps1
# This script stops any running AppHost process and starts a new one with explicit OTLP variables

Write-Host "Restarting AppHost with Explicit OTLP Variables" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Gray

# Stop any running AppBlueprint processes
Write-Host "`nStopping all running AppBlueprint processes..." -ForegroundColor Magenta
$processes = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue
if ($processes) {
    foreach ($process in $processes) {
        Write-Host "  Stopping $($process.ProcessName) (PID: $($process.Id))" -ForegroundColor Yellow
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    }
    # Give processes time to fully stop
    Start-Sleep -Seconds 3
    Write-Host "  All AppBlueprint processes stopped" -ForegroundColor Green
} else {
    Write-Host "  No AppBlueprint processes found running" -ForegroundColor Yellow
}

# Define the OTLP variables
$otlpVariables = @{
    "OTEL_EXPORTER_OTLP_ENDPOINT" = "http://localhost:18889"
    "OTEL_EXPORTER_OTLP_PROTOCOL" = "http/protobuf"  
    "OTEL_EXPORTER_OTLP_HEADERS" = ""
    "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL" = "http://localhost:18889"
    "ASPIRE_DASHBOARD_PORT" = "15068"
}

# Start the AppHost process with environment variables using Start-Process
Write-Host "`nStarting AppHost with explicit environment variables..." -ForegroundColor Magenta

# First, create a temporary script that will set the variables and then start the AppHost
$tempScriptPath = Join-Path $env:TEMP "start-apphost-with-vars.ps1"
$scriptContent = @"
`$env:OTEL_EXPORTER_OTLP_ENDPOINT = 'http://localhost:18889'
`$env:OTEL_EXPORTER_OTLP_PROTOCOL = 'http/protobuf'
`$env:OTEL_EXPORTER_OTLP_HEADERS = ''
`$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = 'http://localhost:18889'
`$env:ASPIRE_DASHBOARD_PORT = '15068'

# Echo the variables for verification
Write-Host 'Starting AppHost with following variables:'
Write-Host "OTEL_EXPORTER_OTLP_ENDPOINT: `$env:OTEL_EXPORTER_OTLP_ENDPOINT"
Write-Host "OTEL_EXPORTER_OTLP_PROTOCOL: `$env:OTEL_EXPORTER_OTLP_PROTOCOL"
Write-Host "OTEL_EXPORTER_OTLP_HEADERS: '`$env:OTEL_EXPORTER_OTLP_HEADERS'"
Write-Host "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: `$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"
Write-Host "ASPIRE_DASHBOARD_PORT: `$env:ASPIRE_DASHBOARD_PORT"

# Start the AppHost
Set-Location '$PSScriptRoot'
dotnet run --project AppBlueprint.AppHost
"@

$scriptContent | Out-File -FilePath $tempScriptPath -Encoding utf8
Write-Host "  Created temporary startup script at $tempScriptPath" -ForegroundColor Gray

# Start new PowerShell process to run the script
Write-Host "  Starting PowerShell with AppHost script..." -ForegroundColor Yellow
$process = Start-Process -FilePath "pwsh.exe" -ArgumentList "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", $tempScriptPath -PassThru -NoNewWindow

if ($process) {
    Write-Host "  ✅ Started AppHost process with PID: $($process.Id)" -ForegroundColor Green
    
    # Wait for process to initialize
    Write-Host "  Waiting for AppHost to initialize (15 seconds)..." -ForegroundColor Yellow
    Start-Sleep -Seconds 15
    
    # Check if the process is still running
    try {
        $refreshedProcess = Get-Process -Id $process.Id -ErrorAction Stop
        Write-Host "  ✅ AppHost is still running (PID: $($refreshedProcess.Id))" -ForegroundColor Green
    } catch {
        Write-Host "  ❌ AppHost process has terminated unexpectedly!" -ForegroundColor Red
    }
} else {
    Write-Host "  ❌ Failed to start AppHost process!" -ForegroundColor Red
}

# Test Aspire dashboard
Write-Host "`nChecking Aspire dashboard..." -ForegroundColor Magenta
try {
    $response = Invoke-WebRequest -Uri "http://localhost:15068" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "  ✅ Aspire dashboard is accessible (Status: $($response.StatusCode))" -ForegroundColor Green
    
    # Try opening it in the default browser
    Write-Host "`nOpening Aspire dashboard in browser..." -ForegroundColor Magenta
    Start-Process "http://localhost:15068"
} catch {
    Write-Host "  ❌ Failed to access Aspire dashboard: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nRestart completed. The AppHost should now be running with the correct OTLP variables." -ForegroundColor Cyan
Write-Host "Access the Aspire dashboard at: http://localhost:15068" -ForegroundColor White
Write-Host "It may take a few minutes for telemetry data to appear in the dashboard." -ForegroundColor White
Write-Host "Check the console output of the new AppHost process for any OpenTelemetry related messages." -ForegroundColor White
