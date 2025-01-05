using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Constants.Enum.Other
{
    public enum WalletTransactionTypeEnum
    {
        WITHDRAW,
        AUTO_WITHDRAW,
        REFUND,
        TOPUP,
        BUYER_ORDER,
        SELLER_ORDER,
        SUPPLIER_ORDER,
        EXPIRED_ORDER,
        ADMIN
    }
}
