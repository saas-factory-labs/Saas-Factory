# Apply EF Core migrations using Npgsql
# This script applies the idempotent migration SQL to your PostgreSQL database

# Default connection parameters (adjust these to match your setup)
$DbHost = "localhost"
$DbPort = "5432"
$Database = "appblueprint"  # Change this to your database name
$Username = "postgres"      # Change this to your username
$Password = "postgres"      # Change this to your password

# Path to the SQL file
$SqlFile = "D:\Development\Development-Projects\Saas-Factory\Code\AppBlueprint\apply-all-migrations-idempotent.sql"

Write-Host "Applying EF Core migrations to database..." -ForegroundColor Cyan
Write-Host "Host: $DbHost" -ForegroundColor Gray
Write-Host "Port: $DbPort" -ForegroundColor Gray
Write-Host "Database: $Database" -ForegroundColor Gray
Write-Host "Username: $Username" -ForegroundColor Gray
Write-Host ""

# Build connection string
$ConnectionString = "Host=$DbHost;Port=$DbPort;Database=$Database;Username=$Username;Password=$Password"

# Read SQL file
$SqlContent = Get-Content -Path $SqlFile -Raw

Write-Host "Executing SQL script using Npgsql..." -ForegroundColor Cyan

# Use dotnet script to execute SQL
$TempScriptPath = Join-Path $env:TEMP "apply-migration.csx"

$CSharpScript = @"
#r "nuget: Npgsql, 8.0.5"
using Npgsql;
using System;

var connectionString = Args[0];
var sql = Args[1];

try
{
    using var conn = new NpgsqlConnection(connectionString);
    await conn.OpenAsync();
    Console.WriteLine("Connected to database successfully.");
    
    using var cmd = new NpgsqlCommand(sql, conn);
    cmd.CommandTimeout = 120;
    await cmd.ExecuteNonQueryAsync();
    
    Console.WriteLine("✅ Migration executed successfully!");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    return 1;
}
"@

Set-Content -Path $TempScriptPath -Value $CSharpScript

# Check if dotnet-script is installed
$DotnetScriptInstalled = dotnet tool list -g | Select-String "dotnet-script"

if (-not $DotnetScriptInstalled) {
    Write-Host "dotnet-script not found. Installing..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-script
}

# Run the script
dotnet script $TempScriptPath -- $ConnectionString $SqlContent

$ExitCode = $LASTEXITCODE
Remove-Item $TempScriptPath -ErrorAction SilentlyContinue

if ($ExitCode -eq 0) {
    Write-Host ""
    Write-Host "✅ Migrations applied successfully!" -ForegroundColor Green
    Write-Host "Your database now has SearchVector columns with GIN indexes." -ForegroundColor Green
    Write-Host "Refresh your browser to test the search functionality." -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "❌ Migration failed. Please check the error messages above." -ForegroundColor Red
    Write-Host ""
    Write-Host "Alternative: Run the SQL manually in pgAdmin or another PostgreSQL client:" -ForegroundColor Yellow
    Write-Host "  File: $SqlFile" -ForegroundColor Gray
}

exit $ExitCode
