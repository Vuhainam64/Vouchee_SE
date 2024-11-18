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
    [Table(nameof(OrderDetail))]
    public partial class OrderDetail
    {
        [Key]
        public string? OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty(nameof(Order.OrderDetails))]
        public virtual Order? Order { get; set; }

        [Key]
        public Guid? ModalId { get; set; }
        [ForeignKey(nameof(ModalId))]
        [InverseProperty(nameof(Modal.OrderDetails))]
        public virtual Modal? Modal { get; set; }

        public Guid? ShopPromotionId { get; set; }
        [ForeignKey(nameof(ShopPromotionId))]
        [InverseProperty(nameof(ShopPromotion.ShopPromotionOrderDetails))]
        public virtual Promotion? ShopPromotion { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public int UnitPrice { get; set; }
        public int ShopDiscountPercent { get; set; } = 0;
        public int ShopDiscountMoney { get; set; } = 0;
        public int TotalPrice => UnitPrice * Quantity;
        public int DiscountPrice => (TotalPrice * ShopDiscountPercent * 100) + ShopDiscountMoney;
        public int FinalPrice => TotalPrice - DiscountPrice;
        public int Quantity { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
