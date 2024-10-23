using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Image_VoucherId",
                table: "Media",
                newName: "IX_Media_VoucherId");

            migrationBuilder.AddColumn<string>(
                name: "Index",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Video",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Modal");

            migrationBuilder.RenameIndex(
                name: "IX_Media_VoucherId",
                table: "Media",
                newName: "IX_Image_VoucherId");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Media",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModalId",
                table: "Media",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Media",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Image_AddressId",
                table: "Media",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Image_ModalId",
                table: "Media",
                column: "ModalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Address_ModalId",
                table: "Media",
                column: "ModalId",
                principalTable: "Address",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Modal_ModalId",
                table: "Media",
                column: "ModalId",
                principalTable: "Modal",
                principalColumn: "Id");
        }
    }
}
