# QUICK REFERENCE - Logto Authentication

## ✅ Implementation Status: COMPLETE

Following official Logto Blazor Server documentation exactly.

---

## Current Configuration

### Endpoint
```
https://32nkyp.logto.app/
```
(Trailing slash required for SDK v0.2.0)

### Application ID
```
uovd1gg5ef7i1c4w46mt6
```

---

## Required Logto Console Settings

Go to: https://32nkyp.logto.app/console → Your Application

### Redirect URIs
```
http://localhost/Callback
https://localhost/Callback
https://localhost:443/Callback
```

### Post sign-out redirect URIs
```
http://localhost/SignedOutCallback
https://localhost/SignedOutCallback
https://localhost:443/SignedOutCallback
```

**NOTE:** SDK uses `/SignedOutCallback` by default, NOT `/logout-complete`

---

## Testing URLs

### Diagnostic Page
```
http://localhost/auth-status
```
Shows current auth state, claims, and test buttons

### Sign In
```
http://localhost/SignIn
```
Triggers Logto authentication

### Sign Out
```
http://localhost/SignOut
```
Signs out and clears session

---

## Quick Tests

### 1. Test Login
```
1. Go to http://localhost
2. Click "Sign In with Logto"
3. Authenticate at Logto
4. Should return to dashboard
5. Appbar should show "Sign Out"
```

### 2. Test Logout
```
1. While logged in, click "Sign Out"
2. Should redirect through Logto
3. Should return to home
4. Appbar should show "Login"
```

### 3. Test Protection
```
1. While logged out, go to /dashboard
2. Should redirect to /login
```

---

## Expected Console Logs

### On Startup
```
[Web] Logto authentication configuration found
[Web] Endpoint: https://32nkyp.logto.app/
[Web] AppId: uovd1gg5ef7i1c4w46mt6
[Web] Logto authentication configured successfully
```

### On Login
```
[Appbar] Authentication state checked: true
```

### On Logout
```
[Appbar] LOGOUT BUTTON CLICKED!
[Appbar] Navigating to /SignOut endpoint (Logto SDK)
[Appbar] Authentication state checked: false
```

---

## Troubleshooting

### Logout Not Working?

1. **Check Logto Console** - Is `/SignedOutCallback` registered?
2. **Clear Browser Cookies** - F12 → Application → Clear Site Data
3. **Check /auth-status** - What does it show?
4. **Check Browser Network** - Follow the redirect chain

### Still Authenticated After Logout?

1. Visit `/auth-status`
2. Check if claims are still present
3. Try hard refresh (Ctrl+F5)
4. Clear all cookies manually

---

## Code Locations

### Authentication Setup
```
Code/AppBlueprint/AppBlueprint.Web/Program.cs
Lines ~201-207
```

### Sign In/Out Endpoints
```
Code/AppBlueprint/AppBlueprint.Web/Program.cs
Lines ~414-425
```

### Appbar Logout
```
Code/AppBlueprint/Shared-Modules/AppBlueprint.UiKit/
  Components/PageLayout/NavigationComponents/
  AppBarComponents/Appbar.razor
```

---

## Summary

✅ **Logto SDK** - v0.2.0  
✅ **Authentication** - Cookie + OIDC  
✅ **Endpoints** - `/SignIn`, `/SignOut`  
✅ **Callbacks** - `/Callback`, `/SignedOutCallback`  
✅ **Implementation** - Official Logto docs  

**Everything is configured correctly according to official documentation.**

If logout still doesn't work:
1. Verify `/SignedOutCallback` in Logto Console
2. Clear browser cookies
3. Use `/auth-status` to diagnose

---

**Next Step:** Test logout and check `/auth-status` page!

