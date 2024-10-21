using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Filters
{
    public class OrderFilter
    {
        public decimal? totalPrice { get; set; }
        public decimal? discountPrice { get; set; }
        public decimal? finalPrice { get; set; }
    }
}
