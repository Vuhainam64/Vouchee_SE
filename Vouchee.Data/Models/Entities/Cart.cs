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
    [Table("Cart")]
    [Index(nameof(BuyerId), Name = "IX_Cart_BuyerId")]
    [Index(nameof(VoucherId), Name = "IX_Cart_VoucherId")]
    public partial class Cart
    {
        public Guid VoucherId { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("Carts")]
        public virtual Voucher? Voucher { get; set; }

        public Guid? BuyerId { get; set; }
        [ForeignKey(nameof(BuyerId))]
        [InverseProperty("Carts")]
        public virtual User? Buyer { get; set; }

        public int Quantity { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public Guid CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
