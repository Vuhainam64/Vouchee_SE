using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixLonLat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Address",
                type: "decimal(38,38)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Address",
                type: "decimal(38,38)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

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
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 34, 26, 871, DateTimeKind.Local).AddTicks(7360), new DateTime(2024, 10, 13, 10, 34, 26, 871, DateTimeKind.Local).AddTicks(7359) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 17, 10, 34, 26, 871, DateTimeKind.Local).AddTicks(7343), new DateTime(2024, 10, 13, 10, 34, 26, 871, DateTimeKind.Local).AddTicks(7326) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Lon",
                table: "Address",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,38)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Lat",
                table: "Address",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,38)",
                oldNullable: true);

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
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 16, 20, 31, 39, 87, DateTimeKind.Local).AddTicks(8893), new DateTime(2024, 10, 12, 20, 31, 39, 87, DateTimeKind.Local).AddTicks(8892) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 16, 20, 31, 39, 87, DateTimeKind.Local).AddTicks(8872), new DateTime(2024, 10, 12, 20, 31, 39, 87, DateTimeKind.Local).AddTicks(8863) });
        }
    }
}
