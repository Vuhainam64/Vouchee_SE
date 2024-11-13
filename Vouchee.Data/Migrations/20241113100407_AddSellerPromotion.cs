using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSellerPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SellerId",
                table: "Promotion",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotion_SellerId",
                table: "Promotion",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotion_User_SellerId",
                table: "Promotion",
                column: "SellerId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotion_User_SellerId",
                table: "Promotion");

            migrationBuilder.DropIndex(
                name: "IX_Promotion_SellerId",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Promotion");
        }
    }
}
