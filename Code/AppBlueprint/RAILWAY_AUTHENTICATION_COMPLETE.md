# Railway API Authentication Configuration Guide

## Overview
Both the **Web** and **API** services can use Logto for authentication in Railway. The configuration is now **optional** - services will start successfully with or without Logto credentials.

## Authentication Flow

### Web Service (Client)
1. User visits Web application
2. Web redirects to Logto for login
3. Logto returns ID token and access token
4. Web stores tokens and sends access token to API

### API Service (Resource Server)
1. Receives HTTP requests with Bearer token
2. Validates JWT token against Logto's public keys
3. Extracts user identity and claims
4. Authorizes access to resources

## Environment Variables

### Web Service (OpenID Connect Client)
```bash
# Required for Web authentication
Logto__AppId=uovd1gg5ef7i1c4w46mt6
Logto__Endpoint=https://32nkyp.logto.app/oidc
Logto__AppSecret=1WYlfj9ekHF3UmomvNsn62JWGa6gVYSy

# Also required
API_BASE_URL=http://appblueprint-api.railway.internal:80
```

### API Service (JWT Token Validation)
```bash
# Required for API token validation
Authentication__Provider=Logto
Authentication__Logto__Endpoint=https://32nkyp.logto.app
Authentication__Logto__ClientId=uovd1gg5ef7i1c4w46mt6

# Note: API uses Endpoint (not Endpoint/oidc) and ClientId (not AppId)
# Also required
ConnectionStrings__appblueprintdb=your-database-connection-string
```

## Configuration Differences: Web vs API

### Web Service Configuration
- **Purpose**: OpenID Connect client (initiates login)
- **Config Keys**: 
  - `Logto__Endpoint` = OIDC endpoint URL with `/oidc` suffix
  - `Logto__AppId` = Application ID
  - `Logto__AppSecret` = Application secret
- **Behavior**: Redirects users to Logto for authentication

### API Service Configuration  
- **Purpose**: JWT Bearer token validator (validates tokens)
- **Config Keys**:
  - `Authentication__Provider` = "Logto"
  - `Authentication__Logto__Endpoint` = Base Logto URL (WITHOUT `/oidc`)
  - `Authentication__Logto__ClientId` = Application ID
- **Behavior**: Validates JWT tokens from Authorization header

## Fallback Behavior

### Without Logto Configuration

#### Web Service
```
[Web] Logto authentication NOT configured - running without authentication
[Web] To enable authentication, set environment variables:
[Web]   - Logto__AppId
[Web]   - Logto__Endpoint
[Web]   - Logto__AppSecret (optional)
```
- Minimal cookie authentication configured
- No login required
- All routes publicly accessible ⚠️

#### API Service
```
[API] Logto Endpoint or ClientId not configured - falling back to custom JWT authentication
[API] To enable Logto authentication, set environment variables:
[API]   - Authentication__Logto__Endpoint
[API]   - Authentication__Logto__ClientId
```
- Falls back to symmetric key JWT validation
- Uses `Authentication__JWT__SecretKey` from config
- Development mode fallback enabled

### With Logto Configuration

#### Web Service
```
[Web] Logto authentication configuration found - enabling OpenID Connect
[Web] OpenID Connect configured with Authority: https://32nkyp.logto.app/oidc
```
- Full OIDC authentication enabled
- Users redirected to Logto for login
- Protected routes enforced

#### API Service
```
[API] Logto authentication configured
[API] Authority: https://32nkyp.logto.app/oidc
[API] JWT Bearer validation enabled
```
- JWT tokens validated against Logto
- Public keys fetched from OIDC discovery endpoint
- User claims extracted from validated tokens

## Railway Deployment Scenarios

### Scenario 1: No Authentication (Testing)
**Use Case**: Initial deployment, API testing, staging without login

**Environment Variables**:
```bash
# Web
API_BASE_URL=http://appblueprint-api.railway.internal:80

# API  
ConnectionStrings__appblueprintdb=your-db-connection

# That's it! No Logto variables needed
```

**Result**:
- Web: Publicly accessible, no login
- API: Accepts custom JWT tokens or runs without auth enforcement
- Suitable for testing API connectivity

### Scenario 2: Full Authentication (Production)
**Use Case**: Production deployment with user authentication

**Environment Variables**:
```bash
# Web Service
API_BASE_URL=http://appblueprint-api.railway.internal:80
Logto__AppId=uovd1gg5ef7i1c4w46mt6
Logto__Endpoint=https://32nkyp.logto.app/oidc
Logto__AppSecret=1WYlfj9ekHF3UmomvNsn62JWGa6gVYSy

# API Service
ConnectionStrings__appblueprintdb=your-db-connection
Authentication__Provider=Logto
Authentication__Logto__Endpoint=https://32nkyp.logto.app
Authentication__Logto__ClientId=uovd1gg5ef7i1c4w46mt6
```

**Result**:
- Web: Login required via Logto
- API: Validates Logto JWT tokens
- Full production security

### Scenario 3: Mixed Mode (Web Auth, API Open)
**Use Case**: Web requires login, but API accepts any valid JWT

**Environment Variables**:
```bash
# Web Service - WITH Logto
API_BASE_URL=http://appblueprint-api.railway.internal:80
Logto__AppId=uovd1gg5ef7i1c4w46mt6
Logto__Endpoint=https://32nkyp.logto.app/oidc
Logto__AppSecret=1WYlfj9ekHF3UmomvNsn62JWGa6gVYSy

# API Service - WITHOUT Logto (falls back to custom JWT)
ConnectionStrings__appblueprintdb=your-db-connection
Authentication__Provider=Logto  # Still set to Logto
# But no Endpoint/ClientId - triggers fallback
```

