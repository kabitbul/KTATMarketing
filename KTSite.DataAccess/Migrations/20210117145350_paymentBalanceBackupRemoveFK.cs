using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class paymentBalanceBackupRemoveFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentBalanceBackups_AspNetUsers_UserNameId",
                table: "PaymentBalanceBackups");

            migrationBuilder.DropIndex(
                name: "IX_PaymentBalanceBackups_UserNameId",
                table: "PaymentBalanceBackups");

            migrationBuilder.AlterColumn<string>(
                name: "UserNameId",
                table: "PaymentBalanceBackups",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserNameId",
                table: "PaymentBalanceBackups",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_PaymentBalanceBackups_UserNameId",
                table: "PaymentBalanceBackups",
                column: "UserNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentBalanceBackups_AspNetUsers_UserNameId",
                table: "PaymentBalanceBackups",
                column: "UserNameId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
