using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Other;

namespace Vouchee.Data.Models.Entities
{
    public class SellerWalletTransactionFilter
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? orderId { get; set; }
        public int? Year { get; set; }
    }

    public class BuyerWalletTransactionFilter : SellerWalletTransactionFilter
    {
        public WalletTransactionTypeEnum WalletTransactionTypeEnum { get; set; }
    }
}
