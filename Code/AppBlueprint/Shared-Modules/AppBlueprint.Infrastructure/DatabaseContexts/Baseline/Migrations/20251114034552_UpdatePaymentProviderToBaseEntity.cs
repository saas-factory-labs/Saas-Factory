using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentProviderToBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "PaymentProviders",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "Timestamp when the payment provider was last updated");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PaymentProviders",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP",
                oldComment: "Timestamp when the payment provider was created");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "PaymentProviders",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Unique identifier for the payment provider")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<bool>(
                name: "IsSoftDeleted",
                table: "PaymentProviders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSoftDeleted",
                table: "PaymentProviders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "PaymentProviders",
                type: "timestamp with time zone",
                nullable: true,
                comment: "Timestamp when the payment provider was last updated",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PaymentProviders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                comment: "Timestamp when the payment provider was created",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PaymentProviders",
                type: "integer",
                nullable: false,
                comment: "Unique identifier for the payment provider",
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
