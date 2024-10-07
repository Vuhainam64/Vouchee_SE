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
    public class BrandDTO
    {
        [Required(ErrorMessage = "Tên là bắt buộc")]
        public string? name { get; set; }

        public string? description { get; set; }

        [Range(0, 100, ErrorMessage = "Phần trăm phải nằm trong khoảng từ 0 đến 100")]
        public decimal? percentShow { get; set; }
    }

    public class CreateBrandDTO : BrandDTO
    {
        public IFormFile? image { get; set; }
        public string? status = ObjectStatusEnum.ACTIVE.ToString();
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateBrandDTO : BrandDTO
    {

    }

    public class GetBrandDTO : BrandDTO
    {

    }
}
