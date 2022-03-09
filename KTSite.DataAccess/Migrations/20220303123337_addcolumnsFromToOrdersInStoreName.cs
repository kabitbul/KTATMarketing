using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addcolumnsFromToOrdersInStoreName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FromOrdId",
                table: "UserStoreNames",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ToOrdId",
                table: "UserStoreNames",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromOrdId",
                table: "UserStoreNames");

            migrationBuilder.DropColumn(
                name: "ToOrdId",
                table: "UserStoreNames");
        }
    }
}
