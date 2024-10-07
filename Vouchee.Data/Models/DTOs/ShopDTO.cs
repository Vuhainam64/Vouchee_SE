using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class ShopDTO
    {
        public string? name { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? percentShow { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lat { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? lon { get; set; }
        public string? address { get; set; }
    }

    public class CreateShopDTO : ShopDTO
    {
        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateShopDTO : ShopDTO
    {
        [JsonIgnore]
        public string? status { get; set; }
        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetShopDTO : ShopDTO
    {
        public GetShopDTO()
        {
            vouchers = new HashSet<GetVoucherDTO>();
        }

        public Guid? id { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<GetVoucherDTO>? vouchers { get; set; }
    }
}
