using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(MoneyRequest))]
    public class MoneyRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.MoneyRequests))]
        public virtual User? User { get; set; }

        public int Amount { get; set; }
        public string? Type { get; set; }

        [InverseProperty(nameof(WalletTransaction.TopUpRequest))]
        public virtual WalletTransaction? TopUpWalletTransaction { get; set; }

        [InverseProperty(nameof(WalletTransaction.WithDrawRequest))]
        public virtual WalletTransaction? WithdrawWalletTransaction { get; set; }

        public required string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
