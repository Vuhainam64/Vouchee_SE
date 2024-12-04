using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Other;

namespace Vouchee.Data.Models.Filters
{
    public class BuyerWalletTransactionFilter
    {
        public Guid? id { get; set; }
        public BuyerWalletStatusEnum? status { get; set; }
        public DateOnly? startDate { get; set; }
        public DateOnly? endDate { get; set; }
    }
}
