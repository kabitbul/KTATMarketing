using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addRestockColumnCAAndUSToAsinToSku : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Restock",
                table: "AsinToSku");

            migrationBuilder.AddColumn<bool>(
                name: "RestockCA",
                table: "AsinToSku",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RestockUS",
                table: "AsinToSku",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestockCA",
                table: "AsinToSku");

            migrationBuilder.DropColumn(
                name: "RestockUS",
                table: "AsinToSku");

            migrationBuilder.AddColumn<bool>(
                name: "Restock",
                table: "AsinToSku",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
