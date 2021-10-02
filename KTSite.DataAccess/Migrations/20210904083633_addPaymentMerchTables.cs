using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addPaymentMerchTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MerchType",
                table: "PaymentBalances",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchId",
                table: "arrivingFromChinas",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchType",
                table: "arrivingFromChinas",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentBalancesMerch",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserNameId = table.Column<string>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    MerchType = table.Column<string>(maxLength: 10, nullable: true),
                    paymentAddress = table.Column<string>(maxLength: 100, nullable: true),
                    paymentType = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentBalancesMerch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentBalancesMerch_AspNetUsers_UserNameId",
                        column: x => x.UserNameId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentHistoriesMerch",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SentFromAddress = table.Column<int>(nullable: false),
                    UserNameId = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    PayDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistoriesMerch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentHistoriesMerch_AspNetUsers_UserNameId",
                        column: x => x.UserNameId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentBalancesMerch_UserNameId",
                table: "PaymentBalancesMerch",
                column: "UserNameId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistoriesMerch_UserNameId",
                table: "PaymentHistoriesMerch",
                column: "UserNameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentBalancesMerch");

            migrationBuilder.DropTable(
                name: "PaymentHistoriesMerch");

            migrationBuilder.DropColumn(
                name: "MerchType",
                table: "PaymentBalances");

            migrationBuilder.DropColumn(
                name: "MerchId",
                table: "arrivingFromChinas");

            migrationBuilder.DropColumn(
                name: "MerchType",
                table: "arrivingFromChinas");
        }
    }
}
