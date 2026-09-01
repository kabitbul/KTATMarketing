using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KTSite.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addStrongProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStrongAsin",
                table: "AAmzAsinToSku",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStrongAsin",
                table: "AAmzAsinToSku");
        }
    }
}
