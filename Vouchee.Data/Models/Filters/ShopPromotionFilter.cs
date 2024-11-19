using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Filters
{
    public class ShopPromotionFilter
    {
        public bool? isActive { get; set; }
        public string? name { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}
