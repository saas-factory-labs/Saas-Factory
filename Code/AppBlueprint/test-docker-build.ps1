# Docker Build Test Script
# This script tests building the AppBlueprint Docker images locally

Write-Host "=== AppBlueprint Docker Build Test ===" -ForegroundColor Cyan
Write-Host ""

# Change to repository root
$repoRoot = "C:\Development\Development-Projects\saas-factory-labs"
Set-Location $repoRoot

Write-Host "Repository Root: $repoRoot" -ForegroundColor Yellow
Write-Host "Build Context: Code\AppBlueprint" -ForegroundColor Yellow
Write-Host ""

# Build API Service
Write-Host "=== Building API Service ===" -ForegroundColor Cyan
Write-Host "Command: docker build -f Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile -t appblueprint-api-test Code/AppBlueprint" -ForegroundColor Gray
Write-Host ""

docker build -f Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile -t appblueprint-api-test Code/AppBlueprint

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ API Service build successful!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "❌ API Service build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== Building Web Service ===" -ForegroundColor Cyan
Write-Host "Command: docker build -f Code/AppBlueprint/AppBlueprint.Web/Dockerfile -t appblueprint-web-test Code/AppBlueprint" -ForegroundColor Gray
Write-Host ""

docker build -f Code/AppBlueprint/AppBlueprint.Web/Dockerfile -t appblueprint-web-test Code/AppBlueprint

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ Web Service build successful!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "❌ Web Service build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== Build Summary ===" -ForegroundColor Cyan
Write-Host "✅ All builds completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Images created:" -ForegroundColor Yellow
Write-Host "  - appblueprint-api-test" -ForegroundColor White
Write-Host "  - appblueprint-web-test" -ForegroundColor White
Write-Host ""
Write-Host "To run the images:" -ForegroundColor Yellow
Write-Host "  docker run -p 8080:80 appblueprint-api-test" -ForegroundColor Gray
Write-Host "  docker run -p 8081:80 appblueprint-web-test" -ForegroundColor Gray
Write-Host ""

