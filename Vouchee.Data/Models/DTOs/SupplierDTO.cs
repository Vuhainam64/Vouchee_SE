using System.ComponentModel.DataAnnotations.Schema;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Business.Models.DTOs
{

    public class CreateSupplierDTO 
    {
        public string? name { get; set; }
        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateSupplierDTO 
    {
        public string? name { get; set; }
        public ObjectStatusEnum? status { get; set; }
        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class SupplierDTO
    {
        public Guid? id { get; set; }

        public string? name { get; set; }
        public string? image { get; set; }

        public bool? isVerified { get; set; }
    }

    public class GetSupplierDTO : SupplierDTO
    {
        public int totalQuantitySold { get; set; }
    }

    public class GetDetailSupplierDTO : SupplierDTO
    {
        public GetDetailSupplierDTO()
        {
            vouchers = [];
        }

        public virtual ICollection<GetVoucherDTO>? vouchers { get; set; }
    }
}