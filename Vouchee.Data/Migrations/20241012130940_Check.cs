using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class Check : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressVoucher_Address_AddressesId",
                table: "AddressVoucher");

            migrationBuilder.RenameColumn(
                name: "AddressesId",
                table: "AddressVoucher",
                newName: "AddressId");

            migrationBuilder.AlterColumn<string>(
                name: "Lon",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Lat",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,10)",
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
                values: new object[] { new DateTime(2024, 10, 16, 20, 9, 40, 335, DateTimeKind.Local).AddTicks(7830), new DateTime(2024, 10, 12, 20, 9, 40, 335, DateTimeKind.Local).AddTicks(7830) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 16, 20, 9, 40, 335, DateTimeKind.Local).AddTicks(7816), new DateTime(2024, 10, 12, 20, 9, 40, 335, DateTimeKind.Local).AddTicks(7806) });

            migrationBuilder.AddForeignKey(
                name: "FK_AddressVoucher_Address_AddressId",
                table: "AddressVoucher",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressVoucher_Address_AddressId",
                table: "AddressVoucher");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "AddressVoucher",
                newName: "AddressesId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "Address",
                type: "decimal(18,10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "Address",
                type: "decimal(18,10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
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
                values: new object[] { new DateTime(2024, 10, 16, 18, 50, 21, 915, DateTimeKind.Local).AddTicks(7425), new DateTime(2024, 10, 12, 18, 50, 21, 915, DateTimeKind.Local).AddTicks(7425) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 16, 18, 50, 21, 915, DateTimeKind.Local).AddTicks(7407), new DateTime(2024, 10, 12, 18, 50, 21, 915, DateTimeKind.Local).AddTicks(7398) });

            migrationBuilder.AddForeignKey(
                name: "FK_AddressVoucher_Address_AddressesId",
                table: "AddressVoucher",
                column: "AddressesId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
