using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.Filters
{
    public class WalletTransactionFilter
    {
        public WalletTransactionStatusEnum? status { get; set; }
        public WalletTransactionTypeEnum? type { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public Guid? userId { get; set; }

        public Guid updateId { get; set; }
        public string? BankName { get; set; }
        public string? BankNumber { get; set; }
        public string? note { get; set; }
    }
}
