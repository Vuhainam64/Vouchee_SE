﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;


namespace Vouchee.Business.Models.DTOs
{
    public class OrderDetailDTO
    {
        public int? quantity { get; set; }
    }

    public class CreateOrderDetailDTO : OrderDetailDTO
    {
        public Guid promotionId { get; set; }
        public string status = OrderStatusEnum.PENDING.ToString();
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateOrderDetailDTO : OrderDetailDTO
    {
        public Guid? orderId { get; set; }
        public ObjectStatusEnum? status { get; set; }

        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetOrderDetailDTO : OrderDetailDTO
    {
        /*public GetOrderDetailDTO()
        {
            voucherCodes = new HashSet<GetVoucherCodeModalDTO>();
        }*/

        public string? orderId { get; set; }
        public Guid? modalId { get; set; }
        public string? image { get; set; }
        public string? brandId { get; set; }
        public string? brandName { get; set; }
        public string? brandImage { get; set; }

        public Guid? sellerId { get; set; }
        public string? sellerName { get; set; }

        public int? unitPrice { get; set; }
        public int? shopDiscountPercent { get; set; }
        public int? shopDiscountMoney { get; set; }
        public int? totalPrice => unitPrice * quantity;
        public int? discountPrice => (totalPrice * shopDiscountPercent / 100) + shopDiscountMoney;
        public int? finalPrice => totalPrice - discountPrice;

        public string? status { get; set; }
        public DateTime? createDate { get; set; }

        /*public virtual ICollection<GetVoucherCodeModalDTO>? voucherCodes { get; set; }*/
    }
}
