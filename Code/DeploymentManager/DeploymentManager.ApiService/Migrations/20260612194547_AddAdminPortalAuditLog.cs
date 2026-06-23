using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeploymentManager.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminPortalAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dm_admin_audit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    AppSlug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AdminUserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AdminEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TargetType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TargetId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    TenantId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Details = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dm_admin_audit", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dm_admin_audit_AdminUserId",
                table: "dm_admin_audit",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_dm_admin_audit_AppSlug_OccurredAtUtc",
                table: "dm_admin_audit",
                columns: new[] { "AppSlug", "OccurredAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dm_admin_audit");
        }
    }
}
