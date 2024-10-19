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
    [Table("Image")]
    [Index(nameof(VoucherId), Name = "IX_Image_VoucherId")]
    [Index(nameof(SubVoucherId), Name = "IX_Image_SubVoucherId")]
    public class Image
    {
        public Guid? VoucherId { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("Images")]
        public virtual Voucher? Voucher { get; set; }

        public Guid? SubVoucherId { get; set; }
        [ForeignKey(nameof(SubVoucherId))]
        [InverseProperty("Images")]
        public virtual SubVoucher? SubVoucher { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }

        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
