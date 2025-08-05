using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Achilles.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsReadField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "UserMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "UserMessages",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "UserMessages");
        }
    }
}
