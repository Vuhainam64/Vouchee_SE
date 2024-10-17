using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "Cần title")]
        public string? title { get; set; }

        [Range(0, 100, ErrorMessage = "PersentShow cần từ 0 tới 100")]
        public decimal? percentShow { get; set; }
        public IFormFile? image { get; set; }
        public DateTime createDate = DateTime.Now;
        public string status = ObjectStatusEnum.ACTIVE.ToString();
    }

    public class UpdateCategoryDTO 
    {

    }

    public class GetCategoryDTO
    {
        public Guid? id { get; set; }

        //public Guid? voucherTypeId { get; set; }

        public string? title { get; set; }
        //public decimal? percentShow { get; set; }
        public string? image { get; set; }

        //public string? status { get; set; }
        //public DateTime? createDate { get; set; }
        //public Guid? createBy { get; set; }
        //public DateTime? updateDate { get; set; }
        //public Guid? updateBy { get; set; }
    }
}
