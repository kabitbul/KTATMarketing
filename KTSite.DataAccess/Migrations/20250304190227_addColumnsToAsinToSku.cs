using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addColumnsToAsinToSku : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCanadaAsin",
                table: "AsinToSku",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RestockNOTDECIDED",
                table: "AsinToSku",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCanadaAsin",
                table: "AsinToSku");

            migrationBuilder.DropColumn(
                name: "RestockNOTDECIDED",
                table: "AsinToSku");
        }
    }
}
