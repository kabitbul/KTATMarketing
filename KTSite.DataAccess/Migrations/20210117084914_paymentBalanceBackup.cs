using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class paymentBalanceBackup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentBalanceBackups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserNameId = table.Column<string>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    IsWarehouseBalance = table.Column<bool>(nullable: false),
                    AllowNegativeBalance = table.Column<bool>(nullable: false),
                    BackupDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentBalanceBackups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentBalanceBackups_AspNetUsers_UserNameId",
                        column: x => x.UserNameId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentBalanceBackups_UserNameId",
                table: "PaymentBalanceBackups",
                column: "UserNameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentBalanceBackups");
        }
    }
}
