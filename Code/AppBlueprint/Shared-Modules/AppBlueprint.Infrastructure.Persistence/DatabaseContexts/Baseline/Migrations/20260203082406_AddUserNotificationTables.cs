using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Migrations;

/// <inheritdoc />
public partial class AddUserNotificationTables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Users_SearchVector",
            table: "Users");

        migrationBuilder.DropIndex(
            name: "IX_Tenants_SearchVector",
            table: "Tenants");

        migrationBuilder.DropColumn(
            name: "SearchVector",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "SearchVector",
            table: "Tenants");

        migrationBuilder.CreateTable(
            name: "NotificationPreferences",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                TenantId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                UserId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                EmailEnabled = table.Column<bool>(type: "boolean", nullable: false),
                InAppEnabled = table.Column<bool>(type: "boolean", nullable: false),
                PushEnabled = table.Column<bool>(type: "boolean", nullable: false),
                SmsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                QuietHoursStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                QuietHoursEnd = table.Column<TimeSpan>(type: "interval", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationPreferences", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PushNotificationTokens",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                TenantId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                UserId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                Token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                DeviceType = table.Column<int>(type: "integer", nullable: false),
                DeviceInfo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PushNotificationTokens", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "UserNotifications",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                TenantId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                UserId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                ActionUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                IsRead = table.Column<bool>(type: "boolean", nullable: false),
                ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserNotifications", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_NotificationPreferences_TenantId",
            table: "NotificationPreferences",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_NotificationPreferences_UserId",
            table: "NotificationPreferences",
            column: "UserId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PushNotificationTokens_TenantId",
            table: "PushNotificationTokens",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_PushNotificationTokens_Token",
            table: "PushNotificationTokens",
            column: "Token",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PushNotificationTokens_UserId",
            table: "PushNotificationTokens",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_PushNotificationTokens_UserId_IsActive",
            table: "PushNotificationTokens",
            columns: new[] { "UserId", "IsActive" });

        migrationBuilder.CreateIndex(
            name: "IX_UserNotifications_CreatedAt",
            table: "UserNotifications",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_UserNotifications_TenantId",
            table: "UserNotifications",
            column: "TenantId");

        migrationBuilder.CreateIndex(
            name: "IX_UserNotifications_UserId",
            table: "UserNotifications",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_UserNotifications_UserId_IsRead",
            table: "UserNotifications",
            columns: new[] { "UserId", "IsRead" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "NotificationPreferences");

        migrationBuilder.DropTable(
            name: "PushNotificationTokens");

        migrationBuilder.DropTable(
            name: "UserNotifications");

        migrationBuilder.AddColumn<string>(
            name: "SearchVector",
            table: "Users",
            type: "character varying(1024)",
            maxLength: 1024,
            nullable: true,
            computedColumnSql: "to_tsvector('english', coalesce(\"FirstName\", '') || ' ' || coalesce(\"LastName\", '') || ' ' || coalesce(\"UserName\", '') || ' ' || coalesce(\"Email\", ''))",
            stored: true,
            comment: "Full-text search vector for user search");

        migrationBuilder.AddColumn<string>(
            name: "SearchVector",
            table: "Tenants",
            type: "character varying(1024)",
            maxLength: 1024,
            nullable: true,
            computedColumnSql: "to_tsvector('english', coalesce(\"Name\", '') || ' ' || coalesce(\"Description\", '') || ' ' || coalesce(\"Email\", '') || ' ' || coalesce(\"VatNumber\", ''))",
            stored: true,
            comment: "Full-text search vector for tenant search");

        migrationBuilder.CreateIndex(
            name: "IX_Users_SearchVector",
            table: "Users",
            column: "SearchVector")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "IX_Tenants_SearchVector",
            table: "Tenants",
            column: "SearchVector")
            .Annotation("Npgsql:IndexMethod", "GIN");
    }
}
