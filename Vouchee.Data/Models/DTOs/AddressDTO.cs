using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class CreateAddressDTO
    {
        [Required(ErrorMessage = "Tên không được để trống.")]
        public string? name { get; set; }
        [Required(ErrorMessage = "Kinh độ không được để trống.")]
        [Range(-180, 180, ErrorMessage = "Kinh độ phải nằm trong khoảng từ -180 đến 180.")]
        public decimal? lon { get; set; }
        [Required(ErrorMessage = "Vĩ độ không được để trống.")]
        [Range(-90, 90, ErrorMessage = "Vĩ độ phải nằm trong khoảng từ -90 đến 90.")]
        public decimal? lat { get; set; }
        public bool isActive = true;
        public ObjectStatusEnum status = ObjectStatusEnum.NONE;
        public DateTime CreateDate = DateTime.Now;
    }

    public class UpdateAddressDTO
    {
        [Column(TypeName = "decimal")]
        public string? name { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lon { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lat { get; set; }
        public DateTime? updateDate = DateTime.Now;
    }

    public class AddressDTO
    {
        public Guid? id { get; set; }

        public string? name { get; set; }
        public decimal? lon { get; set; }
        public decimal? lat { get; set; }
        public bool? isActive { get; set; }
        public string? status { get; set; }
    }

    public class GetAddressDTO : AddressDTO
    {

    }

    public class GetDistanceAddressDTO : AddressDTO
    {
        public decimal distance;
    }

    public class GetDetailAddressDTO : AddressDTO
    {
        public GetDetailAddressDTO()
        {
            brands = [];
        }

        public virtual ICollection<GetBrandDTO> brands { get; set; }
    }
}