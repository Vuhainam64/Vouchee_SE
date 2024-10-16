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
    [Table("SubVoucher")]
    public class SubVoucher
    {
        public SubVoucher()
        {
            Images = new HashSet<Image>();
        }

        [InverseProperty(nameof(Image.SubVoucher))]
        public virtual ICollection<Image> Images { get; set; }

        public Guid? VoucherId { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("SubVouchers")]
        public virtual Voucher? Voucher { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? Title { get; set; }
        [Column(TypeName = "decimal(20,3)")]
        public decimal OriginalPrice { get; set; }
        [Column(TypeName = "decimal(20,3)")]
        public decimal SellPrice { get; set; }
        public int? Quantity { get; set; }

        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
