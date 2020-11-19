using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class alterColumnStoreToStoreId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreName",
                table: "SellersInventories");

            migrationBuilder.AddColumn<int>(
                name: "StoreNameId",
                table: "SellersInventories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreNameId",
                table: "SellersInventories");

            migrationBuilder.AddColumn<string>(
                name: "StoreName",
                table: "SellersInventories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
