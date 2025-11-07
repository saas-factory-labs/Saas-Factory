# Railway Setup Script for AppBlueprint
# This script helps configure Railway deployment for the AppBlueprint project

Write-Host "=== Railway Setup for AppBlueprint ===" -ForegroundColor Cyan
Write-Host ""

# Check if Railway CLI is installed
function Test-RailwayCLI {
    try {
        $version = railway --version 2>$null
        if ($version) {
            Write-Host "✓ Railway CLI is installed: $version" -ForegroundColor Green
            return $true
        }
    }
    catch {
        Write-Host "✗ Railway CLI is not installed" -ForegroundColor Red
        return $false
    }
}

# Install Railway CLI
function Install-RailwayCLI {
    Write-Host "Installing Railway CLI..." -ForegroundColor Yellow
    try {
        npm install -g @railway/cli
        Write-Host "✓ Railway CLI installed successfully" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Failed to install Railway CLI" -ForegroundColor Red
        Write-Host "Please install manually: npm install -g @railway/cli" -ForegroundColor Yellow
        exit 1
    }
}

# Check Railway CLI
if (-not (Test-RailwayCLI)) {
    $install = Read-Host "Would you like to install Railway CLI? (y/n)"
    if ($install -eq 'y') {
        Install-RailwayCLI
    }
    else {
        Write-Host "Please install Railway CLI manually and run this script again." -ForegroundColor Yellow
        exit 1
    }
}

Write-Host ""
Write-Host "=== Railway Login ===" -ForegroundColor Cyan
Write-Host "Please log in to Railway (this will open your browser)" -ForegroundColor Yellow
railway login

Write-Host ""
Write-Host "=== Railway Project Setup ===" -ForegroundColor Cyan
$createNew = Read-Host "Create new Railway project? (y/n)"

if ($createNew -eq 'y') {
    Write-Host "Initializing Railway project..." -ForegroundColor Yellow
    railway init
    
    Write-Host ""
    Write-Host "=== Environment Setup ===" -ForegroundColor Cyan
    
    # Create staging environment
    Write-Host "Creating staging environment..." -ForegroundColor Yellow
    railway environment create staging
    
    # Create production environment
    Write-Host "Creating production environment..." -ForegroundColor Yellow
    railway environment create production
    
    Write-Host "✓ Environments created successfully" -ForegroundColor Green
}

Write-Host ""
Write-Host "=== Service Configuration ===" -ForegroundColor Cyan
Write-Host "Select environment to configure:" -ForegroundColor Yellow
Write-Host "1. Staging"
Write-Host "2. Production"
$envChoice = Read-Host "Enter choice (1 or 2)"

$environment = if ($envChoice -eq "1") { "staging" } else { "production" }

Write-Host ""
Write-Host "Configuring $environment environment..." -ForegroundColor Yellow

# Set environment
railway environment set $environment

Write-Host ""
Write-Host "=== Database Setup ===" -ForegroundColor Cyan
$addDb = Read-Host "Add PostgreSQL database to this environment? (y/n)"

if ($addDb -eq 'y') {
    Write-Host "Adding PostgreSQL..." -ForegroundColor Yellow
    railway add --database postgresql
    Write-Host "✓ PostgreSQL added successfully" -ForegroundColor Green
}

Write-Host ""
Write-Host "=== API Service Configuration ===" -ForegroundColor Cyan
Write-Host "Setting environment variables for API service..." -ForegroundColor Yellow

# Get Railway project info
$projectInfo = railway status

