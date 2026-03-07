using System;
using System.Collections.Generic;
using AppBlueprint.SharedKernel.SharedModels;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B.Migrations
{
    /// <inheritdoc />
    public partial class AddApiKeyHashAndLifecycleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiresAt",
                table: "ApiKeys",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Optional expiry timestamp; null means the key never expires");

            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "ApiKeys",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Whether this API key has been revoked");

            migrationBuilder.AddColumn<string>(
                name: "KeyHash",
                table: "ApiKeys",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                comment: "SHA-256 hex hash of the raw API key for fast lookup");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerType",
                table: "Accounts",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "FileMetadata",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    FileKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Folder = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PublicUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CustomMetadata = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: true, defaultValueSql: "'{}'"),
                    TenantId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileMetadata", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "WebhookEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, comment: "Unique webhook event identifier with prefix (e.g., whevt_01ABCD...)"),
                    EventId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "External event ID from webhook provider (e.g., Stripe event ID)"),
                    EventType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Type of webhook event (e.g., payment_intent.succeeded)"),
                    Source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Source of webhook (e.g., stripe, paypal)"),
                    Payload = table.Column<string>(type: "text", maxLength: 1024, nullable: false, comment: "Raw JSON payload of webhook event"),
                    TenantId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false, defaultValue: "", comment: "Tenant ID if webhook is tenant-scoped"),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Processing status (Pending, Processed, Failed, etc.)"),
                    RetryCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "Number of retry attempts"),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true, comment: "Error message if processing failed"),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Timestamp when event was received"),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Timestamp when event was successfully processed"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_KeyHash",
                table: "ApiKeys",
                column: "KeyHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadata_CreatedAt",
                table: "FileMetadata",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadata_FileKey",
                table: "FileMetadata",
                column: "FileKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadata_TenantId",
                table: "FileMetadata",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadata_TenantId_Folder",
                table: "FileMetadata",
                columns: new[] { "TenantId", "Folder" });

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadata_TenantId_UploadedBy",
                table: "FileMetadata",
                columns: new[] { "TenantId", "UploadedBy" });

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

            migrationBuilder.CreateIndex(
                name: "IX_WebhookEvents_EventId_Source",
                table: "WebhookEvents",
                columns: new[] { "EventId", "Source" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebhookEvents_Id",
                table: "WebhookEvents",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebhookEvents_ReceivedAt",
                table: "WebhookEvents",
                column: "ReceivedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookEvents_Status",
                table: "WebhookEvents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookEvents_TenantId",
                table: "WebhookEvents",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileMetadata");

            migrationBuilder.DropTable(
                name: "NotificationPreferences");

            migrationBuilder.DropTable(
                name: "PushNotificationTokens");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "WebhookEvents");

            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_KeyHash",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "KeyHash",
                table: "ApiKeys");

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
