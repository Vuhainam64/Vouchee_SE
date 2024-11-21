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
        public int? vPoint { get; set; } = 0;

        public virtual ICollection<SellerCartDTO>? sellers { get; set; }
    }

    public class DetailCartDTO : CartDTO
    {
        public int? totalPrice => sellers.Sum(x => x.modals.Sum(x => x.totalUnitPrice));
        public int? shopDiscountPrice => sellers.Sum(x => x.modals.Sum(x => x.totalDiscountPrice));
        public int? useVPoint { get; set;} = 0;
        public int? useBalance { get; set; } = 0;
        public int? finalPrice => totalPrice - shopDiscountPrice - useVPoint - useBalance;
        public int? vPointUp => (int?)Math.Ceiling((decimal)(totalPrice + shopDiscountPrice + useVPoint) / 1000);
        public string? giftEmail { get; set; }
    }

    public class SellerCartDTO
    {
        public SellerCartDTO()
        {
            //promotions = [];
            modals = [];
        }

        public Guid? sellerId { get; set; }
        public string? sellerName { get; set; }
        public string? sellerImage { get; set; }
        public GetShopPromotionDTO? appliedPromotion { get; set; }

        //public virtual ICollection<GetShopPromotionDTO> promotions { get; set; }
        public virtual ICollection<CartModalDTO> modals { get; set; }
    }
}
