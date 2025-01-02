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


        [JsonIgnore] public string status = ObjectStatusEnum.ACTIVE.ToString();
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateOrderDTO : OrderDTO
    {
        public List<string>? voucherCodes { get; set; }
    }

    public class GetOrderDTO : OrderDTO
    {
        public string? id { get; set; }

        public int? totalPrice { get; set; }
        public int? discountPrice { get; set; }
        public int? usedVPoint { get; set; }
        public int? usedBalance { get; set; }
        public int? finalPrice { get; set; }
        public int? VPointUp { get; set; }
        public DateTime? createDate { get; set; }
        public DateTime? exprireTime => createDate != null ? createDate.Value.AddMinutes(5) : null;
        public Guid? createBy { get; set; }
        public string? note { get; set; }
        public DateTime? updateDate { get; set; }
        public string? status { get; set; }
    }

    public class GetDetailOrderDTO : GetOrderDTO
    {
        public GetDetailOrderDTO()
        {
            orderDetails = new HashSet<GetOrderDetailDTO>();
        }

        public virtual ICollection<GetOrderDetailDTO>? orderDetails { get; set; }
    }
}