# JWT Authentication Configuration for TodoService

## Problem
The TodoService was receiving **401 Unauthorized** errors when calling the API because JWT authentication tokens were not being included in the HTTP requests.

## Solution
Implemented an `AuthenticationDelegatingHandler` that automatically adds JWT tokens from Logto authentication to all TodoService HTTP requests.

## Implementation Details

### 1. AuthenticationDelegatingHandler
**File:** `AppBlueprint.Web/Services/AuthenticationDelegatingHandler.cs`

This handler:
- Inherits from `DelegatingHandler` to intercept HTTP requests
- Retrieves the JWT token from `ITokenStorageService` (browser local storage)
- Adds the token as a Bearer token in the Authorization header
- Logs authentication status for debugging
- **Handles prerendering gracefully** by catching JavaScript interop exceptions

**Prerendering Support:**
During server-side prerendering, JavaScript interop is not available. The handler catches these exceptions and proceeds without the token, allowing the page to render. Once the Blazor circuit is established (after `OnAfterRenderAsync`), subsequent requests will include the token.

```csharp
protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
{
    try
    {
        // Get token from browser storage
        var token = await _tokenStorageService.GetTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            // Add Bearer token to Authorization header
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
    {
        // During prerendering, JavaScript interop is not available - skip token
        _logger.LogDebug("Skipping token retrieval during prerender");
    }

    return await base.SendAsync(request, cancellationToken);
}
```

### 2. Service Registration
**File:** `AppBlueprint.Web/Program.cs`

The handler is registered and configured with the TodoService HttpClient:

```csharp
// Register authentication handler
builder.Services.AddTransient<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>();

// Configure TodoService with authentication handler
builder.Services.AddHttpClient<AppBlueprint.Web.Services.TodoService>(client =>
{
    client.BaseAddress = new Uri("https+http://apiservice");
})
.AddHttpMessageHandler<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>();
```

## How It Works

### Authentication Flow

1. **User Login (Logto):**
   ```
   User → Logto OAuth/OIDC → JWT Token → Browser Local Storage
   ```

2. **Token Storage:**
   - JWT token is stored via `ITokenStorageService`
   - Key: `auth_token`
   - Location: Browser's local storage

3. **API Request with Authentication:**
   ```
   TodoPage.razor
       ↓ (calls method)
   TodoService
       ↓ (HTTP request)
   AuthenticationDelegatingHandler
       ↓ (retrieves token from storage)
   ITokenStorageService
       ↓ (adds Authorization header)
   HTTP Request with Bearer Token
       ↓
   API Service
   ```

