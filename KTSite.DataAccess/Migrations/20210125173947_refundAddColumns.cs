using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class refundAddColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Cost",
                table: "Refunds",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Refunds",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreNameId",
                table: "Refunds",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserNameId",
                table: "Refunds",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "StoreNameId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "UserNameId",
                table: "Refunds");
        }
    }
}
