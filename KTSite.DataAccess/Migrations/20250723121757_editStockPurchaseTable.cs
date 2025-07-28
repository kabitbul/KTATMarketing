using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class editStockPurchaseTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductSku",
                table: "AAmzStockPurchase");

            migrationBuilder.AddColumn<string>(
                name: "ProductChinaName",
                table: "AAmzStockPurchase",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductChinaName",
                table: "AAmzStockPurchase");

            migrationBuilder.AddColumn<string>(
                name: "ProductSku",
                table: "AAmzStockPurchase",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
