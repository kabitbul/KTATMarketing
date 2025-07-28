using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addNewTAbles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AAmzAWDInventory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    MarketPlace = table.Column<string>(maxLength: 3, nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false),
                    Asin = table.Column<string>(maxLength: 20, nullable: false),
                    totalInboundQuantity = table.Column<int>(nullable: false),
                    totalOnhandQuantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AAmzAWDInventory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AAmzFBAInventory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    MarketPlace = table.Column<string>(maxLength: 3, nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false),
                    Asin = table.Column<string>(maxLength: 20, nullable: false),
                    AvailableQty = table.Column<int>(nullable: false),
                    InboundShippedQty = table.Column<int>(nullable: false),
                    InboundReceivingQty = table.Column<int>(nullable: false),
                    ReservedQty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AAmzFBAInventory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AAmzOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmazonOrdId = table.Column<string>(maxLength: 20, nullable: false),
                    MarketPlace = table.Column<string>(nullable: true),
                    PurchaseDate = table.Column<DateTime>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    Asin = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AAmzOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AAmzStockPurchase",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    MarketPlace = table.Column<string>(maxLength: 3, nullable: false),
                    ProductAsin = table.Column<string>(nullable: false),
                    ProductSku = table.Column<string>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    DateOrdered = table.Column<DateTime>(nullable: false),
                    DateReceived = table.Column<DateTime>(nullable: false),
                    InboundUpdated = table.Column<bool>(nullable: false),
                    lineNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AAmzStockPurchase", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AAmzAWDInventory");

            migrationBuilder.DropTable(
                name: "AAmzFBAInventory");

            migrationBuilder.DropTable(
                name: "AAmzOrders");

            migrationBuilder.DropTable(
                name: "AAmzStockPurchase");
        }
    }
}
