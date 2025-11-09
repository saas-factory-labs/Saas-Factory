# 🔍 COMPREHENSIVE SIGN-OUT INVESTIGATION & TEST PLAN

## STEP 1: Navigate to the Diagnostic Page

1. **Go to**: `https://localhost:8083/auth-diagnostic`
2. **Take a screenshot or note down**:
   - Is Authenticated: (value)
   - User Name: (value)
   - All claims listed
   - Current URL

This will tell us the CURRENT authentication state.

## STEP 2: Test Local Sign-Out First

On the diagnostic page, click **"Test /SignOut/Local (Cookie Only)"**

### What Should Happen:
1. ✅ You're redirected to `/login`
2. ✅ You see the login page
3. ✅ You are NOT redirected back to dashboard

### If this FAILS:
- The cookie clearing mechanism isn't working
- There's a deeper issue with authentication

### If this WORKS:
- Cookie clearing works
- The problem is with Logto's OIDC redirect

## STEP 3: Check Server Console Logs

After clicking local sign-out, check server console for:
```
[Web] Local sign-out endpoint called (bypassing Logto end session)
[Web] Cleared Logto.Cookie
```

**Do you see these messages?** (YES/NO)

## STEP 4: Go Back and Test Full Sign-Out

1. **Sign in again** (click Sign In button)
2. **Go to**: `https://localhost:8083/auth-diagnostic`
3. **Click**: "Test /SignOut (Full Logto)"

### Watch for these things:

#### In Browser URL Bar:
- Does the URL change to Logto's domain? (YES/NO)
- Do you see: `https://32nkyp.logto.app/oidc/session/end?...` (YES/NO)
- What's the final URL you end up at?

#### In Server Console:
```
========================================
[Web] SignOut endpoint called - FULL LOGTO SIGN-OUT
[Web] User authenticated: True
[Web] User name: [your email]
[Web] ID Token available: True/False
[Web] Cleared Logto.Cookie
[Web] Redirecting to Logto end session: https://32nkyp.logto.app/oidc/session/end?...
========================================
```

**Do you see ALL these messages?** (YES/NO)

#### In Browser Console:
- WebSocket errors (expected and harmless)
- Any other errors?

## STEP 5: Check Logto Configuration

### Verify Post-Logout Redirect URIs in Logto Console

1. Go to https://32nkyp.logto.app/
2. Navigate to your application
3. Find "Post sign-out redirect URIs"
4. **Take a screenshot** or list EXACTLY what you have there

**CRITICAL**: They MUST be EXACTLY:
- `https://localhost:8083/login?signed-out=true`
- `http://localhost:8082/login?signed-out=true`

NOT:
- ~~`https://localhost:8083/`~~
- ~~`https://localhost:8083/login`~~ (missing query parameter)
- ~~`https://localhost:8083/login?signed-out%3Dtrue`~~ (URL encoded - wrong!)

## STEP 6: Check What Happens After Sign-Out

If the full sign-out redirects you somewhere:

1. **What URL do you end up at?** (copy the FULL URL from address bar)
2. **Go to**: `https://localhost:8083/auth-diagnostic` again
3. **Check**:
   - Is Authenticated: (should be False)
   - Do you see `signed-out=true` in Query Parameters table?

## STEP 7: Check Login Page Behavior

1. After sign-out, you should be on `/login?signed-out=true`
2. **Open browser console** (F12)
3. **Look for this message**:
   ```
   [Login] User just signed out - staying on login page
   [Login] Auth state after sign-out: False
   ```

**Do you see these messages?** (YES/NO)

4. **If you see**:
   ```
   [Login] User already authenticated: [email] - redirecting to dashboard
   ```
   **This means**: The authentication state is NOT being cleared properly

## POSSIBLE FAILURE POINTS & SOLUTIONS

### Failure Point 1: /SignOut endpoint not being called
**Symptom**: No server console logs when clicking sign-out
**Solution**: Check Appbar.razor is calling NavigationManager.NavigateTo("/SignOut", forceLoad: true)

### Failure Point 2: Logto redirect URIs mismatch
**Symptom**: Redirected to Logto but then error page or stuck
**Solution**: Verify EXACT match of post-logout URIs in Logto console (with query parameter)

### Failure Point 3: Cookie not being cleared
**Symptom**: /SignOut/Local doesn't work either
**Solution**: Check if Logto.Cookie exists in browser, verify SignOutAsync is being called

### Failure Point 4: Login page auto-redirects despite signed-out parameter
**Symptom**: End up at /login?signed-out=true but immediately redirected to dashboard
**Solution**: Login.razor logic might not be checking the parameter correctly

### Failure Point 5: Authentication state not updating
**Symptom**: Auth diagnostic shows Is Authenticated: True even after sign-out
**Solution**: Blazor circuit caching issue, need to force reload or use different approach

## DEBUGGING COMMANDS

### Check browser cookies:
1. F12 → Application/Storage → Cookies
2. Look for `.AspNetCore.Logto.Cookie`
3. Is it present before sign-out? (should be YES)
4. Is it present after sign-out? (should be NO)

### Check Network tab during sign-out:
1. F12 → Network tab
2. Click sign-out
3. Watch for requests to:
   - `/SignOut`
   - Logto's domain
   - `/login`
4. Take note of the redirect chain

## COMPLETE THESE STEPS AND REPORT BACK

Please go through EACH step above and tell me:
1. What URL you end up at after sign-out
2. What server console messages you see
3. What the auth-diagnostic page shows before and after sign-out
4. What's configured in Logto console (exact URIs)
5. Whether /SignOut/Local works

This will tell me EXACTLY where the issue is.

