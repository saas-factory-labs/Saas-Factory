# Create development certificates for HTTPS
Write-Host "Setting up development certificates for AppBlueprint..." -ForegroundColor Cyan

# Clean any existing dev certs and create a new trusted one
dotnet dev-certs https --clean
dotnet dev-certs https --trust

# Create directory for certificates if it doesn't exist
$certPath = "$env:APPDATA\ASP.NET\Https"
if (-not (Test-Path $certPath)) {
    New-Item -Path $certPath -ItemType Directory -Force | Out-Null
    Write-Host "Created certificate directory: $certPath" -ForegroundColor Green
}

# Generate certificates for each service with explicit password
$password = "dev-cert-password"

# Create certificates
dotnet dev-certs https -ep "$certPath\web-service.pfx" -p $password
dotnet dev-certs https -ep "$certPath\api-service.pfx" -p $password
dotnet dev-certs https -ep "$certPath\app-gateway.pfx" -p $password

Write-Host "Development certificates have been created and trusted." -ForegroundColor Green
Write-Host "Certificate location: $certPath" -ForegroundColor Green
Write-Host "Certificate password: $password" -ForegroundColor Yellow

# Display certificate information
Write-Host "`nTo fix any remaining certificate issues:" -ForegroundColor Cyan
Write-Host "1. Make sure your app is using the certificates at: $certPath" -ForegroundColor White
Write-Host "2. Verify that ASPNETCORE_Kestrel__Certificates__Default__Path is set in environment variables" -ForegroundColor White
Write-Host "3. Verify that ASPNETCORE_Kestrel__Certificates__Default__Password is set to the certificate password" -ForegroundColor White