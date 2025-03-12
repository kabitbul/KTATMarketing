using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addLineNumberToInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "lineNumber",
                table: "LitalInventoryOrdersToAmazon",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "lineNumber",
                table: "InventoryOrdersToAmzCA",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "lineNumber",
                table: "InventoryOrdersToAmazons",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lineNumber",
                table: "LitalInventoryOrdersToAmazon");

            migrationBuilder.DropColumn(
                name: "lineNumber",
                table: "InventoryOrdersToAmzCA");

            migrationBuilder.DropColumn(
                name: "lineNumber",
                table: "InventoryOrdersToAmazons");
        }
    }
}
