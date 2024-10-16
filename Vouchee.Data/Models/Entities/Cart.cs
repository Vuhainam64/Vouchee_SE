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
    [Table("Cart")]
    [Index(nameof(UserId), Name = "IX_Cart_UserId")]
    public partial class Cart
    {
        public Cart()
        {
            Vouchers = new HashSet<Voucher>();
        }
        [InverseProperty(nameof(Voucher.Carts))]
        public virtual ICollection<Voucher> Vouchers { get; set; }

        public Guid? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("Carts")]
        public virtual User? User { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid? Id { get; set; }
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
