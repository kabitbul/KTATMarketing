using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addAmazonTablesFirstTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiveItemId",
                table: "InventoriesOnTexas");

            migrationBuilder.CreateTable(
                name: "AmazonOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmazonOrdId = table.Column<string>(nullable: false),
                    MarketPlace = table.Column<string>(nullable: true),
                    PurchaseDate = table.Column<DateTime>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    Asin = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmazonOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AsinToSku",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Asin = table.Column<string>(nullable: false),
                    Sku = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsinToSku", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AmazonOrders");

            migrationBuilder.DropTable(
                name: "AsinToSku");

            migrationBuilder.AddColumn<int>(
                name: "ReceiveItemId",
                table: "InventoriesOnTexas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
