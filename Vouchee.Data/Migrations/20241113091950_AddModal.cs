using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Promotion",
                newName: "Stock");

            migrationBuilder.AddColumn<int>(
                name: "MaxMoneyToDiscount",
                table: "Promotion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinMoneyToAppy",
                table: "Promotion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredQuantity",
                table: "Promotion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ModalPromotion",
                columns: table => new
                {
                    ModalsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PromotionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModalPromotion", x => new { x.ModalsId, x.PromotionsId });
                    table.ForeignKey(
                        name: "FK_ModalPromotion_Modal_ModalsId",
                        column: x => x.ModalsId,
                        principalTable: "Modal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModalPromotion_Promotion_PromotionsId",
                        column: x => x.PromotionsId,
                        principalTable: "Promotion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModalPromotion_PromotionsId",
                table: "ModalPromotion",
                column: "PromotionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModalPromotion");

            migrationBuilder.DropColumn(
                name: "MaxMoneyToDiscount",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "MinMoneyToAppy",
                table: "Promotion");

            migrationBuilder.DropColumn(
                name: "RequiredQuantity",
                table: "Promotion");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Promotion",
                newName: "Quantity");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerTransactionId",
                table: "WalletTransaction",
                type: "int",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PartnerTransaction",
                type: "int",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");
        }
    }
}
