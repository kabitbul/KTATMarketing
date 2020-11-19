using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class updateRefundWithQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Refunds");

            migrationBuilder.AddColumn<int>(
                name: "RefundQuantity",
                table: "Refunds",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefundQuantity",
                table: "Refunds");

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "Refunds",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
