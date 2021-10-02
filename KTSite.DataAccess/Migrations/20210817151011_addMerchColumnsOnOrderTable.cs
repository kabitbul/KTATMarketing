using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addMerchColumnsOnOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MerchId",
                table: "Orders",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchType",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerchId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MerchType",
                table: "Orders");
        }
    }
}
