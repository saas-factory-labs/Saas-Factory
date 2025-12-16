# JWT Token Validation Guide

## Overview

This guide explains how JWT token validation works when connecting from Blazor Server to the API Service.

## What Was Implemented

### ✅ Standard Microsoft JWT Validation (Not Reinvented)

The implementation uses **Microsoft's official JWT Bearer authentication**:
- `Microsoft.AspNetCore.Authentication.JwtBearer` package
- Standard `AddAuthentication()` and `AddJwtBearer()` methods
- Built-in `TokenValidationParameters` for validation

**No custom token validation logic was created** - everything uses Microsoft's battle-tested implementation.

## Architecture

```
┌─────────────────┐         JWT Token          ┌──────────────────┐
│  Blazor Server  │ ─────────────────────────> │   API Service    │
│     (Web)       │    (in Authorization       │                  │
│                 │         header)             │  Validates JWT   │
└─────────────────┘                             │  using Microsoft │
                                                │  JwtBearer       │
                                                └──────────────────┘
```

## How It Works

### 1. **Blazor Server Side (Token Acquisition)**

Your existing infrastructure handles getting tokens:
- **Auth0Provider** - Gets tokens from Auth0
- **LogtoProvider** - Gets tokens from Logto  
- **MockProvider** - Mock tokens for testing
- **TokenStorageService** - Stores tokens client-side
- **IAuthenticationProvider** - Adds tokens to HTTP requests

### 2. **API Service Side (Token Validation)**

The new `JwtAuthenticationExtensions` configures Microsoft's JWT middleware to:
1. Extract the JWT from the `Authorization: Bearer {token}` header
2. Validate the token signature
3. Check issuer, audience, and expiration
4. Create a `ClaimsPrincipal` with user claims
5. Make it available via `HttpContext.User`

## Configuration

### API Service (`appsettings.json`)

```json
{
  "Authentication": {
    "Provider": "JWT",  // or "Auth0" or "Logto"
    "JWT": {
      "SecretKey": "YourSuperSecretKey_ChangeThisInProduction_MustBeAtLeast32Characters!",
      "Issuer": "AppBlueprintAPI",
      "Audience": "AppBlueprintClient",
      "ExpirationMinutes": 60
    }
  }
}
```

### For Auth0

```json
{
  "Authentication": {
    "Provider": "Auth0",
    "Auth0": {
      "Domain": "https://your-domain.auth0.com",
      "Audience": "https://your-api.example.com"
    }
  }
}
```

### For Logto

```json
{
  "Authentication": {
    "Provider": "Logto",
    "Logto": {
      "Endpoint": "https://your-logto-instance.logto.app",
      "ClientId": "your-logto-client-id"
    }
  }
}
```

## Usage in Controllers

### Require Authentication

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires valid JWT token
public class TodoController : ControllerBase
{
    [HttpGet]
    public IActionResult GetTodos()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.Identity?.Name;
        
        // Your logic here
    }
}
```

### Use Authorization Policies

```csharp
[HttpDelete("{id}")]
[Authorize(Policy = "AdminOnly")]
public IActionResult Delete(int id)
{
    // Only users with "Admin" role can access
}
```

## Blazor Server Token Flow

### How Blazor Sends Tokens

Your existing `IAuthenticationProvider` implementations (in `AppBlueprint.Infrastructure.Authorization`) automatically add tokens to API requests:

```csharp
// This is already implemented in your codebase
public class UserAuthenticationProviderAdapter : IUserAuthenticationProvider
{
    public async Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        // Gets token from TokenStorageService
        var token = await GetAccessTokenAsync();
        
        // Adds to request header
        request.Headers.TryAdd("Authorization", $"Bearer {token}");
    }
}
```

### API Client Usage in Blazor

```csharp
@inject ApiClient ApiClient

private async Task LoadData()
{
    // Token is automatically added by IAuthenticationProvider
    var todos = await ApiClient.Api.Todos.GetAsync();
}
```

## Security Features

### Token Validation

Microsoft's JWT middleware validates:
- ✅ **Signature** - Ensures token wasn't tampered with
- ✅ **Issuer** - Verifies who created the token
- ✅ **Audience** - Confirms token is for this API
- ✅ **Expiration** - Checks token hasn't expired
- ✅ **Not Before** - Ensures token is valid now

### Logging

The implementation logs important events:
- Authentication failures (with token preview for debugging)
- Successful token validation (with username)
- Authorization challenges (when token is missing/invalid)

## Testing

### Test with Swagger

1. Navigate to `/swagger` on your API
2. Click "Authorize" button
3. Enter: `Bearer {your-jwt-token}`
4. Test authenticated endpoints

### Test from Blazor

1. Login through your Blazor app (uses Auth0/Logto/Mock)
2. Token is stored in `TokenStorageService`
3. API calls automatically include token
4. API validates token using Microsoft's JwtBearer middleware

## Troubleshooting

### 401 Unauthorized

Check logs for:
- "Authentication failed" - Token validation issue
- "Authorization challenge" - Missing or invalid token

Common causes:
- Token expired
- Wrong secret key / issuer / audience
- Token not being sent from Blazor

### Token Not Sent from Blazor

Verify:
1. User is authenticated: `@context.User.Identity.IsAuthenticated`
2. Token is stored: Check `TokenStorageService`
3. `IAuthenticationProvider` is registered in DI

## Key Files

- **`JwtAuthenticationExtensions.cs`** - JWT configuration (uses Microsoft standard)
- **`AppBlueprint.ApiService/Program.cs`** - Adds JWT authentication
- **`AppBlueprint.Infrastructure/Authorization/*`** - Token acquisition (client-side)
- **`appsettings.json`** - Authentication configuration

## What You DON'T Need To Do

❌ **Don't create custom token validation** - Microsoft handles it  
❌ **Don't manually parse JWT tokens** - Middleware does this  
❌ **Don't write signature verification code** - Built-in  
❌ **Don't implement token refresh in API** - Client-side responsibility

## Summary

Your setup now has:
1. **Client-side**: Existing Auth0/Logto/Mock providers get tokens
2. **Transport**: Tokens sent in Authorization header automatically
3. **Server-side**: Microsoft's JWT middleware validates tokens (newly configured)
4. **Controllers**: Use `[Authorize]` attribute to require authentication

Everything uses **standard Microsoft implementations** - no custom validation logic was created.

