# Logto Sign-Out Configuration Guide

## 🚨 ACTION REQUIRED - CONFIGURE LOGTO NOW

**Sign-out WILL NOT WORK until you configure the post-logout redirect URIs in Logto.**

### Immediate Steps:
1. Open https://32nkyp.logto.app/ in your browser
2. Go to Applications → Select your app (`uovd1gg5ef7i1c4w46mt6`)
3. Scroll to **"Post sign-out redirect URIs"**
4. Click "Add URI" and add:
   - `https://localhost:8083/`
   - `http://localhost:8082/`
5. Click "Save changes"
6. Try logout again

---

## Problem
Sign-out was not working because the `/SignOut` endpoint was not properly configured to use Logto's authentication schemes.

## Solution Applied

### Code Changes
Updated `WebAuthenticationExtensions.cs` to use the correct Logto sign-out pattern:

```csharp
app.MapGet("/SignOut", () =>
{
    return Results.SignOut(
        new AuthenticationProperties { RedirectUri = "/" },
        authenticationSchemes: LogtoSchemes // ["Logto.Cookie", "Logto"]
    );
}).AllowAnonymous();
```

This properly:
1. Signs out from the local cookie authentication (`Logto.Cookie`)
2. Redirects to Logto's end session endpoint to sign out from the IdP
3. Redirects back to our app at the specified redirect URI

## Required Logto Configuration

**IMPORTANT**: You must configure the post-logout redirect URI in your Logto application settings:

### In Logto Console:
1. Go to your Logto application settings at https://32nkyp.logto.app/
2. Navigate to your application (App ID: `uovd1gg5ef7i1c4w46mt6`)
3. Find **"Post sign-out redirect URIs"** section  
4. Add your application URLs:
   - Development HTTPS: `https://localhost:8083/login?signed-out=true`
   - Development HTTP: `http://localhost:8082/login?signed-out=true`
   - Production: `https://your-production-url.com/login?signed-out=true`

**CRITICAL**: Without these URIs configured, Logto will reject the sign-out redirect and the logout will fail.

### Common Issues

#### Sign-out hangs or redirects to error page
**Cause**: Post-logout redirect URI not configured in Logto  
**Solution**: Add your application URL to Logto's post sign-out redirect URIs

#### User is redirected but still appears logged in
**Cause**: Authentication cookie not being cleared  
**Solution**: 
- Check browser console for errors
- Clear browser cookies manually
- Verify both `Logto.Cookie` and `Logto` schemes are being signed out

#### Sign-out works locally but not in production
**Cause**: Production URL not added to Logto's post-logout redirect URIs  
**Solution**: Add production URL to Logto console

## Testing Sign-Out

1. **Sign in** to the application
2. **Click the logout button** in the app bar
3. **Expected behavior**:
   - You should be redirected to Logto's logout page briefly
   - Then redirected back to your app at `/`
   - You should no longer be authenticated
   - Accessing protected pages should redirect to login

## Debugging

### Check Console Logs
When sign-out is triggered, you should see:
```
========================================
[Web] SignOut endpoint called - triggering Logto sign out
[Web] Using authentication schemes:
[Web]   - Logto.Cookie
[Web]   - Logto
========================================
```

### Verify Configuration
Test the Logto connection:
- Navigate to `/test-logto-connection`
- Verify DNS resolution and HTTPS connectivity

## Related Files
- `WebAuthenticationExtensions.cs` - Sign-out endpoint implementation
- `Appbar.razor` - UI logout button that calls `/SignOut`
- `Logout.razor` - Logout confirmation page

