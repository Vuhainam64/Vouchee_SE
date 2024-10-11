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
    [Table("Address")]
    [Index(nameof(ShopId), Name = "IX_Address_ShopId")]
    public class Address
    {
        public Guid? ShopId { get; set; }
        [ForeignKey(nameof(ShopId))]
        [InverseProperty("Addresses")]
        public virtual Shop? Shop { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? Lon { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? Lat { get; set; }
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
