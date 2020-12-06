using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class columnsToOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreName",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserNameToShow",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StoreName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserNameToShow",
                table: "Orders");
        }
    }
}
