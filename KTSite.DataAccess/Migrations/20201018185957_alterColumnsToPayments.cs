using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class alterColumnsToPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedPayment",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "SentFromAddress",
                table: "PaymentHistories");

            migrationBuilder.AddColumn<int>(
                name: "SentFromAddressId",
                table: "PaymentHistories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PaymentHistories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentFromAddressId",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PaymentHistories");

            migrationBuilder.AddColumn<bool>(
                name: "ApprovedPayment",
                table: "PaymentHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SentFromAddress",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
