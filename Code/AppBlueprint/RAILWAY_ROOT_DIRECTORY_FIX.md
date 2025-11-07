# 🚨 URGENT: Railway Root Directory Configuration Issue

## The Problem

Railway is building with the WRONG Dockerfile because the **Root Directory** is not set correctly in Railway!

### Evidence:
- ✅ The Dockerfile in GitHub is CORRECT (line 24 has the right COPY command)
- ❌ Railway error shows line 22 with WRONG format
- ❌ Railway is using a DIFFERENT file or OLD cached version

## The Root Cause

Railway needs to know that your project is in `Code/AppBlueprint/`, not in the repository root!

When Railway builds without the Root Directory set, it looks for files in the **repository root**, but your Dockerfile expects files relative to `Code/AppBlueprint/`.

## ✅ SOLUTION - Configure Railway Root Directory

### Step 1: Go to Railway Dashboard

1. Go to https://railway.app/dashboard
2. Select your project
3. Click on the **api-service** (or the service that's failing)

### Step 2: Set Root Directory

1. Click **Settings** tab
2. Find **Root Directory** setting
3. Set it to: `Code/AppBlueprint`
4. Click **Save** or it auto-saves

### Step 3: Set RAILWAY_DOCKERFILE_PATH

In the same Settings page, or in Variables tab:

1. Go to **Variables** tab
2. Add/Update variable:
   - **Name**: `RAILWAY_DOCKERFILE_PATH`
   - **Value**: `AppBlueprint.ApiService/Dockerfile`
3. Save

### Step 4: Redeploy

1. Click **Deployments** tab
2. Click **Deploy** button (or wait for auto-deploy)
3. Railway will rebuild with correct context

## 🎯 Why This Fixes It

### Without Root Directory:
```
Railway Context: /  (repository root)
Dockerfile Path: Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile
COPY Command:    Shared-Modules/AppBlueprint.SharedKernel/...

Railway looks for: /Shared-Modules/... ❌ (doesn't exist in repo root)
```

### With Root Directory Set to `Code/AppBlueprint`:
```
Railway Context: /Code/AppBlueprint  (correct!)
Dockerfile Path: AppBlueprint.ApiService/Dockerfile (relative to root dir)
COPY Command:    Shared-Modules/AppBlueprint.SharedKernel/...

Railway looks for: /Code/AppBlueprint/Shared-Modules/... ✅ (exists!)
```

## 📋 Complete Railway Service Configuration

### For API Service:

**Settings → General:**
- **Root Directory**: `Code/AppBlueprint`

**Settings → Deploy (or Variables):**
- **RAILWAY_DOCKERFILE_PATH**: `AppBlueprint.ApiService/Dockerfile`

**Variables:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:${PORT}
DATABASE_CONNECTION_STRING=${DATABASE_URL}
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
DOTNET_ENVIRONMENT=Production
RAILWAY_DOCKERFILE_PATH=AppBlueprint.ApiService/Dockerfile
```

### For Web Service:

**Settings → General:**
- **Root Directory**: `Code/AppBlueprint`

**Variables:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:${PORT}
ApiService__BaseUrl=${API_SERVICE_URL}
ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
DOTNET_ENVIRONMENT=Production
RAILWAY_DOCKERFILE_PATH=AppBlueprint.Web/Dockerfile
```

## 🔍 How to Verify

After setting Root Directory and redeploying:

1. Check Railway build logs
2. Look for: "context: Code/AppBlueprint" or similar
3. Verify COPY commands succeed
4. Build should complete without "not found" errors

## ⚡ Quick Action Steps

1. **Open Railway Dashboard**: https://railway.app/dashboard
2. **Select API Service**
3. **Settings Tab** → Set **Root Directory** to `Code/AppBlueprint`
4. **Variables Tab** → Set **RAILWAY_DOCKERFILE_PATH** to `AppBlueprint.ApiService/Dockerfile`
5. **Redeploy** (Deployments tab → Deploy button)
6. **Repeat for Web Service** with `AppBlueprint.Web/Dockerfile`

## 📸 Visual Guide

### Where to Find Root Directory Setting:

```
Railway Dashboard
  → Your Project
    → api-service
      → Settings (tab at top)
        → Root Directory (scroll down)
          → Enter: Code/AppBlueprint
          → Auto-saves
```

### Where to Set Variables:

```
Railway Dashboard
  → Your Project
    → api-service
      → Variables (tab at top)
        → Add Variable
          → RAILWAY_DOCKERFILE_PATH = AppBlueprint.ApiService/Dockerfile
```

## 🎯 Expected Result

After configuration:

```
✅ Railway Context: Code/AppBlueprint
✅ Dockerfile: AppBlueprint.ApiService/Dockerfile
✅ COPY Shared-Modules/... → looks in Code/AppBlueprint/Shared-Modules/
✅ Build succeeds!
```

## 💡 Alternative: Use GitHub Actions

If you prefer to let GitHub Actions handle deployment (already configured):

1. Just push your code
2. GitHub Actions will deploy using the Railway CLI
3. The CLI automatically uses the correct context from `railway.json`

The `railway.json` already has:
```json
{
  "build": {
    "dockerContext": "."
  }
}
```

This works when the Railway CLI runs from `Code/AppBlueprint/` directory.

## 🚀 Bottom Line

**Set Root Directory to `Code/AppBlueprint` in Railway Dashboard NOW!**

This is a Railway service configuration issue, not a code issue. Your Dockerfiles are correct!

---

**Time to fix**: 2 minutes  
**Action required**: Configure Railway Root Directory in dashboard

