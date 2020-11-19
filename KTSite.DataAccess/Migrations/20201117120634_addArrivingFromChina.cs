using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addArrivingFromChina : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "arrivingFromChinas",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: false),
                    NumOfBoxes = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    DateArriving = table.Column<DateTime>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    UpdatedByAdmin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_arrivingFromChinas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "arrivingFromChinas");
        }
    }
}
