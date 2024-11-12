using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class Test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransaction_PartnerTransaction_PartnerTransactionId",
                table: "WalletTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnerTransaction",
                table: "PartnerTransaction");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PartnerTransaction");

            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerTransactionId",
                table: "WalletTransaction",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TestId",
                table: "PartnerTransaction",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<string>(
                name: "PartnerName",
                table: "PartnerTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartnerTransactionId",
                table: "PartnerTransaction",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnerTransaction",
                table: "PartnerTransaction",
                column: "TestId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransaction_PartnerTransaction_PartnerTransactionId",
                table: "WalletTransaction",
                column: "PartnerTransactionId",
                principalTable: "PartnerTransaction",
                principalColumn: "TestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransaction_PartnerTransaction_PartnerTransactionId",
                table: "WalletTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartnerTransaction",
                table: "PartnerTransaction");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "PartnerTransaction");

            migrationBuilder.DropColumn(
                name: "PartnerName",
                table: "PartnerTransaction");

            migrationBuilder.DropColumn(
                name: "PartnerTransactionId",
                table: "PartnerTransaction");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerTransactionId",
                table: "WalletTransaction",
                type: "int",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PartnerTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnerTransaction",
                table: "PartnerTransaction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransaction_PartnerTransaction_PartnerTransactionId",
                table: "WalletTransaction",
                column: "PartnerTransactionId",
                principalTable: "PartnerTransaction",
                principalColumn: "Id");
        }
    }
}
