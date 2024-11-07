using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameNoti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_FromUserId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_ToUserId",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "Notification",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "FromUserId",
                table: "Notification",
                newName: "ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_ToUserId",
                table: "Notification",
                newName: "IX_Notification_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_FromUserId",
                table: "Notification",
                newName: "IX_Notification_ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_ReceiverId",
                table: "Notification",
                column: "ReceiverId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_SenderId",
                table: "Notification",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_ReceiverId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_SenderId",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Notification",
                newName: "ToUserId");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Notification",
                newName: "FromUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_SenderId",
                table: "Notification",
                newName: "IX_Notification_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_ReceiverId",
                table: "Notification",
                newName: "IX_Notification_FromUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_FromUserId",
                table: "Notification",
                column: "FromUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_ToUserId",
                table: "Notification",
                column: "ToUserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
