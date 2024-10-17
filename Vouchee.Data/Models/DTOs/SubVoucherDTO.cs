using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.DTOs
{
    public class CreateSubVoucherDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? title { get; set; }

        [Required(ErrorMessage = "Original price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Original price must be greater than zero")]
        public decimal originalPrice { get; set; }

        [Required(ErrorMessage = "Sell price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sell price must be greater than zero")]
        public decimal sellPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value")]
        public int? quantity { get; set; }

        public VoucherStatusEnum status { get; set; }

        public IList<IFormFile>? productImages { get; set; }

        public DateTime createDate = DateTime.Now;
    }

    public class UpdateSubVoucherDTO : CreateSubVoucherDTO
    {

    }

    public class GetSubVoucherDTO
    {

    }
}
