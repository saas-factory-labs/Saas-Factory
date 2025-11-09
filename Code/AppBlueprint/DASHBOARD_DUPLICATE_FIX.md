# Fix: AmbiguousMatchException - Duplicate /dashboard Routes

## Error
```
AmbiguousMatchException: The request matched multiple endpoints. 
Matches: /dashboard (/dashboard) /dashboard (/dashboard)
```

## Root Cause
This error occurs when Blazor's router detects the same route defined multiple times. In this case, the `/dashboard` route appears to be registered twice, even though the Dashboard.razor file only has one `@page "/dashboard"` directive.

**This is a build cache issue** - the Razor compiler has cached the component and is seeing it twice.

## Solution

### Option 1: Restart dotnet watch (RECOMMENDED)

1. **Stop the current dotnet watch process**: Press `Ctrl+C` in the terminal where AppHost is running
2. **Wait for it to fully stop**
3. **Start it again**: Run `dotnet watch` or restart the AppHost project

This will clear the build cache and properly recompile all components.

### Option 2: Clean and Rebuild

If Option 1 doesn't work, do a full clean:

```powershell
# Stop dotnet watch first (Ctrl+C)

# Navigate to the Web project
cd C:\Development\Development-Projects\saas-factory-labs\Code\AppBlueprint\AppBlueprint.Web

# Clean the project
dotnet clean

# Build the project
dotnet build

# Start watch mode again
cd ..\AppBlueprint.AppHost
dotnet watch
```

### Option 3: Delete bin/obj folders (NUCLEAR OPTION)

If the above doesn't work:

```powershell
# Stop dotnet watch first (Ctrl+C)

# Delete build artifacts
Remove-Item -Recurse -Force .\AppBlueprint.Web\bin
Remove-Item -Recurse -Force .\AppBlueprint.Web\obj

# Rebuild
cd AppBlueprint.Web
dotnet build

# Start watch mode again
cd ..\AppBlueprint.AppHost
dotnet watch
```

## Why This Happened

When you create a new Razor component while `dotnet watch` is running, sometimes the hot-reload mechanism:
1. Detects the new file
2. Compiles it
3. But doesn't clear the previous compilation state properly
4. Results in the route being registered twice

This is a known issue with dotnet watch and Blazor hot reload.

## Verification

After restarting, you should see in the logs:
- No "AmbiguousMatchException" errors
- Navigating to `/dashboard` works correctly
- Only one route registration for `/dashboard`

## Prevention

To avoid this in the future:
- When adding new pages, consider stopping dotnet watch first
- Add the file, save it
- Then start dotnet watch
- OR: Be prepared to restart dotnet watch after adding new pages

## Quick Summary

**JUST RESTART dotnet watch** - Press Ctrl+C to stop it, then start it again. That's usually all you need.

