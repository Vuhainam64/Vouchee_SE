using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class VoucherCodeDTO
    {
        public string? code { get; set; } = null;
        public string? image { get; set; } = null;

        public DateOnly? startDate { get; set; }
        public DateOnly? endDate { get; set; }
    }

    public class CreateVoucherCodeDTO : VoucherCodeDTO
    {
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

        public Guid? modalId { get; set; }
        public string? orderId { get; set; }
        public Guid? buyerId { get; set; }

        public string? status { get; set; }
    }

    public class UpdateCodeVoucherCodeDTO 
    {
        [Required]
        public string code { get; set; }
        [Required]
        public string newcode { get; set; }
    }
}
