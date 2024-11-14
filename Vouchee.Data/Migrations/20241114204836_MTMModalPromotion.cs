using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class MTMModalPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modal_ModalPromotion_ModalPromotionId",
                table: "Modal");

            migrationBuilder.DropIndex(
                name: "IX_Modal_ModalPromotionId",
                table: "Modal");

            migrationBuilder.DropColumn(
                name: "ModalPromotionId",
                table: "Modal");

            migrationBuilder.CreateTable(
                name: "ModalModalPromotion",
                columns: table => new
                {
                    ModalPromotionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModalsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModalModalPromotion", x => new { x.ModalPromotionsId, x.ModalsId });
                    table.ForeignKey(
                        name: "FK_ModalModalPromotion_ModalPromotion_ModalPromotionsId",
                        column: x => x.ModalPromotionsId,
                        principalTable: "ModalPromotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModalModalPromotion_Modal_ModalsId",
                        column: x => x.ModalsId,
                        principalTable: "Modal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModalModalPromotion_ModalsId",
                table: "ModalModalPromotion",
                column: "ModalsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModalModalPromotion");

            migrationBuilder.AddColumn<Guid>(
                name: "ModalPromotionId",
                table: "Modal",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modal_ModalPromotionId",
                table: "Modal",
                column: "ModalPromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modal_ModalPromotion_ModalPromotionId",
                table: "Modal",
                column: "ModalPromotionId",
                principalTable: "ModalPromotion",
                principalColumn: "Id");
        }
    }
}
