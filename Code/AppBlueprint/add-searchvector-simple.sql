-- Add SearchVector columns to enable full-text search
-- Run this in your PostgreSQL database (Railway web console, pgAdmin, DBeaver, etc.)

-- Drop SearchVector columns if they exist (in case of previous failed attempts)
ALTER TABLE "Tenants" DROP COLUMN IF EXISTS "SearchVector";
ALTER TABLE "Users" DROP COLUMN IF EXISTS "SearchVector";

-- Add SearchVector to Tenants (without IF NOT EXISTS - must be clean add)
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

-- Add SearchVector to Users (without IF NOT EXISTS - must be clean add)
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

-- Create GIN indexes
CREATE INDEX IF NOT EXISTS "IX_Tenants_SearchVector" ON "Tenants" USING GIN("SearchVector");
CREATE INDEX IF NOT EXISTS "IX_Users_SearchVector" ON "Users" USING GIN("SearchVector");

-- Mark migrations as applied
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES 
    ('20260202085124_AddFileMetadataEntity', '10.0.1'),
    ('20260203071153_AddFullTextSearchVectors', '10.0.1')
ON CONFLICT DO NOTHING;

-- Verify (should return 2 rows)
SELECT table_name, column_name 
FROM information_schema.columns
WHERE table_name IN ('Tenants', 'Users')
AND column_name = 'SearchVector';
