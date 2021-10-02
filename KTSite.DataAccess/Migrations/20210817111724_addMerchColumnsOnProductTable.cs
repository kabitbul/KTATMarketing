using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addMerchColumnsOnProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminApproved",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchId",
                table: "Products",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchType",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminApproved",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MerchId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MerchType",
                table: "Products");
        }
    }
}
