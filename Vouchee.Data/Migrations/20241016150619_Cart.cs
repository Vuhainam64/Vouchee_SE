using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class Cart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CartId",
                table: "User",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartVoucher",
                columns: table => new
                {
                    CartsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VouchersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartVoucher", x => new { x.CartsId, x.VouchersId });
                    table.ForeignKey(
                        name: "FK_CartVoucher_Cart_CartsId",
                        column: x => x.CartsId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartVoucher_Voucher_VouchersId",
                        column: x => x.VouchersId,
                        principalTable: "Voucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserId",
                table: "Cart",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartVoucher_VouchersId",
                table: "CartVoucher",
                column: "VouchersId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Cart_CartId",
                table: "User",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Cart_CartId",
                table: "User");

            migrationBuilder.DropTable(
                name: "CartVoucher");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_User_CartId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_CartId1",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CartId",
                table: "User");
        }
    }
}