4. **Request Headers:**
   ```http
   GET /api/v1.0/todo HTTP/1.1
   Host: apiservice
   Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

### Existing Infrastructure Used

The solution leverages existing authentication infrastructure:

- **ITokenStorageService:** Already implemented for storing/retrieving tokens
- **TokenStorageService:** Browser local storage implementation using JSInterop
- **Authentication Services:** Already configured in Program.cs via `AddAuthenticationServices()`
- **Logto Integration:** Already configured for OAuth/OIDC authentication

## Benefits

✅ **Automatic Token Injection:** Tokens are automatically added to all TodoService requests
✅ **No Code Changes in Components:** TodoPage.razor doesn't need authentication logic
✅ **Centralized Authentication:** Single point of configuration for API authentication
✅ **Reusable Pattern:** Can be used for other services that need authentication
✅ **Logging Support:** Built-in logging for debugging authentication issues
✅ **Clean Architecture:** Separation of concerns between business logic and authentication

## Verifying It Works

### 1. Check Token Storage
Open browser DevTools:
- Navigate to **Application** tab
- Select **Local Storage** > your domain
- Look for key: `auth_token`
- Value should be a JWT token (long string starting with `eyJ...`)

### 2. Check Request Headers
Open browser DevTools:
- Navigate to **Network** tab
- Trigger a todo operation (add, load, etc.)
- Find the request to `/api/v1.0/todo`
- Check **Headers** tab
- Look for: `Authorization: Bearer eyJ...`

### 3. Check Logs
Look for log messages in the browser console:
- `"Added authentication token to request: GET https://apiservice/api/v1.0/todo"`
- Or warning: `"No authentication token found in storage..."`

## Troubleshooting

### Hot Reload Error After Implementation

**Error:**
```
System.Runtime.CompilerServices.HotReloadException: Attempted to invoke a deleted lambda or local function implementation.
```

**Cause:**  
This error occurs because we modified HttpClient service registration in Program.cs while the app was running. Hot reload cannot handle DI configuration changes.

**Solution:**  
**Restart the application** (stop and start the AppHost). This is a one-time issue after implementing the authentication handler.

See [HOT_RELOAD_ERROR_FIX.md](./HOT_RELOAD_ERROR_FIX.md) for detailed restart instructions.

### JavaScript Interop Error During Prerendering

**Error:**
```
JavaScript interop calls cannot be issued at this time. This is because the component is being statically rendered.
```

**Cause:**  
Blazor Server apps use prerendering by default, which means components are first rendered on the server without JavaScript interop. The `AuthenticationDelegatingHandler` tries to access browser local storage during this phase.

**Solution Already Implemented:**  
The handler now catches JavaScript interop exceptions during prerendering and proceeds without the token. The TodoPage loads data in `OnAfterRenderAsync` instead of `OnInitializedAsync`, ensuring JavaScript interop is available.

**How it works:**
1. **During prerender:** Handler skips token retrieval (no JavaScript available)
2. **After interactive render:** Handler successfully retrieves token from local storage
3. **Subsequent requests:** Include authentication token normally

**No action required** - this is handled automatically in the implementation.

### Still Getting 401 Unauthorized?

**Check if logged in:**
```
1. Navigate to login page
2. Complete Logto authentication
3. Verify redirect back to app
4. Check local storage for auth_token
```

**Token Expired:**
```
JWT tokens typically expire after 1 hour.
Solution: Log out and log back in to get fresh token.
```

**Token Not Added to Request:**
```
Check browser console for handler logs.
Verify AuthenticationDelegatingHandler is registered.
```

**API Service Authentication Configuration:**
```
Ensure API service is configured to accept JWT tokens.
Check JWT validation settings in API appsettings.json.
```

### Debug Logging

The handler logs at different levels:

**Debug Level:**
- `"Added authentication token to request: {Method} {Uri}"`
- Logged when token is successfully added

**Warning Level:**
- `"No authentication token found in storage for request: {Method} {Uri}"`
- Logged when no token is available

Enable debug logging in appsettings.json:
```json
{
  "Logging": {
    "LogLevel": {
      "AppBlueprint.Web.Services": "Debug"
    }
  }
}
```

## Alternative Approaches Considered

### 1. ❌ Manual Token Passing
```csharp
// Would require changing every service method
await TodoService.GetTodosAsync(token);
```
**Why not:** Too much boilerplate, error-prone, couples components to authentication

### 2. ❌ Base HttpClient Configuration
```csharp
// Would require access to IServiceProvider in configuration
client.DefaultRequestHeaders.Authorization = ...;
```
**Why not:** Cannot access scoped services (ITokenStorageService) during configuration

### 3. ✅ DelegatingHandler (Chosen Solution)
```csharp
// Clean, automatic, reusable
.AddHttpMessageHandler<AuthenticationDelegatingHandler>();
```
**Why:** Follows .NET best practices, clean separation of concerns, reusable

## Future Enhancements

Potential improvements:
- [ ] Automatic token refresh on 401 responses
- [ ] Token expiration checking before requests
- [ ] Retry logic for authentication failures
- [ ] Support for multiple authentication schemes
- [ ] Per-request authentication override capability

## Related Files

- `AuthenticationDelegatingHandler.cs` - New handler implementation
- `TodoService.cs` - Service using authenticated HttpClient
- `Program.cs` - Handler registration
- `ITokenStorageService.cs` - Token storage interface (existing)
- `TokenStorageService.cs` - Browser storage implementation (existing)

## Testing Checklist

- [x] Handler registered in DI container
- [x] Handler added to TodoService HttpClient pipeline
- [x] Token retrieved from storage
- [x] Authorization header added to requests
- [x] Logging implemented for debugging
- [x] Documentation updated
- [ ] Test with valid token (requires running app)
- [ ] Test with expired token (requires running app)
- [ ] Test without token (requires running app)
- [ ] Verify API accepts token (requires API implementation)

