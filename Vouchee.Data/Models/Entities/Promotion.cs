using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(Promotion))]
    public class Promotion
    {
        public Promotion()
        {
            ModalPromotionOrderDetails = [];
            ShopPromotionOrderDetails = [];
            Modals = [];
        }

        [InverseProperty(nameof(OrderDetail.ShopPromotion))]
        public virtual ICollection<OrderDetail> ShopPromotionOrderDetails { get; set; }
        [InverseProperty(nameof(OrderDetail.ModalPromotion))]
        public virtual ICollection<OrderDetail> ModalPromotionOrderDetails { get; set; }
        [InverseProperty(nameof(Modal.Promotions))]
        public virtual ICollection<Modal> Modals { get; set; }

        public Guid? SellerId { get; set; }
        [ForeignKey(nameof(SellerId))]
        public required virtual User? Seller { get; set; }

        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        public string? Image { get; set; }
        public int? PercentDiscount { get; set; }
        public int? Stock { get; set; }
        public string? Code { get; set; }
        public int? MoneyDiscount { get; set; }
        public int? MaxMoneyToDiscount { get; set; }
        public int? MinMoneyToAppy { get; set; }
        public int? RequiredQuantity { get; set; }
        public string? Type { get; set; }

        public bool IsActive { get; set; }
        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
