using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Business.Models.DTOs
{
    public class VoucherCodeDTO
    {
        public Guid? VoucherId { get; set; }
        public Guid? OrderDetailId { get; set; }

        public string? Code { get; set; }
        public string? Image { get; set; }
    }

    public class CreateVoucherCodeDTO : VoucherCodeDTO
    {
        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateVoucherCodeDTO : VoucherCodeDTO
    {
        public ObjectStatusEnum? status { get; set; }
        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetVoucherCodeDTO : VoucherCodeDTO
    {
        public Guid id { get; }

        public string? status { get; }
        public DateTime? createDate { get; }
        public Guid? createBy { get; }
        public DateTime? updateDate { get; }
        public Guid? updateBy { get; }
    }
}
