# Test if Doppler properly injects DATABASE_CONNECTIONSTRING
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Doppler Environment Injection" -ForegroundColor Cyan  
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "1. Testing Doppler secrets command..." -ForegroundColor Yellow
$secretValue = doppler secrets get DATABASE_CONNECTIONSTRING --plain 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "[FAIL] Doppler command failed! Are you in the correct directory?" -ForegroundColor Red
    Write-Host "Error: $secretValue" -ForegroundColor Red
    exit 1
}

if ($secretValue -match 'Password=') {
    Write-Host "[OK] Doppler secret contains Password field" -ForegroundColor Green
} else {
    Write-Host "[FAIL] Doppler secret MISSING Password field!" -ForegroundColor Red
    exit 1
}

Write-Host "`n2. Testing Doppler run environment injection..." -ForegroundColor Yellow
$testResult = doppler run -- powershell -NoProfile -Command {
    if ($env:DATABASE_CONNECTIONSTRING -match 'Password=') {
        Write-Output "PASSWORD_PRESENT"
    } else {
        Write-Output "PASSWORD_MISSING"
    }
}

if ($testResult -eq "PASSWORD_PRESENT") {
    Write-Host "[OK] Doppler run properly injects PASSWORD into environment" -ForegroundColor Green
} else {
    Write-Host "[FAIL] Doppler run does NOT inject PASSWORD into environment!" -ForegroundColor Red
    Write-Host "This means AppHost will NOT see the password!" -ForegroundColor Red
   exit 1
}

Write-Host "`n3. Checking for conflicting .env files..." -ForegroundColor Yellow
if (Test-Path ".env") {
    Write-Host "[WARNING] Found .env file - it may override Doppler!" -ForegroundColor Yellow
    $envContent = Get-Content ".env" | Select-String "DATABASE_CONNECTIONSTRING"
    if ($envContent) {
        Write-Host "[ISSUE] .env contains DATABASE_CONNECTIONSTRING:" -ForegroundColor Red
        Write-Host $envContent -ForegroundColor Red
        Write-Host "Remove or rename .env file to let Doppler take precedence!" -ForegroundColor Red
    }
} else {
    Write-Host "[OK] No .env file found" -ForegroundColor Green
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "All checks PASSED! Doppler injection works." -ForegroundColor Green
Write-Host "If AppHost still doesn't work, it's a code issue." -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
