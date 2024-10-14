using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixNamingAddress2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address_name",
                table: "Address",
                newName: "AddressName");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddressName",
                table: "Address",
                newName: "Address_name");

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 52, 4, 416, DateTimeKind.Local).AddTicks(7024), new DateTime(2024, 10, 13, 11, 52, 4, 416, DateTimeKind.Local).AddTicks(7024) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 11, 52, 4, 416, DateTimeKind.Local).AddTicks(7008), new DateTime(2024, 10, 13, 11, 52, 4, 416, DateTimeKind.Local).AddTicks(7000) });
        }
    }
}
