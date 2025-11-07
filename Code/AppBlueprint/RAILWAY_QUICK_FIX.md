# Railway Quick Fix - Root Directory

## ⚡ 30-Second Fix

1. Go to: https://railway.app/dashboard
2. Click: Your Project → **api-service**
3. Click: **Settings** tab
4. Find: **Root Directory**
5. Enter: `Code/AppBlueprint`
6. Click: **Variables** tab
7. Add: `RAILWAY_DOCKERFILE_PATH` = `AppBlueprint.ApiService/Dockerfile`
8. Click: **Deployments** → **Deploy**

**Repeat for web-service** with `AppBlueprint.Web/Dockerfile`

---

## ✅ What This Does

Tells Railway your project is in `Code/AppBlueprint/`, not in the repository root.

Without this: Railway looks in `/Shared-Modules/` ❌  
With this: Railway looks in `/Code/AppBlueprint/Shared-Modules/` ✅

---

## 🎯 Result

After setting Root Directory and redeploying:
- ✅ All COPY commands succeed
- ✅ Build completes
- ✅ Service deploys
- ✅ No more "not found" errors

---

**Time to fix: 2 minutes**  
**Your Dockerfiles are already correct - just need Railway configuration!**

