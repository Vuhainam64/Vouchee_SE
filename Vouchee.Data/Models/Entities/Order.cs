﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers;
using shortid;
using shortid.Configuration;

namespace Vouchee.Data.Models.Entities
{
    [Table(nameof(Order))]
    public partial class Order
    {
        public Order()
        {
            WalletTransactions = [];
            OrderDetails = [];
            Id = ShortId.Generate(new GenerationOptions(useSpecialCharacters: false));
            VoucherCodes = [];
        }

        [InverseProperty(nameof(Rating.Order))]
        public virtual Rating? Rating { get; set; }

        [InverseProperty(nameof(VoucherCode.Order))]
        public virtual ICollection<VoucherCode> VoucherCodes { get; set; }
        [InverseProperty(nameof(OrderDetail.Order))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty(nameof(WalletTransaction.Order))]
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }

        public Guid CreateBy { get; set; }
        [ForeignKey(nameof(CreateBy))]
        [InverseProperty(nameof(Buyer.Orders))]
        public virtual User? Buyer { get; set; }

        public Guid? PartnerTransactionId { get; set; }
        [ForeignKey(nameof(PartnerTransactionId))]
        [InverseProperty(nameof(PartnerTransaction.Orders))]
        public virtual PartnerTransaction? PartnerTransaction { get; set; }

        [Key]
        public string Id { get; set; }

        public int TotalPrice { get; set; }
        public int DiscountPrice { get; set; }
        public int UsedVPoint { get; set; }
        public int UsedBalance { get; set; }
        public int FinalPrice => TotalPrice - DiscountPrice - UsedVPoint - UsedBalance;
        public string? GiftEmail { get; set; }
        public int VPointUp => (int)Math.Ceiling((decimal)(TotalPrice + DiscountPrice + UsedVPoint) / 1000);
        public string? Note { get; set; }

        public required string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
    }
}
