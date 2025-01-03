﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(Brand))]
    public class Brand
    {
        public Brand()
        {
            Addresses = [];
            Vouchers = [];
        }

        [InverseProperty(nameof(Voucher.Brand))]
        public virtual ICollection<Voucher> Vouchers { get; set; }
        [InverseProperty(nameof(Address.Brands))]
        public virtual ICollection<Address> Addresses { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }
        public string? Image { get; set; }
        public bool IsVerified { get; set; }

        public bool IsActive { get; set; }
        //public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        //public DateTime? VerifiedDate { get; set; }
        //public Guid? VerifiedBy { get; set; }
    }
}
