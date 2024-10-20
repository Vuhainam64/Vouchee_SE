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
    [Table("Media")]
    [Index(nameof(VoucherId), Name = "IX_Image_VoucherId")]
    [Index(nameof(SubVoucherId), Name = "IX_Image_SubVoucherId")]
    public class Media
    {
        public Guid? VoucherId { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("Medias")]
        public virtual Voucher? Voucher { get; set; }

        public Guid? SubVoucherId { get; set; }
        [ForeignKey(nameof(SubVoucherId))]
        [InverseProperty("Medias")]
        public virtual SubVoucher? SubVoucher { get; set; }

        public Guid? AddressId { get; set; }
        [ForeignKey(nameof(SubVoucherId))]
        [InverseProperty("Medias")]
        public virtual Address? Address { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Url { get; set; }
        public required string Type { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public Guid CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
