using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class updateNameOfAdminApprovalOnProductsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminApproved",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "AdminApproval",
                table: "Products",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminApproval",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "AdminApproved",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
