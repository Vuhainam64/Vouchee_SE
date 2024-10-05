﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vouchee.Data.Helpers;

#nullable disable

namespace Vouchee.Data.Migrations
{
    [DbContext(typeof(VoucheeContext))]
    partial class VoucheeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PromotionVoucher", b =>
                {
                    b.Property<Guid>("PromotionsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("VouchersId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("PromotionsId", "VouchersId");

                    b.HasIndex("VouchersId");

                    b.ToTable("PromotionVoucher");
                });

            modelBuilder.Entity("ShopVoucher", b =>
                {
                    b.Property<Guid>("ShopsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("VouchersId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ShopsId", "VouchersId");

                    b.HasIndex("VouchersId");

                    b.ToTable("ShopVoucher");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid?>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<decimal>("DiscountValue")
                        .HasColumnType("decimal");

                    b.Property<string>("PaymentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("PromotionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "PromotionId" }, "IX_Order_PromotionId");

                    b.HasIndex(new[] { "CreateBy" }, "IX_Order_UserId");

                    b.ToTable("Order");

                    b.HasData(
                        new
                        {
                            Id = new Guid("01aa28e3-4554-48c5-8324-80c4e6abd582"),
                            CreateBy = new Guid("e55ee134-b4ec-43f2-a565-8bcec52dff23"),
                            DiscountValue = 0m,
                            Status = "PENDING",
                            TotalPrice = 10000m
                        },
                        new
                        {
                            Id = new Guid("f87dade4-73ac-4922-a09d-e6efe4f7ac17"),
                            CreateBy = new Guid("e55ee134-b4ec-43f2-a565-8bcec52dff23"),
                            DiscountValue = 0m,
                            Status = "PENDING",
                            TotalPrice = 20000m
                        });
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.OrderDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid?>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<decimal>("DiscountValue")
                        .HasColumnType("decimal");

                    b.Property<Guid?>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PromotionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<Guid?>("VoucherId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "OrderId" }, "IX_OrderDetail_OrderId");

                    b.HasIndex(new[] { "PromotionId" }, "IX_OrderDetail_PromotionId");

                    b.HasIndex(new[] { "VoucherId" }, "IX_OrderDetail_VoucherId");

                    b.ToTable("OrderDetail");

                    b.HasData(
                        new
                        {
                            Id = new Guid("9c79e86d-3c11-422d-9e51-0ef985e87084"),
                            DiscountValue = 0m,
                            OrderId = new Guid("01aa28e3-4554-48c5-8324-80c4e6abd582"),
                            Quantity = 2,
                            Status = "ACTIVE",
                            UnitPrice = 1000m,
                            VoucherId = new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1")
                        },
                        new
                        {
                            Id = new Guid("d03c5b75-25b0-4ec8-98b3-9f80fcb9311a"),
                            DiscountValue = 0m,
                            OrderId = new Guid("01aa28e3-4554-48c5-8324-80c4e6abd582"),
                            Quantity = 1,
                            Status = "ACTIVE",
                            UnitPrice = 1000m,
                            VoucherId = new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1")
                        });
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Promotion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Policy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.ToTable("Promotion");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid?>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.ToTable("Role");

                    b.HasData(
                        new
                        {
                            Id = new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940"),
                            Name = "ADMIN",
                            Status = "ACTIVE"
                        },
                        new
                        {
                            Id = new Guid("015ae3c5-eee9-4f5c-befb-57d41a43d9df"),
                            Name = "STAFF",
                            Status = "ACTIVE"
                        },
                        new
                        {
                            Id = new Guid("ad5f37da-ca48-4dc5-9f4b-963d94b535e6"),
                            Name = "BUYER",
                            Status = "ACTIVE"
                        },
                        new
                        {
                            Id = new Guid("2d80393a-3a3d-495d-8dd7-f9261f85cc8f"),
                            Name = "SELLER",
                            Status = "ACTIVE"
                        });
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Shop", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid?>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Lat")
                        .HasColumnType("decimal");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("PercentShow")
                        .HasColumnType("decimal");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.ToTable("Shop");

                    b.HasData(
                        new
                        {
                            Id = new Guid("58203073-d9c1-41a0-9aac-fd62977c860c"),
                            Description = "Katinat gần phố đi bộ",
                            Lat = 10.7703411m,
                            Name = "Katinat 1",
                            PercentShow = 70m,
                            Status = "ACTIVE",
                            Title = "KATINAT Gất Đẹp"
                        },
                        new
                        {
                            Id = new Guid("7654b4f2-87ad-4146-8116-3e9303cfe84a"),
                            Description = "Katinat gần Hồ Thị Kỷ",
                            Lat = 10.770832m,
                            Name = "Katinat 2",
                            PercentShow = 80m,
                            Status = "ACTIVE",
                            Title = "KATINAT Gất yêu"
                        },
                        new
                        {
                            Id = new Guid("665dcada-509d-4bef-977b-a3ea097c10ec"),
                            Description = "Katinat gần Bến xe",
                            Lat = 10.770832m,
                            Name = "Katinat 3",
                            PercentShow = 80m,
                            Status = "ACTIVE",
                            Title = "KATINAT Zô địch"
                        });
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Supplier", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Contact")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("PercentShow")
                        .HasColumnType("decimal");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.ToTable("Supplier");

                    b.HasData(
                        new
                        {
                            Id = new Guid("a053e9fc-7962-4eaa-8377-91c56c85cda6"),
                            Name = "Katinat",
                            PercentShow = 5m,
                            Status = "ACTIVE"
                        },
                        new
                        {
                            Id = new Guid("2ab13953-4e2f-4233-a1ff-f10434982ee7"),
                            Name = "Katinat",
                            PercentShow = 5m,
                            Status = "ACTIVE"
                        },
                        new
                        {
                            Id = new Guid("fa01b122-47db-4d5d-9c35-9bb6f94c4861"),
                            Name = "Katinat",
                            PercentShow = 5m,
                            Status = "ACTIVE"
                        });
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("District")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("PercentShow")
                        .HasColumnType("decimal");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("ResponsibilityScore")
                        .HasColumnType("int");

                    b.Property<Guid?>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SecretKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "RoleId" }, "IX_User_RoleId");

                    b.ToTable("User");

                    b.HasData(
                        new
                        {
                            Id = new Guid("deee9638-da34-4230-be77-34137aa5fcff"),
                            Email = "advouchee@gmail.com",
                            FirstName = "ADMIN 1",
                            ResponsibilityScore = 0,
                            RoleId = new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940")
                        },
                        new
                        {
                            Id = new Guid("fb4fc5a5-4564-4100-8fd2-8d406afa11e7"),
                            Email = "advouchee2@gmail.com",
                            FirstName = "ADMIN 2",
                            ResponsibilityScore = 0,
                            RoleId = new Guid("ff54acc6-c4e9-4b73-a158-fd640b4b6940")
                        },
                        new
                        {
                            Id = new Guid("fe3f8e20-d720-460a-bdaf-486bfa813eb1"),
                            Email = "stvouchee1@gmail.com",
                            FirstName = "STAFF 1",
                            ResponsibilityScore = 0,
                            RoleId = new Guid("015ae3c5-eee9-4f5c-befb-57d41a43d9df")
                        },
                        new
                        {
                            Id = new Guid("db2d2745-93a8-4cb0-9d04-4de79d58fe43"),
                            Email = "stvouchee2@gmail.com",
                            FirstName = "STAFF 2",
                            ResponsibilityScore = 0,
                            RoleId = new Guid("015ae3c5-eee9-4f5c-befb-57d41a43d9df")
                        },
                        new
                        {
                            Id = new Guid("b4583f49-baba-4916-8e2b-2d44c3412733"),
                            Email = "voucheeseller@gmail.com",
                            FirstName = "SELLER 1",
                            PercentShow = 80m,
                            ResponsibilityScore = 100,
                            RoleId = new Guid("2d80393a-3a3d-495d-8dd7-f9261f85cc8f")
                        },
                        new
                        {
                            Id = new Guid("f7618901-65a4-45c1-b23d-4f225ee0c588"),
                            Email = "sellervouchee2@gmail.com",
                            FirstName = "SELLER 2",
                            PercentShow = 90m,
                            ResponsibilityScore = 70,
                            RoleId = new Guid("2d80393a-3a3d-495d-8dd7-f9261f85cc8f")
                        },
                        new
                        {
                            Id = new Guid("e55ee134-b4ec-43f2-a565-8bcec52dff23"),
                            Email = "buyervouchee1@gmail.com",
                            FirstName = "BUYER 2",
                            ResponsibilityScore = 0,
                            RoleId = new Guid("ad5f37da-ca48-4dc5-9f4b-963d94b535e6")
                        },
                        new
                        {
                            Id = new Guid("9e1a13f2-738b-4ae4-994d-26d5272c13fa"),
                            Email = "buyervouchee2@gmail.com",
                            FirstName = "BUYER 2",
                            ResponsibilityScore = 0,
                            RoleId = new Guid("ad5f37da-ca48-4dc5-9f4b-963d94b535e6")
                        });
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Voucher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid?>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("PercentShow")
                        .HasColumnType("decimal");

                    b.Property<string>("Policy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("StarDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SupplierId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<Guid?>("VoucherTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CreateBy");

                    b.HasIndex(new[] { "VoucherTypeId" }, "IX_Voucher_CreateBy")
                        .HasDatabaseName("IX_Voucher_CreateBy1");

                    b.HasIndex(new[] { "SupplierId" }, "IX_Voucher_SupplierId");

                    b.HasIndex(new[] { "VoucherTypeId" }, "IX_Voucher_VoucherTypeId");

                    b.ToTable("Voucher");

                    b.HasData(
                        new
                        {
                            Id = new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                            CreateBy = new Guid("b4583f49-baba-4916-8e2b-2d44c3412733"),
                            EndDate = new DateTime(2024, 10, 9, 12, 32, 24, 349, DateTimeKind.Local).AddTicks(7621),
                            Name = "Voucher sale",
                            PercentShow = 10m,
                            Price = 100000m,
                            Quantity = 100,
                            StarDate = new DateTime(2024, 10, 5, 12, 32, 24, 349, DateTimeKind.Local).AddTicks(7610),
                            Status = "ACTIVE",
                            SupplierId = new Guid("a053e9fc-7962-4eaa-8377-91c56c85cda6"),
                            VoucherTypeId = new Guid("3e676315-1a28-4a0b-beb5-eaa5336a108d")
                        },
                        new
                        {
                            Id = new Guid("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                            CreateBy = new Guid("b4583f49-baba-4916-8e2b-2d44c3412733"),
                            EndDate = new DateTime(2024, 10, 9, 12, 32, 24, 349, DateTimeKind.Local).AddTicks(7637),
                            Name = "Voucher sale",
                            PercentShow = 10m,
                            Price = 100000m,
                            Quantity = 100,
                            StarDate = new DateTime(2024, 10, 5, 12, 32, 24, 349, DateTimeKind.Local).AddTicks(7636),
                            Status = "ACTIVE",
                            SupplierId = new Guid("a053e9fc-7962-4eaa-8377-91c56c85cda6"),
                            VoucherTypeId = new Guid("3e676315-1a28-4a0b-beb5-eaa5336a108d")
                        });
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.VoucherCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("OrderDetailId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<Guid?>("VoucherId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "OrderDetailId" }, "IX_Voucher_OrderDetailId");

                    b.HasIndex(new[] { "VoucherId" }, "IX_Voucher_VoucherId");

                    b.ToTable("VoucherCode");

                    b.HasData(
                        new
                        {
                            Id = new Guid("5fa2fa86-0211-450b-8aaa-fbf8aabcfd40"),
                            Code = "123",
                            Status = "ACTIVE",
                            VoucherId = new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1")
                        },
                        new
                        {
                            Id = new Guid("152b1e75-8643-4cbd-a4e3-5d73999fc6d3"),
                            Code = "123",
                            OrderDetailId = new Guid("9c79e86d-3c11-422d-9e51-0ef985e87084"),
                            Status = "SOLD",
                            VoucherId = new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1")
                        },
                        new
                        {
                            Id = new Guid("20a45a99-5560-4910-8d80-13f8c217f5e4"),
                            Code = "123",
                            OrderDetailId = new Guid("d03c5b75-25b0-4ec8-98b3-9f80fcb9311a"),
                            Status = "SOLD",
                            VoucherId = new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1")
                        },
                        new
                        {
                            Id = new Guid("5a87a8f5-e447-4ec4-9223-491eaffe672d"),
                            Code = "123",
                            OrderDetailId = new Guid("d03c5b75-25b0-4ec8-98b3-9f80fcb9311a"),
                            Status = "SOLD",
                            VoucherId = new Guid("494b5347-378e-4e2d-9553-6032a42cd8d1")
                        });
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.VoucherType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid?>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("PercentShow")
                        .HasColumnType("decimal");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.ToTable("VoucherType");

                    b.HasData(
                        new
                        {
                            Id = new Guid("3e676315-1a28-4a0b-beb5-eaa5336a108d"),
                            Name = "FOOD",
                            PercentShow = 10m,
                            Status = "ACTIVE"
                        },
                        new
                        {
                            Id = new Guid("4eccaac5-ecea-4876-91ae-fb18ad265a91"),
                            Name = "TRAVEL",
                            PercentShow = 20m,
                            Status = "ACTIVE"
                        },
                        new
                        {
                            Id = new Guid("c34c676b-ba20-466b-8577-5234c8c60bef"),
                            Name = "COSMETIC",
                            PercentShow = 15m,
                            Status = "ACTIVE"
                        });
                });

            modelBuilder.Entity("PromotionVoucher", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.Promotion", null)
                        .WithMany()
                        .HasForeignKey("PromotionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vouchee.Data.Models.Entities.Voucher", null)
                        .WithMany()
                        .HasForeignKey("VouchersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShopVoucher", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.Shop", null)
                        .WithMany()
                        .HasForeignKey("ShopsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vouchee.Data.Models.Entities.Voucher", null)
                        .WithMany()
                        .HasForeignKey("VouchersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Order", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("CreateBy");

                    b.HasOne("Vouchee.Data.Models.Entities.Promotion", "Promotion")
                        .WithMany("Orders")
                        .HasForeignKey("PromotionId");

                    b.Navigation("Promotion");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.OrderDetail", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId");

                    b.HasOne("Vouchee.Data.Models.Entities.Promotion", "Promotion")
                        .WithMany("OrderDetails")
                        .HasForeignKey("PromotionId");

                    b.HasOne("Vouchee.Data.Models.Entities.Voucher", "Voucher")
                        .WithMany("OrderDetails")
                        .HasForeignKey("VoucherId");

                    b.Navigation("Order");

                    b.Navigation("Promotion");

                    b.Navigation("Voucher");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.User", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Voucher", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.User", "Seller")
                        .WithMany("Vouchers")
                        .HasForeignKey("CreateBy");

                    b.HasOne("Vouchee.Data.Models.Entities.Supplier", "Supplier")
                        .WithMany("Vouchers")
                        .HasForeignKey("SupplierId");

                    b.HasOne("Vouchee.Data.Models.Entities.VoucherType", "VoucherType")
                        .WithMany("Vouchers")
                        .HasForeignKey("VoucherTypeId");

                    b.Navigation("Seller");

                    b.Navigation("Supplier");

                    b.Navigation("VoucherType");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.VoucherCode", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.OrderDetail", "OrderDetail")
                        .WithMany("VoucherCodes")
                        .HasForeignKey("OrderDetailId");

                    b.HasOne("Vouchee.Data.Models.Entities.Voucher", "Voucher")
                        .WithMany("VoucherCodes")
                        .HasForeignKey("VoucherId");

                    b.Navigation("OrderDetail");

                    b.Navigation("Voucher");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.OrderDetail", b =>
                {
                    b.Navigation("VoucherCodes");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Promotion", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Supplier", b =>
                {
                    b.Navigation("Vouchers");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.User", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("Vouchers");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Voucher", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("VoucherCodes");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.VoucherType", b =>
                {
                    b.Navigation("Vouchers");
                });
#pragma warning restore 612, 618
        }
    }
}
