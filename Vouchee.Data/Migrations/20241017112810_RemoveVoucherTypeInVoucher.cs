using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVoucherTypeInVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voucher_VoucherType_VoucherTypeId",
                table: "Voucher");

            migrationBuilder.DropIndex(
                name: "IX_Voucher_VoucherTypeId",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "VoucherTypeId",
                table: "Voucher");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VoucherTypeId",
                table: "Voucher",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                column: "VoucherTypeId",
                value: new Guid("3e676315-1a28-4a0b-beb5-eaa5336a108d"));

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                column: "VoucherTypeId",
                value: new Guid("3e676315-1a28-4a0b-beb5-eaa5336a108d"));

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_VoucherTypeId",
                table: "Voucher",
                column: "VoucherTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Voucher_VoucherType_VoucherTypeId",
                table: "Voucher",
                column: "VoucherTypeId",
                principalTable: "VoucherType",
                principalColumn: "Id");
        }
    }
}
