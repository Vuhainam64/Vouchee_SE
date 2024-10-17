using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSubVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Voucher",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SellPrice",
                table: "Voucher",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Image",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubVoucherId",
                table: "Image",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubVoucher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    SellPrice = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubVoucher", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "Rating", "SellPrice" },
                values: new object[] { 0, 0m });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "Rating", "SellPrice" },
                values: new object[] { 0, 0m });

            migrationBuilder.CreateIndex(
                name: "IX_Image_SubVoucherId",
                table: "Image",
                column: "SubVoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_SubVoucher_SubVoucherId",
                table: "Image",
                column: "SubVoucherId",
                principalTable: "SubVoucher",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_SubVoucher_SubVoucherId",
                table: "Image");

            migrationBuilder.DropTable(
                name: "SubVoucher");

            migrationBuilder.DropIndex(
                name: "IX_Image_SubVoucherId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "SellPrice",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "SubVoucherId",
                table: "Image");
        }
    }
}
