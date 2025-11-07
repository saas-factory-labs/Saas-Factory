# Hot Reload Error - Quick Fix

## Error Message
```
System.Runtime.CompilerServices.HotReloadException: Attempted to invoke a deleted lambda or local function implementation.
```

## Root Cause
This error occurs when making changes to dependency injection configuration (particularly HttpClient registration) while the application is running in watch mode. .NET's hot reload feature cannot handle these types of structural changes.

## Solution: Restart the Application

### Option 1: Full Restart (Recommended)
1. **Stop the AppHost:**
   - In Visual Studio: Stop debugging (Shift+F5)
   - In terminal: Press `Ctrl+C` in the terminal running the AppHost

2. **Restart the AppHost:**
   - In Visual Studio: Press F5 or click "Start"
   - In terminal: Run `dotnet run` in the AppHost directory

### Option 2: Web Project Only Restart (Faster)
1. **Stop only the Web project** if running separately
2. **Restart the Web project**

### Option 3: Terminal Commands
If using terminal/command line:
```powershell
# Stop all dotnet processes (if needed)
taskkill /IM dotnet.exe /F

# Navigate to AppHost directory
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\AppBlueprint.AppHost

# Start the application
dotnet run
```

## Why This Happens

Hot reload works well for:
✅ Razor component changes
✅ CSS/JavaScript changes
✅ Method implementation changes
✅ Simple code logic changes

Hot reload **cannot** handle:
❌ Dependency injection configuration changes
❌ Service registration changes
❌ HttpClient factory configuration changes
❌ Middleware pipeline changes
❌ Startup configuration changes

## What We Changed

The changes that triggered this error:
1. Added `AuthenticationDelegatingHandler` service registration
2. Modified `TodoService` HttpClient configuration to include the handler
3. These are structural changes to the DI container that require a full restart

## After Restart

Once restarted, the following will work correctly:
- ✅ TodoService will have the AuthenticationDelegatingHandler configured
- ✅ JWT tokens will be automatically added to API requests
- ✅ 401 Unauthorized errors should be resolved (if logged in)

## Verification Steps After Restart

1. **Navigate to /todos page**
2. **Open browser DevTools → Console**
3. **Look for log messages:**
   - ✅ `"Added authentication token to request: GET ..."`
   - ⚠️ `"No authentication token found in storage..."` (means not logged in)

4. **Open browser DevTools → Network tab**
5. **Trigger a todo operation (try to load todos)**
6. **Check the request headers:**
   - Look for: `Authorization: Bearer eyJ...`

7. **If you see "No authentication token found":**
   - Ensure you're logged in via Logto
   - Check Application → Local Storage → `auth_token`
   - If no token, log in again

## Future Development

To avoid hot reload issues:
- Make all DI configuration changes before starting the app
- Or, plan for a restart after making DI changes
- Hot reload will work fine for TodoPage.razor component changes

## Next Steps

1. **Restart the application** (see Option 1 above)
2. **Log in via Logto** to get a JWT token
3. **Navigate to /todos page**
4. **Try adding a todo** - should work without 401 errors
5. **Check browser DevTools** to verify token is being sent

---

**TL;DR:** Restart the AppHost/application. The error is caused by modifying HttpClient configuration while the app was running - hot reload can't handle these changes.

