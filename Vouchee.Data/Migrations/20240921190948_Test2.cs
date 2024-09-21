using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vouchee.Data.Migrations
{
    /// <inheritdoc />
    public partial class Test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("d603e285-f00a-406d-ab98-f7a077a99915"));

            migrationBuilder.DropColumn(
                name: "ResponsibilityScore",
                table: "Shop");

            migrationBuilder.AddColumn<decimal>(
                name: "PercentShow",
                table: "Voucher",
                type: "decimal(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentShow",
                table: "User",
                type: "decimal(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResponsibilityScore",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentShow",
                table: "Type",
                type: "decimal(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PercentShow",
                table: "Supplier",
                type: "decimal(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Lat",
                table: "Shop",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "Shop",
                columns: new[] { "Id", "CreateBy", "CreateDate", "Description", "Lat", "Name", "PercentShow", "Status", "Title", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("58203073-d9c1-41a0-9aac-fd62977c860c"), null, null, "Katinat gần phố đi bộ", 10.7703411m, "Katinat 1", 70m, "ACTIVE", "KATINAT Gất Đẹp", null, null },
                    { new Guid("665dcada-509d-4bef-977b-a3ea097c10ec"), null, null, "Katinat gần Bến xe", 10.770832m, "Katinat 3", 80m, "ACTIVE", "KATINAT Zô địch", null, null },
                    { new Guid("7654b4f2-87ad-4146-8116-3e9303cfe84a"), null, null, "Katinat gần Hồ Thị Kỷ", 10.770832m, "Katinat 2", 80m, "ACTIVE", "KATINAT Gất yêu", null, null }
                });

            migrationBuilder.InsertData(
                table: "Supplier",
                columns: new[] { "Id", "Contact", "CreateBy", "CreateDate", "Name", "PercentShow", "Status", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("2ab13953-4e2f-4233-a1ff-f10434982ee7"), null, null, null, "Katinat", 5m, "ACTIVE", null, null },
                    { new Guid("a053e9fc-7962-4eaa-8377-91c56c85cda6"), null, null, null, "Katinat", 5m, "ACTIVE", null, null },
                    { new Guid("fa01b122-47db-4d5d-9c35-9bb6f94c4861"), null, null, null, "Katinat", 5m, "ACTIVE", null, null }
                });

            migrationBuilder.InsertData(
                table: "Type",
                columns: new[] { "Id", "CreateBy", "CreateDate", "Name", "PercentShow", "Status", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("3e676315-1a28-4a0b-beb5-eaa5336a108d"), null, null, "FOOD", 10m, "ACTIVE", null, null },
                    { new Guid("4eccaac5-ecea-4876-91ae-fb18ad265a91"), null, null, "TRAVEL", 20m, "ACTIVE", null, null },
                    { new Guid("c34c676b-ba20-466b-8577-5234c8c60bef"), null, null, "COSMETIC", 15m, "ACTIVE", null, null }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "BankAccount", "BankName", "City", "CreateBy", "CreateDate", "DateOfBirth", "District", "Email", "FirstName", "Gender", "Image", "LastName", "PercentShow", "PhoneNumber", "ResponsibilityScore", "RoleId", "SecretKey", "Status", "UpdateBy", "UpdateDate" },
                values: new object[,]
                {
                    { new Guid("9e1a13f2-738b-4ae4-994d-26d5272c13fa"), null, null, null, null, null, null, null, null, "buyervouchee2@gmail.com", "BUYER 2", null, null, null, null, null, 0, new Guid("ad5f37da-ca48-4dc5-9f4b-963d94b535e6"), null, null, null, null },
                    { new Guid("b4583f49-baba-4916-8e2b-2d44c3412733"), null, null, null, null, null, null, null, null, "sellervouchee1@gmail.com", "SELLER 1", null, null, null, 80m, null, 100, new Guid("2d80393a-3a3d-495d-8dd7-f9261f85cc8f"), null, null, null, null },
                    { new Guid("db2d2745-93a8-4cb0-9d04-4de79d58fe43"), null, null, null, null, null, null, null, null, "stvouchee2@gmail.com", "STAFF 2", null, null, null, null, null, 0, new Guid("015ae3c5-eee9-4f5c-befb-57d41a43d9df"), null, null, null, null },
                    { new Guid("deee9638-da34-4230-be77-34137aa5fcff"), null, null, null, null, null, null, null, null, "advouchee1@gmail.com", "ADMIN 1", null, null, null, null, null, 0, new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940"), null, null, null, null },
                    { new Guid("e55ee134-b4ec-43f2-a565-8bcec52dff23"), null, null, null, null, null, null, null, null, "buyervouchee1@gmail.com", "BUYER 2", null, null, null, null, null, 0, new Guid("ad5f37da-ca48-4dc5-9f4b-963d94b535e6"), null, null, null, null },
                    { new Guid("f7618901-65a4-45c1-b23d-4f225ee0c588"), null, null, null, null, null, null, null, null, "sellervouchee2@gmail.com", "SELLER 2", null, null, null, 90m, null, 70, new Guid("2d80393a-3a3d-495d-8dd7-f9261f85cc8f"), null, null, null, null },
                    { new Guid("fb4fc5a5-4564-4100-8fd2-8d406afa11e7"), null, null, null, null, null, null, null, null, "advouchee2@gmail.com", "ADMIN 2", null, null, null, null, null, 0, new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940"), null, null, null, null },
                    { new Guid("fe3f8e20-d720-460a-bdaf-486bfa813eb1"), null, null, null, null, null, null, null, null, "stvouchee1@gmail.com", "STAFF 1", null, null, null, null, null, 0, new Guid("015ae3c5-eee9-4f5c-befb-57d41a43d9df"), null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "Id", "CreateBy", "CreateDate", "DiscountPrice", "DiscountValue", "FinalPrice", "PaymentType", "Status", "TotalPrice", "UpdateBy", "UpdateDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("01aa28e3-4554-48c5-8324-80c4e6abd582"), null, null, 1000m, 0m, 9000m, null, "PENDING", 10000m, null, null, new Guid("e55ee134-b4ec-43f2-a565-8bcec52dff23") },
                    { new Guid("f87dade4-73ac-4922-a09d-e6efe4f7ac17"), null, null, 1000m, 0m, 19000m, null, "PENDING", 20000m, null, null, new Guid("e55ee134-b4ec-43f2-a565-8bcec52dff23") }
                });

            migrationBuilder.InsertData(
                table: "Voucher",
                columns: new[] { "Id", "CreateBy", "CreateDate", "Description", "EndDate", "Image", "Name", "PercentShow", "Policy", "Price", "Quantity", "StarDate", "Status", "SupplierId", "UpdateBy", "UpdateDate", "VoucherTypeId" },
                values: new object[,]
                {
                    { new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"), new Guid("b4583f49-baba-4916-8e2b-2d44c3412733"), null, null, new DateTime(2024, 9, 26, 2, 9, 48, 214, DateTimeKind.Local).AddTicks(1236), null, "Voucher sale", 10m, null, 100000m, 100, new DateTime(2024, 9, 22, 2, 9, 48, 214, DateTimeKind.Local).AddTicks(1235), "ACTIVE", new Guid("a053e9fc-7962-4eaa-8377-91c56c85cda6"), null, null, new Guid("3e676315-1a28-4a0b-beb5-eaa5336a108d") },
                    { new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"), new Guid("b4583f49-baba-4916-8e2b-2d44c3412733"), null, null, new DateTime(2024, 9, 26, 2, 9, 48, 214, DateTimeKind.Local).AddTicks(1210), null, "Voucher sale", 10m, null, 100000m, 100, new DateTime(2024, 9, 22, 2, 9, 48, 214, DateTimeKind.Local).AddTicks(1197), "ACTIVE", new Guid("a053e9fc-7962-4eaa-8377-91c56c85cda6"), null, null, new Guid("3e676315-1a28-4a0b-beb5-eaa5336a108d") }
                });

            migrationBuilder.InsertData(
                table: "OrderDetail",
                columns: new[] { "Id", "CreateBy", "CreateDate", "DiscountPrice", "DiscountValue", "FinalPrice", "OrderId", "Quantity", "Status", "TotalPrice", "UnitPrice", "UpdateBy", "UpdateDate", "VoucherId" },
                values: new object[,]
                {
                    { new Guid("9c79e86d-3c11-422d-9e51-0ef985e87084"), null, null, 0m, 0m, 4000m, new Guid("01aa28e3-4554-48c5-8324-80c4e6abd582"), 2, "ACTIVE", 2000m, 1000m, null, null, new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1") },
                    { new Guid("d03c5b75-25b0-4ec8-98b3-9f80fcb9311a"), null, null, 0m, 0m, 4000m, new Guid("01aa28e3-4554-48c5-8324-80c4e6abd582"), 1, "ACTIVE", 2000m, 1000m, null, null, new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1") }
                });

            migrationBuilder.InsertData(
                table: "VoucherCode",
                columns: new[] { "Id", "Code", "CreateBy", "CreateDate", "Image", "OrderDetailId", "Status", "UpdateBy", "UpdateDate", "VoucherId" },
                values: new object[,]
                {
                    { new Guid("5fa2fa86-0211-450b-8aaa-fbf8aabcfd40"), "123", null, null, null, null, "ACTIVE", null, null, new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1") },
                    { new Guid("152b1e75-8643-4cbd-a4e3-5d73999fc6d3"), "123", null, null, null, new Guid("9c79e86d-3c11-422d-9e51-0ef985e87084"), "SOLD", null, null, new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1") },
                    { new Guid("20a45a99-5560-4910-8d80-13f8c217f5e4"), "123", null, null, null, new Guid("d03c5b75-25b0-4ec8-98b3-9f80fcb9311a"), "SOLD", null, null, new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1") },
                    { new Guid("5a87a8f5-e447-4ec4-9223-491eaffe672d"), "123", null, null, null, new Guid("d03c5b75-25b0-4ec8-98b3-9f80fcb9311a"), "SOLD", null, null, new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_CreateBy",
                table: "Voucher",
                column: "CreateBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Voucher_User_CreateBy",
                table: "Voucher",
                column: "CreateBy",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voucher_User_CreateBy",
                table: "Voucher");

            migrationBuilder.DropIndex(
                name: "IX_Voucher_CreateBy",
                table: "Voucher");

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "Id",
                keyValue: new Guid("f87dade4-73ac-4922-a09d-e6efe4f7ac17"));

            migrationBuilder.DeleteData(
                table: "Shop",
                keyColumn: "Id",
                keyValue: new Guid("58203073-d9c1-41a0-9aac-fd62977c860c"));

            migrationBuilder.DeleteData(
                table: "Shop",
                keyColumn: "Id",
                keyValue: new Guid("665dcada-509d-4bef-977b-a3ea097c10ec"));

            migrationBuilder.DeleteData(
                table: "Shop",
                keyColumn: "Id",
                keyValue: new Guid("7654b4f2-87ad-4146-8116-3e9303cfe84a"));

            migrationBuilder.DeleteData(
                table: "Supplier",
                keyColumn: "Id",
                keyValue: new Guid("2ab13953-4e2f-4233-a1ff-f10434982ee7"));

            migrationBuilder.DeleteData(
                table: "Supplier",
                keyColumn: "Id",
                keyValue: new Guid("fa01b122-47db-4d5d-9c35-9bb6f94c4861"));

            migrationBuilder.DeleteData(
                table: "Type",
                keyColumn: "Id",
                keyValue: new Guid("4eccaac5-ecea-4876-91ae-fb18ad265a91"));

            migrationBuilder.DeleteData(
                table: "Type",
                keyColumn: "Id",
                keyValue: new Guid("c34c676b-ba20-466b-8577-5234c8c60bef"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("9e1a13f2-738b-4ae4-994d-26d5272c13fa"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("db2d2745-93a8-4cb0-9d04-4de79d58fe43"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("deee9638-da34-4230-be77-34137aa5fcff"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("f7618901-65a4-45c1-b23d-4f225ee0c588"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("fb4fc5a5-4564-4100-8fd2-8d406afa11e7"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("fe3f8e20-d720-460a-bdaf-486bfa813eb1"));

            migrationBuilder.DeleteData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"));

            migrationBuilder.DeleteData(
                table: "VoucherCode",
                keyColumn: "Id",
                keyValue: new Guid("152b1e75-8643-4cbd-a4e3-5d73999fc6d3"));

            migrationBuilder.DeleteData(
                table: "VoucherCode",
                keyColumn: "Id",
                keyValue: new Guid("20a45a99-5560-4910-8d80-13f8c217f5e4"));

            migrationBuilder.DeleteData(
                table: "VoucherCode",
                keyColumn: "Id",
                keyValue: new Guid("5a87a8f5-e447-4ec4-9223-491eaffe672d"));

            migrationBuilder.DeleteData(
                table: "VoucherCode",
                keyColumn: "Id",
                keyValue: new Guid("5fa2fa86-0211-450b-8aaa-fbf8aabcfd40"));

            migrationBuilder.DeleteData(
                table: "OrderDetail",
                keyColumn: "Id",
                keyValue: new Guid("9c79e86d-3c11-422d-9e51-0ef985e87084"));

            migrationBuilder.DeleteData(
                table: "OrderDetail",
                keyColumn: "Id",
                keyValue: new Guid("d03c5b75-25b0-4ec8-98b3-9f80fcb9311a"));

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "Id",
                keyValue: new Guid("01aa28e3-4554-48c5-8324-80c4e6abd582"));

            migrationBuilder.DeleteData(
                table: "Voucher",
                keyColumn: "Id",
                keyValue: new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"));

            migrationBuilder.DeleteData(
                table: "Supplier",
                keyColumn: "Id",
                keyValue: new Guid("a053e9fc-7962-4eaa-8377-91c56c85cda6"));

            migrationBuilder.DeleteData(
                table: "Type",
                keyColumn: "Id",
                keyValue: new Guid("3e676315-1a28-4a0b-beb5-eaa5336a108d"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("b4583f49-baba-4916-8e2b-2d44c3412733"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("e55ee134-b4ec-43f2-a565-8bcec52dff23"));

            migrationBuilder.DropColumn(
                name: "PercentShow",
                table: "Voucher");

            migrationBuilder.DropColumn(
                name: "PercentShow",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ResponsibilityScore",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PercentShow",
                table: "Type");

            migrationBuilder.DropColumn(
                name: "PercentShow",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Shop");

            migrationBuilder.AddColumn<int>(
                name: "ResponsibilityScore",
                table: "Shop",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "BankAccount", "BankName", "City", "CreateBy", "CreateDate", "DateOfBirth", "District", "Email", "FirstName", "Gender", "Image", "LastName", "PhoneNumber", "RoleId", "SecretKey", "Status", "UpdateBy", "UpdateDate" },
                values: new object[] { new Guid("d603e285-f00a-406d-ab98-f7a077a99915"), null, null, null, null, null, null, null, null, "advouchee@gmail.com", "ADMIN", null, null, null, null, new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940"), null, null, null, null });
        }
    }
}
