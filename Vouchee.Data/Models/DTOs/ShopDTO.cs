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
    public class CreateShopDTO
    {
        [Column(TypeName = "decimal")]
        public string? addressName { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lon { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lat { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? percentShow { get; set; }
        public IFormFile? image { get; set; }
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateShopDTO 
    {
        [Column(TypeName = "decimal")]
        public string? addressName { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lon { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lat { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? percentShow { get; set; }
        public DateTime? updateDate = DateTime.Now;
    }

    public class GetShopDTO
    {
        public GetShopDTO()
        {
            vouchers = new HashSet<GetVoucherDTO>();
            addresses = new HashSet<GetAddressDTO>();
        }

        public Guid? id { get; set; }

        public string? image { get; set; }
        [Column(TypeName = "decimal")]
        public string? addressName { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lon { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lat { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? percentShow { get; set; }
        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<GetVoucherDTO>? vouchers { get; set; }
        public virtual ICollection<GetAddressDTO>? addresses { get; set; }
    }
}