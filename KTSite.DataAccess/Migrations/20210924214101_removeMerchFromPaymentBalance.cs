using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class removeMerchFromPaymentBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerchType",
                table: "PaymentBalances");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MerchType",
                table: "PaymentBalances",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
