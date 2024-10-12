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
        public Guid? supplierId { get; set; }
        public Guid? brandId { get; set; }
    }
}