using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.Filters
{
    public class VoucherFilter
    {
        public VoucherFilter()
        {
            categoryIds = [];
            supplierIDs = [];
        }

        public string? title { get; set; }
        public VoucherStatusEnum? status { get; set; }
        public bool? isActive { get; set; }
        public decimal? minPrice { get; set; } = 5000;
        public decimal? maxPrice { get; set; } = 10000000;
        public IList<Guid>? categoryIds { get; set; }
        public IList<Guid>? supplierIDs { get; set; }
    }
}