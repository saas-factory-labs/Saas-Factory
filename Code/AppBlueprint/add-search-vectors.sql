-- Add SearchVector columns to TenantEntity and UserEntity
-- Run this directly in PostgreSQL to fix the immediate error

-- Add SearchVector to Tenants table
ALTER TABLE "Tenants" 
ADD COLUMN IF NOT EXISTS "SearchVector" tsvector 
GENERATED ALWAYS AS (
    to_tsvector('english', 
        coalesce("Name", '') || ' ' || 
        coalesce("ContactEmail", '')
    )
) STORED;

-- Create GIN index for Tenants
CREATE INDEX IF NOT EXISTS "IX_Tenants_SearchVector" 
ON "Tenants" 
USING GIN("SearchVector");

-- Add SearchVector to Users table
ALTER TABLE "Users" 
ADD COLUMN IF NOT EXISTS "SearchVector" tsvector 
GENERATED ALWAYS AS (
    to_tsvector('english', 
        coalesce("Email", '') || ' ' || 
        coalesce("FirstName", '') || ' ' || 
        coalesce("LastName", '')
    )
) STORED;

-- Create GIN index for Users
CREATE INDEX IF NOT EXISTS "IX_Users_SearchVector" 
ON "Users" 
USING GIN("SearchVector");

-- Verify columns were added
SELECT 
    table_name,
    column_name,
    data_type,
    is_generated
FROM information_schema.columns
WHERE table_name IN ('Tenants', 'Users')
AND column_name = 'SearchVector';
