using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class VoucherCodeDTO
    {
        public string? code { get; set; }
    }

    public class CreateVoucherCodeDTO : VoucherCodeDTO
    {
        public IFormFile? image { get; set; }
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateVoucherCodeDTO : VoucherCodeDTO
    {
        public ObjectStatusEnum? status { get; set; }
        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetVoucherCodeDTO : VoucherCodeDTO
    {
        public Guid id { get; set; }

        public Guid? voucherId { get; set; }
        public Guid? orderDetailId { get; set; }

        public string? image { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? updateBy { get; set; }
    }
}
