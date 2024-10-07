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
    public class CategoryDTO
    {
        [Required(ErrorMessage = "Cần title")]
        public string? title { get; set; }

        [Required(ErrorMessage = "Cân value")]
        public string? value { get; set; }

        [Required(ErrorMessage = "Cần key")]
        public string? key { get; set; }

        [Range(0, 100, ErrorMessage = "PersentShow cần từ 0 tới 100")]
        public decimal? percentShow { get; set; }
    }

    public class CreateCategoryDTO : CategoryDTO
    {
        public DateTime createDate = DateTime.Now;
        public string status = ObjectStatusEnum.ACTIVE.ToString();
    }

    public class UpdateCategoryDTO : CategoryDTO
    {

    }

    public class GetCategoryDTO : CategoryDTO
    {

    }
}
