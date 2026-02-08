using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Migrations;

/// <inheritdoc />
public partial class AddFullTextSearchVectors : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
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
            name: "IX_Users_SearchVector",
            table: "Users",
            column: "SearchVector")
            .Annotation("Npgsql:IndexMethod", "GIN");

        migrationBuilder.CreateIndex(
            name: "IX_Tenants_SearchVector",
            table: "Tenants",
            column: "SearchVector")
            .Annotation("Npgsql:IndexMethod", "GIN");

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
            name: "WebhookEvents");

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
    }
}
