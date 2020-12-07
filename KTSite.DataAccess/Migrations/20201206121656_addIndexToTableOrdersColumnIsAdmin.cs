using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addIndexToTableOrdersColumnIsAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
   name: "IX_Order_IsAdmin",
   table: "Orders",
   column: "IsAdmin"
    );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
             name: "IX_Order_IsAdmin",
             table: "Orders"
              );
        }
    }
}
