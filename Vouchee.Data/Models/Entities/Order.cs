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
    [Table("Order")]
    [Index(nameof(CreateBy), Name = "IX_Order_UserId")]
    [Index(nameof(PromotionId), Name = "IX_Order_PromotionId")]
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [InverseProperty(nameof(OrderDetail.Order))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [ForeignKey(nameof(CreateBy))]
        [InverseProperty("Orders")]
        public virtual User? User { get; set; }

        public Guid? PromotionId { get; set; }
        [ForeignKey(nameof(PromotionId))]
        [InverseProperty("Orders")]
        public virtual Promotion? Promotion { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? PaymentType { get; set; }
        [Column(TypeName = "decimal")]
        public decimal DiscountValue { get; set; } = 0;
        [Column(TypeName = "decimal")]
        public decimal TotalPrice { get; set; }
        [Column(TypeName = "decimal")]
        public decimal DiscountPrice => TotalPrice * DiscountValue;
        [Column(TypeName = "decimal")]
        public decimal FinalPrice => TotalPrice - DiscountPrice;

        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
