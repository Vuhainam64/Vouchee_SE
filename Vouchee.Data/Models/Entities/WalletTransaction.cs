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
    [Table("WalletTransaction")]
    [Index(nameof(OrderId), Name = "IX_WalletTransaction_OrderId")]
    [Index(nameof(AccountTransactionId), Name = "IX_WalletTransaction_AccountTransactionId")]
    [Index(nameof(SellerWalletId), Name = "IX_WalletTransaction_SellerWalletId")]
    [Index(nameof(BuyerWalletId), Name = "IX_WalletTransaction_BuyerWalletId")]
    public class WalletTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public Guid? AccountTransactionId { get; set; }
        [ForeignKey(nameof(AccountTransactionId))]
        [InverseProperty("WalletTransaction")]
        public virtual AccountTransaction? AccountTransaction { get; set; }

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

        public int Amount { get; set; }

        public required string Status { get; set; }
        public required DateTime CreateDate { get; set; }
        public required Guid CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
