using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyVoucherCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voucher_Category_CategoryId",
                table: "Voucher");

            migrationBuilder.DropIndex(
                name: "IX_Voucher_CategoryId",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Voucher");

            migrationBuilder.CreateTable(
                name: "CategoryVoucher",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VouchersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryVoucher", x => new { x.CategoriesId, x.VouchersId });
                    table.ForeignKey(
                        name: "FK_CategoryVoucher_Category_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryVoucher_Voucher_VouchersId",
                        column: x => x.VouchersId,
                        principalTable: "Voucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 20, 44, 40, 344, DateTimeKind.Local).AddTicks(978), new DateTime(2024, 10, 13, 20, 44, 40, 344, DateTimeKind.Local).AddTicks(977) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 17, 20, 44, 40, 344, DateTimeKind.Local).AddTicks(962), new DateTime(2024, 10, 13, 20, 44, 40, 344, DateTimeKind.Local).AddTicks(954) });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryVoucher_VouchersId",
                table: "CategoryVoucher",
                column: "VouchersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryVoucher");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Voucher",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "CategoryId", "EndDate", "StartDate" },
                values: new object[] { null, new DateTime(2024, 10, 17, 20, 21, 51, 28, DateTimeKind.Local).AddTicks(5044), new DateTime(2024, 10, 13, 20, 21, 51, 28, DateTimeKind.Local).AddTicks(5043) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "CategoryId", "EndDate", "StartDate" },
                values: new object[] { null, new DateTime(2024, 10, 17, 20, 21, 51, 28, DateTimeKind.Local).AddTicks(5028), new DateTime(2024, 10, 13, 20, 21, 51, 28, DateTimeKind.Local).AddTicks(5018) });

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_CategoryId",
                table: "Voucher",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Voucher_Category_CategoryId",
                table: "Voucher",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }
    }
}
