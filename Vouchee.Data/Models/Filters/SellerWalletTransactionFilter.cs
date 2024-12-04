using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Filters
{
    public class SellerWalletTransactionFilter
    {
        public DateOnly? startDate { get; set; }
        public DateOnly? endDate { get; set; }
        public string? orderId { get; set; }
    }
}
