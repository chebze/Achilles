using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Achilles.Migrations
{
    /// <inheritdoc />
    public partial class UserFieldsUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SSOTicket",
                table: "Users",
                column: "SSOTicket",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SSOTicket",
                table: "Users");
        }
    }
}
