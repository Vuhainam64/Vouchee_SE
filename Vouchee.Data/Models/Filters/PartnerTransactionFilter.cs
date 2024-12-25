using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Filters
{
    public class PartnerTransactionFilter
    {
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
    }
}
