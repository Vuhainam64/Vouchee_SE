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

    public class GetSupplierDTO
    {
        //public GetSupplierDTO()
        //{
        //    vouchers = new HashSet<GetAllVoucherDTO>();
        //}

        public Guid? id { get; set; }

        public string? name { get; set; }
        public string? image { get; set; }

        // public virtual ICollection<GetAllVoucherDTO>? vouchers { get; set; }
    }
}