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
        public OrderDetail()
        {
            VoucherCodes = [];
        }

        [InverseProperty(nameof(VoucherCode.OrderDetail))]
        public virtual ICollection<VoucherCode> VoucherCodes { get; set; }

        public Guid OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty(nameof(Order.OrderDetails))]
        public virtual Order? Order { get; set; }

        public Guid? ModalId { get; set; }
        [ForeignKey(nameof(ModalId))]
        [InverseProperty(nameof(Modal.OrderDetails))]
        public virtual Modal? Modal { get; set; }

        public Guid? ModalPromotionId { get; set; }
        [ForeignKey(nameof(ModalPromotionId))]
        [InverseProperty(nameof(ModalPromotion.OrderDetails))]
        public virtual ModalPromotion? ModalPromotion { get; set; }

        public Guid? ShopPromotionId { get; set; }
        [ForeignKey(nameof(ShopPromotionId))]
        [InverseProperty(nameof(ShopPromotion.OrderDetails))]
        public virtual ShopPromotion? ShopPromotion { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public int UnitPrice { get; set; }
        public int ShopDiscountPercent { get; set; } = 0;
        public int ModalDiscountPercent { get; set; } = 0;
        public int ModalDiscountMoney { get; set; } = 0;
        public int TotalPrice => UnitPrice * Quantity;
        public int DiscountPrice => (TotalPrice * ShopDiscountPercent * 100) + (TotalPrice * ModalDiscountPercent * 100) + ModalDiscountMoney;
        public int FinalPrice => TotalPrice - DiscountPrice;
        public int Quantity { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
