# Logto Authentication Fixes - January 2026

## Summary

This document describes two related authentication issues that were fixed in the Logto/Blazor Server integration:

1. **Sign-out not clearing authentication state** - Users remained authenticated after logout
2. **Onboarding page flash on login** - Existing users saw a brief flash of the onboarding page

Both issues are now **completely resolved** ✅

---

## Problem 1: Sign-Out Not Clearing Authentication State

When logging out with a Logto (B2C) account, two issues occurred:
1. Users were initially redirected to a blank page at `http://localhost:9200/signout-callback-logto`
2. After fixing the redirect, authentication state was not cleared - clicking login would auto-redirect to dashboard without requiring credentials

### Root Cause

The sign-out implementation was **manually constructing the Logto end session URL** and handling the post-logout callback with a custom endpoint. This broke the standard OIDC sign-out flow because:

1. We were manually calling `SignOutAsync(LogtoCookieScheme)` and `SignOutAsync(LogtoScheme)` separately before the redirect
2. We manually constructed the end session URL with parameters
3. We created a custom `/signout-callback-logto` endpoint that tried to handle the callback
4. This interfered with the ASP.NET Core OIDC middleware's automatic handling

### Incorrect Implementation (Before)

```csharp
// ❌ WRONG - Manual end session URL construction
app.MapGet("/auth/signout", async (HttpContext context, IConfiguration config) =>
{
    // Manually clear cookies first
    await context.SignOutAsync(LogtoCookieScheme);
    await context.SignOutAsync(LogtoScheme);
    
    // Manually build end session URL
    var endSessionUrl = $"{logtoEndpoint}/session/end?post_logout_redirect_uri={Uri.EscapeDataString(postLogoutRedirectUri)}";
    
    // Manually redirect to Logto
    context.Response.Redirect(endSessionUrl);
});

// ❌ WRONG - Custom callback endpoint interfering with middleware
app.MapGet("/signout-callback-logto", (HttpContext context) =>
{
    return Results.Redirect("/login", permanent: false, preserveMethod: false);
});
```

**Why this failed:**
- The custom callback endpoint prevented the OIDC middleware from processing the callback properly
- The middleware expects to handle the `SignedOutCallbackPath` automatically
- Manual cookie clearing before redirect interfered with the middleware's flow
- The response was empty (chunked transfer encoding with no body) because the custom endpoint wasn't properly integrated with the middleware

## Solution

