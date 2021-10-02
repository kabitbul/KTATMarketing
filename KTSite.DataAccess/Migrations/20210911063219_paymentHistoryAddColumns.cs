using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class paymentHistoryAddColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SentFromAddress",
                table: "PaymentHistoriesMerch",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "PaymentHistoriesMerch",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SentToAddress",
                table: "PaymentHistoriesMerch",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "PaymentHistoriesMerch");

            migrationBuilder.DropColumn(
                name: "SentToAddress",
                table: "PaymentHistoriesMerch");

            migrationBuilder.AlterColumn<int>(
                name: "SentFromAddress",
                table: "PaymentHistoriesMerch",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);
        }
    }
}
