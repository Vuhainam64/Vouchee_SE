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
    [Table("VoucherCode")]
    [Index(nameof(VoucherId), Name = "IX_Voucher_VoucherId")]
    [Index(nameof(OrderDetailId), Name = "IX_Voucher_OrderDetailId")]
    public partial class VoucherCode
    {
        public Guid VoucherId { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("VoucherCodes")]
        public required virtual Voucher Voucher { get; set; }

        public Guid? OrderDetailId { get; set; }
        [ForeignKey(nameof(OrderDetailId))]
        [InverseProperty("VoucherCodes")]
        public virtual OrderDetail? OrderDetail { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Code { get; set; }
        public string? Image { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public Guid CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
