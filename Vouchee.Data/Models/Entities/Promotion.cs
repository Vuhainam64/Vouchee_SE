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
            Vouchers = new HashSet<Voucher>();
            Orders = new HashSet<Order>();
            OrderDetails = new HashSet<OrderDetail>();
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

        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Quantity { get; set; }
        public string? Code { get; set; }
        public string? Type { get; set; }
        public string? Policy { get; set; }

        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
