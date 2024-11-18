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
        [Required(ErrorMessage = "Tên là cần thiết")]
        [MaxLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        public string? name { get; set; }
        [MaxLength(2000, ErrorMessage = "Tối đa 2000 kí tự")]
        public string? description { get; set; }
        public DateTime? startDate { get; set; }
        [DateGreaterThan(nameof(startDate), ErrorMessage = "Ngày kết thúc phải lớn hơn ngày bắt đầu")]
        public DateTime? endDate { get; set; }
        public string? image { get; set; }
        [Required(ErrorMessage = "Phần trăm giảm là cần thiết")]
        public int? percentDiscount { get; set; }
        public int? stock { get; set; }

        public bool isActive { get; set; }
        public string? status = ObjectStatusEnum.NONE.ToString();
    }

    //public class UpdateShopPromotionDTO
    //{
    //    [Required(ErrorMessage = "Name is required.")]
    //    [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
    //    public string? Name { get; set; }

    //    [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
    //    public string? Description { get; set; }

    //    [Required(ErrorMessage = "Start date is required.")]
    //    [DataType(DataType.Date)]
    //    [Display(Name = "Start Date")]
    //    public DateTime? StartDate { get; set; }

    //    [Required(ErrorMessage = "End date is required.")]
    //    [DataType(DataType.Date)]
    //    [Display(Name = "End Date")]
    //    [DateGreaterThan("StartDate", ErrorMessage = "End date must be greater than start date.")]
    //    public DateTime? EndDate { get; set; }

    //    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
    //    public int? Quantity { get; set; } = null;

    //    [StringLength(50, ErrorMessage = "Code can't be longer than 50 characters.")]
    //    public string? Code { get; set; } = null;

    //    [Required(ErrorMessage = "Promotion type is required.")]
    //    [Display(Name = "Promotion Type")]
    //    public PromotionTypeEnum? Type { get; set; }

    //    [StringLength(1000, ErrorMessage = "Policy can't be longer than 1000 characters.")]
    //    public string? Policy { get; set; }
    //    public DateTime updateDate = DateTime.Now;
    //}

    public class GetShopPromotionDTO
    {
        public Guid id { get; set; }

        public string? name { get; set; }
        public string? description { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? image { get; set; }
        public int? percentDiscount { get; set; }
        public int? stock { get; set; }
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