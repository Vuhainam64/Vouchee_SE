using Vouchee.Data.Models.Constants.Enum.Status;


namespace Vouchee.Business.Models.DTOs
{
    public class OrderDetailDTO
    {
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
        //public DateTime? createDate = DateTime.Now;
        //public Guid? createBy { get; set; }
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

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<GetVoucherCodeDTO>? voucherCodes { get; set; }
    }
}
