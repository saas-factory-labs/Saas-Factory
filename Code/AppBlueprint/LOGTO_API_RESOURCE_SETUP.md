# Logto API Resource Configuration

## Problem
The todos page cannot load data from the API because Logto is returning **opaque access tokens** instead of **JWT access tokens**. The API server cannot validate opaque tokens.

## Root Cause
When no API resource is configured, Logto defaults to issuing opaque (non-JWT) access tokens. These tokens are just random strings that are only valid when calling Logto's userinfo endpoint, not for your own API.

## Solution
Configure an API Resource in Logto to receive JWT access tokens that your API can validate.

### Step 1: Create API Resource in Logto Dashboard

1. Log in to your Logto dashboard: https://32nkyp.logto.app/ (or your Logto instance)

2. Navigate to **API Resources** in the left sidebar

3. Click **Create API Resource**

4. Fill in the details:
   - **API Name**: AppBlueprint API (or any descriptive name)
   - **API Identifier** (Resource Indicator): `https://api.appblueprint.local` 
     - This is a URI that uniquely identifies your API
     - It doesn't need to be a real URL - it's just an identifier
     - Common formats:
       - `https://api.yourdomain.com`
       - `https://api.appblueprint.local`
       - `urn:appblueprint:api`
   
5. Click **Create**

6. Note down the **API Identifier** - you'll need it for the next step

### Step 2: Configure API Resource in Your Application

Add the API Resource identifier to your configuration:

#### Option A: appsettings.json (Development)

```json
{
  "Logto": {
    "Endpoint": "https://32nkyp.logto.app/",
    "AppId": "your-app-id",
    "AppSecret": "your-app-secret",
    "Resource": "https://api.appblueprint.local"
  }
}
```

#### Option B: Environment Variable (Production/Railway)

Set the following environment variable:

```bash
Logto__Resource=https://api.appblueprint.local
```

In Railway:
1. Go to your service settings
2. Navigate to **Variables**
3. Add a new variable:
   - Key: `Logto__Resource`
   - Value: `https://api.appblueprint.local` (use the same identifier from Step 1)

### Step 3: Update API to Validate the Resource

The API server needs to validate that incoming tokens are intended for this API resource. Update the JWT validation configuration in your API service:

In `AppBlueprint.ApiService/Program.cs` or your authentication setup:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://32nkyp.logto.app/oidc";
        options.Audience = "https://api.appblueprint.local"; // Same as Resource identifier
        
        // Other JWT validation options...
    });
```

### Step 4: Restart and Test

1. Restart the Web application
2. Sign out and sign in again to get a new token with the resource claim
3. Try loading the todos page
4. Check the logs - you should see:
   ```
   [AuthHandler] ✅ Retrieved VALID JWT access_token from HttpContext (length: ~800)
   ```
   Instead of:
   ```
   [AuthHandler] ⚠️ access_token in HttpContext is NOT a valid JWT (length: 43)
   ```

## Verification

After configuration, the access token should:
- Be a valid JWT with 3 parts separated by dots
- Be approximately 600-1000 characters long (not 43!)
- Contain claims including:
  - `aud`: The API resource identifier
  - `iss`: The Logto issuer URL
  - `sub`: The user ID
  - `scope`: The requested scopes

## Troubleshooting

### Still Getting Opaque Tokens?

1. **Check configuration**: Verify the `Logto:Resource` setting is correct
2. **Check application logs**: Look for the warning about API Resource not configured
3. **Clear authentication**: Sign out completely and sign in again
4. **Verify API Resource in Logto**: Make sure it's created and the identifier matches exactly

### API Still Rejecting Tokens?

1. **Check audience claim**: The API's `options.Audience` must match the resource identifier
2. **Check issuer**: The API's `options.Authority` must match Logto's OIDC endpoint
3. **Check logs**: Look for JWT validation errors in the API service logs

## Additional Resources

- [Logto API Resources Documentation](https://docs.logto.io/docs/recipes/protect-your-api/)
- [OAuth 2.0 Resource Indicators (RFC 8707)](https://www.rfc-editor.org/rfc/rfc8707.html)
- [JWT Bearer Token Authentication](https://jwt.io/)

## Summary

1. Create API Resource in Logto dashboard
2. Add `Logto:Resource` configuration with the API identifier
3. Update API JWT validation to accept the audience
4. Restart, sign out, sign in again
5. Verify JWT access tokens are being received
