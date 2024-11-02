using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Supplier",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_UserId",
                table: "Supplier",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_UserId1",
                table: "Supplier",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Supplier_User_UserId",
                table: "Supplier",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supplier_User_UserId",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_UserId",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_UserId1",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Supplier");
        }
    }
}
