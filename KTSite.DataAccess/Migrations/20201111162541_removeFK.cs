using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class removeFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellersInventories_Products_ProductId",
                table: "SellersInventories");

            migrationBuilder.DropIndex(
                name: "IX_SellersInventories_ProductId",
                table: "SellersInventories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SellersInventories_ProductId",
                table: "SellersInventories",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellersInventories_Products_ProductId",
                table: "SellersInventories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
