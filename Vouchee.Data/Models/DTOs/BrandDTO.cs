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
    public class CreateBrandDTO
    {
        [Required(ErrorMessage = "Tên là bắt buộc")]
        public string? name { get; set; }

        public string? description { get; set; }

        [Range(0, 100, ErrorMessage = "Phần trăm phải nằm trong khoảng từ 0 đến 100")]
        public decimal? percentShow { get; set; }
        public IFormFile? image { get; set; }
        public string? status = ObjectStatusEnum.ACTIVE.ToString();
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateBrandDTO
    {

    }

    public class GetBrandDTO
    {
        public GetBrandDTO()
        {
            addresses = new HashSet<GetAddressDTO>();
        }

        public Guid? id { get; set; }

        public string? image { get; set; }
        public string? name { get; set; }
        //public bool? isVerfied { get; set; }

        //public string? status { get; set; }
        //[Column(TypeName = "datetime")]
        //public DateTime? createDate { get; set; }
        //public Guid? createBy { get; set; }
        //[Column(TypeName = "datetime")]
        //public DateTime? updateDate { get; set; }
        //public Guid? updateBy { get; set; }
        //public DateTime? verifiedDate { get; set; }
        //public Guid? verifiedBy { get; set; }

        public virtual ICollection<GetAddressDTO> addresses { get; set; }
    }
}
