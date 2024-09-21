using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table("Shop")]
    public partial class Shop
    {
        public Shop()
        {
            Vouchers = new HashSet<Voucher>();
        }

        [InverseProperty(nameof(Voucher.Shops))]
        public virtual ICollection<Voucher> Vouchers { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? PercentShow { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Lat { get; set; }

        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
