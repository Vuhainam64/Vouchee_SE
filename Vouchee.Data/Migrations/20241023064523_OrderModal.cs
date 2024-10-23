using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderModal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Voucher_VoucherId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherCode_Voucher_VoucherId",
                table: "VoucherCode");

            migrationBuilder.RenameColumn(
                name: "VoucherId",
                table: "VoucherCode",
                newName: "ModalId");

            migrationBuilder.RenameIndex(
                name: "IX_Voucher_VoucherId",
                table: "VoucherCode",
                newName: "IX_Voucher_ModalId");

            migrationBuilder.RenameColumn(
                name: "VoucherId",
                table: "OrderDetail",
                newName: "ModalId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetail_VoucherId",
                table: "OrderDetail",
                newName: "IX_OrderDetail_ModalId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Modal_ModalId",
                table: "OrderDetail",
                column: "ModalId",
                principalTable: "Modal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherCode_Modal_ModalId",
                table: "VoucherCode",
                column: "ModalId",
                principalTable: "Modal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Modal_ModalId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherCode_Modal_ModalId",
                table: "VoucherCode");

            migrationBuilder.RenameColumn(
                name: "ModalId",
                table: "VoucherCode",
                newName: "VoucherId");

            migrationBuilder.RenameIndex(
                name: "IX_Voucher_ModalId",
                table: "VoucherCode",
                newName: "IX_Voucher_VoucherId");

            migrationBuilder.RenameColumn(
                name: "ModalId",
                table: "OrderDetail",
                newName: "VoucherId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetail_ModalId",
                table: "OrderDetail",
                newName: "IX_OrderDetail_VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Voucher_VoucherId",
                table: "OrderDetail",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherCode_Voucher_VoucherId",
                table: "VoucherCode",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
