DO $$
DECLARE
    appUser TEXT := 'app_user';
    sampleSchema TEXT := 'sample';
    rlsPolicyName TEXT := 'tenant_customer_isolation_policy';
BEGIN
    ---------------------------
    -- USERS                 --
    ---------------------------
    IF NOT EXISTS (
      SELECT FROM pg_catalog.pg_roles
      WHERE  rolname = appUser) THEN

      EXECUTE 'CREATE ROLE ' || appUser || ' LOGIN PASSWORD ''app_user''';

    END IF;

    ---------------------------
    -- RLS                   --
    ---------------------------
    EXECUTE 'ALTER TABLE ' || sampleSchema || '.customer ENABLE ROW LEVEL SECURITY';

    ---------------------------
    -- RLS POLICIES         --
    ---------------------------
    EXECUTE 'DROP POLICY IF EXISTS ' || rlsPolicyName || ' ON ' || sampleSchema || '.customer';

    EXECUTE 'CREATE POLICY ' || rlsPolicyName || ' ON ' || sampleSchema || '.customer
        USING (tenant_name = current_setting(''app.current_tenant'')::VARCHAR)';

    --------------------------------
    -- GRANTS                     --
    --------------------------------
    EXECUTE 'GRANT USAGE ON SCHEMA ' || sampleSchema || ' TO ' || appUser;

    -------------------------------------
    -- GRANT TABLE                     --
    -------------------------------------
    EXECUTE 'GRANT SELECT ON TABLE ' || sampleSchema || '.tenant TO ' || appUser;

    EXECUTE 'GRANT ALL ON SEQUENCE ' || sampleSchema || '.customer_customer_id_seq TO ' || appUser;
    EXECUTE 'GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE ' || sampleSchema || '.customer TO ' || appUser;

END $$;