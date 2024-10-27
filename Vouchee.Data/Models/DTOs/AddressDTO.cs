using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class CreateAddressDTO
    {
        public string? name { get; set; }
        public decimal? lon { get; set; }
        public decimal? lat { get; set; }
        public decimal? percentShow { get; set; }
        public IFormFile? image { get; set; }
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateAddressDTO
    {
        [Column(TypeName = "decimal")]
        public string? name { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lon { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lat { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? percentShow { get; set; }
        public DateTime? updateDate = DateTime.Now;
    }

    public class AddressDTO
    {
        public Guid? id { get; set; }

        public string? name { get; set; }
        public decimal? lon { get; set; }
        public decimal? lat { get; set; }
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