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
    [Table("Category")]
    [Index(nameof(VoucherTypeId), Name = "IX_Category_VoucherTypeId")]
    public class Category
    {
        public Category()
        {
            Vouchers = [];
        }

        public Guid VoucherTypeId { get; set; }
        [ForeignKey(nameof(VoucherTypeId))]
        [InverseProperty("Categories")]
        public required virtual VoucherType VoucherType { get; set; }

        [InverseProperty(nameof(Voucher.Categories))]
        public virtual ICollection<Voucher> Vouchers { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Title { get; set; }
        public string? Image { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public Guid CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
