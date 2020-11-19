using Microsoft.EntityFrameworkCore.Migrations;

namespace KTSite.DataAccess.Migrations
{
    public partial class addPaymentSentAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "PaymentHistories");

            migrationBuilder.AddColumn<string>(
                name: "SentFromAddress",
                table: "PaymentHistories",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentSentAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserNameId = table.Column<string>(nullable: false),
                    PaymentTypeAddress = table.Column<string>(nullable: false),
                    PaymentType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSentAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentSentAddresses_AspNetUsers_UserNameId",
                        column: x => x.UserNameId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSentAddresses_UserNameId",
                table: "PaymentSentAddresses",
                column: "UserNameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentSentAddresses");

            migrationBuilder.DropColumn(
                name: "SentFromAddress",
                table: "PaymentHistories");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
