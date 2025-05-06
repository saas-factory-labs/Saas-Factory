# This script is used to test the OpenTelemetry configuration in the Aspire application
# It sets the required environment variables and verifies the current configuration

Write-Host "Testing OpenTelemetry Configuration for AppBlueprint" -ForegroundColor Cyan

# Check if OTEL_EXPORTER_OTLP_ENDPOINT is set
$otlpEndpoint = [System.Environment]::GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
if (-not $otlpEndpoint) {
    Write-Host "OTEL_EXPORTER_OTLP_ENDPOINT is not set. Setting to default value: http://localhost:4318" -ForegroundColor Yellow
    [System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4318", "Process")
    $otlpEndpoint = "http://localhost:4318"
} else {
    Write-Host "OTEL_EXPORTER_OTLP_ENDPOINT is already set to: $otlpEndpoint" -ForegroundColor Green
}

# Check if OTEL_EXPORTER_OTLP_HEADERS is set
$otlpHeaders = [System.Environment]::GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS")
if (-not $otlpHeaders) {
    Write-Host "OTEL_EXPORTER_OTLP_HEADERS is not set. Setting to empty value." -ForegroundColor Yellow
    [System.Environment]::SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "", "Process")
} else {
    Write-Host "OTEL_EXPORTER_OTLP_HEADERS is already set to: $otlpHeaders" -ForegroundColor Green
}

# Print information about the OpenTelemetry configuration
Write-Host "`nOpenTelemetry Configuration Summary:" -ForegroundColor Cyan
Write-Host "--------------------------------" -ForegroundColor Gray

Write-Host "OTEL_EXPORTER_OTLP_ENDPOINT: $otlpEndpoint" -ForegroundColor White
Write-Host "OTEL_EXPORTER_OTLP_HEADERS: $otlpHeaders" -ForegroundColor White

# Check if OTLP endpoint is reachable
Write-Host "`nVerifying OTLP endpoint connectivity..." -ForegroundColor Yellow

# Function to test endpoint connectivity
function Test-OtlpEndpoint {
    param (
        [string]$Endpoint
    )

    try {
        # Remove trailing slash if present
        $Endpoint = $Endpoint.TrimEnd('/')
        
        # Try to reach the metrics endpoint
        $result = Invoke-WebRequest -Uri "$Endpoint/v1/metrics" -Method HEAD -TimeoutSec 5 -ErrorAction SilentlyContinue
        
        if ($result.StatusCode -eq 200) {
            Write-Host "✓ OTLP endpoint $Endpoint is reachable!" -ForegroundColor Green
            return $true
        } else {
            Write-Host "! OTLP endpoint $Endpoint returned status code: $($result.StatusCode)" -ForegroundColor Yellow
            return $false
        }
    } catch {
        Write-Host "✗ OTLP endpoint $Endpoint is not reachable. Error: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Try to connect to the endpoint
$connected = Test-OtlpEndpoint -Endpoint $otlpEndpoint
if (-not $connected) {
    # If using https, try http as fallback
    if ($otlpEndpoint -like "https://*") {
        $httpEndpoint = $otlpEndpoint -replace "^https://", "http://"
        Write-Host "  Trying fallback to HTTP endpoint: $httpEndpoint" -ForegroundColor Yellow
        $connected = Test-OtlpEndpoint -Endpoint $httpEndpoint
        
        if ($connected) {
            Write-Host "  Consider updating your OTEL_EXPORTER_OTLP_ENDPOINT to use HTTP instead of HTTPS" -ForegroundColor Cyan
            $env:OTEL_EXPORTER_OTLP_ENDPOINT = $httpEndpoint
        }
    }
    
    if (-not $connected) {
        Write-Host "  Make sure the Aspire dashboard is running to receive telemetry data." -ForegroundColor Red
        Write-Host "  Try starting the Aspire dashboard with 'dotnet run --project AppBlueprint.AppHost'" -ForegroundColor Red
    }
}

# Check if any project is running with telemetry enabled
Write-Host "`nChecking for running services:" -ForegroundColor Yellow
$processes = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue
if ($processes) {
    foreach ($proc in $processes) {
        Write-Host "- $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Green
    }
} else {
    Write-Host "No AppBlueprint processes found. Start the application to send telemetry data." -ForegroundColor Yellow
}

# Check for ASPIRE_DASHBOARD_URL environment variable
$dashboardUrl = $env:ASPIRE_DASHBOARD_URL
if (-not $dashboardUrl) {
    $dashboardUrl = "http://localhost:18888"
}
Write-Host "`nTesting Aspire dashboard connectivity..." -ForegroundColor Yellow
try {
    $result = Invoke-WebRequest -Uri $dashboardUrl -Method HEAD -TimeoutSec 5 -ErrorAction SilentlyContinue
    if ($result.StatusCode -eq 200) {
        Write-Host "✓ Aspire dashboard is reachable at $dashboardUrl" -ForegroundColor Green
        Write-Host "  Open the dashboard in your browser to view telemetry data" -ForegroundColor Cyan
    } else {
        Write-Host "! Aspire dashboard returned status code: $($result.StatusCode)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ Aspire dashboard is not reachable at $dashboardUrl. Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  Start the application with 'dotnet run --project AppBlueprint.AppHost' to launch the dashboard" -ForegroundColor Yellow
}

Write-Host "`nOpenTelemetry Configuration Summary:" -ForegroundColor Magenta
Write-Host "1. OTLP Endpoint: $otlpEndpoint" -ForegroundColor White
Write-Host "2. Aspire Dashboard: $dashboardUrl" -ForegroundColor White
Write-Host "3. Services configured to send telemetry:" -ForegroundColor White
Write-Host "   - AppBlueprint-Api" -ForegroundColor White
Write-Host "   - AppBlueprint-Web" -ForegroundColor White 
Write-Host "   - AppBlueprint-AppGw" -ForegroundColor White

Write-Host "`nOpenTelemetry Troubleshooting Tips:" -ForegroundColor Magenta
Write-Host "1. Ensure Aspire dashboard is running to collect telemetry"
Write-Host "2. OTLP endpoint must be reachable from all services"
Write-Host "3. Set OTEL_EXPORTER_OTLP_ENDPOINT environment variable if needed"
Write-Host "4. Check for errors in service logs related to OpenTelemetry"
Write-Host "5. To manually verify telemetry, run: dotnet run --project AppBlueprint.AppHost"
        if ($packages -match "$package.*Version=""([^""]+)""") {
            Write-Host "$package version: $($Matches[1])" -ForegroundColor Green
        } else {
            Write-Host "$package: Not found" -ForegroundColor Red
        }
    }
} else {
    Write-Host "Directory.Packages.props file not found at: $packagesPath" -ForegroundColor Red
}

# Instructions for verifying telemetry in Aspire dashboard
Write-Host "`nTo verify OpenTelemetry is working:" -ForegroundColor Cyan
Write-Host "--------------------------------" -ForegroundColor Gray
Write-Host "1. Run the Aspire application using Visual Studio or 'dotnet run'"
Write-Host "2. Open the Aspire dashboard (default: http://localhost:18888)"
Write-Host "3. Navigate to the Metrics and Traces tabs to verify telemetry is being collected"
Write-Host "4. If no telemetry appears, check that the OTLP endpoint is correctly configured"
Write-Host "   and that all services are properly referencing the ServiceDefaults project."
Write-Host "`nFor more information, see: https://learn.microsoft.com/en-us/dotnet/aspire/telemetry/"