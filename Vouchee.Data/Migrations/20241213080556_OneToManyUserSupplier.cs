using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class OneToManyUserSupplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supplier_User_UserId",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_UserId",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Supplier");

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierWalletId",
                table: "WalletTransaction",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "Wallet",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "User",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransaction_SupplierWalletId",
                table: "WalletTransaction",
                column: "SupplierWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_SupplierId",
                table: "Wallet",
                column: "SupplierId",
                unique: true,
                filter: "[SupplierId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_SupplierId",
                table: "User",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Supplier_SupplierId",
                table: "User",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_Supplier_SupplierId",
                table: "Wallet",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransaction_Wallet_SupplierWalletId",
                table: "WalletTransaction",
                column: "SupplierWalletId",
                principalTable: "Wallet",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Supplier_SupplierId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_Supplier_SupplierId",
                table: "Wallet");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransaction_Wallet_SupplierWalletId",
                table: "WalletTransaction");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_SupplierWalletId",
                table: "WalletTransaction");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_SupplierId",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_User_SupplierId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "SupplierWalletId",
                table: "WalletTransaction");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "User");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Supplier",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_UserId",
                table: "Supplier",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Supplier_User_UserId",
                table: "Supplier",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
