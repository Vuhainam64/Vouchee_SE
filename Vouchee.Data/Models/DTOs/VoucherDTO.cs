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

namespace Vouchee.Business.Models.DTOs
{
    public class VoucherDTO
    {
        public Guid? supplierId { get; set; }
        public Guid? voucherTypeId { get; set; }

        public string? name { get; set; }
        public string? description { get; set; }
        public decimal? price { get; set; }
        public DateTime? starDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? policy { get; set; }
        public int? quantity { get; set; }
    }

    public class CreateVoucherDTO : VoucherDTO
    {
        public IFormFile? image { get; set; }

        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateVoucherDTO : VoucherDTO
    {
        public VoucherStatusEnum? status { get; set; }
        public IFormFile? image { get; set; }

        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetVoucherDTO : VoucherDTO
    {
        public GetVoucherDTO()
        {
            voucherCodes = new HashSet<GetVoucherCodeDTO>();
            orderDetails = new HashSet<GetOrderDetailDTO>();
            shops = new HashSet<GetShopDTO>();
        }

        public Guid? id { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<GetVoucherCodeDTO>? voucherCodes { get; set; }
        public virtual ICollection<GetOrderDetailDTO>? orderDetails { get; set; }
        public virtual ICollection<GetShopDTO>? shops { get; set; }
    }
}
