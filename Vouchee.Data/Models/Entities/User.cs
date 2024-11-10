using System;
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
            Orders = new List<Order>();
            Vouchers = new List<Voucher>();
            Carts = new List<Cart>();
            ReceiverNotifications = new List<Notification>();
            SenderNotifications = new List<Notification>();
            WithdrawRequests = new List<WithdrawRequest>();
        }

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

        public Guid? RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public virtual Role? Role { get; set; }

        // Separate navigation properties for buyer and seller wallets
        [InverseProperty(nameof(Wallet.Buyer))]
        public virtual Wallet? BuyerWallet { get; set; }

        [InverseProperty(nameof(Wallet.Seller))]
        public virtual Wallet? SellerWallet { get; set; }

        public virtual Supplier? Supplier { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Notification> ReceiverNotifications { get; set; }
        public virtual ICollection<Notification> SenderNotifications { get; set; }
        public virtual ICollection<WithdrawRequest> WithdrawRequests { get; set; }
    }
}
