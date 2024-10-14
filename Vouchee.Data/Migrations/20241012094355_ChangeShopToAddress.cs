using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeShopToAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopVoucher");

            migrationBuilder.DropTable(
                name: "Shop");

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AddressName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lon = table.Column<decimal>(type: "decimal(18,10)", nullable: true),
                    Lat = table.Column<decimal>(type: "decimal(18,10)", nullable: true),
                    PercentShow = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AddressVoucher",
                columns: table => new
                {
                    AddressesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VouchersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressVoucher", x => new { x.AddressesId, x.VouchersId });
                    table.ForeignKey(
                        name: "FK_AddressVoucher_Address_AddressesId",
                        column: x => x.AddressesId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AddressVoucher_Voucher_VouchersId",
                        column: x => x.VouchersId,
                        principalTable: "Voucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "Id", "AddressName", "CreateBy", "CreateDate", "Image", "Lat", "Lon", "PercentShow", "Status", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("58203073-d9c1-41a0-9aac-fd62977c860c"), null, null, null, null, null, null, 70m, "ACTIVE", null, null },
                    { new Guid("665dcada-509d-4bef-977b-a3ea097c10ec"), null, null, null, null, null, null, 80m, "ACTIVE", null, null },
                    { new Guid("7654b4f2-87ad-4146-8116-3e9303cfe84a"), null, null, null, null, null, null, 80m, "ACTIVE", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 16, 16, 43, 55, 412, DateTimeKind.Local).AddTicks(5226), new DateTime(2024, 10, 12, 16, 43, 55, 412, DateTimeKind.Local).AddTicks(5225) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 16, 16, 43, 55, 412, DateTimeKind.Local).AddTicks(5211), new DateTime(2024, 10, 12, 16, 43, 55, 412, DateTimeKind.Local).AddTicks(5202) });

            migrationBuilder.CreateIndex(
                name: "IX_AddressVoucher_VouchersId",
                table: "AddressVoucher",
                column: "VouchersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressVoucher");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.CreateTable(
                name: "Shop",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    AddressName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lat = table.Column<decimal>(type: "decimal(18,10)", nullable: true),
                    Lon = table.Column<decimal>(type: "decimal(18,10)", nullable: true),
                    PercentShow = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shop", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopVoucher",
                columns: table => new
                {
                    ShopsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VouchersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopVoucher", x => new { x.ShopsId, x.VouchersId });
                    table.ForeignKey(
                        name: "FK_ShopVoucher_Shop_ShopsId",
                        column: x => x.ShopsId,
                        principalTable: "Shop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopVoucher_Voucher_VouchersId",
                        column: x => x.VouchersId,
                        principalTable: "Voucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Shop",
                columns: new[] { "Id", "AddressName", "CreateBy", "CreateDate", "Image", "Lat", "Lon", "PercentShow", "Status", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("58203073-d9c1-41a0-9aac-fd62977c860c"), null, null, null, null, null, null, 70m, "ACTIVE", null, null },
                    { new Guid("665dcada-509d-4bef-977b-a3ea097c10ec"), null, null, null, null, null, null, 80m, "ACTIVE", null, null },
                    { new Guid("7654b4f2-87ad-4146-8116-3e9303cfe84a"), null, null, null, null, null, null, 80m, "ACTIVE", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 15, 23, 14, 36, 451, DateTimeKind.Local).AddTicks(2465), new DateTime(2024, 10, 11, 23, 14, 36, 451, DateTimeKind.Local).AddTicks(2465) });

            migrationBuilder.UpdateData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                columns: new[] { "EndDate", "StarDate" },
                values: new object[] { new DateTime(2024, 10, 15, 23, 14, 36, 451, DateTimeKind.Local).AddTicks(2429), new DateTime(2024, 10, 11, 23, 14, 36, 451, DateTimeKind.Local).AddTicks(2420) });

            migrationBuilder.CreateIndex(
                name: "IX_ShopVoucher_VouchersId",
                table: "ShopVoucher",
                column: "VouchersId");
        }
    }
}
