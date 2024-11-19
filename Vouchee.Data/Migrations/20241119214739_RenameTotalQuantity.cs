using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTotalQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantitySold",
                table: "Voucher",
                newName: "TotalQuantitySole");

            migrationBuilder.RenameColumn(
                name: "QuantitySold",
                table: "Supplier",
                newName: "TotalQuantitySole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalQuantitySole",
                table: "Voucher",
                newName: "QuantitySold");

            migrationBuilder.RenameColumn(
                name: "TotalQuantitySole",
                table: "Supplier",
                newName: "QuantitySold");
        }
    }
}
