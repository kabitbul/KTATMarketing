using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTSite.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAAmzFBAReceivingAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AAmzFBAReceivingAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Marketplace = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Asin = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AvailableQty = table.Column<int>(type: "int", nullable: false),
                    InboundShippedQty = table.Column<int>(type: "int", nullable: false),
                    InboundReceivingQty = table.Column<int>(type: "int", nullable: false),
                    ReservedQty = table.Column<int>(type: "int", nullable: false),
                    DetectionReason = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsHandled = table.Column<bool>(type: "bit", nullable: false),
                    HandledDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AAmzFBAReceivingAlerts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AAmzFBAReceivingAlerts_Handled_Date",
                table: "AAmzFBAReceivingAlerts",
                columns: new[] { "IsHandled", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AAmzFBAReceivingAlerts_Store_Marketplace_Asin_Date",
                table: "AAmzFBAReceivingAlerts",
                columns: new[] { "StoreId", "Marketplace", "Asin", "CreatedDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AAmzFBAReceivingAlerts");
        }
    }
}
