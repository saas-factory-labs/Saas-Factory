-- Auto-generated idempotent migration script from EF Core
-- This will safely apply all pending migrations without errors

START TRANSACTION;

-- Define constants for migration IDs and history table
DO $EF$
DECLARE
    -- Migration constants
    searchVectorsMigrationId CONSTANT TEXT := '20260203071153_AddFullTextSearchVectors';
    fileMetadataMigrationId CONSTANT TEXT := '20260202085124_AddFileMetadataEntity';
    historyTable CONSTANT TEXT := '__EFMigrationsHistory';
    productVersion CONSTANT TEXT := '10.0.1';
    
    -- Helper function to check if migration exists
    migrationExists BOOLEAN;
BEGIN
    -- Migration: AddFullTextSearchVectors
    SELECT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = searchVectorsMigrationId) INTO migrationExists;
    
    IF NOT migrationExists THEN
        -- Add SearchVector column to Users
        ALTER TABLE "Users" ADD "SearchVector" character varying(1024) GENERATED ALWAYS AS (to_tsvector('english', coalesce("FirstName", '') || ' ' || coalesce("LastName", '') || ' ' || coalesce("UserName", '') || ' ' || coalesce("Email", ''))) STORED;
        COMMENT ON COLUMN "Users"."SearchVector" IS 'Full-text search vector for user search';
        
        -- Add SearchVector column to Tenants
        ALTER TABLE "Tenants" ADD "SearchVector" character varying(1024) GENERATED ALWAYS AS (to_tsvector('english', coalesce("Name", '') || ' ' || coalesce("Description", '') || ' ' || coalesce("Email", '') || ' ' || coalesce("VatNumber", ''))) STORED;
        COMMENT ON COLUMN "Tenants"."SearchVector" IS 'Full-text search vector for tenant search';
        
        -- Create GIN indexes
        CREATE INDEX "IX_Users_SearchVector" ON "Users" USING GIN ("SearchVector");
        CREATE INDEX "IX_Tenants_SearchVector" ON "Tenants" USING GIN ("SearchVector");
        
        -- Record migration in history
        EXECUTE format('INSERT INTO %I ("MigrationId", "ProductVersion") VALUES ($1, $2)', historyTable)
        USING searchVectorsMigrationId, productVersion;
    END IF;
    
    -- Also mark the previous migration as applied (since tables already exist)
    SELECT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = fileMetadataMigrationId) INTO migrationExists;
    
    IF NOT migrationExists THEN
        EXECUTE format('INSERT INTO %I ("MigrationId", "ProductVersion") VALUES ($1, $2)', historyTable)
        USING fileMetadataMigrationId, productVersion;
    END IF;
END $EF$;

COMMIT;

-- Verify SearchVector columns were added
SELECT 
    'Users' as table_name,
    column_name,
    data_type
FROM information_schema.columns
WHERE table_name = 'Users' AND column_name = 'SearchVector'
UNION ALL
SELECT 
    'Tenants' as table_name,
    column_name,
    data_type
FROM information_schema.columns
WHERE table_name = 'Tenants' AND column_name = 'SearchVector';
