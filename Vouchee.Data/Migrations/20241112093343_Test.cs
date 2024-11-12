using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voucher_User_SellerID",
                table: "Voucher");

            migrationBuilder.RenameColumn(
                name: "SellerID",
                table: "Voucher",
                newName: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Voucher_User_SellerId",
                table: "Voucher",
                column: "SellerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voucher_User_SellerId",
                table: "Voucher");

            migrationBuilder.RenameColumn(
                name: "SellerId",
                table: "Voucher",
                newName: "SellerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Voucher_User_SellerID",
                table: "Voucher",
                column: "SellerID",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
