using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModalPromotionAllowNul : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modal_ModalPromotion_ModalPromotionId",
                table: "Modal");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModalPromotionId",
                table: "Modal",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Modal_ModalPromotion_ModalPromotionId",
                table: "Modal",
                column: "ModalPromotionId",
                principalTable: "ModalPromotion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modal_ModalPromotion_ModalPromotionId",
                table: "Modal");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModalPromotionId",
                table: "Modal",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Modal_ModalPromotion_ModalPromotionId",
                table: "Modal",
                column: "ModalPromotionId",
                principalTable: "ModalPromotion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
