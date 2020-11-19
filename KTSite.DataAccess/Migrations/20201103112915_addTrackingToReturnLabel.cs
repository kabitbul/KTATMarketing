using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addTrackingToReturnLabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReturnDelivered",
                table: "returnLabels",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReturnTracking",
                table: "returnLabels",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnDelivered",
                table: "returnLabels");

            migrationBuilder.DropColumn(
                name: "ReturnTracking",
                table: "returnLabels");
        }
    }
}
