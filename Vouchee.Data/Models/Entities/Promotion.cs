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
        }

        [InverseProperty(nameof(Voucher.Promotions))]
        public virtual ICollection<Voucher> Vouchers { get; set; }
        [InverseProperty(nameof(Order.Promotion))]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty(nameof(OrderDetail.Promotion))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Quantity { get; set; }
        public string? Code { get; set; }
        public required string Type { get; set; }
        public string? Policy { get; set; }
        public string? Image { get; set; }
        [Column(TypeName = "decimal(10,5)")]
        public decimal? PercentDiscount { get; set; }
        [Column(TypeName = "decimal(10,5)")]
        public decimal? MoneyDiscount { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public required DateTime CreateDate { get; set; }
        public required Guid CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
