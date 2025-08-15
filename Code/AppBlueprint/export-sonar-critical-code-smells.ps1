# Export CRITICAL code smells for saas-factory-labs_Saas-Factory to CSV
#
# Prerequisites:
# - Set an environment variable SONAR_TOKEN with a SonarCloud user token that has browse permissions.
# - Run this script in PowerShell. It will write the CSV to "reports/sonar/critical-code-smells.csv" next to this script.

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $env:SONAR_TOKEN) {
    Write-Error "Environment variable SONAR_TOKEN is not set. Create a SonarCloud token and set it before running."
}

$organization = 'saas-factory-labs'
$projectKey   = 'saas-factory-labs_Saas-Factory'
$baseUrl      = 'https://sonarcloud.io/api/issues/search'
$pageSize     = 500

# Basic auth with token as username and empty password
$basic = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("$($env:SONAR_TOKEN):"))
$headers = @{ Authorization = "Basic $basic" }

$allIssues = @()
$page = 1
do {
    $query = @{
        organization  = $organization
        componentKeys  = $projectKey
        types          = 'CODE_SMELL'
        severities     = 'CRITICAL'
        resolved       = 'false'
        s              = 'SEVERITY'
        ps             = $pageSize
        p              = $page
    }

    $qs = ($query.GetEnumerator() | ForEach-Object { "{0}={1}" -f $_.Key, [uri]::EscapeDataString([string]$_.Value) }) -join '&'
    $uri = "$baseUrl?$qs"

    Write-Host "Fetching page $page ..." -ForegroundColor Cyan
    $resp = Invoke-RestMethod -Method Get -Uri $uri -Headers $headers

    if ($null -ne $resp.issues) {
        $allIssues += $resp.issues
    }

    $total    = [int]$resp.paging.total
    $pageSize = [int]$resp.paging.pageSize
    $page    += 1
} while ( ($page - 1) * $pageSize -lt $total )

# Deduplicate by issue key (defensive)
$unique = @{}
$deduped = foreach ($i in $allIssues) {
    if (-not $unique.ContainsKey($i.key)) { $unique[$i.key] = $true; $i }
}

$rows = foreach ($issue in $deduped) {
    $componentKey = [string]$issue.component
    $filePath = if ($componentKey -like '*:*') { ($componentKey -split ':', 2)[1] } else { $componentKey }
    $line = ''
    if ($null -ne $issue.line) { $line = [string]$issue.line }
    elseif ($null -ne $issue.textRange -and $null -ne $issue.textRange.startLine) { $line = [string]$issue.textRange.startLine }

    [pscustomobject]@{
        issueKey = [string]$issue.key
        rule     = [string]$issue.rule
        severity = [string]$issue.severity
        type     = [string]$issue.type
        message  = [string]$issue.message
        filePath = $filePath
        line     = $line
        issueUrl = "https://sonarcloud.io/project/issues?id=$projectKey&issues=$($issue.key)&open=$($issue.key)"
        fileUrl  = "https://sonarcloud.io/code?id=$projectKey&selected=$componentKey"
    }
}

$outDir = Join-Path $PSScriptRoot 'reports/sonar'
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$outFile = Join-Path $outDir 'critical-code-smells.csv'

$rows | Sort-Object issueKey | Export-Csv -NoTypeInformation -Encoding UTF8 -Path $outFile
Write-Host "Export complete: $($rows.Count) issues -> $outFile" -ForegroundColor Green
