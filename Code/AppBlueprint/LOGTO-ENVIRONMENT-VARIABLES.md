# Logto Environment Variables

## Required Environment Variables

All Logto-related environment variables **MUST be UPPERCASE with single underscores** for consistency.

### Web App (AppBlueprint.Web)

```bash
# Logto OpenID Connect Configuration
LOGTO_ENDPOINT="https://32nkyp.logto.app/oidc"
LOGTO_APPID="uovd1gg5ef7i1c4w46mt6"
LOGTO_APPSECRET="<your-secret-from-doppler>"

# API Resource (CRITICAL - Required for JWT access tokens)
LOGTO_RESOURCE="https://api.appblueprint.local"
```

**Without `LOGTO_RESOURCE`**, Logto will issue **opaque tokens** (43 characters) instead of **JWT tokens** (~800 characters), causing all API calls to fail with 401 Unauthorized.

### API Service (AppBlueprint.ApiService)

```bash
# Authentication Provider
AUTHENTICATION_PROVIDER="Logto"

# Logto JWT Validation Configuration
AUTHENTICATION_LOGTO_ENDPOINT="https://32nkyp.logto.app"
AUTHENTICATION_LOGTO_CLIENTID="uovd1gg5ef7i1c4w46mt6"

# API Resource (CRITICAL - Validates JWT audience claim)
AUTHENTICATION_LOGTO_APIRESOURCE="https://api.appblueprint.local"
```

**Without `AUTHENTICATION_LOGTO_APIRESOURCE`**, JWT validation will fail because the `aud` claim won't match.

---

## Doppler Setup

Add these secrets to Doppler (development environment):

```bash
cd D:\Development\Development-Projects\Saas-Factory\Code\AppBlueprint\AppBlueprint.AppHost

# Web app secrets
doppler secrets set LOGTO_RESOURCE="https://api.appblueprint.local"
doppler secrets set LOGTO_APPSECRET="<your-secret>"

# API service secrets are configured in AppHost Program.cs
```

---

## Verification

After adding variables and restarting AppHost, check logs for:

```
[Web] ✅ API Resource configured: https://api.appblueprint.local
[Web] ✅ Requesting scopes: read:todos, read:files, write:files
[Web] AccessToken length: 800+ (JWT format)
[AuthHandler] ✅ Retrieved VALID JWT access_token
```

**Red flags** (indicates missing configuration):

```
[Web] ⚠️ WARNING: No API Resource configured - will receive OPAQUE access tokens
[Web] AccessToken length: 43 (opaque format - WRONG!)
[AuthHandler] ⚠️ access_token is NOT a valid JWT
```

---

## Logto Dashboard Configuration

1. **API Resource**: Create `https://api.appblueprint.local` in Logto Console → API Resources
2. **Permissions**: Add scopes:
   - `read:todos`
   - `read:files`
   - `write:files`
3. **Machine-to-Machine App**: Assign permissions to your application
4. **Redirect URIs**: Configure `http://localhost:9200/callback` and `/signout-callback-logto`

---

## Troubleshooting

| Issue | Cause | Solution |
|-------|-------|----------|
| 401 Unauthorized | Missing `LOGTO_RESOURCE` | Add to Doppler, restart AppHost |
| Opaque tokens (43 chars) | API Resource not requested | Set `LOGTO_RESOURCE` environment variable |
| JWT without scopes | Permissions not assigned in Logto | Add permissions in Logto Dashboard |
| Audience validation fails | Mismatched API Resource URLs | Ensure both Web and API use same URL |
