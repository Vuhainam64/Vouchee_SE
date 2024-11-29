using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    public class CreateModalDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? title { get; set; }

        [Required(ErrorMessage = "Original price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Original price must be greater than zero")]
        public decimal originalPrice { get; set; }

        [Required(ErrorMessage = "Sell price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sell price must be greater than zero")]
        public decimal sellPrice { get; set; }

        public string? image { get; set; }

        public VoucherStatusEnum status = VoucherStatusEnum.NONE;

        public bool IsActive { get; set; }

        public DateTime createDate = DateTime.Now;
    }

    public class UpdateModalDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? title { get; set; }

        [Required(ErrorMessage = "Original price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Original price must be greater than zero")]
        public decimal originalPrice { get; set; }

        [Required(ErrorMessage = "Sell price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sell price must be greater than zero")]
        public decimal sellPrice { get; set; }

        public string? image { get; set; }

        public DateTime updateDate = DateTime.Now;
    }

    public class ModalDTO 
    {
        public Guid? id { get; set; }
        public Guid? voucherId { get; set; }
        public Guid? shopPromotionId { get; set; }
        public Guid? brandId { get; set; }
        public string? brandName { get; set; }
        public string? brandImage { get; set; }
        public string? title { get; set; }
        public int? originalPrice { get; set; }
        public int? sellPrice { get; set; }
        public int? shopDiscountMoney { get; set; } = 0;
        public int? shopDiscountPercent { get; set; } = 0;
        public int? discountPrice => (sellPrice * shopDiscountPercent / 100) + shopDiscountMoney;
        public int? salePrice => sellPrice - discountPrice;
        public string? image { get; set; }
        public int? index { get; set; }
        public DateOnly? startDate { get; set; }
        public DateOnly? endDate { get; set; }
        public int stock { get; set; } = 0;
        public string? status { get; set; }
        public bool? isActive { get; set; }
    }

    public class GetModalDTO : ModalDTO
    {
        
    }

    public class GetOrderedModalDTO
    {
        public GetOrderedModalDTO()
        {
            voucherCodes = [];
        }

        public Guid? id { get; set; }

        public string? title { get; set; }
        public string? image { get; set; }

        public int? voucherCodeCount { get; set; } = 0;

        public Guid? SellerId { get; set; }
        public string? SellerImage { get; set; }

        public virtual ICollection<GetVoucherCodeDTO> voucherCodes { get; set; }
    }

    //public class GetPendingModalDTO : ModalDTO
    //{
    //    public GetPendingModalDTO()
    //    {
    //        orderDetails = [];
    //    }

    //    public virtual ICollection<GetOrderDetailDTO> orderDetails { get; set; }
    //}

    public class GetDetailModalDTO : ModalDTO
    {
        public GetDetailModalDTO()
        {
            voucherCodes = [];
        }

        public virtual ICollection<GetVoucherCodeDTO>? voucherCodes { get; set; }
    }

    public class CartModalDTO : ModalDTO
    {
        public int quantity { get; set; }

        public int? totalUnitPrice => quantity * sellPrice;
        public int? totalDiscountPrice => quantity * discountPrice;
        public int? totalFinalPrice => totalUnitPrice - totalDiscountPrice;
    }
}
