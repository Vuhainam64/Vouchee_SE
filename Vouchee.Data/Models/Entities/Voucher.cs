using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers;

namespace Vouchee.Data.Models.Entities
{
    [Table("Voucher")]
    [Index(nameof(SupplierId), Name = "IX_Voucher_SupplierId")]
    [Index(nameof(VoucherTypeId), Name = "IX_Voucher_VoucherTypeId")]
    [Index(nameof(VoucherTypeId), Name = "IX_Voucher_CreateBy")]
    public partial class Voucher
    {
        public Voucher()
        {
            VoucherCodes = new HashSet<VoucherCode>();
            Shops = new HashSet<Shop>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        [InverseProperty(nameof(VoucherCode.Voucher))]
        public virtual ICollection<VoucherCode> VoucherCodes { get; set; }
        [InverseProperty(nameof(OrderDetail.Voucher))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(Shop.Vouchers))]
        public virtual ICollection<Shop> Shops { get; set; }

        public Guid? SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("Vouchers")]
        public virtual Supplier? Supplier { get; set; }

        public Guid? CreateBy { get; set; }
        [ForeignKey(nameof(CreateBy))]
        [InverseProperty("Vouchers")]
        public virtual User? Seller { get; set; }

        public Guid? VoucherTypeId { get; set; }
        [ForeignKey(nameof(VoucherTypeId))]
        [InverseProperty("Vouchers")]
        public virtual VoucherType? VoucherType { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Price { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StarDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime EndDate { get; set; }
        public string? Policy { get; set; }
        public int Quantity { get; set; }
        public string? Image { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? PercentShow { get; set; }

        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
