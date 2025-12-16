# Railway Deployment Implementation Summary

## Overview
This implementation adds complete Railway deployment support to the AppBlueprint project via GitHub Actions.

## Files Created

### 1. GitHub Actions Workflow
**File**: `.github/workflows/deploy-to-railway.yml`

A comprehensive workflow that provides:
- **Automatic staging deployment** on push to `main`
- **Manual production deployment** via workflow dispatch
- **Build and test** validation before deployment
- **Database migrations** after successful deployment
- **Health checks** for deployed services
- **Deployment notifications** with URLs

**Key Features**:
- Supports deploying all services or individual services (API/Web)
- Environment-based deployment (staging/production)
- Parallel deployment optimization
- Comprehensive error handling
- Deployment status summaries

### 2. Railway Configuration Files

#### `Code/AppBlueprint/railway.json`
- Defines build and deployment settings for Railway
- Configures Dockerfile path using `RAILWAY_DOCKERFILE_PATH`
- Sets health check endpoint and retry policies
- Optimized for API service deployment

#### `Code/AppBlueprint/railway-project.json`
- Multi-service project definition
- Defines API, Web, and PostgreSQL services
- Sets up service dependencies
- Configures environment variables for each service

#### `Code/AppBlueprint/docker-compose.railway.yml`
- Railway-specific Docker Compose overrides
- Handles Railway's dynamic `PORT` variable
- Configures health checks
- Sets up proper service dependencies

### 3. Documentation

#### `Code/AppBlueprint/RAILWAY_DEPLOYMENT.md`
Comprehensive deployment guide covering:
- Service configuration details
- Environment variables reference
- GitHub secrets setup
- Railway setup instructions
- Monitoring and troubleshooting

#### `Code/AppBlueprint/RAILWAY_QUICKSTART.md`
Quick start guide with:
- Step-by-step setup instructions
- Deployment methods (automatic, manual, CLI)
- Common commands reference
- Troubleshooting guide
- Security best practices

### 4. Setup Script

#### `Code/AppBlueprint/setup-railway.ps1`
PowerShell automation script that:
- Installs Railway CLI if needed
- Authenticates with Railway
- Creates staging/production environments
- Adds PostgreSQL database
- Guides through configuration
- Generates GitHub secrets information

## How It Works

### Deployment Flow

```
┌─────────────────┐
│  Push to main   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Build & Test    │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Deploy API      │ → Railway Staging
│ (Staging)       │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Run Migrations  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Deploy Web      │ → Railway Staging
│ (Staging)       │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Health Checks   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Notify Success  │
└─────────────────┘
```

### Production Deployment

Production requires manual trigger:
1. Navigate to GitHub Actions
2. Select "Deploy to Railway" workflow
3. Click "Run workflow"
4. Choose environment: `production`
5. Approve deployment (via GitHub environment protection)

## Required Setup

### GitHub Secrets

Add these to your repository (Settings → Secrets):

```
RAILWAY_TOKEN_STAGING          - Railway API token for staging
RAILWAY_TOKEN_PRODUCTION       - Railway API token for production
RAILWAY_DATABASE_URL_STAGING   - PostgreSQL URL from Railway (staging)
RAILWAY_DATABASE_URL_PRODUCTION - PostgreSQL URL from Railway (production)
```

### Railway Configuration

For each service in Railway dashboard:

#### API Service
```bash
RAILWAY_DOCKERFILE_PATH=AppBlueprint.ApiService/Dockerfile
ASPNETCORE_URLS=http://+:${PORT}
DATABASE_CONNECTION_STRING=${DATABASE_URL}
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ASPNETCORE_ENVIRONMENT=Production
DOTNET_ENVIRONMENT=Production
```

#### Web Service
```bash
RAILWAY_DOCKERFILE_PATH=AppBlueprint.Web/Dockerfile
ASPNETCORE_URLS=http://+:${PORT}
ApiService__BaseUrl=${API_SERVICE_URL}
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ASPNETCORE_ENVIRONMENT=Production
DOTNET_ENVIRONMENT=Production
```

