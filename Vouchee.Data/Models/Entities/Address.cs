﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(Address))]
    public partial class Address
    {
        public Address()
        {
            Brands = [];
        }

        [InverseProperty(nameof(Brand.Addresses))]
        public virtual ICollection<Brand> Brands { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }
        [Column(TypeName = "decimal(38, 20)")]
        public decimal? Lon { get; set; }
        [Column(TypeName = "decimal(38, 20)")]
        public decimal? Lat { get; set; }
        public bool IsVerified { get; set; }
        //public DateTime? VerifiedDate { get; set; }
        //public Guid? VerifiedBy { get; set; }

        public bool IsActive { get; set; }
        //public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
