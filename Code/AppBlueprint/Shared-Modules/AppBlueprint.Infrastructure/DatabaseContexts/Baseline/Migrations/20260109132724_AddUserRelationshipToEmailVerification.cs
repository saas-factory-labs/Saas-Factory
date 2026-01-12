using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelationshipToEmailVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationEntity_Users_UserEntityId",
                table: "EmailVerificationEntity");

            migrationBuilder.RenameColumn(
                name: "UserEntityId",
                table: "EmailVerificationEntity",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EmailVerificationEntity_UserEntityId",
                table: "EmailVerificationEntity",
                newName: "IX_EmailVerificationEntity_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationEntity_Users_UserId",
                table: "EmailVerificationEntity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailVerificationEntity_Users_UserId",
                table: "EmailVerificationEntity");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "EmailVerificationEntity",
                newName: "UserEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_EmailVerificationEntity_UserId",
                table: "EmailVerificationEntity",
                newName: "IX_EmailVerificationEntity_UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailVerificationEntity_Users_UserEntityId",
                table: "EmailVerificationEntity",
                column: "UserEntityId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
