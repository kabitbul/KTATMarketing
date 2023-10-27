using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class changeLengthInAsinToSku : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Asin",
                table: "AsinToSku",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Asin",
                table: "AsinToSku",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 15);
        }
    }
}
