using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addLitalAmazonTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LitalAmazonOrders",
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
                    table.PrimaryKey("PK_LitalAmazonOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LitalAsinToSku",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Asin = table.Column<string>(maxLength: 15, nullable: false),
                    ChinaName = table.Column<string>(nullable: true),
                    Restock = table.Column<bool>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    RestockNOTDECIDED = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitalAsinToSku", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LitalInventoryOrdersToAmazon",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductAsin = table.Column<string>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    DateOrdered = table.Column<DateTime>(nullable: false),
                    DateReceived = table.Column<DateTime>(nullable: false),
                    InboundUpdated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitalInventoryOrdersToAmazon", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LitalAmazonOrders");

            migrationBuilder.DropTable(
                name: "LitalAsinToSku");

            migrationBuilder.DropTable(
                name: "LitalInventoryOrdersToAmazon");
        }
    }
}
