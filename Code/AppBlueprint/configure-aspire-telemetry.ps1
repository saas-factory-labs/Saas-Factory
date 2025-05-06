#!/usr/bin/env pwsh
# configure-aspire-telemetry.ps1
# A unified script to configure telemetry for Aspire applications

# Set standard variables
$otlpEndpoint = "http://localhost:18889"
$dashboardPort = "15068"
$otlpProtocol = "http/protobuf"

Write-Host "Configuring Aspire Telemetry" -ForegroundColor Cyan
Write-Host "=============================" -ForegroundColor Gray

# 1. Set environment variables at the process level
Write-Host "`nSetting telemetry environment variables for current process..." -ForegroundColor Magenta
$env:OTEL_EXPORTER_OTLP_ENDPOINT = $otlpEndpoint
$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = $otlpEndpoint
$env:ASPIRE_DASHBOARD_PORT = $dashboardPort
$env:OTEL_EXPORTER_OTLP_PROTOCOL = $otlpProtocol
$env:OTEL_EXPORTER_OTLP_HEADERS = ""
$env:ASPIRE_ALLOW_UNSECURED_TRANSPORT = "true"

Write-Host "  ✅ Process environment variables set" -ForegroundColor Green
Write-Host "    OTEL_EXPORTER_OTLP_ENDPOINT: $otlpEndpoint" -ForegroundColor Gray
Write-Host "    DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $otlpEndpoint" -ForegroundColor Gray
Write-Host "    ASPIRE_DASHBOARD_PORT: $dashboardPort" -ForegroundColor Gray
Write-Host "    OTEL_EXPORTER_OTLP_PROTOCOL: $otlpProtocol" -ForegroundColor Gray
Write-Host "    OTEL_EXPORTER_OTLP_HEADERS: (empty)" -ForegroundColor Gray
Write-Host "    ASPIRE_ALLOW_UNSECURED_TRANSPORT: true" -ForegroundColor Gray

# 2. Set environment variables at the user level (persists across sessions)
Write-Host "`nSetting telemetry environment variables at user level..." -ForegroundColor Magenta
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $otlpEndpoint, "User")
[System.Environment]::SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", $otlpEndpoint, "User")
[System.Environment]::SetEnvironmentVariable("ASPIRE_DASHBOARD_PORT", $dashboardPort, "User")
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", $otlpProtocol, "User")
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "", "User")
[System.Environment]::SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true", "User")
Write-Host "  ✅ User environment variables set" -ForegroundColor Green

# 3. Stop any running services
Write-Host "`nStopping any running AppBlueprint services..." -ForegroundColor Magenta
$appProcesses = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue

if ($appProcesses) {
    foreach ($proc in $appProcesses) {
        Write-Host "  Stopping $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Yellow
        Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    }
    Write-Host "  ✅ All AppBlueprint processes stopped" -ForegroundColor Green
    # Give processes time to fully stop
    Start-Sleep -Seconds 2
} else {
    Write-Host "  No AppBlueprint processes running" -ForegroundColor Yellow
}

# 4. Check if ports are available
Write-Host "`nChecking if required ports are available..." -ForegroundColor Magenta
$dashboardPortInUse = Get-NetTCPConnection -LocalPort $dashboardPort -ErrorAction SilentlyContinue
$otlpPortInUse = Get-NetTCPConnection -LocalPort 18889 -ErrorAction SilentlyContinue

if ($dashboardPortInUse) {
    Write-Host "  ⚠️ Warning: Dashboard port $dashboardPort is already in use" -ForegroundColor Yellow
    Write-Host "     Process ID: $($dashboardPortInUse.OwningProcess)" -ForegroundColor Yellow
    $processName = (Get-Process -Id $dashboardPortInUse.OwningProcess -ErrorAction SilentlyContinue).ProcessName
    if ($processName) {
        Write-Host "     Process Name: $processName" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ✅ Dashboard port $dashboardPort is available" -ForegroundColor Green
}

if ($otlpPortInUse) {
    Write-Host "  ⚠️ Warning: OTLP port 18889 is already in use" -ForegroundColor Yellow
    Write-Host "     Process ID: $($otlpPortInUse.OwningProcess)" -ForegroundColor Yellow
    $processName = (Get-Process -Id $otlpPortInUse.OwningProcess -ErrorAction SilentlyContinue).ProcessName
    if ($processName) {
        Write-Host "     Process Name: $processName" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ✅ OTLP port 18889 is available" -ForegroundColor Green
}

# 5. Build the solution
Write-Host "`nBuilding the solution..." -ForegroundColor Magenta
try {
    Push-Location "C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"
    dotnet build
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✅ Solution built successfully" -ForegroundColor Green
    } else {
        Write-Host "  ❌ Error building solution (Exit code: $LASTEXITCODE)" -ForegroundColor Red
    }
} catch {
    Write-Host "  ❌ Error building solution: $_" -ForegroundColor Red
} finally {
    Pop-Location
}

# 6. Start the AppHost with proper environment variables
Write-Host "`nStarting AppHost with proper environment variables..." -ForegroundColor Magenta
try {
    # Create a temporary script to run the AppHost with the correct environment variables
    $tempScript = [System.IO.Path]::Combine([System.IO.Path]::GetTempPath(), "start-apphost-with-vars.ps1")
    
    @"
Write-Host "Starting AppHost with following variables:"
Write-Host "OTEL_EXPORTER_OTLP_ENDPOINT: $otlpEndpoint"
Write-Host "OTEL_EXPORTER_OTLP_PROTOCOL: $otlpProtocol"
Write-Host "OTEL_EXPORTER_OTLP_HEADERS: ''"
Write-Host "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $otlpEndpoint"
Write-Host "ASPIRE_DASHBOARD_PORT: $dashboardPort"

cd C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint
dotnet run --project AppBlueprint.AppHost
"@ | Out-File -FilePath $tempScript -Encoding utf8

    Write-Host "  Created temporary startup script at $tempScript" -ForegroundColor Gray
    Write-Host "  Starting PowerShell with AppHost script..." -ForegroundColor Gray
    
    # Start PowerShell with the script, ensuring environment variables are set
    $startInfo = New-Object System.Diagnostics.ProcessStartInfo
    $startInfo.FileName = "pwsh.exe"
    $startInfo.Arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$tempScript`""
    $startInfo.UseShellExecute = $false
    
    # Set environment variables for the new process
    $startInfo.EnvironmentVariables["OTEL_EXPORTER_OTLP_ENDPOINT"] = $otlpEndpoint
    $startInfo.EnvironmentVariables["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"] = $otlpEndpoint
    $startInfo.EnvironmentVariables["ASPIRE_DASHBOARD_PORT"] = $dashboardPort
    $startInfo.EnvironmentVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] = $otlpProtocol
    $startInfo.EnvironmentVariables["OTEL_EXPORTER_OTLP_HEADERS"] = ""
    $startInfo.EnvironmentVariables["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true"
    
    $process = [System.Diagnostics.Process]::Start($startInfo)
    Write-Host "  ✅ Started AppHost process with PID: $($process.Id)" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Error starting AppHost: $_" -ForegroundColor Red
}

Write-Host "`nSetup complete!" -ForegroundColor Cyan
Write-Host "The Aspire dashboard should be available at: http://localhost:$dashboardPort" -ForegroundColor White
Write-Host "The OTLP endpoint is configured at: $otlpEndpoint" -ForegroundColor White
Write-Host "`nIf you don't see telemetry data in the dashboard, please wait a few minutes for data to be collected." -ForegroundColor White
Write-Host "You can also generate traffic by accessing the application endpoints." -ForegroundColor White
