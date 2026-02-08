using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.SharedKernel.Migrations;

/// <inheritdoc />
public partial class AddLanguagesBaseEntityColumns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        // Add BaseEntity columns to Languages table
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Languages",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

        migrationBuilder.AddColumn<bool>(
            name: "IsSoftDeleted",
            table: "Languages",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "LastUpdatedAt",
            table: "Languages",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        // Remove BaseEntity columns from Languages table
        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Languages");

        migrationBuilder.DropColumn(
            name: "IsSoftDeleted",
            table: "Languages");

        migrationBuilder.DropColumn(
            name: "LastUpdatedAt",
            table: "Languages");
    }
}
