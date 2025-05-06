using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.SharedKernel.Migrations
{
    /// <inheritdoc />
    public partial class Refinedschematoenforcenotnulloncolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop the existing FileSize column
            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "DataExports");

            // Step 2: Recreate FileSize with the correct type
            migrationBuilder.AddColumn<double>(
                name: "FileSize",
                table: "DataExports",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "DownloadUrl",
                table: "DataExports",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "AuditLogs",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ApiKeys",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop the new FileSize column
            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "DataExports");

            // Step 2: Recreate FileSize with the old type (string)
            migrationBuilder.AddColumn<string>(
                name: "FileSize",
                table: "DataExports",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "0");

            migrationBuilder.AlterColumn<string>(
                name: "DownloadUrl",
                table: "DataExports",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "AuditLogs",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ApiKeys",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024,
                oldNullable: true);
        }
    }
}
