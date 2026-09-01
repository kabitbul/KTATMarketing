using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTSite.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class LimitAAmzOrdersAsinLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Asin",
                table: "AAmzOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12,
                oldNullable: true);
        }
    }
}
