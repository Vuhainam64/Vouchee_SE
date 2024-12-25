using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class VoucherCodeDTO
    {
        [Required(ErrorMessage = "Voucher code là bắt buộc")]
        public string? code { get; set; } 
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
        public string name { get; set; }
        public string modalname { get; set; }
        public string brand { get; set; }
        public Guid? modalId { get; set; }
        public string? orderId { get; set; }
        public Guid? buyerId { get; set; }

        public string? status { get; set; }
        public string? newCode { get; set; }

        public DateTime? updateDate { get; set; }
        public Guid? UpdateId { get; set; }
        public string? Comment { get; set; }

    }
    public class GetVoucherCodechangeStatusDTO : VoucherCodeDTO
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string modalname { get; set; }
        public string brand { get; set; }
        public Guid? modalId { get; set; }
        public string? status { get; set; }
        public Guid? UpdateId { get; set; }
        public string? Comment { get; set; }
    }

    public class GetVoucherCodeModalDTO : VoucherCodeDTO
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string modalname { get; set; }
        public string brand { get; set; }
        public Guid? modalId { get; set; }
        public string? orderId { get; set; }
        public Guid? buyerId { get; set; }

        public string? status { get; set; }

        public DateTime? updateDate { get; set; }
        public Guid? UpdateId { get; set; }
        public string? Comment { get; set; }

    }
    public class UpdateCodeVoucherCodeDTO 
    {
        [Required]
        public Guid id { get; set; }
        
        public string? newcode { get; set; }

        public string? Comment { get; set; }

        public UpdateStatusEnum? UpdateStatus { get; set; }
    }
    public class UpdateVoucherCodeStatusDTO
    {
        [Required]
        public VoucherCodeStatusEnum status { get; set; }
        [Required]
        public Guid id { get; set; }
    }

    public class GroupedVoucherCodeDTO
    {
        public Guid? UpdateId { get; set; }
        public int Count { get; set; } // Number of items in the group

        public DateTime? UpdateTime { get; set; }

        public string Status { get; set; }
        public GetVoucherCodeDTO FirstItem { get; set; } // Example of holding one representative item
    }

}
