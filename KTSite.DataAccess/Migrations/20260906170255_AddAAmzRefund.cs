using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTSite.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAAmzRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AAmzRefunds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Marketplace = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RefundId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Asin = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    SaleDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    NetRefundLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AAmzRefunds", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AAmzRefunds");
        }
    }
}
