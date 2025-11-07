# 🔐 Authentication Quick Reference Guide

## Current Status: ✅ WORKING

Last Updated: 2025-11-07  
Issue Fixed: `/login` route now redirects to Logto

---

## 🎯 Authentication Entry Points

### For Users:

| URL | What It Does |
|-----|--------------|
| `http://localhost:8092/login` | Auto-redirects to Logto login |
| `http://localhost:8092/logto-signin` | Shows manual login button page |
| `http://localhost:8092/signin-logto` | Direct OAuth challenge (triggers Logto) |

### For Developers:

```csharp
// In Blazor components - redirect to login
NavigationManager.NavigateTo("/signin-logto", forceLoad: true);

// Use component for unauthorized access
<RedirectToLogin />

// Protect a page
@page "/mypage"
@attribute [Authorize]

// Check authentication state
<AuthorizeView>
    <Authorized>User is logged in</Authorized>
    <NotAuthorized>User is NOT logged in</NotAuthorized>
</AuthorizeView>
```

---

## 📋 Authentication Routes Map

```
┌─────────────────────────────────────────────────────────────┐
│                    AUTHENTICATION ROUTES                     │
└─────────────────────────────────────────────────────────────┘

/login (UiKit)
    │
    ├─→ Shows loading spinner
    │
    └─→ Redirects to: /signin-logto

/logto-signin (Web)
    │
    ├─→ Shows "Welcome" page with button
    │
    └─→ Button links to: /signin-logto

/signin-logto (Middleware)
    │
    ├─→ Intercepted by: Logto.AspNetCore.Authentication
    │
    ├─→ Generates OAuth URL
    │
    └─→ Redirects to: https://32nkyp.logto.app/oidc/auth?...

https://32nkyp.logto.app/oidc/auth
    │
    ├─→ User enters credentials
    │
    ├─→ Logto validates
    │
    └─→ Redirects to: /callback?code=xxx&state=xxx

/callback (Middleware)
    │
    ├─→ Intercepted by: Logto.AspNetCore.Authentication
    │
    ├─→ Exchanges code for tokens
    │
    ├─→ Creates auth cookie (HttpOnly)
    │
    └─→ Redirects to: original URL or /

✅ USER AUTHENTICATED!

/signout-logto (Middleware)
    │
    ├─→ Clears auth cookie
    │
    ├─→ Redirects to Logto logout
    │
    └─→ Redirects back to: /

✅ USER LOGGED OUT!
```

---

## 🔧 Configuration Checklist

### ✅ Program.cs
```csharp
// 1. Add Logto authentication
builder.Services.AddLogtoAuthentication(options =>
{
    options.Endpoint = "https://32nkyp.logto.app";
    options.AppId = "your-app-id";
    options.AppSecret = "your-app-secret";
});

// 2. Configure cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/signin-logto";
    options.LogoutPath = "/signout-logto";
    options.AccessDeniedPath = "/access-denied";
});

// 3. Add middleware (ORDER MATTERS!)
app.UseRouting();
app.UseAuthentication();  // Before Authorization
app.UseAuthorization();
```

### ✅ Logto Console
```
Application → Redirect URIs:
  ✓ http://localhost:8092/callback
  ✓ http://localhost:8092/signin-logto
  ✓ https://localhost:8092/callback
  ✓ https://localhost:8092/signin-logto

Application → Post Logout Redirect URIs:
  ✓ http://localhost:8092/signout-callback-logto
  ✓ http://localhost:8092
  ✓ https://localhost:8092
```

### ✅ appsettings.json
```json
{
  "Logto": {
    "Endpoint": "https://32nkyp.logto.app",
    "AppId": "your-app-id",
    "AppSecret": "your-app-secret"
  }
}
```

---

## 🧪 Quick Test Commands

### Test 1: Login Flow
```powershell
# Open browser
Start-Process "http://localhost:8092/login"

# Expected:
# 1. Brief loading screen
# 2. Redirect to Logto (32nkyp.logto.app)
# 3. Enter credentials
# 4. Redirect back authenticated
```

### Test 2: Check Authentication State
```powershell
# Navigate to protected page
Start-Process "http://localhost:8092/todos"

# If not authenticated:
# 1. Auto-redirect to /signin-logto
# 2. Then to Logto
# 3. After login, back to /todos
```

