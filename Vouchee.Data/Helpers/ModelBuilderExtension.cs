using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Dictionary;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Helpers
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // ROLE
            modelBuilder.Entity<Role>().HasData(
                new Role()
                {
                    Id = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString())),
                    Name = "Admin",
                    Status = ObjectStatusEnum.ACTIVE.ToString()
                },
                new Role()
                {
                    Id = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.STAFF.ToString())),
                    Name = "Staff",
                    Status = ObjectStatusEnum.ACTIVE.ToString()
                },
                new Role()
                {
                    Id = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString())),
                    Name = "Buyer",
                    Status = ObjectStatusEnum.ACTIVE.ToString()
                },
                new Role()
                {
                    Id = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.SELLER.ToString())),
                    Name = "Seller",
                    Status = ObjectStatusEnum.ACTIVE.ToString()
                }
            );

            // USER
            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = Guid.Parse("deee9638-da34-4230-be77-34137aa5fcff"),
                    Email = "advouchee@gmail.com",
                    RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString())),
                    FirstName = "ADMIN 1",
                },
                new User()
                {
                    Id = Guid.Parse("fb4fc5a5-4564-4100-8fd2-8d406afa11e7"),
                    Email = "advouchee2@gmail.com",
                    RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.ADMIN.ToString())),
                    FirstName = "ADMIN 2",
                },
                new User()
                {
                    Id = Guid.Parse("fe3f8e20-d720-460a-bdaf-486bfa813eb1"),
                    Email = "stvouchee1@gmail.com",
                    RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.STAFF.ToString())),
                    FirstName = "STAFF 1"
                },
                new User()
                {
                    Id = Guid.Parse("db2d2745-93a8-4cb0-9d04-4de79d58fe43"),
                    Email = "stvouchee2@gmail.com",
                    RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.STAFF.ToString())),
                    FirstName = "STAFF 2"
                },
                new User()
                {
                    Id = Guid.Parse("b4583f49-baba-4916-8e2b-2d44c3412733"),
                    Email = "voucheeseller@gmail.com",
                    RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.SELLER.ToString())),
                    FirstName = "SELLER 1",
                    PercentShow = 80,
                    ResponsibilityScore = 100
                },
                new User()
                {
                    Id = Guid.Parse("f7618901-65a4-45c1-b23d-4f225ee0c588"),
                    Email = "sellervouchee2@gmail.com",
                    RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.SELLER.ToString())),
                    FirstName = "SELLER 2",
                    PercentShow = 90,
                    ResponsibilityScore = 70
                },
                new User()
                {
                    Id = Guid.Parse("e55ee134-b4ec-43f2-a565-8bcec52dff23"),
                    Email = "buyervouchee1@gmail.com",
                    RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString())),
                    FirstName = "BUYER 2",
                },
                new User()
                {
                    Id = Guid.Parse("9e1a13f2-738b-4ae4-994d-26d5272c13fa"),
                    Email = "buyervouchee2@gmail.com",
                    RoleId = Guid.Parse(RoleDictionary.role.GetValueOrDefault(RoleEnum.BUYER.ToString())),
                    FirstName = "BUYER 2",
                }
            );

            // VOUCHER TYPE
            modelBuilder.Entity<VoucherType>().HasData(
                new VoucherType()
                {
                    Id = Guid.Parse("3e676315-1a28-4a0b-beb5-eaa5336a108d"),
                    Name = "FOOD",
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    PercentShow = 10,
                },
                new VoucherType()
                {
                    Id = Guid.Parse("4eccaac5-ecea-4876-91ae-fb18ad265a91"),
                    Name = "TRAVEL",
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    PercentShow = 20
                },
                new VoucherType()
                {
                    Id = Guid.Parse("c34c676b-ba20-466b-8577-5234c8c60bef"),
                    Name = "COSMETIC",
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    PercentShow = 15
                }
            );

            // SHOP
            modelBuilder.Entity<Shop>().HasData(
                new Shop()
                {
                    Id = Guid.Parse("58203073-d9c1-41a0-9aac-fd62977c860c"),
                    Name = "Katinat 1",
                    Status = ShopStatusEnum.ACTIVE.ToString(),
                    PercentShow = 70,
                    Description = "Katinat gần phố đi bộ",
                    Lat = 10.7703411M,
                    Title = "KATINAT Gất Đẹp",
                },
                new Shop()
                {
                    Id = Guid.Parse("7654b4f2-87ad-4146-8116-3e9303cfe84a"),
                    Name = "Katinat 2",
                    Status = ShopStatusEnum.ACTIVE.ToString(),
                    PercentShow = 80,
                    Description = "Katinat gần Hồ Thị Kỷ",
                    Lat = 10.770832M,
                    Title = "KATINAT Gất yêu",
                },
                new Shop()
                {
                    Id = Guid.Parse("665dcada-509d-4bef-977b-a3ea097c10ec"),
                    Name = "Katinat 3",
                    Status = ShopStatusEnum.ACTIVE.ToString(),
                    PercentShow = 80,
                    Description = "Katinat gần Bến xe",
                    Lat = 10.770832M,
                    Title = "KATINAT Zô địch",
                }
            );

            // SUPPLIER
            modelBuilder.Entity<Supplier>().HasData(
                new Supplier()
                {
                    Id = Guid.Parse("a053e9fc-7962-4eaa-8377-91c56c85cda6"),
                    Name = "Katinat",
                    PercentShow = 5,
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                },
                new Supplier()
                {
                    Id = Guid.Parse("2ab13953-4e2f-4233-a1ff-f10434982ee7"),
                    Name = "Katinat",
                    PercentShow = 5,
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                },
                new Supplier()
                {
                    Id = Guid.Parse("fa01b122-47db-4d5d-9c35-9bb6f94c4861"),
                    Name = "Katinat",
                    PercentShow = 5,
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                }
            );

            // VOUCHER
            modelBuilder.Entity<Voucher>().HasData(
                new Voucher()
                {
                    Id = Guid.Parse("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                    StarDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(4),
                    PercentShow = 10,
                    Price = 100000,
                    Quantity = 100,
                    Name = "Voucher sale",
                    CreateBy = Guid.Parse("b4583f49-baba-4916-8e2b-2d44c3412733"),
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    SupplierId = Guid.Parse("a053e9fc-7962-4eaa-8377-91c56c85cda6"),
                    VoucherTypeId = Guid.Parse("3e676315-1a28-4a0b-beb5-eaa5336a108d")
                },
                new Voucher()
                {
                    Id = Guid.Parse("0c20c3c9-2200-4b09-81f5-a0ceb74eba8c"),
                    StarDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(4),
                    PercentShow = 10,
                    Price = 100000,
                    Quantity = 100,
                    Name = "Voucher sale",
                    CreateBy = Guid.Parse("b4583f49-baba-4916-8e2b-2d44c3412733"),
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    SupplierId = Guid.Parse("a053e9fc-7962-4eaa-8377-91c56c85cda6"),
                    VoucherTypeId = Guid.Parse("3e676315-1a28-4a0b-beb5-eaa5336a108d")
                }
            );

            // VOUCHER CODE
            modelBuilder.Entity<VoucherCode>().HasData(
                new VoucherCode()
                {
                    Id = Guid.Parse("5fa2fa86-0211-450b-8aaa-fbf8aabcfd40"),
                    Code = "123",
                    Status = VoucherCodeStatusEnum.ACTIVE.ToString(),
                    VoucherId = Guid.Parse("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                },
                new VoucherCode()
                {
                    Id = Guid.Parse("152b1e75-8643-4cbd-a4e3-5d73999fc6d3"),
                    Code = "123",
                    Status = VoucherCodeStatusEnum.SOLD.ToString(),
                    VoucherId = Guid.Parse("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                    OrderDetailId = Guid.Parse("9c79e86d-3c11-422d-9e51-0ef985e87084")
                },
                new VoucherCode()
                {
                    Id = Guid.Parse("20a45a99-5560-4910-8d80-13f8c217f5e4"),
                    Code = "123",
                    Status = VoucherCodeStatusEnum.SOLD.ToString(),
                    VoucherId = Guid.Parse("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                    OrderDetailId = Guid.Parse("d03c5b75-25b0-4ec8-98b3-9f80fcb9311a"),
                },
                new VoucherCode()
                {
                    Id = Guid.Parse("5a87a8f5-e447-4ec4-9223-491eaffe672d"),
                    Code = "123",
                    Status = VoucherCodeStatusEnum.SOLD.ToString(),
                    VoucherId = Guid.Parse("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                    OrderDetailId = Guid.Parse("d03c5b75-25b0-4ec8-98b3-9f80fcb9311a"),
                }
            );

            // ORDER
            modelBuilder.Entity<Order>().HasData(
                new Order()
                {
                    Id = Guid.Parse("01aa28e3-4554-48c5-8324-80c4e6abd582"),
                    TotalPrice = 10000,
                    DiscountPrice = 1000,
                    FinalPrice = 9000,
                    Status = OrderStatusEnum.PENDING.ToString(),
                    UserId = Guid.Parse("e55ee134-b4ec-43f2-a565-8bcec52dff23")
                },
                new Order()
                {
                    Id = Guid.Parse("f87dade4-73ac-4922-a09d-e6efe4f7ac17"),
                    TotalPrice = 20000,
                    DiscountPrice = 1000,
                    FinalPrice = 19000,
                    Status = OrderStatusEnum.PENDING.ToString(),
                    UserId = Guid.Parse("e55ee134-b4ec-43f2-a565-8bcec52dff23")
                }
            );

            // ORDER DETAIL
            modelBuilder.Entity<OrderDetail>().HasData(
                new OrderDetail()
                {
                    Id = Guid.Parse("9c79e86d-3c11-422d-9e51-0ef985e87084"),
                    TotalPrice = 2000,
                    Quantity = 2,
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    VoucherId = Guid.Parse("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                    DiscountPrice = 0,
                    DiscountValue = 0,
                    FinalPrice = 4000,
                    OrderId = Guid.Parse("01aa28e3-4554-48c5-8324-80c4e6abd582"),
                    UnitPrice = 1000,
                },
                new OrderDetail()
                {
                    Id = Guid.Parse("d03c5b75-25b0-4ec8-98b3-9f80fcb9311a"),
                    TotalPrice = 2000,
                    Quantity = 1,
                    Status = ObjectStatusEnum.ACTIVE.ToString(),
                    VoucherId = Guid.Parse("494b5347-378e-4e2d-9553-6032a42cd8d1"),
                    DiscountPrice = 0,
                    DiscountValue = 0,
                    FinalPrice = 4000,
                    OrderId = Guid.Parse("01aa28e3-4554-48c5-8324-80c4e6abd582"),
                    UnitPrice = 1000,
                }
            );
        }
    }
}
