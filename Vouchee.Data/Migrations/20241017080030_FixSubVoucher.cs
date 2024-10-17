using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSubVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VoucherId",
                table: "SubVoucher",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubVoucher_VoucherId",
                table: "SubVoucher",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubVoucher_Voucher_VoucherId",
                table: "SubVoucher",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubVoucher_Voucher_VoucherId",
                table: "SubVoucher");

            migrationBuilder.DropIndex(
                name: "IX_SubVoucher_VoucherId",
                table: "SubVoucher");

            migrationBuilder.DropColumn(
                name: "VoucherId",
                table: "SubVoucher");
        }
    }
}
