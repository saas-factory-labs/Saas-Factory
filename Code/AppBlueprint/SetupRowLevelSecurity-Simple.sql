-- ========================================
-- PostgreSQL Row-Level Security Setup
-- SIMPLIFIED VERSION for GUI tools
-- ========================================

-- Create RLS helper functions
CREATE OR REPLACE FUNCTION get_tenant_config_key()
RETURNS TEXT AS $$
BEGIN
    RETURN 'app.current_tenant_id';
END;
$$ LANGUAGE plpgsql IMMUTABLE;

CREATE OR REPLACE FUNCTION set_current_tenant(tenant_id TEXT)
RETURNS VOID AS $$
BEGIN
    PERFORM set_config(get_tenant_config_key(), tenant_id, FALSE);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION get_current_tenant()
RETURNS TEXT AS $$
BEGIN
    RETURN current_setting(get_tenant_config_key(), TRUE);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION is_admin_user()
RETURNS BOOLEAN AS $$
BEGIN
    RETURN current_setting('app.is_admin', TRUE) = 'true';
EXCEPTION
    WHEN OTHERS THEN
        RETURN FALSE;
END;
$$ LANGUAGE plpgsql STABLE;

-- Enable RLS on core tables
ALTER TABLE "Users" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Teams" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Organizations" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "ContactPersons" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "EmailAddresses" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "PhoneNumbers" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Addresses" ENABLE ROW LEVEL SECURITY;

-- Create policies for Users table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Users";
DROP POLICY IF EXISTS tenant_isolation_read_policy ON "Users";
DROP POLICY IF EXISTS tenant_isolation_write_policy ON "Users";

CREATE POLICY tenant_isolation_read_policy ON "Users"
    FOR SELECT
    USING ("TenantId" = get_current_tenant() OR is_admin_user());

CREATE POLICY tenant_isolation_write_policy ON "Users"
    FOR ALL
    USING ("TenantId" = get_current_tenant())
    WITH CHECK ("TenantId" = get_current_tenant());

-- Create policies for Teams table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Teams";
DROP POLICY IF EXISTS tenant_isolation_read_policy ON "Teams";
DROP POLICY IF EXISTS tenant_isolation_write_policy ON "Teams";

CREATE POLICY tenant_isolation_read_policy ON "Teams"
    FOR SELECT
    USING ("TenantId" = get_current_tenant() OR is_admin_user());

CREATE POLICY tenant_isolation_write_policy ON "Teams"
    FOR ALL
    USING ("TenantId" = get_current_tenant())
    WITH CHECK ("TenantId" = get_current_tenant());

-- Create policies for Organizations table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Organizations";
CREATE POLICY tenant_isolation_policy ON "Organizations"
    USING ("TenantId" = get_current_tenant());

-- Create policies for ContactPersons table
DROP POLICY IF EXISTS tenant_isolation_policy ON "ContactPersons";
CREATE POLICY tenant_isolation_policy ON "ContactPersons"
    USING ("TenantId" = get_current_tenant());

-- Create policies for EmailAddresses table
DROP POLICY IF EXISTS tenant_isolation_policy ON "EmailAddresses";
CREATE POLICY tenant_isolation_policy ON "EmailAddresses"
    USING ("TenantId" = get_current_tenant());

-- Create policies for PhoneNumbers table
DROP POLICY IF EXISTS tenant_isolation_policy ON "PhoneNumbers";
CREATE POLICY tenant_isolation_policy ON "PhoneNumbers"
    USING ("TenantId" = get_current_tenant());

-- Create policies for Addresses table
DROP POLICY IF EXISTS tenant_isolation_policy ON "Addresses";
CREATE POLICY tenant_isolation_policy ON "Addresses"
    USING ("TenantId" = get_current_tenant());

-- Create Admin Audit Log table
CREATE TABLE IF NOT EXISTS "AdminAuditLog" (
    "Id" SERIAL PRIMARY KEY,
    "AdminUserId" TEXT NOT NULL,
    "TenantId" TEXT NOT NULL,
    "Operation" TEXT NOT NULL,
    "Reason" TEXT NOT NULL,
    "Result" TEXT NOT NULL,
    "IpAddress" INET,
    "Timestamp" TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_admin_audit_tenant ON "AdminAuditLog"("TenantId", "Timestamp" DESC);
CREATE INDEX IF NOT EXISTS idx_admin_audit_user ON "AdminAuditLog"("AdminUserId", "Timestamp" DESC);
CREATE INDEX IF NOT EXISTS idx_admin_audit_timestamp ON "AdminAuditLog"("Timestamp" DESC);

-- Verify RLS is enabled
SELECT 
    tablename, 
    rowsecurity as rls_enabled
FROM pg_tables 
WHERE schemaname = 'public' 
    AND rowsecurity
ORDER BY tablename;

-- Verify policies exist
SELECT 
    tablename,
    policyname
FROM pg_policies
WHERE schemaname = 'public'
ORDER BY tablename, policyname;