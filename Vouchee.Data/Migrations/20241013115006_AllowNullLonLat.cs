using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullLonLat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Address",
                type: "decimal(38,10)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,10)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Address",
                type: "decimal(38,10)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,10)");

            migrationBuilder.UpdateData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("58203073-d9c1-41a0-9aac-fd62977c860c"),
                columns: new[] { "Lat", "Lon" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("665dcada-509d-4bef-977b-a3ea097c10ec"),
                columns: new[] { "Lat", "Lon" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("7654b4f2-87ad-4146-8116-3e9303cfe84a"),
                columns: new[] { "Lat", "Lon" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 18, 50, 4, 611, DateTimeKind.Local).AddTicks(9865), new DateTime(2024, 10, 13, 18, 50, 4, 611, DateTimeKind.Local).AddTicks(9864) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 18, 50, 4, 611, DateTimeKind.Local).AddTicks(9849), new DateTime(2024, 10, 13, 18, 50, 4, 611, DateTimeKind.Local).AddTicks(9840) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Address",
                type: "decimal(38,10)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Address",
                type: "decimal(38,10)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,10)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("58203073-d9c1-41a0-9aac-fd62977c860c"),
                columns: new[] { "Lat", "Lon" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("665dcada-509d-4bef-977b-a3ea097c10ec"),
                columns: new[] { "Lat", "Lon" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Address",
                keyColumn: "Id",
                keyValue: new Guid("7654b4f2-87ad-4146-8116-3e9303cfe84a"),
                columns: new[] { "Lat", "Lon" },
                values: new object[] { 0m, 0m });

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
    }
}
