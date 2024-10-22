using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVoucherMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Voucher_VoucherId",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Image_VoucherId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "VoucherId",
                table: "Media");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VoucherId",
                table: "Media",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Image_VoucherId",
                table: "Media",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Voucher_VoucherId",
                table: "Media",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id");
        }
    }
}
