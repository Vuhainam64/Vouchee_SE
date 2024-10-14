using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixVoucherAndAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Voucher",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "AddressName",
                table: "Address",
                newName: "Address_name");

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 16, 18, 50, 21, 915, DateTimeKind.Local).AddTicks(7425), new DateTime(2024, 10, 12, 18, 50, 21, 915, DateTimeKind.Local).AddTicks(7425) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 16, 18, 50, 21, 915, DateTimeKind.Local).AddTicks(7407), new DateTime(2024, 10, 12, 18, 50, 21, 915, DateTimeKind.Local).AddTicks(7398) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Voucher",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Address_name",
                table: "Address",
                newName: "AddressName");

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 16, 16, 44, 55, 640, DateTimeKind.Local).AddTicks(900), new DateTime(2024, 10, 12, 16, 44, 55, 640, DateTimeKind.Local).AddTicks(899) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 16, 16, 44, 55, 640, DateTimeKind.Local).AddTicks(885), new DateTime(2024, 10, 12, 16, 44, 55, 640, DateTimeKind.Local).AddTicks(876) });
        }
    }
}
