# Railway Deployment Guide - AppBlueprint.ApiService

## Overview

This guide covers deploying the AppBlueprint API Service to Railway with all required environment variables.

## ✅ Fixes Applied

1. **Port Configuration**: Dockerfile now uses Railway's dynamic `PORT` environment variable
2. **Railway Config Location**: Moved `railway.json` and `railway.toml` to repository root
3. **Dockerfile Path**: Corrected path to `Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile`

## 🔧 Required Environment Variables

> 📚 **For complete environment variable reference with code analysis**, see [API-ENVIRONMENT-VARIABLES-REFERENCE.md](API-ENVIRONMENT-VARIABLES-REFERENCE.md)

### Database Configuration

```bash
# PostgreSQL connection  
# Priority: DATABASE_CONNECTIONSTRING env var > ConnectionStrings:appblueprintdb config
DATABASE_CONNECTIONSTRING="${{Postgres.DATABASE_URL}}"

# Database Context Options (Optional - defaults shown below)
# Only set these if you need to override the defaults
# DATABASECONTEXT_TYPE="B2C"  # Values: Baseline, B2C, B2B (default: B2C)
# DATABASECONTEXT_ENABLEHYBRIDMODE="true"  # Registers all contexts (default: true)
# DATABASECONTEXT_COMMANDTIMEOUT="60"  # Command timeout in seconds (default: 60)
# DATABASECONTEXT_MAXRETRYCOUNT="5"  # Max retry count for transient errors (default: 5)
```

**Note**: 
- Railway automatically creates `DATABASE_URL` when you add PostgreSQL. Reference it using `${{Postgres.DATABASE_URL}}`.
- `DATABASECONTEXT_ENABLEHYBRIDMODE` defaults to `true`, registering all database contexts (Baseline, B2C, B2B) to support all features. This ensures services like file storage, notifications, and repositories work correctly in production.

### Authentication (Logto) - **REQUIRED**

```bash
# Authentication Provider (only Logto is supported)
AUTHENTICATION_PROVIDER="Logto"

# Logto Endpoint (without /oidc suffix)
LOGTO_ENDPOINT="https://YOUR-LOGTO-SUBDOMAIN.logto.app"
# Alternative naming:
AUTHENTICATION_LOGTO_ENDPOINT="https://YOUR-LOGTO-SUBDOMAIN.logto.app"

# Logto Client/App ID
LOGTO_APPID="your-client-id"
# Alternative naming (all supported):
LOGTO_CLIENTID="your-client-id"
AUTHENTICATION_LOGTO_CLIENTID="your-client-id"
LOGTO_APP_ID="your-client-id"

# Logto API Resource (CRITICAL for JWT validation)
LOGTO_APIRESOURCE="https://api.your-domain.com"
# Alternative naming:
LOGTO_RESOURCE="https://api.your-domain.com"
AUTHENTICATION_LOGTO_APIRESOURCE="https://api.your-domain.com"
```

**⚠️ Critical**: 
- `LOGTO_APIRESOURCE` MUST match the API Resource configured in your Logto dashboard
- Without this, JWT validation will fail with 401 errors
- The endpoint should NOT include `/oidc` suffix (the code adds it automatically)

### Firebase Configuration (Optional - for push notifications)

```bash
# Firebase credentials (service account JSON as single-line string)
# Get from Firebase Console → Project Settings → Service Accounts → Generate Private Key
FIREBASE_CREDENTIALSJSON='{"type":"service_account","project_id":"...","private_key":"...","client_email":"..."}'

# Firebase Project ID
FIREBASE_PROJECTID="your-firebase-project-id"
```

**Note**: If Firebase is not configured, push notifications will be disabled (logged as warning).

### Stripe (Optional - for payment processing)

```bash
# Stripe API Secret Key
STRIPE_APIKEY="sk_live_..."  # Use sk_test_... for testing
# Alternative naming:
STRIPE_API_KEY="sk_live_..."

# Stripe Webhook Secret (for webhook signature verification)
STRIPE_WEBHOOK_SECRET="whsec_..."
# Alternative naming:
STRIPE_WEBHOOKSECRET="whsec_..."
```

