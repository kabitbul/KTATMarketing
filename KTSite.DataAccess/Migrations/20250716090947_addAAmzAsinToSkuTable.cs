using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addAAmzAsinToSkuTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AAmzAsinToSku",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    Asin = table.Column<string>(maxLength: 15, nullable: false),
                    Sku = table.Column<string>(nullable: true),
                    ChinaName = table.Column<string>(nullable: true),
                    RestockUS = table.Column<bool>(nullable: false),
                    RestockCA = table.Column<bool>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    RestockNOTDECIDED = table.Column<bool>(nullable: false),
                    RestockNOTDECIDEDCA = table.Column<bool>(nullable: false),
                    IsCanadaAsin = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AAmzAsinToSku", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AAmzAsinToSku");
        }
    }
}
