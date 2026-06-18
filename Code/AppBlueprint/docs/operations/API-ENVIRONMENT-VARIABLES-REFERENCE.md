# API Service Environment Variables - Code Reference

This document lists the **exact environment variables** that the API service code reads, based on actual code analysis (not documentation).

## 🔍 Source Files Analyzed

- `ConfigurationServiceCollectionExtensions.cs` - Main configuration
- `JwtAuthenticationExtensions.cs` - Authentication
- `ServiceCollectionExtensions.cs` - Database
- `StripeWebhookServiceExtensions.cs` - Stripe
- `FirebasePushNotificationService.cs` - Firebase
- `ApplicationBuilderExtensions.cs` - CORS

---

## ✅ Required Environment Variables

These **MUST** be set for the API to function:

### Database Connection
```bash
# Priority: Environment variable > Configuration
DATABASE_CONNECTIONSTRING="postgresql://user:pass@host:5432/dbname"
# Alternative names checked by code:
# - ConnectionStrings:appblueprintdb (config section)
# - ConnectionStrings:postgres-server (config section)
# - ConnectionStrings:DefaultConnection (config section)
```

### Authentication (Logto)
```bash
# Provider (default: Logto if not set)
AUTHENTICATION_PROVIDER="Logto"

# Endpoint (priority order - first found wins)
LOGTO_ENDPOINT="https://your-subdomain.logto.app"
# OR: AUTHENTICATION_LOGTO_ENDPOINT

# Client/App ID (priority order - first found wins)
LOGTO_APPID="your-client-id"
# OR: LOGTO_CLIENTID
# OR: AUTHENTICATION_LOGTO_CLIENTID
# OR: LOGTO_APP_ID

# API Resource (priority order - first found wins)
LOGTO_APIRESOURCE="https://api.your-domain.com"
# OR: LOGTO_RESOURCE
# OR: AUTHENTICATION_LOGTO_APIRESOURCE
```

**Code Behavior**:
- If `LOGTO_ENDPOINT` or `LOGTO_APPID` is missing → Throws `InvalidOperationException` at startup
- Endpoint has trailing slash automatically removed
- Authority is set to `{endpoint}/oidc`
- Metadata address is `{endpoint}/oidc/.well-known/openid-configuration`

---

## 🔧 Optional Environment Variables

### Database Context Options

```bash
# Context type (default: B2C)
DATABASECONTEXT_TYPE="B2C"  # Values: Baseline, B2C, B2B
# OR: DATABASECONTEXT_CONTEXTTYPE (legacy)

# Hybrid mode flag (default: true - registers ALL contexts regardless of TYPE)
DATABASECONTEXT_ENABLEHYBRIDMODE="true"

# Baseline only mode (default: false)
DATABASECONTEXT_BASELINEONLY="false"

# Command timeout in seconds (default: 60)
DATABASECONTEXT_COMMANDTIMEOUT="60"

# Max retry count (default: 5)
DATABASECONTEXT_MAXRETRYCOUNT="5"
```

**Code Behavior**:
- When `ENABLEHYBRIDMODE=true` (default): All contexts (Baseline, B2C, B2B) are registered regardless of TYPE setting
- When `ENABLEHYBRIDMODE=false`: Only the specified TYPE context is registered
- `BASELINEONLY=true` overrides both settings and registers only BaselineDbContext

### Stripe Payment Processing

```bash
# API Key (priority order - first found wins)
STRIPE_APIKEY="sk_live_..."
# OR: STRIPE_API_KEY (legacy)
# OR: Stripe:ApiKey (config section)

# Webhook Secret (priority order - first found wins)
STRIPE_WEBHOOK_SECRET="whsec_..."
# OR: STRIPE_WEBHOOKSECRET
# OR: Stripe:WebhookSecret (config section)
```

**Code Behavior**:
- If not configured → Logged as warning, signature verification skipped
- If configured → ApiKey must start with `sk_` (validated)

### Firebase Push Notifications

```bash
# Service account credentials (must be valid JSON)
FIREBASE_CREDENTIALSJSON='{"type":"service_account","project_id":"...","private_key":"...","client_email":"..."}'
# OR: Firebase:CredentialsJson (config section)

# Project ID
FIREBASE_PROJECTID="your-project-id"
# OR: Firebase:ProjectId (config section)
```

**Code Behavior**:
- If not configured → Logged as warning, push notifications disabled
- If invalid JSON → Logged as error, push notifications disabled
- Initialization happens at service construction time

### Cloudflare R2 Storage

```bash
# Access credentials
CLOUDFLARE_R2_ACCESSKEYID="your-access-key-id"
CLOUDFLARE_R2_SECRETACCESSKEY="your-secret-access-key"
CLOUDFLARE_R2_ENDPOINTURL="https://account-id.r2.cloudflarestorage.com"

# Bucket configuration
CLOUDFLARE_R2_PRIVATEBUCKETNAME="private-files"
CLOUDFLARE_R2_PUBLICBUCKETNAME="public-files"
CLOUDFLARE_R2_PUBLICDOMAIN="https://files.your-domain.com"

# Size limits (MB)
CLOUDFLARE_R2_MAXIMAGESIZEMB="10"
CLOUDFLARE_R2_MAXDOCUMENTSIZEMB="50"
CLOUDFLARE_R2_MAXVIDEOSIZEMB="500"
```

