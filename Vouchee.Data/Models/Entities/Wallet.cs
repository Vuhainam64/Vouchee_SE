﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vouchee.Data.Models.Entities
{
    [Table("Wallet")]
    public class Wallet
    {
        public Wallet()
        {
            SellerWalletTransactions = new List<WalletTransaction>();
            BuyerWalletTransactions = new List<WalletTransaction>();
        }

        [InverseProperty(nameof(WalletTransaction.SellerWallet))]
        public virtual ICollection<WalletTransaction> SellerWalletTransactions { get; set; }

        [InverseProperty(nameof(WalletTransaction.BuyerWallet))]
        public virtual ICollection<WalletTransaction> BuyerWalletTransactions { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        // Foreign Key for Buyer Wallet
        public Guid? BuyerId { get; set; }

        [ForeignKey(nameof(BuyerId))]
        [InverseProperty(nameof(User.BuyerWallet))]
        public virtual User? Buyer { get; set; }

        // Foreign Key for Seller Wallet
        public Guid? SellerId { get; set; }

        [ForeignKey(nameof(SellerId))]
        [InverseProperty(nameof(User.SellerWallet))]
        public virtual User? Seller { get; set; }

        public int Balance { get; set; }

        public bool IsActive = true;
        public required string Status { get; set; }
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
