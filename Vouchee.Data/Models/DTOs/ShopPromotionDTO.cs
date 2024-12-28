using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Entities;
using Vouchee.Business.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Vouchee.Data.Helpers.DateAnnotations;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.DTOs
{
    public class CreateShopPromotionDTO
    {
        [Required(ErrorMessage = "Tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự.")]
        public string? name { get; set; }
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? description { get; set; }
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải nằm trong khoảng từ 0 đến 100.")]
        public int? percentDiscount { get; set; }
        //[Range(0, int.MaxValue, ErrorMessage = "Số tiền giảm giá phải lớn hơn hoặc bằng 0.")]
        //public int? moneyDiscount { get; set; }
        //[Range(1, int.MaxValue, ErrorMessage = "Số lượng yêu cầu phải lớn hơn 0.")]
        //public int? requiredQuantity { get; set; }
        //[Range(0, int.MaxValue, ErrorMessage = "Số tiền tối đa được giảm phải lớn hơn hoặc bằng 0.")]
        //public int? maxMoneyToDiscount { get; set; }
        //[Range(0, int.MaxValue, ErrorMessage = "Số tiền tối thiểu để áp dụng phải lớn hơn hoặc bằng 0.")]
        //public int? minMoneyToApply { get; set; }
        //[DataType(DataType.Date, ErrorMessage = "Ngày bắt đầu không hợp lệ.")]
        //public DateTime? startDate { get; set; }
        //[DataType(DataType.Date, ErrorMessage = "Ngày kết thúc không hợp lệ.")]
        //[DateGreaterThan("startDate", ErrorMessage = "Ngày kết thúc phải lớn hơn ngày bắt đầu.")]
        //public DateTime? endDate { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho phải lớn hơn hoặc bằng 0.")]
        public int? stock { get; set; }
        //[Url(ErrorMessage = "Hình ảnh phải là URL hợp lệ.")]
        //public string? image { get; set; }
        public bool isActive { get; set; } = true;
        public string? status = ObjectStatusEnum.NONE.ToString();
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateShopPromotionDTO
    {
        [Required(ErrorMessage = "Tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự.")]
        public string? name { get; set; }
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? description { get; set; }
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải nằm trong khoảng từ 0 đến 100.")]
        public int? percentDiscount { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho phải lớn hơn hoặc bằng 0.")]
        public int? stock { get; set; }
        public DateTime? updateDate = DateTime.Now;
    }

    public class GetShopPromotionDTO
    {
        public Guid? id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public int? percentDiscount { get; set; }
        //public int? moneyDiscount { get; set; }
        //public int? requiredQuantity { get; set; }
        //public int? maxMoneyToDiscount { get; set; }
        //public int? minMoneyToApply { get; set; }
        //public DateTime? startDate { get; set; }
        //public DateTime? endDate { get; set; }
        public int? stock { get; set; }
        //public string? image { get; set; }
        //public string type = "SHOP";

        public bool? isActive { get; set; }
        public string? status { get; set; }
    }

    //    public class GetDetailShopPromotionDTO
    //    {
    //        public GetDetailShopPromotionDTO()
    //        {
    //        }

    //        public Guid? id { get; set; }

    //        public string? name { get; set; }
    //        public string? description { get; set; }
    //        public DateTime? startDate { get; set; }
    //        public DateTime? endDate { get; set; }
    //        public int? quantity { get; set; } = null;
    //        public string? code { get; set; } = null;
    //        public PromotionTypeEnum? type { get; set; }
    //        public string? policy { get; set; }
    //        public string? image { get; set; }
    //        public decimal? percentDiscount { get; set; }
    //        public decimal? moneyDiscount { get; set; }

    //        public DateTime? createDate { get; set; }
    //        public Guid? createBy { get; set; }
    //    }
    //}

    //    public class ShopPromotion
    //    {
    //        public ShopPromotion()
    //        {
    //            OrderDetails = [];
    //        }

    //        [InverseProperty(nameof(OrderDetail.ShopPromotion))]
    //        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

    //        public Guid? SellerId { get; set; }
    //        [ForeignKey(nameof(SellerId))]
    //        [InverseProperty(nameof(Seller.ShopPromotions))]
    //        public required virtual User? Seller { get; set; }

    //        [Key]
    //        public Guid Id { get; set; }

    //        public required string Name { get; set; }
    //        public string? Description { get; set; }
    //        [Column(TypeName = "datetime")]
    //        public DateTime? StartDate { get; set; }
    //        [Column(TypeName = "datetime")]
    //        public DateTime? EndDate { get; set; }
    //        public string? Image { get; set; }
    //        public int? PercentDiscount { get; set; }

    //        public required string Status { get; set; }
    //        [Column(TypeName = "datetime")]
    //        public DateTime? CreateDate { get; set; } = DateTime.Now;
    //        public Guid? CreateBy { get; set; }
    //        [Column(TypeName = "datetime")]
    //        public DateTime? UpdateDate { get; set; }
    //        public Guid? UpdateBy { get; set; }
    //    }
    //}

}