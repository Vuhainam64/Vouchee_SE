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
    [Table("Modal")]
    [Index(nameof(VoucherId), Name = "IX_Modal_VoucherId")]
    public class Modal
    {
        public Modal()
        {
            Medias = [];
            Carts = [];
        }

        [InverseProperty(nameof(Cart.Modal))]
        public virtual ICollection<Cart> Carts { get; set; }
        [InverseProperty(nameof(Media.Modal))]
        public virtual ICollection<Media> Medias { get; set; }

        public Guid VoucherId { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("Modals")]
        public required virtual Voucher Voucher { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Title { get; set; }
        [Column(TypeName = "decimal(10,5)")]
        public decimal OriginalPrice { get; set; }
        [Column(TypeName = "decimal(10,5)")]
        public decimal SellPrice { get; set; }
        public int Stock { get; set; }
        public int Index { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        public Guid CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
