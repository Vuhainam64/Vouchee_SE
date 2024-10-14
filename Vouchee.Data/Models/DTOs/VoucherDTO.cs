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
        public Guid? brandId { get; set; }
        public Guid? supplierId { get; set; }
        public Guid? voucherTypeId { get; set; }
        public Guid? addressId { get; set; }
        public Guid? categoryId { get; set; }

        public string? name { get; set; }
        public string? description { get; set; }
        public decimal? price { get; set; }
        public DateTime? starDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? policy { get; set; }
        public int? quantity { get; set; }
        public IFormFile? image { get; set; }
        public DateTime? createDate = DateTime.Now;
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
            addresses = new HashSet<GetAllAddressDTO>();
            categories = new HashSet<GetCategoryDTO>();
        }
        public Guid? id { get; set; }

        public string? title { get; set; }
        //public string? description { get; set; }
        public string? image { get; set; }
        public decimal? originalPrice { get; set; }
        public decimal? salePrice { get; set; }
        //public DateTime? starDate { get; set; }
        //public DateTime? endDate { get; set; }
        //public string? policy { get; set; }
        //public int? quantity { get; set; }
        //public Guid? brandId { get; set; }
        //public string? brandName { get; set; }
        //public string? brandImage { get; set; }
        //public Guid? supplierId { get; set; }
        //public string? supplierName { get; set; }
        //public string? supplierImage { get; set; }
        //public Guid? voucherTypeId { get; set; }
        //public string? voucherTypeName { get; set; }

        //public string? status { get; set; }
        //public DateTime? createDate { get; set; }
        //public Guid? createBy { get; set; }
        //public DateTime? updateDate { get; set; }
        //public Guid? updateBy { get; set; }

        public virtual ICollection<GetCategoryDTO>? categories { get; set; }
        public virtual ICollection<GetAllAddressDTO>? addresses { get; set; }
    }

    public class GetDetailVoucherDTO
    {
        public GetDetailVoucherDTO()
        {
            voucherCodes = new HashSet<GetVoucherCodeDTO>();
            addresses = new HashSet<GetAllAddressDTO>();
        }

        public Guid? id { get; set; }

        public string? name { get; set; }
        public string? description { get; set; }
        public string? image { get; set; }
        public decimal? price { get; set; }
        public DateTime? starDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? policy { get; set; }
        public int? quantity { get; set; }

        public Guid? brandId { get; set; }
        public string? brandName { get; set; }
        public string? brandImage { get; set; }
        public Guid? supplierId { get; set; }
        public string? supplierName { get; set; }
        public string? supplierImage { get; set; }
        public Guid? voucherTypeId { get; set; }
        public string? voucherTypeName { get; set; }

        public virtual ICollection<GetAllAddressDTO>? addresses { get; set; }
        public virtual ICollection<GetVoucherCodeDTO>? voucherCodes { get; set; }
    }
}
