# Authentication Configuration Fix - 401 Unauthorized

## Problem Identified

**Root Cause:** Authentication provider mismatch between Web and API projects.

### The Issue:
```
❌ Web Project:  Authentication:Provider = "Mock"  (in appsettings.json)
✅ Web Project:  Authentication:Provider = "Logto" (in appsettings.Development.json)
❌ API Project:  Authentication:Provider = "JWT"   (expecting custom JWT)
```

**What was happening:**
1. User logs in via Logto OAuth/OIDC in the Web app
2. Logto issues a JWT token with:
   - Issuer: `https://32nkyp.logto.app/oidc`
   - Audience: `uovd1gg5ef7i1c4w46mt6` (Logto ClientId)
3. Web app stores token in browser localStorage
4. TodoService sends request with `Authorization: Bearer {logto-token}`
5. API tries to validate token using custom JWT settings:
   - Expected Issuer: `AppBlueprintAPI`
   - Expected Audience: `AppBlueprintClient`
   - Expected Signing Key: Custom secret key
6. ❌ **Token validation fails** → 401 Unauthorized

## Solution Applied

### Changed API Authentication Provider

**File:** `AppBlueprint.ApiService/appsettings.json`

```json
{
  "Authentication": {
    "Provider": "Logto",  // ← Changed from "JWT" to "Logto"
    "Logto": {
      "Endpoint": "https://32nkyp.logto.app",
      "ClientId": "uovd1gg5ef7i1c4w46mt6"
    }
  }
}
```

### How JWT Validation Works Now

The API now validates Logto-issued JWT tokens with:
- **Authority:** `https://32nkyp.logto.app/oidc`
- **Issuer:** `https://32nkyp.logto.app/oidc`
- **Audience:** `uovd1gg5ef7i1c4w46mt6`
- **Signing Keys:** Downloaded from Logto's OIDC discovery endpoint

## What Permissions Are Required?

### For Todo Endpoints:

The `TodoController` uses the `[Authorize]` attribute, which means:

**Minimum Requirement:**
- ✅ **Valid JWT token** from Logto
- ✅ **Token not expired**
- ✅ **Token signature valid**
- ✅ **Token issuer matches** Logto endpoint
- ✅ **Token audience matches** ClientId

**No specific roles or permissions required!**

The controller only requires **authenticated user** - that's it!

```csharp
[Authorize]  // ← Just requires authentication, no specific roles
[ApiController]
public class TodoController : ControllerBase
{
    [HttpGet]  // Any authenticated user can access
    public Task<ActionResult<IEnumerable<TodoEntity>>> GetTodosAsync()
    {
        // ...
    }
}
```

### User Claims Available

When a user logs in via Logto, the JWT token contains claims like:
- `sub` - User ID (subject)
- `iss` - Issuer (Logto endpoint)
- `aud` - Audience (ClientId)
- `exp` - Expiration time
- `iat` - Issued at time
- `email` - User's email (if requested in scope)
- `name` - User's name (if requested in scope)

**Currently, the TodoController doesn't check for specific claims or roles.**

## Authentication Flow (Complete)

```
1. User clicks "Login" in Web app
   ↓
2. Redirected to Logto login page
   ↓
3. User enters credentials
   ↓
4. Logto validates credentials
   ↓
5. Logto redirects back with authorization code
   ↓
6. Web app exchanges code for tokens
   ↓
7. JWT access token stored in localStorage
   ↓
8. User navigates to /todos page
   ↓
9. TodoPage loads (OnAfterRenderAsync)
   ↓
10. TodoService.GetTodosAsync() called
   ↓
11. AuthenticationDelegatingHandler adds Bearer token
   ↓
12. HTTP Request → API Service
   ↓
13. API receives request with Authorization header
   ↓
14. JWT Middleware validates token:
    - Downloads Logto signing keys (JWKS)
    - Validates signature
    - Checks issuer, audience, expiration
   ↓
15. ✅ Token valid → Request proceeds to controller
   ↓
16. TodoController.GetTodosAsync() executes
   ↓
17. Returns todos for the authenticated user
```

## Token Validation Details

### What the API Checks:

1. **Token Format:**
   - Must be in format: `Bearer {token}`
   - Token must be valid JWT (base64 encoded JSON)

2. **Signature Validation:**
   - Downloads public keys from: `https://32nkyp.logto.app/oidc/.well-known/jwks.json`
   - Verifies token signature using Logto's public key
   - Ensures token hasn't been tampered with

3. **Claims Validation:**
   - **Issuer (`iss`):** Must be `https://32nkyp.logto.app/oidc`
   - **Audience (`aud`):** Must be `uovd1gg5ef7i1c4w46mt6`
   - **Expiration (`exp`):** Token must not be expired
   - **Not Before (`nbf`):** Token must be valid now (if present)

4. **Clock Skew:**
   - Allows 5 minutes of clock skew for expiration checks
   - Handles time differences between servers

### What the API Does NOT Check:

❌ **Specific roles** - No role-based authorization configured
❌ **Specific permissions** - No permission-based authorization configured
❌ **User attributes** - No custom claim validation
❌ **Tenant isolation** - Not yet implemented (TODO in controller)

## Configuration Files

### Web Project (Already Correct)

**appsettings.json:**
```json
{
  "Authentication": {
    "Provider": "Mock"  // Default for non-development
  }
}
```

