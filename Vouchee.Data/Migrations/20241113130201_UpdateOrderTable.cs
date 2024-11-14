using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PointUp",
                table: "Order",
                newName: "UsedVPoint");

            migrationBuilder.RenameColumn(
                name: "PointDown",
                table: "Order",
                newName: "UsedBalance");

            migrationBuilder.AddColumn<string>(
                name: "GiftEmail",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerTransactionId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_PartnerTransactionId",
                table: "Order",
                column: "PartnerTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PartnerTransaction_PartnerTransactionId",
                table: "Order",
                column: "PartnerTransactionId",
                principalTable: "PartnerTransaction",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_PartnerTransaction_PartnerTransactionId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_PartnerTransactionId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "GiftEmail",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "PartnerTransactionId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "UsedVPoint",
                table: "Order",
                newName: "PointUp");

            migrationBuilder.RenameColumn(
                name: "UsedBalance",
                table: "Order",
                newName: "PointDown");
        }
    }
}
