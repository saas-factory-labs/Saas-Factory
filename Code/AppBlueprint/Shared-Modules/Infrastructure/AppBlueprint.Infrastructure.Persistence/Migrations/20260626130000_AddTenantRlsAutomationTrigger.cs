using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.Infrastructure.Persistence.Migrations
{
    // Requires: 20260626120000_EnableRowLevelSecurity (creates get_current_tenant, is_admin_user functions)
    // Requires: DB role has CREATE EVENT TRIGGER privilege (superuser or pg_ddl_admin membership)
    public partial class AddTenantRlsAutomationTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Auto-applies RLS policies to any table that has a TenantId column,
            // triggered on CREATE TABLE or ALTER TABLE. Covers all future migrations
            // across all DbContexts without manual table list maintenance.
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION apply_tenant_rls_on_ddl()
                RETURNS event_trigger
                LANGUAGE plpgsql AS $$
                DECLARE
                    cmd        record;
                    tbl_name   text;
                    has_tenant boolean;
                BEGIN
                    FOR cmd IN
                        SELECT * FROM pg_event_trigger_ddl_commands()
                        WHERE command_tag IN ('CREATE TABLE', 'ALTER TABLE')
                          AND object_type = 'table'
                    LOOP
                        SELECT c.relname INTO tbl_name
                        FROM pg_class c
                        JOIN pg_namespace n ON n.oid = c.relnamespace
                        WHERE c.oid = cmd.objid AND n.nspname = 'public';

                        CONTINUE WHEN tbl_name IS NULL;

                        SELECT EXISTS (
                            SELECT 1 FROM information_schema.columns
                            WHERE table_schema = 'public'
                              AND table_name   = tbl_name
                              AND column_name  = 'TenantId'
                        ) INTO has_tenant;

                        CONTINUE WHEN NOT has_tenant;

                        EXECUTE format('ALTER TABLE public.%I ENABLE ROW LEVEL SECURITY', tbl_name);
                        EXECUTE format('ALTER TABLE public.%I FORCE ROW LEVEL SECURITY',  tbl_name);

                        EXECUTE format('DROP POLICY IF EXISTS tenant_isolation_policy       ON public.%I', tbl_name);
                        EXECUTE format('DROP POLICY IF EXISTS tenant_isolation_read_policy  ON public.%I', tbl_name);
                        EXECUTE format('DROP POLICY IF EXISTS tenant_isolation_write_policy ON public.%I', tbl_name);

                        EXECUTE format(
                            'CREATE POLICY tenant_isolation_read_policy ON public.%I ' ||
                            'FOR SELECT USING ("TenantId" = get_current_tenant() OR is_admin_user())',
                            tbl_name
                        );
                        EXECUTE format(
                            'CREATE POLICY tenant_isolation_write_policy ON public.%I ' ||
                            'FOR ALL USING ("TenantId" = get_current_tenant()) ' ||
                            'WITH CHECK ("TenantId" = get_current_tenant())',
                            tbl_name
                        );
                    END LOOP;
                END;
                $$;

                DROP EVENT TRIGGER IF EXISTS auto_tenant_rls_trigger;

                CREATE EVENT TRIGGER auto_tenant_rls_trigger
                    ON ddl_command_end
                    WHEN TAG IN ('CREATE TABLE', 'ALTER TABLE')
                    EXECUTE FUNCTION apply_tenant_rls_on_ddl();
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP EVENT TRIGGER IF EXISTS auto_tenant_rls_trigger;
                DROP FUNCTION IF EXISTS apply_tenant_rls_on_ddl();
                """);
        }
    }
}
