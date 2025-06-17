param(
    [string]$CertPath = "$env:APPDATA\ASP.NET\Https\web-service.pfx",
    [string]$Password = "dev-cert-password"
)

Write-Host "Fixing Firefox Certificate Trust Issues..." -ForegroundColor Green
Write-Host "=======================================" -ForegroundColor Green

# First, clean up any existing dev certificates
Write-Host "1. Cleaning existing ASP.NET Core dev certificates..." -ForegroundColor Yellow
try {
    dotnet dev-certs https --clean
    Write-Host "   ✓ Cleaned existing certificates" -ForegroundColor Green
} catch {
    Write-Host "   ⚠ Error cleaning certificates: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Create a new trusted certificate
Write-Host "2. Creating new ASP.NET Core dev certificate..." -ForegroundColor Yellow
try {
    dotnet dev-certs https --trust
    Write-Host "   ✓ Created and trusted new certificate" -ForegroundColor Green
} catch {
    Write-Host "   ⚠ Error creating certificate: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Export the certificate for manual import
$ExportPath = Join-Path $env:TEMP "aspnet-dev-cert.crt"
Write-Host "3. Exporting certificate to $ExportPath..." -ForegroundColor Yellow
try {
    dotnet dev-certs https --export-path $ExportPath --format Pem
    Write-Host "   ✓ Certificate exported" -ForegroundColor Green
    
    if (Test-Path $ExportPath) {
        Write-Host ""
        Write-Host "MANUAL STEPS FOR FIREFOX:" -ForegroundColor Cyan
        Write-Host "=========================" -ForegroundColor Cyan
        Write-Host "1. Open Firefox" -ForegroundColor White
        Write-Host "2. Go to Settings (about:preferences)" -ForegroundColor White
        Write-Host "3. Search for 'certificates'" -ForegroundColor White
        Write-Host "4. Click 'View Certificates'" -ForegroundColor White
        Write-Host "5. Go to 'Authorities' tab" -ForegroundColor White
        Write-Host "6. Click 'Import'" -ForegroundColor White
        Write-Host "7. Select this file: $ExportPath" -ForegroundColor Yellow
        Write-Host "8. Check 'Trust this CA to identify websites'" -ForegroundColor White
        Write-Host "9. Click OK" -ForegroundColor White
        Write-Host "10. Restart Firefox" -ForegroundColor White
        Write-Host ""
    }
} catch {
    Write-Host "   ⚠ Error exporting certificate: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Alternative: Try to add to Windows certificate store more aggressively
Write-Host "4. Adding certificate to Windows Root Certificate Store..." -ForegroundColor Yellow
try {
    $tempCert = Join-Path $env:TEMP "aspnet-dev-cert-root.crt"
    dotnet dev-certs https --export-path $tempCert --format Pem
    
    if (Test-Path $tempCert) {
        # Import to root store
        certutil -addstore -enterprise -f "Root" $tempCert
        certutil -addstore -user -f "Root" $tempCert
        Write-Host "   ✓ Certificate added to Windows Root store" -ForegroundColor Green
        Remove-Item $tempCert -Force
    }
} catch {
    Write-Host "   ⚠ Error adding to root store: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Certificate setup complete!" -ForegroundColor Green
Write-Host "If Firefox still shows warnings, try the manual import steps above." -ForegroundColor Yellow
