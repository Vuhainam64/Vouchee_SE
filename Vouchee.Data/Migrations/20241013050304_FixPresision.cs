using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixPresision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Address",
                type: "decimal(38,10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,38)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Address",
                type: "decimal(38,10)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,38)");

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 12, 3, 3, 809, DateTimeKind.Local).AddTicks(909), new DateTime(2024, 10, 13, 12, 3, 3, 809, DateTimeKind.Local).AddTicks(909) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 12, 3, 3, 809, DateTimeKind.Local).AddTicks(894), new DateTime(2024, 10, 13, 12, 3, 3, 809, DateTimeKind.Local).AddTicks(885) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Address",
                type: "decimal(38,38)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Address",
                type: "decimal(38,38)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,10)");

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 53, 15, 222, DateTimeKind.Local).AddTicks(281), new DateTime(2024, 10, 13, 11, 53, 15, 222, DateTimeKind.Local).AddTicks(281) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 53, 15, 222, DateTimeKind.Local).AddTicks(266), new DateTime(2024, 10, 13, 11, 53, 15, 222, DateTimeKind.Local).AddTicks(257) });
        }
    }
}
