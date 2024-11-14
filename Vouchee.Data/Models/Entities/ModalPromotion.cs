using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(ModalPromotion))]
    public partial class ModalPromotion
    {
        public ModalPromotion()
        {
            OrderDetails = [];
            Modals = [];
        }

        [InverseProperty(nameof(OrderDetail.ModalPromotion))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(Modal.ModalPromotion))]
        public virtual ICollection<Modal> Modals { get; set; }

        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        public int? Stock { get; set; }
        public string? Code { get; set; }
        public int? PercentDiscount { get; set; }
        public int? MoneyDiscount { get; set; }
        public int? MaxMoneyToDiscount { get; set; }
        public int? MinMoneyToAppy { get; set; }
        public int? RequiredQuantity { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
