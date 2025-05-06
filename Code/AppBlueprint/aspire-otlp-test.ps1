#!/usr/bin/env pwsh
# aspire-otlp-test.ps1
# This script tests the Aspire OpenTelemetry OTLP endpoint specifically

Write-Host "Aspire OpenTelemetry OTLP Endpoint Test" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Gray

# Function to test HTTP endpoint with proper payloads
function Test-OtlpEndpoint {
    param (
        [string]$Url,
        [string]$Method = "POST",
        [int]$TimeoutSec = 5,
        [string]$EndpointType = "metrics"
    )

    try {
        # Create the appropriate payload based on endpoint type
        $payloadJson = switch ($EndpointType) {
            "metrics" {
                @{
                    "resourceMetrics" = @(
                        @{
                            "resource" = @{
                                "attributes" = @(
                                    @{
                                        "key" = "service.name"
                                        "value" = @{
                                            "stringValue" = "aspire-test-service"
                                        }
                                    }
                                )
                            }
                            "scopeMetrics" = @()
                        }
                    )
                } | ConvertTo-Json -Depth 10
            }
            "traces" {
                @{
                    "resourceSpans" = @(
                        @{
                            "resource" = @{
                                "attributes" = @(
                                    @{
                                        "key" = "service.name"
                                        "value" = @{
                                            "stringValue" = "aspire-test-service"
                                        }
                                    }
                                )
                            }
                            "scopeSpans" = @()
                        }
                    )
                } | ConvertTo-Json -Depth 10
            }
            default {
                "{}"
            }
        }

        $headers = @{
            "Content-Type" = "application/json"
        }

        $params = @{
            Uri = $Url
            Method = $Method
            Body = $payloadJson
            ContentType = "application/json"
            Headers = $headers
            TimeoutSec = $TimeoutSec
            ErrorAction = "SilentlyContinue"
        }

        $response = Invoke-WebRequest @params
        return @{
            Success = $true
            StatusCode = $response.StatusCode
            StatusDescription = $response.StatusDescription
            Content = $response.Content
        }
    }
    catch [System.Net.WebException] {
        $ex = $_.Exception
        if ($ex.Response) {
            $response = $ex.Response
            $reader = New-Object System.IO.StreamReader($response.GetResponseStream())
            $content = $reader.ReadToEnd()
            return @{
                Success = $false
                StatusCode = $response.StatusCode.value__
                StatusDescription = $response.StatusDescription
                Error = $ex.Message
                Content = $content
            }
        }
        else {
            return @{
                Success = $false
                Error = $ex.Message
            }
        }
    }
    catch {
        return @{
            Success = $false
            Error = $_.Exception.Message
        }
    }
}

# Detect Aspire dashboard port
$aspirePortSources = @(
    @{Source = "Launch Settings"; Value = "15068"},
    @{Source = "Environment Variable"; Value = [System.Environment]::GetEnvironmentVariable("ASPIRE_DASHBOARD_PORT")},
    @{Source = "Process Environment Variable"; Value = $env:ASPIRE_DASHBOARD_PORT}
)

$dashboardPort = $null
foreach ($source in $aspirePortSources) {
    if (-not [string]::IsNullOrEmpty($source.Value)) {
        $dashboardPort = $source.Value
        Write-Host "Using Aspire dashboard port from $($source.Source): $dashboardPort" -ForegroundColor Green
        break
    }
}

if (-not $dashboardPort) {
    $dashboardPort = "15068"
    Write-Host "No dashboard port found in known sources. Using default: $dashboardPort" -ForegroundColor Yellow
}

# Get OTLP endpoint
$dashboardEndpoint = [System.Environment]::GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL")
$otlpEndpoint = [System.Environment]::GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")

if ([string]::IsNullOrEmpty($dashboardEndpoint) -and [string]::IsNullOrEmpty($otlpEndpoint)) {
    # If no endpoint is set, try to determine it from the dashboard port
    $inferredEndpoint = "http://localhost:18889"
    Write-Host "No OTLP endpoint found. Using inferred endpoint: $inferredEndpoint" -ForegroundColor Yellow
    $otlpEndpoint = $inferredEndpoint
}
elseif (-not [string]::IsNullOrEmpty($dashboardEndpoint)) {
    Write-Host "Using DOTNET_DASHBOARD_OTLP_ENDPOINT_URL: $dashboardEndpoint" -ForegroundColor Green
    $otlpEndpoint = $dashboardEndpoint
}
else {
    Write-Host "Using OTEL_EXPORTER_OTLP_ENDPOINT: $otlpEndpoint" -ForegroundColor Green
}

# Check if Aspire dashboard is running
$dashboardUrl = "http://localhost:$dashboardPort"
Write-Host "`nChecking Aspire dashboard at $dashboardUrl..." -ForegroundColor Cyan
try {
    $dashboardResponse = Invoke-WebRequest -Uri $dashboardUrl -TimeoutSec 5 -ErrorAction SilentlyContinue
    Write-Host "✅ Aspire dashboard is accessible! Status: $($dashboardResponse.StatusCode)" -ForegroundColor Green
}
catch {
    Write-Host "❌ Aspire dashboard is not accessible: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  Make sure the Aspire application host is running." -ForegroundColor Yellow
}

# Test the OTLP metrics endpoint
Write-Host "`nTesting OTLP metrics endpoint at $otlpEndpoint/v1/metrics..." -ForegroundColor Cyan
$metricsResult = Test-OtlpEndpoint -Url "$otlpEndpoint/v1/metrics" -EndpointType "metrics"

