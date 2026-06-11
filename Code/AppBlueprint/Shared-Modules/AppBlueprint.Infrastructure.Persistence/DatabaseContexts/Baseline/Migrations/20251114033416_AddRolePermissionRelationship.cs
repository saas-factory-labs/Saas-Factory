using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Migrations;

/// <inheritdoc />
public partial class AddRolePermissionRelationship : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.AddColumn<string>(
            name: "PermissionId",
            table: "RolePermissions",
            type: "character varying(40)",
            maxLength: 40,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateIndex(
            name: "IX_RolePermissions_PermissionId",
            table: "RolePermissions",
            column: "PermissionId");

        migrationBuilder.CreateIndex(
            name: "IX_RolePermissions_RoleId_PermissionId_Unique",
            table: "RolePermissions",
            columns: new[] { "RoleId", "PermissionId" },
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_RolePermissions_Permissions_PermissionId",
            table: "RolePermissions",
            column: "PermissionId",
            principalTable: "Permissions",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropForeignKey(
            name: "FK_RolePermissions_Permissions_PermissionId",
            table: "RolePermissions");

        migrationBuilder.DropIndex(
            name: "IX_RolePermissions_PermissionId",
            table: "RolePermissions");

        migrationBuilder.DropIndex(
            name: "IX_RolePermissions_RoleId_PermissionId_Unique",
            table: "RolePermissions");

        migrationBuilder.DropColumn(
            name: "PermissionId",
            table: "RolePermissions");
    }
}
