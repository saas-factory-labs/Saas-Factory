# Flow 2 (CI/CD): downloads private admin portal plugin packages from a GitHub Packages
# NuGet feed and extracts their dlls into a plugins/ folder that the Docker build bakes
# into the DeploymentManager image. Plugin NAMES live in GitHub Actions variables -
# never in this public repo's code.
#
# Environment variables (or parameters):
#   ADMIN_PLUGIN_PACKAGES   Comma/space-separated "PackageId:Version" list,
#                           e.g. "SaaSFactory.Dating.Admin:1.2.3, SaaSFactory.Bolig.Admin:2.0.0"
#   ADMIN_PLUGINS_FEED_OWNER GitHub org/user that owns the private package feed
#   PLUGINS_FEED_TOKEN      PAT or GITHUB_TOKEN with read:packages on the private repos
#
# Usage (GitHub Actions step, before docker build):
#   pwsh Code/DeploymentManager/Scripts/download-admin-plugins.ps1 -OutputDir Code/DeploymentManager/plugins
param(
    [string]$Packages = $env:ADMIN_PLUGIN_PACKAGES,
    [string]$FeedOwner = $env:ADMIN_PLUGINS_FEED_OWNER,
    [string]$Token = $env:PLUGINS_FEED_TOKEN,
    [string]$OutputDir = (Join-Path $PSScriptRoot '..\plugins'),
    [string]$TargetFramework = 'net10.0'
)

$ErrorActionPreference = 'Stop'

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

if ([string]::IsNullOrWhiteSpace($Packages)) {
    Write-Host "ADMIN_PLUGIN_PACKAGES is empty - produced an empty plugins folder (no admin portals in this build)."
    return
}

if ([string]::IsNullOrWhiteSpace($FeedOwner) -or [string]::IsNullOrWhiteSpace($Token)) {
    throw "ADMIN_PLUGINS_FEED_OWNER and PLUGINS_FEED_TOKEN are required when ADMIN_PLUGIN_PACKAGES is set."
}

$authHeader = @{ Authorization = 'Basic ' + [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("user:$Token")) }

$packageList = $Packages -split '[,\s]+' | Where-Object { $_ -ne '' }
foreach ($package in $packageList) {
    $parts = $package.Split(':')
    if ($parts.Count -ne 2) {
        throw "Invalid package spec '$package' - expected PackageId:Version."
    }

    $packageId = $parts[0].Trim().ToLowerInvariant()
    $version = $parts[1].Trim()
    $url = "https://nuget.pkg.github.com/$FeedOwner/download/$packageId/$version/$packageId.$version.nupkg"
    $nupkgPath = Join-Path ([IO.Path]::GetTempPath()) "$packageId.$version.nupkg"

    Write-Host "Downloading $packageId $version from $FeedOwner feed..."
    Invoke-WebRequest -Uri $url -Headers $authHeader -OutFile $nupkgPath

    $extractDir = Join-Path ([IO.Path]::GetTempPath()) "adminplugin-$packageId-$version"
    if (Test-Path $extractDir) { Remove-Item -Recurse -Force $extractDir }
    Expand-Archive -Path $nupkgPath -DestinationPath $extractDir

    $libDir = Join-Path $extractDir "lib\$TargetFramework"
    if (-not (Test-Path $libDir)) {
        throw "Package $packageId $version contains no lib/$TargetFramework folder - wrong target framework?"
    }

    Copy-Item (Join-Path $libDir '*.dll') $OutputDir -Force
    Copy-Item (Join-Path $libDir '*.pdb') $OutputDir -Force -ErrorAction SilentlyContinue

    Remove-Item $nupkgPath -Force
    Remove-Item -Recurse -Force $extractDir
}

Write-Host "Plugins folder content ($OutputDir):"
Get-ChildItem $OutputDir | ForEach-Object { Write-Host "  $($_.Name)" }
