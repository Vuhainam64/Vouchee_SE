using shortid.Configuration;
using shortid;
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
        public MoneyRequest()
        {
            Id = ShortId.Generate(new GenerationOptions(useSpecialCharacters: false));
        }

        public Guid? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.MoneyRequests))]
        public virtual User? User { get; set; }

        [InverseProperty(nameof(WalletTransaction.TopUpRequest))]
        public virtual WalletTransaction? TopUpWalletTransaction { get; set; }
        [InverseProperty(nameof(WalletTransaction.WithDrawRequest))]
        public virtual WalletTransaction? WithdrawWalletTransaction { get; set; }

        [Key]
        public string Id { get; set; }

        public int Amount { get; set; }
        public string? Type { get; set; }
        public Guid? UpdateId { get; set; }
        public string? Note { get; set; }

        public required string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
