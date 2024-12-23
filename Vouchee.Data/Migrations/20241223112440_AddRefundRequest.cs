using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRefundRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RefundRequestId",
                table: "WalletTransaction",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateStatus",
                table: "VoucherCode",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RefundRequestId",
                table: "Media",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RefundRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lon = table.Column<decimal>(type: "decimal(38,20)", nullable: false),
                    Lat = table.Column<decimal>(type: "decimal(38,20)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefundRequest_VoucherCode_VoucherCodeId",
                        column: x => x.VoucherCodeId,
                        principalTable: "VoucherCode",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransaction_RefundRequestId",
                table: "WalletTransaction",
                column: "RefundRequestId",
                unique: true,
                filter: "[RefundRequestId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Media_RefundRequestId",
                table: "Media",
                column: "RefundRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequest_VoucherCodeId",
                table: "RefundRequest",
                column: "VoucherCodeId",
                unique: true,
                filter: "[VoucherCodeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_RefundRequest_RefundRequestId",
                table: "Media",
                column: "RefundRequestId",
                principalTable: "RefundRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransaction_RefundRequest_RefundRequestId",
                table: "WalletTransaction",
                column: "RefundRequestId",
                principalTable: "RefundRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_RefundRequest_RefundRequestId",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransaction_RefundRequest_RefundRequestId",
                table: "WalletTransaction");

            migrationBuilder.DropTable(
                name: "RefundRequest");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransaction_RefundRequestId",
                table: "WalletTransaction");

            migrationBuilder.DropIndex(
                name: "IX_Media_RefundRequestId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "RefundRequestId",
                table: "WalletTransaction");

            migrationBuilder.DropColumn(
                name: "UpdateStatus",
                table: "VoucherCode");

            migrationBuilder.DropColumn(
                name: "RefundRequestId",
                table: "Media");
        }
    }
}
