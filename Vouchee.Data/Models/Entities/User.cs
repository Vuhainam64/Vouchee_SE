﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(User))]
    public partial class User
    {
        public User()
        {
            Orders = [];
            Vouchers = [];
            Carts = [];
            ReceiverNotifications = [];
            //SenderNotifications = [];
            MoneyRequests = [];
            ShopPromotions = [];
            DeviceTokens = [];
            Reports = [];
        }

        [InverseProperty(nameof(Wallet.Buyer))]
        public virtual Wallet? BuyerWallet { get; set; }
        [InverseProperty(nameof(Wallet.Seller))]
        public virtual Wallet? SellerWallet { get; set; }

        public Guid? SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty(nameof(Supplier.Users))]
        public virtual Supplier? Supplier { get; set; }

        [InverseProperty(nameof(Order.Buyer))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(Voucher.Seller))]
        public virtual ICollection<Voucher> Vouchers { get; set; }
        [InverseProperty(nameof(Cart.Buyer))]
        public virtual ICollection<Cart> Carts { get; set; }
        [InverseProperty(nameof(Notification.Receiver))]
        public virtual ICollection<Notification> ReceiverNotifications { get; set; }
        //[InverseProperty(nameof(Notification.Sender))]
        //public virtual ICollection<Notification> SenderNotifications { get; set; }
        [InverseProperty(nameof(MoneyRequest.User))]
        public virtual ICollection<MoneyRequest> MoneyRequests { get; set; }
        [InverseProperty(nameof(Promotion.Seller))]
        public virtual ICollection<Promotion> ShopPromotions { get; set; }
        [InverseProperty(nameof(DeviceToken.Users))]
        public virtual ICollection<DeviceToken> DeviceTokens { get; set; }
        [InverseProperty(nameof(Report.User))]
        public virtual ICollection<Report> Reports { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? HashPassword { get; set; }
        public int NumberOfReport => Reports.Count();
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? PhoneNumber { get; set; }
        public int VPoint { get; set; }
        public string? Role { get; set; }
        //public string? BankName { get; set; }
        //public string? BankNumber { get; set; }
        //public string? BankAccount { get; set; }

        public bool IsActive { get; set; }
        public required string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? LastAccessTime { get; set; }
    }
}
