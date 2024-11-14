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

        public Guid? OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty(nameof(Order.WalletTransaction))]
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

        public Guid? PartnerTransactionId { get; set; }
        [ForeignKey(nameof(PartnerTransactionId))]
        [InverseProperty(nameof(PartnerTransaction.WalletTransactions))]
        public virtual PartnerTransaction? PartnerTransaction { get; set; }

        public required string Type { get; set; }
        public int BeforeBalance { get; set; }
        public int Amount { get; set; }
        public int AfterBalance { get; set; }

        public required string Status { get; set; }
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
