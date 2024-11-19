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
    [Table(nameof(Cart))]
    public partial class Cart
    {
        public Guid ModalId { get; set; }
        [ForeignKey(nameof(ModalId))]
        [InverseProperty(nameof(Modal.Carts))]
        public virtual Modal? Modal { get; set; }

        public Guid? BuyerId { get; set; }
        [ForeignKey(nameof(BuyerId))]
        [InverseProperty(nameof(Buyer.Carts))]
        public virtual User? Buyer { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
