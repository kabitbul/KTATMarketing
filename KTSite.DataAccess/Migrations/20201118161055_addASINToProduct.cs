using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addASINToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductASIN",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductURL",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductASIN",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductURL",
                table: "Products");
        }
    }
}
