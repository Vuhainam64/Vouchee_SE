using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class VoucherDTO
    {

    }

    public class CreateVoucherDTO : VoucherDTO
    {

    }

    public class UpdateVoucherDTO : VoucherDTO
    {

    }

    public class GetVoucherDTO : VoucherDTO
    {
        public Guid? id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public decimal? price { get; set; }
        public DateTime? starDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? policy { get; set; }
        public int? quantity { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }

    public class VoucherFiler
    {
        public string? name { get; set; }
        public decimal? price { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public Guid? supplierId { get; set; }
        public Guid? voucherTypeId { get; set; }
        public string? status { get; set; }
    }
}
