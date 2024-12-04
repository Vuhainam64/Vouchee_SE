﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCartAfterBuyer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_User_BuyerId",
                table: "Cart");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_User_BuyerId",
                table: "Cart",
                column: "BuyerId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_User_BuyerId",
                table: "Cart");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_User_BuyerId",
                table: "Cart",
                column: "BuyerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
