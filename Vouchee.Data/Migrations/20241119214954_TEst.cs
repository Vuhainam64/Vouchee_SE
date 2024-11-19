using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class TEst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalQuantitySole",
                table: "Voucher",
                newName: "TotalQuantitySold");

            migrationBuilder.RenameColumn(
                name: "TotalQuantitySole",
                table: "Supplier",
                newName: "TotalQuantitySold");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalQuantitySold",
                table: "Voucher",
                newName: "TotalQuantitySole");

            migrationBuilder.RenameColumn(
                name: "TotalQuantitySold",
                table: "Supplier",
                newName: "TotalQuantitySole");
        }
    }
}
