using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.Filters
{
    public class SupplierWalletTransactionFilter
    {
        public Guid? id { get; set; }
        public SupplierWalletTransactionStatusEnum? status { get; set; }
        public DateOnly? startDate { get; set; }
        public DateOnly? endDate { get; set; }
    }
}
