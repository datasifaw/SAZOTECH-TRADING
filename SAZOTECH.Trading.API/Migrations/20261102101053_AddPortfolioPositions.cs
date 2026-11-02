using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAZOTECH.Trading.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPortfolioPositions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortfolioPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialInstrumentId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AveragePrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioPositions_FinancialInstruments_FinancialInstrumentId",
                        column: x => x.FinancialInstrumentId,
                        principalTable: "FinancialInstruments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioPositions_FinancialInstrumentId",
                table: "PortfolioPositions",
                column: "FinancialInstrumentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioPositions");
        }
    }
}