**Code Behavior**:
- All settings optional
- Validation only runs if any credential is provided
- If partial config → Throws validation error

### Resend Email Service

```bash
# Read from configuration section only (no environment variable override)
# Add to appsettings.json:
# {
#   "ResendEmail": {
#     "ApiKey": "re_..."
#   }
# }
```

---

## 🌐 ASP.NET Core Environment Variables

### Port Configuration

```bash
# Railway injects this automatically - DO NOT override
PORT="8080"
```

**Code Behavior** (from Dockerfile):
- Default: `PORT=8080`
- Dockerfile CMD uses: `dotnet AppBlueprint.ApiService.dll --urls "http://+:${PORT}"`
- Railway overrides `PORT` at runtime

### Environment

```bash
# Standard ASP.NET Core variable
ASPNETCORE_ENVIRONMENT="Production"
```

**Code Behavior**:
- Development → CORS allows any origin, Swagger enabled
- Production → CORS requires `Cors:AllowedOrigins` config, no Swagger

---

## 📋 CORS Configuration

**Note**: CORS is NOT configured via environment variables. It reads from configuration section.

**In `appsettings.json` or Railway config editor**:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://your-frontend.railway.app",
      "https://your-domain.com"
    ]
  }
}
```

**Code Behavior**:
- Development → Allows any origin (ignores config)
- Production → MUST have `AllowedOrigins` array or throws `InvalidOperationException`

---

## 🎯 Priority Order Reference

When multiple environment variables can provide the same value, the code checks in this order:

### Logto Endpoint
1. `LOGTO_ENDPOINT`
2. `AUTHENTICATION_LOGTO_ENDPOINT`
3. `Authentication:Logto:Endpoint` (config)

### Logto Client ID
1. `LOGTO_APPID`
2. `LOGTO_CLIENTID`
3. `AUTHENTICATION_LOGTO_CLIENTID`
4. `LOGTO_APP_ID`
5. `Authentication:Logto:ClientId` (config)

### Logto API Resource
1. `LOGTO_APIRESOURCE`
2. `LOGTO_RESOURCE`
3. `AUTHENTICATION_LOGTO_APIRESOURCE`
4. `Authentication:Logto:ApiResource` (config)

### Database Connection String
1. `DATABASE_CONNECTIONSTRING`
2. `ConnectionStrings:appblueprintdb` (config)
3. `ConnectionStrings:postgres-server` (config)
4. `ConnectionStrings:DefaultConnection` (config)

### Database Context Type
1. `DATABASECONTEXT_TYPE`
2. `DATABASECONTEXT_CONTEXTTYPE` (legacy)
3. `DatabaseContext:ContextType` (config)

### Stripe API Key
1. Environment variable binding to `Stripe:ApiKey` (config)
2. `Stripe:ApiKey` (config)

### Stripe Webhook Secret
1. `STRIPE_WEBHOOK_SECRET`
2. `Stripe:WebhookSecret` (config)

---

## 🧪 Minimal Working Configuration

For Railway deployment, set these **minimum required** variables:

```bash
# Database (Railway auto-creates when you add PostgreSQL)
DATABASE_CONNECTIONSTRING="${{Postgres.DATABASE_URL}}"

# Authentication
AUTHENTICATION_PROVIDER="Logto"
LOGTO_ENDPOINT="https://your-subdomain.logto.app"
LOGTO_APPID="your-client-id"
LOGTO_APIRESOURCE="https://api.your-domain.com"

# Environment
ASPNETCORE_ENVIRONMENT="Production"
```

Plus in `appsettings.json` or config editor:
```json
{
  "Cors": {
    "AllowedOrigins": ["https://your-frontend.railway.app"]
  }
}
```

**This is sufficient** for the API to start and handle authenticated requests. Optional services (Stripe, Firebase, R2) can be added later.

---

## 🐛 Debugging Tips

### Check What Variables Are Actually Set

Log into Railway container or check logs for these diagnostic messages:

```
[CloudflareR2Options] Loaded - AccessKeyId: True/False, ...
[AppBlueprint.Infrastructure] WARNING: Stripe webhook secret not configured
Firebase credentials not configured. Push notifications will be disabled.
```

### Common Mistakes

1. **Setting `ASPNETCORE_URLS` manually** → Let Railway control the `PORT` variable
2. **Including `/oidc` in `LOGTO_ENDPOINT`** → Code adds it automatically
3. **Using hierarchical config names as env vars** → Use UPPERCASE_UNDERSCORE format
4. **Forgetting `CORS:AllowedOrigins` in production** → Causes startup exception

---

**Last Updated**: February 9, 2026  
**Based on Code Analysis**: AppBlueprint API Service (Main branch)
