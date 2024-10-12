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
            vouchers = new HashSet<GetAllVoucherDTO>();
        }

        public Guid? id { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<GetAllVoucherDTO>? vouchers { get; set; }
    }
}