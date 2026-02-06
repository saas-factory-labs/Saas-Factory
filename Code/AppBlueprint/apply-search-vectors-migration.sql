-- Apply SearchVector columns manually (from migration 20260203071153_AddFullTextSearchVectors)
-- This bypasses the migration history conflict

DO $$
DECLARE
    -- Migration constants
    AddFullTextSearchVectorsMigrationId CONSTANT TEXT := '20260203071153_AddFullTextSearchVectors';
    AddFileMetadataEntityMigrationId CONSTANT TEXT := '20260202085124_AddFileMetadataEntity';
    HistoryTableName CONSTANT TEXT := '__EFMigrationsHistory';
    ProductVersion CONSTANT TEXT := '10.0.0';
    
    -- Table and column name constants
    UsersTableName CONSTANT TEXT := 'Users';
    TenantsTableName CONSTANT TEXT := 'Tenants';
    SearchVectorColumnName CONSTANT TEXT := 'SearchVector';
BEGIN

-- Add SearchVector to Users table
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = UsersTableName AND column_name = SearchVectorColumnName
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
        
        COMMENT ON COLUMN "Users"."SearchVector" IS 'Full-text search vector for user search';
    END IF;

-- Add SearchVector to Tenants table
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = TenantsTableName AND column_name = SearchVectorColumnName
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
        
        COMMENT ON COLUMN "Tenants"."SearchVector" IS 'Full-text search vector for tenant search';
    END IF;

-- Create GIN index for Users if not exists
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE tablename = UsersTableName AND indexname = 'IX_Users_SearchVector'
    ) THEN
        CREATE INDEX "IX_Users_SearchVector" ON "Users" USING GIN("SearchVector");
    END IF;

-- Create GIN index for Tenants if not exists
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE tablename = TenantsTableName AND indexname = 'IX_Tenants_SearchVector'
    ) THEN
        CREATE INDEX "IX_Tenants_SearchVector" ON "Tenants" USING GIN("SearchVector");
    END IF;

-- Mark the migration as applied in EF Core's migration history
-- First, mark the previous migration if not already
EXECUTE format('INSERT INTO %I ("MigrationId", "ProductVersion") SELECT $1, $2 WHERE NOT EXISTS (SELECT 1 FROM %I WHERE "MigrationId" = $1)', 
    HistoryTableName, HistoryTableName)
USING AddFileMetadataEntityMigrationId, ProductVersion;

-- Then mark the SearchVector migration
EXECUTE format('INSERT INTO %I ("MigrationId", "ProductVersion") SELECT $1, $2 WHERE NOT EXISTS (SELECT 1 FROM %I WHERE "MigrationId" = $1)', 
    HistoryTableName, HistoryTableName)
USING AddFullTextSearchVectorsMigrationId, ProductVersion;

-- Verify the changes
SELECT 
    UsersTableName as table_name,
    column_name,
    data_type,
    is_generated
FROM information_schema.columns
WHERE table_name = UsersTableName AND column_name = SearchVectorColumnName
UNION ALL
SELECT 
    TenantsTableName as table_name,
    column_name,
    data_type,
    is_generated
FROM information_schema.columns
WHERE table_name = TenantsTableName AND column_name = SearchVectorColumnName;

-- Verify indexes
SELECT tablename, indexname, indexdef
FROM pg_indexes
WHERE indexname IN ('IX_Users_SearchVector', 'IX_Tenants_SearchVector')
ORDER BY tablename ASC;
END $$;
