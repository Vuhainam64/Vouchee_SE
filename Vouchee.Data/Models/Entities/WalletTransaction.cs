using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(WalletTransaction))]
    public class WalletTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty(nameof(Order.WalletTransactions))]
        public virtual Order? Order { get; set; }

        public Guid? SellerWalletId { get; set; }
        [ForeignKey(nameof(SellerWalletId))]
        [InverseProperty(nameof(SellerWallet.SellerWalletTransactions))]
        public virtual Wallet? SellerWallet { get; set; }

        public Guid? BuyerWalletId { get; set; }
        [ForeignKey(nameof(BuyerWalletId))]
        [InverseProperty(nameof(BuyerWallet.BuyerWalletTransactions))]
        public virtual Wallet? BuyerWallet { get; set; }

        public Guid? SupplierWalletId { get; set; }
        [ForeignKey(nameof(SupplierWalletId))]
        [InverseProperty(nameof(Wallet.SupplierWalletTransactions))]
        public virtual Wallet? SupplierWallet { get; set; }

        public string? TopUpRequestId { get; set; }
        [ForeignKey(nameof(TopUpRequestId))]
        [InverseProperty(nameof(MoneyRequest.TopUpWalletTransaction))]
        public virtual MoneyRequest? TopUpRequest { get; set; }

        public string? WithdrawRequestId { get; set; }
        [ForeignKey(nameof(WithdrawRequestId))]
        [InverseProperty(nameof(MoneyRequest.WithdrawWalletTransaction))]
        public virtual MoneyRequest? WithDrawRequest { get; set; }

        public Guid? PartnerTransactionId { get; set; }
        [ForeignKey(nameof(PartnerTransactionId))]
        [InverseProperty(nameof(PartnerTransaction.WalletTransactions))]
        public virtual PartnerTransaction? PartnerTransaction { get; set; }

        public required string Type { get; set; }
        public int BeforeBalance { get; set; }
        public int Amount { get; set; }
        public int AfterBalance { get; set; }
        public string? Note { get; set; }
        public Guid? UpdateId { get; set; }

        public required string Status { get; set; }
        public DateTime? CreateDate { get; set; } 
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
