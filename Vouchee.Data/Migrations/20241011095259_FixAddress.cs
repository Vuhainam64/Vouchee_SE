using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Address",
                newName: "AddressName");

            migrationBuilder.AddColumn<decimal>(
                name: "Lat",
                table: "Address",
                type: "decimal(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Lon",
                table: "Address",
                type: "decimal(18,0)",
                nullable: true);

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
                values: new object[] { new DateTime(2024, 10, 15, 16, 52, 59, 223, DateTimeKind.Local).AddTicks(3588), new DateTime(2024, 10, 11, 16, 52, 59, 223, DateTimeKind.Local).AddTicks(3588) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 15, 16, 52, 59, 223, DateTimeKind.Local).AddTicks(3573), new DateTime(2024, 10, 11, 16, 52, 59, 223, DateTimeKind.Local).AddTicks(3563) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Lon",
                table: "Address");

            migrationBuilder.RenameColumn(
                name: "AddressName",
                table: "Address",
                newName: "Title");

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 15, 16, 37, 26, 792, DateTimeKind.Local).AddTicks(2406), new DateTime(2024, 10, 11, 16, 37, 26, 792, DateTimeKind.Local).AddTicks(2405) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 15, 16, 37, 26, 792, DateTimeKind.Local).AddTicks(2391), new DateTime(2024, 10, 11, 16, 37, 26, 792, DateTimeKind.Local).AddTicks(2381) });
        }
    }
}
