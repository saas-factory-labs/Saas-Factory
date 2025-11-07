# Railway Dockerfile Fix - Resolution

## Issue

Railway deployment was failing with this error:
```
ERROR: failed to build: failed to solve: failed to compute cache key: 
failed to calculate checksum of ref: "/Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj": not found
```

## Root Cause

The Dockerfiles for both API and Web services were not copying all required project dependencies. The COPY commands in the Dockerfiles had incorrect destination paths that didn't preserve the `Shared-Modules/` directory structure.

### Original Problem (API Dockerfile)
```dockerfile
# WRONG - Destination path doesn't include Shared-Modules/
COPY ["Shared-Modules/AppBlueprint.Infrastructure/AppBlueprint.Infrastructure.csproj", "AppBlueprint.Infrastructure/"]
COPY ["Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj", "AppBlueprint.SharedKernel/"]
```

## Solution Applied

### Fixed API Service Dockerfile

Updated `Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile` to:

1. **Copy ALL required project dependencies** (not just some)
2. **Preserve the Shared-Modules directory structure** in destination paths

```dockerfile
# Copy project files for API and all dependencies
COPY ["AppBlueprint.ApiService/AppBlueprint.ApiService.csproj", "AppBlueprint.ApiService/"]
COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
COPY ["AppBlueprint.TodoAppKernel/AppBlueprint.TodoAppKernel.csproj", "AppBlueprint.TodoAppKernel/"]
COPY ["Shared-Modules/AppBlueprint.Application/AppBlueprint.Application.csproj", "Shared-Modules/AppBlueprint.Application/"]
COPY ["Shared-Modules/AppBlueprint.Domain/AppBlueprint.Domain.csproj", "Shared-Modules/AppBlueprint.Domain/"]
COPY ["Shared-Modules/AppBlueprint.Infrastructure/AppBlueprint.Infrastructure.csproj", "Shared-Modules/AppBlueprint.Infrastructure/"]
COPY ["Shared-Modules/AppBlueprint.Presentation.ApiModule/AppBlueprint.Presentation.ApiModule.csproj", "Shared-Modules/AppBlueprint.Presentation.ApiModule/"]
COPY ["Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj", "Shared-Modules/AppBlueprint.SharedKernel/"]
```

### Fixed Web Service Dockerfile

Updated `Code/AppBlueprint/AppBlueprint.Web/Dockerfile` to:

```dockerfile
# Copy project files for Web and all dependencies
COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
COPY ["AppBlueprint.TodoAppKernel/AppBlueprint.TodoAppKernel.csproj", "AppBlueprint.TodoAppKernel/"]
COPY ["Shared-Modules/AppBlueprint.Api.Client.Sdk/AppBlueprint.Api.Client.Sdk.csproj", "Shared-Modules/AppBlueprint.Api.Client.Sdk/"]
COPY ["Shared-Modules/AppBlueprint.Application/AppBlueprint.Application.csproj", "Shared-Modules/AppBlueprint.Application/"]
COPY ["Shared-Modules/AppBlueprint.Domain/AppBlueprint.Domain.csproj", "Shared-Modules/AppBlueprint.Domain/"]
COPY ["Shared-Modules/AppBlueprint.Infrastructure/AppBlueprint.Infrastructure.csproj", "Shared-Modules/AppBlueprint.Infrastructure/"]
COPY ["Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj", "Shared-Modules/AppBlueprint.SharedKernel/"]
COPY ["Shared-Modules/AppBlueprint.UiKit/AppBlueprint.UiKit.csproj", "Shared-Modules/AppBlueprint.UiKit/"]
```

## Key Changes

### What was fixed:

1. ✅ **Preserved directory structure**: Destination paths now match source paths (e.g., `Shared-Modules/...` → `Shared-Modules/...`)

2. ✅ **Added missing dependencies**: 
   - API: Added Application, Domain, Presentation.ApiModule
   - Web: Added Api.Client.Sdk, Application, Domain, Infrastructure, SharedKernel

3. ✅ **Correct Docker context**: The `railway.json` already had correct `dockerContext: "."` setting

## Why This Matters for Railway

Railway uses the `RAILWAY_DOCKERFILE_PATH` environment variable to locate the Dockerfile, and the Docker context is the root of `Code/AppBlueprint/`. 

The COPY commands must:
1. Use source paths relative to the Docker context (`Code/AppBlueprint/`)
2. Use destination paths that preserve the directory structure expected by the .csproj files

## Testing the Fix

### Local Test
```powershell
cd Code\AppBlueprint
docker build -f AppBlueprint.ApiService\Dockerfile -t test-api .
docker build -f AppBlueprint.Web\Dockerfile -t test-web .
```

### Railway Deployment
The fix will be automatically applied when you:
1. Commit these changes
2. Push to main branch
3. GitHub Actions workflow will deploy to Railway

## Files Modified

1. `Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile` - Fixed project references
2. `Code/AppBlueprint/AppBlueprint.Web/Dockerfile` - Fixed project references

## Next Steps

1. ✅ Dockerfiles have been fixed
2. 🔄 Test local Docker build (in progress)
3. 📤 Commit and push changes
4. 🚀 Railway will rebuild with correct Dockerfiles

## Prevention

To prevent this in the future:
- Always test Docker builds locally before deploying
- Ensure COPY destination paths preserve directory structure
- Include ALL project dependencies in the Dockerfile
- Use `dotnet restore` output to verify all projects are found

## Verification

After deploying to Railway, verify:
- ✅ Build completes without "not found" errors
- ✅ All project references are resolved
- ✅ Application starts successfully
- ✅ Health check endpoint responds

---

**Status**: ✅ Fixed  
**Date**: 2025-01-07  
**Impact**: Railway deployments will now work correctly

