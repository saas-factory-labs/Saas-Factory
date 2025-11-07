# Git Commit Message for Dockerfile Fix

## Short Version
```
fix: update Dockerfiles to include all project dependencies for Railway deployment

Fixes Railway build error where Shared-Modules projects were not found.
Updated API and Web Dockerfiles to copy all required project references
with correct directory structure preservation.
```

## Detailed Version
```
fix: update Dockerfiles to include all project dependencies for Railway deployment

Railway deployment was failing with error:
"failed to calculate checksum: Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj: not found"

Root Cause:
- Dockerfiles had incomplete list of project dependencies
- COPY commands used incorrect destination paths that didn't preserve Shared-Modules/ directory structure
- Several required projects were missing from COPY commands

Changes:

AppBlueprint.ApiService/Dockerfile:
- Added missing project dependencies: Application, Domain, Presentation.ApiModule
- Fixed COPY destination paths to preserve Shared-Modules/ structure
- Now copies all 8 required project files before dotnet restore

AppBlueprint.Web/Dockerfile:
- Added missing project dependencies: Api.Client.Sdk, Application, Domain, Infrastructure, SharedKernel, UiKit
- Fixed COPY destination paths to preserve Shared-Modules/ structure
- Now copies all 9 required project files before dotnet restore

Impact:
✅ Railway builds will now complete successfully
✅ All project references will be resolved during dotnet restore
✅ Docker images will build without "not found" errors
✅ Automated Railway deployment via GitHub Actions will work

Testing:
- Local Docker builds tested and verified
- No errors found in Dockerfile syntax

Files Modified:
- Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile
- Code/AppBlueprint/AppBlueprint.Web/Dockerfile

Documentation:
- Added Code/AppBlueprint/RAILWAY_DOCKERFILE_FIX.md

Breaking Changes: None
Related Issue: Railway deployment failure in europe-west4 region
```

