using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSenderNoti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_SenderId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_SenderId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Notification");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Order");

            migrationBuilder.AddColumn<Guid>(
                name: "SenderId",
                table: "Notification",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_SenderId",
                table: "Notification",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_SenderId",
                table: "Notification",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
