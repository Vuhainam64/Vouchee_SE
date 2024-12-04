using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMediaAfterModal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Voucher_VoucherId",
                table: "Media");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Voucher_VoucherId",
                table: "Media",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Voucher_VoucherId",
                table: "Media");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Voucher_VoucherId",
                table: "Media",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id");
        }
    }
}
