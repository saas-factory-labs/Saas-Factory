# JWT Authentication Testing Guide

## Quick Start Testing

I've created a test controller (`AuthTestController`) with several endpoints to test JWT validation.

### Test Endpoints Available

1. **`GET /api/v1/authtest/public`** - No auth required (baseline test)
2. **`GET /api/v1/authtest/protected`** - Requires valid JWT token
3. **`GET /api/v1/authtest/admin`** - Requires Admin role
4. **`GET /api/v1/authtest/user`** - Requires User or Admin role
5. **`GET /api/v1/authtest/echo`** - Returns request details (requires auth)

## Testing Methods

### Method 1: PowerShell Script (Easiest)

I've created a PowerShell script to generate test tokens and call the API.

Run: `.\test-jwt-auth.ps1`

### Method 2: Swagger UI

1. Start the API Service
2. Navigate to `https://localhost:5002/swagger` (or your API URL)
3. Click the **"Authorize"** button (top right)
4. Generate a test token (see "Generate Test Token" below)
5. Enter: `Bearer {your-token}`
6. Click **Authorize**
7. Try the `/api/v1/authtest/protected` endpoint

### Method 3: Postman/Insomnia

1. Create a new GET request to: `https://localhost:5002/api/v1/authtest/protected`
2. Go to **Authorization** tab
3. Select **Bearer Token**
4. Paste your JWT token
5. Send request

### Method 4: Blazor App (End-to-End)

This tests the full flow from Blazor → API:

1. Run both AppHost (which starts Web and API)
2. Login to Blazor app using Mock/Auth0/Logto
3. Navigate to a page that calls the API
4. Check browser console for any 401 errors

## Generate Test Tokens

### Option A: Using PowerShell Script

The `test-jwt-auth.ps1` script generates tokens automatically.

### Option B: Using Online JWT Generator

1. Go to https://jwt.io/
2. In the **PAYLOAD** section, paste:
```json
{
  "sub": "test-user-123",
  "name": "Test User",
  "email": "test@example.com",
  "role": "User",
  "nbf": 1699000000,
  "exp": 9999999999,
  "iss": "AppBlueprintAPI",
  "aud": "AppBlueprintClient"
}
```
3. In the **VERIFY SIGNATURE** section, paste your secret key:
   - Default: `YourSuperSecretKey_ChangeThisInProduction_MustBeAtLeast32Characters!`
   - Or get from your `appsettings.json`: `Authentication:JWT:SecretKey`
4. Copy the encoded token (left panel)

### Option C: Using C# Console App

