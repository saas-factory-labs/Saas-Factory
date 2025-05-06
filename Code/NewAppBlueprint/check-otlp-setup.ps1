#!/usr/bin/env pwsh
# check-otlp-setup.ps1
# This script tests if the Aspire OpenTelemetry OTLP endpoint is properly configured for NewAppBlueprint

Write-Host "NewAppBlueprint OpenTelemetry Configuration Check" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Gray

# Check env vars
Write-Host "`nChecking environment variables:" -ForegroundColor Magenta
$envVars = @(
    @{Name = "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"; Value = [System.Environment]::GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL")},
    @{Name = "OTEL_EXPORTER_OTLP_ENDPOINT"; Value = [System.Environment]::GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")},
    @{Name = "OTEL_EXPORTER_OTLP_PROTOCOL"; Value = [System.Environment]::GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL")},
    @{Name = "ASPIRE_DASHBOARD_PORT"; Value = [System.Environment]::GetEnvironmentVariable("ASPIRE_DASHBOARD_PORT")}
)

$needsFixing = $false
foreach ($var in $envVars) {
    if ($var.Value) {
        Write-Host "✅ $($var.Name): $($var.Value)" -ForegroundColor Green
    } else {
        Write-Host "❌ $($var.Name): Not set" -ForegroundColor Red
        $needsFixing = $true
    }
}

# Check if we need to set up the environment variables
if ($needsFixing) {
    Write-Host "`nEnvironment variables are missing. Would you like to set them now? (Y/N)" -ForegroundColor Yellow
    $response = Read-Host
    if ($response -eq "Y" -or $response -eq "y") {
        # Set up environment variables for the Aspire OTLP receiver
        $aspireOtlpEndpoint = "http://localhost:18889"
        $dashboardPort = "15068"

        Write-Host "Setting ASPIRE_DASHBOARD_PORT to $dashboardPort" -ForegroundColor Green
        [System.Environment]::SetEnvironmentVariable("ASPIRE_DASHBOARD_PORT", $dashboardPort, "User")
        $env:ASPIRE_DASHBOARD_PORT = $dashboardPort

        Write-Host "Setting DOTNET_DASHBOARD_OTLP_ENDPOINT_URL to $aspireOtlpEndpoint" -ForegroundColor Green
        [System.Environment]::SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", $aspireOtlpEndpoint, "User")
        $env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = $aspireOtlpEndpoint

        Write-Host "Setting OTEL_EXPORTER_OTLP_ENDPOINT to $aspireOtlpEndpoint" -ForegroundColor Green
        [System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $aspireOtlpEndpoint, "User")
        $env:OTEL_EXPORTER_OTLP_ENDPOINT = $aspireOtlpEndpoint

        Write-Host "Setting OTEL_EXPORTER_OTLP_PROTOCOL to http/protobuf" -ForegroundColor Green
        [System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf", "User")
        $env:OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"

        Write-Host "`nEnvironment variables set!" -ForegroundColor Cyan
    }
}

# Check ServiceDefaults extension
Write-Host "`nChecking OpenTelemetry config in ServiceDefaults:" -ForegroundColor Magenta
$serviceDefaultsPath = Join-Path $PSScriptRoot "AppBlueprint.ServiceDefaults/Extensions.cs"

if (Test-Path $serviceDefaultsPath) {
    $extensionsContent = Get-Content $serviceDefaultsPath -Raw
    
    if ($extensionsContent -match "ConfigureDefaultOpenTelemetrySettings") {
        Write-Host "✅ ConfigureDefaultOpenTelemetrySettings method found" -ForegroundColor Green
    } else {
        Write-Host "❌ ConfigureDefaultOpenTelemetrySettings method not found" -ForegroundColor Red
    }
    
    # Check for AddOpenTelemetryExporters method with endpoint URI
    if ($extensionsContent -match "new Uri\(endpoint\)") {
        Write-Host "✅ AddOpenTelemetryExporters method configures endpoint URI correctly" -ForegroundColor Green
    } else {
        Write-Host "❌ AddOpenTelemetryExporters method doesn't seem to configure endpoint URI correctly" -ForegroundColor Red
    }
    
    # Check for HTTP/2 unencrypted support
    if ($extensionsContent -match "Http2UnencryptedSupport") {
        Write-Host "✅ HTTP/2 unencrypted support enabled for localhost" -ForegroundColor Green
    } else {
        Write-Host "❌ HTTP/2 unencrypted support not found" -ForegroundColor Red
    }
} else {
    Write-Host "❌ ServiceDefaults Extensions.cs file not found at: $serviceDefaultsPath" -ForegroundColor Red
}

# Check if the application builds
Write-Host "`nAttempting to build the application:" -ForegroundColor Magenta
$buildOutput = dotnet build
$buildSuccess = $LASTEXITCODE -eq 0

if ($buildSuccess) {
    Write-Host "✅ Application builds successfully" -ForegroundColor Green
} else {
    Write-Host "❌ Application build failed. Please check the errors above." -ForegroundColor Red
}

Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Make sure to fix any issues identified above"
Write-Host "2. Run the AppHost project: dotnet run --project AppBlueprint.AppHost"
Write-Host "3. Check if telemetry data appears in the Aspire dashboard"
