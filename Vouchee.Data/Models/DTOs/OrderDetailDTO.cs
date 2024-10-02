using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Vouchee.Data.Models.Constants.Enum.Status;


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
