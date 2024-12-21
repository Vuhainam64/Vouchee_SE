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
    [Table("VoucherCode")]
    public partial class VoucherCode
    {
        public string? OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty(nameof(Order.VoucherCodes))]
        public required virtual Order Order { get; set; }

        public Guid? ModalId { get; set; }
        [ForeignKey(nameof(ModalId))]
        [InverseProperty(nameof(Modal.VoucherCodes))]
        public required virtual Modal Modal { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Code { get; set; }
        public string? NewCode { get; set; }
        public string? Image { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public Guid? UpdateId { get; set; }

        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? VerifiedDate {get; set; }
        public Guid? VerifiedBy { get; set; }
    }
}
