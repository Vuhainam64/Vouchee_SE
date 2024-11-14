﻿using Microsoft.EntityFrameworkCore;
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
        public Guid ModalId { get; set; }
        [ForeignKey(nameof(ModalId))]
        [InverseProperty(nameof(Modal.VoucherCodes))]
        public required virtual Modal Modal { get; set; }

        public Guid? OrderDetailId { get; set; }
        [ForeignKey(nameof(OrderDetailId))]
        [InverseProperty(nameof(OrderDetail.VoucherCodes))]
        public virtual OrderDetail? OrderDetail { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Code { get; set; }
        public string? Image { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
