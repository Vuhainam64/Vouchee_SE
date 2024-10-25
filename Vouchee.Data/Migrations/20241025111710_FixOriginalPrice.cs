using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixOriginalPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SellPrice",
                table: "Modal",
                type: "decimal(18,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalPrice",
                table: "Modal",
                type: "decimal(18,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,5)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SellPrice",
                table: "Modal",
                type: "decimal(10,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OriginalPrice",
                table: "Modal",
                type: "decimal(10,5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,5)");
        }
    }
}
