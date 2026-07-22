using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTSite.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class LimitAAmzOrdersMarketPlaceLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
                name: "MarketPlace",
                table: "AAmzOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldNullable: true);
        }
    }
}
