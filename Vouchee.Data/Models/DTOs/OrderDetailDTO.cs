using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;


namespace Vouchee.Business.Models.DTOs
{
    public class OrderDetailDTO
    {
        public Guid? voucherId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int quantity { get; set; }
    }

    public class CreateOrderDetailDTO : OrderDetailDTO
    {
        //[JsonIgnore] public decimal UnitPrice { get; set; }
        //[JsonIgnore] public decimal DiscountValue { get; set; } = 0;
        //[JsonIgnore] public decimal TotalPrice { get; set; }
        //public decimal DiscountPrice => TotalPrice * DiscountValue / 100;
        //public decimal FinalPrice => TotalPrice - DiscountPrice;
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

        public decimal? unitPrice { get; set; }
        public decimal? discountValue { get; set; }
        public decimal? totalPrice { get; set; }
        public decimal? discountPrice { get; set; }
        public decimal? finalPrice { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<GetVoucherCodeDTO>? voucherCodes { get; set; }
    }
}
