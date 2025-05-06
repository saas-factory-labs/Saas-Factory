#!/usr/bin/env pwsh
# fix-all-telemetry-issues.ps1
# This script fixes all telemetry issues in the AppBlueprint project

Write-Host "Advanced OpenTelemetry Troubleshooter" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Gray

# Step 1: Stop all AppBlueprint processes
Write-Host "`nStep 1: Stopping all AppBlueprint processes..." -ForegroundColor Magenta
$processes = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue
if ($processes) {
    $processes | ForEach-Object {
        Write-Host "  Stopping $($_.ProcessName) (PID: $($_.Id))" -ForegroundColor Yellow
        $_ | Stop-Process -Force
    }
    Write-Host "  All AppBlueprint processes stopped." -ForegroundColor Green
    # Give processes time to fully stop
    Start-Sleep -Seconds 2
} else {
    Write-Host "  No AppBlueprint processes running." -ForegroundColor Yellow
}

# Step 2: Clean Environment Variables
Write-Host "`nStep 2: Cleaning environment variables..." -ForegroundColor Magenta

# First, clear all OTLP-related environment variables at user level
$envVarsToRemove = @(
    "OTEL_EXPORTER_OTLP_ENDPOINT",
    "OTEL_EXPORTER_OTLP_HEADERS",
    "OTEL_EXPORTER_OTLP_PROTOCOL",
    "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL",
    "ASPIRE_DASHBOARD_PORT"
)

foreach ($var in $envVarsToRemove) {
    [System.Environment]::SetEnvironmentVariable($var, $null, "User")
    Write-Host "  Cleared $var from User environment" -ForegroundColor Gray
    $env:$var = $null
    Write-Host "  Cleared $var from Process environment" -ForegroundColor Gray
}

# Step 3: Set correct environment variables for local Aspire collection
Write-Host "`nStep 3: Setting correct environment variables..." -ForegroundColor Magenta

$aspireOtlpEndpoint = "http://localhost:18889"
$dashboardPort = "15068"

# Set user environment variables
[System.Environment]::SetEnvironmentVariable("ASPIRE_DASHBOARD_PORT", $dashboardPort, "User")
Write-Host "  Set ASPIRE_DASHBOARD_PORT to $dashboardPort (User level)" -ForegroundColor Green

[System.Environment]::SetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", $aspireOtlpEndpoint, "User")
Write-Host "  Set DOTNET_DASHBOARD_OTLP_ENDPOINT_URL to $aspireOtlpEndpoint (User level)" -ForegroundColor Green

