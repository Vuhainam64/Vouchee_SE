using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;


namespace Vouchee.Business.Models.DTOs
{
    public class OrderDetailDTO
    {
        public int? quantity { get; set; }
    }

    public class CreateOrderDetailDTO : OrderDetailDTO
    {
        public Guid promotionId { get; set; }
        public string status = OrderStatusEnum.PENDING.ToString();
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateOrderDetailDTO : OrderDetailDTO
    {
        public Guid? orderId { get; set; }
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

        public Guid? id { get; set; }
        public Guid? orderId { get; set; }
        public Guid? modalId { get; set; }

        public int? unitPrice { get; set; }
        public int? discountValue { get; set; }
        public int? totalPrice { get; set; }
        public int? discountPrice { get; set; }
        public int? finalPrice { get; set; }

        public string? status { get; set; }

        public virtual ICollection<GetVoucherCodeDTO>? voucherCodes { get; set; }
    }
}