Write-Host ""
Write-Host "Please configure the following environment variables in Railway dashboard:" -ForegroundColor Yellow
Write-Host ""
Write-Host "For API Service:" -ForegroundColor Cyan
Write-Host "  RAILWAY_DOCKERFILE_PATH=AppBlueprint.ApiService/Dockerfile"
Write-Host "  ASPNETCORE_ENVIRONMENT=$($environment -eq 'staging' ? 'Staging' : 'Production')"
Write-Host "  ASPNETCORE_URLS=http://+:`${PORT}"
Write-Host "  DATABASE_CONNECTION_STRING=`${DATABASE_URL}"
Write-Host "  ASPNETCORE_FORWARDEDHEADERS_ENABLED=true"
Write-Host "  DOTNET_ENVIRONMENT=$($environment -eq 'staging' ? 'Staging' : 'Production')"
Write-Host ""
Write-Host "For Web Service:" -ForegroundColor Cyan
Write-Host "  RAILWAY_DOCKERFILE_PATH=AppBlueprint.Web/Dockerfile"
Write-Host "  ASPNETCORE_ENVIRONMENT=$($environment -eq 'staging' ? 'Staging' : 'Production')"
Write-Host "  ASPNETCORE_URLS=http://+:`${PORT}"
Write-Host "  ASPNETCORE_FORWARDEDHEADERS_ENABLED=true"
Write-Host "  ApiService__BaseUrl=<API_SERVICE_URL>"
Write-Host "  DOTNET_ENVIRONMENT=$($environment -eq 'staging' ? 'Staging' : 'Production')"
Write-Host ""

Write-Host "=== GitHub Secrets Setup ===" -ForegroundColor Cyan
Write-Host "Creating Railway API token..." -ForegroundColor Yellow
Write-Host ""
Write-Host "To create a Railway API token:" -ForegroundColor Yellow
Write-Host "1. Go to Railway dashboard: https://railway.app/account/tokens"
Write-Host "2. Click 'Create Token'"
Write-Host "3. Give it a name (e.g., 'GitHub Actions $environment')"
Write-Host "4. Copy the token"
Write-Host ""
Write-Host "Add the following secrets to your GitHub repository:" -ForegroundColor Cyan
Write-Host "  RAILWAY_TOKEN_$(($environment).ToUpper())=<your-token>"
Write-Host "  RAILWAY_DATABASE_URL_$(($environment).ToUpper())=<database-connection-string>"
Write-Host ""

# Get database URL
Write-Host "=== Getting Database Connection String ===" -ForegroundColor Cyan
$getDbUrl = Read-Host "Retrieve database URL from Railway? (y/n)"

if ($getDbUrl -eq 'y') {
    try {
        Write-Host "Fetching database URL..." -ForegroundColor Yellow
        $dbUrl = railway variables get DATABASE_URL --service postgres --environment $environment 2>$null
        if ($dbUrl) {
            Write-Host "✓ Database URL retrieved" -ForegroundColor Green
            Write-Host ""
            Write-Host "Add this to GitHub Secrets as RAILWAY_DATABASE_URL_$(($environment).ToUpper()):" -ForegroundColor Yellow
            Write-Host $dbUrl
        }
        else {
            Write-Host "✗ Could not retrieve database URL. Please get it from Railway dashboard." -ForegroundColor Red
        }
    }
    catch {
        Write-Host "✗ Error retrieving database URL: $_" -ForegroundColor Red
        Write-Host "Please get it manually from Railway dashboard." -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=== Next Steps ===" -ForegroundColor Cyan
Write-Host "1. Configure environment variables in Railway dashboard"
Write-Host "2. Add GitHub secrets (RAILWAY_TOKEN_*, RAILWAY_DATABASE_URL_*)"
Write-Host "3. Set up service-specific settings in Railway:"
Write-Host "   - API Service: Set root directory to 'Code/AppBlueprint'"
Write-Host "   - Web Service: Set root directory to 'Code/AppBlueprint'"
Write-Host "4. Configure domains for your services (optional)"
Write-Host "5. Run GitHub Actions workflow to deploy"
Write-Host ""
Write-Host "=== Setup Complete! ===" -ForegroundColor Green
Write-Host "Railway project is configured for $environment environment" -ForegroundColor Green
Write-Host ""
Write-Host "For more information, see: Code/AppBlueprint/RAILWAY_DEPLOYMENT.md" -ForegroundColor Cyan

