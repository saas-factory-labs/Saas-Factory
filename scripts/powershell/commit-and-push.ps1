# Quick Commit Script for Railway Deployment Fix

Write-Host "=== Committing Railway Deployment Changes ===" -ForegroundColor Cyan
Write-Host ""

# Navigate to repository root
$repoRoot = "C:\Development\Development-Projects\saas-factory-labs"
Set-Location $repoRoot

# Show status
Write-Host "Checking git status..." -ForegroundColor Yellow
git status --short

Write-Host ""
Write-Host "=== Files to be committed ===" -ForegroundColor Cyan
Write-Host ""

# Add all Railway-related files
Write-Host "Adding files..." -ForegroundColor Yellow

# GitHub Actions
git add .github/workflows/deploy-to-railway.yml

# Railway Configuration
git add Code/AppBlueprint/railway.json
git add Code/AppBlueprint/railway-project.json
git add Code/AppBlueprint/docker-compose.railway.yml
git add Code/AppBlueprint/.railwayignore

# Documentation
git add Code/AppBlueprint/RAILWAY_README.md
git add Code/AppBlueprint/RAILWAY_QUICKSTART.md
git add Code/AppBlueprint/RAILWAY_DEPLOYMENT.md
git add Code/AppBlueprint/RAILWAY_CHECKLIST.md
git add Code/AppBlueprint/RAILWAY_OVERVIEW.md
git add Code/AppBlueprint/RAILWAY_IMPLEMENTATION_SUMMARY.md
git add Code/AppBlueprint/RAILWAY_DOCKERFILE_FIX.md
git add Code/AppBlueprint/DOCKER_BUILD_INSTRUCTIONS.md

# Scripts
git add Code/AppBlueprint/setup-railway.ps1
git add Code/AppBlueprint/test-docker-build.ps1

# Modified Dockerfiles (THE FIX!)
git add Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile
git add Code/AppBlueprint/AppBlueprint.Web/Dockerfile

# Commit message templates
git add RAILWAY_COMMIT_MESSAGE.md
git add RAILWAY_DOCKERFILE_FIX_COMMIT.md
git add FINAL_RAILWAY_COMMIT.md

Write-Host "✓ Files staged" -ForegroundColor Green
Write-Host ""

# Commit
Write-Host "Creating commit..." -ForegroundColor Yellow
git commit -m "feat: add Railway deployment with GitHub Actions and fix Docker build context

Implement complete Railway deployment infrastructure with automated GitHub Actions workflow
and comprehensive documentation. Fix Docker build context issues for local testing.

New Features:
- Automated Railway deployment via GitHub Actions
- Staging deployment on push to main
- Manual production deployment with approval
- Database migrations with EF Core
- Health checks and deployment notifications
- Multi-service support (API, Web, PostgreSQL)

Files Modified:
- Code/AppBlueprint/AppBlueprint.ApiService/Dockerfile - Added all project dependencies
- Code/AppBlueprint/AppBlueprint.Web/Dockerfile - Added all project dependencies

Docker Build Context Fix:
- Updated both Dockerfiles to include ALL required project references
- Preserved Shared-Modules/ directory structure in COPY destinations

Required GitHub Secrets:
- RAILWAY_TOKEN_STAGING
- RAILWAY_TOKEN_PRODUCTION
- RAILWAY_DATABASE_URL_STAGING
- RAILWAY_DATABASE_URL_PRODUCTION

Impact:
✅ Enables automated Railway deployment
✅ Local Docker builds work correctly
✅ Supports staging and production environments"

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ Commit created successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Ask to push
    Write-Host "=== Ready to Push ===" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "This will push to origin/main and trigger Railway deployment." -ForegroundColor Yellow
    Write-Host ""
    $push = Read-Host "Push to origin/main now? (y/n)"
    
    if ($push -eq 'y') {
        Write-Host ""
        Write-Host "Pushing to origin/main..." -ForegroundColor Yellow
        git push origin main
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "✅ Successfully pushed to origin/main!" -ForegroundColor Green
            Write-Host ""
            Write-Host "=== Next Steps ===" -ForegroundColor Cyan
            Write-Host "1. Go to GitHub Actions: https://github.com/saas-factory-labs/Saas-Factory/actions" -ForegroundColor White
            Write-Host "2. Watch the 'Deploy to Railway' workflow" -ForegroundColor White
            Write-Host "3. Monitor Railway dashboard: https://railway.app/dashboard" -ForegroundColor White
            Write-Host ""
            Write-Host "Railway will now rebuild with the FIXED Dockerfiles! 🚀" -ForegroundColor Green
        } else {
            Write-Host ""
            Write-Host "❌ Push failed!" -ForegroundColor Red
            Write-Host "Check the error above and try manually: git push origin main" -ForegroundColor Yellow
        }
    } else {
        Write-Host ""
        Write-Host "Commit created but not pushed." -ForegroundColor Yellow
        Write-Host "To push later, run: git push origin main" -ForegroundColor Gray
    }
} else {
    Write-Host ""
    Write-Host "❌ Commit failed!" -ForegroundColor Red
    Write-Host "Check the error above." -ForegroundColor Yellow
}

Write-Host ""

