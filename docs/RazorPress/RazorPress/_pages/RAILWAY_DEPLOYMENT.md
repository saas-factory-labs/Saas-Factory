# Railway Deployment Configuration

This directory contains Railway-specific configuration files for deploying the AppBlueprint services.

## Services

### API Service
- **Service Name**: `api-service`
- **Dockerfile**: `AppBlueprint.ApiService/Dockerfile`
- **Health Check**: `/health`
- **Environment Variable**: `RAILWAY_DOCKERFILE_PATH=AppBlueprint.ApiService/Dockerfile`

### Web Service
- **Service Name**: `web-service`
- **Dockerfile**: `AppBlueprint.Web/Dockerfile`
- **Environment Variable**: `RAILWAY_DOCKERFILE_PATH=AppBlueprint.Web/Dockerfile`

### Database Service
- **Service Name**: `postgres`
- **Type**: PostgreSQL 16
- **Managed**: Railway-managed PostgreSQL instance

## Environment Variables

### Required for API Service
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:${PORT}
DATABASE_CONNECTION_STRING=${DATABASE_URL}
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
DOTNET_ENVIRONMENT=Production
RAILWAY_DOCKERFILE_PATH=AppBlueprint.ApiService/Dockerfile
```

### Required for Web Service
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:${PORT}
ApiService__BaseUrl=${API_SERVICE_URL}
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
DOTNET_ENVIRONMENT=Production
RAILWAY_DOCKERFILE_PATH=AppBlueprint.Web/Dockerfile
```

## GitHub Secrets

Required secrets in your GitHub repository:

- `RAILWAY_TOKEN_STAGING` - Railway API token for staging environment
- `RAILWAY_TOKEN_PRODUCTION` - Railway API token for production environment
- `RAILWAY_DATABASE_URL_STAGING` - PostgreSQL connection string (Railway provides this as `DATABASE_URL`)
- `RAILWAY_DATABASE_URL_PRODUCTION` - PostgreSQL connection string for production

## Deployment

### Automatic Deployment (Staging)
- Triggered on push to `main` branch
- Deploys API and Web services
- Runs database migrations

### Manual Deployment (Production)
- Use GitHub Actions workflow dispatch
- Requires manual approval via GitHub environment protection rules
- Includes database backup before migrations

## Railway Setup

1. **Create Railway Project**
   ```bash
   railway login
   railway init
   ```

2. **Create Environments**
   ```bash
   railway environment create staging
   railway environment create production
   ```

3. **Add PostgreSQL**
   ```bash
   railway add --database postgresql
   ```

4. **Link Services**
   - Railway will automatically inject `DATABASE_URL` from PostgreSQL to services
   - Set `RAILWAY_DOCKERFILE_PATH` for each service to point to correct Dockerfile

5. **Get API Token**
   ```bash
   railway tokens create
   ```

## Testing Locally

To test Railway configuration locally:

```powershell
# Set environment variables
$env:PORT = "8080"
$env:ASPNETCORE_URLS = "http://+:8080"
$env:DATABASE_CONNECTION_STRING = "your-connection-string"

# Run the service
cd Code\AppBlueprint\AppBlueprint.ApiService
dotnet run
```

## Monitoring

Railway provides:
- Real-time logs
- Metrics dashboard
- Deployment history
- Resource usage monitoring

Access via: https://railway.app/dashboard

## Troubleshooting

### Deployment Fails
1. Check Railway logs in dashboard
2. Verify `RAILWAY_DOCKERFILE_PATH` is correctly set
3. Ensure all environment variables are configured

### Database Connection Issues
1. Verify `DATABASE_URL` is available in service
2. Check PostgreSQL service is running
3. Verify connection string format

### Health Check Failures
1. Ensure `/health` endpoint exists in API
2. Check health check timeout settings
3. Verify service is listening on correct PORT

## Cost Optimization

- Use staging environment with minimal resources
- Scale down replicas when not in use
- Monitor usage in Railway dashboard
- Set up usage alerts

