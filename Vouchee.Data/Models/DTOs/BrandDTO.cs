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
        public CreateBrandDTO()
        {
            addresses = [];
        }

        [Required(ErrorMessage = "Tên là bắt buộc")]
        public string? name { get; set; }
        public string? image { get; set; }
        public bool? isVerified = false;
        public ObjectStatusEnum? status = ObjectStatusEnum.NONE;
        public DateTime? createDate = DateTime.Now;
        public bool? isActive = false;

        public IList<CreateAddressDTO> addresses { get; set; }
    }

    public class UpdateBrandDTO
    {
        [Required(ErrorMessage = "Tên là bắt buộc")]
        public string? name { get; set; }
        public string? image { get; set; }
        public DateTime? updateDate = DateTime.Now;
    }

    public class BrandDTO
    {
        public Guid? id { get; set; }

        public string? name { get; set; }
        public string? image { get; set; }
        public bool? isVerified { get; set; }
        public bool? isActive { get; set; }
        public string? status { get; set; }
    }

    public class GetDetalBrandDTO : BrandDTO
    {
        public GetDetalBrandDTO()
        {
            addresses = [];
        }

        public virtual ICollection<GetAddressDTO> addresses { get; set; }
    }

    public class GetBrandDTO : BrandDTO
    {

    }
}
