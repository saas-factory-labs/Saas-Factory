using AppBlueprint.SharedKernel.SharedModels;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWebhookTenantAndEventTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Webhooks",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                comment: "Customer-registered outbound endpoint URL",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Secret",
                table: "Webhooks",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                comment: "HMAC-SHA256 signing secret for outbound payloads",
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AddColumn<string>(
                name: "EventTypes",
                table: "Webhooks",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "",
                comment: "Comma-separated event type filter; empty means all events");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Webhooks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                comment: "Tenant this webhook endpoint belongs to");

            migrationBuilder.AddColumn<EntityMetadata>(
                name: "Metadata",
                table: "Users",
                type: "jsonb",
                nullable: true,
                comment: "Generic metadata including PII detection results");

            migrationBuilder.AlterColumn<string>(
                name: "IsoCode",
                table: "Countries",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<EntityMetadata>(
                name: "Metadata",
                table: "AuditLogs",
                type: "jsonb",
                nullable: true,
                comment: "Generic metadata including PII detection results");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerType",
                table: "Accounts",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

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
                name: "IX_Webhooks_TenantId",
                table: "Webhooks",
                column: "TenantId");

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

            migrationBuilder.DropIndex(
                name: "IX_Webhooks_TenantId",
                table: "Webhooks");

            migrationBuilder.DropColumn(
                name: "EventTypes",
                table: "Webhooks");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Webhooks");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "AuditLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Webhooks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048,
                oldComment: "Customer-registered outbound endpoint URL");

            migrationBuilder.AlterColumn<string>(
                name: "Secret",
                table: "Webhooks",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldComment: "HMAC-SHA256 signing secret for outbound payloads");

            migrationBuilder.AlterColumn<int>(
                name: "IsoCode",
                table: "Countries",
                type: "integer",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerType",
                table: "Accounts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
