using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Cart_CartId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_CartId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_CartId1",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CartId",
                table: "User");

            migrationBuilder.AddColumn<Guid>(
                name: "CreateBy",
                table: "Cart",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Cart",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Cart",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdateBy",
                table: "Cart",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Cart",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_User_UserId",
                table: "Cart",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_User_UserId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "UpdateBy",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Cart");

            migrationBuilder.AddColumn<Guid>(
                name: "CartId",
                table: "User",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("9e1a13f2-738b-4ae4-994d-26d5272c13fa"),
                column: "CartId",
                value: null);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("b4583f49-baba-4916-8e2b-2d44c3412733"),
                column: "CartId",
                value: null);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("db2d2745-93a8-4cb0-9d04-4de79d58fe43"),
                column: "CartId",
                value: null);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("deee9638-da34-4230-be77-34137aa5fcff"),
                column: "CartId",
                value: null);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("e55ee134-b4ec-43f2-a565-8bcec52dff23"),
                column: "CartId",
                value: null);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("f7618901-65a4-45c1-b23d-4f225ee0c588"),
                column: "CartId",
                value: null);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("fb4fc5a5-4564-4100-8fd2-8d406afa11e7"),
                column: "CartId",
                value: null);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("fe3f8e20-d720-460a-bdaf-486bfa813eb1"),
                column: "CartId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_User_CartId",
                table: "User",
                column: "CartId",
                unique: true,
                filter: "[CartId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_CartId1",
                table: "User",
                column: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Cart_CartId",
                table: "User",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id");
        }
    }
}
