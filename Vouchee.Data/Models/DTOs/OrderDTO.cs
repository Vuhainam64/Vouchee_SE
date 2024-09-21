using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Business.Models.DTOs
{
    public class OrderDTO
    {
        public Guid? userId { get; set; }

        public string? paymentType { get; set; }
        public decimal? discountValue { get; set; }
        public decimal? totalPrice { get; set; }
        public decimal? discountPrice { get; set; }
        public decimal? finalPrice => totalPrice - discountPrice;
    }

    public class CreateOrderDTO : OrderDTO
    {
        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateOrderDTO : OrderDTO
    {
        public OrderStatusEnum? status { get; set; }
        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetOrderDTO : OrderDTO
    {
        public GetOrderDTO()
        {
            orderDetails = new HashSet<GetOrderDetailDTO>();
        }

        public Guid? id { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<GetOrderDetailDTO>? orderDetails { get; set; }
    }
}