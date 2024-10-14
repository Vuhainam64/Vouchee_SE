using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MoneyDiscount",
                table: "Promotion",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentDiscount",
                table: "Promotion",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 18, 18, 51, 7, 797, DateTimeKind.Local).AddTicks(3805), new DateTime(2024, 10, 14, 18, 51, 7, 797, DateTimeKind.Local).AddTicks(3804) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 18, 18, 51, 7, 797, DateTimeKind.Local).AddTicks(3789), new DateTime(2024, 10, 14, 18, 51, 7, 797, DateTimeKind.Local).AddTicks(3780) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoneyDiscount",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "PercentDiscount",
                table: "Promotion");

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
    }
}
