using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AppBlueprint.SharedKernel.Migrations
{
    /// <inheritdoc />
    public partial class Smallchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_CityEntity_CityId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_CountryEntity_CountryId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_StreetEntity_StreetId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_CityEntity_CountryEntity_CountryId",
                table: "CityEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_CityEntity_StateEntity_StateId",
                table: "CityEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_CountryEntity_GlobalRegionEntity_GlobalRegionId",
                table: "CountryEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_StateEntity_CountryEntity_CountryId",
                table: "StateEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_StreetEntity_CityEntity_CityId",
                table: "StreetEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_StreetEntity_CityEntity_CountryId",
                table: "StreetEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StreetEntity",
                table: "StreetEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CountryEntity",
                table: "CountryEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CityEntity",
                table: "CityEntity");

            migrationBuilder.RenameTable(
                name: "StreetEntity",
                newName: "Streets");

            migrationBuilder.RenameTable(
                name: "CountryEntity",
                newName: "Countries");

            migrationBuilder.RenameTable(
                name: "CityEntity",
                newName: "Cities");

            migrationBuilder.RenameIndex(
                name: "IX_StreetEntity_CountryId",
                table: "Streets",
                newName: "IX_Streets_CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_StreetEntity_CityId",
                table: "Streets",
                newName: "IX_Streets_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_CountryEntity_GlobalRegionId",
                table: "Countries",
                newName: "IX_Countries_GlobalRegionId");

            migrationBuilder.RenameIndex(
                name: "IX_CityEntity_StateId",
                table: "Cities",
                newName: "IX_Cities_StateId");

            migrationBuilder.RenameIndex(
                name: "IX_CityEntity_CountryId",
                table: "Cities",
                newName: "IX_Cities_CountryId");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationEntityId",
                table: "Teams",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationEntityId",
                table: "Customers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Streets",
                table: "Streets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countries",
                table: "Countries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cities",
                table: "Cities",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Secret = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "IX_Teams_OrganizationEntityId",
                table: "Teams",
                column: "OrganizationEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_OrganizationEntityId",
                table: "Customers",
                column: "OrganizationEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_OwnerId",
                table: "ApiKeys",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Name",
                table: "Organizations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OwnerId",
                table: "Organizations",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Cities_CityId",
                table: "Addresses",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Streets_StreetId",
                table: "Addresses",
                column: "StreetId",
                principalTable: "Streets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_StateEntity_StateId",
                table: "Cities",
                column: "StateId",
                principalTable: "StateEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Countries_GlobalRegionEntity_GlobalRegionId",
                table: "Countries",
                column: "GlobalRegionId",
                principalTable: "GlobalRegionEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Organizations_OrganizationEntityId",
                table: "Customers",
                column: "OrganizationEntityId",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StateEntity_Countries_CountryId",
                table: "StateEntity",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Streets_Cities_CityId",
                table: "Streets",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Streets_Cities_CountryId",
                table: "Streets",
                column: "CountryId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Organizations_OrganizationEntityId",
                table: "Teams",
                column: "OrganizationEntityId",
                principalTable: "Organizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Cities_CityId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Streets_StreetId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_Cities_StateEntity_StateId",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_Countries_GlobalRegionEntity_GlobalRegionId",
                table: "Countries");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Organizations_OrganizationEntityId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_StateEntity_Countries_CountryId",
                table: "StateEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_Streets_Cities_CityId",
                table: "Streets");

            migrationBuilder.DropForeignKey(
                name: "FK_Streets_Cities_CountryId",
                table: "Streets");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Organizations_OrganizationEntityId",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Teams_OrganizationEntityId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Customers_OrganizationEntityId",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Streets",
                table: "Streets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cities",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "OrganizationEntityId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "OrganizationEntityId",
                table: "Customers");

            migrationBuilder.RenameTable(
                name: "Streets",
                newName: "StreetEntity");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "CountryEntity");

            migrationBuilder.RenameTable(
                name: "Cities",
                newName: "CityEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Streets_CountryId",
                table: "StreetEntity",
                newName: "IX_StreetEntity_CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_Streets_CityId",
                table: "StreetEntity",
                newName: "IX_StreetEntity_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_Countries_GlobalRegionId",
                table: "CountryEntity",
                newName: "IX_CountryEntity_GlobalRegionId");

            migrationBuilder.RenameIndex(
                name: "IX_Cities_StateId",
                table: "CityEntity",
                newName: "IX_CityEntity_StateId");

            migrationBuilder.RenameIndex(
                name: "IX_Cities_CountryId",
                table: "CityEntity",
                newName: "IX_CityEntity_CountryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StreetEntity",
                table: "StreetEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CountryEntity",
                table: "CountryEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CityEntity",
                table: "CityEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_CityEntity_CityId",
                table: "Addresses",
                column: "CityId",
                principalTable: "CityEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_CountryEntity_CountryId",
                table: "Addresses",
                column: "CountryId",
                principalTable: "CountryEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_StreetEntity_StreetId",
                table: "Addresses",
                column: "StreetId",
                principalTable: "StreetEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CityEntity_CountryEntity_CountryId",
                table: "CityEntity",
                column: "CountryId",
                principalTable: "CountryEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CityEntity_StateEntity_StateId",
                table: "CityEntity",
                column: "StateId",
                principalTable: "StateEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CountryEntity_GlobalRegionEntity_GlobalRegionId",
                table: "CountryEntity",
                column: "GlobalRegionId",
                principalTable: "GlobalRegionEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StateEntity_CountryEntity_CountryId",
                table: "StateEntity",
                column: "CountryId",
                principalTable: "CountryEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StreetEntity_CityEntity_CityId",
                table: "StreetEntity",
                column: "CityId",
                principalTable: "CityEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StreetEntity_CityEntity_CountryId",
                table: "StreetEntity",
                column: "CountryId",
                principalTable: "CityEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
