-- Verification script - run this to see what's actually in your database

-- 1. Check if SearchVector columns exist
SELECT 
    table_name,
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_name IN ('Tenants', 'Users', 'tenants', 'users')
AND column_name ILIKE '%search%'
ORDER BY table_name, column_name;

-- 2. Check all columns in Tenants table
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = 'Tenants'
ORDER BY ordinal_position;

-- 3. Check all columns in Users table
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = 'Users'
ORDER BY ordinal_position;

-- 4. Check if tables exist at all (case-sensitive)
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public'
AND table_name IN ('Tenants', 'Users', 'tenants', 'users')
ORDER BY table_name;

-- 5. Check migration history
DO $EF$
DECLARE
    historyTable TEXT := '__EFMigrationsHistory';
BEGIN
    PERFORM * FROM historyTable ORDER BY "MigrationId";
END $EF$;

-- 6. Check if GIN indexes exist
SELECT 
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE tablename IN ('Tenants', 'Users', 'tenants', 'users')
AND indexname ILIKE '%search%'
ORDER BY tablename;
