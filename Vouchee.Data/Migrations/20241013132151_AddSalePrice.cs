using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSalePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Voucher",
                newName: "SalePrice");

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Voucher",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "OriginalPrice", "SalePrice", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 20, 21, 51, 28, DateTimeKind.Local).AddTicks(5044), 100000m, 0m, new DateTime(2024, 10, 13, 20, 21, 51, 28, DateTimeKind.Local).AddTicks(5043) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "OriginalPrice", "SalePrice", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 20, 21, 51, 28, DateTimeKind.Local).AddTicks(5028), 100000m, 0m, new DateTime(2024, 10, 13, 20, 21, 51, 28, DateTimeKind.Local).AddTicks(5018) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Voucher");

            migrationBuilder.RenameColumn(
                name: "SalePrice",
                table: "Voucher",
                newName: "Price");

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "Price", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 18, 50, 4, 611, DateTimeKind.Local).AddTicks(9865), 100000m, new DateTime(2024, 10, 13, 18, 50, 4, 611, DateTimeKind.Local).AddTicks(9864) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "Price", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 18, 50, 4, 611, DateTimeKind.Local).AddTicks(9849), 100000m, new DateTime(2024, 10, 13, 18, 50, 4, 611, DateTimeKind.Local).AddTicks(9840) });
        }
    }
}