**Note**: If Stripe is not configured, payment features will be disabled.

### Cloudflare R2 (Optional - for file storage)

```bash
# Cloudflare R2 Access Credentials
CLOUDFLARE_R2_ACCESSKEYID="your-access-key-id"
CLOUDFLARE_R2_SECRETACCESSKEY="your-secret-access-key"
CLOUDFLARE_R2_ENDPOINTURL="https://your-account-id.r2.cloudflarestorage.com"

# Bucket Configuration
CLOUDFLARE_R2_PRIVATEBUCKETNAME="private-files"
CLOUDFLARE_R2_PUBLICBUCKETNAME="public-files"
CLOUDFLARE_R2_PUBLICDOMAIN="https://files.your-domain.com"

# File Size Limits (in MB, optional)
CLOUDFLARE_R2_MAXIMAGESIZEMB="10"
CLOUDFLARE_R2_MAXDOCUMENTSIZEMB="50"
CLOUDFLARE_R2_MAXVIDEOSIZEMB="500"
```

### CORS Configuration (Required for Production)

```bash
# Comma-separated list of allowed origins
# Note: This is read from appsettings.json, not environment variables
# Add to your appsettings.json:
# {
#   "Cors": {
#     "AllowedOrigins": [
#       "https://your-frontend.railway.app",
#       "https://your-domain.com"
#     ]
#   }
# }
```

**Alternative**: Set in Railway's raw configuration editor or mount as config file.

### Application Settings

```bash
# ASP.NET Core Environment
ASPNETCORE_ENVIRONMENT="Production"

# Port (Railway sets this automatically - DO NOT override)
# PORT is automatically injected by Railway and used by Dockerfile
```

## 🚀 Deployment Steps

### 1. Create Railway Project

```bash
# Install Railway CLI
npm i -g @railway/cli

# Login to Railway
railway login

# Create new project
railway init
```

### 2. Add PostgreSQL Database

In Railway dashboard:
1. Click **New** → **Database** → **Add PostgreSQL**
2. Railway auto-creates `DATABASE_URL` variable

### 3. Set Environment Variables

#### Option A: Railway Dashboard
1. Go to your service → **Variables** tab
2. Click **New Variable** for each required variable
3. Deploy

#### Option B: Railway CLI
```bash
# Set authentication variables (REQUIRED)
railway variables set AUTHENTICATION_PROVIDER="Logto"
railway variables set LOGTO_ENDPOINT="https://32nkyp.logto.app"
railway variables set LOGTO_APPID="your-client-id"
railway variables set LOGTO_APIRESOURCE="https://api.your-domain.com"

# Set database connection (Railway auto-creates this when you add PostgreSQL)
railway variables set DATABASE_CONNECTIONSTRING="${{Postgres.DATABASE_URL}}"

# Set Firebase credentials (optional - single-line JSON)
railway variables set FIREBASE_CREDENTIALSJSON='{"type":"service_account","project_id":"...","private_key":"...","client_email":"..."}'
railway variables set FIREBASE_PROJECTID="your-firebase-project-id"

# Set Stripe keys (optional)
railway variables set STRIPE_APIKEY="sk_live_..."
railway variables set STRIPE_WEBHOOK_SECRET="whsec_..."

# Set environment
railway variables set ASPNETCORE_ENVIRONMENT="Production"
```

### 4. Deploy

```bash
# Deploy current branch
railway up

# Watch logs
railway logs
```

## 🔍 Troubleshooting

### Container Failed to Start

**Symptom**: Build succeeds, but "Container failed to start"

**Common Causes**:

1. **Missing environment variables** - Check logs for configuration errors:
   ```bash
   railway logs
   ```

2. **Database migration failures** - Ensure `DATABASE_CONNECTION_STRING` is set correctly

