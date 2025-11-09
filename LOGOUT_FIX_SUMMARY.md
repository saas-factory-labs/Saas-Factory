# Logout Fix Summary

## Problem
The logout button was not working. When clicked, it would navigate to `/signout-logto` but nothing would happen.

## Root Causes

### 1. Authorization Required on Logout Endpoint
The `/signout-logto` endpoint had `.RequireAuthorization()` which created a catch-22:
- Users need to be authenticated to access the logout endpoint
- But if something goes wrong with the authentication state, they can't logout
- This blocked the logout flow

### 2. Missing OIDC Logout Event Handlers
The OpenID Connect configuration was missing event handlers for:
- `OnRedirectToIdentityProviderForSignOut` - Triggered when signing out
- `OnSignedOutCallbackRedirect` - Triggered after Logto completes the logout

### 3. Incorrect Signout Order
The original code signed out from OIDC first, then cookie auth. This is backwards.

## Solutions Applied

### 1. Changed Endpoint Authorization
**Before:**
```csharp
app.MapGet("/signout-logto", async (HttpContext context) =>
{
    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("/");
}).RequireAuthorization(); // ❌ Blocks logout!
```

**After:**
```csharp
app.MapGet("/signout-logto", async (HttpContext context) =>
{
    // Check authentication status first
    if (context.User?.Identity?.IsAuthenticated != true)
    {
        context.Response.Redirect("/");
        return;
    }
    
    // Sign out from cookie first, then OIDC
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, 
        new AuthenticationProperties { RedirectUri = "/" });
        
}).AllowAnonymous(); // ✅ Allows access
```

### 2. Added OIDC Logout Event Handlers
```csharp
OnRedirectToIdentityProviderForSignOut = context =>
{
    Console.WriteLine("[OIDC] Redirecting to Logto logout endpoint");
    Console.WriteLine($"[OIDC] Post logout redirect URI: {context.ProtocolMessage.PostLogoutRedirectUri}");
    return Task.CompletedTask;
},
OnSignedOutCallbackRedirect = context =>
{
    Console.WriteLine("[OIDC] User successfully signed out from Logto");
    // Note: RemoteSignOutContext doesn't have RedirectUri property
    return Task.CompletedTask;
}
```

### 3. Fixed Signout Order
The correct order is:
1. Sign out from **Cookie authentication** (clears local session)
2. Sign out from **OIDC** (redirects to Logto to clear Logto session)
3. Logto redirects back to `/signout-callback-logto`
4. User is redirected to home page

### 4. Added Error Handling
```csharp
try
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, 
        new AuthenticationProperties { RedirectUri = "/" });
}
catch (Exception ex)
{
    Console.WriteLine($"[Web] Error during logout: {ex.Message}");
    // Fallback - just redirect to home
    context.Response.Redirect("/");
}
```

## How Logout Works Now

1. **User clicks "Sign Out" in AppBar menu**
   - Calls `HandleLogoutDirectly()` in `Appbar.razor`
   - Navigates to `/signout-logto` with `forceLoad: true`

2. **Server-side `/signout-logto` endpoint is hit**
   - Checks if user is authenticated
   - Signs out from cookie authentication (local)
   - Signs out from OIDC (triggers redirect to Logto)

3. **OnRedirectToIdentityProviderForSignOut event fires**
   - Logs the redirect to Logto logout endpoint
   - User is redirected to Logto's logout URL

4. **Logto clears its session**
   - Logto clears the user's session on their side
   - Redirects back to app at `/signout-callback-logto`

5. **OnSignedOutCallbackRedirect event fires**
   - Logs successful logout
   - User is redirected to home page (`/`)

6. **User lands on home page as unauthenticated**
   - `RedirectRoot.razor` detects no authentication
   - Redirects to `/login` page

## Testing
To test the logout functionality:
1. Login to the application
2. Click on the Account menu in the top right
3. Click "Sign Out"
4. You should see console logs showing the logout flow
5. You should be redirected through Logto's logout
6. You should land back on the login page

## Configuration Required
The post-logout redirect URI must be configured in Logto:
- Add to "Post sign-out redirect URIs" in Logto application settings:
  - Local: `https://localhost:443/signout-callback-logto`
  - Production: `https://your-domain.com/signout-callback-logto`

