# Railway Deployment - Setup Checklist

Use this checklist to ensure proper Railway deployment configuration.

## ✅ Pre-Deployment Checklist

### 1. Railway Account Setup
- [ ] Create Railway account at https://railway.app
- [ ] Verify email address
- [ ] Set up billing (if using paid tier)
- [ ] Enable 2FA for security

### 2. Install Tools
- [ ] Install Railway CLI: `npm install -g @railway/cli`
- [ ] Verify installation: `railway --version`
- [ ] Install .NET 9 SDK
- [ ] Install Docker (for local testing)

### 3. Railway Project Configuration
- [ ] Run setup script: `.\Code\AppBlueprint\setup-railway.ps1`
- [ ] Create Railway project
- [ ] Create `staging` environment
- [ ] Create `production` environment
- [ ] Add PostgreSQL database to each environment

### 4. Service Configuration in Railway

#### API Service (Staging)
- [ ] Create service named `api-service`
- [ ] Set root directory: `Code/AppBlueprint`
- [ ] Set environment variables:
  - [ ] `RAILWAY_DOCKERFILE_PATH=AppBlueprint.ApiService/Dockerfile`
  - [ ] `ASPNETCORE_ENVIRONMENT=Staging`
  - [ ] `ASPNETCORE_URLS=http://+:${PORT}`
  - [ ] `DATABASE_CONNECTION_STRING=${DATABASE_URL}`
  - [ ] `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true`
  - [ ] `DOTNET_ENVIRONMENT=Staging`
- [ ] Link to PostgreSQL service
- [ ] Configure health check path: `/health`

#### Web Service (Staging)
- [ ] Create service named `web-service`
- [ ] Set root directory: `Code/AppBlueprint`
- [ ] Set environment variables:
  - [ ] `RAILWAY_DOCKERFILE_PATH=AppBlueprint.Web/Dockerfile`
  - [ ] `ASPNETCORE_ENVIRONMENT=Staging`
  - [ ] `ASPNETCORE_URLS=http://+:${PORT}`
  - [ ] `ASPNETCORE_FORWARDEDHEADERS_ENABLED=true`
  - [ ] `ApiService__BaseUrl=<API_SERVICE_URL>`
  - [ ] `DOTNET_ENVIRONMENT=Staging`
- [ ] Link to API service (dependency)

#### API Service (Production)
- [ ] Create service named `api-service` in production environment
- [ ] Set root directory: `Code/AppBlueprint`
- [ ] Set environment variables (same as staging but with `Production`)
- [ ] Link to PostgreSQL service
- [ ] Configure health check path: `/health`

#### Web Service (Production)
- [ ] Create service named `web-service` in production environment
- [ ] Set root directory: `Code/AppBlueprint`
- [ ] Set environment variables (same as staging but with `Production`)
- [ ] Link to API service (dependency)

### 5. GitHub Secrets Configuration
- [ ] Navigate to GitHub repository → Settings → Secrets and variables → Actions
- [ ] Add `RAILWAY_TOKEN_STAGING`:
  1. Go to https://railway.app/account/tokens
  2. Create token named "GitHub Actions Staging"
  3. Copy token and add to GitHub secrets
- [ ] Add `RAILWAY_TOKEN_PRODUCTION`:
  1. Create token named "GitHub Actions Production"
  2. Copy token and add to GitHub secrets
- [ ] Add `RAILWAY_DATABASE_URL_STAGING`:
  1. Get from Railway: `railway variables get DATABASE_URL --service postgres --environment staging`
  2. Add to GitHub secrets
- [ ] Add `RAILWAY_DATABASE_URL_PRODUCTION`:
  1. Get from Railway: `railway variables get DATABASE_URL --service postgres --environment production`
  2. Add to GitHub secrets

### 6. GitHub Environment Protection
- [ ] Go to GitHub repository → Settings → Environments
- [ ] Create `staging` environment
- [ ] Create `production` environment
- [ ] Configure production environment:
  - [ ] Enable "Required reviewers"
  - [ ] Add yourself and/or team as reviewers
  - [ ] Set deployment branch to `main` only

### 7. Database Setup
- [ ] Verify PostgreSQL is running in Railway
- [ ] Note database credentials:
  - [ ] Host
  - [ ] Port
  - [ ] Database name
  - [ ] Username
  - [ ] Password (or use `${DATABASE_URL}`)
- [ ] Test connection locally (optional):
  ```powershell
  psql "railway-database-url"
  ```

### 8. Code Verification
- [ ] Review `.github/workflows/deploy-to-railway.yml`
- [ ] Review `Code/AppBlueprint/railway.json`
- [ ] Review `Code/AppBlueprint/railway-project.json`
- [ ] Review `Code/AppBlueprint/docker-compose.railway.yml`
- [ ] Ensure Dockerfiles exist:
  - [ ] `Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile`
  - [ ] [ ] `Code/AppBlueprint/AppBlueprint.Web/Dockerfile`

