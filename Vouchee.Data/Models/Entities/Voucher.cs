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
    [Index(nameof(SellerID), Name = "IX_Voucher_SellerId")]
    [Index(nameof(BrandId), Name = "IX_Voucher_BrandId")]
    public partial class Voucher
    {
        public Voucher()
        {
            Medias = [];
            VoucherCodes = [];
            OrderDetails = [];
            Promotions = [];
            Categories = [];
            Modals = [];
        }

        [InverseProperty(nameof(Modal.Voucher))]
        public virtual ICollection<Modal> Modals { get; set; }
        [InverseProperty(nameof(Media.Voucher))]
        public virtual ICollection<Media> Medias { get; set; }
        [InverseProperty(nameof(VoucherCode.Voucher))]
        public virtual ICollection<VoucherCode> VoucherCodes { get; set; }
        [InverseProperty(nameof(OrderDetail.Voucher))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(Promotion.Vouchers))]
        public virtual ICollection<Promotion> Promotions { get; set; }
        [InverseProperty(nameof(Category.Vouchers))]
        public virtual ICollection<Category> Categories { get; set; }

        public Guid BrandId { get; set; }
        [ForeignKey(nameof(BrandId))]
        [InverseProperty("Vouchers")]
        public required virtual Brand Brand { get; set; }

        public Guid SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty("Vouchers")]
        public required virtual Supplier? Supplier { get; set; }

        public Guid SellerID { get; set; }
        [ForeignKey(nameof(SellerID))]
        [InverseProperty("Vouchers")]
        public required virtual User? Seller { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Title { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(10,5)")]
        public decimal Rating { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
