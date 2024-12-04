using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVoucherCodeAfterModal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherCode_Modal_ModalId",
                table: "VoucherCode");

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
                name: "FK_VoucherCode_Modal_ModalId",
                table: "VoucherCode");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherCode_Modal_ModalId",
                table: "VoucherCode",
                column: "ModalId",
                principalTable: "Modal",
                principalColumn: "Id");
        }
    }
}