Create a quick token generator (see `JwtTokenGenerator.cs` file I'll create next).

## Test Scenarios

### ✅ Scenario 1: Public Endpoint (No Token)

**Request:**
```bash
curl https://localhost:5002/api/v1/authtest/public
```

**Expected Result:** 200 OK
```json
{
  "message": "This is a public endpoint - no authentication required",
  "isAuthenticated": false
}
```

### ✅ Scenario 2: Protected Endpoint (No Token)

**Request:**
```bash
curl https://localhost:5002/api/v1/authtest/protected
```

**Expected Result:** 401 Unauthorized

**What to check in logs:**
- "Authorization challenge" warning
- "Missing or invalid token"

### ✅ Scenario 3: Protected Endpoint (Valid Token)

**Request:**
```bash
curl -H "Authorization: Bearer {your-token}" https://localhost:5002/api/v1/authtest/protected
```

**Expected Result:** 200 OK
```json
{
  "message": "Successfully authenticated!",
  "user": {
    "isAuthenticated": true,
    "name": "Test User",
    "userId": "test-user-123",
    "email": "test@example.com"
  }
}
```

**What to check in logs:**
- "Token validated successfully for user: Test User"

### ✅ Scenario 4: Protected Endpoint (Expired Token)

Use a token with past expiration date.

**Expected Result:** 401 Unauthorized

**What to check in logs:**
- "Authentication failed" error
- Exception about token expiration

### ✅ Scenario 5: Protected Endpoint (Invalid Signature)

Modify any part of the token or use wrong secret key.

**Expected Result:** 401 Unauthorized

**What to check in logs:**
- "Authentication failed" error
- Exception about invalid signature

### ✅ Scenario 6: Admin Endpoint (User Role)

Use token with "User" role (not "Admin").

**Expected Result:** 403 Forbidden

### ✅ Scenario 7: Admin Endpoint (Admin Role)

Generate token with:
```json
{
  "role": "Admin",
  ...other claims
}
```

**Expected Result:** 200 OK

## Testing from Blazor Server

### Step 1: Verify Token Acquisition

Add this to a Blazor page:

```razor
@inject ITokenStorageService TokenStorage

<button @onclick="CheckToken">Check Token</button>
<p>@tokenStatus</p>

@code {
    private string tokenStatus = "";
    
    private async Task CheckToken()
    {
        var token = await TokenStorage.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            tokenStatus = "❌ No token found - user may not be authenticated";
        }
        else
        {
            tokenStatus = $"✅ Token found: {token.Substring(0, Math.Min(50, token.Length))}...";
        }
    }
}
```

### Step 2: Test API Call

```razor
@inject ApiClient ApiClient

<button @onclick="TestApiCall">Test Protected Endpoint</button>
<p>@apiResult</p>

@code {
    private string apiResult = "";
    
    private async Task TestApiCall()
    {
        try
        {
            // This will automatically include the JWT token
            var response = await ApiClient.Api.V1.Authtest.Protected.GetAsync();
            apiResult = "✅ Success! API call authenticated";
        }
        catch (Exception ex)
        {
            apiResult = $"❌ Error: {ex.Message}";
        }
    }
}
```

### Step 3: Monitor Network Traffic

1. Open browser DevTools (F12)
2. Go to **Network** tab
3. Make API call from Blazor
4. Find the request to your API
5. Check **Headers** → Look for `Authorization: Bearer ...`
6. Verify token is being sent

## Troubleshooting Guide

### Issue: Always Getting 401 Unauthorized

**Check:**
1. Token is being sent: Look in request headers
2. Token format: Must be `Authorization: Bearer {token}`
3. Secret key matches: API and token generator use same key
4. Issuer/Audience match: Check `appsettings.json` values
5. Token not expired: Check `exp` claim

**Debug steps:**
```powershell
# Check API logs for authentication errors
# Look for "Authentication failed" messages
# Check the exception details
```

### Issue: Token Not Being Sent from Blazor

**Check:**
1. User is authenticated
2. Token is stored in `TokenStorageService`
3. `IAuthenticationProvider` is adding the header
4. CORS is not stripping headers

**Debug steps:**
```csharp
// Add logging to your authentication provider
_logger.LogInformation("Adding auth header: {Header}", $"Bearer {token.Substring(0, 20)}...");
```

### Issue: 403 Forbidden (Not 401)

**Meaning:** Token is valid, but user lacks required role/claims

**Check:**
1. Token contains required role claim
2. Claim type matches (e.g., `role` vs `http://schemas.microsoft.com/ws/2008/06/identity/claims/role`)
3. Policy configuration in `JwtAuthenticationExtensions`

### Issue: CORS Errors

If you see CORS errors in browser console:

**Fix in API Program.cs:**
```csharp
app.UseCors(); // Ensure this is BEFORE UseAuthentication
app.UseAuthentication();
app.UseAuthorization();
```

## Automated Testing

### Integration Test Example

See `AppBlueprint.Tests/JwtAuthenticationTests.cs` (I'll create this next).

### Run Tests

```powershell
cd Code\AppBlueprint\AppBlueprint.Tests
dotnet test --filter "Category=JwtAuth"
```

## Success Indicators

✅ **Everything is working if:**

1. Public endpoint returns 200 without token
2. Protected endpoint returns 401 without token
3. Protected endpoint returns 200 with valid token
4. Token validation logs appear in console
5. User claims are correctly extracted
6. Role-based authorization works
7. Blazor → API calls succeed when logged in

## Next Steps

1. Run the PowerShell test script
2. Try Swagger UI testing
3. Test from your Blazor app
4. Review logs for authentication events
5. Add `[Authorize]` to your actual controllers
6. Test with real Auth0/Logto tokens

## Configuration Checklist

- [ ] `appsettings.json` has `Authentication` section
- [ ] `Authentication:Provider` is set (JWT/Auth0/Logto)
- [ ] Secret key is configured (for JWT mode)
- [ ] API Program.cs calls `AddJwtAuthentication()`
- [ ] API Program.cs calls `UseAuthentication()` and `UseAuthorization()`
- [ ] Controllers have `[Authorize]` attribute
- [ ] Blazor has `IAuthenticationProvider` configured
- [ ] Tokens are being stored in `TokenStorageService`

## Monitoring

Watch these logs while testing:

**API Service logs:**
- "Token validated successfully for user: {User}"
- "Authentication failed" (on invalid token)
- "Authorization challenge" (on missing token)

**Blazor Web logs:**
- Token acquisition from Auth0/Logto/Mock
- Token storage in `TokenStorageService`
- HTTP request with Authorization header

