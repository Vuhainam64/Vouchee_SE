﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vouchee.Data.Models.Entities
{
    [Table("User")]
    public partial class User
    {
        public User()
        {
            Orders = [];
            Vouchers = [];
            Carts = [];
            ReceiverNotifications = [];
            SenderNotifications = [];
            WithdrawRequests = [];
            Promotions = [];
        }

        public Guid? RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public virtual Role? Role { get; set; }

        [InverseProperty(nameof(Wallet.Buyer))]
        public virtual Wallet? BuyerWallet { get; set; }

        [InverseProperty(nameof(Wallet.Seller))]
        public virtual Wallet? SellerWallet { get; set; }

        [InverseProperty(nameof(Supplier.User))]
        public virtual Supplier? Supplier { get; set; }

        [InverseProperty(nameof(Order.Buyer))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(Voucher.Seller))]
        public virtual ICollection<Voucher> Vouchers { get; set; }
        [InverseProperty(nameof(Cart.Buyer))]
        public virtual ICollection<Cart> Carts { get; set; }
        [InverseProperty(nameof(Notification.Receiver))]
        public virtual ICollection<Notification> ReceiverNotifications { get; set; }
        [InverseProperty(nameof(Notification.Sender))]
        public virtual ICollection<Notification> SenderNotifications { get; set; }
        [InverseProperty(nameof(WithdrawRequest.User))]
        public virtual ICollection<WithdrawRequest> WithdrawRequests { get; set; }
        [InverseProperty(nameof(Promotion.Seller))]
        public virtual ICollection<Promotion> Promotions { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? HashPassword { get; set; }
        public string? Image { get; set; }
        public string? PhoneNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public int ResponsibilityScore { get; set; }
        public int VPoint { get; set; }
        public required string Status { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
