#!/usr/bin/env pwsh
# check-process-variables.ps1
# This script checks the process environment variables for running AppBlueprint processes

Write-Host "Checking Process Environment Variables" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Gray

# Find all running AppBlueprint processes
$processes = Get-Process -Name "AppBlueprint*" -ErrorAction SilentlyContinue

if (-not $processes) {
    Write-Host "No AppBlueprint processes found running. Please start the AppHost first." -ForegroundColor Red
    exit 1
}

Write-Host "Found $($processes.Count) AppBlueprint processes:" -ForegroundColor Green

foreach ($process in $processes) {
    $processName = $process.ProcessName
    $pid = $process.Id
    
    Write-Host "`nProcess: $processName (PID: $pid)" -ForegroundColor Magenta
    
    try {
        # Call Get-ProcessEnvironmentVariable.exe to get environment variables for each process
        # Note: You need to have Process Explorer (procexp.exe) or similar tool installed
        # Alternative approach: Use WMI to get environment variables
        $envVars = Get-CimInstance -ClassName Win32_Process -Filter "ProcessId = $pid" | 
                  Select-Object -ExpandProperty CommandLine
        
        Write-Host "  Command Line: $envVars" -ForegroundColor Yellow
        
        # Alternative method using powershell
        Write-Host "  Process Start Info:" -ForegroundColor Yellow
        try {
            $process2 = Get-Process -Id $pid
            $startInfo = $process2.StartInfo
            Write-Host "    Arguments: $($startInfo.Arguments)" -ForegroundColor Gray
            Write-Host "    Working Directory: $($startInfo.WorkingDirectory)" -ForegroundColor Gray
        } catch {
            Write-Host "    Cannot get detailed start info for this process" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "  Could not retrieve environment variables for process $pid" -ForegroundColor Red
        Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Show current process environment variables related to OpenTelemetry
Write-Host "`nCurrent PowerShell Process Environment Variables:" -ForegroundColor Magenta
Get-ChildItem Env: | Where-Object { 
    $_.Name -like "OTEL*" -or 
    $_.Name -like "*DASHBOARD*" -or 
    $_.Name -like "ASPIRE*" 
} | Format-Table -AutoSize

# Show user environment variables
Write-Host "`nUser Environment Variables:" -ForegroundColor Magenta
$userEnvVars = [Environment]::GetEnvironmentVariables([System.EnvironmentVariableTarget]::User)
$filteredUserEnvVars = $userEnvVars.GetEnumerator() | Where-Object { 
    $_.Key -like "OTEL*" -or 
    $_.Key -like "*DASHBOARD*" -or 
    $_.Key -like "ASPIRE*" 
}

if ($filteredUserEnvVars) {
    $filteredUserEnvVars | ForEach-Object {
        Write-Host "  $($_.Key): $($_.Value)" -ForegroundColor Yellow
    }
} else {
    Write-Host "  No relevant environment variables found at User level" -ForegroundColor Yellow
}

# Provide instructions for checking process variables using procexp
Write-Host "`nTo check process environment variables directly:" -ForegroundColor Magenta
Write-Host "1. Download and run Process Explorer from Sysinternals" -ForegroundColor White
Write-Host "2. Find the AppBlueprint.AppHost process" -ForegroundColor White
Write-Host "3. Right-click and select 'Properties'" -ForegroundColor White
Write-Host "4. Go to the 'Environment' tab to see all environment variables" -ForegroundColor White
Write-Host "5. Look for OTEL_EXPORTER_OTLP_ENDPOINT and related variables" -ForegroundColor White
