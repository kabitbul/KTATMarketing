using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class paymentMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "paymentAddress",
                table: "PaymentBalancesMerch");

            migrationBuilder.DropColumn(
                name: "paymentType",
                table: "PaymentBalancesMerch");

            migrationBuilder.CreateTable(
                name: "PaymentMethodMerchs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserNameId = table.Column<string>(nullable: false),
                    PaymentTypeAddress = table.Column<string>(maxLength: 100, nullable: false),
                    PaymentType = table.Column<string>(maxLength: 20, nullable: false),
                    PrefferdMethod = table.Column<bool>(nullable: false),
                    MerchType = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodMerchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethodMerchs_AspNetUsers_UserNameId",
                        column: x => x.UserNameId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethodMerchs_UserNameId",
                table: "PaymentMethodMerchs",
                column: "UserNameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentMethodMerchs");

            migrationBuilder.AddColumn<string>(
                name: "paymentAddress",
                table: "PaymentBalancesMerch",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "paymentType",
                table: "PaymentBalancesMerch",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
