using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Address",
                type: "decimal(38,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Address",
                type: "decimal(38,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,10)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 21, 37, 7, 414, DateTimeKind.Local).AddTicks(3504), new DateTime(2024, 10, 13, 21, 37, 7, 414, DateTimeKind.Local).AddTicks(3503) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 21, 37, 7, 414, DateTimeKind.Local).AddTicks(3479), new DateTime(2024, 10, 13, 21, 37, 7, 414, DateTimeKind.Local).AddTicks(3467) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Address",
                type: "decimal(38,10)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Address",
                type: "decimal(38,10)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,30)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 20, 44, 40, 344, DateTimeKind.Local).AddTicks(978), new DateTime(2024, 10, 13, 20, 44, 40, 344, DateTimeKind.Local).AddTicks(977) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 20, 44, 40, 344, DateTimeKind.Local).AddTicks(962), new DateTime(2024, 10, 13, 20, 44, 40, 344, DateTimeKind.Local).AddTicks(954) });
        }
    }
}
