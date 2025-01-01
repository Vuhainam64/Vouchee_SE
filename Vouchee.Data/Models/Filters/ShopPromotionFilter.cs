using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.Filters
{
    public class ShopPromotionFilter
    {
        public bool? isActive { get; set; }
        public string? name { get; set; }
        public PromotionStatusEnum? status { get; set; }

        //public DateTime? startDate { get; set; }
        //public DateTime? endDate { get; set; }
    }
}
