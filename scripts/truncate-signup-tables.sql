-- ========================================
-- Truncate Signup Tables for Testing
-- ========================================
-- This script clears all data from signup-related tables
-- Use this to reset your test environment between signup tests
--
-- USAGE:
--   docker run --rm -i -e PGPASSWORD="<password>" postgres:17 psql \
--     -h switchyard.proxy.rlwy.net -p 58225 -U postgres -d appblueprintdb \
--     < truncate-signup-tables.sql
-- ========================================

-- Disable triggers temporarily to avoid FK constraint issues
SET session_replication_role = 'replica';

-- Truncate tables in correct order (child tables first due to FKs)
TRUNCATE TABLE "SignupAuditLog" CASCADE;
TRUNCATE TABLE "ProfileEntity" CASCADE;
TRUNCATE TABLE "Users" CASCADE;
TRUNCATE TABLE "Tenants" CASCADE;

-- Re-enable triggers
SET session_replication_role = 'origin';

-- Verify truncation
SELECT 'Tenants' as table_name, COUNT(*) as record_count FROM "Tenants"
UNION ALL
SELECT 'Users', COUNT(*) FROM "Users"
UNION ALL
SELECT 'ProfileEntity', COUNT(*) FROM "ProfileEntity"
UNION ALL
SELECT 'SignupAuditLog', COUNT(*) FROM "SignupAuditLog";
