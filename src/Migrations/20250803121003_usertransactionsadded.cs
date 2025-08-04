using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Achilles.Migrations
{
    /// <inheritdoc />
    public partial class UserTransactionsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Time = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    CreditValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    RealValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionSystemName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTransactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTransactions_Date",
                table: "UserTransactions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_UserTransactions_Time",
                table: "UserTransactions",
                column: "Time");

            migrationBuilder.CreateIndex(
                name: "IX_UserTransactions_TransactionSystemName",
                table: "UserTransactions",
                column: "TransactionSystemName");

            migrationBuilder.CreateIndex(
                name: "IX_UserTransactions_UserId",
                table: "UserTransactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTransactions");
        }
    }
}
