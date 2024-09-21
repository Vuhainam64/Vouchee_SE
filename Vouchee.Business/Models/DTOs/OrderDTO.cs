using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class OrderDTO
    {
        public Guid? userId { get; set; }

        public string? paymentType { get; set; }
        public decimal discountValue { get; set; }
        public decimal totalPrice { get; set; }
        public decimal discountPrice { get; set; }
        public decimal finalPrice { get; set; }
    }

    public class CreateOrderDTO : OrderDTO
    {
        [JsonIgnore]
        public OrderEnum? status { get; set; }

        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateOrderDTO : OrderDTO
    {
        public OrderEnum? status { get; set; }

        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetOrderDTO : OrderDTO
    {
        public GetOrderDTO()
        {
            orderDetails = new HashSet<GetOrderDetailDTO>();
        }

        public Guid id { get; set; }

        public virtual ICollection<GetOrderDetailDTO> orderDetails { get; set; }


        public string? status { get; set; }
        public DateTime? createDate { get; }
        public Guid? createBy { get; }
        public DateTime? updateDate { get; }
        public Guid? updateBy { get; }
    }

    public class OrderFilter
    {
        public string? paymentType { get; set; }
        public decimal? discountValue { get; set; }
        public decimal? totalPrice { get; set; }
        public decimal? discountPrice { get; set; }
        public decimal? finalPrice { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }
}