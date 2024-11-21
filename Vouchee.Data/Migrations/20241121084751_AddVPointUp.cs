using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVPointUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Modal_ModalId",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Order_OrderId",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rating",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Rating_OrderId",
                table: "Rating");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModalId",
                table: "Rating",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "Rating",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Rating",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<Guid>(
                name: "RatingId",
                table: "Media",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rating",
                table: "Rating",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_OrderId",
                table: "Rating",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Media_RatingId",
                table: "Media",
                column: "RatingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Rating_RatingId",
                table: "Media",
                column: "RatingId",
                principalTable: "Rating",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Modal_ModalId",
                table: "Rating",
                column: "ModalId",
                principalTable: "Modal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Order_OrderId",
                table: "Rating",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Rating_RatingId",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Modal_ModalId",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Order_OrderId",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rating",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Rating_OrderId",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Media_RatingId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "RatingId",
                table: "Media");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "Rating",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ModalId",
                table: "Rating",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rating",
                table: "Rating",
                columns: new[] { "OrderId", "ModalId" });

            migrationBuilder.CreateIndex(
                name: "IX_Rating_OrderId",
                table: "Rating",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Modal_ModalId",
                table: "Rating",
                column: "ModalId",
                principalTable: "Modal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Order_OrderId",
                table: "Rating",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
