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
    [Table("Category")]
    [Index(nameof(VoucherTypeId), Name = "IX_Category_VoucherTypeId")]
    public class Category
    {
        public Guid? VoucherTypeId { get; set; }
        [ForeignKey(nameof(VoucherTypeId))]
        [InverseProperty("Categories")]
        public virtual VoucherType? VoucherType { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Title { get; set; }
        public string? Image { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? PercentShow { get; set; }

        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
