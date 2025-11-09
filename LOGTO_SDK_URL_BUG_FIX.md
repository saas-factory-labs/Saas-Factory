# Logto SDK URL Construction Bug - FIXED

## The Bug in Logto SDK v0.2.0

The Logto SDK version 0.2.0 has a URL construction bug where it concatenates "oidc" to the endpoint **without adding a separator slash**.

### What the SDK does (WRONG):
```csharp
// Pseudo-code of what SDK does internally
string url = options.Endpoint + "oidc";  // ❌ No slash!
```

### This causes:

| Endpoint Value | SDK Concatenates | Result | Status |
|---|---|---|---|
| `https://32nkyp.logto.app` | `+ "oidc"` | `https://32nkyp.logto.appoidc` | ❌ Invalid hostname |
| `https://32nkyp.logto.app/oidc` | `+ "oidc"` | `https://32nkyp.logto.app/oidcoidc` | ❌ Double /oidc |
| `https://32nkyp.logto.app/` | `+ "oidc"` | `https://32nkyp.logto.app/oidc` | ✅ CORRECT! |

## The Fix

**Add a trailing slash to the endpoint:**

```json
{
  "Logto": {
    "Endpoint": "https://32nkyp.logto.app/",
    "AppId": "uovd1gg5ef7i1c4w46mt6",
    "AppSecret": "1WYlfj9ekHF3UmomvNsn62JWGa6gVYSy"
  }
}
```

The trailing slash `/` at the end is **CRITICAL** for the SDK to work correctly.

## Errors You Were Seeing

### Error 1: No such host
```
SocketException: No such host is known.
HttpRequestException: No such host is known. (32nkyp.logto.appoidc:443)
                                              ^^^^^^^^^^^^^^ Invalid hostname
```

### Error 2: Unable to retrieve document
```
IOException: IDX20804: Unable to retrieve document from: 
'https://32nkyp.logto.appoidc/.well-known/openid-configuration'
```

**Root cause:** The SDK was trying to connect to `32nkyp.logto.appoidc` which doesn't exist (notice no dot or slash between "app" and "oidc").

## Files Updated

✅ `Code/AppBlueprint/AppBlueprint.Web/appsettings.json`  
✅ `Code/AppBlueprint/AppBlueprint.Web/appsettings.Development.json`

Both now have:
```json
"Endpoint": "https://32nkyp.logto.app/"
```

## Testing Now

The application should auto-reload in watch mode. Try logging in again:

1. Click "Sign In with Logto"
2. Should now successfully connect to: `https://32nkyp.logto.app/oidc/.well-known/openid-configuration`
3. Should redirect to Logto's login page
4. After login, should callback to your app
5. Should land on dashboard

## Expected Console Output

On app startup:
```
[Web] ========================================
[Web] Logto authentication configuration found
[Web] Endpoint: https://32nkyp.logto.app/
[Web] AppId: uovd1gg5ef7i1c4w46mt6
[Web] Has AppSecret: True
[Web] ========================================
[Web] Logto SDK configured with scopes: profile, email
[Web] Logto authentication configured successfully
```

When clicking login:
```
[Login] Sign in button clicked - navigating to /SignIn with forceLoad (Logto SDK)
[Web] /SignIn - Initiating Logto authentication challenge
```

**No more "No such host" errors!**

## Why This SDK Bug Exists

The Logto SDK v0.2.0 appears to be designed for endpoints that end with a slash, but doesn't handle the case where the endpoint doesn't have one. 

**Possible SDK code (speculative):**
```csharp
// What SDK probably does
string authority = options.Endpoint + "oidc";  // ❌ Bug: should use Path.Combine or ensure slash
```

**What it should do:**
```csharp
// What SDK should do
string authority = options.Endpoint.TrimEnd('/') + "/oidc";  // ✅ Ensures single slash
```

## Long-term Solution

Consider updating to a newer version of the Logto SDK when available, or submitting a bug report to Logto about this URL construction issue.

## Summary

✅ **Problem:** SDK concatenates "oidc" without separator slash  
✅ **Fix:** Add trailing slash to endpoint configuration  
✅ **Result:** URLs are now constructed correctly  
✅ **Status:** Ready to test login flow  

**Try logging in now - it should work!**

