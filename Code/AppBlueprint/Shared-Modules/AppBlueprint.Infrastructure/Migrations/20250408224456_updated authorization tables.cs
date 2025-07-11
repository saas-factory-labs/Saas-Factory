using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AppBlueprint.SharedKernel.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAuthorizationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Cities_CityId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiLogs_SessionEntity_SessionEntityId",
                table: "ApiLogs");

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
                name: "FK_EmailAddresses_ContactPersons_ContactPersonId",
                table: "EmailAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailAddresses_Users_UserId",
                table: "EmailAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailInviteEntity_Users_UserEntityId",
                table: "EmailInviteEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationEntity_Users_UserEntityId",
                table: "EmailVerificationEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Roles_RoleEntityId",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_PhoneNumbers_ContactPersons_ContactPersonId",
                table: "PhoneNumbers");

            migrationBuilder.DropForeignKey(
                name: "FK_StateEntity_Countries_CountryId",
                table: "StateEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_Streets_Cities_CityId",
                table: "Streets");

            migrationBuilder.DropForeignKey(
                name: "FK_Streets_Cities_CountryId",
                table: "Streets");

            migrationBuilder.DropTable(
                name: "GlobalRegionEntity");

            migrationBuilder.DropIndex(
                name: "IX_PhoneNumbers_ContactPersonId",
                table: "PhoneNumbers");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_RoleEntityId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_EmailAddresses_ContactPersonId",
                table: "EmailAddresses");

            migrationBuilder.DropIndex(
                name: "IX_EmailAddresses_UserId",
                table: "EmailAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionEntity",
                table: "SessionEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailVerificationEntity",
                table: "EmailVerificationEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailInviteEntity",
                table: "EmailInviteEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cities",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ContactPersonId",
                table: "PhoneNumbers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "RoleEntityId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ContactPersonId",
                table: "EmailAddresses");

            migrationBuilder.RenameTable(
                name: "SessionEntity",
                newName: "Sessions");

            migrationBuilder.RenameTable(
                name: "EmailVerificationEntity",
                newName: "EmailVerifications");

            migrationBuilder.RenameTable(
                name: "EmailInviteEntity",
                newName: "EmailInvites");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "Countrys");

            migrationBuilder.RenameTable(
                name: "Cities",
                newName: "Citys");

            migrationBuilder.RenameIndex(
                name: "IX_EmailVerificationEntity_UserEntityId",
                table: "EmailVerifications",
                newName: "IX_EmailVerifications_UserEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_EmailInviteEntity_UserEntityId",
                table: "EmailInvites",
                newName: "IX_EmailInvites_UserEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Countries_GlobalRegionId",
                table: "Countrys",
                newName: "IX_Countrys_GlobalRegionId");

            migrationBuilder.RenameIndex(
                name: "IX_Cities_StateId",
                table: "Citys",
                newName: "IX_Citys_StateId");

            migrationBuilder.RenameIndex(
                name: "IX_Cities_CountryId",
                table: "Citys",
                newName: "IX_Citys_CountryId");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Subscriptions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Streets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "PhoneNumbers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "PhoneNumbers",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PhoneNumbers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Permissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "EmailAddresses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "ContactPersons",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "ContactPersons",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "SessionKey",
                table: "Sessions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "EmailVerifications",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "EmailInvites",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Countrys",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Citys",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Citys",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AddColumn<int>(
                name: "CountryEntityId",
                table: "Citys",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailVerifications",
                table: "EmailVerifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailInvites",
                table: "EmailInvites",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countrys",
                table: "Countrys",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Citys",
                table: "Citys",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CountryRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CountryRegions_Countrys_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countrys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlobalRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalRegions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourcePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId1 = table.Column<int>(type: "integer", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourcePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourcePermissions_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourcePermissionTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourcePermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourcePermissionTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourcePermissionTypes_ResourcePermissions_ResourcePermiss~",
                        column: x => x.ResourcePermissionId,
                        principalTable: "ResourcePermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumbers_Id",
                table: "PhoneNumbers",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId",
                table: "Permissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPersons_Id",
                table: "ContactPersons",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countrys_Name",
                table: "Countrys",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Citys_CountryEntityId",
                table: "Citys",
                column: "CountryEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryRegions_CountryId",
                table: "CountryRegions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePermissions_UserId1",
                table: "ResourcePermissions",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ResourcePermissionTypes_ResourcePermissionId",
                table: "ResourcePermissionTypes",
                column: "ResourcePermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Citys_CityId",
                table: "Addresses",
                column: "CityId",
                principalTable: "Citys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Countrys_CountryId",
                table: "Addresses",
                column: "CountryId",
                principalTable: "Countrys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApiLogs_Sessions_SessionEntityId",
                table: "ApiLogs",
                column: "SessionEntityId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Citys_Countrys_CountryEntityId",
                table: "Citys",
                column: "CountryEntityId",
                principalTable: "Countrys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Citys_Countrys_CountryId",
                table: "Citys",
                column: "CountryId",
                principalTable: "Countrys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Citys_StateEntity_StateId",
                table: "Citys",
                column: "StateId",
                principalTable: "StateEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Countrys_GlobalRegions_GlobalRegionId",
                table: "Countrys",
                column: "GlobalRegionId",
                principalTable: "GlobalRegions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailAddresses_ContactPersons_Id",
                table: "EmailAddresses",
                column: "Id",
                principalTable: "ContactPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailAddresses_Users_Id",
                table: "EmailAddresses",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailInvites_Users_UserEntityId",
                table: "EmailInvites",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerifications_Users_UserEntityId",
                table: "EmailVerifications",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_RoleId",
                table: "Permissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneNumbers_ContactPersons_Id",
                table: "PhoneNumbers",
                column: "Id",
                principalTable: "ContactPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StateEntity_Countrys_CountryId",
                table: "StateEntity",
                column: "CountryId",
                principalTable: "Countrys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Streets_Citys_CityId",
                table: "Streets",
                column: "CityId",
                principalTable: "Citys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Streets_Citys_CountryId",
                table: "Streets",
                column: "CountryId",
                principalTable: "Citys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Citys_CityId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Countrys_CountryId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ApiLogs_Sessions_SessionEntityId",
                table: "ApiLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Citys_Countrys_CountryEntityId",
                table: "Citys");

            migrationBuilder.DropForeignKey(
                name: "FK_Citys_Countrys_CountryId",
                table: "Citys");

            migrationBuilder.DropForeignKey(
                name: "FK_Citys_StateEntity_StateId",
                table: "Citys");

            migrationBuilder.DropForeignKey(
                name: "FK_Countrys_GlobalRegions_GlobalRegionId",
                table: "Countrys");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailAddresses_ContactPersons_Id",
                table: "EmailAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailAddresses_Users_Id",
                table: "EmailAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailInvites_Users_UserEntityId",
                table: "EmailInvites");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerifications_Users_UserEntityId",
                table: "EmailVerifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Roles_RoleId",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_PhoneNumbers_ContactPersons_Id",
                table: "PhoneNumbers");

            migrationBuilder.DropForeignKey(
                name: "FK_StateEntity_Countrys_CountryId",
                table: "StateEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_Streets_Citys_CityId",
                table: "Streets");

            migrationBuilder.DropForeignKey(
                name: "FK_Streets_Citys_CountryId",
                table: "Streets");

            migrationBuilder.DropTable(
                name: "CountryRegions");

            migrationBuilder.DropTable(
                name: "GlobalRegions");

            migrationBuilder.DropTable(
                name: "PaymentProviders");

            migrationBuilder.DropTable(
                name: "ResourcePermissionTypes");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "ResourcePermissions");

            migrationBuilder.DropIndex(
                name: "IX_Users_Id",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_PhoneNumbers_Id",
                table: "PhoneNumbers");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_RoleId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_ContactPersons_Id",
                table: "ContactPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailVerifications",
                table: "EmailVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailInvites",
                table: "EmailInvites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countrys",
                table: "Countrys");

            migrationBuilder.DropIndex(
                name: "IX_Countrys_Name",
                table: "Countrys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Citys",
                table: "Citys");

            migrationBuilder.DropIndex(
                name: "IX_Citys_CountryEntityId",
                table: "Citys");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CountryEntityId",
                table: "Citys");

            migrationBuilder.RenameTable(
                name: "Sessions",
                newName: "SessionEntity");

            migrationBuilder.RenameTable(
                name: "EmailVerifications",
                newName: "EmailVerificationEntity");

            migrationBuilder.RenameTable(
                name: "EmailInvites",
                newName: "EmailInviteEntity");

            migrationBuilder.RenameTable(
                name: "Countrys",
                newName: "Countries");

            migrationBuilder.RenameTable(
                name: "Citys",
                newName: "Cities");

            migrationBuilder.RenameIndex(
                name: "IX_EmailVerifications_UserEntityId",
                table: "EmailVerificationEntity",
                newName: "IX_EmailVerificationEntity_UserEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_EmailInvites_UserEntityId",
                table: "EmailInviteEntity",
                newName: "IX_EmailInviteEntity_UserEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Countrys_GlobalRegionId",
                table: "Countries",
                newName: "IX_Countries_GlobalRegionId");

            migrationBuilder.RenameIndex(
                name: "IX_Citys_StateId",
                table: "Cities",
                newName: "IX_Cities_StateId");

            migrationBuilder.RenameIndex(
                name: "IX_Citys_CountryId",
                table: "Cities",
                newName: "IX_Cities_CountryId");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Subscriptions",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Streets",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "PhoneNumbers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                table: "PhoneNumbers",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PhoneNumbers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "ContactPersonId",
                table: "PhoneNumbers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Permissions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Permissions",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Permissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Permissions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RoleEntityId",
                table: "Permissions",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "EmailAddresses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "ContactPersonId",
                table: "EmailAddresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "ContactPersons",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "ContactPersons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SessionKey",
                table: "SessionEntity",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "EmailVerificationEntity",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "EmailInviteEntity",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Countries",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Cities",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cities",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionEntity",
                table: "SessionEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailVerificationEntity",
                table: "EmailVerificationEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailInviteEntity",
                table: "EmailInviteEntity",
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
                name: "GlobalRegionEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalRegionEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumbers_ContactPersonId",
                table: "PhoneNumbers",
                column: "ContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleEntityId",
                table: "Permissions",
                column: "RoleEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAddresses_ContactPersonId",
                table: "EmailAddresses",
                column: "ContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAddresses_UserId",
                table: "EmailAddresses",
                column: "UserId");

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
                name: "FK_ApiLogs_SessionEntity_SessionEntityId",
                table: "ApiLogs",
                column: "SessionEntityId",
                principalTable: "SessionEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_EmailAddresses_ContactPersons_ContactPersonId",
                table: "EmailAddresses",
                column: "ContactPersonId",
                principalTable: "ContactPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailAddresses_Users_UserId",
                table: "EmailAddresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailInviteEntity_Users_UserEntityId",
                table: "EmailInviteEntity",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationEntity_Users_UserEntityId",
                table: "EmailVerificationEntity",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_RoleEntityId",
                table: "Permissions",
                column: "RoleEntityId",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhoneNumbers_ContactPersons_ContactPersonId",
                table: "PhoneNumbers",
                column: "ContactPersonId",
                principalTable: "ContactPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
        }
    }
}
