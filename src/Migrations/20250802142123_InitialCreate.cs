using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Achilles.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Birthday = table.Column<string>(type: "TEXT", nullable: false),
                    Motto = table.Column<string>(type: "TEXT", nullable: false),
                    ConsoleMotto = table.Column<string>(type: "TEXT", nullable: false),
                    Gender = table.Column<char>(type: "TEXT", nullable: false),
                    Figure = table.Column<string>(type: "TEXT", nullable: false),
                    PoolFigure = table.Column<string>(type: "TEXT", nullable: false),
                    Credits = table.Column<int>(type: "INTEGER", nullable: false),
                    Film = table.Column<int>(type: "INTEGER", nullable: false),
                    Tickets = table.Column<int>(type: "INTEGER", nullable: false),
                    ClubSubscriptionStart = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClubSubscriptionEnd = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CurrentBadge = table.Column<string>(type: "TEXT", nullable: true),
                    ShowBadge = table.Column<bool>(type: "INTEGER", nullable: false),
                    Badges = table.Column<string>(type: "TEXT", nullable: false),
                    AllowStalking = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowFriendRequests = table.Column<bool>(type: "INTEGER", nullable: false),
                    SoundEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    TutorialFinished = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastOnline = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
