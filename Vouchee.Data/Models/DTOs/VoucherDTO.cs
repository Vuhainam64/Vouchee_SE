﻿using Microsoft.EntityFrameworkCore;
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
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? description { get; set; }

        public IList<string>? images { get; set; }

        public string? videoUrl { get; set; }

        public DateTime? createDate = DateTime.Now;
        public string? status = ObjectStatusEnum.ACTIVE.ToString();

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

    public class GetAllVoucherDTO
    {
        public GetAllVoucherDTO()
        {
            categories = [];
            medias = [];
            addresses = [];
            modals = [];
        }

        public Guid? id { get; set; }

        public string? title { get; set; }
        public string? description { get; set; }
        public string? image { get; set; }
        public Guid? brandId { get; set; }
        public string? brandName { get; set; }
        public string? brandImage { get; set; }
        public Guid? supplierId { get; set; }
        public string? supplierName { get; set; }
        public string? supplierImage { get; set; }
        public int? quantity { get; set; }
        public decimal? rating { get; set; }

        public virtual ICollection<GetAllAddressDTO> addresses { get; set; }
        public virtual ICollection<GetCategoryDTO>? categories { get; set; }
        public virtual ICollection<GetModalDTO>? modals { get; set; }
        public virtual ICollection<GetMediaDTO> medias { get; set; }
    }
    public class GetBestBuyVoucherDTO
    {
        public GetBestBuyVoucherDTO()
        {
            // addresses = new HashSet<GetAllAddressDTO>();
            categories = new HashSet<GetCategoryDTO>();
            medias = new HashSet<GetMediaDTO>();
        }
        public Guid? id { get; set; }

        public string? title { get; set; }
        //public string? description { get; set; }
        public string? image { get; set; }
        public decimal? originalPrice { get; set; }
        public decimal? sellPrice { get; set; }
        public decimal? salePrice { get; set; }
        public decimal? percentDiscount { get; set; }
        //public DateTime? starDate { get; set; }
        //public DateTime? endDate { get; set; }
        //public string? policy { get; set; }
        //public int? quantity { get; set; }
        public Guid? brandId { get; set; }
        public string? brandName { get; set; }
        public string? brandImage { get; set; }
        public Guid? supplierId { get; set; }
        public string? supplierName { get; set; }
        public string? supplierImage { get; set; }
        public int? quantity { get; set; }
        public decimal? rating { get; set; }
        //public Guid? voucherTypeId { get; set; }
        //public string? voucherTypeName { get; set; }

        //public string? status { get; set; }
        //public DateTime? createDate { get; set; }
        //public Guid? createBy { get; set; }
        //public DateTime? updateDate { get; set; }
        //public Guid? updateBy { get; set; }
        public decimal? TotalQuantitySold { get; set; }
        public virtual ICollection<GetCategoryDTO>? categories { get; set; }
        public virtual ICollection<GetMediaDTO>? medias { get; set; }
    }
    public class GetNearestVoucherDTO
    {
        public GetNearestVoucherDTO()
        {
            addresses = new HashSet<GetAllAddressDTO>();
            categories = new HashSet<GetCategoryDTO>();
            medias = new HashSet<GetMediaDTO>();
        }
        public Guid? id { get; set; }

        public string? title { get; set; }
        //public string? description { get; set; }
        public string? image { get; set; }
        public decimal? originalPrice { get; set; }
        public decimal? sellPrice { get; set; }
        public decimal? salePrice { get; set; }
        public decimal? percentDiscount { get; set; }
        //public DateTime? starDate { get; set; }
        //public DateTime? endDate { get; set; }
        //public string? policy { get; set; }
        //public int? quantity { get; set; }
        public Guid? brandId { get; set; }
        public string? brandName { get; set; }
        public string? brandImage { get; set; }
        public Guid? supplierId { get; set; }
        public string? supplierName { get; set; }
        public string? supplierImage { get; set; }
        public int? quantity { get; set; }
        public decimal? rating { get; set; }
        //public Guid? voucherTypeId { get; set; }
        //public string? voucherTypeName { get; set; }

        //public string? status { get; set; }
        //public DateTime? createDate { get; set; }
        //public Guid? createBy { get; set; }
        //public DateTime? updateDate { get; set; }
        //public Guid? updateBy { get; set; }
        //public string? distance { get; set; }
        public virtual ICollection<GetCategoryDTO>? categories { get; set; }
        public virtual ICollection<GetAllAddressDTO>? addresses { get; set; }
        //public Brand brand { get; set; }
        //public string? imageBrand { get; set; }
        public virtual ICollection<GetMediaDTO>? medias { get; set; }
    }
    public class GetDetailVoucherDTO
    {
        public GetDetailVoucherDTO()
        {
            voucherCodes = [];
            addresses = [];
            categories = [];
            medias = [];
            modals = [];
        }

        public Guid? id { get; set; }

        public string? title { get; set; }
        public string? description { get; set; }
        public string? image { get; set; }
        public decimal? originalPrice { get; set; }
        public decimal? sellPrice { get; set; }
        public decimal? salePrice { get; set; }
        public decimal? percentDiscount { get; set; }
        public int? quantity { get; set; }
        public decimal? rating { get; set; }

        public Guid? brandId { get; set; }
        public string? brandName { get; set; }
        public string? brandImage { get; set; }
        public Guid? supplierId { get; set; }
        public string? supplierName { get; set; }
        public string? supplierImage { get; set; }
        public Guid? createBy { get; set; }
        public string? sellerName { get; set; }

        public virtual ICollection<GetVoucherCodeDTO>? voucherCodes { get; set; }
        public virtual ICollection<GetModalDTO>? modals { get; set; }
        public virtual ICollection<GetCategoryDTO> categories { get; set; }
        public virtual ICollection<GetMediaDTO> medias { get; set; }
        public virtual ICollection<GetAllAddressDTO>? addresses { get; set; }
    }

    public class GetNewestVoucherDTO
    {
        public GetNewestVoucherDTO()
        {
            categories = new HashSet<GetCategoryDTO>();
            medias = new HashSet<GetMediaDTO>();
        }

        public Guid? id { get; set; }

        public string? title { get; set; }
        //public string? image { get; set; }
        public decimal? originalPrice { get; set; }
        public decimal? sellPrice { get; set; }
        public decimal? salePrice { get; set; }
        public decimal? percentDiscount { get; set; }
        public Guid? brandId { get; set; }
        public string? brandName { get; set; }
        public string? brandImage { get; set; }
        // Tạm thời sẽ trả ra hình ảnh đầu tiên
        public string? image { get; set; }
        public Guid? supplierId { get; set; }
        public string? supplierName { get; set; }
        public string? supplierImage { get; set; }
        public int? quantity { get; set; }
        public decimal? rating { get; set; }

        public DateTime? createDate { get; set; }

        public virtual ICollection<GetMediaDTO> medias { get; set; }
        public virtual ICollection<GetCategoryDTO>? categories { get; set; }
    }

    public class VoucherDTO
    {
        public Guid? id { get; set; }

        public string? title { get; set; }
        public decimal? originalPrice { get; set; }
        public decimal? sellPrice { get; set; }
        public decimal? salePrice { get; set; }
        public string? productImage { get; set; }
        public decimal? percentDiscount { get; set; }
    }

    public class CartModalDTO : VoucherDTO
    {
        public int quantity { get; set; }
    }
}
