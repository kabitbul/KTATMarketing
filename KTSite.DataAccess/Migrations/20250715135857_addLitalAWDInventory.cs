using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addLitalAWDInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LitalAmazonAWDInventories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketPlace = table.Column<string>(maxLength: 3, nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false),
                    Asin = table.Column<string>(maxLength: 20, nullable: false),
                    totalInboundQuantity = table.Column<int>(nullable: false),
                    totalOnhandQuantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitalAmazonAWDInventories", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LitalAmazonAWDInventories");
        }
    }
}