if ($metricsResult.Success) {
    Write-Host "✅ Metrics endpoint is accessible! Status: $($metricsResult.StatusCode)" -ForegroundColor Green
    Write-Host "Response: $($metricsResult.Content)" -ForegroundColor Gray
} else {
    Write-Host "❌ Metrics endpoint is not accessible: $($metricsResult.Error)" -ForegroundColor Red
    
    if ($metricsResult.Content) {
        Write-Host "Error details: $($metricsResult.Content)" -ForegroundColor Red
    }
    
    # Try HTTP if HTTPS fails
    if ($otlpEndpoint -like "https://*") {
        $httpEndpoint = $otlpEndpoint -replace "^https://", "http://"
        Write-Host "Testing fallback HTTP endpoint: $httpEndpoint/v1/metrics" -ForegroundColor Yellow
        $httpResult = Test-OtlpEndpoint -Url "$httpEndpoint/v1/metrics" -EndpointType "metrics"
        
        if ($httpResult.Success) {
            Write-Host "✅ HTTP fallback endpoint is accessible! Status: $($httpResult.StatusCode)" -ForegroundColor Green
            Write-Host "Response: $($httpResult.Content)" -ForegroundColor Gray
            Write-Host "Consider using HTTP instead of HTTPS for your OTLP endpoint" -ForegroundColor Yellow
            $otlpEndpoint = $httpEndpoint
        } else {
            Write-Host "❌ HTTP fallback endpoint is also not accessible: $($httpResult.Error)" -ForegroundColor Red
            if ($httpResult.Content) {
                Write-Host "Error details: $($httpResult.Content)" -ForegroundColor Red
            }
        }
    }
}

# Test the OTLP traces endpoint
Write-Host "`nTesting OTLP traces endpoint at $otlpEndpoint/v1/traces..." -ForegroundColor Cyan
$tracesResult = Test-OtlpEndpoint -Url "$otlpEndpoint/v1/traces" -EndpointType "traces"

if ($tracesResult.Success) {
    Write-Host "✅ Traces endpoint is accessible! Status: $($tracesResult.StatusCode)" -ForegroundColor Green
    Write-Host "Response: $($tracesResult.Content)" -ForegroundColor Gray
} else {
    Write-Host "❌ Traces endpoint is not accessible: $($tracesResult.Error)" -ForegroundColor Red
    if ($tracesResult.Content) {
        Write-Host "Error details: $($tracesResult.Content)" -ForegroundColor Red
    }
}

# Check for the OTLP health endpoint
Write-Host "`nTesting OTLP health endpoint at $otlpEndpoint/health..." -ForegroundColor Cyan
try {
    $healthResponse = Invoke-WebRequest -Uri "$otlpEndpoint/health" -Method GET -TimeoutSec 5 -ErrorAction SilentlyContinue
    Write-Host "✅ OTLP health endpoint is accessible! Status: $($healthResponse.StatusCode)" -ForegroundColor Green
    Write-Host "Response: $($healthResponse.Content)" -ForegroundColor Gray
}
catch {
    Write-Host "❌ OTLP health endpoint is not accessible: $($_.Exception.Message)" -ForegroundColor Red
}

# Check for running AppBlueprint processes
Write-Host "`nChecking for running AppBlueprint processes..." -ForegroundColor Cyan
$processes = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue
if ($processes) {
    Write-Host "Found running AppBlueprint processes:" -ForegroundColor Green
    foreach ($proc in $processes) {
        Write-Host "  - $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor White
    }
} else {
    Write-Host "❌ No AppBlueprint processes found." -ForegroundColor Red
    Write-Host "  Start the AppHost project to run the application." -ForegroundColor Yellow
}

# Display OpenTelemetry package versions
Write-Host "`nChecking OpenTelemetry package versions..." -ForegroundColor Cyan
$packagesPath = Join-Path $PSScriptRoot "Directory.Packages.props"
if (Test-Path $packagesPath) {
    $packages = Get-Content $packagesPath -Raw
    
    $otelPackages = @(
        "OpenTelemetry",
        "OpenTelemetry.Api",
        "OpenTelemetry.Exporter.Console",
        "OpenTelemetry.Exporter.OpenTelemetryProtocol",
        "OpenTelemetry.Extensions.Hosting",
        "OpenTelemetry.Instrumentation.AspNetCore",
        "OpenTelemetry.Instrumentation.Http",
        "OpenTelemetry.Instrumentation.Runtime"
    )
    
    foreach ($package in $otelPackages) {
        if ($packages -match [regex]::Escape($package) + '"\s+Version="([^"]+)"') {
            Write-Host "  $($package): $($Matches[1])" -ForegroundColor Green
        } else {
            Write-Host "  $($package): Not found" -ForegroundColor Red
        }
    }
} else {
    Write-Host "  Directory.Packages.props not found!" -ForegroundColor Red
}

# Provide troubleshooting tips
Write-Host "`nAspire Dashboard OpenTelemetry Configuration:" -ForegroundColor Magenta
Write-Host "1. In Aspire, the dashboard provides an OTLP collector on port 18889 by default"
Write-Host "2. The exporter in ServiceDefaults should point to this endpoint"
Write-Host "3. Make sure the AppHost has the right environment variables set: DOTNET_DASHBOARD_OTLP_ENDPOINT_URL and OTEL_EXPORTER_OTLP_ENDPOINT"

Write-Host "`nTo fix OTLP connection issues:" -ForegroundColor Green
Write-Host "1. Ensure the Aspire dashboard is running (dotnet run --project AppBlueprint.AppHost)"
Write-Host "2. Update ServiceDefaults to use the correct OTLP endpoint (http://localhost:18889)"
Write-Host "3. Rebuild and restart all services"
Write-Host "4. Check the dashboard at http://localhost:15068"
