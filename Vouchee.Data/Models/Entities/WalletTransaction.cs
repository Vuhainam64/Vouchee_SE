﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table("WalletTransaction")]
    public class WalletTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public Guid? OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty("WalletTransaction")]
        public virtual Order? Order { get; set; }

        public Guid? SellerWalletId { get; set; }
        [ForeignKey(nameof(SellerWalletId))]
        [InverseProperty(nameof(SellerWallet.SellerWalletTransactions))]
        public virtual Wallet? SellerWallet { get; set; }

        public Guid? BuyerWalletId { get; set; }
        [ForeignKey(nameof(BuyerWalletId))]
        [InverseProperty(nameof(BuyerWallet.BuyerWalletTransactions))]
        public virtual Wallet? BuyerWallet { get; set; }

        public Guid? TopUpRequestId { get; set; }
        [ForeignKey(nameof(TopUpRequestId))]
        [InverseProperty(nameof(TopUpRequest.WalletTransaction))]
        public virtual TopUpRequest? TopUpRequest { get; set; }

        public int Amount { get; set; }

        public required string Status { get; set; }
        public required DateTime CreateDate { get; set; }
        public required Guid CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
