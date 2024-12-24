using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(RefundRequest))]
    public class RefundRequest
    {
        public RefundRequest() 
        {
            Medias = [];
        }

        [InverseProperty(nameof(Media.RefundRequest))]
        public virtual ICollection<Media> Medias { get; set; }

        [InverseProperty(nameof(WalletTransaction.RefundRequest))]
        public virtual WalletTransaction? WalletTransaction { get; set; }

        public Guid? VoucherCodeId { get; set; }
        [ForeignKey(nameof(VoucherCodeId))]
        [InverseProperty(nameof(VoucherCode.RefundRequest))]
        public virtual VoucherCode? VoucherCode { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Content { get; set; }
        [Column(TypeName = "decimal(38, 20)")]
        public decimal Lon { get; set; }
        [Column(TypeName = "decimal(38, 20)")]
        public decimal Lat { get; set; }
        public string? Reason { get; set; }

        public string? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
