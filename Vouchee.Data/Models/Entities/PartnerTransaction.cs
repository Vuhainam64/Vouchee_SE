using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(PartnerTransaction))]
    public class PartnerTransaction
    {
        [InverseProperty(nameof(Order.PartnerTransaction))]
        public virtual Order? Order { get; set; }
        [InverseProperty(nameof(WalletTransaction.PartnerTransaction))]
        public virtual WalletTransaction? WalletTransaction { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Gateway { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? AccountNumber { get; set; }
        public string? SubAccount { get; set; }
        public int AmountIn { get; set; }
        public int AmountOut { get; set; }
        public int Accumulated { get; set; }
        public string? Code { get; set; }
        public string? Content { get; set; }
        public string? ReferenceCode { get; set; }
        public string? Description { get; set; }
        public string? PartnerName { get; set; }
        public int PartnerTransactionId { get; set; }
    }
}
