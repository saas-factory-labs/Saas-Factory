# ✅ FIXED: AppHost Was Forcing Port 8080

## THE REAL PROBLEM

**The AppHost (Aspire orchestrator) was overriding the port configuration!**

### What Was Happening:
```csharp
// AppHost/Program.cs (OLD - WRONG)
builder.AddProject<Projects.AppBlueprint_Web>("webfrontend")
    .WithHttpEndpoint(port: 8080, name: "web")  // ❌ WRONG PORT
```

The AppHost was forcing the web project to run on **port 8080**, not 8082/8083!

## THE FIX APPLIED

### 1. Updated AppHost Configuration
```csharp
// AppHost/Program.cs (NEW - CORRECT)
builder.AddProject<Projects.AppBlueprint_Web>("webfrontend")
    .WithHttpsEndpoint(port: 8083, name: "web-https")  // ✅ HTTPS on 8083
    .WithHttpEndpoint(port: 8082, name: "web-http")    // ✅ HTTP on 8082
```

### 2. Simplified Web Program.cs
Removed Kestrel port configuration in development since AppHost controls it:
```csharp
// Only configure ports for production
// In development, Aspire AppHost controls the ports
```

## WHAT YOU NEED TO DO

### 1. Restart the Application
**You MUST restart** because the AppHost configuration changed:
```bash
# Stop current dotnet watch (Ctrl+C)
# Then restart
dotnet run --project Code/AppBlueprint/AppBlueprint.AppHost
```

### 2. Update Logto Post-Logout Redirect URIs
Go to https://32nkyp.logto.app/ and update:
- `https://localhost:8083/logout-complete` ✅
- `http://localhost:8082/logout-complete` ✅

### 3. Verify Ports in Console
When the app starts, you should see:
```
[Web] Development mode - Ports controlled by Aspire AppHost
```

Check the Aspire dashboard to verify the web is running on ports 8082/8083.

## WHY THIS WAS THE ISSUE

**Aspire AppHost > Kestrel Configuration > launchSettings.json**

The priority order for port configuration is:
1. **AppHost** (highest priority - was using 8080)
2. Kestrel ConfigureKestrel() in Program.cs
3. launchSettings.json (lowest priority)

Even though we configured Kestrel to use 8082/8083, the AppHost was overriding it with 8080!

## EXPECTED BEHAVIOR NOW

After restart:
1. ✅ Web runs on **8082 (HTTP)** and **8083 (HTTPS)**
2. ✅ Sign-out redirects to `https://localhost:8083/logout-complete`
3. ✅ Logto accepts the redirect (if you updated the URIs)
4. ✅ **Sign-out works!**

## FILES MODIFIED

1. `AppBlueprint.AppHost/Program.cs` - Changed web ports from 8080 to 8082/8083
2. `AppBlueprint.Web/Program.cs` - Removed Kestrel config for development

## TEST IT NOW

1. **Restart the AppHost**
2. **Update Logto configuration** (post-logout redirect URIs)
3. **Test sign-out**

The sign-out should finally work! 🎉

## Git Commit Message
```
fix(apphost): Configure web project to use correct ports 8082/8083

AppHost was overriding port configuration and forcing web to use port 8080.
This caused Host header to not match Logto's expected redirect URIs.

Changes:
- AppHost: Use WithHttpsEndpoint(8083) and WithHttpEndpoint(8082)
- Web Program.cs: Let AppHost control ports in development
- Production still uses port 80 (Railway requirement)

This fixes the sign-out redirect URI mismatch with Logto.

BREAKING CHANGE: Update Logto post-logout redirect URIs to:
- https://localhost:8083/logout-complete
- http://localhost:8082/logout-complete
```