**appsettings.Development.json:** (Takes precedence in development)
```json
{
  "Authentication": {
    "Provider": "Logto",  // ✅ Uses Logto in development
    "Logto": {
      "Endpoint": "https://32nkyp.logto.app/",
      "ClientId": "uovd1gg5ef7i1c4w46mt6",
      "ClientSecret": "1WYlfj9ekHF3UmomvNsn62JWGa6gVYSy",
      "Scope": "openid profile email offline_access",
      "UseAuthorizationCodeFlow": true
    }
  }
}
```

### API Project (Fixed)

**appsettings.json:** (NOW UPDATED)
```json
{
  "Authentication": {
    "Provider": "Logto",  // ✅ Changed from "JWT" to "Logto"
    "Logto": {
      "Endpoint": "https://32nkyp.logto.app",
      "ClientId": "uovd1gg5ef7i1c4w46mt6"
    }
  }
}
```

**Note:** API doesn't need ClientSecret because it only validates tokens, doesn't request them.

## Verifying the Fix

### 1. Check Logto Token

**Browser DevTools → Application → Local Storage:**
```
Key: auth_token
Value: eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IjEyMyJ9.eyJzdWIiOiJ1c2VyMTIzIiwiaXNzIjoiaHR0cHM6Ly8zMm5reXAubG9ndG8uYXBwL29pZGMiLCJhdWQiOiJ1b3ZkMWdnNWVmN2kxYzR3NDZtdDYiLCJleHAiOjE3MzEwMDAwMDB9...
```

**Decode at jwt.io:**
- Header: Algorithm RS256, key ID present
- Payload: Check iss, aud, exp claims
- Signature: Will show "Signature Verified" if valid

### 2. Check API Logs

After restart, look for:
```
[Information] Token validated successfully for user: {User}
```

Or if failing:
```
[Error] Authentication failed. Token preview: {TokenPreview}
[Warning] Authorization challenge. Error: {Error}
```

### 3. Check Network Request

**Browser DevTools → Network → Request to /api/v1.0/todo:**

**Request Headers:**
```
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response:**
- ✅ **200 OK:** Authentication successful
- ❌ **401 Unauthorized:** Authentication still failing

### 4. Test Todo Operations

1. Navigate to `/todos` page
2. Should see empty todo list (or existing todos)
3. Try adding a new todo
4. Should succeed without 401 errors

## Troubleshooting

### Still Getting 401 After Fix?

**1. Restart Required:**
```bash
# Authentication configuration changes require restart
Stop AppHost (Ctrl+C or Shift+F5)
Start AppHost (dotnet run or F5)
```

**2. Check Token is Present:**
```javascript
// Browser Console
localStorage.getItem('auth_token')
// Should return a long JWT string
```

**3. Check Token Expiration:**
```javascript
// Browser Console
const token = localStorage.getItem('auth_token');
const payload = JSON.parse(atob(token.split('.')[1]));
console.log('Expires:', new Date(payload.exp * 1000));
console.log('Is expired:', Date.now() > payload.exp * 1000);
```

**If expired:**
- Log out
- Log back in
- Get fresh token

**4. Check API is Using Logto:**
```
API Logs should show:
[Information] Token validated successfully...

If seeing:
[Error] Authentication failed...

Then configuration not loaded correctly - check appsettings.json
```

**5. Verify Logto Endpoint:**
```bash
# Test Logto discovery endpoint
curl https://32nkyp.logto.app/oidc/.well-known/openid-configuration

# Should return JSON with issuer, jwks_uri, etc.
```

## Advanced: Adding Role-Based Authorization

If you want to restrict todo operations to specific roles in the future:

```csharp
[Authorize(Roles = "TodoUser")]  // Require specific role
[HttpGet]
public Task<ActionResult<IEnumerable<TodoEntity>>> GetTodosAsync()
{
    // ...
}

// Or use policies:
[Authorize(Policy = "CanManageTodos")]
[HttpPost]
public Task<ActionResult<TodoEntity>> CreateTodoAsync(...)
{
    // ...
}
```

**But currently:** No roles/policies required, just authentication!

## Security Considerations

### Current Security:

✅ **JWT signature validation** - Tokens can't be forged
✅ **Expiration checking** - Tokens expire after set time
✅ **HTTPS in production** - Tokens encrypted in transit
✅ **Token storage** - Stored in localStorage (acceptable for demo)

### Future Improvements:

🔄 **Tenant isolation** - Users should only see their own todos
🔄 **Role-based access** - Different permissions for different users
🔄 **Refresh tokens** - Automatic token renewal
🔄 **Token revocation** - Ability to invalidate tokens
🔄 **HttpOnly cookies** - More secure than localStorage

## Summary

### What Changed:
✅ API authentication provider changed from "JWT" to "Logto"
✅ API now validates Logto-issued JWT tokens correctly

### What Permissions Needed:
✅ **Just authentication** - User must be logged in via Logto
✅ **No specific roles required**
✅ **No specific permissions required**

### Next Steps:
1. **Restart the application**
2. **Log in via Logto** (if not already logged in)
3. **Navigate to /todos**
4. **Should work without 401 errors!** 🎉

---

## Related Documentation

- **[JWT_AUTHENTICATION_CONFIGURATION.md](./JWT_AUTHENTICATION_CONFIGURATION.md)** - Authentication handler implementation
- **[PRERENDERING_FIX.md](./PRERENDERING_FIX.md)** - JavaScript interop fix
- **[TODO_IMPLEMENTATION.md](./TODO_IMPLEMENTATION.md)** - Complete feature documentation

