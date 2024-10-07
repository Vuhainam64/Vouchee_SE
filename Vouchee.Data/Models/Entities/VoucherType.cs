using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table("VoucherType")]
    public partial class VoucherType
    {
        public VoucherType()
        {
            Vouchers = new HashSet<Voucher>();
            Categories = new HashSet<Category>();
        }

        [InverseProperty(nameof(Voucher.VoucherType))]
        public virtual ICollection<Voucher> Vouchers { get; set; }

        [InverseProperty(nameof(Category.VoucherType))]
        public virtual ICollection<Category> Categories { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Title { get; set; }
        public string? Image { get; set; }

        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
