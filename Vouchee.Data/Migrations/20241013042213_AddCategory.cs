using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StarDate",
                table: "Voucher",
                newName: "StartDate");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Voucher",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "CategoryId", "EndDate", "StartDate" },
                values: new object[] { null, new DateTime(2024, 10, 17, 11, 22, 12, 899, DateTimeKind.Local).AddTicks(3598), new DateTime(2024, 10, 13, 11, 22, 12, 899, DateTimeKind.Local).AddTicks(3597) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "CategoryId", "EndDate", "StartDate" },
                values: new object[] { null, new DateTime(2024, 10, 17, 11, 22, 12, 899, DateTimeKind.Local).AddTicks(3577), new DateTime(2024, 10, 13, 11, 22, 12, 899, DateTimeKind.Local).AddTicks(3569) });

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_CategoryId",
                table: "Voucher",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Voucher_Category_CategoryId",
                table: "Voucher",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voucher_Category_CategoryId",
                table: "Voucher");

            migrationBuilder.DropIndex(
                name: "IX_Voucher_CategoryId",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Voucher");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Voucher",
                newName: "StarDate");

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
    }
}
