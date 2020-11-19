using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: false),
                    UserNameId = table.Column<string>(nullable: true),
                    StoreNameId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    CustName = table.Column<string>(nullable: false),
                    CustStreet1 = table.Column<string>(nullable: true),
                    CustStreet2 = table.Column<string>(nullable: true),
                    CustCity = table.Column<string>(nullable: true),
                    CustState = table.Column<string>(nullable: true),
                    CustZipCode = table.Column<string>(nullable: true),
                    CustPhone = table.Column<string>(nullable: true),
                    Cost = table.Column<double>(nullable: false),
                    Carrier = table.Column<string>(nullable: true),
                    TrackingNumber = table.Column<string>(nullable: true),
                    UsDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_UserStoreNames_StoreNameId",
                        column: x => x.StoreNameId,
                        principalTable: "UserStoreNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserNameId",
                        column: x => x.UserNameId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StoreNameId",
                table: "Orders",
                column: "StoreNameId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserNameId",
                table: "Orders",
                column: "UserNameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
