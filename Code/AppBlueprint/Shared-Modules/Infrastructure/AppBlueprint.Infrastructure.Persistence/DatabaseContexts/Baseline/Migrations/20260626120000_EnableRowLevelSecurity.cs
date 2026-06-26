using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Migrations;

public partial class EnableRowLevelSecurity : Migration
{
    private static readonly string[] TenantScopedTables =
    [
        "Accounts",
        "Addresses",
        "AuditLogs",
        "ContactPersons",
        "Credits",
        "DataExports",
        "EmailAddresses",
        "FileMetadata",
        "NotificationPreferences",
        "PhoneNumbers",
        "PushNotificationTokens",
        "Subscriptions",
        "UserNotifications",
        "Users",
        "WebhookEvents",
    ];

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE OR REPLACE FUNCTION get_tenant_config_key()
            RETURNS TEXT LANGUAGE plpgsql IMMUTABLE AS $$
            BEGIN
                RETURN 'app.current_tenant_id';
            END;
            $$;

            CREATE OR REPLACE FUNCTION set_current_tenant(tenant_id TEXT)
            RETURNS VOID LANGUAGE plpgsql AS $$
            BEGIN
                PERFORM set_config(get_tenant_config_key(), tenant_id, FALSE);
            END;
            $$;

            CREATE OR REPLACE FUNCTION get_current_tenant()
            RETURNS TEXT LANGUAGE plpgsql AS $$
            BEGIN
                RETURN current_setting(get_tenant_config_key(), TRUE);
            END;
            $$;

            CREATE OR REPLACE FUNCTION is_admin_user()
            RETURNS BOOLEAN LANGUAGE plpgsql STABLE AS $$
            BEGIN
                RETURN current_setting('app.is_admin', TRUE) = 'true';
            EXCEPTION
                WHEN OTHERS THEN
                    RETURN FALSE;
            END;
            $$;
            """);

        foreach (string table in TenantScopedTables)
        {
            migrationBuilder.Sql($"""
                ALTER TABLE "{table}" ENABLE ROW LEVEL SECURITY;
                ALTER TABLE "{table}" FORCE ROW LEVEL SECURITY;

                DROP POLICY IF EXISTS tenant_isolation_policy ON "{table}";
                DROP POLICY IF EXISTS tenant_isolation_read_policy ON "{table}";
                DROP POLICY IF EXISTS tenant_isolation_write_policy ON "{table}";

                CREATE POLICY tenant_isolation_read_policy ON "{table}"
                    FOR SELECT
                    USING ("TenantId" = get_current_tenant() OR is_admin_user());

                CREATE POLICY tenant_isolation_write_policy ON "{table}"
                    FOR ALL
                    USING ("TenantId" = get_current_tenant())
                    WITH CHECK ("TenantId" = get_current_tenant());
                """);
        }
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (string table in TenantScopedTables)
        {
            migrationBuilder.Sql($"""
                DROP POLICY IF EXISTS tenant_isolation_read_policy ON "{table}";
                DROP POLICY IF EXISTS tenant_isolation_write_policy ON "{table}";
                DROP POLICY IF EXISTS tenant_isolation_policy ON "{table}";
                ALTER TABLE "{table}" DISABLE ROW LEVEL SECURITY;
                """);
        }

        migrationBuilder.Sql("""
            DROP FUNCTION IF EXISTS is_admin_user();
            DROP FUNCTION IF EXISTS get_current_tenant();
            DROP FUNCTION IF EXISTS set_current_tenant(TEXT);
            DROP FUNCTION IF EXISTS get_tenant_config_key();
            """);
    }
}
