using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixAccuracy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PercentShow",
                table: "Address",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Address",
                type: "decimal(18,10)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Address",
                type: "decimal(18,10)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 15, 22, 36, 52, 34, DateTimeKind.Local).AddTicks(9852), new DateTime(2024, 10, 11, 22, 36, 52, 34, DateTimeKind.Local).AddTicks(9851) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 15, 22, 36, 52, 34, DateTimeKind.Local).AddTicks(9836), new DateTime(2024, 10, 11, 22, 36, 52, 34, DateTimeKind.Local).AddTicks(9826) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PercentShow",
                table: "Address",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Address",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Address",
                type: "decimal(18,0)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,10)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 15, 16, 52, 59, 223, DateTimeKind.Local).AddTicks(3588), new DateTime(2024, 10, 11, 16, 52, 59, 223, DateTimeKind.Local).AddTicks(3588) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 15, 16, 52, 59, 223, DateTimeKind.Local).AddTicks(3573), new DateTime(2024, 10, 11, 16, 52, 59, 223, DateTimeKind.Local).AddTicks(3563) });
        }
    }
}
