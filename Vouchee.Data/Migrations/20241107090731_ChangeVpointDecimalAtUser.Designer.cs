﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vouchee.Data.Helpers;

#nullable disable

namespace Vouchee.Data.Migrations
{
    [DbContext(typeof(VoucheeContext))]
    [Migration("20241107090731_ChangeVpointDecimalAtUser")]
    partial class ChangeVpointDecimalAtUser
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AddressBrand", b =>
                {
                    b.Property<Guid>("AddressesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BrandsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("AddressesId", "BrandsId");

                    b.HasIndex("BrandsId");

                    b.ToTable("AddressBrand");
                });

            modelBuilder.Entity("CategoryVoucher", b =>
                {
                    b.Property<Guid>("CategoriesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("VouchersId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CategoriesId", "VouchersId");

                    b.HasIndex("VouchersId");

                    b.ToTable("CategoryVoucher");
                });

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

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsVerfied")
                        .HasColumnType("bit");

                    b.Property<decimal?>("Lat")
                        .HasColumnType("decimal(38, 20)");

                    b.Property<decimal?>("Lon")
                        .HasColumnType("decimal(38, 20)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<Guid?>("VerifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("VerifiedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Brand", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsVerfied")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<Guid?>("VerifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("VerifiedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Brand");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Cart", b =>
                {
                    b.Property<Guid?>("BuyerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ModalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("BuyerId", "ModalId");

                    b.HasIndex(new[] { "BuyerId" }, "IX_Cart_BuyerId");

                    b.HasIndex(new[] { "ModalId" }, "IX_Cart_ModalId");

                    b.ToTable("Cart");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<Guid>("VoucherTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "VoucherTypeId" }, "IX_Category_VoucherTypeId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Media", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("VoucherId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "VoucherId" }, "IX_Media_VoucherId");

                    b.ToTable("Media");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Modal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("OriginalPrice")
                        .HasColumnType("int");

                    b.Property<int>("SellPrice")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("date");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<Guid>("VoucherId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "VoucherId" }, "IX_Modal_VoucherId");

                    b.ToTable("Modal");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<int>("DiscountValue")
                        .HasColumnType("int");

                    b.Property<string>("PaymentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PointDown")
                        .HasColumnType("int");

                    b.Property<int>("PointUp")
                        .HasColumnType("int");

                    b.Property<Guid?>("PromotionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalPrice")
                        .HasColumnType("int");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "PromotionId" }, "IX_Order_PromotionId");

                    b.HasIndex(new[] { "CreateBy" }, "IX_Order_UserId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.OrderDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid?>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<int>("DiscountValue")
                        .HasColumnType("int");

                    b.Property<Guid?>("ModalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PromotionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UnitPrice")
                        .HasColumnType("int");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "ModalId" }, "IX_OrderDetail_ModalId");

                    b.HasIndex(new[] { "OrderId" }, "IX_OrderDetail_OrderId");

                    b.HasIndex(new[] { "PromotionId" }, "IX_OrderDetail_PromotionId");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Promotion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("MoneyDiscount")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PercentDiscount")
                        .HasColumnType("int");

                    b.Property<string>("Policy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
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

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Supplier", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Contact")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsVerfied")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("VerifiedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("VerifiedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasFilter("[UserId] IS NOT NULL");

                    b.HasIndex(new[] { "UserId" }, "IX_Supplier_UserId")
                        .HasDatabaseName("IX_Supplier_UserId1");

                    b.ToTable("Supplier");
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

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("District")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HashPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("ResponsibilityScore")
                        .HasColumnType("int");

                    b.Property<Guid?>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<int>("VPoint")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "RoleId" }, "IX_User_RoleId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Voucher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("BrandId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<decimal>("Rating")
                        .HasColumnType("decimal(10,5)");

                    b.Property<Guid>("SellerID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<Guid>("SupplierId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Video")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "BrandId" }, "IX_Voucher_BrandId");

                    b.HasIndex(new[] { "SellerID" }, "IX_Voucher_SellerId");

                    b.HasIndex(new[] { "SupplierId" }, "IX_Voucher_SupplierId");

                    b.ToTable("Voucher");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.VoucherCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ModalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("OrderDetailId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "ModalId" }, "IX_Voucher_ModalId");

                    b.HasIndex(new[] { "OrderDetailId" }, "IX_Voucher_OrderDetailId");

                    b.ToTable("VoucherCode");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.VoucherType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("CreateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UpdateBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdateDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.ToTable("VoucherType");
                });

            modelBuilder.Entity("AddressBrand", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.Address", null)
                        .WithMany()
                        .HasForeignKey("AddressesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vouchee.Data.Models.Entities.Brand", null)
                        .WithMany()
                        .HasForeignKey("BrandsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CategoryVoucher", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vouchee.Data.Models.Entities.Voucher", null)
                        .WithMany()
                        .HasForeignKey("VouchersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
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

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Cart", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.User", "Buyer")
                        .WithMany("Carts")
                        .HasForeignKey("BuyerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vouchee.Data.Models.Entities.Modal", "Modal")
                        .WithMany("Carts")
                        .HasForeignKey("ModalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Buyer");

                    b.Navigation("Modal");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Category", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.VoucherType", "VoucherType")
                        .WithMany("Categories")
                        .HasForeignKey("VoucherTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("VoucherType");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Media", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.Voucher", "Voucher")
                        .WithMany("Medias")
                        .HasForeignKey("VoucherId");

                    b.Navigation("Voucher");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Modal", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.Voucher", "Voucher")
                        .WithMany("Modals")
                        .HasForeignKey("VoucherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Voucher");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Order", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("CreateBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vouchee.Data.Models.Entities.Promotion", "Promotion")
                        .WithMany("Orders")
                        .HasForeignKey("PromotionId");

                    b.Navigation("Promotion");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.OrderDetail", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.Modal", "Modal")
                        .WithMany("OrderDetails")
                        .HasForeignKey("ModalId");

                    b.HasOne("Vouchee.Data.Models.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vouchee.Data.Models.Entities.Promotion", "Promotion")
                        .WithMany("OrderDetails")
                        .HasForeignKey("PromotionId");

                    b.Navigation("Modal");

                    b.Navigation("Order");

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Supplier", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.User", "User")
                        .WithOne("Supplier")
                        .HasForeignKey("Vouchee.Data.Models.Entities.Supplier", "UserId");

                    b.Navigation("User");
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
                    b.HasOne("Vouchee.Data.Models.Entities.Brand", "Brand")
                        .WithMany("Vouchers")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vouchee.Data.Models.Entities.User", "Seller")
                        .WithMany("Vouchers")
                        .HasForeignKey("SellerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vouchee.Data.Models.Entities.Supplier", "Supplier")
                        .WithMany("Vouchers")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Seller");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.VoucherCode", b =>
                {
                    b.HasOne("Vouchee.Data.Models.Entities.Modal", "Modal")
                        .WithMany("VoucherCodes")
                        .HasForeignKey("ModalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vouchee.Data.Models.Entities.OrderDetail", "OrderDetail")
                        .WithMany("VoucherCodes")
                        .HasForeignKey("OrderDetailId");

                    b.Navigation("Modal");

                    b.Navigation("OrderDetail");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Brand", b =>
                {
                    b.Navigation("Vouchers");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Modal", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("OrderDetails");

                    b.Navigation("VoucherCodes");
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
                    b.Navigation("Carts");

                    b.Navigation("Orders");

                    b.Navigation("Supplier");

                    b.Navigation("Vouchers");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.Voucher", b =>
                {
                    b.Navigation("Medias");

                    b.Navigation("Modals");
                });

            modelBuilder.Entity("Vouchee.Data.Models.Entities.VoucherType", b =>
                {
                    b.Navigation("Categories");
                });
#pragma warning restore 612, 618
        }
    }
}
