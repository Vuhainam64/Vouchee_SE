﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        [InverseProperty("Cart")]
        public virtual User? User { get; set; }
        
        public decimal? Quantity { get; set; }
    }
}
