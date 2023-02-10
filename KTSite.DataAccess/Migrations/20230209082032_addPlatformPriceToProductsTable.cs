using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addPlatformPriceToProductsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PlatformsPrice",
                table: "Products",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlatformsPrice",
                table: "Products");
        }
    }
}
