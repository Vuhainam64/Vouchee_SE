using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSalePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "Voucher");

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 18, 18, 37, 23, 281, DateTimeKind.Local).AddTicks(3811), new DateTime(2024, 10, 14, 18, 37, 23, 281, DateTimeKind.Local).AddTicks(3810) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 18, 18, 37, 23, 281, DateTimeKind.Local).AddTicks(3789), new DateTime(2024, 10, 14, 18, 37, 23, 281, DateTimeKind.Local).AddTicks(3775) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                table: "Voucher",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "SalePrice", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 21, 41, 47, 336, DateTimeKind.Local).AddTicks(9322), 0m, new DateTime(2024, 10, 13, 21, 41, 47, 336, DateTimeKind.Local).AddTicks(9321) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "SalePrice", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 21, 41, 47, 336, DateTimeKind.Local).AddTicks(9306), 0m, new DateTime(2024, 10, 13, 21, 41, 47, 336, DateTimeKind.Local).AddTicks(9297) });
        }
    }
}