[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", $aspireOtlpEndpoint, "User")
Write-Host "  Set OTEL_EXPORTER_OTLP_ENDPOINT to $aspireOtlpEndpoint (User level)" -ForegroundColor Green

[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf", "User")
Write-Host "  Set OTEL_EXPORTER_OTLP_PROTOCOL to http/protobuf (User level)" -ForegroundColor Green

[System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "", "User")
Write-Host "  Set OTEL_EXPORTER_OTLP_HEADERS to empty string (User level)" -ForegroundColor Green

# Set process environment variables (for current session)
$env:ASPIRE_DASHBOARD_PORT = $dashboardPort
$env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = $aspireOtlpEndpoint
$env:OTEL_EXPORTER_OTLP_ENDPOINT = $aspireOtlpEndpoint
$env:OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
$env:OTEL_EXPORTER_OTLP_HEADERS = ""

# Step 4: Fix launch settings for all projects
Write-Host "`nStep 4: Fixing launchSettings.json files..." -ForegroundColor Magenta

# AppGateway launchSettings.json
$appGatewayLaunchSettingsPath = Join-Path $PSScriptRoot "AppBlueprint.AppGateway" "Properties" "launchSettings.json"
if (Test-Path $appGatewayLaunchSettingsPath) {
    try {
        $json = Get-Content $appGatewayLaunchSettingsPath -Raw | ConvertFrom-Json
        
        # HTTP profile
        if ($json.profiles.http) {
            $json.profiles.http.environmentVariables.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = "http://localhost:18889"
            $json.profiles.http.environmentVariables.OTEL_EXPORTER_OTLP_ENDPOINT = "http://localhost:18889"
            $json.profiles.http.environmentVariables.OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
            $json.profiles.http.environmentVariables.OTEL_EXPORTER_OTLP_HEADERS = ""
        }
        
        # HTTPS profile
        if ($json.profiles.https) {
            $json.profiles.https.environmentVariables.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = "https://localhost:18889"
            $json.profiles.https.environmentVariables.OTEL_EXPORTER_OTLP_ENDPOINT = "https://localhost:18889"
            $json.profiles.https.environmentVariables.OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
            $json.profiles.https.environmentVariables.OTEL_EXPORTER_OTLP_HEADERS = ""
        }
        
        # Save changes
        $json | ConvertTo-Json -Depth 10 | Set-Content $appGatewayLaunchSettingsPath
        Write-Host "  Fixed AppGateway launchSettings.json" -ForegroundColor Green
    } catch {
        Write-Host "  Error updating AppGateway launchSettings.json: $_" -ForegroundColor Red
    }
} else {
    Write-Host "  AppGateway launchSettings.json not found: $appGatewayLaunchSettingsPath" -ForegroundColor Red
}

# ApiService launchSettings.json
$apiServiceLaunchSettingsPath = Join-Path $PSScriptRoot "AppBlueprint.ApiService" "Properties" "launchSettings.json"
if (Test-Path $apiServiceLaunchSettingsPath) {
    try {
        $json = Get-Content $apiServiceLaunchSettingsPath -Raw | ConvertFrom-Json
        
        # HTTP profile
        if ($json.profiles.http) {
            $json.profiles.http.environmentVariables.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = "http://localhost:18889"
            $json.profiles.http.environmentVariables.OTEL_EXPORTER_OTLP_ENDPOINT = "http://localhost:18889"
            $json.profiles.http.environmentVariables.OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
            $json.profiles.http.environmentVariables.OTEL_EXPORTER_OTLP_HEADERS = ""
        }
        
        # HTTPS profile
        if ($json.profiles.https) {
            $json.profiles.https.environmentVariables.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = "https://localhost:18889"
            $json.profiles.https.environmentVariables.OTEL_EXPORTER_OTLP_ENDPOINT = "https://localhost:18889"
            $json.profiles.https.environmentVariables.OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
            $json.profiles.https.environmentVariables.OTEL_EXPORTER_OTLP_HEADERS = ""
        }
        
        # Save changes
        $json | ConvertTo-Json -Depth 10 | Set-Content $apiServiceLaunchSettingsPath
        Write-Host "  Fixed ApiService launchSettings.json" -ForegroundColor Green
    } catch {
        Write-Host "  Error updating ApiService launchSettings.json: $_" -ForegroundColor Red
    }
} else {
    Write-Host "  ApiService launchSettings.json not found: $apiServiceLaunchSettingsPath" -ForegroundColor Red
}

# Web launchSettings.json
$webLaunchSettingsPath = Join-Path $PSScriptRoot "AppBlueprint.Web" "Properties" "launchSettings.json"
if (Test-Path $webLaunchSettingsPath) {
    try {
        $json = Get-Content $webLaunchSettingsPath -Raw | ConvertFrom-Json
        
        # HTTP profile
        if ($json.profiles.http) {
            $json.profiles.http.environmentVariables.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = "http://localhost:18889"
            $json.profiles.http.environmentVariables.OTEL_EXPORTER_OTLP_ENDPOINT = "http://localhost:18889"
            $json.profiles.http.environmentVariables.OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
            $json.profiles.http.environmentVariables.OTEL_EXPORTER_OTLP_HEADERS = ""
        }
        
        # HTTPS profile
        if ($json.profiles.https) {
            $json.profiles.https.environmentVariables.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = "https://localhost:18889"
            $json.profiles.https.environmentVariables.OTEL_EXPORTER_OTLP_ENDPOINT = "https://localhost:18889"
            $json.profiles.https.environmentVariables.OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
            $json.profiles.https.environmentVariables.OTEL_EXPORTER_OTLP_HEADERS = ""
        }
        
        # Save changes
        $json | ConvertTo-Json -Depth 10 | Set-Content $webLaunchSettingsPath
        Write-Host "  Fixed Web launchSettings.json" -ForegroundColor Green
    } catch {
        Write-Host "  Error updating Web launchSettings.json: $_" -ForegroundColor Red
    }
} else {
    Write-Host "  Web launchSettings.json not found: $webLaunchSettingsPath" -ForegroundColor Red
}

# AppHost launchSettings.json
$appHostLaunchSettingsPath = Join-Path $PSScriptRoot "AppBlueprint.AppHost" "Properties" "launchSettings.json"
if (Test-Path $appHostLaunchSettingsPath) {
    try {
        $json = Get-Content $appHostLaunchSettingsPath -Raw | ConvertFrom-Json
        
        # HTTP profile
        if ($json.profiles.http) {
            $json.profiles.http.environmentVariables.ASPIRE_DASHBOARD_PORT = $dashboardPort
            $json.profiles.http.environmentVariables.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = "http://localhost:18889"
            $json.profiles.http.environmentVariables.OTEL_EXPORTER_OTLP_ENDPOINT = "http://localhost:18889"
            $json.profiles.http.environmentVariables.OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
            # Add OTEL_SERVICE_NAME if it doesn't exist
            if (-not $json.profiles.http.environmentVariables.OTEL_SERVICE_NAME) {
                $json.profiles.http.environmentVariables | Add-Member -MemberType NoteProperty -Name "OTEL_SERVICE_NAME" -Value "appblueprint"
            }
        }
        
        # HTTPS profile
        if ($json.profiles.https) {
            $json.profiles.https.environmentVariables.ASPIRE_DASHBOARD_PORT = $dashboardPort
            $json.profiles.https.environmentVariables.DOTNET_DASHBOARD_OTLP_ENDPOINT_URL = "https://localhost:18889"
            $json.profiles.https.environmentVariables.OTEL_EXPORTER_OTLP_ENDPOINT = "https://localhost:18889"
            $json.profiles.https.environmentVariables.OTEL_EXPORTER_OTLP_PROTOCOL = "http/protobuf"
            # Add OTEL_SERVICE_NAME if it doesn't exist
            if (-not $json.profiles.https.environmentVariables.OTEL_SERVICE_NAME) {
                $json.profiles.https.environmentVariables | Add-Member -MemberType NoteProperty -Name "OTEL_SERVICE_NAME" -Value "appblueprint"
            }
        }
        
        # Save changes
        $json | ConvertTo-Json -Depth 10 | Set-Content $appHostLaunchSettingsPath
        Write-Host "  Fixed AppHost launchSettings.json" -ForegroundColor Green
    } catch {
        Write-Host "  Error updating AppHost launchSettings.json: $_" -ForegroundColor Red
    }
} else {
    Write-Host "  AppHost launchSettings.json not found: $appHostLaunchSettingsPath" -ForegroundColor Red
}

# Step 5: Rebuild the project
Write-Host "`nStep 5: Rebuilding the project..." -ForegroundColor Magenta
try {
    dotnet build (Join-Path (Split-Path $PSScriptRoot -Parent) "AppBlueprint.sln") | Out-Null
    Write-Host "  Project rebuilt successfully." -ForegroundColor Green
} catch {
    Write-Host "  Error rebuilding project: $_" -ForegroundColor Red
}

# Step 6: Start AppHost with fixed configuration
Write-Host "`nStep 6: Starting AppHost..." -ForegroundColor Magenta
$appHostProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "AppBlueprint.AppHost" -PassThru -NoNewWindow
if ($appHostProcess) {
    Write-Host "  AppHost started with PID: $($appHostProcess.Id)" -ForegroundColor Green
    # Wait for AppHost to start up
    Write-Host "  Waiting for AppHost to initialize (15 seconds)..." -ForegroundColor Yellow
    Start-Sleep -Seconds 15
} else {
    Write-Host "  Failed to start AppHost." -ForegroundColor Red
}

# Step 7: Verify that everything is working
Write-Host "`nStep 7: Verifying configuration..." -ForegroundColor Magenta

# Check process environment variables
Write-Host "  Process environment variables:" -ForegroundColor Yellow
Write-Host "    ASPIRE_DASHBOARD_PORT: $env:ASPIRE_DASHBOARD_PORT" -ForegroundColor Gray
Write-Host "    DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $env:DOTNET_DASHBOARD_OTLP_ENDPOINT_URL" -ForegroundColor Gray
Write-Host "    OTEL_EXPORTER_OTLP_ENDPOINT: $env:OTEL_EXPORTER_OTLP_ENDPOINT" -ForegroundColor Gray
Write-Host "    OTEL_EXPORTER_OTLP_PROTOCOL: $env:OTEL_EXPORTER_OTLP_PROTOCOL" -ForegroundColor Gray
Write-Host "    OTEL_EXPORTER_OTLP_HEADERS: '$env:OTEL_EXPORTER_OTLP_HEADERS'" -ForegroundColor Gray

# Check if AppHost is running
$appHostProcess = Get-Process -Name "AppBlueprint.AppHost" -ErrorAction SilentlyContinue
if ($appHostProcess) {
    Write-Host "  AppHost is running (PID: $($appHostProcess.Id))" -ForegroundColor Green
} else {
    Write-Host "  AppHost is NOT running" -ForegroundColor Red
}

# Check if dashboard is accessible
$dashboardUrl = "http://localhost:$dashboardPort"
try {
    $response = Invoke-WebRequest -Uri $dashboardUrl -TimeoutSec 5 -ErrorAction Stop
    Write-Host "  Aspire dashboard is accessible at $dashboardUrl" -ForegroundColor Green
} catch {
    Write-Host "  Aspire dashboard is NOT accessible at $dashboardUrl" -ForegroundColor Red
    Write-Host "  Error: $_" -ForegroundColor Red
}

# Check if OTLP endpoint is accessible
try {
    $response = Invoke-WebRequest -Uri "$aspireOtlpEndpoint/v1/metrics" -Method Head -TimeoutSec 2 -ErrorAction SilentlyContinue
    Write-Host "  OTLP endpoint is responding at $aspireOtlpEndpoint" -ForegroundColor Green
} catch {
    # 400 Bad Request is expected for HEAD request to OTLP endpoint without payload
    if ($_.Exception.Response.StatusCode.value__ -eq 400) {
        Write-Host "  OTLP endpoint is responding at $aspireOtlpEndpoint (400 Bad Request is expected)" -ForegroundColor Green
    } else {
        Write-Host "  OTLP endpoint is NOT responding correctly at $aspireOtlpEndpoint" -ForegroundColor Red
        Write-Host "  Error: $_" -ForegroundColor Red
    }
}

# Generate some traffic for telemetry
Write-Host "`nGenerating test traffic for telemetry..." -ForegroundColor Magenta
$endpoints = @(
    "http://localhost:8090/health", # API
    "http://localhost:8092",        # Web
    "http://localhost:8094/health"  # Gateway
)

foreach ($endpoint in $endpoints) {
    try {
        Write-Host "  Testing $endpoint..." -ForegroundColor Yellow -NoNewline
        $response = Invoke-WebRequest -Uri $endpoint -TimeoutSec 5 -ErrorAction Stop
        Write-Host " ✅ Success ($($response.StatusCode))" -ForegroundColor Green
    } catch {
        Write-Host " ❌ Failed: $_" -ForegroundColor Red
    }
}

Write-Host "`n===== OpenTelemetry Configuration Fix Complete! =====" -ForegroundColor Cyan
Write-Host "The Aspire dashboard should now be accessible at: $dashboardUrl" -ForegroundColor Green
Write-Host "Telemetry data should now appear in the dashboard." -ForegroundColor Green
Write-Host "It may take a minute or two for telemetry data to start appearing." -ForegroundColor Yellow
Write-Host "If you're still experiencing issues, please check the output of the AppHost for any errors." -ForegroundColor Yellow
