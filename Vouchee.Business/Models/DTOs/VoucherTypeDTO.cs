using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class VoucherTypeDTO
    {
        public string? name { get; set; }
    }

    public class CreateVoucherTypeDTO : VoucherTypeDTO
    {
        [JsonIgnore]
        public VoucherTypeStatusEnum status { get; set; }
        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateVoucherTypeDTO : VoucherTypeDTO
    {
        public VoucherTypeStatusEnum status { get; set; }
        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetVoucherTypeDTO : VoucherTypeDTO
    {
        public GetVoucherTypeDTO()
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

    public class VoucherTypeFilter 
    {
        public string? name { get; }
        public string? status { get; }
        public DateTime? createDate { get; }
        public Guid? createBy { get; }
        public DateTime? updateDate { get; }
        public Guid? updateBy { get; }
    }
}
