# Sign-Out Troubleshooting Guide

## ⚠️ CURRENT ISSUE
**Symptom**: Clicking sign-out just refreshes the dashboard instead of logging out.

**What this means**: The sign-out endpoint is being called, but either:
1. The authentication cookie isn't being cleared
2. The sign-out completes but redirects you back to dashboard (because you're still authenticated)
3. The Logto OIDC sign-out isn't triggering the redirect to Logto's end session endpoint

## URGENT: Check Console Logs NOW

When you click sign out, check your **server console** (where the app is running) for these messages:

### What You Should See:
```
========================================
[Web] SignOut endpoint called
[Web] User authenticated: True
[Web] User name: [your email]
[Web] Sign-out initiated - should redirect to Logto end session
========================================
```

### Critical Questions:
1. **Do you see these messages at all?** 
   - YES → The endpoint is being called, continue below
   - NO → The sign-out button isn't calling the endpoint

2. **Do you see this WARNING?**
   ```
   [Web] WARNING: Response hasn't started - manually redirecting to /login
   ```
   - YES → Logto's sign-out isn't working, the OIDC scheme didn't trigger a redirect
   - NO → The Logto OIDC sign-out tried to redirect

3. **After sign-out, do you see you're redirected to a Logto page?**
   - YES → Logto is working, but the post-logout redirect might be wrong
   - NO → Logto OIDC sign-out isn't configured properly

## Immediate Fix Options

### Option 1: Use Local Sign-Out (Quick Fix)
This bypasses Logto's end session endpoint and just clears local cookies:

**Navigate to**: `https://localhost:8083/SignOut/Local`

This should immediately log you out locally. If this works, the problem is with Logto's OIDC sign-out configuration.

### Option 2: Temporarily Change the Sign-Out Button
Edit the Appbar to use local sign-out temporarily:

In `Appbar.razor`, find the logout handler and change:
```csharp
NavigationManager.NavigateTo("/SignOut", forceLoad: true);
```
to:
```csharp
NavigationManager.NavigateTo("/SignOut/Local", forceLoad: true);
```

This gives you working logout while we debug the Logto issue.

## Current Status
The sign-out functionality has been completely rewritten with extensive debugging and fallback mechanisms.

## Two Sign-Out Methods Available

### 1. Standard Sign-Out (Recommended)
**URL**: `/SignOut`  
**Description**: Signs out from both local cookies and Logto's IdP (full sign-out)  
**Requires**: Post-logout redirect URIs configured in Logto

### 2. Local Sign-Out (Debugging/Fallback)
**URL**: `/SignOut/Local`  
**Description**: Only clears local cookies (doesn't sign out from Logto IdP)  
**Use Case**: Testing when Logto configuration is incomplete

## Testing Steps

### Step 1: Test Local Sign-Out First
This will help us confirm the basic mechanism works:

1. **Navigate to**: `https://localhost:8083/SignOut/Local`
2. **Expected behavior**:
   - Console should show: `[Web] Local sign-out endpoint called (bypassing Logto end session)`
   - Console should show: `[Web] Cleared Logto.Cookie`
   - You should be redirected to `/login`
   - You should no longer be authenticated in your app

3. **If this works**, the local cookie clearing works fine and the issue is with Logto's end session redirect

### Step 2: Configure Logto Post-Logout URIs
**This is REQUIRED for `/SignOut` to work**

1. Open https://32nkyp.logto.app/
2. Log in to Logto Console
3. Navigate to **Applications**
4. Select your application: `uovd1gg5ef7i1c4w46mt6`
5. Scroll to **"Post sign-out redirect URIs"**
6. Add these URIs:
   - `https://localhost:8083/`
   - `http://localhost:8082/`
7. Click **Save changes**

### Step 3: Test Full Sign-Out
After configuring Logto:

1. **Navigate to**: `https://localhost:8083/SignOut`
2. **Expected behavior**:
   - Console shows: `[Web] SignOut endpoint called`
   - Console shows: `[Web] Attempting sign-out using Results.SignOut...`
   - You are redirected to Logto's logout page briefly
   - Then redirected back to `/login`
   - You are fully logged out from both app and Logto

## Debugging Console Output

### Successful Sign-Out
```
========================================
[Web] SignOut endpoint called
[Web] User authenticated: True
[Web] User name: [your email]
[Web] Using authentication schemes:
[Web]   - Logto.Cookie
[Web]   - Logto
========================================
[Web] Attempting sign-out using Results.SignOut...
[Web] Results.SignOut created successfully
[Web] Sign-out completed
```

### Failed Sign-Out (Missing Logto Config)
```
========================================
[Web] SignOut endpoint called
[Web] User authenticated: True
[Web] User name: [your email]
[Web] Using authentication schemes:
[Web]   - Logto.Cookie
[Web]   - Logto
========================================
[Web] Attempting sign-out using Results.SignOut...
[Web] ERROR during Results.SignOut: [error message]
[Web] Exception type: [exception type]
[Web] Attempting manual sign-out fallback...
[Web] Manual sign-out completed
```

## Common Issues

### Issue: Sign-out hangs or shows error page
**Cause**: Post-logout redirect URI not configured in Logto  
**Solution**: Follow Step 2 above to configure Logto

### Issue: Redirects to Logto error page
**Cause**: The redirect URI in your app doesn't match what's configured in Logto  
**Solution**: 
- Check the exact URL (including trailing slash)
- Verify HTTPS vs HTTP
- Check for typos

### Issue: Local sign-out works but full sign-out fails
**Cause**: Logto configuration issue  
**Solution**: This confirms the app code is working. Configure Logto as per Step 2

### Issue: Neither sign-out method works
**Cause**: Possible cookie configuration or authentication state issue  
**Solution**:
1. Clear all browser cookies manually
2. Check browser console for JavaScript errors
3. Verify you're actually authenticated before trying to sign out

## Alternative: Update Appbar to Use Local Sign-Out
If you can't configure Logto right away, you can temporarily use the local sign-out:

Edit `Appbar.razor`, change:
```csharp
NavigationManager.NavigateTo("/SignOut", forceLoad: true);
```
to:
```csharp
NavigationManager.NavigateTo("/SignOut/Local", forceLoad: true);
```

**NOTE**: This only clears local cookies. Users will still have active sessions in Logto and may be auto-logged back in when they visit the app again.

## Next Steps

1. ✅ Try `/SignOut/Local` to verify basic logout works
2. ✅ Configure post-logout redirect URIs in Logto console  
3. ✅ Try `/SignOut` for full sign-out
4. ✅ Check console logs for any errors
5. ✅ If still failing, share the console output for further debugging

## Files Modified
- `WebAuthenticationExtensions.cs` - Added debugging, fallback logic, and `/SignOut/Local` endpoint
- `LOGTO_SIGNOUT_CONFIGURATION.md` - Updated with correct URLs and configuration steps

