# JWT Authentication Test Script
# This script tests JWT authentication on the API Service

param(
    [string]$ApiBaseUrl = "https://localhost:5002",
    [string]$SecretKey = "YourSuperSecretKey_ChangeThisInProduction_MustBeAtLeast32Characters!",
    [string]$Issuer = "AppBlueprintAPI",
    [string]$Audience = "AppBlueprintClient"
)

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "JWT Authentication Test Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Function to generate JWT token
function New-JwtToken {
    param(
        [string]$UserId = "test-user-123",
        [string]$UserName = "Test User",
        [string]$Email = "test@example.com",
        [string[]]$Roles = @("User")
    )
    
    Write-Host "⚠️  PowerShell cannot generate JWT tokens natively." -ForegroundColor Yellow
    Write-Host "   Please use one of these methods:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "   Option 1: Go to https://jwt.io/" -ForegroundColor White
    Write-Host "   Option 2: Use the C# token generator (see below)" -ForegroundColor White
    Write-Host "   Option 3: Get a real token from Auth0/Logto" -ForegroundColor White
    Write-Host ""
    
    # Show the payload to use
    $payload = @{
        sub = $UserId
        name = $UserName
        email = $Email
        role = $Roles
        nbf = [int][double]::Parse((Get-Date -Date "01/01/1970").AddSeconds(0).ToString())
        exp = [int][double]::Parse((Get-Date).AddHours(24).ToString("yyyyMMddHHmmss"))
        iss = $Issuer
        aud = $Audience
    }
    
    Write-Host "Payload to use at jwt.io:" -ForegroundColor Green
    Write-Host ($payload | ConvertTo-Json) -ForegroundColor Gray
    Write-Host ""
    Write-Host "Secret Key to use:" -ForegroundColor Green
    Write-Host $SecretKey -ForegroundColor Gray
    Write-Host ""
    
    return $null
}

# Function to test endpoint
function Test-Endpoint {
    param(
        [string]$Url,
        [string]$Token = "",
        [string]$Description
    )
    
    Write-Host "Testing: $Description" -ForegroundColor Cyan
    Write-Host "URL: $Url" -ForegroundColor Gray
    
    try {
        $headers = @{}
        if ($Token) {
            $headers["Authorization"] = "Bearer $Token"
            Write-Host "Auth: Using Bearer token" -ForegroundColor Gray
        } else {
            Write-Host "Auth: No token (anonymous)" -ForegroundColor Gray
        }
        
        # Ignore SSL certificate errors for local testing
        if ($PSVersionTable.PSVersion.Major -ge 6) {
            $response = Invoke-RestMethod -Uri $Url -Method Get -Headers $headers -SkipCertificateCheck
        } else {
            # PowerShell 5.1 workaround
            add-type @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public class TrustAllCertsPolicy : ICertificatePolicy {
    public bool CheckValidationResult(
        ServicePoint srvPoint, X509Certificate certificate,
        WebRequest request, int certificateProblem) {
        return true;
    }
}
"@
            [System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
            [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
            
            $response = Invoke-RestMethod -Uri $Url -Method Get -Headers $headers
        }
        
        Write-Host "✅ SUCCESS (200 OK)" -ForegroundColor Green
        Write-Host "Response:" -ForegroundColor Gray
        Write-Host ($response | ConvertTo-Json -Depth 3) -ForegroundColor Gray
        
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -eq 401) {
            Write-Host "🔒 UNAUTHORIZED (401)" -ForegroundColor Yellow
            Write-Host "   This is expected if no valid token was provided" -ForegroundColor Gray
        } elseif ($statusCode -eq 403) {
            Write-Host "🚫 FORBIDDEN (403)" -ForegroundColor Yellow
            Write-Host "   Token is valid but lacks required permissions" -ForegroundColor Gray
        } else {
            Write-Host "❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    
    Write-Host ""
}

# Main execution
Write-Host "Step 1: Testing Public Endpoint (No Auth Required)" -ForegroundColor Magenta
Write-Host "---------------------------------------------------" -ForegroundColor Magenta
Test-Endpoint -Url "$ApiBaseUrl/api/v1/authtest/public" -Description "Public endpoint without token"
Start-Sleep -Seconds 1

Write-Host "Step 2: Testing Protected Endpoint Without Token" -ForegroundColor Magenta
Write-Host "---------------------------------------------------" -ForegroundColor Magenta
Test-Endpoint -Url "$ApiBaseUrl/api/v1/authtest/protected" -Description "Protected endpoint without token (should fail)"
Start-Sleep -Seconds 1

Write-Host "Step 3: Generate Test Token" -ForegroundColor Magenta
Write-Host "---------------------------------------------------" -ForegroundColor Magenta
$token = New-JwtToken -UserName "Test User" -Email "test@example.com" -Roles @("User")

if (-not $token) {
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Yellow
    Write-Host "⚠️  MANUAL STEP REQUIRED" -ForegroundColor Yellow
    Write-Host "================================================" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Generate a JWT token using the information above, then run:" -ForegroundColor White
    Write-Host ""
    Write-Host '  $token = "your-generated-token-here"' -ForegroundColor Cyan
    Write-Host "  Test-Endpoint -Url '$ApiBaseUrl/api/v1/authtest/protected' -Token `$token -Description 'Protected endpoint with token'" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Or use the C# token generator:" -ForegroundColor White
    Write-Host "  cd Code\AppBlueprint" -ForegroundColor Cyan
    Write-Host "  dotnet run --project JwtTokenGenerator" -ForegroundColor Cyan
    Write-Host ""
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Quick Test Commands" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "# Test public endpoint:" -ForegroundColor Green
Write-Host "Invoke-RestMethod -Uri '$ApiBaseUrl/api/v1/authtest/public' -SkipCertificateCheck" -ForegroundColor Gray
Write-Host ""
Write-Host "# Test protected endpoint (will fail):" -ForegroundColor Green
Write-Host "Invoke-RestMethod -Uri '$ApiBaseUrl/api/v1/authtest/protected' -SkipCertificateCheck" -ForegroundColor Gray
Write-Host ""
Write-Host "# Test with token:" -ForegroundColor Green
Write-Host '$token = "your-jwt-token-here"' -ForegroundColor Gray
Write-Host 'Invoke-RestMethod -Uri "' + $ApiBaseUrl + '/api/v1/authtest/protected" -Headers @{Authorization="Bearer $token"} -SkipCertificateCheck' -ForegroundColor Gray
Write-Host ""

