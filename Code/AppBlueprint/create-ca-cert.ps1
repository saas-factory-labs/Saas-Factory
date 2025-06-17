param(
    [string]$OutputPath = "$env:TEMP\localhost-ca.crt"
)

Write-Host "Creating a proper CA certificate for Firefox..." -ForegroundColor Green

# Create OpenSSL config for CA
$configContent = @"
[req]
distinguished_name = req_distinguished_name
x509_extensions = v3_ca
prompt = no

[req_distinguished_name]
C = US
ST = Development
L = Local
O = ASP.NET Core Development
OU = Development
CN = ASP.NET Core Development CA

[v3_ca]
basicConstraints = CA:TRUE
keyUsage = keyCertSign, cRLSign
subjectKeyIdentifier = hash
authorityKeyIdentifier = keyid,issuer
"@

$configPath = "$env:TEMP\ca.conf"
$configContent | Out-File -FilePath $configPath -Encoding ASCII

# Create the CA certificate using OpenSSL if available, otherwise use PowerShell
try {
    $cert = New-SelfSignedCertificate -Subject "CN=ASP.NET Core Development CA" -KeyUsage CertSign -CertStoreLocation "Cert:\LocalMachine\My" -NotAfter (Get-Date).AddYears(10) -Type Custom -KeyExportPolicy Exportable -KeySpec Signature

    # Export to file
    Export-Certificate -Cert $cert -FilePath $OutputPath -Type CERT
    
    Write-Host "CA certificate created at: $OutputPath" -ForegroundColor Green
    Write-Host ""
    Write-Host "FIREFOX IMPORT INSTRUCTIONS:" -ForegroundColor Cyan
    Write-Host "1. Open Firefox" -ForegroundColor White
    Write-Host "2. Go to about:preferences" -ForegroundColor White
    Write-Host "3. Search for 'certificates'" -ForegroundColor White
    Write-Host "4. Click 'View Certificates'" -ForegroundColor White
    Write-Host "5. Go to 'Authorities' tab" -ForegroundColor White
    Write-Host "6. Click 'Import'" -ForegroundColor White
    Write-Host "7. Select: $OutputPath" -ForegroundColor Yellow
    Write-Host "8. Check 'Trust this CA to identify websites'" -ForegroundColor White
    Write-Host "9. Click OK and restart Firefox" -ForegroundColor White
} catch {
    Write-Host "Could not create CA certificate: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "ALTERNATIVE: Accept the risk in Firefox" -ForegroundColor Yellow
    Write-Host "1. Go to https://localhost/login in Firefox" -ForegroundColor White
    Write-Host "2. Click 'Advanced' on the security warning" -ForegroundColor White
    Write-Host "3. Click 'Accept the Risk and Continue'" -ForegroundColor White
}
