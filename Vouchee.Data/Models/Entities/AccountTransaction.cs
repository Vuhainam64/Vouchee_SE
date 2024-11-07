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
    [Table("AccountTransaction")]
    [Index(nameof(FromUserId), Name = "IX_AccountTransaction_FromUserId")]
    [Index(nameof(ToUserId), Name = "IX_AccountTransaction_ToUserId")]
    public class AccountTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public Guid? FromUserId { get; set; }
        [ForeignKey(nameof(FromUserId))]
        public virtual User? FromUser { get; set; }

        public Guid? ToUserId { get; set; }
        [ForeignKey(nameof(ToUserId))]
        public virtual User? ToUser { get; set; }

        public virtual WalletTransaction? WalletTransaction { get; set; }

        public int Amount { get; set; }
        public string? Type { get; set; }

        public required string Status { get; set; }
        public required DateTime CreateDate { get; set; }
        public required Guid CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
