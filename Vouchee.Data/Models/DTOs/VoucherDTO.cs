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
    public class MiniVoucher
    {
        public Guid? id { get; set; }
        public string? title { get; set; }

        public string? image { get; set; }
    }

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

        public int stock = 0;

        public VoucherStatusEnum status = VoucherStatusEnum.NONE;

        public bool IsActive { get; set; }

        public virtual ICollection<CreateModalDTO> modals { get; set; }
    }

    public class UpdateVoucherDTO
    {
        public UpdateVoucherDTO()
        {
            categoryId = [];
            images = [];
        }

        [Required(ErrorMessage = "Brand is required.")]
        public Guid brandId { get; set; }

        [Required(ErrorMessage = "Supplier is required.")]
        public Guid supplierId { get; set; }

        [Required(ErrorMessage = "At least one category is required.")]
        public IList<Guid> categoryId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(10000, ErrorMessage = "Description cannot exceed 10000 characters.")]
        public string? description { get; set; }

        public IList<string> images { get; set; }

        public string? video { get; set; }

        public bool IsActive { get; set; }

        public DateTime updateDate = DateTime.Now;
    }

    public class VoucherDTO
    {
        public VoucherDTO()
        {
            categories = [];
            modals = [];
        }

        public Guid? id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public string? video { get; set; }
        public int stock => modals.Sum(x => x.stock);
        public DateTime? createDate { get; set; }

        public int? shopDiscount { get; set; }
        public int? originalPrice { get; set; }
        public int? sellPrice { get; set; }
        public int? salePrice => sellPrice - ((sellPrice * shopDiscount) / 100);
        public int? totalQuantitySold { get; set; }
        public decimal? averageRating { get; set; } = 0;

        public string? image { get; set; }

        public Guid? brandId { get; set; }
        public string? brandName { get; set; }
        public string? brandImage { get; set; }

        public Guid? supplierId { get; set; }
        public string? supplierName { get; set; }
        public string? supplierImage { get; set; }

        public Guid? sellerId { get; set; }
        public string? sellerName { get; set; }
        public string? sellerImage { get; set; }

        public string? status { get; set; }
        public bool? isActive { get; set; }

        public virtual ICollection<GetCategoryDTO> categories { get; set; }
        public virtual ICollection<GetModalDTO> modals { get; set; }
    }

    public class GetVoucherDTO : VoucherDTO
    {
        
    }

    public class GetVoucherSellerDTO : VoucherDTO
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
            medias = [];
        }

        public virtual IEnumerable<GetAddressDTO> addresses { get; set; }
        public virtual ICollection<GetMediaDTO> medias { get; set; }
    }
}
