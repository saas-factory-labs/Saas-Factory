# Docker Build Commands for Local Testing

## ⚠️ Important: Docker Context

The Dockerfiles are designed to be built with the **`Code/AppBlueprint`** directory as the Docker context, NOT the individual service directories.

---

## ✅ Correct Build Commands

### Build API Service
```powershell
# From repository root
cd C:\Development\Development-Projects\saas-factory-labs

# Build with correct context
docker build -f Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile -t appblueprint-api Code/AppBlueprint
```

### Build Web Service
```powershell
# From repository root
cd C:\Development\Development-Projects\saas-factory-labs

# Build with correct context
docker build -f Code/AppBlueprint/AppBlueprint.Web/Dockerfile -t appblueprint-web Code/AppBlueprint
```

---

## ❌ Wrong Commands (What You Tried)

```powershell
# WRONG - Context is too narrow
docker build ".\Code\AppBlueprint\AppBlueprint.Web\"
docker build ".\Code\AppBlueprint\AppBlueprint.ApiService\"
```

**Why it fails**: The context is set to the service directory, but the Dockerfile expects to find files in parent directories like `Directory.Packages.props`, `Shared-Modules/`, etc.

---

## 📁 Docker Context Explanation

### Command Structure
```powershell
docker build -f <path-to-dockerfile> -t <image-name> <context-directory>
               ^^^^^^^^^^^^^^^^^^^^     ^^^^^^^^^^^^^^ ^^^^^^^^^^^^^^^^^^
               Dockerfile location      Image tag      Build context (where COPY looks for files)
```

### What the Dockerfile Expects

When you run:
```powershell
docker build -f Code/AppBlueprint/AppBlueprint.Web/Dockerfile -t web Code/AppBlueprint
```

The context is `Code/AppBlueprint`, so:
- `COPY Directory.Packages.props ./` → looks in `Code/AppBlueprint/Directory.Packages.props` ✅
- `COPY Shared-Modules/...` → looks in `Code/AppBlueprint/Shared-Modules/...` ✅
- `COPY AppBlueprint.Web/...` → looks in `Code/AppBlueprint/AppBlueprint.Web/...` ✅

When you run (WRONG):
```powershell
docker build ".\Code\AppBlueprint\AppBlueprint.Web\"
```

The context is `Code/AppBlueprint/AppBlueprint.Web`, so:
- `COPY Directory.Packages.props ./` → looks in `Code/AppBlueprint/AppBlueprint.Web/Directory.Packages.props` ❌ (doesn't exist)
- `COPY Shared-Modules/...` → looks in `Code/AppBlueprint/AppBlueprint.Web/Shared-Modules/...` ❌ (doesn't exist)

---

## 🎯 Quick Reference

### From Repository Root
```powershell
# API Service
docker build -f Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile -t appblueprint-api Code/AppBlueprint

# Web Service
docker build -f Code/AppBlueprint/AppBlueprint.Web/Dockerfile -t appblueprint-web Code/AppBlueprint
```

### From Code/AppBlueprint Directory
```powershell
cd Code/AppBlueprint

# API Service
docker build -f AppBlueprint.ApiService/Dockerfile -t appblueprint-api .

# Web Service
docker build -f AppBlueprint.Web/Dockerfile -t appblueprint-web .
```

---

## 🚀 Railway Configuration

Railway is already configured correctly:

**In Railway Dashboard → Service Settings:**
- **Root Directory**: `Code/AppBlueprint`
- **RAILWAY_DOCKERFILE_PATH**: `AppBlueprint.ApiService/Dockerfile` (for API)
- **RAILWAY_DOCKERFILE_PATH**: `AppBlueprint.Web/Dockerfile` (for Web)

Railway will automatically use `Code/AppBlueprint` as the context, which matches our Dockerfile structure.

---

## ✅ Test Your Build

Run this from the repository root:

```powershell
cd C:\Development\Development-Projects\saas-factory-labs

# Test API build
docker build -f Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile -t test-api Code/AppBlueprint

# Test Web build
docker build -f Code/AppBlueprint/AppBlueprint.Web/Dockerfile -t test-web Code/AppBlueprint
```

If both complete without errors, you're ready to deploy to Railway! 🎉

---

## 📝 For GitHub Actions

The workflow already uses the correct context:

```yaml
working-directory: Code/AppBlueprint
run: |
  railway up --service api-service --environment staging
```

Railway CLI automatically uses the current directory as context.

---

## 💡 Remember

**Always specify the context as `Code/AppBlueprint`** when building locally!

The last argument in `docker build` is the **context**, not just a path to build.

