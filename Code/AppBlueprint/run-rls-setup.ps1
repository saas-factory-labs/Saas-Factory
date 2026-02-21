# Run Row-Level Security Setup SQL
# This script helps you run the RLS setup on your local database

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Row-Level Security Setup for AppBlueprint" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if psql is available
$psqlPath = Get-Command psql -ErrorAction SilentlyContinue

if ($psqlPath) {
    Write-Host "✓ psql found at: $($psqlPath.Source)" -ForegroundColor Green
    Write-Host ""
    Write-Host "Please enter your PostgreSQL connection details:" -ForegroundColor Yellow
    Write-Host "(You can find these in Aspire Dashboard > postgres-server resource)" -ForegroundColor Gray
    Write-Host ""
    
    $host_input = Read-Host "Host (default: localhost)"
    $port_input = Read-Host "Port (default: 5432)"
    $user_input = Read-Host "Username (default: postgres)"
    $db_input = Read-Host "Database (default: appblueprintdb)"
    $password = Read-Host "Password" -AsSecureString
    
    # Set defaults
    $host = if ($host_input) { $host_input } else { "localhost" }
    $port = if ($port_input) { $port_input } else { "5432" }
    $user = if ($user_input) { $user_input } else { "postgres" }
    $db = if ($db_input) { $db_input } else { "appblueprintdb" }
    
    # Convert SecureString to plain text for environment variable
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
    $plainPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
    
    Write-Host ""
    Write-Host "Running RLS setup SQL..." -ForegroundColor Yellow
    
    # Set password environment variable and run psql
    $env:PGPASSWORD = $plainPassword
    
    try {
        psql -h $host -p $port -U $user -d $db -f "SetupRowLevelSecurity.sql"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "========================================" -ForegroundColor Green
            Write-Host "✓ RLS Setup Complete!" -ForegroundColor Green
            Write-Host "========================================" -ForegroundColor Green
            Write-Host ""
            Write-Host "Next steps:" -ForegroundColor Yellow
            Write-Host "1. Restart your AppHost (Ctrl+C and run again)" -ForegroundColor White
            Write-Host "2. Check health endpoint: http://localhost:9100/health" -ForegroundColor White
            Write-Host "3. All checks should be Healthy ✓" -ForegroundColor White
        } else {
            Write-Host ""
            Write-Host "❌ Error running SQL script" -ForegroundColor Red
            Write-Host "Exit code: $LASTEXITCODE" -ForegroundColor Red
        }
    } finally {
        # Clear password from environment
        $env:PGPASSWORD = $null
        [System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($BSTR)
    }
    
} else {
    Write-Host "❌ psql not found in PATH" -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative options:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Option 1: Install PostgreSQL client tools" -ForegroundColor White
    Write-Host "  Scoop: scoop install postgresql" -ForegroundColor Gray
    Write-Host "  Chocolatey: choco install postgresql" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Option 2: Use a GUI tool (pgAdmin, DBeaver, DataGrip)" -ForegroundColor White
    Write-Host "  1. Get connection details from Aspire Dashboard" -ForegroundColor Gray
    Write-Host "  2. Open SetupRowLevelSecurity.sql" -ForegroundColor Gray
    Write-Host "  3. Copy all contents and run in query window" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Option 3: Exec into Docker container" -ForegroundColor White
    Write-Host "  docker exec -it <container-name> psql -U postgres -d appblueprintdb -f /path/to/sql" -ForegroundColor Gray
}# Run Row-Level Security Setup SQL
# This script helps you run the RLS setup on your local database

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Row-Level Security Setup for AppBlueprint" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if psql is available
$psqlPath = Get-Command psql -ErrorAction SilentlyContinue

if ($psqlPath) {
    Write-Host "✓ psql found at: $($psqlPath.Source)" -ForegroundColor Green
    Write-Host ""
    Write-Host "Please enter your PostgreSQL connection details:" -ForegroundColor Yellow
    Write-Host "(You can find these in Aspire Dashboard > postgres-server resource)" -ForegroundColor Gray
    Write-Host ""
    
    $host_input = Read-Host "Host (default: localhost)"
    $port_input = Read-Host "Port (default: 5432)"
    $user_input = Read-Host "Username (default: postgres)"
    $db_input = Read-Host "Database (default: appblueprintdb)"
    $password = Read-Host "Password" -AsSecureString
    
    # Set defaults
    $host = if ($host_input) { $host_input } else { "localhost" }
    $port = if ($port_input) { $port_input } else { "5432" }
    $user = if ($user_input) { $user_input } else { "postgres" }
    $db = if ($db_input) { $db_input } else { "appblueprintdb" }
    
    # Convert SecureString to plain text for environment variable
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
    $plainPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
    
    Write-Host ""
    Write-Host "Running RLS setup SQL..." -ForegroundColor Yellow
    
    # Set password environment variable and run psql
    $env:PGPASSWORD = $plainPassword
    
    try {
        psql -h $host -p $port -U $user -d $db -f "SetupRowLevelSecurity.sql"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "========================================" -ForegroundColor Green
            Write-Host "✓ RLS Setup Complete!" -ForegroundColor Green
            Write-Host "========================================" -ForegroundColor Green
            Write-Host ""
            Write-Host "Next steps:" -ForegroundColor Yellow
            Write-Host "1. Restart your AppHost (Ctrl+C and run again)" -ForegroundColor White
            Write-Host "2. Check health endpoint: http://localhost:9100/health" -ForegroundColor White
            Write-Host "3. All checks should be Healthy ✓" -ForegroundColor White
        } else {
            Write-Host ""
            Write-Host "❌ Error running SQL script" -ForegroundColor Red
            Write-Host "Exit code: $LASTEXITCODE" -ForegroundColor Red
        }
    } finally {
        # Clear password from environment
        $env:PGPASSWORD = $null
        [System.Runtime.InteropServices.Marshal]::ZeroFreeBSTR($BSTR)
    }
    
} else {
    Write-Host "❌ psql not found in PATH" -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative options:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Option 1: Install PostgreSQL client tools" -ForegroundColor White
    Write-Host "  Scoop: scoop install postgresql" -ForegroundColor Gray
    Write-Host "  Chocolatey: choco install postgresql" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Option 2: Use a GUI tool (pgAdmin, DBeaver, DataGrip)" -ForegroundColor White
    Write-Host "  1. Get connection details from Aspire Dashboard" -ForegroundColor Gray
    Write-Host "  2. Open SetupRowLevelSecurity.sql" -ForegroundColor Gray
    Write-Host "  3. Copy all contents and run in query window" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Option 3: Exec into Docker container" -ForegroundColor White
    Write-Host "Option 3: Exec into Docker container" -ForegroundColor White