### Test 3: Check Console Logs
```powershell
# Should see in console:
# [Login] /login route accessed - redirecting to /signin-logto
# [Web] Logto Authentication configured: https://32nkyp.logto.app
```

---

## 🐛 Troubleshooting

### Problem: /login shows old form
**Solution:** Hot reload may not have picked up changes
```powershell
# Restart AppHost
# Press Ctrl+C in terminal running AppHost
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\AppBlueprint.AppHost
dotnet run
```

### Problem: Redirect loop
**Cause:** Logto redirect URIs not configured
**Solution:** Add all URIs to Logto console (see checklist above)

### Problem: 404 on /callback
**Cause:** Logto middleware not registered
**Solution:** Check Program.cs has `AddLogtoAuthentication()` and middleware

### Problem: User not authenticated after login
**Cause:** Cookie configuration issue
**Solution:** Check `ConfigureApplicationCookie()` and middleware order

### Problem: CORS errors
**Cause:** Logto endpoint mismatch
**Solution:** Ensure `Logto:Endpoint` matches Logto console exactly

---

## 📁 Key Files

### Authentication Files
```
Code/AppBlueprint/
├── AppBlueprint.Web/
│   ├── Program.cs                                    ← Logto configuration
│   ├── Components/
│   │   ├── Pages/
│   │   │   └── Login.razor                          ← Manual login page (/logto-signin)
│   │   └── Shared/
│   │       └── RedirectToLogin.razor                ← Redirect component
│   └── Components/Routes.razor                      ← Uses RedirectToLogin
└── Shared-Modules/
    └── AppBlueprint.UiKit/
        └── Components/
            └── Pages/
                └── Login.razor                       ← Auto-redirect (/login) ✅ FIXED
```

### Configuration Files
```
Code/AppBlueprint/AppBlueprint.Web/
├── appsettings.json              ← Logto settings
├── appsettings.Development.json  ← Dev overrides
└── launchSettings.json           ← Launch profiles
```

### Documentation Files
```
Code/AppBlueprint/AppBlueprint.Web/
├── LOGIN_REDIRECT_FIX_COMPLETE.md          ← Fix details
├── AUTHENTICATION_FLOW_VERIFICATION.md     ← Flow verification
└── AUTHENTICATION_QUICK_REFERENCE.md       ← This file

Code/AppBlueprint/
├── LOGTO_SETUP_GUIDE.md                    ← Initial setup
├── LOGTO_INTEGRATION_COMPLETE.md           ← Integration guide
└── JWT_TESTING_GUIDE.md                    ← Token testing
```

---

## 🚨 Important Notes

### DO ✅
- Use `/signin-logto` for programmatic redirects
- Use `forceLoad: true` when redirecting to auth endpoints
- Check console logs for debugging
- Verify Logto console URIs match exactly

### DON'T ❌
- Don't implement custom password authentication
- Don't mix form-based and OAuth authentication
- Don't use reflection to access Logto providers
- Don't modify the simplified Login.razor (it's meant to stay simple!)

---

## 📞 Support Resources

### Documentation
- Official Logto Docs: https://docs.logto.io/
- ASP.NET Core Auth: https://learn.microsoft.com/aspnet/core/security/authentication/
- OAuth 2.0 / OIDC: https://oauth.net/2/

### Internal Docs
- See `Code/AppBlueprint/AppBlueprint.Web/*.md` files
- Check `LOGTO_*.md` files for setup guides
- Review `JWT_*.md` files for token details

### Console Logs
- Look for `[Login]` prefix for login route logs
- Look for `[Web]` prefix for application logs
- Check browser DevTools Network tab for redirects

---

## ✅ Success Criteria

**The system is working correctly if:**

1. ✅ Navigating to `/login` shows loading spinner then redirects
2. ✅ Redirect goes to Logto (32nkyp.logto.app)
3. ✅ After login, redirected back to app
4. ✅ User is authenticated (see user info in UI)
5. ✅ Protected routes accessible
6. ✅ No console errors
7. ✅ No infinite redirect loops

**Test it now:**
```
http://localhost:8092/login
```

---

**Last verified:** 2025-11-07  
**Status:** ✅ WORKING  
**Next review:** When upgrading Logto package or changing auth flow

🎉 **Authentication system is fully functional!**

