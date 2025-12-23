-- ========================================
-- PostgreSQL Row-Level Security Setup
-- for Multi-Tenant AppBlueprint Applications
-- ========================================
-- 
-- This script enables Row-Level Security (RLS) on all tenant-scoped tables
-- to provide database-level tenant isolation in a shared database architecture.
--
-- IMPORTANT: Run this AFTER applying all Entity Framework migrations
--
-- Usage:
--   psql -h localhost -U postgres -d your_database -f SetupRowLevelSecurity.sql
--
-- Or via Entity Framework migration:
--   Add this to a new migration's Up() method
-- ========================================

-- Function to set current tenant context
-- This function stores the tenant ID in a PostgreSQL session variable
-- that can be accessed by RLS policies
CREATE OR REPLACE FUNCTION set_current_tenant(tenant_id TEXT)
RETURNS VOID AS $$
BEGIN
    PERFORM set_config('app.current_tenant_id', tenant_id, FALSE);
END;
$$ LANGUAGE plpgsql;

-- Function to get current tenant context
CREATE OR REPLACE FUNCTION get_current_tenant()
RETURNS TEXT AS $$
BEGIN
    RETURN current_setting('app.current_tenant_id', TRUE);
END;
$$ LANGUAGE plpgsql;

-- ========================================
-- Enable Row-Level Security on All Tenant-Scoped Tables
-- ========================================

-- Core tenant-scoped tables
ALTER TABLE "Users" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Teams" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Organizations" ENABLE ROW LEVEL SECURITY;

-- Contact information tables
ALTER TABLE "ContactPersons" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "EmailAddresses" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "PhoneNumbers" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Addresses" ENABLE ROW LEVEL SECURITY;

-- Audit and logging tables (if tenant-scoped)
ALTER TABLE "AuditLogs" ENABLE ROW LEVEL SECURITY IF EXISTS;
ALTER TABLE "ApiLogs" ENABLE ROW LEVEL SECURITY IF EXISTS;

-- Application-specific tables (Todo app example)
ALTER TABLE "Todos" ENABLE ROW LEVEL SECURITY IF EXISTS;

-- ========================================
-- Create RLS Policies for Tenant Isolation
-- ========================================
-- These policies ensure users can only see data for their tenant
-- The policy checks that TenantId matches the current session's tenant context

-- Users table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Users";
CREATE POLICY tenant_isolation_policy ON "Users"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

-- Teams table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Teams";
CREATE POLICY tenant_isolation_policy ON "Teams"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

-- Organizations table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Organizations";
CREATE POLICY tenant_isolation_policy ON "Organizations"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

-- ContactPersons table
DROP POLICY IF EXISTS tenant_isolation_policy ON "ContactPersons";
CREATE POLICY tenant_isolation_policy ON "ContactPersons"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

-- EmailAddresses table
DROP POLICY IF EXISTS tenant_isolation_policy ON "EmailAddresses";
CREATE POLICY tenant_isolation_policy ON "EmailAddresses"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

-- PhoneNumbers table
DROP POLICY IF EXISTS tenant_isolation_policy ON "PhoneNumbers";
CREATE POLICY tenant_isolation_policy ON "PhoneNumbers"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

-- Addresses table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Addresses";
CREATE POLICY tenant_isolation_policy ON "Addresses"
    USING ("TenantId" = current_setting('app.current_tenant_id', TRUE)::TEXT);

-- Audit logs (if exists and tenant-scoped)
DO $$ 
BEGIN
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'AuditLogs') THEN
        EXECUTE 'DROP POLICY IF EXISTS tenant_isolation_policy ON "AuditLogs"';
        EXECUTE 'CREATE POLICY tenant_isolation_policy ON "AuditLogs"
                 USING ("TenantId" = current_setting(''app.current_tenant_id'', TRUE)::TEXT)';
    END IF;
END $$;

-- Todos table (application-specific)
DO $$ 
BEGIN
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'Todos') THEN
        EXECUTE 'DROP POLICY IF EXISTS tenant_isolation_policy ON "Todos"';
        EXECUTE 'CREATE POLICY tenant_isolation_policy ON "Todos"
                 USING ("TenantId" = current_setting(''app.current_tenant_id'', TRUE)::TEXT)';
    END IF;
END $$;

-- ========================================
-- Create Super Admin Bypass Policy (Optional)
-- ========================================
-- Allows system administrators to see all tenant data
-- Only enable this if you need super admin access

-- Uncomment if needed:
-- CREATE POLICY superadmin_bypass_policy ON "Users"
--     USING (current_setting('app.is_superadmin', TRUE)::BOOLEAN = TRUE);

-- ========================================
-- Verification Queries
-- ========================================

-- Verify RLS is enabled on tables
SELECT 
    schemaname, 
    tablename, 
    rowsecurity as rls_enabled
FROM pg_tables 
WHERE schemaname = 'public' 
    AND rowsecurity = true
ORDER BY tablename;

-- Verify policies exist
SELECT 
    schemaname,
    tablename,
    policyname,
    permissive,
    roles,
    cmd,
    qual as using_expression
FROM pg_policies
WHERE schemaname = 'public'
ORDER BY tablename, policyname;

-- ========================================
-- Test RLS Policies
-- ========================================

-- Test setting tenant context
SELECT set_current_tenant('test-tenant-123');

-- Verify context is set
SELECT current_setting('app.current_tenant_id', TRUE) as current_tenant;

-- Test querying with RLS (should only return data for test-tenant-123)
-- SELECT * FROM "Users" LIMIT 5;

-- ========================================
-- Disable RLS (Emergency Only)
-- ========================================
-- Only use this if you need to temporarily disable RLS for maintenance
-- NEVER disable in production without understanding the consequences

-- ALTER TABLE "Users" DISABLE ROW LEVEL SECURITY;
-- ALTER TABLE "Teams" DISABLE ROW LEVEL SECURITY;
-- ... etc

PRINT('Row-Level Security setup complete!');
PRINT('Remember to set tenant context before queries:');
PRINT('  SELECT set_current_tenant(''your-tenant-id'');');
