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

    public class GetAllAddressDTO
    {
        public GetAllAddressDTO()
        {
            // vouchers = new HashSet<GetAllVoucherDTO>();
            // addresses = new HashSet<GetAddressDTO>();
        }
        public decimal distance;
        public Guid? id { get; set; }
        public string? name { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? lon { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lat { get; set; }
    }


    public class GetDetailAddressDTO
    {

    }

    public class GetAddressDTO
    {
        public Guid? id { get; set; }

        public string? name { get; set; }
        public decimal? lon { get; set; }
        public decimal? lat { get; set; }
    }

    public class GetAddressBrandDTO
    {
        public GetAddressBrandDTO()
        {
            brands = [];
        }

        public Guid? id { get; set; }

        public string? name { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lon { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lat { get; set; }

        public virtual ICollection<GetBrandAddressDTO> brands { get; set; }
    }
}