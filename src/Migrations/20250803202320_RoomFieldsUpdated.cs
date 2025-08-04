using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Achilles.Migrations
{
    /// <inheritdoc />
    public partial class RoomFieldsUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomModelId",
                table: "Rooms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomModelId",
                table: "Rooms");
        }
    }
}
