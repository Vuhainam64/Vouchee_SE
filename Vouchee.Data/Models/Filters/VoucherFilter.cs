using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Filters
{
    public class VoucherFiler
    {
        public string? name { get; set; }
        public decimal? price { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public Guid? supplierId { get; set; }
        public Guid? voucherTypeId { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }
}
