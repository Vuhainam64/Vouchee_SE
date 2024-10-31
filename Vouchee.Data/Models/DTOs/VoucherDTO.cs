using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Entities;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using Vouchee.Data.Models.Constants.Enum;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Models.DTOs
{
    public class CreateVoucherDTO
    {
        public CreateVoucherDTO()
        {
            modals = new HashSet<CreateModalDTO>();
        }

        [Required(ErrorMessage = "Brand is required.")]
        public Guid? brandId { get; set; }

        [Required(ErrorMessage = "Supplier is required.")]
        public Guid? supplierId { get; set; }

        [Required(ErrorMessage = "At least one category is required.")]
        public IList<Guid>? categoryId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(10000, ErrorMessage = "Description cannot exceed 10000 characters.")]
        public string? description { get; set; }

        public IList<string>? images { get; set; }

        public string? video { get; set; }

        public DateTime? createDate = DateTime.Now;
        public VoucherStatusEnum status { get; set; }

        public virtual ICollection<CreateModalDTO> modals { get; set; }
    }

    public class UpdateVoucherDTO
    {
        public Guid? brandId { get; set; }
        public Guid? supplierId { get; set; }
        public Guid? voucherTypeId { get; set; }
        public Guid? shopId { get; set; }

        public string? name { get; set; }
        public string? description { get; set; }
        public decimal? price { get; set; }
        public DateTime? starDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? policy { get; set; }
        public int? quantity { get; set; }
        public VoucherStatusEnum? status { get; set; }
        public IFormFile? image { get; set; }

        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class VoucherDTO
    {
        public VoucherDTO()
        {
            categories = [];
            modals = [];
            medias = [];
            promotions = [];
        }

        public Guid? id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public decimal? rating { get; set; }
        public string? video { get; set; }
        public int stock { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? sellerId { get; set; }

        public Guid? promotionId { get; set; }
        public decimal? percentDiscount { get; set; }
        public decimal? originalPrice => modals.FirstOrDefault(m => m.index == 0)?.originalPrice;
        public decimal? sellPrice => modals.FirstOrDefault(m => m.index == 0)?.sellPrice;
        public decimal? salePrice => sellPrice.HasValue && percentDiscount.HasValue ? sellPrice * (1 - percentDiscount / 100) : null;

        public string? image => modals.FirstOrDefault(m => m.index == 0)?.image;

        public Guid? brandId { get; set; }
        public string? brandName { get; set; }
        public string? brandImage { get; set; }

        public virtual ICollection<GetPromotionDTO> promotions { get; set; }
        public virtual ICollection<GetMediaDTO> medias { get; set; }
        public virtual ICollection<GetCategoryDTO> categories { get; set; }
        public virtual ICollection<GetModalDTO> modals { get; set; }
    }

    public class GetVoucherDTO : VoucherDTO
    {

    }

    public class GetNearestVoucherDTO : VoucherDTO
    {
        public GetNearestVoucherDTO()
        {
            addresses = [];
        }

        public virtual IEnumerable<GetDistanceAddressDTO> addresses { get; set; }
    }

    public class GetDetailVoucherDTO : VoucherDTO
    {
        public GetDetailVoucherDTO()
        {
            addresses = [];
        }

        public string? sellerName { get; set; }
        public string? sellerImage { get; set; }

        public Guid? supplierId { get; set; }
        public string? supplierName { get; set; }
        public string? supplierImage { get; set; }

        public virtual IEnumerable<GetAddressDTO> addresses { get; set; }
    }

    public class GetBestSoldVoucherDTO : VoucherDTO
    {
        public int? totalQuantitySold { get; set; }
    }


    public class CartVoucherDTO : VoucherDTO
    {
        public int quantity { get; set; }
    }
}
