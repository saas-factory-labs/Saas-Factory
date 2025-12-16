# Sync Documentation to RazorPress and Writerside
# This script copies documentation files from content/ to RazorPress/_pages/ and Writerside/topics/

$contentPath = "C:\Development\Development-Projects\saas-factory-labs\docs\content"
$razorPressPath = "C:\Development\Development-Projects\saas-factory-labs\docs\RazorPress\RazorPress\_pages"
$writersidePath = "C:\Development\Development-Projects\saas-factory-labs\Writerside\topics"

# List of documentation files to sync (relative to content/ directory)
$filesToSync = @(
    "getting-started/Quick-start.md",
    "architecture/Architectural-decision-record.md",
    "guides/Migration.md",
    "guides/Versioning.md",
    "development/Code-Structure.md",
    # Authentication guides
    "guides/authentication/AUTH0_SETUP_GUIDE.md",
    "guides/authentication/AUTHENTICATION_QUICK_SETUP.md",
    "guides/authentication/BLAZOR_JWT_GUIDE.md",
    "guides/authentication/JWT_QUICK_REFERENCE.md",
    "guides/authentication/JWT_TESTING_GUIDE.md",
    "guides/authentication/JWT_VALIDATION_GUIDE.md",
    "guides/authentication/LOGTO_API_RESOURCE_SETUP.md",
    "guides/authentication/LOGTO_CLOUD_SETUP.md",
    "guides/authentication/LOGTO_QUICKSTART.md",
    "guides/authentication/LOGTO_SETUP_GUIDE.md",
    "guides/authentication/LOGTO_SIGNOUT_CONFIGURATION.md",
    "guides/authentication/QUICKSTART_JWT_TESTING.md",
    # Deployment guides
    "guides/deployment/RAILWAY_CHECKLIST.md",
    "guides/deployment/RAILWAY_DEPLOYMENT.md",
    "guides/deployment/RAILWAY_IMPLEMENTATION_SUMMARY.md",
    "guides/deployment/RAILWAY_OVERVIEW.md",
    "guides/deployment/RAILWAY_PORT_80_CONFIGURATION.md",
    "guides/deployment/RAILWAY_QUICKSTART.md",
    "guides/deployment/RAILWAY_README.md",
    "guides/deployment/COMPLETE_RAILWAY_SOLUTION.md",
    "guides/deployment/DOCKER_BUILD_INSTRUCTIONS.md",
    "guides/deployment/DOCKER.md",
    "guides/deployment/CLOUD_DATABASE_SETUP.md",
    "guides/deployment/HTTPS-SETUP.md"
)

Write-Host "Syncing documentation files to RazorPress and Writerside..." -ForegroundColor Cyan

foreach ($file in $filesToSync) {
    $sourcePath = Join-Path $contentPath $file
    $fileName = Split-Path $file -Leaf
    $razorPressDestPath = Join-Path $razorPressPath $fileName
    $writersideDestPath = Join-Path $writersidePath $fileName
    
    if (Test-Path $sourcePath) {
        # Sync to RazorPress
        Copy-Item -Path $sourcePath -Destination $razorPressDestPath -Force
        
        # Sync to Writerside
        Copy-Item -Path $sourcePath -Destination $writersideDestPath -Force
        
        Write-Host "✓ Synced: $file -> RazorPress + Writerside" -ForegroundColor Green
    } else {
        Write-Host "✗ Not found: $file" -ForegroundColor Red
    }
}

Write-Host "`nSync complete!" -ForegroundColor Cyan
Write-Host "RazorPress: Update sidebar.json if you added new files." -ForegroundColor Yellow
Write-Host "Writerside: Update sf.tree if you added new files." -ForegroundColor Yellow
