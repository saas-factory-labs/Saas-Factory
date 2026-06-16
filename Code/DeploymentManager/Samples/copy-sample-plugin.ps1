# Flow 1 (local development) dress rehearsal:
# builds the sample admin plugin and copies dll+pdb to the shared local-plugins folder
# (sibling of the repo root by default, e.g. c:\SaaS-Udvikling\local-plugins).
# Private app repos do the same via a post-build target - see the kernel README.
#
# Usage: .\copy-sample-plugin.ps1 [-Destination <folder>] [-Configuration Debug|Release]
param(
    [string]$Destination = (Join-Path (Resolve-Path "$PSScriptRoot\..\..\..\..") 'local-plugins'),
    [string]$Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'

$projectDir = Join-Path $PSScriptRoot 'SaaSFactory.Sample.Admin'

Write-Host "Building $projectDir ($Configuration)..."
dotnet build $projectDir --configuration $Configuration --nologo -v q
if ($LASTEXITCODE -ne 0) {
    throw "Build failed with exit code $LASTEXITCODE"
}

$outputDir = Join-Path $projectDir "bin\$Configuration\net10.0"

New-Item -ItemType Directory -Force -Path $Destination | Out-Null
Copy-Item (Join-Path $outputDir 'SaaSFactory.Sample.Admin.dll') $Destination -Force
Copy-Item (Join-Path $outputDir 'SaaSFactory.Sample.Admin.pdb') $Destination -Force

Write-Host "Copied SaaSFactory.Sample.Admin.dll + .pdb to $Destination"
Write-Host "Set AdminPortal:PluginsPath to that folder (DeploymentManager.Web appsettings.Development.json does this by default)"
Write-Host "and configure AdminPortal:Modules:sample:ConnectionString, then run DeploymentManager.Web with 'dotnet watch'."
