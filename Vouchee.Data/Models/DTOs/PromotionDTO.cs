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

namespace Vouchee.Data.Models.DTOs
{
    public class PromotionDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        [DateGreaterThan("StartDate", ErrorMessage = "End date must be greater than start date.")]
        public DateTime? EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int? Quantity { get; set; } = null;

        [StringLength(50, ErrorMessage = "Code can't be longer than 50 characters.")]
        public string? Code { get; set; } = null;

        [Required(ErrorMessage = "Promotion type is required.")]
        [Display(Name = "Promotion Type")]
        public PromotionTypeEnum? Type { get; set; }

        [StringLength(1000, ErrorMessage = "Policy can't be longer than 1000 characters.")]
        public string? Policy { get; set; }
    }

    public class CreatePromotionDTO : PromotionDTO
    {
        public CreatePromotionDTO()
        {
            voucherCodeId = new List<Guid>();
        }

        public IFormFile? image { get; set; }
        public DateTime createDate = DateTime.Now;
        public virtual IList<Guid> voucherCodeId { get; set; }
    }

    public class UpdatePromotionDTO : PromotionDTO
    {
        public DateTime updateDate = DateTime.Now;
    }

    public class GetPromotionDTO : PromotionDTO
    {
        public GetPromotionDTO()
        {
            vouchers = new HashSet<GetAllVoucherDTO>();
            orders = new HashSet<GetOrderDTO>();
            orderDetails = new HashSet<GetOrderDetailDTO>();
        }

        public Guid? id { get; set; }

        public string? status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<GetAllVoucherDTO>? vouchers { get; set; }
        public virtual ICollection<GetOrderDTO>? orders { get; set; }
        public virtual ICollection<GetOrderDetailDTO>? orderDetails { get; set; }
    }
}