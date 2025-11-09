# LOGOUT CALLBACK PATH FIX

## Problem Found

The logout was failing with this error from Logto:
```
"post_logout_redirect_uri not registered"
```

The logout URL showed:
```
post_logout_redirect_uri=http://localhost/SignedOutCallback
```

But we configured the app to use `/logout-complete`, not `/SignedOutCallback`.

## Root Cause

The Logto SDK uses OpenID Connect under the hood, which has a default `SignedOutCallbackPath` set to `/SignedOutCallback`. 

When we called `SignOutAsync()` with `RedirectUri = "/logout-complete"`, the OIDC middleware was **ignoring** that and using its configured `SignedOutCallbackPath` instead to construct the `post_logout_redirect_uri` sent to Logto.

## The Fix

### 1. Configure OIDC Options After Logto SDK Setup

Added configuration to override the default `SignedOutCallbackPath`:

```csharp
// Use Logto's official SDK for Blazor Server
builder.Services.AddLogtoAuthentication(options =>
{
    options.Endpoint = logtoEndpoint;
    options.AppId = logtoAppId;
    options.AppSecret = logtoAppSecret;
    options.Scopes = new[] { "profile", "email" };
});

// Configure OpenIdConnect options after Logto SDK setup
// This allows us to customize the callback paths
builder.Services.Configure<OpenIdConnectOptions>(LogtoDefaults.AuthenticationScheme, options =>
{
    // Override the default SignedOutCallbackPath to use our custom logout-complete page
    options.SignedOutCallbackPath = "/logout-complete";
    
    Console.WriteLine("[Web] OIDC SignedOutCallbackPath set to: /logout-complete");
});
```

**Why this works:**
- The Logto SDK configures OIDC with default settings
- We then override just the `SignedOutCallbackPath` setting
- Now when OIDC constructs the logout URL, it uses `/logout-complete`

### 2. Removed RedirectUri from SignOutAsync Call

**Before:**
```csharp
await context.SignOutAsync(
    LogtoDefaults.AuthenticationScheme,
    new AuthenticationProperties
    {
        RedirectUri = "/logout-complete"  // ❌ This was being ignored
    });
```

**After:**
```csharp
await context.SignOutAsync(LogtoDefaults.AuthenticationScheme);
```

**Why:**
- The `RedirectUri` in `AuthenticationProperties` is for a different purpose
- The `SignedOutCallbackPath` OIDC option is what controls the post-logout redirect URI sent to Logto
- By removing the redundant `RedirectUri`, we let OIDC use its configured path

## How Logout Works Now

```
User clicks "Sign Out"
  ↓
Navigate to /SignOut (forceLoad: true)
  ↓
Server calls SignOutAsync(LogtoDefaults.AuthenticationScheme)
  ↓
OIDC middleware constructs logout URL:
  - Uses SignedOutCallbackPath: "/logout-complete"
  - Constructs: post_logout_redirect_uri=http://localhost/logout-complete
  ↓
Redirects to: https://32nkyp.logto.app/oidc/session/end?
                post_logout_redirect_uri=http://localhost/logout-complete
                &client_id=...
  ↓
Logto checks if "http://localhost/logout-complete" is in post-logout redirect URIs
  ✅ YES - proceeds with logout
  ↓
Logto clears its session
  ↓
Redirects to: http://localhost/logout-complete
  ↓
LogoutComplete.razor loads
  ↓
Calls Navigation.NavigateTo("/", forceLoad: true)
  ↓
Full page reload - Blazor circuit terminated
  ↓
Fresh circuit starts - authentication state cleared
  ↓
RedirectRoot checks auth → Not authenticated
  ↓
Redirects to /login ✅
```

## Logto Configuration Required

In Logto Console, add this to **Post sign-out redirect URIs**:

```
http://localhost/logout-complete
```

**For HTTPS (if using port 443):**
```
https://localhost/logout-complete
https://localhost:443/logout-complete
```

**For production:**
```
https://your-domain.com/logout-complete
```

## Files Modified

✅ `Code/AppBlueprint/AppBlueprint.Web/Program.cs`
- Added `Configure<OpenIdConnectOptions>` after `AddLogtoAuthentication`
- Set `SignedOutCallbackPath = "/logout-complete"`
- Removed `RedirectUri` from `SignOutAsync` call

## Testing

The application should auto-reload. Now when you logout:

### Expected Behavior:

1. Click "Sign Out"
2. Console should show:
   ```
   [Web] /SignOut endpoint hit - signing out
   [Web] OIDC SignedOutCallbackPath set to: /logout-complete
   ```
3. Browser navigates to Logto with:
   ```
   post_logout_redirect_uri=http://localhost/logout-complete
   ```
4. **No more "post_logout_redirect_uri not registered" error** ✅
5. Logto clears session
6. Redirects to `/logout-complete`
7. Page reloads with `forceLoad: true`
8. Lands on `/login` page as logged out ✅

### Console Logs:
```
[Appbar] LOGOUT BUTTON CLICKED!
[Appbar] Navigating to /SignOut endpoint (Logto SDK)
[Web] /SignOut endpoint hit - signing out
[Web] /SignOut - Signing out user: [email]
[Web] /SignOut - SignOutAsync completed - will redirect to /logout-complete via OIDC SignedOutCallbackPath
[LogoutComplete] OnInitialized - User has been signed out
[LogoutComplete] Forcing full page reload to clear Blazor state
[RedirectRoot] User is NOT authenticated - redirecting to /login
```

## Summary

The issue was that the Logto SDK's underlying OIDC configuration had a default `SignedOutCallbackPath` set to `/SignedOutCallback`, which wasn't registered in Logto. 

By configuring OIDC options after the Logto SDK setup, we override this path to use our custom `/logout-complete` page, which:
1. Is registered in Logto's post-logout redirect URIs
2. Forces a full page reload to clear Blazor circuit state
3. Ensures proper logout behavior

**Status:** ✅ Logout should now work correctly without the "post_logout_redirect_uri not registered" error!

