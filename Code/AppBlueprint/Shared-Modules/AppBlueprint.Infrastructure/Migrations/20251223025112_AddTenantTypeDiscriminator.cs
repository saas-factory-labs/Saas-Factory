using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantTypeDiscriminator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamInviteEntity");

            migrationBuilder.DropTable(
                name: "TeamMemberEntity");

            migrationBuilder.DropTable(
                name: "Todos");

            migrationBuilder.DropTable(
                name: "TeamEntity");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_Email",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tenants");

            migrationBuilder.AlterColumn<string>(
                name: "VatNumber",
                table: "Tenants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                comment: "VAT/Tax number (B2B only)",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Tenants",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                comment: "Contact phone for the tenant",
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "Tenant name: Full name for Personal, Company name for Organization",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Timestamp when tenant was last modified",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsSoftDeleted",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "Soft delete flag",
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrimary",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                comment: "Indicates primary tenant for multi-tenant B2C users",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                comment: "Whether tenant can access the system",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                comment: "Contact email for the tenant",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tenants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                comment: "Optional description, typically used for Organization tenants",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Tenants",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: false,
                comment: "Timestamp when tenant was created",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                comment: "Country code (B2B only)",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Tenants",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                comment: "Unique tenant identifier with prefix (e.g., tenant_01ABCD...)",
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<int>(
                name: "TenantType",
                table: "Tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "0 = Personal (B2C), 1 = Organization (B2B)");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Email_TenantType",
                table: "Tenants",
                columns: new[] { "Email", "TenantType" });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_TenantType",
                table: "Tenants",
                column: "TenantType");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Type_Active_NotDeleted",
                table: "Tenants",
                columns: new[] { "TenantType", "IsActive", "IsSoftDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenants_Email_TenantType",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_TenantType",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_Type_Active_NotDeleted",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TenantType",
                table: "Tenants");

            migrationBuilder.AlterColumn<string>(
                name: "VatNumber",
                table: "Tenants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "VAT/Tax number (B2B only)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Tenants",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Contact phone for the tenant");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "Tenant name: Full name for Personal, Company name for Organization");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Timestamp when tenant was last modified");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSoftDeleted",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "Soft delete flag");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrimary",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldComment: "Indicates primary tenant for multi-tenant B2C users");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldComment: "Whether tenant can access the system");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Contact email for the tenant");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tenants",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "Optional description, typically used for Organization tenants");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "Tenants",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldComment: "Timestamp when tenant was created");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Tenants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Country code (B2B only)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Tenants",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldComment: "Unique tenant identifier with prefix (e.g., tenant_01ABCD...)");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Tenants",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TeamEntity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    OwnerId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    TenantId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamEntity_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Teams_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    AssignedToId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Title = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamInviteEntity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    OwnerId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    TeamId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpireAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamInviteEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamInviteEntity_TeamEntity_TeamId",
                        column: x => x.TeamId,
                        principalTable: "TeamEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamInviteEntity_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMemberEntity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    TeamId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    UserId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Alias = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMemberEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMemberEntity_TeamEntity_TeamId",
                        column: x => x.TeamId,
                        principalTable: "TeamEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMemberEntity_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Email",
                table: "Tenants",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamEntity_OwnerId",
                table: "TeamEntity",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamEntity_TenantId",
                table: "TeamEntity",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamInviteEntity_OwnerId",
                table: "TeamInviteEntity",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamInviteEntity_TeamId",
                table: "TeamInviteEntity",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberEntity_TeamId",
                table: "TeamMemberEntity",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberEntity_UserId",
                table: "TeamMemberEntity",
                column: "UserId");
        }
    }
}
