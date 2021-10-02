using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addMerchIdToRefundAndComplaints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MerchId",
                table: "returnLabels",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchType",
                table: "returnLabels",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchId",
                table: "Refunds",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchType",
                table: "Refunds",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MerchId",
                table: "Orders",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchId",
                table: "Complaints",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchType",
                table: "Complaints",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerchId",
                table: "returnLabels");

            migrationBuilder.DropColumn(
                name: "MerchType",
                table: "returnLabels");

            migrationBuilder.DropColumn(
                name: "MerchId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "MerchType",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "MerchId",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "MerchType",
                table: "Complaints");

            migrationBuilder.AlterColumn<string>(
                name: "MerchId",
                table: "Orders",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
