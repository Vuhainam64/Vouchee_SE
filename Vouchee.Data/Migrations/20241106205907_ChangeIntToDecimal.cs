using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIntToDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VPoint",
                table: "User",
                type: "decimal(18,5)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PointDown",
                table: "Order",
                type: "decimal(18,5)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PointUp",
                table: "Order",
                type: "decimal(18,5)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VPoint",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PointDown",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PointUp",
                table: "Order");
        }
    }
}
