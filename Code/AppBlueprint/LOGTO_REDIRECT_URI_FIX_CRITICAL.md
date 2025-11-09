# 🚨 CRITICAL: LOGTO REDIRECT URI MISMATCH

## THE PROBLEM

Your app is running on ports **8082/8083**, but Logto is configured with different redirect URIs!

### What's Happening:
```
Error: "redirect_uri did not match any of the client's registered redirect_uris"

App is trying to use: http://localhost:8082/Callback
But Logto has configured: http://localhost:8092/callback (WRONG PORT!)
```

## IMMEDIATE FIX REQUIRED

### Go to Logto Console NOW:
1. **Open**: https://32nkyp.logto.app/
2. **Navigate to**: Applications → Your app (`uovd1gg5ef7i1c4w46mt6`)
3. **Find**: "Redirect URIs" section

### Update Redirect URIs to:
**REMOVE all the old mismatched URIs and ADD these:**

#### For Sign-In (Redirect URIs):
- `http://localhost:8082/callback` ✅
- `https://localhost:8083/callback` ✅
- `http://localhost:8082/Callback` ✅ (capital C - for compatibility)
- `https://localhost:8083/Callback` ✅ (capital C - for compatibility)

#### For Sign-Out (Post sign-out redirect URIs):
- `http://localhost:8082/logout-complete` ✅
- `https://localhost:8083/logout-complete` ✅

### What to Remove:
❌ `http://localhost:8092/callback` (WRONG PORT)
❌ `https://localhost:443/callback` (WRONG PORT)
❌ `http://localhost:80/callback` (WRONG PORT)
❌ `http://localhost/callback` (MISSING PORT)
❌ Any other localhost URIs with wrong ports

## WHY THIS HAPPENED

Your app is configured to run on ports **8082/8083**, but Logto still has URIs configured for old ports (8092, 443, 80, etc.).

When you click "Sign In", the app constructs the redirect URI as:
```
http://localhost:8082/Callback
```

But Logto checks its list and doesn't find a match, so it rejects with:
```
"redirect_uri did not match any of the client's registered redirect_uris"
```

## COMPLETE LOGTO CONFIGURATION

### Redirect URIs (Sign-In):
```
http://localhost:8082/callback
https://localhost:8083/callback
http://localhost:8082/Callback
https://localhost:8083/Callback
https://appblueprint-web-staging.up.railway.app/callback
```

### Post sign-out redirect URIs (Sign-Out):
```
http://localhost:8082/logout-complete
https://localhost:8083/logout-complete
https://appblueprint-web-staging.up.railway.app/logout-complete
```

**Note**: Keep the Railway production URLs, just fix the localhost ones!

## AFTER UPDATING LOGTO

1. ✅ **Save** the changes in Logto
2. ✅ **Try signing in again** - it should work now
3. ✅ **Try signing out** - it should also work now

## VERIFICATION CHECKLIST

- [ ] Logto redirect URIs include `http://localhost:8082/callback` ✅
- [ ] Logto redirect URIs include `https://localhost:8083/callback` ✅
- [ ] Logto post-logout URIs include `http://localhost:8082/logout-complete` ✅
- [ ] Logto post-logout URIs include `https://localhost:8083/logout-complete` ✅
- [ ] Removed all URIs with wrong ports (8092, 443, 80, etc.) ✅
- [ ] App is running on ports 8082/8083 ✅

## EXPECTED BEHAVIOR AFTER FIX

### Sign-In:
1. Click "Sign In with Logto"
2. Redirected to Logto login page
3. Enter credentials
4. ✅ **Redirected back to dashboard** (no error!)

### Sign-Out:
1. Click sign-out button
2. Redirected to Logto
3. Signed out from Logto
4. Redirected to `/logout-complete`
5. Redirected to `/login`
6. ✅ **Successfully logged out!**

## THIS IS THE FINAL PIECE!

Once you update the Logto redirect URIs, both sign-in AND sign-out will work correctly.

The app configuration is correct (ports 8082/8083).
The sign-out code is correct (/logout-complete endpoint).
**Only the Logto URIs need to be updated!**

## UPDATE LOGTO NOW AND TEST!

