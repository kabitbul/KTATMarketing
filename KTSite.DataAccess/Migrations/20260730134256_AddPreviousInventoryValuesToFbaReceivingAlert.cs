using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTSite.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPreviousInventoryValuesToFbaReceivingAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PreviousAvailableQty",
                table: "AAmzFBAReceivingAlerts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreviousInboundReceivingQty",
                table: "AAmzFBAReceivingAlerts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreviousInboundShippedQty",
                table: "AAmzFBAReceivingAlerts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreviousReservedQty",
                table: "AAmzFBAReceivingAlerts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousAvailableQty",
                table: "AAmzFBAReceivingAlerts");

            migrationBuilder.DropColumn(
                name: "PreviousInboundReceivingQty",
                table: "AAmzFBAReceivingAlerts");

            migrationBuilder.DropColumn(
                name: "PreviousInboundShippedQty",
                table: "AAmzFBAReceivingAlerts");

            migrationBuilder.DropColumn(
                name: "PreviousReservedQty",
                table: "AAmzFBAReceivingAlerts");
        }
    }
}
