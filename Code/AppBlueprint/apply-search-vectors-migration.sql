-- Apply SearchVector columns manually (from migration 20260203071153_AddFullTextSearchVectors)
-- This bypasses the migration history conflict

DO $$
DECLARE
    AddFullTextSearchVectorsMigrationId TEXT := '20260203071153_AddFullTextSearchVectors';
    AddFileMetadataEntityMigrationId TEXT := '20260202085124_AddFileMetadataEntity';
    HistoryTable TEXT := '__EFMigrationsHistory';
    ProductVersion TEXT := '10.0.0';
BEGIN

-- Add SearchVector to Users table
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
        
        COMMENT ON COLUMN "Users"."SearchVector" IS 'Full-text search vector for user search';
    END IF;

-- Add SearchVector to Tenants table
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
        
        COMMENT ON COLUMN "Tenants"."SearchVector" IS 'Full-text search vector for tenant search';
    END IF;

-- Create GIN index for Users if not exists
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE tablename = 'Users' AND indexname = 'IX_Users_SearchVector'
    ) THEN
        CREATE INDEX "IX_Users_SearchVector" ON "Users" USING GIN("SearchVector");
    END IF;

-- Create GIN index for Tenants if not exists
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE tablename = 'Tenants' AND indexname = 'IX_Tenants_SearchVector'
    ) THEN
        CREATE INDEX "IX_Tenants_SearchVector" ON "Tenants" USING GIN("SearchVector");
    END IF;

-- Mark the migration as applied in EF Core's migration history
-- First, mark the previous migration if not already
INSERT INTO HistoryTable ("MigrationId", "ProductVersion")
SELECT AddFileMetadataEntityMigrationId, ProductVersion
WHERE NOT EXISTS (
    SELECT 1 FROM HistoryTable 
    WHERE "MigrationId" = AddFileMetadataEntityMigrationId
);

-- Then mark the SearchVector migration
INSERT INTO HistoryTable ("MigrationId", "ProductVersion")
SELECT AddFullTextSearchVectorsMigrationId, ProductVersion
WHERE NOT EXISTS (
    SELECT 1 FROM HistoryTable 
    WHERE "MigrationId" = AddFullTextSearchVectorsMigrationId
);

-- Verify the changes
SELECT 
    'Users' as table_name,
    column_name,
    data_type,
    is_generated
FROM information_schema.columns
WHERE table_name = 'Users' AND column_name = 'SearchVector'
UNION ALL
SELECT 
    'Tenants' as table_name,
    column_name,
    data_type,
    is_generated
FROM information_schema.columns
WHERE table_name = 'Tenants' AND column_name = 'SearchVector';

-- Verify indexes
SELECT tablename, indexname, indexdef
FROM pg_indexes
WHERE indexname IN ('IX_Users_SearchVector', 'IX_Tenants_SearchVector')
ORDER BY tablename;
END $$;