3. **Health check failures** - API must respond to `/health` within 100 seconds:
   ```bash
   curl https://your-api.railway.app/health
   ```

4. **Port binding issues** - Ensure Dockerfile uses `CMD` with `${PORT}` (fixed in this repo)

### Authentication Errors (401 Unauthorized)

**Symptom**: API returns 401 for authenticated requests

**Fixes**:
- ✅ Verify `LOGTO_APIRESOURCE` (or `AUTHENTICATION_LOGTO_APIRESOURCE`) matches Logto dashboard configuration
- ✅ Check JWT token includes correct audience (`aud`) claim
- ✅ Ensure `LOGTO_ENDPOINT` doesn't include `/oidc` suffix (API adds it automatically: `{endpoint}/oidc`)
- ✅ Verify Logto CORS settings allow your Railway domain
- ✅ Check all required variables are set: `AUTHENTICATION_PROVIDER`, `LOGTO_ENDPOINT`, `LOGTO_APPID`, `LOGTO_APIRESOURCE`

### Database Connection Errors

**Symptom**: `Npgsql.NpgsqlException: connection refused`

**Fixes**:
- ✅ Link PostgreSQL service to API service in Railway dashboard
- ✅ Verify `DATABASE_CONNECTIONSTRING` uses `${{Postgres.DATABASE_URL}}`
- ✅ Check database is running (Railway → Postgres → Metrics)

### Firebase Notification Errors

**Symptom**: Notifications fail to send or "Firebase not initialized" in logs

**Fixes**:
- ✅ Verify `FIREBASE_CREDENTIALSJSON` is valid JSON (properly escaped)
- ✅ Ensure `FIREBASE_PROJECTID` matches your Firebase project
- ✅ Check Firebase project has Cloud Messaging enabled
- ✅ Ensure service account has Firebase Admin SDK permissions
- **Note**: If Firebase is not configured, the API will still run but push notifications will be disabled

## 📊 Monitoring

### Health Check Endpoint

```bash
curl https://your-api.railway.app/health
```

Expected response:
```json
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "authentication": "Healthy"
  }
}
```

### Railway Logs

```bash
# Stream logs
railway logs

# Filter by service
railway logs --service api
```

### OpenAPI Documentation

Once deployed, access Swagger UI at:
```
https://your-api.railway.app/swagger
```

## 🔐 Security Checklist

Before deploying to production:

- [ ] Use **live Stripe keys** (not test keys)
- [ ] Set `ASPNETCORE_ENVIRONMENT="Production"`
- [ ] Configure **production Logto environment**
- [ ] Enable **Railway SSL/TLS** (automatic)
- [ ] Set restrictive **CORS_ALLOWED_ORIGINS**
- [ ] Rotate **FIREBASE_CREDENTIALS** service account regularly
- [ ] Enable **Railway deployment protection**
- [ ] Set up **monitoring/alerting** for errors

## 📚 Additional Resources

- [API Environment Variables Reference](API-ENVIRONMENT-VARIABLES-REFERENCE.md) - **Complete guide based on code analysis**
- [Railway Documentation](https://docs.railway.app/)
- [Logto Environment Variables](Code/AppBlueprint/LOGTO-ENVIRONMENT-VARIABLES.md)
- [Firebase Setup Guide](Code/AppBlueprint/FIREBASE-ENV-SETUP.md)
- [Database Context Documentation](Code/AppBlueprint/DATABASE_CONTEXT_DOCUMENTATION_INDEX.md)

## 🆘 Support

If deployment continues to fail:

1. Check Railway logs: `railway logs`
2. Verify all environment variables are set
3. Test health endpoint: `curl https://your-api.railway.app/health`
4. Check Railway status: https://railway.statuspage.io/
5. Review [Railway troubleshooting docs](https://docs.railway.app/troubleshooting/build-failures)

---

**Last Updated**: February 9, 2026
**Railway Configuration**: `railway.json`, `railway.toml`
**Dockerfile**: `Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile`
