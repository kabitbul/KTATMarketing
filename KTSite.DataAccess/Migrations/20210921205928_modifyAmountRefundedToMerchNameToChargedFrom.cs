using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class modifyAmountRefundedToMerchNameToChargedFrom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountRefundedToMerch",
                table: "Refunds");

            migrationBuilder.AddColumn<double>(
                name: "AmountChargedFromMerch",
                table: "Refunds",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountChargedFromMerch",
                table: "Refunds");

            migrationBuilder.AddColumn<double>(
                name: "AmountRefundedToMerch",
                table: "Refunds",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
