#!/usr/bin/env pwsh
# check-otel-overrides.ps1
# This script checks for hardcoded OpenTelemetry configurations that might be overriding environment variables

Write-Host "Checking for OpenTelemetry Configuration Overrides" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Gray

# Function to search for OTLP setting patterns in code files
function Find-OtelConfigInCode {
    param (
        [string]$SearchPath,
        [string]$FilePattern = "*.cs"
    )

    $searchTerms = @(
        "OTEL_EXPORTER_OTLP_ENDPOINT", 
        "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL",
        "Environment.SetEnvironmentVariable",
        "SetEnvironmentVariable"
    )

    Write-Host "`nSearching for OTLP configuration code in $SearchPath" -ForegroundColor Magenta

    foreach ($term in $searchTerms) {
        Write-Host "`nFiles containing '$term':" -ForegroundColor Yellow
        $files = Get-ChildItem -Path $SearchPath -Recurse -Include $FilePattern | 
                 Select-String -Pattern $term -List |
                 Select-Object Path, LineNumber

        if ($files.Count -eq 0) {
            Write-Host "  No files found with term '$term'" -ForegroundColor Gray
            continue
        }

        foreach ($file in $files) {
            Write-Host "  $($file.Path) (line $($file.LineNumber))" -ForegroundColor White
            
            # Get 5 lines of context
            $content = Get-Content $file.Path
            $start = [Math]::Max(0, $file.LineNumber - 3)
            $end = [Math]::Min($content.Length, $file.LineNumber + 2)
            
            for ($i = $start; $i -lt $end; $i++) {
                $lineNumber = $i + 1
                $prefix = if ($lineNumber -eq $file.LineNumber) { ">" } else { " " }
                Write-Host "    $prefix $lineNumber: $($content[$i])" -ForegroundColor $(if ($lineNumber -eq $file.LineNumber) { "Green" } else { "Gray" })
            }
        }
    }
}

# Function to check environment variables in processes
function Get-ProcessEnvVars {
    param (
        [string]$ProcessName
    )

    Write-Host "`nChecking environment variables for process: $ProcessName" -ForegroundColor Magenta
    
    $processes = Get-Process -Name $ProcessName -ErrorAction SilentlyContinue
    
    if (-not $processes) {
        Write-Host "  Process '$ProcessName' not found" -ForegroundColor Red
        return
    }
    
    foreach ($process in $processes) {
        try {
            $handle = (Get-Process -Id $process.Id).Handle
            Write-Host "  Process ID: $($process.Id), Handle: $handle" -ForegroundColor Yellow
            
            # Using wmic to get environment variables (this only shows a subset of variables)
            $envVars = & wmic process where "ProcessId=$($process.Id)" get CommandLine /format:list

            Write-Host "  Command Line: $envVars" -ForegroundColor White
            
            # Unfortunately, getting all environment variables of another process is difficult
            # We can use a workaround by checking config files and sources
            Write-Host "  Note: Full environment variables can't be extracted from another process." -ForegroundColor Gray
            Write-Host "  Check startup code and launchSettings.json files instead." -ForegroundColor Gray
        }
        catch {
            Write-Host "  Error accessing process: $_" -ForegroundColor Red
        }
    }
}

# Main execution starts here
$appBlueprintDir = "C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"

# 1. Check current environment variables
Write-Host "`nCurrent environment variables:" -ForegroundColor Magenta
$otelVars = @(
    "OTEL_EXPORTER_OTLP_ENDPOINT",
    "OTEL_EXPORTER_OTLP_HEADERS",
    "OTEL_EXPORTER_OTLP_PROTOCOL",
    "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL", 
    "ASPIRE_DASHBOARD_PORT"
)

foreach ($var in $otelVars) {
    $value = [Environment]::GetEnvironmentVariable($var, "User")
    Write-Host "  $var (User): $value" -ForegroundColor White
    
    $value = [Environment]::GetEnvironmentVariable($var, "Process")
    Write-Host "  $var (Process): $value" -ForegroundColor White
}

# 2. Search for hardcoded OTLP settings in code files
Find-OtelConfigInCode -SearchPath "$appBlueprintDir\AppBlueprint.Web"
Find-OtelConfigInCode -SearchPath "$appBlueprintDir\AppBlueprint.ApiService"
Find-OtelConfigInCode -SearchPath "$appBlueprintDir\AppBlueprint.AppGateway"
Find-OtelConfigInCode -SearchPath "$appBlueprintDir\AppBlueprint.ServiceDefaults"

# 3. Check environment variables in running processes
Get-ProcessEnvVars -ProcessName "AppBlueprint.Web"
Get-ProcessEnvVars -ProcessName "AppBlueprint.ApiService" 
Get-ProcessEnvVars -ProcessName "AppBlueprint.AppGateway"
Get-ProcessEnvVars -ProcessName "AppBlueprint.AppHost"

# 4. Suggest fixes
Write-Host "`nPotential issues and fixes:" -ForegroundColor Magenta
Write-Host "1. Hardcoded OTLP endpoint in AppBlueprint.Web/Program.cs" -ForegroundColor White
Write-Host "   Remove or comment out the following lines:" -ForegroundColor White
Write-Host "   Environment.SetEnvironmentVariable(\"OTEL_EXPORTER_OTLP_ENDPOINT\", \"http://localhost:4318\");" -ForegroundColor Gray
Write-Host "   Environment.SetEnvironmentVariable(\"OTEL_EXPORTER_OTLP_PROTOCOL\", \"http/protobuf\");" -ForegroundColor Gray

Write-Host "`n2. Ensure all projects have ServiceDefaults added" -ForegroundColor White
Write-Host "   Check that each Program.cs includes:" -ForegroundColor White
Write-Host "   builder.AddServiceDefaults();" -ForegroundColor Gray

Write-Host "`n3. Ensure the AppHost is stopped before restarting to avoid port conflicts" -ForegroundColor White
Write-Host "   Get-Process -Name \"AppBlueprint.*\" | Stop-Process -Force" -ForegroundColor Gray
Write-Host "   Wait a few seconds, then restart AppHost" -ForegroundColor Gray

Write-Host "`n4. Try using the fix script:" -ForegroundColor White
Write-Host "   .\fix-all-telemetry-issues.ps1" -ForegroundColor Gray
Write-Host "   Restart your PowerShell session afterward" -ForegroundColor Gray
Write-Host "   Then start AppHost: dotnet run --project AppBlueprint.AppHost" -ForegroundColor Gray