According to the [official Logto Blazor Server documentation](https://docs.logto.io/quick-starts/dotnet-core/blazor-server), the correct approach is to **let the OIDC middleware handle the sign-out flow automatically**. However, there are **two critical configuration requirements** that must be set:

### 1. Configure Default Sign-Out Scheme

In the authentication configuration, you **MUST** set `DefaultSignOutScheme` to the OIDC scheme:

```csharp
services.AddAuthentication(options =>
{
    options.DefaultScheme = LogtoCookieScheme;
    options.DefaultChallengeScheme = LogtoScheme;
    options.DefaultSignInScheme = LogtoCookieScheme;
    options.DefaultAuthenticateScheme = LogtoCookieScheme;
    options.DefaultSignOutScheme = LogtoScheme; // ⚠️ CRITICAL - Must be set!
})
```

**Why this matters:** Without `DefaultSignOutScheme`, ASP.NET Core doesn't know which authentication handler to use for sign-out, which means the authentication state won't be cleared.

### 2. Sign Out from Both Schemes Explicitly

In the sign-out endpoint, you must sign out from **both** the Cookie scheme and the OIDC scheme:

### Correct Implementation (After)

```csharp
// ✅ CORRECT - Explicit sign-out from both schemes
app.MapGet("/auth/signout", async (HttpContext context) =>
{
    if (context.User?.Identity?.IsAuthenticated ?? false)
    {
        // Sign out from Cookie scheme first (clears local authentication cookie)
        await context.SignOutAsync(LogtoCookieScheme);
        
        // Then sign out from OIDC scheme with RedirectUri (triggers full Logto flow)
        await context.SignOutAsync(LogtoScheme, new AuthenticationProperties { RedirectUri = "/login" });
    }
    else
    {
        context.Response.Redirect("/login");
    }
});

// ✅ NO custom /signout-callback-logto endpoint needed - middleware handles it!
```

**Why both are needed:**
- `SignOutAsync(LogtoCookieScheme)` - Clears the local authentication cookie immediately
- `SignOutAsync(LogtoScheme, ...)` - Triggers the OIDC sign-out flow with Logto, which includes redirecting to Logto's end session endpoint

---

## Problem 2: Onboarding Page Flash on Login

After fixing the sign-out issue, existing users would see the onboarding page briefly flash before being redirected to the dashboard upon login.

### Root Cause

The `OnTokenValidated` OIDC event handler always set `context.Properties.RedirectUri = "/onboarding"` after successful authentication, regardless of whether the user had already completed onboarding. The onboarding page would then check if the user had a tenant and redirect to the dashboard, causing a visible flash.

**Flow that caused the flash:**
1. User signs in → OIDC callback received
2. `OnTokenValidated` fires → sets `RedirectUri = "/onboarding"`
3. User lands on `/onboarding` page (flash visible)
4. Onboarding page checks user state → detects existing tenant
5. Onboarding page redirects to `/dashboard`

### Solution

Check the user's tenant status during the `OnTokenValidated` event and set the appropriate redirect destination immediately:

```csharp
// In OnTokenValidated event handler after fetching user from database:
if (result is not null && !string.IsNullOrEmpty(result.TenantId))
{
    // User has a tenant - redirect directly to dashboard (skip onboarding)
    context.Properties.RedirectUri = "/dashboard";
    Console.WriteLine("[Web] ✅ User has tenant - redirecting to /dashboard");
}
else
{
    // User has no tenant - redirect to onboarding for profile completion
    context.Properties.RedirectUri = "/onboarding";
    Console.WriteLine("[Web] ⚠️ Redirecting to /onboarding for profile completion");
}
```

**Result:** Existing users go directly to `/dashboard`, new users go to `/onboarding` - no page flash! ✅

---

## How It Works

When you call the two `SignOutAsync()` methods in sequence:

1. **`SignOutAsync(LogtoCookieScheme)`** - Immediately clears the local authentication cookie
2. **`SignOutAsync(LogtoScheme, new AuthenticationProperties { RedirectUri = "/login" })`** - Triggers the full OIDC sign-out flow:
   - Middleware constructs the proper OIDC end session URL with required parameters:
     ```
     https://32nkyp.logto.app/oidc/session/end?
       post_logout_redirect_uri=http://localhost:9200/signout-callback-logto
       &id_token_hint={token}
     ```
   - User is redirected to Logto's end session endpoint
   - Logto destroys the centralized sign-in session
   - Logto redirects back to `http://localhost:9200/signout-callback-logto`
   - The OIDC middleware processes this callback automatically (no custom endpoint needed)
   - User is redirected to `/login` (the `RedirectUri` we specified)

**Result:** User is completely signed out from both the local application cookies AND the Logto session.

## Configuration Required

### 1. Authentication Options

In `WebAuthenticationExtensions.cs`, ensure the authentication options include `DefaultSignOutScheme`:

```csharp
services.AddAuthentication(options =>
{
    options.DefaultScheme = LogtoCookieScheme;
    options.DefaultChallengeScheme = LogtoScheme;
    options.DefaultSignInScheme = LogtoCookieScheme;
    options.DefaultAuthenticateScheme = LogtoCookieScheme;
    options.DefaultSignOutScheme = LogtoScheme; // ⚠️ REQUIRED for proper sign-out
})
```

### 2. OIDC Options

Ensure the OIDC options are configured with the signed-out callback path:

```csharp
options.SignedOutCallbackPath = "/signout-callback-logto";
```

### 3. Logto Application Settings

In **Logto Application Settings** (Admin Console), ensure this URI is registered in **"Post sign-out redirect URIs"**:

```
http://localhost:9200/signout-callback-logto
```

For production, add your production URL:
```
https://yourdomain.com/signout-callback-logto
```

## Key Takeaways

1. **Set `DefaultSignOutScheme`** - Without this, ASP.NET Core doesn't know which authentication handler to use for sign-out
2. **Sign out from both schemes explicitly** - Cookie scheme for immediate local cookie clearing, OIDC scheme for Logto session clearing
3. **Order matters** - Sign out from Cookie scheme first, then OIDC scheme
4. **Never manually construct OIDC protocol URLs** - Let the middleware handle it
5. **Don't create custom endpoints for OIDC callbacks** - The middleware handles them automatically
6. **Trust the OIDC middleware** - It implements the OpenID Connect specification correctly
7. **Follow official SDK documentation** - Logto's Blazor Server guide provides the foundation
8. **Test thoroughly** - Verify both local cookies and Logto session are cleared after sign-out

## References

- [Logto End-User Sign-Out Flow](https://docs.logto.io/end-user-flows/sign-out)
- [Logto Blazor Server Quick Start](https://docs.logto.io/quick-starts/dotnet-core/blazor-server)
- [OpenID Connect RP-Initiated Logout](https://openid.net/specs/openid-connect-rpinitiated-1_0.html)

## Related Files

### WebAuthenticationExtensions.cs
Location: `/Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure/Authentication/WebAuthenticationExtensions.cs`

**Sign-Out Fix:**
- Line ~176: `DefaultSignOutScheme = LogtoScheme` configuration
- Line ~733: `/auth/signout` endpoint with explicit scheme sign-out
- Line ~223: `SignedOutCallbackPath` configuration

**Onboarding Flash Fix:**
- Line ~615-648: `OnTokenValidated` event handler with smart redirect logic based on tenant status

## Date Fixed

January 7, 2026

## Status

✅ **CONFIRMED WORKING** - Sign-out now properly clears both local authentication cookies and Logto session. Users must re-authenticate after logging out.

## Known Issue

⚠️ **~~Onboarding Page Flash on Login~~** - **FIXED!** 

The onboarding page was briefly flashing when logging in because the `OnTokenValidated` event handler always redirected to `/onboarding`, which then checked if the user had a tenant and redirected to dashboard.

**Fix Applied:** The authentication callback now checks the user's tenant status immediately during token validation and redirects existing users directly to `/dashboard`, skipping the onboarding page entirely. New users without a tenant are still redirected to `/onboarding`.

```csharp
// In OnTokenValidated event handler:
if (result is not null && !string.IsNullOrEmpty(result.TenantId))
{
    // User has a tenant - redirect directly to dashboard
    context.Properties.RedirectUri = "/dashboard";
}
else
{
    // User has no tenant - redirect to onboarding
    context.Properties.RedirectUri = "/onboarding";
}
```

## Testing

To verify the fix:
1. Start AppHost: `doppler run dotnet watch`
2. Navigate to `http://localhost:9200`
3. Sign in with a Logto account
4. Click "Sign Out"
5. **Expected behavior**: 
   - ✅ Redirected to Logto sign-out page
   - ✅ Automatically redirected back to `/login`
   - ✅ No blank page appears
6. Click "Sign In" again
7. **Expected behavior**:
   - ✅ Logto prompts for credentials (not auto-signed-in)
   - ✅ After authentication, redirected directly to dashboard
   - ✅ No onboarding page flash (users with existing tenant skip onboarding)
