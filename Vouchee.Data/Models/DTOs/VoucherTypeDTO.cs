using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Business.Models.DTOs
{
    public class VoucherTypeDTO
    {
        public string? name { get; set; }
    }

    public class CreateVoucherTypeDTO : VoucherTypeDTO
    {
        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateVoucherTypeDTO : VoucherTypeDTO
    {
        public ObjectStatusEnum status { get; set; }
        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetVoucherTypeDTO : VoucherTypeDTO
    {
        public GetVoucherTypeDTO()
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
