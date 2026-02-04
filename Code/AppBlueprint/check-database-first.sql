-- STEP 1: Check what tables exist (see exact case)
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public'
ORDER BY table_name;

-- STEP 2: If you see "Tenants" table, check its columns
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = 'Tenants'
ORDER BY ordinal_position;

-- STEP 3: If you see "Users" table, check its columns
SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = 'Users'
ORDER BY ordinal_position;
