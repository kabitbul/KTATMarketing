using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addArchiveTablesOrdersRelated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplaintsArchive",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalId = table.Column<long>(nullable: false),
                    OrderId = table.Column<long>(nullable: true),
                    UserNameId = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    SolutionDesc = table.Column<string>(nullable: true),
                    HandledBy = table.Column<string>(nullable: true),
                    Solved = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    NewTrackingNumber = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    WarehouseResponsibility = table.Column<bool>(nullable: false),
                    StoreId = table.Column<int>(nullable: false),
                    ProductName = table.Column<string>(nullable: true),
                    CustName = table.Column<string>(nullable: true),
                    TicketResolution = table.Column<string>(nullable: true),
                    MerchId = table.Column<string>(maxLength: 100, nullable: true),
                    MerchType = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintsArchive", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplaintsArchive_AspNetUsers_UserNameId",
                        column: x => x.UserNameId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdersArchive",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalId = table.Column<long>(nullable: false),
                    OrderStatus = table.Column<string>(maxLength: 15, nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    UserNameId = table.Column<string>(maxLength: 450, nullable: false),
                    StoreNameId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    CustName = table.Column<string>(nullable: false),
                    CustStreet1 = table.Column<string>(nullable: false),
                    CustStreet2 = table.Column<string>(nullable: true),
                    CustCity = table.Column<string>(nullable: false),
                    CustState = table.Column<string>(nullable: false),
                    CustZipCode = table.Column<string>(nullable: false),
                    CustPhone = table.Column<string>(nullable: true),
                    Cost = table.Column<double>(nullable: false),
                    Carrier = table.Column<string>(nullable: true),
                    TrackingNumber = table.Column<string>(nullable: true),
                    UsDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    ProductName = table.Column<string>(nullable: true),
                    UserNameToShow = table.Column<string>(nullable: true),
                    StoreName = table.Column<string>(nullable: true),
                    TrackingUpdated = table.Column<bool>(nullable: false),
                    MerchId = table.Column<string>(maxLength: 100, nullable: true),
                    MerchType = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersArchive", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefundsArchive",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalId = table.Column<long>(nullable: false),
                    OrderId = table.Column<long>(nullable: false),
                    ReturnId = table.Column<long>(nullable: true),
                    RefundQuantity = table.Column<int>(nullable: false),
                    RefundedBy = table.Column<string>(nullable: true),
                    RefundDate = table.Column<DateTime>(nullable: false),
                    AmountRefunded = table.Column<double>(nullable: false),
                    AmountChargedFromMerch = table.Column<double>(nullable: false),
                    Cost = table.Column<double>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    UserNameId = table.Column<string>(nullable: true),
                    MerchId = table.Column<string>(maxLength: 100, nullable: true),
                    MerchType = table.Column<string>(maxLength: 20, nullable: true),
                    StoreNameId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundsArchive", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "returnLabelsArchive",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalId = table.Column<long>(nullable: false),
                    OrderId = table.Column<long>(nullable: false),
                    UserNameId = table.Column<string>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    FileURL = table.Column<string>(nullable: true),
                    CommentsToWarehouse = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    ReturnTracking = table.Column<string>(nullable: true),
                    ReturnDelivered = table.Column<bool>(nullable: false),
                    ReturnQuantity = table.Column<int>(nullable: false),
                    MerchId = table.Column<string>(maxLength: 100, nullable: true),
                    MerchType = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_returnLabelsArchive", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintsArchive_UserNameId",
                table: "ComplaintsArchive",
                column: "UserNameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplaintsArchive");

            migrationBuilder.DropTable(
                name: "OrdersArchive");

            migrationBuilder.DropTable(
                name: "RefundsArchive");

            migrationBuilder.DropTable(
                name: "returnLabelsArchive");
        }
    }
}
