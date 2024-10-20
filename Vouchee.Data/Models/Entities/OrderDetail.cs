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
    [Table("OrderDetail")]
    [Index(nameof(OrderId), Name = "IX_OrderDetail_OrderId")]
    [Index(nameof(VoucherId), Name = "IX_OrderDetail_VoucherId")]
    [Index(nameof(PromotionId), Name = "IX_OrderDetail_PromotionId")]
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            VoucherCodes = [];
        }

        [InverseProperty(nameof(VoucherCode.OrderDetail))]
        public virtual ICollection<VoucherCode> VoucherCodes { get; set; }

        public Guid OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty("OrderDetails")]
        public virtual Order? Order { get; set; }

        public Guid? VoucherId { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("OrderDetails")]
        public virtual Voucher? Voucher { get; set; }

        public Guid? PromotionId { get; set; }
        [ForeignKey(nameof(PromotionId))]
        [InverseProperty("OrderDetails")]
        public virtual Promotion? Promotion { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "decimal(10,5)")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "decimal(10,5)")]
        public decimal DiscountValue { get; set; } = 0;
        [Column(TypeName = "decimal(10,5)")]
        public decimal TotalPrice => UnitPrice * Quantity;
        [Column(TypeName = "decimal(10,5)")]
        public decimal DiscountPrice => TotalPrice * DiscountValue * 100;
        [Column(TypeName = "decimal(10,5)")]
        public decimal FinalPrice => TotalPrice - DiscountPrice;
        public int Quantity { get; set; }
    }
}
