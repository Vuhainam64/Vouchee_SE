using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixPartnerTransaction : Migration
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

            migrationBuilder.RenameColumn(
                name: "TransactionContent",
                table: "PartnerTransaction",
                newName: "ReferenceCode");

            migrationBuilder.RenameColumn(
                name: "ReferenceNumber",
                table: "PartnerTransaction",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "PartnerTransaction",
                newName: "Content");

            migrationBuilder.AlterColumn<int>(
                name: "PartnerTransactionId",
                table: "WalletTransaction",
                type: "int",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartnerTransactionId",
                table: "PartnerTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartnerTransaction",
                table: "PartnerTransaction",
                column: "PartnerTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransaction_PartnerTransaction_PartnerTransactionId",
                table: "WalletTransaction",
                column: "PartnerTransactionId",
                principalTable: "PartnerTransaction",
                principalColumn: "PartnerTransactionId");
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
                name: "PartnerTransactionId",
                table: "PartnerTransaction");

            migrationBuilder.RenameColumn(
                name: "ReferenceCode",
                table: "PartnerTransaction",
                newName: "TransactionContent");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "PartnerTransaction",
                newName: "ReferenceNumber");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "PartnerTransaction",
                newName: "Body");

            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerTransactionId",
                table: "WalletTransaction",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PartnerTransaction",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

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
