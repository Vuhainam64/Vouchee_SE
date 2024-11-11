using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_User_UserId",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_UserId",
                table: "Wallet");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Wallet",
                newName: "SellerId");

            migrationBuilder.AddColumn<Guid>(
                name: "BuyerId",
                table: "Wallet",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "User",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "User",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "User",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_User_BuyerId",
                table: "Wallet",
                column: "BuyerId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_User_SellerId",
                table: "Wallet",
                column: "SellerId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_User_BuyerId",
                table: "Wallet");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_User_SellerId",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_BuyerId",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_SellerId",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "Wallet");

            migrationBuilder.RenameColumn(
                name: "SellerId",
                table: "Wallet",
                newName: "UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "User",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "User",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateDate",
                table: "User",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_UserId",
                table: "Wallet",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_User_UserId",
                table: "Wallet",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
