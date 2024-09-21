using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class SupplierDTO
    {
        public string? name { get; set; }
        public string? contact { get; set; }
    }

    public class CreateSupplierDTO : SupplierDTO
    {
        [JsonIgnore]
        public ObjectStatusEnum? status { get; set; }
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

    public class SupplierFilter
    {
        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }
}