# Railway Configuration for Repository Root Build

## Current Setup

The Dockerfiles are now configured to build from the **repository root**, with all paths prefixed with `Code/AppBlueprint/`.

## Required Railway Configuration Changes

### **IMPORTANT: Remove Root Directory Setting**

Since the Dockerfiles now include `Code/AppBlueprint/` in all paths, Railway should build from the repository root.

#### For Each Service (api-service, web-service):

1. **Go to Railway Dashboard** → Your Project → Select Service
2. **Settings Tab**
3. **Root Directory**: **DELETE THIS** or set to empty `/` or `.`
4. **Variables Tab** → Update/Add:
   - `RAILWAY_DOCKERFILE_PATH`: `Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile` (for API)
   - `RAILWAY_DOCKERFILE_PATH`: `Code/AppBlueprint/AppBlueprint.Web/Dockerfile` (for Web)
5. **Redeploy**

### Configuration Summary

#### API Service:
```
Root Directory: (empty) or . or /
RAILWAY_DOCKERFILE_PATH: Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile
```

#### Web Service:
```
Root Directory: (empty) or . or /
RAILWAY_DOCKERFILE_PATH: Code/AppBlueprint/AppBlueprint.Web/Dockerfile
```

## Why This Works

### Dockerfile Paths:
```dockerfile
COPY Code/AppBlueprint/Directory.Packages.props ./
COPY ["Code/AppBlueprint/AppBlueprint.ApiService/...", "..."]
```

### Railway Build Context:
```
Root Directory: / (repository root)
Docker context: / 
Dockerfile says: Code/AppBlueprint/...
Railway looks for: /Code/AppBlueprint/... ✅
```

## Local Build Command

From repository root:
```powershell
docker build -f Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile -t api .
docker build -f Code/AppBlueprint/AppBlueprint.Web/Dockerfile -t web .
```

## Verification

After updating Railway configuration and redeploying, check logs for:
```
✅ COPY Code/AppBlueprint/Directory.Packages.props
✅ COPY Code/AppBlueprint/Shared-Modules/...
✅ dotnet restore succeeds
✅ Build completes
```

