param (
    [switch]$Force
)

# Show warning message
Write-Host "WARNING: This script will stop all Docker containers and remove the PostgreSQL data volume." -ForegroundColor Yellow
Write-Host "All database data will be lost and the database will be recreated on next run." -ForegroundColor Yellow
Write-Host ""

if (-not $Force) {
    # Ask for confirmation
    $confirmation = Read-Host "Are you sure you want to proceed? (y/n)"
    if ($confirmation -ne 'y') {
        Write-Host "Operation cancelled." -ForegroundColor Green
        exit
    }
}

Write-Host "Stopping all running Docker containers..." -ForegroundColor Cyan
docker stop $(docker ps -q)

Write-Host "Removing PostgreSQL data volume..." -ForegroundColor Cyan
docker volume rm appblueprint-postgres-data

Write-Host "Done! You can now restart your application with a clean database." -ForegroundColor Green
