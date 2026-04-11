using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addamzinventoryCostTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AAmzInventoryCost",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    MarketPlace = table.Column<string>(maxLength: 3, nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Asin = table.Column<string>(maxLength: 20, nullable: false),
                    FBACost = table.Column<double>(nullable: false),
                    AWDCost = table.Column<double>(nullable: false),
                    OnTheWayCost = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AAmzInventoryCost", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AAmzInventoryCost");
        }
    }
}
