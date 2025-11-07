# Railway Deployment Quick Start

## Prerequisites

1. **Railway Account**: Sign up at https://railway.app
2. **Railway CLI**: Install via `npm install -g @railway/cli`
3. **GitHub Repository**: Connected to Railway project

## Initial Setup (One-time)

### 1. Run Setup Script

```powershell
cd Code\AppBlueprint
.\setup-railway.ps1
```

This script will:
- Install Railway CLI (if needed)
- Log you into Railway
- Create staging/production environments
- Add PostgreSQL database
- Guide you through configuration

### 2. Configure GitHub Secrets

Add these secrets to your GitHub repository (Settings → Secrets and variables → Actions):

#### Staging
- `RAILWAY_TOKEN_STAGING`: Railway API token for staging
- `RAILWAY_DATABASE_URL_STAGING`: PostgreSQL connection string from Railway

#### Production
- `RAILWAY_TOKEN_PRODUCTION`: Railway API token for production
- `RAILWAY_DATABASE_URL_PRODUCTION`: PostgreSQL connection string from Railway

### 3. Configure Railway Services

In Railway dashboard, for each service (api-service, web-service):

#### API Service Settings
```
Root Directory: Code/AppBlueprint
RAILWAY_DOCKERFILE_PATH: AppBlueprint.ApiService/Dockerfile
ASPNETCORE_URLS: http://+:${PORT}
DATABASE_CONNECTION_STRING: ${DATABASE_URL}
ASPNETCORE_FORWARDEDHEADERS_ENABLED: true
ASPNETCORE_ENVIRONMENT: Staging (or Production)
DOTNET_ENVIRONMENT: Staging (or Production)
```

#### Web Service Settings
```
Root Directory: Code/AppBlueprint
RAILWAY_DOCKERFILE_PATH: AppBlueprint.Web/Dockerfile
ASPNETCORE_URLS: http://+:${PORT}
ASPNETCORE_FORWARDEDHEADERS_ENABLED: true
ApiService__BaseUrl: ${API_SERVICE_URL}
ASPNETCORE_ENVIRONMENT: Staging (or Production)
DOTNET_ENVIRONMENT: Staging (or Production)
```

## Deployment Methods

### Method 1: Automatic Deployment (Staging)

Push to `main` branch:
```bash
git add .
git commit -m "Your changes"
git push origin main
```

This automatically:
1. Builds and tests the application
2. Deploys API service to Railway staging
3. Deploys Web service to Railway staging
4. Runs database migrations

### Method 2: Manual Deployment (Production)

1. Go to GitHub repository → Actions tab
2. Select "Deploy to Railway" workflow
3. Click "Run workflow"
4. Select:
   - Environment: `production`
   - Service: `all` (or specific service)
5. Click "Run workflow"

### Method 3: CLI Deployment

Deploy directly from command line:

```powershell
# Login to Railway
railway login

# Set environment
railway environment set staging  # or production

# Deploy API service
cd Code\AppBlueprint
railway up --service api-service

# Deploy Web service
railway up --service web-service
```

## Database Migrations

### Automatic (via GitHub Actions)
Migrations run automatically after deployment.

### Manual Migration
```powershell
# Set connection string
$env:DATABASE_CONNECTION_STRING = "your-railway-database-url"

# Navigate to AppBlueprint
cd Code\AppBlueprint

# Run migration
dotnet ef database update `
  --project Shared-Modules\AppBlueprint.Infrastructure `
  --startup-project AppBlueprint.ApiService `
  --connection $env:DATABASE_CONNECTION_STRING
```

## Monitoring

### View Logs
```powershell
railway logs --service api-service --environment staging
```

Or view in Railway dashboard: https://railway.app/dashboard

### Check Deployment Status
```powershell
railway status
```

### Get Service URL
```powershell
railway domain --service api-service
```

## Common Commands

```powershell
# List environments
railway environment list

# Switch environment
railway environment set staging

# List services
railway service list

# View environment variables
railway variables

# Set environment variable
railway variables set KEY=value --service api-service

# Open Railway dashboard
railway open
```

## Local Docker Build Testing

Before deploying to Railway, test your Docker builds locally:

```powershell
# From repository root
cd C:\Development\Development-Projects\saas-factory-labs

# Build API Service
docker build -f Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile -t appblueprint-api Code/AppBlueprint

# Build Web Service  
docker build -f Code/AppBlueprint/AppBlueprint.Web/Dockerfile -t appblueprint-web Code/AppBlueprint
```

**⚠️ Important**: The build context must be `Code/AppBlueprint`, not the service directory!

**Or use the test script**:
```powershell
cd Code\AppBlueprint
.\test-docker-build.ps1
```

See `DOCKER_BUILD_INSTRUCTIONS.md` for detailed explanations.

## Troubleshooting

### Deployment Failed

1. **Check logs**:
   ```powershell
   railway logs --service api-service
   ```

2. **Verify Dockerfile path**:
   - Ensure `RAILWAY_DOCKERFILE_PATH` is set correctly
   - Check root directory is `Code/AppBlueprint`

3. **Test build locally**:
   ```powershell
   docker build -f AppBlueprint.ApiService/Dockerfile -t test .
   ```

### Database Connection Issues

1. **Verify DATABASE_URL exists**:
   ```powershell
   railway variables get DATABASE_URL --service postgres
   ```

2. **Check PostgreSQL service is running** in Railway dashboard

3. **Test connection locally**:
   ```powershell
   psql "your-railway-database-url"
   ```

### Health Check Failures

1. **Verify health endpoint** exists at `/health`
2. **Check service is listening** on `${PORT}`
3. **Increase timeout** in railway.json if needed

## Environment Variables Reference

### Railway Provides These Automatically
- `PORT`: Port number for your service
- `DATABASE_URL`: PostgreSQL connection string (from linked database)
- `RAILWAY_ENVIRONMENT`: Current environment name
- `RAILWAY_PROJECT_ID`: Your project ID
- `RAILWAY_SERVICE_NAME`: Service name

### You Must Configure These
- `RAILWAY_DOCKERFILE_PATH`: Path to Dockerfile (relative to root directory)
- `ASPNETCORE_ENVIRONMENT`: ASP.NET Core environment
- `ASPNETCORE_URLS`: URL binding (use `http://+:${PORT}`)
- `DATABASE_CONNECTION_STRING`: Set to `${DATABASE_URL}`

## Cost Monitoring

- View usage: https://railway.app/account/usage
- Set up budget alerts in Railway dashboard
- Monitor resource usage per service

## Getting Help

- Railway Documentation: https://docs.railway.app
- Railway Discord: https://discord.gg/railway
- GitHub Issues: https://github.com/saas-factory-labs/Saas-Factory/issues

## Security Best Practices

1. **Never commit tokens** to version control
2. **Use GitHub Secrets** for all sensitive data
3. **Rotate tokens regularly** (every 90 days recommended)
4. **Enable 2FA** on Railway account
5. **Restrict production access** to authorized users only
6. **Review Railway audit logs** regularly

## Next Steps

After successful deployment:
1. Configure custom domains (optional)
2. Set up monitoring and alerts
3. Configure backup strategy for database
4. Set up CI/CD branch protection rules
5. Document your specific configuration

