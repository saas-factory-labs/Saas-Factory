# Create EF Core Migration for Row-Level Security Setup
# This script creates a migration that will run the RLS SQL automatically

Write-Host "Creating Row-Level Security EF Core Migration..." -ForegroundColor Cyan

# Get connection string from Aspire dashboard or use local default
$connectionString

Write-Host "Using connection string for migration creation" -ForegroundColor Gray

# Set environment variable for EF tools
$env:DATABASE_CONNECTIONSTRING = $connectionString

# Create migration
Set-Location "AppBlueprint.Infrastructure"

dotnet ef migrations add EnableRowLevelSecurity `
    --context ApplicationDbContext `
    --startup-project ../AppBlueprint.ApiService

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✓ Migration created successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Now we need to add the RLS SQL to the migration file." -ForegroundColor Yellow
    Write-Host "Opening the migration file..." -ForegroundColor Gray
    
    # Find the latest migration file
    $migrationFile = Get-ChildItem -Path "Migrations" -Filter "*EnableRowLevelSecurity.cs" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    
    if ($migrationFile) {
        Write-Host "Migration file: $($migrationFile.FullName)" -ForegroundColor Cyan
        code $migrationFile.FullName
    }
} else {
    Write-Host ""
    Write-Host "❌ Migration creation failed" -ForegroundColor Red
}

# Clean up
$env:DATABASE_CONNECTIONSTRING = $null
Set-Location ..