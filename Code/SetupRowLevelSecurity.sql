
--------------------------
-- USERS                 --
---------------------------
IF NOT EXISTS (
  SELECT FROM pg_catalog.pg_roles
  WHERE  rolname = 'app_user') THEN

  CREATE ROLE app_user LOGIN PASSWORD 'app_user';

END IF;

---------------------------
-- RLS                   --
---------------------------
ALTER TABLE sample.customer ENABLE ROW LEVEL SECURITY;

---------------------------
-- RLS POLICIES         --
---------------------------

DROP POLICY IF EXISTS tenant_customer_isolation_policy ON sample.customer;

CREATE POLICY tenant_customer_isolation_policy ON sample.customer
    USING (tenant_name = current_setting('app.current_tenant')::VARCHAR);

--------------------------------
-- GRANTS                     --
--------------------------------
GRANT USAGE ON SCHEMA sample TO app_user;

-------------------------------------
-- GRANT TABLE                     --
-------------------------------------
GRANT SELECT ON TABLE sample.tenant TO app_user;

GRANT ALL ON SEQUENCE sample.customer_customer_id_seq TO app_user;
GRANT SELECT, UPDATE, INSERT, DELETE ON TABLE 