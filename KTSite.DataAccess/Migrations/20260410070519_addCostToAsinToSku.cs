using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addCostToAsinToSku : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CanadianCost",
                table: "AAmzAsinToSku",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Cost",
                table: "AAmzAsinToSku",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanadianCost",
                table: "AAmzAsinToSku");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "AAmzAsinToSku");
        }
    }
}
