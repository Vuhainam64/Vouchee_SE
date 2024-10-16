﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table("Address")]
    public partial class Address
    {
        public Address()
        {
            Vouchers = new HashSet<Voucher>();
        }

        [InverseProperty(nameof(Voucher.Addresses))]
        public virtual ICollection<Voucher>? Vouchers { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string? AddressName { get; set; }
        [Column(TypeName = "decimal(38, 20)")]
        public decimal? Lon { get; set; }
        [Column(TypeName = "decimal(38, 20)")]
        public decimal? Lat { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? PercentShow { get; set; }
        public string? Image { get; set; }

        public string? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
