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
    [Index(nameof(ModalId), Name = "IX_OrderDetail_ModalId")]
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

        public Guid? ModalId { get; set; }
        [ForeignKey(nameof(ModalId))]
        [InverseProperty("OrderDetails")]
        public virtual Modal? Modal { get; set; }

        public Guid? PromotionId { get; set; }
        [ForeignKey(nameof(PromotionId))]
        [InverseProperty("OrderDetails")]
        public virtual Promotion? Promotion { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "decimal(18,5)")]
        public decimal DiscountValue { get; set; } = 0;
        [Column(TypeName = "decimal(18,5)")]
        public decimal TotalPrice => UnitPrice * Quantity;
        [Column(TypeName = "decimal(18,5)")]
        public decimal DiscountPrice => TotalPrice * DiscountValue * 100;
        [Column(TypeName = "decimal(18,5)")]
        public decimal FinalPrice => TotalPrice - DiscountPrice;
        public int Quantity { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
