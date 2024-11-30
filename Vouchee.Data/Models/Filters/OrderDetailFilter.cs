using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Sort;

namespace Vouchee.Data.Models.Filters
{

    public class OrderDetailFilter
    {
        public SortOrderEnum? sortOrderEnum { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }

        public string? orderId { get; set; }
    }
}
