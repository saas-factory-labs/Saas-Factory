-- Verification script - run this to see what's actually in your database

-- Define table name constants
DO $$
DECLARE
    v_tenants_pascal CONSTANT TEXT := 'Tenants';
    v_users_pascal CONSTANT TEXT := 'Users';
    v_tenants_lower CONSTANT TEXT := 'tenants';
    v_users_lower CONSTANT TEXT := 'users';
    v_table_names TEXT[];
    v_query TEXT;
BEGIN
    -- Create table names array using constants
    v_table_names := ARRAY[v_tenants_pascal, v_users_pascal, v_tenants_lower, v_users_lower];
    
    RAISE NOTICE 'Table names: %, %, %, %', v_tenants_pascal, v_users_pascal, v_tenants_lower, v_users_lower;
    
    -- 1. Check if SearchVector columns exist
    RAISE NOTICE '--- 1. SearchVector columns ---';
    v_query := format($q$
        WITH table_names AS (
            SELECT unnest($1) AS name
        )
        SELECT 
            c.table_name,
            c.column_name,
            c.data_type,
            c.is_nullable,
            c.column_default
        FROM information_schema.columns c
        INNER JOIN table_names t ON c.table_name = t.name
        WHERE c.column_name ILIKE '%%search%%'
        ORDER BY c.table_name ASC, c.column_name ASC
    $q$);
    EXECUTE v_query USING v_table_names;
    
    -- 2. Check all columns in Tenants table
    RAISE NOTICE '--- 2. Columns in % table ---', v_tenants_pascal;
    v_query := format($q$
        SELECT column_name, data_type
        FROM information_schema.columns
        WHERE table_name = %L
        ORDER BY ordinal_position ASC
    $q$, v_tenants_pascal);
    EXECUTE v_query;
    
    -- 3. Check all columns in Users table
    RAISE NOTICE '--- 3. Columns in % table ---', v_users_pascal;
    v_query := format($q$
        SELECT column_name, data_type
        FROM information_schema.columns
        WHERE table_name = %L
        ORDER BY ordinal_position ASC
    $q$, v_users_pascal);
    EXECUTE v_query;
    
    -- 4. Check if tables exist at all (case-sensitive)
    RAISE NOTICE '--- 4. Tables that exist ---';
    v_query := format($q$
        WITH table_names AS (
            SELECT unnest($1) AS name
        )
        SELECT t.table_name 
        FROM information_schema.tables t
        INNER JOIN table_names tn ON t.table_name = tn.name
        WHERE t.table_schema = 'public'
        ORDER BY t.table_name ASC
    $q$);
    EXECUTE v_query USING v_table_names;
    
    -- 5. Check migration history
    RAISE NOTICE '--- 5. Migration history ---';
    EXECUTE 'SELECT "MigrationId", "ProductVersion" FROM "__EFMigrationsHistory" ORDER BY "MigrationId" ASC';
    
    -- 6. Check if GIN indexes exist
    RAISE NOTICE '--- 6. GIN indexes ---';
    v_query := format($q$
        WITH table_names AS (
            SELECT unnest($1) AS name
        )
        SELECT 
            p.tablename,
            p.indexname,
            p.indexdef
        FROM pg_indexes p
        INNER JOIN table_names t ON p.tablename = t.name
        WHERE p.indexname ILIKE '%%search%%'
        ORDER BY p.tablename ASC
    $q$);
    EXECUTE v_query USING v_table_names;
END $$;
