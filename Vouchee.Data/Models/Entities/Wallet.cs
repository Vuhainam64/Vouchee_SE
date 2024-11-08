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
    [Table("Wallet")]
    [Index(nameof(UserId), Name = "IX_Wallet_UserId")]
    public class Wallet
    {
        public Wallet()
        {
            SellerWalletTransactions = [];
            BuyerWalletTransactions = [];
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }


        [InverseProperty(nameof(WalletTransaction.SellerWallet))]
        public virtual ICollection<WalletTransaction> SellerWalletTransactions { get; set; }
        [InverseProperty(nameof(WalletTransaction.BuyerWallet))]
        public virtual ICollection<WalletTransaction> BuyerWalletTransactions { get; set; }

        public Guid? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("Wallets")]
        public virtual User? User { get; set; }

        public int Balance { get; set; }
        public required string Type { get; set; }

        public required string Status { get; set; }
        public required DateTime CreateDate { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
