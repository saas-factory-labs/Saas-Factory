# Logto Authentication Setup Guide

This guide covers setting up Logto authentication for the AppBlueprint web application on both macOS and Windows.

--

## Prerequisites

- Logto account with a configured application
- Logto Endpoint, App ID, and App Secret from your Logto console

---

## Step 1: Get Logto Credentials

1. **Go to your Logto console:** https://[your-tenant].logto.app/console
2. **Navigate to:** Applications → Your Application
3. **Copy the following:**
   - **Endpoint:** `https://[your-tenant].logto.app/oidc` (note: includes `/oidc` suffix)
   - **App ID:** (e.g., `uovd1gg5ef7i1c4w46mt6`)
   - **App Secret:** Click "Show" and copy the secret

---

## Step 2: Set Environment Variable

The App Secret must be stored as an environment variable (never commit it to git).

### macOS / Linux

1. **Edit your shell profile:**
   ```bash
   nano ~/.zshrc
   ```
   (Or `~/.bashrc` if using bash)

2. **Add this line** (replace with your actual secret):
   ```bash
   export LOGTO_APP_SECRET="your-actual-app-secret-here"
   ```

3. **Save and exit:**
   - Press `Ctrl+X`
   - Press `Y` to confirm
   - Press `Enter`

4. **Reload the environment variable:**
   ```bash
   source ~/.zshrc
   ```

5. **Verify it's set correctly:**
   ```bash
   echo "$LOGTO_APP_SECRET"
   ```
   Should print your secret on **one line** (no line breaks).

### Windows

1. **Open PowerShell as Administrator**

2. **Set User Environment Variable:**
   ```powershell
   [System.Environment]::SetEnvironmentVariable('LOGTO_APP_SECRET', 'your-actual-app-secret-here', 'User')
   ```

3. **Restart your terminal** to load the new variable

4. **Verify it's set:**
   ```powershell
   $env:LOGTO_APP_SECRET
   ```

---

## Step 3: Configure AppHost

The Logto configuration is already set in `AppBlueprint.AppHost/Program.cs`:

```csharp
builder.AddProject<Projects.AppBlueprint_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpEndpoint(port: 5000, name: "web-http")
    .WithReference(apiService)
    .WithEnvironment("Logto__Endpoint", "https://32nkyp.logto.app/oidc")
    .WithEnvironment("Logto__AppId", "uovd1gg5ef7i1c4w46mt6")
    .WithEnvironment("Logto__AppSecret", Environment.GetEnvironmentVariable("LOGTO_APP_SECRET") ?? "");
```

**If you need to update it for your tenant:**
- Change `Logto__Endpoint` to your tenant's OIDC endpoint
- Change `Logto__AppId` to your application's ID
- Leave `Logto__AppSecret` as is (it reads from the environment variable)

---

## Step 4: Configure Logto Redirect URIs

1. **Go to your Logto console:** https://[your-tenant].logto.app/console
2. **Navigate to:** Applications → Your Application
3. **Add Redirect URIs:**
   ```
   http://localhost:5000/Callback
   https://localhost:5000/Callback
   ```

4. **Add Post sign-out redirect URIs:**
   ```
   http://localhost:5000/signout-callback-logto
   https://localhost:5000/signout-callback-logto
   ```

5. **Save changes**

---

## Step 5: Run the Application

### macOS / Linux

```bash
cd /path/to/Saas-Factory/Code/AppBlueprint/AppBlueprint.AppHost
dotnet watch
```

### Windows

```powershell
cd C:\path\to\Saas-Factory\Code\AppBlueprint\AppBlueprint.AppHost
dotnet watch
```

---

## Step 6: Test Authentication

1. **Open browser:** http://localhost:5000/
2. **Expected flow:**
   - Redirects to `/signin`
   - Then to `/auth/signin`
   - Then to Logto login page
   - After login, redirects back to app (authenticated)

3. **Check console output** for:
   ```
   [Web] Configured OpenID Connect 'Logto' scheme with Authority=https://...
   [Web] Configured correlation/nonce cookies: Path=/, IsEssential=true, SameSite=Lax, Secure=None
   ```

---

## Troubleshooting

### "No authentication handler registered for scheme 'Logto'"

**Cause:** Environment variable not loaded or Logto config missing.

**Solution:**
- Verify `LOGTO_APP_SECRET` is set: `echo $env:LOGTO_APP_SECRET` (Windows) or `echo "$LOGTO_APP_SECRET"` (Mac)
- Restart the terminal/AppHost after setting the variable

### "invalid_client - client authentication failed"

**Cause:** App Secret is incorrect or has line breaks.

**Solution:**
- Check the secret has no newlines: `echo "$LOGTO_APP_SECRET" | cat -A` (Mac)
- Re-copy the secret from Logto console (one line, no spaces/newlines)
- Update environment variable and restart AppHost

### "Correlation failed - cookie not found"

**Cause:** Cookies not surviving redirect (fixed in latest code).

**Solution:**
- Make sure you're running the latest code with `SameSite=Lax` and `Secure=None` for development
- Check console shows: `[Web] Configured correlation/nonce cookies: ... SameSite=Lax, Secure=None`

### "redirect_uri did not match"

**Cause:** Callback URL not registered in Logto.

**Solution:**
- Add `http://localhost:5000/Callback` to Redirect URIs in Logto console
- Make sure it matches exactly (case-sensitive)

### Port 5000 permission denied (macOS)

**Cause:** Ports below 1024 require root on macOS (fixed - now using non-privileged port).

**Solution:**
- The app is configured to use port 5000 which doesn't require root
- If you get permission errors, check `launchSettings.json` and `Program.cs` use port 5000

---

## Production Configuration

For production (HTTPS), the cookie configuration automatically switches to:
- `SameSite=None` (required for OAuth)
- `Secure=Always` (requires HTTPS)

Update your Logto redirect URIs to your production domain:
```
https://your-domain.com/Callback
https://your-domain.com/signout-callback-logto
```

And configure the environment variables in your hosting environment (Azure, Railway, etc.).

---

## Key Files

- **AppHost Configuration:** `AppBlueprint.AppHost/Program.cs`
- **Authentication Setup:** `Shared-Modules/AppBlueprint.Infrastructure/Authentication/WebAuthenticationExtensions.cs`
- **Port Configuration:** `AppBlueprint.Web/Properties/launchSettings.json`

---

## Summary

✅ **Set `LOGTO_APP_SECRET` environment variable** (different for Mac/Windows)  
✅ **Configure Logto credentials in AppHost** (Endpoint, AppId)  
✅ **Add redirect URIs in Logto console** (http://localhost:5000/Callback)  
✅ **Run AppHost with `dotnet watch`**  
✅ **Navigate to http://localhost:5000/** and log in via Logto  

**Authentication should now work successfully!** 🎉
