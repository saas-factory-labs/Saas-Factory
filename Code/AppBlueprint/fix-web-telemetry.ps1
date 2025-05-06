#!/usr/bin/env pwsh
# fix-web-telemetry.ps1
# This script fixes the hardcoded OTLP endpoint in the AppBlueprint.Web project

Write-Host "Fixing hardcoded OTLP endpoint in AppBlueprint.Web" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Gray

# 1. Change the hardcoded endpoint in the Web project
$webProgramFile = "C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint\AppBlueprint.Web\Program.cs"

if (Test-Path $webProgramFile) {
    Write-Host "`nFixing OTLP endpoint in $webProgramFile" -ForegroundColor Magenta
    
    # Read the current content
    $content = Get-Content $webProgramFile -Raw
    
    # Replace the hardcoded endpoint with the correct one for Aspire
    $updatedContent = $content -replace "Environment.SetEnvironmentVariable\(""OTEL_EXPORTER_OTLP_ENDPOINT"", ""http://localhost:4318""\);", 
        '// Get the OTLP endpoint from environment or use Aspire default
string? dashboardEndpoint = Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");
Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", 
    !string.IsNullOrEmpty(dashboardEndpoint) ? dashboardEndpoint : "http://localhost:18889");'
    
    # Save the updated content
    $updatedContent | Set-Content $webProgramFile
    
    Write-Host "  ✅ Updated hardcoded OTLP endpoint in $webProgramFile" -ForegroundColor Green
} else {
    Write-Host "  ❌ Web Program.cs file not found at: $webProgramFile" -ForegroundColor Red
}

# 2. Make sure the environment variables are set correctly
Write-Host "`nUpdating environment variables" -ForegroundColor Magenta
$aspireOtlpEndpoint = "http://localhost:18889"
$dashboardPort = "15068"

# Set the environment variables at User level
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $aspireOtlpEndpoint, "User")
[System.Environment]::SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", $aspireOtlpEndpoint, "User")
[System.Environment]::SetEnvironmentVariable("ASPIRE_DASHBOARD_PORT", $dashboardPort, "User")
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf", "User")
[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "", "User")

# Also set for current process
$env:OTEL_EXPORTER_OTLP_ENDPOINT = $aspireOtlpEndpoint
$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = $aspireOtlpEndpoint
$env:ASPIRE_DASHBOARD_PORT = $dashboardPort
$env:OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
$env:OTEL_EXPORTER_OTLP_HEADERS = ""

Write-Host "  ✅ Environment variables updated" -ForegroundColor Green

# 3. Stop any running services
Write-Host "`nStopping any running AppBlueprint services" -ForegroundColor Magenta
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

# 4. Rebuild the project
Write-Host "`nRebuilding the project" -ForegroundColor Magenta
try {
    Push-Location "C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"
    dotnet build
    Write-Host "  ✅ Project rebuilt successfully" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Error rebuilding project: $_" -ForegroundColor Red
} finally {
    Pop-Location
}

Write-Host "`nFix complete!" -ForegroundColor Cyan
Write-Host "Next steps:" -ForegroundColor White
Write-Host "1. Start the AppHost: dotnet run --project AppBlueprint.AppHost" -ForegroundColor White
Write-Host "2. Test the telemetry: .\full-aspire-test.ps1" -ForegroundColor White
Write-Host "3. Check the Aspire dashboard at: http://localhost:15068" -ForegroundColor White
