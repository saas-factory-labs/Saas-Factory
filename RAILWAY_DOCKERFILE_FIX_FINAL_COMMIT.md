# Git Commit Message

```
fix: update Dockerfiles to build from repository root for Railway deployment

Update both API and Web Dockerfiles to use Code/AppBlueprint/ prefix in all
COPY commands, enabling builds from repository root instead of requiring
Code/AppBlueprint as the Docker context.

Changes:
- Updated all COPY commands to use Code/AppBlueprint/ prefix
- Added missing AppBlueprint.Contracts project to COPY commands
- Created missing README.md for AppBlueprint.Application
- Updated source COPY to use Code/AppBlueprint/ directory

This allows:
- Building from repo root locally: docker build -f Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile -t api .
- Railway to build with Root Directory cleared (using repo root as context)

Files Modified:
- Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile
- Code/AppBlueprint/AppBlueprint.Web/Dockerfile

Files Created:
- Code/AppBlueprint/Shared-Modules/AppBlueprint.Application/README.md

Railway Configuration Required:
- Clear Root Directory setting (use repo root)
- Set RAILWAY_DOCKERFILE_PATH to: Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile
- Set RAILWAY_DOCKERFILE_PATH to: Code/AppBlueprint/AppBlueprint.Web/Dockerfile

Fixes: Railway deployment errors with "file not found" for Shared-Modules/
```

