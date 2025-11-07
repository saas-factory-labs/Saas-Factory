# 🔍 API AUTHENTICATION 401 UNAUTHORIZED - TROUBLESHOOTING

## 🎯 Current Status

**Good News:** 
- ✅ Token is being retrieved from HttpContext
- ✅ Token is being sent to API in Authorization header
- ✅ Headers look correct: `Authorization: Bearer {token}`, `tenant-id: default-tenant`

**Problem:**
- ❌ API returns `401 Unauthorized`
- ❌ Token validation is failing on the API side

---

## 🔍 Root Cause Analysis

The API is configured to validate Logto tokens, but the validation is failing. Possible reasons:

### 1. **ID Token vs Access Token**
- The handler tries `access_token` first, then falls back to `id_token`
- Logto `id_token` might not have the claims the API expects
- API might need a proper `access_token` with audience claim

### 2. **Token Validation Settings**
- API's `JwtAuthenticationExtensions.cs` validates against Logto
- Issuer must match: `https://32nkyp.logto.app/oidc`
- API has `ValidateAudience = false` which is good

### 3. **Logto Application Configuration**
- Logto application might not be configured to issue access tokens
- Resource indicators might be missing
- Scopes might not include API access

---

## 🔧 IMMEDIATE FIXES TO TRY

### Fix 1: Check API Logs (MOST IMPORTANT)

The API has detailed JWT authentication logging enabled. Check the console logs for:

```
[ERROR] Authentication failed. Token preview: {token}, Exception Type: {type}, Message: {message}
```

This will tell us exactly why the token is being rejected.

### Fix 2: Verify Logto Configuration

In Logto Console (`https://32nkyp.logto.app`):

1. **Go to your Application** (ID: `uovd1gg5ef7i1c4w46mt6`)

2. **Check "Application Type":**
   - Should be: `Traditional Web App`

3. **Check "Grant Types":**
   - ✅ Authorization Code must be enabled
   - ✅ Refresh Token should be enabled

4. **Check "Resources" or "API Resources":**
   - If Logto requires API resources to be configured
   - Add your API as a resource
   - Configure allowed scopes

5. **Check "Token Settings":**
   - Ensure tokens include necessary claims
   - Check token expiration

### Fix 3: Request Proper Scopes in Web App

Update the OpenID Connect configuration in `Program.cs` to request API access:

```csharp
// Add required scopes
options.Scope.Clear();
options.Scope.Add("openid");
options.Scope.Add("profile");
options.Scope.Add("email");
options.Scope.Add("offline_access");  // For refresh tokens
// Add any API-specific scopes here if Logto requires them
```

### Fix 4: Disable Token Validation Temporarily (TESTING ONLY)

To verify the issue is with token validation, temporarily disable validation in the API:

**File:** `AppBlueprint.Presentation.ApiModule/Extensions/JwtAuthenticationExtensions.cs`

In `ConfigureLogto` method, add:

```csharp
// TEMPORARY - FOR TESTING ONLY
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = false,  // Disable for testing
    ValidateAudience = false,
    ValidateLifetime = false,  // Disable for testing
    ValidateIssuerSigningKey = false,  // Disable for testing
    SignatureValidator = (token, parameters) => new JwtSecurityToken(token)  // Accept any token
};
```

**⚠️ WARNING:** This disables security! Only use for testing to identify the issue.

---

## 🔍 Diagnostic Steps

### Step 1: Check API Console Logs

Look for these log messages:

**Authentication Failed:**
```
[ERROR] Authentication failed. Token preview: xxx, Exception Type: SecurityTokenException
```

**Token Validated (Success):**
```
[INFO] Token validated successfully. User: {name}, UserId: {id}, Issuer: {issuer}
```

**Challenge:**
```
[WARNING] Authorization challenge. Error: {error}, ErrorDescription: {desc}
```

### Step 2: Decode the Token

Copy the token from the diagnostic output and decode it at https://jwt.io

Check:
- `iss` (issuer) - Should be `https://32nkyp.logto.app/oidc`
- `aud` (audience) - Might be missing or wrong
- `exp` (expiration) - Make sure not expired
- `sub` (subject) - User ID
- Claims present

### Step 3: Compare Token with API Expectations

**API expects (from `ConfigureLogto`):**
```
Issuer: https://32nkyp.logto.app/oidc
Audience: Not validated (ValidateAudience = false)
Lifetime: Must be valid
Signature: Must be valid (from Logto's JWKS)
```

---

## 🚀 RECOMMENDED SOLUTION

Based on the diagnostic output showing the token is being sent correctly, the most likely issue is:

**The token being sent is an ID token, not an access token suitable for API calls.**

### Solution: Configure Logto for API Access

1. **In Logto Console, create or configure an API Resource:**
   - Name: "AppBlueprint API" or similar
   - Identifier: Your API URL or a custom identifier
   - Configure scopes if needed

2. **Update your Web Application to request this API:**
   - In application settings, grant access to the API resource
   - Ensure the application can request access tokens for the API

3. **Update OpenID Connect configuration to request API resource:**

```csharp
// In Program.cs, AddOpenIdConnect configuration:
options.Scope.Add("api");  // Or whatever scope Logto requires for your API
options.Resource = "your-api-identifier";  // If Logto uses resource parameter
```

4. **Verify access_token is now available:**
   - After login, the `HttpContext.GetTokenAsync("access_token")` should return a token
   - This token will have the API as audience
   - API will be able to validate it

---

## 📋 Quick Checklist

### In Logto Console:
- [ ] Application type is "Traditional Web App"
- [ ] Authorization Code grant type enabled
- [ ] API Resource configured (if applicable)
- [ ] Application has access to API Resource
- [ ] Redirect URIs include `http://localhost:8092/callback`

### In Web App (Program.cs):
- [ ] `SaveTokens = true` (already set ✅)
- [ ] Scopes include what Logto needs
- [ ] Resource parameter set (if needed)

### In API (appsettings.json):
- [ ] `Authentication:Provider` = "Logto"
- [ ] `Authentication:Logto:Endpoint` = "https://32nkyp.logto.app"
- [ ] `Authentication:Logto:ClientId` = "uovd1gg5ef7i1c4w46mt6"

---

## 🎯 NEXT STEPS

1. **Check API console logs** to see the exact authentication failure reason
2. **Decode the token** at jwt.io to see what claims it has
3. **Check Logto Console** for API resource configuration
4. **If needed, temporarily disable validation** to verify that's the issue
5. **Configure Logto properly** for API access tokens

---

## 📞 IF STILL STUCK

Share:
1. API console log showing authentication failure
2. Decoded token contents (from jwt.io)
3. Screenshot of Logto application configuration
4. Screenshot of Logto API resource configuration (if exists)

---

**Date:** 2025-11-07  
**Issue:** API returning 401 Unauthorized despite token being sent  
**Most Likely Cause:** ID token being used instead of access token, or token validation failing  
**Action:** Check API logs and Logto console configuration

🔍 **Check the API console logs first - they will tell us exactly why authentication is failing!**

