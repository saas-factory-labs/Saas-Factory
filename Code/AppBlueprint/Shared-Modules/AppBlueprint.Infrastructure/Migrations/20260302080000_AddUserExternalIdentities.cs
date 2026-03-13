using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AppBlueprint.Infrastructure.Migrations;

/// <summary>
/// Adds UserExternalIdentities table for multi-provider auth support
/// and registers the AuthenticationProviders DbSet.
/// </summary>
public partial class AddUserExternalIdentities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create UserExternalIdentities table
        migrationBuilder.CreateTable(
            name: "UserExternalIdentities",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false, comment: "Unique identifier for the external identity link")
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<string>(type: "character varying(40)", nullable: false, comment: "Internal user ID"),
                AuthenticationProviderId = table.Column<int>(type: "integer", nullable: false, comment: "Foreign key to the authentication provider"),
                ExternalUserId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false, comment: "User ID from the external auth provider (e.g., Logto sub, Auth0 user_id, Firebase localId)"),
                ExternalEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true, comment: "Email from the external provider (may differ from primary email)"),
                ExternalDisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, comment: "Display name from the external provider"),
                LinkedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "When this provider was linked to the user"),
                LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Last login via this provider"),
                IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Whether this identity link is active")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserExternalIdentities", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserExternalIdentities_AuthenticationProviders_AuthenticationProviderId",
                    column: x => x.AuthenticationProviderId,
                    principalTable: "AuthenticationProviders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_UserExternalIdentities_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserExternalIdentities_UserId",
            table: "UserExternalIdentities",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_UserExternalIdentities_UserId_ProviderId",
            table: "UserExternalIdentities",
            columns: new[] { "UserId", "AuthenticationProviderId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserExternalIdentities_ProviderId_ExternalUserId",
            table: "UserExternalIdentities",
            columns: new[] { "AuthenticationProviderId", "ExternalUserId" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "UserExternalIdentities");
    }
}
