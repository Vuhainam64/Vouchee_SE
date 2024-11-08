using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWithdrawRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransaction_AccountTransaction_AccountTransactionId",
                table: "WalletTransaction");

            migrationBuilder.DropTable(
                name: "AccountTransaction");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_AccountTransactionId",
                table: "WalletTransaction");

            migrationBuilder.RenameColumn(
                name: "AccountTransactionId",
                table: "WalletTransaction",
                newName: "TopUpRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransaction_AccountTransactionId1",
                table: "WalletTransaction",
                newName: "IX_WalletTransaction_TopUpRequestId1");

            migrationBuilder.CreateTable(
                name: "TopUpRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopUpRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WithdrawRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithdrawRequest_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransaction_TopUpRequestId",
                table: "WalletTransaction",
                column: "TopUpRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawRequest_UserId",
                table: "WithdrawRequest",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransaction_TopUpRequest_TopUpRequestId",
                table: "WalletTransaction",
                column: "TopUpRequestId",
                principalTable: "TopUpRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransaction_TopUpRequest_TopUpRequestId",
                table: "WalletTransaction");

            migrationBuilder.DropTable(
                name: "TopUpRequest");

            migrationBuilder.DropTable(
                name: "WithdrawRequest");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_TopUpRequestId",
                table: "WalletTransaction");

            migrationBuilder.RenameColumn(
                name: "TopUpRequestId",
                table: "WalletTransaction",
                newName: "AccountTransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransaction_TopUpRequestId1",
                table: "WalletTransaction",
                newName: "IX_WalletTransaction_AccountTransactionId1");

            migrationBuilder.CreateTable(
                name: "AccountTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    FromUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountTransaction_User_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountTransaction_User_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransaction_AccountTransactionId",
                table: "WalletTransaction",
                column: "AccountTransactionId",
                unique: true,
                filter: "[AccountTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_FromUserId",
                table: "AccountTransaction",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransaction_ToUserId",
                table: "AccountTransaction",
                column: "ToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransaction_AccountTransaction_AccountTransactionId",
                table: "WalletTransaction",
                column: "AccountTransactionId",
                principalTable: "AccountTransaction",
                principalColumn: "Id");
        }
    }
}
