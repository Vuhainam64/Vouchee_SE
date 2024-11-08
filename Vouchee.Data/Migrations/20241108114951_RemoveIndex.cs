using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_OrderId",
                table: "WalletTransaction");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_TopUpRequestId",
                table: "WalletTransaction");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_SellerWalletId",
                table: "WalletTransaction");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_BuyerWalletId",
                table: "WalletTransaction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WalletTransaction_OrderId1",
                table: "WalletTransaction",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransaction_TopUpRequestId1",
                table: "WalletTransaction",
                column: "TopUpRequestId");
        }
    }
}
