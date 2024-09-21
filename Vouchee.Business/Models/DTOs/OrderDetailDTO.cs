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
    public class OrderDetailDTO
    {
        public Guid? orderId { get; set; }
        public Guid? voucherId { get; set; }

        public decimal unitPrice { get; set; }
        public decimal discountValue { get; set; }
        public decimal totalPrice => unitPrice * quantity;
        public decimal discountPrice => totalPrice * discountValue / 100;
        public decimal finalPrice => totalPrice - discountPrice; 
        public int quantity { get; set; }
    }

    public class CreateOrderDetailDTO : OrderDetailDTO
    {
        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateOrderDetailDTO : OrderDetailDTO
    {
        public ObjectStatusEnum? status { get; set; }

        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetOrderDetailDTO : OrderDetailDTO
    {
        public GetOrderDetailDTO()
        {
            voucherCodes = new HashSet<GetVoucherCodeDTO>();
        }

        public Guid id { get; }

        public virtual ICollection<GetVoucherCodeDTO> voucherCodes { get; set; }

        public string? status { get; }
        public DateTime? createDate { get; }
        public Guid? createBy { get; }
        public DateTime? updateDate { get; }
        public Guid? updateBy { get;  }
    }
}