## ✅ First Deployment Checklist

### Test Build Locally (Optional)
- [ ] Build API Docker image:
  ```powershell
  cd Code\AppBlueprint
  docker build -f AppBlueprint.ApiService\Dockerfile -t api-test .
  ```
- [ ] Build Web Docker image:
  ```powershell
  docker build -f AppBlueprint.Web\Dockerfile -t web-test .
  ```

### Deploy to Staging (Automatic)
- [ ] Commit changes:
  ```bash
  git add .
  git commit -m "feat: add Railway deployment configuration"
  ```
- [ ] Push to main:
  ```bash
  git push origin main
  ```
- [ ] Monitor GitHub Actions:
  1. Go to repository → Actions tab
  2. Watch "Deploy to Railway" workflow
  3. Verify all jobs complete successfully

### Verify Staging Deployment
- [ ] Check GitHub Actions summary for deployment URLs
- [ ] Open API URL and verify it's running
- [ ] Check API health endpoint: `<API_URL>/health`
- [ ] Open Web URL and verify it's running
- [ ] Test basic functionality

### Check Railway Dashboard
- [ ] Go to https://railway.app/dashboard
- [ ] Select your project
- [ ] Verify services are running:
  - [ ] api-service (green indicator)
  - [ ] web-service (green indicator)
  - [ ] postgres (green indicator)
- [ ] Check logs for any errors
- [ ] Verify metrics look normal

### Database Migration Verification
- [ ] Check GitHub Actions migration job logs
- [ ] Verify migrations completed successfully
- [ ] (Optional) Connect to database and verify schema:
  ```powershell
  psql "$env:RAILWAY_DATABASE_URL_STAGING"
  \dt  # List tables
  ```

## ✅ Production Deployment Checklist

### Pre-Production
- [ ] Test all features in staging
- [ ] Review code changes
- [ ] Backup production database (if exists)
- [ ] Notify team of deployment
- [ ] Schedule deployment during low-traffic period

### Deploy to Production
- [ ] Go to GitHub repository → Actions
- [ ] Select "Deploy to Railway" workflow
- [ ] Click "Run workflow"
- [ ] Select:
  - Environment: `production`
  - Service: `all`
- [ ] Click "Run workflow"
- [ ] Approve deployment when prompted
- [ ] Monitor deployment progress

### Verify Production Deployment
- [ ] Check deployment URLs in GitHub Actions summary
- [ ] Open API URL: `<PRODUCTION_API_URL>/health`
- [ ] Open Web URL and login
- [ ] Test critical user flows
- [ ] Check Railway logs for errors
- [ ] Monitor performance metrics

### Post-Deployment
- [ ] Verify all features working
- [ ] Check error rates in Railway
- [ ] Monitor resource usage
- [ ] Update documentation with production URLs
- [ ] Notify team of successful deployment
- [ ] Create release notes (optional)

## ✅ Ongoing Maintenance Checklist

### Weekly
- [ ] Review Railway logs for errors
- [ ] Check resource usage and costs
- [ ] Verify backups are being created
- [ ] Test staging deployments

### Monthly
- [ ] Review and optimize resource allocation
- [ ] Update dependencies
- [ ] Review GitHub Actions workflow efficiency
- [ ] Check for Railway platform updates

### Quarterly
- [ ] Rotate Railway API tokens
- [ ] Review and update documentation
- [ ] Audit access controls
- [ ] Review deployment process

## ✅ Troubleshooting Checklist

### Deployment Failed
- [ ] Check GitHub Actions logs
- [ ] Verify Railway tokens are valid
- [ ] Check Railway service status
- [ ] Verify environment variables are set
- [ ] Check Dockerfile paths are correct
- [ ] Review Railway service logs

### Health Check Failing
- [ ] Verify `/health` endpoint exists
- [ ] Check service is listening on `${PORT}`
- [ ] Review application logs
- [ ] Increase health check timeout
- [ ] Test health endpoint locally

### Database Connection Issues
- [ ] Verify `DATABASE_URL` exists in Railway
- [ ] Check PostgreSQL service is running
- [ ] Verify connection string format
- [ ] Test connection from local machine
- [ ] Check firewall/network settings

### High Costs
- [ ] Review resource usage in Railway dashboard
- [ ] Check for inefficient queries
- [ ] Optimize Docker images
- [ ] Scale down unused environments
- [ ] Review Railway pricing tier

## 📝 Notes

- Keep this checklist updated as your deployment process evolves
- Document any custom configuration specific to your project
- Share this checklist with new team members
- Review and improve deployment process regularly

## 🔗 Quick Links

- Railway Dashboard: https://railway.app/dashboard
- Railway Documentation: https://docs.railway.app
- GitHub Actions: https://github.com/saas-factory-labs/Saas-Factory/actions
- Railway Discord: https://discord.gg/railway

---

**Last Updated**: 2025-01-07
**Maintained By**: DevOps Team

