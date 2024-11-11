using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    public class CartDTO
    {
        public CartDTO()
        {
            sellers = [];
        }

        public Guid buyerId { get; set; }
        public int? balance { get; set; } = 0;
        public int? totalQuantity { get; set; } = 0;
        public int? totalPrice { get; set; } = 0;
        public int? discountPrice { get; set; } = 0;
        public int? finalPrice => totalPrice - discountPrice;
        public int? vPoint { get; set; } = 0;
        public int? appliedVPointPrice => finalPrice - vPoint;

        public virtual ICollection<SellerCartDTO>? sellers { get; set; }
    }

    public class SellerCartDTO
    {
        public SellerCartDTO()
        {
            modals = [];
        }

        public Guid? sellerId { get; set; }
        public string? sellerName { get; set; }
        public string? sellerImage { get; set; }

        public virtual ICollection<CartModalDTO> modals { get; set; }
    }
}
