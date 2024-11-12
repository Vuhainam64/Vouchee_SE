using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPartnerTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PartnerTransactionId",
                table: "WalletTransaction",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PartnerTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Gateway = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AmountIn = table.Column<int>(type: "int", nullable: true),
                    AmountOut = table.Column<int>(type: "int", nullable: true),
                    Accumulated = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerTransaction", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransaction_PartnerTransactionId",
                table: "WalletTransaction",
                column: "PartnerTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransaction_PartnerTransaction_PartnerTransactionId",
                table: "WalletTransaction",
                column: "PartnerTransactionId",
                principalTable: "PartnerTransaction",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransaction_PartnerTransaction_PartnerTransactionId",
                table: "WalletTransaction");

            migrationBuilder.DropTable(
                name: "PartnerTransaction");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_PartnerTransactionId",
                table: "WalletTransaction");

            migrationBuilder.DropColumn(
                name: "PartnerTransactionId",
                table: "WalletTransaction");
        }
    }
}
