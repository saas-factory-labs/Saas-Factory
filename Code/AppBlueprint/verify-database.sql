-- Verification script - run this to see what's actually in your database

-- Define table name constants
DO $$
DECLARE
    v_tenants_pascal CONSTANT TEXT := 'Tenants';
    v_users_pascal CONSTANT TEXT := 'Users';
    v_tenants_lower CONSTANT TEXT := 'tenants';
    v_users_lower CONSTANT TEXT := 'users';
BEGIN
    -- Note: These constants are for documentation. Queries below use inline values due to PostgreSQL limitations.
    RAISE NOTICE 'Table names: %, %, %, %', v_tenants_pascal, v_users_pascal, v_tenants_lower, v_users_lower;
END $$;

-- 1. Check if SearchVector columns exist
WITH table_names AS (
    SELECT unnest(ARRAY['Tenants', 'Users', 'tenants', 'users']) AS name
)
SELECT 
    c.table_name,
    c.column_name,
    c.data_type,
    c.is_nullable,
    c.column_default
FROM information_schema.columns c
INNER JOIN table_names t ON c.table_name = t.name
WHERE c.column_name ILIKE '%search%'
ORDER BY c.table_name, c.column_name;

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
WITH table_names AS (
    SELECT unnest(ARRAY['Tenants', 'Users', 'tenants', 'users']) AS name
)
SELECT t.table_name 
FROM information_schema.tables t
INNER JOIN table_names tn ON t.table_name = tn.name
WHERE t.table_schema = 'public'
ORDER BY t.table_name ASC;

-- 5. Check migration history
DO $EF$
DECLARE
    historyTable TEXT := '__EFMigrationsHistory';
BEGIN
    PERFORM * FROM historyTable ORDER BY "MigrationId";
END $EF$;

-- 6. Check if GIN indexes exist
WITH table_names AS (
    SELECT unnest(ARRAY['Tenants', 'Users', 'tenants', 'users']) AS name
)
SELECT 
    p.tablename,
    p.indexname,
    p.indexdef
FROM pg_indexes p
INNER JOIN table_names t ON p.tablename = t.name
WHERE p.indexname ILIKE '%search%'
ORDER BY p.tablename;
