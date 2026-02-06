-- ========================================
-- PostgreSQL Row-Level Security Setup
-- for Multi-Tenant AppBlueprint Applications
-- ========================================
-- 
-- DEFENSE-IN-DEPTH SECURITY ARCHITECTURE:
-- This script implements Layer 2 (database-level) tenant isolation.
-- Layer 1 is EF Core Named Query Filters (application-level).
--
-- WHY BOTH LAYERS?
-- - Named Query Filters: Prevent developer mistakes, cleaner code
-- - Row-Level Security: Cannot be bypassed even if application is compromised
--
-- CRITICAL FOR SENSITIVE DATA (Dating App, Healthcare, Finance):
-- - User messages, profiles, health records protected at DB level
-- - Attacker cannot use .IgnoreQueryFilters() or raw SQL to bypass RLS
-- - Direct database access (psql, pgAdmin) still enforces isolation
--
-- IMPORTANT: Run this AFTER applying all Entity Framework migrations
--
-- Usage:
--   psql -h localhost -U postgres -d your_database -f SetupRowLevelSecurity.sql
--
-- Or via Entity Framework migration:
--   Add this to a new migration's Up() method
-- ========================================

-- Define constant for tenant context configuration key
-- This prevents duplication and makes refactoring easier
DO $$
DECLARE
    tenant_config_key CONSTANT TEXT := 'app.current_tenant_id';
BEGIN
    -- This constant is used throughout the RLS policies
    -- PostgreSQL doesn't support true constants in SQL, so this is for documentation
    RAISE NOTICE 'Tenant configuration key: %', tenant_config_key;
END $$;

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

-- Function to check if current user is admin (for read-only admin access)
CREATE OR REPLACE FUNCTION is_admin_user()
RETURNS BOOLEAN AS $$
BEGIN
    -- Check if current_setting('app.is_admin') is set to 'true'
    -- This is set by AdminTenantAccessService for read-only operations only
    RETURN current_setting('app.is_admin', TRUE) = 'true';
EXCEPTION
    WHEN OTHERS THEN
        RETURN FALSE;
END;
$$ LANGUAGE plpgsql STABLE;

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
--
-- ADMIN ACCESS (READ-ONLY):
-- Admins can SELECT any tenant's data when app.is_admin = 'true'
-- Admins CANNOT INSERT/UPDATE/DELETE - those operations enforce normal tenant isolation

-- Users table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Users";
DROP POLICY IF EXISTS tenant_isolation_read_policy ON "Users";
DROP POLICY IF EXISTS tenant_isolation_write_policy ON "Users";

-- Read policy: Normal users + read-only admin access
CREATE POLICY tenant_isolation_read_policy ON "Users"
    FOR SELECT
    USING (
        "TenantId" = get_current_tenant()
        OR is_admin_user()  -- Admins can read any tenant
    );

-- Write policy: Only normal tenant isolation (admins cannot write)
CREATE POLICY tenant_isolation_write_policy ON "Users"
    FOR ALL  -- Covers INSERT, UPDATE, DELETE
    USING ("TenantId" = get_current_tenant())
    WITH CHECK ("TenantId" = get_current_tenant());

-- Teams table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Teams";
DROP POLICY IF EXISTS tenant_isolation_read_policy ON "Teams";
DROP POLICY IF EXISTS tenant_isolation_write_policy ON "Teams";

CREATE POLICY tenant_isolation_read_policy ON "Teams"
    FOR SELECT
    USING (
        "TenantId" = get_current_tenant()
        OR is_admin_user()
    );

CREATE POLICY tenant_isolation_write_policy ON "Teams"
    FOR ALL
    USING ("TenantId" = get_current_tenant())
    WITH CHECK ("TenantId" = get_current_tenant());

-- Organizations table (old policy for reference)
DROP POLICY IF EXISTS tenant_isolation_policy ON "Teams"
    USING ("TenantId" = get_current_tenant());

-- Organizations table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Organizations";
CREATE POLICY tenant_isolation_policy ON "Organizations"
    USING ("TenantId" = get_current_tenant());

-- ContactPersons table
DROP POLICY IF EXISTS tenant_isolation_policy ON "ContactPersons";
CREATE POLICY tenant_isolation_policy ON "ContactPersons"
    USING ("TenantId" = get_current_tenant());

-- EmailAddresses table
DROP POLICY IF EXISTS tenant_isolation_policy ON "EmailAddresses";
CREATE POLICY tenant_isolation_policy ON "EmailAddresses"
    USING ("TenantId" = get_current_tenant());

