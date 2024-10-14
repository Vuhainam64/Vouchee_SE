using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Supplier",
                keyColumn: "Id",
                keyValue: new Guid("2ab13953-4e2f-4233-a1ff-f10434982ee7"),
                column: "Image",
                value: null);

            migrationBuilder.UpdateData(
                table: "Supplier",
                keyColumn: "Id",
                keyValue: new Guid("a053e9fc-7962-4eaa-8377-91c56c85cda6"),
                column: "Image",
                value: null);

            migrationBuilder.UpdateData(
                table: "Supplier",
                keyColumn: "Id",
                keyValue: new Guid("fa01b122-47db-4d5d-9c35-9bb6f94c4861"),
                column: "Image",
                value: null);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 15, 23, 14, 36, 451, DateTimeKind.Local).AddTicks(2465), new DateTime(2024, 10, 11, 23, 14, 36, 451, DateTimeKind.Local).AddTicks(2465) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 15, 23, 14, 36, 451, DateTimeKind.Local).AddTicks(2429), new DateTime(2024, 10, 11, 23, 14, 36, 451, DateTimeKind.Local).AddTicks(2420) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Supplier");

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
    }
}
