using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class Reverse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryVoucherType");

            migrationBuilder.AddColumn<Guid>(
                name: "VoucherTypeId",
                table: "Category",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 20, 14, 44, 36, 593, DateTimeKind.Local).AddTicks(6124), new DateTime(2024, 10, 16, 14, 44, 36, 593, DateTimeKind.Local).AddTicks(6123) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 20, 14, 44, 36, 593, DateTimeKind.Local).AddTicks(6047), new DateTime(2024, 10, 16, 14, 44, 36, 593, DateTimeKind.Local).AddTicks(6029) });

            migrationBuilder.CreateIndex(
                name: "IX_Category_VoucherTypeId",
                table: "Category",
                column: "VoucherTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_VoucherType_VoucherTypeId",
                table: "Category",
                column: "VoucherTypeId",
                principalTable: "VoucherType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_VoucherType_VoucherTypeId",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_VoucherTypeId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "VoucherTypeId",
                table: "Category");

            migrationBuilder.CreateTable(
                name: "CategoryVoucherType",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherTypesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryVoucherType", x => new { x.CategoriesId, x.VoucherTypesId });
                    table.ForeignKey(
                        name: "FK_CategoryVoucherType_Category_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryVoucherType_VoucherType_VoucherTypesId",
                        column: x => x.VoucherTypesId,
                        principalTable: "VoucherType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 20, 13, 17, 49, 921, DateTimeKind.Local).AddTicks(6272), new DateTime(2024, 10, 16, 13, 17, 49, 921, DateTimeKind.Local).AddTicks(6271) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StartDate" },
                values: new object[] { new DateTime(2024, 10, 20, 13, 17, 49, 921, DateTimeKind.Local).AddTicks(6249), new DateTime(2024, 10, 16, 13, 17, 49, 921, DateTimeKind.Local).AddTicks(6237) });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryVoucherType_VoucherTypesId",
                table: "CategoryVoucherType",
                column: "VoucherTypesId");
        }
    }
}