## Key Technical Details

### RAILWAY_DOCKERFILE_PATH
This environment variable tells Railway which Dockerfile to use:
- **API Service**: `AppBlueprint.ApiService/Dockerfile`
- **Web Service**: `AppBlueprint.Web/Dockerfile`

This is crucial because Railway needs to know which Dockerfile to build when you have multiple services in the same repository.

### Dynamic PORT Handling
Railway assigns a random port via `${PORT}` environment variable. The configuration handles this with:
```bash
ASPNETCORE_URLS=http://+:${PORT}
```

### Database Connection
Railway provides PostgreSQL connection via `${DATABASE_URL}`, which we map to:
```bash
DATABASE_CONNECTION_STRING=${DATABASE_URL}
```

### Health Checks
The workflow includes health checks to verify deployment success:
- Endpoint: `/health`
- Timeout: 100 seconds
- Retries: 3 times with 10-second intervals

## Usage Examples

### Deploy to Staging (Automatic)
```bash
git add .
git commit -m "feat: add new feature"
git push origin main
```

### Deploy to Production (Manual)
1. Go to GitHub Actions
2. Select "Deploy to Railway"
3. Run workflow with environment: `production`

### Deploy Single Service
1. Go to GitHub Actions
2. Select "Deploy to Railway"
3. Choose service: `api` or `web`

### Run Migrations Manually
```powershell
$env:DATABASE_CONNECTION_STRING = "postgresql://..."
cd Code\AppBlueprint
dotnet ef database update `
  --project Shared-Modules\AppBlueprint.Infrastructure `
  --startup-project AppBlueprint.ApiService
```

## Monitoring and Logs

### View Deployment Logs
GitHub Actions provides detailed logs for each step:
- Build output
- Deployment progress
- Migration results
- Health check status

### Railway Logs
```bash
railway logs --service api-service --environment staging
```

Or via Railway dashboard: https://railway.app/dashboard

## Benefits

1. **Automated Deployments**: Push to main → automatic staging deployment
2. **Safety**: Manual approval required for production
3. **Validation**: Tests run before any deployment
4. **Migrations**: Automatic database migrations
5. **Monitoring**: Health checks and deployment status
6. **Flexibility**: Deploy all services or individual ones
7. **Cost Effective**: Railway's pricing is competitive
8. **Easy Setup**: Scripts automate most configuration

## Migration from Azure

If migrating from Azure:
1. Export Azure PostgreSQL database
2. Import to Railway PostgreSQL
3. Update GitHub secrets with Railway tokens
4. Run workflow to deploy
5. Verify deployment
6. Update DNS records
7. Decommission Azure resources

## Next Steps

1. **Run setup script**: `.\setup-railway.ps1`
2. **Configure GitHub secrets**: Add Railway tokens and database URLs
3. **Configure Railway services**: Set environment variables
4. **Test deployment**: Push to main or run workflow manually
5. **Monitor**: Check Railway dashboard and logs
6. **Production**: Deploy to production when ready

## Support

- **Documentation**: See `RAILWAY_DEPLOYMENT.md` and `RAILWAY_QUICKSTART.md`
- **Railway Docs**: https://docs.railway.app
- **Issues**: Create issue in GitHub repository
- **Railway Discord**: https://discord.gg/railway

## Security Notes

- All secrets stored in GitHub Secrets (encrypted)
- Railway tokens should be rotated every 90 days
- Production deployments require manual approval
- Database backups created before production migrations
- Environment variables validated before deployment

## Cost Estimation

**Staging Environment**:
- API Service: ~$5-10/month
- Web Service: ~$5-10/month
- PostgreSQL: ~$5/month
- **Total**: ~$15-25/month

**Production Environment**:
- API Service: ~$20-30/month
- Web Service: ~$20-30/month
- PostgreSQL: ~$10/month
- **Total**: ~$50-70/month

Costs vary based on:
- Traffic volume
- Database size
- CPU/Memory usage
- Data transfer

Monitor usage at: https://railway.app/account/usage

