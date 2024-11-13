using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table("Promotion")]

    public partial class Promotion
    {
        public Promotion()
        {
            Vouchers = [];
            Orders = [];
            OrderDetails = [];
            Modals = [];
        }

        [InverseProperty(nameof(Voucher.Promotions))]
        public virtual ICollection<Voucher> Vouchers { get; set; }
        [InverseProperty(nameof(Modal.Promotions))]
        public virtual ICollection<Modal> Modals { get; set; }
        [InverseProperty(nameof(Order.Promotion))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(OrderDetail.Promotion))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public Guid? SellerId { get; set; }
        [ForeignKey(nameof(SellerId))]
        [InverseProperty("Promotions")]
        public required virtual User? Seller { get; set; }

        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Stock { get; set; }
        public string? Code { get; set; }
        public required string Type { get; set; }
        public string? Policy { get; set; }
        public string? Image { get; set; }
        public int? PercentDiscount { get; set; }
        public int? MoneyDiscount { get; set; }
        public int? MaxMoneyToDiscount { get; set; }
        public int? MinMoneyToAppy { get; set; }
        public int? RequiredQuantity { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public required DateTime CreateDate { get; set; }
        public required Guid CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
