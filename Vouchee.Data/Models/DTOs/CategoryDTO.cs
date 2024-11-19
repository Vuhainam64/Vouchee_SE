using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "Cần title")]
        public string? title { get; set; }
        public DateTime createDate = DateTime.Now;
        [Required(ErrorMessage = "Cần hình ảnh")]
        public string? image { get; set; }
    }

    public class UpdateCategoryDTO 
    {
        [Required(ErrorMessage = "Cần title")]
        public string? title { get; set; }
        public string? image { get; set; }
        public DateTime updateDate = DateTime.Now;
    }

    public class CategoryDTO
    {
        public Guid? id { get; set; }

        public Guid? voucherTypeId { get; set; }
        public string? voucherTypeTitle { get; set; }

        public string? title { get; set; }
        public string? image { get; set; }
    }

    public class GetCategoryDTO : CategoryDTO
    {

    }

    public class GetDetailCategoryDTO : CategoryDTO
    {
        public GetDetailCategoryDTO()
        {
            vouchers = [];
        }

        public virtual ICollection<GetVoucherDTO> vouchers { get; set; }
    }
}
