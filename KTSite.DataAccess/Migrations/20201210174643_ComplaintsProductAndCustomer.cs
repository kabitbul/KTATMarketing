using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class ComplaintsProductAndCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustName",
                table: "Complaints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "Complaints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustName",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "Complaints");
        }
    }
}
