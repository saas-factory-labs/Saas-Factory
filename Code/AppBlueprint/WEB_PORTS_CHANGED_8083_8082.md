# ✅ WEB PROJECT PORTS CHANGED TO 8083/8082

## CHANGES MADE

Changed the web project to use non-privileged ports in development:

### Before (BROKEN):
- HTTP: Port 80 (requires admin)
- HTTPS: Port 443 (requires admin)
- ❌ Application failed to start without admin privileges

### After (WORKING):
- **HTTP**: Port **8082**
- **HTTPS**: Port **8083**
- ✅ No admin privileges required
- ✅ Same ports as specified in launchSettings.json

## CRITICAL: UPDATE LOGTO CONFIGURATION

You MUST update your Logto post-logout redirect URIs to use the new ports:

### Go to Logto Console:
1. https://32nkyp.logto.app/
2. Your application settings
3. **Post sign-out redirect URIs**
4. **UPDATE these URIs**:
   - `https://localhost:8083/logout-complete` ✅
   - `http://localhost:8082/logout-complete` ✅

5. **REMOVE the old ones**:
   - ~~`https://localhost/logout-complete`~~ (old, no port)
   - Any other localhost URIs without ports

## WHAT TO DO NOW

1. **Restart the application** (dotnet watch should auto-reload, but restart to be sure)
2. **Update Logto** as described above
3. **Test sign-out**

### Expected Console Output:
```
[Web] Development mode - HTTP (8082) and HTTPS (8083) enabled
[Web] SignOut endpoint called - FULL LOGTO SIGN-OUT
[Web] Request Host: localhost:8083
[Web] Post-logout redirect URI: https://localhost:8083/logout-complete
[Web] ➡️  Redirecting to Logto end session:
[Web] https://32nkyp.logto.app/oidc/session/end?post_logout_redirect_uri=https%3A%2F%2Flocalhost%3A8083%2Flogout-complete...
```

### What Should Happen:
1. Click sign-out
2. Redirected to Logto (brief)
3. Logto redirects to `https://localhost:8083/logout-complete`
4. You see "Signing out..." message
5. Redirected to `/login`
6. **You are logged out!** ✅

## WHY THIS FIXES THE ISSUE

The previous error was:
```
https://32nkyp.logto.app/oidc/session/end?post_logout_redirect_uri=https%3A%2F%2Flocalhost%2Flogout-complete
"invalid_request"
```

**Problem**: The redirect URI was `https://localhost/logout-complete` (no port) because:
- Web was running on port 80
- Default HTTPS ports (80/443) are not included in the Host header
- Logto rejected it because it didn't match the configured URI

**Solution**: Now running on 8083/8082:
- The Host header includes the port: `localhost:8083`
- The post-logout URI is: `https://localhost:8083/logout-complete`
- This matches what's configured in Logto ✅

## FILES MODIFIED

1. `AppBlueprint.Web/Program.cs` - Changed Kestrel configuration to use 8082/8083 in development
2. `WebAuthenticationExtensions.cs` - Simplified post-logout URI construction

## TEST IT NOW

The app should now be running on:
- **HTTPS**: https://localhost:8083
- **HTTP**: http://localhost:8082

**UPDATE LOGTO CONFIGURATION AND TEST SIGN-OUT!**

## Git Commit Message
```
fix(web): Change development ports from 80/443 to 8082/8083 to fix sign-out

Port 80/443 require admin privileges and caused Host header to omit ports,
breaking Logto's post-logout redirect URI validation.

Changes:
- Development: Use 8082 (HTTP) and 8083 (HTTPS) - no admin required
- Production: Still use port 80 (Railway requirement)
- Host header now includes port, matching Logto configuration

This fixes the "invalid_request" error from Logto when signing out.
The post-logout redirect URI now correctly includes the port number.

BREAKING CHANGE: Update Logto post-logout redirect URIs to:
- https://localhost:8083/logout-complete
- http://localhost:8082/logout-complete
```

