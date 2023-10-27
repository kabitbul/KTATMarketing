using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addRestockColumnToAsinToSku : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Restock",
                table: "AsinToSku",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Restock",
                table: "AsinToSku");
        }
    }
}
