using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class _11PartnerTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_PartnerTransactionId",
                table: "WalletTransaction");

            migrationBuilder.DropIndex(
                name: "IX_Order_PartnerTransactionId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransaction_PartnerTransactionId",
                table: "WalletTransaction",
                column: "PartnerTransactionId",
                unique: true,
                filter: "[PartnerTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PartnerTransactionId",
                table: "Order",
                column: "PartnerTransactionId",
                unique: true,
                filter: "[PartnerTransactionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_PartnerTransactionId",
                table: "WalletTransaction");

            migrationBuilder.DropIndex(
                name: "IX_Order_PartnerTransactionId",
                table: "Order");

            migrationBuilder.AddColumn<int>(
                name: "DiscountPrice",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalPrice",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransaction_PartnerTransactionId",
                table: "WalletTransaction",
                column: "PartnerTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PartnerTransactionId",
                table: "Order",
                column: "PartnerTransactionId");
        }
    }
}
