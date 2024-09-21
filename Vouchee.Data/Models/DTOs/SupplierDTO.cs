using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Business.Models.DTOs
{
    public class SupplierDTO
    {
        public string? name { get; set; }
        public string? contact { get; set; }
    }

    public class CreateSupplierDTO : SupplierDTO
    {
        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateSupplierDTO : SupplierDTO
    {
        public ObjectStatusEnum? status { get; set; }
        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetSupplierDTO : SupplierDTO
    {
        public GetSupplierDTO()
        {
            vouchers = new HashSet<GetVoucherDTO>();
        }

        public virtual ICollection<GetVoucherDTO> vouchers { get; }

        public Guid id { get; }

        public string? status { get; }
        public DateTime? createDate { get; }
        public Guid? createBy { get; }
        public DateTime? updateDate { get; }
        public Guid? updateBy { get; }
    }
}