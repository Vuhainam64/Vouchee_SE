using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table("PartnerTransaction")]
    public class PartnerTransaction
    {
        public PartnerTransaction()
        {
            WalletTransactions = [];
        }

        [InverseProperty(nameof(WalletTransaction.PartnerTransaction))]
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Gateway { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? AccountNumber { get; set; }
        public string? SubAccount { get; set; }
        public int? AmountIn { get; set; }
        public int? AmountOut { get; set; }
        public int? Accumulated { get; set; }
        public string? Code { get; set; }
        public string? TransactionContent { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Body { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
