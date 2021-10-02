using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addarchiveToPaymentHistoryAndReturningItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentHistoriesArchive",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalId = table.Column<long>(nullable: false),
                    SentFromAddressId = table.Column<int>(nullable: false),
                    UserNameId = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    PayDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistoriesArchive", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReturningItemsArchive",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginalId = table.Column<long>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    ItemStatus = table.Column<string>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    DateArrived = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturningItemsArchive", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentHistoriesArchive");

            migrationBuilder.DropTable(
                name: "ReturningItemsArchive");
        }
    }
}
