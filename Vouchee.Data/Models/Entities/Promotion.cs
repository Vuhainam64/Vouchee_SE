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
            ShopPromotionOrderDetails = [];
        }

        [InverseProperty(nameof(OrderDetail.ShopPromotion))]
        public virtual ICollection<OrderDetail> ShopPromotionOrderDetails { get; set; }

        public Guid? SellerId { get; set; }
        [ForeignKey(nameof(SellerId))]
        public required virtual User? Seller { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? PercentDiscount { get; set; }
        public int? MoneyDiscount { get; set; }
        public int? RequiredQuantity { get; set; }
        public int? MaxMoneyToDiscount { get; set; }
        public int? MinMoneyToApply { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Stock { get; set; }
        public string? Image { get; set; }
        public string? Type { get; set; }

        public bool IsActive { get; set; }
        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
