using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class removeKTTypeFromArrivingFromChina : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerchType",
                table: "arrivingFromChinas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MerchType",
                table: "arrivingFromChinas",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
