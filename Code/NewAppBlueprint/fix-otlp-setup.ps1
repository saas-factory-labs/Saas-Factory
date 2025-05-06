#!/usr/bin/env pwsh
# fix-otlp-setup.ps1
# This script fixes OpenTelemetry configuration for NewAppBlueprint

Write-Host "NewAppBlueprint OpenTelemetry Fix Tool" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Gray

# Step 1: Set environment variables
Write-Host "`nStep 1: Setting environment variables..." -ForegroundColor Magenta
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

Write-Host "Setting ASPIRE_ALLOW_UNSECURED_TRANSPORT to true" -ForegroundColor Green
[System.Environment]::SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true", "User")
$env:ASPIRE_ALLOW_UNSECURED_TRANSPORT = "true"

# Step 2: Check if AppHost/Program.cs has correct OTLP configuration
Write-Host "`nStep 2: Checking AppHost/Program.cs..." -ForegroundColor Magenta
$appHostPath = Join-Path $PSScriptRoot "AppBlueprint.AppHost/Program.cs"

if (Test-Path $appHostPath) {
    $appHostContent = Get-Content $appHostPath -Raw
    
    if ($appHostContent -match "ConfigureOpenTelemetry") {
        Write-Host "✅ ConfigureOpenTelemetry method found in AppHost" -ForegroundColor Green
    } else {
        Write-Host "❌ ConfigureOpenTelemetry method not found in AppHost. Please add it manually." -ForegroundColor Red
        Write-Host "Add a private static method like this to AppHost/Program.cs:" -ForegroundColor Yellow
        Write-Host @"
    private static void ConfigureOpenTelemetry()
    {
        // Set environment variables for OpenTelemetry
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
        Environment.SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");
        
        // Check for Aspire dashboard OTLP endpoint first
        string? dashboardEndpoint = Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");
        
        // If dashboard endpoint is set, use it for OTEL exporter
        if (!string.IsNullOrEmpty(dashboardEndpoint))
        {
            // Ensure we're using http:// for local connections
            if (dashboardEndpoint.Contains("localhost", StringComparison.OrdinalIgnoreCase) && 
                dashboardEndpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                dashboardEndpoint = dashboardEndpoint.Replace("https://", "http://", StringComparison.OrdinalIgnoreCase);
                Console.WriteLine($"Converted HTTPS to HTTP for localhost OTLP endpoint: {dashboardEndpoint}");
            }
            
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", dashboardEndpoint);
            Console.WriteLine($"Using Aspire dashboard OTLP endpoint: {dashboardEndpoint}");
        }
        else
        {
            // Default OTLP endpoint for Aspire dashboard collector
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889");
            Console.WriteLine($"Using default OTLP endpoint: http://localhost:18889");
        }
    }
"@
    }
    
    if ($appHostContent -match "WithTelemetryDefaults") {
        Write-Host "✅ WithTelemetryDefaults method calls found in AppHost" -ForegroundColor Green
    } else {
        Write-Host "❌ WithTelemetryDefaults method calls not found in AppHost. Please add them manually." -ForegroundColor Red
        Write-Host "When adding resources, include telemetry like this:" -ForegroundColor Yellow
        Write-Host @"
    var apiService = builder.AddProject<Projects.AppBlueprint_ApiService>("apiservice")
        .WithReference(postgres)
        .WithTelemetryDefaults("AppBlueprint-Api");
"@
    }
} else {
    Write-Host "❌ AppHost/Program.cs not found at: $appHostPath" -ForegroundColor Red
}

# Step 3: Check if the solution builds
Write-Host "`nStep 3: Building solution..." -ForegroundColor Magenta
$buildOutput = dotnet build
$buildSuccess = $LASTEXITCODE -eq 0

if ($buildSuccess) {
    Write-Host "✅ Application builds successfully" -ForegroundColor Green
} else {
    Write-Host "❌ Application build failed. Please check the errors above." -ForegroundColor Red
}

Write-Host "`nFix complete. To test OTLP configuration:" -ForegroundColor Cyan
Write-Host "1. Make sure to fix any issues identified above"
Write-Host "2. Run the AppHost project: dotnet run --project AppBlueprint.AppHost"
Write-Host "3. Check if telemetry data appears in the Aspire dashboard"
Write-Host "4. Use .\check-otlp-setup.ps1 to verify your configuration"