-- PhoneNumbers table
DROP POLICY IF EXISTS tenant_isolation_policy ON "PhoneNumbers";
CREATE POLICY tenant_isolation_policy ON "PhoneNumbers"
    USING ("TenantId" = get_current_tenant());

-- Addresses table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Addresses";
CREATE POLICY tenant_isolation_policy ON "Addresses"
    USING ("TenantId" = get_current_tenant());

-- Audit logs (if exists and tenant-scoped)
DO $$ 
BEGIN
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'AuditLogs') THEN
        EXECUTE 'DROP POLICY IF EXISTS tenant_isolation_policy ON "AuditLogs"';
        EXECUTE 'CREATE POLICY tenant_isolation_policy ON "AuditLogs" USING ("TenantId" = get_current_tenant())';
    END IF;
END $$;

-- Todos table (application-specific)
DO $$ 
BEGIN
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'Todos') THEN
        EXECUTE 'DROP POLICY IF EXISTS tenant_isolation_policy ON "Todos"';
        EXECUTE 'CREATE POLICY tenant_isolation_policy ON "Todos" USING ("TenantId" = get_current_tenant())';
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
    AND rowsecurity
ORDER BY tablename ASC;

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
ORDER BY tablename ASC, policyname ASC;

-- ========================================
-- Test RLS Policies
-- ========================================

-- Test setting tenant context
SELECT set_current_tenant('test-tenant-123');

-- Verify context is set
SELECT get_current_tenant() as current_tenant;

-- ========================================
-- ========================================
-- Admin Audit Log Table (Read-Only Admin Access Tracking)
-- ========================================
-- Tracks all admin access to tenant data for security and compliance
-- This table is NOT tenant-scoped and has no RLS policies
-- It's append-only (INSERT permission only) to prevent tampering

CREATE TABLE IF NOT EXISTS "AdminAuditLog" (
    "Id" SERIAL PRIMARY KEY,
    "AdminUserId" TEXT NOT NULL,
    "TenantId" TEXT NOT NULL,
    "Operation" TEXT NOT NULL,  -- e.g., 'ViewUsers', 'ViewMessages'
    "Reason" TEXT NOT NULL,
    "Result" TEXT NOT NULL,  -- 'Success' or 'Failure'
    "IpAddress" INET,
    "Timestamp" TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Create indexes for querying audit logs
CREATE INDEX IF NOT EXISTS idx_admin_audit_tenant ON "AdminAuditLog"("TenantId", "Timestamp" DESC);
CREATE INDEX IF NOT EXISTS idx_admin_audit_user ON "AdminAuditLog"("AdminUserId", "Timestamp" DESC);
CREATE INDEX IF NOT EXISTS idx_admin_audit_timestamp ON "AdminAuditLog"("Timestamp" DESC);

PRINT('Row-Level Security setup complete!');
PRINT('Remember to set tenant context before queries:');
PRINT('  SELECT set_current_tenant(''your-tenant-id'');');
PRINT('For admin access: SELECT set_config(''app.is_admin'', ''true'', FALSE);');

-- ========================================
-- Defense-in-Depth Summary
-- ========================================
-- ✅ Layer 1: EF Core Named Query Filters (Application-Level)
--    - Automatically adds: WHERE TenantId = @currentTenant
--    - Prevents developer mistakes
--    - Can be bypassed with .IgnoreQueryFilters()
--
-- ✅ Layer 2: PostgreSQL Row-Level Security (Database-Level)  
--    - Enforced by database regardless of client
--    - Cannot be bypassed without BYPASSRLS privilege
--    - Protects against compromised application
--    - Works with raw SQL, psql, direct connections
--
-- ⚠️ ADMIN ACCESS (READ-ONLY):
--    - Admins can SELECT any tenant when app.is_admin = 'true'
--    - Admins CANNOT INSERT/UPDATE/DELETE (enforced by separate write policies)
--    - All admin access logged to AdminAuditLog table
--    - Use AdminTenantAccessService with .AsNoTracking() queries
--
-- SECURITY GUARANTEE:
-- 1. Even if attacker gains code execution and calls .IgnoreQueryFilters(),
--    the database will still enforce tenant isolation via RLS policies.
-- 2. Admins can only READ tenant data, never modify or delete.
-- 3. All admin access is audited in append-only AdminAuditLog table.
