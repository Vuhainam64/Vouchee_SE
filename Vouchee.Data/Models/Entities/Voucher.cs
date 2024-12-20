﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(Voucher))]
    public partial class Voucher
    {
        public Voucher()
        {
            Medias = [];
            Categories = [];
            Modals = [];
        }

        [InverseProperty(nameof(Modal.Voucher))]
        public virtual ICollection<Modal> Modals { get; set; }
        [InverseProperty(nameof(Media.Voucher))]
        public virtual ICollection<Media> Medias { get; set; }
        [InverseProperty(nameof(Category.Vouchers))]
        public virtual ICollection<Category> Categories { get; set; }

        public Guid BrandId { get; set; }
        [ForeignKey(nameof(BrandId))]
        [InverseProperty(nameof(Brand.Vouchers))]
        public required virtual Brand Brand { get; set; }

        public Guid SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))]
        [InverseProperty(nameof(Supplier.Vouchers))]
        public required virtual Supplier? Supplier { get; set; }

        public Guid SellerId { get; set; }
        [ForeignKey(nameof(SellerId))]
        [InverseProperty(nameof(Seller.Vouchers))]
        public required virtual User? Seller { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Video { get; set; }
        public decimal AverageRating => Modals.Any(modal => modal.AverageRating > 0)
            ? Math.Round(
                Modals
                    .Where(modal => modal.AverageRating > 0)
                    .Average(modal => modal.AverageRating),
                1)
            : 0;

        public int TotalQuantitySold => Modals.Sum(x => x.OrderDetails.Sum(x => x.Quantity));
        public int Stock => Modals.Sum(x => x.Stock);

        public bool IsActive { get; set; }
        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
