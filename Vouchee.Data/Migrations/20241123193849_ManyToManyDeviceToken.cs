using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyDeviceToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeviceToken_User_UserId",
                table: "DeviceToken");

            migrationBuilder.DropIndex(
                name: "IX_DeviceToken_UserId",
                table: "DeviceToken");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DeviceToken");

            migrationBuilder.CreateTable(
                name: "DeviceTokenUser",
                columns: table => new
                {
                    DeviceTokensId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTokenUser", x => new { x.DeviceTokensId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_DeviceTokenUser_DeviceToken_DeviceTokensId",
                        column: x => x.DeviceTokensId,
                        principalTable: "DeviceToken",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceTokenUser_User_UsersId",
                        column: x => x.UsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokenUser_UsersId",
                table: "DeviceTokenUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceTokenUser");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "DeviceToken",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DeviceToken_UserId",
                table: "DeviceToken",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeviceToken_User_UserId",
                table: "DeviceToken",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
