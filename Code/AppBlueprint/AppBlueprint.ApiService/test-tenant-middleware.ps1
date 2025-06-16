#!/usr/bin/env pwsh

# Test script to verify that the TenantMiddleware excludes Swagger UI paths

Write-Host "Testing TenantMiddleware exclusions..." -ForegroundColor Green

# Test paths that should be excluded (should return 200 or 404, not 400)
$excludedPaths = @(
    "/swagger",
    "/swagger/index.html", 
    "/swagger/v1/swagger.json",
    "/health",
    "/metrics"
)

# Test paths that should require tenant-id (should return 400)
$protectedPaths = @(
    "/api/users",
    "/api/tenants",
    "/api/authentication/login"
)

Write-Host "`nTesting excluded paths (should NOT require tenant-id):" -ForegroundColor Yellow
foreach ($path in $excludedPaths) {    try {
        Write-Host "Testing: $path" -NoNewline
        $response = Invoke-WebRequest -Uri "http://localhost:8090$path" -Method GET -ErrorAction SilentlyContinue
        Write-Host " -> Status: $($response.StatusCode)" -ForegroundColor Green
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 400) {
            Write-Host " -> Status: 400 (BAD - still requires tenant-id!)" -ForegroundColor Red
        } else {
            Write-Host " -> Status: $statusCode (OK - not requiring tenant-id)" -ForegroundColor Green
        }
    }
}

Write-Host "`nTesting protected paths (should require tenant-id):" -ForegroundColor Yellow
foreach ($path in $protectedPaths) {    try {
        Write-Host "Testing: $path" -NoNewline
        $response = Invoke-WebRequest -Uri "http://localhost:8090$path" -Method GET -ErrorAction SilentlyContinue
        Write-Host " -> Status: $($response.StatusCode) (BAD - should require tenant-id!)" -ForegroundColor Red
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 400) {
            Write-Host " -> Status: 400 (OK - correctly requires tenant-id)" -ForegroundColor Green
        } else {
            Write-Host " -> Status: $statusCode" -ForegroundColor Yellow
        }
    }
}

Write-Host "`nTest completed!" -ForegroundColor Green
