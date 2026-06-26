-- ========================================
-- PostgreSQL Row-Level Security Setup
-- ========================================

-- Helper functions
CREATE OR REPLACE FUNCTION get_tenant_config_key()
RETURNS TEXT
LANGUAGE plpgsql IMMUTABLE
AS $func$
BEGIN
    RETURN 'app.current_tenant_id';
END
$func$;

CREATE OR REPLACE FUNCTION set_current_tenant(tenant_id TEXT)
RETURNS VOID
LANGUAGE plpgsql
AS $func$
BEGIN
    PERFORM set_config(get_tenant_config_key(), tenant_id, FALSE);
END
$func$;

CREATE OR REPLACE FUNCTION get_current_tenant()
RETURNS TEXT
LANGUAGE plpgsql
AS $func$
BEGIN
    RETURN current_setting(get_tenant_config_key(), TRUE);
END
$func$;

CREATE OR REPLACE FUNCTION is_admin_user()
RETURNS BOOLEAN
LANGUAGE plpgsql STABLE
AS $func$
BEGIN
    RETURN current_setting('app.is_admin', TRUE) = 'true';
EXCEPTION
    WHEN OTHERS THEN
        RETURN FALSE;
END
$func$;

-- Enable RLS on core tables
ALTER TABLE "Users" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Teams" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Organizations" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "ContactPersons" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "EmailAddresses" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "PhoneNumbers" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "Addresses" ENABLE ROW LEVEL SECURITY;

-- Users policies
DROP POLICY IF EXISTS tenant_isolation_read_policy ON "Users";
DROP POLICY IF EXISTS tenant_isolation_write_policy ON "Users";

CREATE POLICY tenant_isolation_read_policy ON "Users"
    FOR SELECT
    USING ("TenantId" = get_current_tenant() OR is_admin_user());

CREATE POLICY tenant_isolation_write_policy ON "Users"
    FOR ALL
    USING ("TenantId" = get_current_tenant())
    WITH CHECK ("TenantId" = get_current_tenant());

-- Teams policies
DROP POLICY IF EXISTS tenant_isolation_read_policy ON "Teams";
DROP POLICY IF EXISTS tenant_isolation_write_policy ON "Teams";

CREATE POLICY tenant_isolation_read_policy ON "Teams"
    FOR SELECT
    USING ("TenantId" = get_current_tenant() OR is_admin_user());

CREATE POLICY tenant_isolation_write_policy ON "Teams"
    FOR ALL
    USING ("TenantId" = get_current_tenant())
    WITH CHECK ("TenantId" = get_current_tenant());

-- Simple policies for other tables
DROP POLICY IF EXISTS tenant_isolation_policy ON "Organizations";
CREATE POLICY tenant_isolation_policy ON "Organizations"
    USING ("TenantId" = get_current_tenant());

DROP POLICY IF EXISTS tenant_isolation_policy ON "ContactPersons";
CREATE POLICY tenant_isolation_policy ON "ContactPersons"
    USING ("TenantId" = get_current_tenant());

DROP POLICY IF EXISTS tenant_isolation_policy ON "EmailAddresses";
CREATE POLICY tenant_isolation_policy ON "EmailAddresses"
    USING ("TenantId" = get_current_tenant());

DROP POLICY IF EXISTS tenant_isolation_policy ON "PhoneNumbers";
CREATE POLICY tenant_isolation_policy ON "PhoneNumbers"
    USING ("TenantId" = get_current_tenant());

DROP POLICY IF EXISTS tenant_isolation_policy ON "Addresses";
CREATE POLICY tenant_isolation_policy ON "Addresses"
    USING ("TenantId" = get_current_tenant());

-- Admin audit log table
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

-- Success message
SELECT 'Row-Level Security setup complete!' AS message;