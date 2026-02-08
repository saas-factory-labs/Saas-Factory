using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.Infrastructure.Migrations;

/// <inheritdoc />
public partial class FixIndexColumnReferences : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Customers_Organizations_OrganizationEntityId",
            table: "Customers");

        migrationBuilder.DropForeignKey(
            name: "FK_TeamInviteEntity_Teams_TeamId",
            table: "TeamInviteEntity");

        migrationBuilder.DropForeignKey(
            name: "FK_TeamMemberEntity_Teams_TeamId",
            table: "TeamMemberEntity");

        migrationBuilder.DropForeignKey(
            name: "FK_Teams_Organizations_OrganizationEntityId",
            table: "Teams");

        migrationBuilder.DropForeignKey(
            name: "FK_Teams_Users_OwnerId",
            table: "Teams");

        migrationBuilder.DropTable(
            name: "ApiKeys");

        migrationBuilder.DropTable(
            name: "Organizations");

        migrationBuilder.DropIndex(
            name: "IX_Customers_OrganizationEntityId",
            table: "Customers");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Teams",
            table: "Teams");

        migrationBuilder.DropIndex(
            name: "IX_Teams_OrganizationEntityId",
            table: "Teams");

        migrationBuilder.DropColumn(
            name: "OrganizationEntityId",
            table: "Customers");

        migrationBuilder.DropColumn(
            name: "OrganizationEntityId",
            table: "Teams");

        migrationBuilder.RenameTable(
            name: "Teams",
            newName: "TeamEntity");

        migrationBuilder.RenameIndex(
            name: "IX_Teams_TenantId",
            table: "TeamEntity",
            newName: "IX_TeamEntity_TenantId");

        migrationBuilder.RenameIndex(
            name: "IX_Teams_OwnerId",
            table: "TeamEntity",
            newName: "IX_TeamEntity_OwnerId");

        migrationBuilder.AddPrimaryKey(
            name: "PK_TeamEntity",
            table: "TeamEntity",
            column: "Id");

        migrationBuilder.CreateTable(
            name: "Searches",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                Url = table.Column<string>(type: "text", nullable: false),
                SearchType = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                SearchCriteria = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                SearchResults = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                SearchStatus = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                SearchError = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                SearchErrorMessage = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Searches", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Todos",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                Title = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                Priority = table.Column<int>(type: "integer", nullable: false),
                DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                TenantId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                CreatedById = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                AssignedToId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Todos", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Webhooks",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                Url = table.Column<string>(type: "text", nullable: false),
                Secret = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Webhooks", x => x.Id);
            });

        migrationBuilder.AddForeignKey(
            name: "FK_TeamEntity_Users_OwnerId",
            table: "TeamEntity",
            column: "OwnerId",
            principalTable: "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_TeamInviteEntity_TeamEntity_TeamId",
            table: "TeamInviteEntity",
            column: "TeamId",
            principalTable: "TeamEntity",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_TeamMemberEntity_TeamEntity_TeamId",
            table: "TeamMemberEntity",
            column: "TeamId",
            principalTable: "TeamEntity",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_TeamEntity_Users_OwnerId",
            table: "TeamEntity");

        migrationBuilder.DropForeignKey(
            name: "FK_TeamInviteEntity_TeamEntity_TeamId",
            table: "TeamInviteEntity");

        migrationBuilder.DropForeignKey(
            name: "FK_TeamMemberEntity_TeamEntity_TeamId",
            table: "TeamMemberEntity");

        migrationBuilder.DropTable(
            name: "Searches");

        migrationBuilder.DropTable(
            name: "Todos");

        migrationBuilder.DropTable(
            name: "Webhooks");

        migrationBuilder.DropPrimaryKey(
            name: "PK_TeamEntity",
            table: "TeamEntity");

        migrationBuilder.RenameTable(
            name: "TeamEntity",
            newName: "Teams");

        migrationBuilder.RenameIndex(
            name: "IX_TeamEntity_TenantId",
            table: "Teams",
            newName: "IX_Teams_TenantId");

        migrationBuilder.RenameIndex(
            name: "IX_TeamEntity_OwnerId",
            table: "Teams",
            newName: "IX_Teams_OwnerId");

        migrationBuilder.AddColumn<string>(
            name: "OrganizationEntityId",
            table: "Customers",
            type: "character varying(1024)",
            maxLength: 1024,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "OrganizationEntityId",
            table: "Teams",
            type: "character varying(1024)",
            maxLength: 1024,
            nullable: true);

        migrationBuilder.AddPrimaryKey(
            name: "PK_Teams",
            table: "Teams",
            column: "Id");

        migrationBuilder.CreateTable(
            name: "ApiKeys",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                OwnerId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false),
                LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                SecretRef = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                TenantId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                UserId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApiKeys", x => x.Id);
                table.ForeignKey(
                    name: "FK_ApiKeys_Users_OwnerId",
                    column: x => x.OwnerId,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Organizations",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                OwnerId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false),
                LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                TenantId = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Organizations", x => x.Id);
                table.ForeignKey(
                    name: "FK_Organizations_Users_OwnerId",
                    column: x => x.OwnerId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Customers_OrganizationEntityId",
            table: "Customers",
            column: "OrganizationEntityId");

        migrationBuilder.CreateIndex(
            name: "IX_Teams_OrganizationEntityId",
            table: "Teams",
            column: "OrganizationEntityId");

        migrationBuilder.CreateIndex(
            name: "IX_ApiKeys_OwnerId",
            table: "ApiKeys",
            column: "OwnerId");

        migrationBuilder.CreateIndex(
            name: "IX_Organizations_OwnerId",
            table: "Organizations",
            column: "OwnerId");

        migrationBuilder.AddForeignKey(
            name: "FK_Customers_Organizations_OrganizationEntityId",
            table: "Customers",
            column: "OrganizationEntityId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_TeamInviteEntity_Teams_TeamId",
            table: "TeamInviteEntity",
            column: "TeamId",
            principalTable: "Teams",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_TeamMemberEntity_Teams_TeamId",
            table: "TeamMemberEntity",
            column: "TeamId",
            principalTable: "Teams",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Teams_Organizations_OrganizationEntityId",
            table: "Teams",
            column: "OrganizationEntityId",
            principalTable: "Organizations",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Teams_Users_OwnerId",
            table: "Teams",
            column: "OwnerId",
            principalTable: "Users",
            principalColumn: "Id");
    }
}
