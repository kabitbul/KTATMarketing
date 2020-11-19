using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addUserStoreName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserStoreNames",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserNameId = table.Column<int>(nullable: false),
                    StoreName = table.Column<string>(nullable: false),
                    IsAdminStore = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStoreNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStoreNames_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStoreNames");
        }
    }
}
