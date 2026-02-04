# Execute SQL directly using Npgsql (no psql required)
# This adds SearchVector columns to the database

$ErrorActionPreference = "Stop"

# Get connection string from environment
$ConnectionString = $env:APPBLUEPRINT_RAILWAY_CONNECTIONSTRING
if (-not $ConnectionString) {
    $ConnectionString = $env:DATABASE_CONNECTION_STRING
}

if (-not $ConnectionString) {
    Write-Host "❌ Error: No database connection string found!" -ForegroundColor Red
    Write-Host "Set APPBLUEPRINT_RAILWAY_CONNECTIONSTRING or DATABASE_CONNECTION_STRING environment variable." -ForegroundColor Yellow
    exit 1
}

Write-Host "Found connection string: $($ConnectionString.Substring(0, [Math]::Min(30, $ConnectionString.Length)))..." -ForegroundColor Green

# SQL to add SearchVector columns
$SqlScript = @"
-- Add SearchVector to Tenants table
DO `$`$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'Tenants' AND column_name = 'SearchVector'
    ) THEN
        ALTER TABLE "Tenants" 
        ADD COLUMN "SearchVector" tsvector 
        GENERATED ALWAYS AS (
            to_tsvector('english', 
                coalesce("Name", '') || ' ' || 
                coalesce("Description", '') || ' ' || 
                coalesce("Email", '') || ' ' || 
                coalesce("VatNumber", '')
            )
        ) STORED;
        
        RAISE NOTICE 'Added SearchVector column to Tenants';
    ELSE
        RAISE NOTICE 'SearchVector column already exists in Tenants';
    END IF;
END `$`$;

-- Add SearchVector to Users table
DO `$`$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'Users' AND column_name = 'SearchVector'
    ) THEN
        ALTER TABLE "Users" 
        ADD COLUMN "SearchVector" tsvector 
        GENERATED ALWAYS AS (
            to_tsvector('english', 
                coalesce("FirstName", '') || ' ' || 
                coalesce("LastName", '') || ' ' || 
                coalesce("UserName", '') || ' ' || 
                coalesce("Email", '')
            )
        ) STORED;
        
        RAISE NOTICE 'Added SearchVector column to Users';
    ELSE
        RAISE NOTICE 'SearchVector column already exists in Users';
    END IF;
END `$`$;

-- Create GIN index for Tenants
DO `$`$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE tablename = 'Tenants' AND indexname = 'IX_Tenants_SearchVector'
    ) THEN
        CREATE INDEX "IX_Tenants_SearchVector" ON "Tenants" USING GIN("SearchVector");
        RAISE NOTICE 'Created GIN index on Tenants.SearchVector';
    ELSE
        RAISE NOTICE 'GIN index already exists on Tenants.SearchVector';
    END IF;
END `$`$;

-- Create GIN index for Users
DO `$`$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE tablename = 'Users' AND indexname = 'IX_Users_SearchVector'
    ) THEN
        CREATE INDEX "IX_Users_SearchVector" ON "Users" USING GIN("SearchVector");
        RAISE NOTICE 'Created GIN index on Users.SearchVector';
    ELSE
        RAISE NOTICE 'GIN index already exists on Users.SearchVector';
    END IF;
END `$`$;

-- Mark migrations as applied
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES 
    ('20260202085124_AddFileMetadataEntity', '10.0.1'),
    ('20260203071153_AddFullTextSearchVectors', '10.0.1')
ON CONFLICT DO NOTHING;

-- Verify
SELECT 
    table_name,
    column_name,
    data_type
FROM information_schema.columns
WHERE table_name IN ('Tenants', 'Users')
AND column_name = 'SearchVector';
"@

Write-Host ""
Write-Host "Executing SQL to add SearchVector columns..." -ForegroundColor Cyan

# Save SQL to temp file
$TempSqlFile = Join-Path $env:TEMP "add-searchvector.sql"
Set-Content -Path $TempSqlFile -Value $SqlScript -Encoding UTF8

try {
    # Use EF Core's connection to execute raw SQL
    $CSharpScript = @"
using System;
using Npgsql;
using System.Threading.Tasks;

var connectionString = args[0];
var sql = args[1];

await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();
Console.WriteLine("✅ Connected to database");

await using var cmd = new NpgsqlCommand(sql, conn);
cmd.CommandTimeout = 120;

await using var reader = await cmd.ExecuteReaderAsync();
do
{
    while (await reader.ReadAsync())
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            Console.WriteLine(`$"{reader.GetName(i)}: {reader.GetValue(i)}");
        }
    }
} while (await reader.NextResultAsync());

Console.WriteLine("");
Console.WriteLine("✅ SearchVector columns added successfully!");
"@

    # Save C# script
    $TempCsxFile = Join-Path $env:TEMP "execute-migration.csx"
    Set-Content -Path $TempCsxFile -Value $CSharpScript -Encoding UTF8
    
    # Execute using dotnet-script
    $Command = "dotnet script `"$TempCsxFile`" -- `"$ConnectionString`" `"$SqlScript`""
    Invoke-Expression $Command
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ Migration completed successfully!" -ForegroundColor Green
        Write-Host "Refresh your browser - the search should now work!" -ForegroundColor Green
    } else {
        throw "Command failed with exit code $LASTEXITCODE"
    }
}
catch {
    Write-Host ""
    Write-Host "❌ Error executing migration: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Manual fallback - run this SQL in pgAdmin or DBeaver:" -ForegroundColor Yellow
    Write-Host "File: $TempSqlFile" -ForegroundColor Gray
    Write-Host ""
    Write-Host "SQL content:" -ForegroundColor Gray
    Write-Host $SqlScript -ForegroundColor DarkGray
    exit 1
}
finally {
    # Cleanup
    if (Test-Path $TempSqlFile) { Remove-Item $TempSqlFile -ErrorAction SilentlyContinue }
    if (Test-Path $TempCsxFile) { Remove-Item $TempCsxFile -ErrorAction SilentlyContinue }
}
