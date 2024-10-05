using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class OrderDTO
    {
    }

    public class CreateOrderDTO : OrderDTO
    {
        public CreateOrderDTO()
        {
            orderDetails = new HashSet<CreateOrderDetailDTO>();
        }

        [JsonIgnore] public string status = ObjectStatusEnum.ACTIVE.ToString();
        public DateTime? createDate = DateTime.Now;
        public Guid promotionId { get; set; }
        public PaymentTypeEnum paymentType { get; set; }
        public virtual ICollection<CreateOrderDetailDTO> orderDetails { get; set; }
    }

    public class UpdateOrderDTO : OrderDTO
    {
        //public OrderStatusEnum? status { get; set; }
        //public DateTime? updateDate = DateTime.Now;
        //public Guid? updateBy { get; set; }
        public List<string>? voucherCodes { get; set; }
    }

    public class GetOrderDTO : OrderDTO
    {
        public GetOrderDTO()
        {
            orderDetails = new HashSet<GetOrderDetailDTO>();
        }

        public Guid? id { get; set; }
        public Guid? promotionId { get; set; }

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

        public virtual ICollection<GetOrderDetailDTO>? orderDetails { get; set; }
    }
}