**Result**:
- Web: Login required
- API: Accepts custom JWT tokens
- Useful for gradual migration

## appsettings.json Configuration

### Web Service
```json
{
  "Logto": {
    "Endpoint": "https://32nkyp.logto.app/oidc",
    "AppId": "uovd1gg5ef7i1c4w46mt6",
    "AppSecret": "1WYlfj9ekHF3UmomvNsn62JWGa6gVYSy"
  }
}
```

### API Service
```json
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://32nkyp.logto.app",
      "ClientId": "uovd1gg5ef7i1c4w46mt6"
    },
    "JWT": {
      "SecretKey": "YourSuperSecretKey_ChangeThisInProduction_MustBeAtLeast32Characters!",
      "Issuer": "AppBlueprintAPI",
      "Audience": "AppBlueprintClient",
      "ExpirationMinutes": 60
    }
  }
}
```

**Important**: Railway uses environment variables which override appsettings.json values.

## Token Flow Example

### 1. User Logs In (Web)
```
User → Web → Logto Login Page
           ← User enters credentials
           → Logto validates
           ← Returns ID token + Access token
Web stores tokens in session/cookie
```

### 2. Web Calls API
```
Web → API Request
      Headers: 
        Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
        tenant-id: default-tenant
```

### 3. API Validates Token
```
API → Fetches Logto public keys from:
      https://32nkyp.logto.app/oidc/.well-known/openid-configuration
    → Validates JWT signature
    → Validates issuer, expiration, etc.
    → Extracts user claims (sub, name, email, etc.)
    → Processes request with user context
```

## JWT Token Claims (Logto)

Typical Logto JWT token claims:
```json
{
  "sub": "user-id-12345",
  "iss": "https://32nkyp.logto.app/oidc",
  "aud": "uovd1gg5ef7i1c4w46mt6",
  "exp": 1699999999,
  "iat": 1699996399,
  "name": "John Doe",
  "email": "john@example.com",
  "email_verified": true
}
```

API extracts these claims for authorization decisions.

## Security Considerations

### Production Recommendations

1. **Always Use HTTPS in Production**
   ```csharp
   options.RequireHttpsMetadata = true;  // Set in production
   ```

2. **Set Strong JWT Secret** (if using fallback)
   ```bash
   Authentication__JWT__SecretKey=YourVerySecureRandomKey_AtLeast32Characters_ChangeMeInProduction!
   ```

3. **Enable All Validations**
   ```csharp
   ValidateIssuer = true,
   ValidateAudience = true,  // Logto may not include audience
   ValidateLifetime = true,
   ValidateIssuerSigningKey = true
   ```

4. **Use Environment Variables for Secrets**
   - Never commit secrets to source control
   - Use Railway's environment variable management
   - Rotate secrets periodically

### Development vs Production

| Setting | Development | Production |
|---------|-------------|------------|
| RequireHttpsMetadata | false | true |
| Logto Config | appsettings.json | Environment variables |
| JWT Secret | Default (insecure) | Strong random key |
| Token Validation | Permissive | Strict |
| Audience Validation | Optional | Required (if supported) |

## Troubleshooting

### Issue: API returns 401 Unauthorized
**Possible Causes**:
1. Logto not configured in API
2. Token from Web doesn't match API's expected issuer
3. Token expired
4. Invalid signature

**Solution**:
1. Check API logs for authentication errors
2. Verify `Authentication__Logto__Endpoint` matches Web's `Logto__Endpoint` (minus `/oidc`)
3. Ensure `Authentication__Logto__ClientId` matches Web's `Logto__AppId`
4. Check token hasn't expired

### Issue: "Logto Endpoint or ClientId not configured"
**Expected Behavior**: API falls back to custom JWT validation

**Solution**:
- If you want Logto: Set environment variables
- If you want custom JWT: Provide `Authentication__JWT__SecretKey`
- If you want no auth (testing): This fallback is intentional

### Issue: Token signature validation fails
**Possible Causes**:
1. Logto public keys not fetched
2. Network issue reaching Logto discovery endpoint
3. Wrong endpoint configured

**Solution**:
1. Check API can reach `https://32nkyp.logto.app/oidc/.well-known/openid-configuration`
2. Verify endpoint URL is correct (no trailing slash, no /oidc for API config)
3. Check Railway network policies allow outbound HTTPS

## Files Modified

1. ✅ `AppBlueprint.Presentation.ApiModule/Extensions/JwtAuthenticationExtensions.cs`
   - Made Logto configuration optional in `ConfigureLogto` method
   - Falls back to custom JWT if Endpoint or ClientId missing
   - Helpful console logging

## Expected Console Output

### Railway - Without Logto (Both Services)
```
[Web] Logto authentication NOT configured - running without authentication
[API] Logto Endpoint or ClientId not configured - falling back to custom JWT authentication
[Web] Application started successfully
[API] Application started successfully
```

### Railway - With Logto (Both Services)
```
[Web] Logto authentication configuration found - enabling OpenID Connect
[Web] OpenID Connect configured with Authority: https://32nkyp.logto.app/oidc
[API] Logto authentication configured
[API] Authority: https://32nkyp.logto.app/oidc
[Web] Application started successfully
[API] Application started successfully
```

## Summary

✅ **Web Authentication**: Optional Logto OIDC login  
✅ **API Authentication**: Optional Logto JWT validation  
✅ **Fallback**: Custom JWT if Logto not configured  
✅ **Railway Compatible**: Works with or without Logto env vars  
✅ **Flexible Deployment**: Test without auth, then enable  

Both services can now run in Railway with gradual authentication rollout! 🎉

