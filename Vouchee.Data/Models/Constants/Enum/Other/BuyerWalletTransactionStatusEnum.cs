using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Constants.Enum.Other
{
    public enum BuyerWalletTransactionStatusEnum
    {
        TRANSACTION_SUCCESS, // Thanh toán thành công
        TRANSACTION_FAIL, // Thanh toán thất bại
        MANUAL_WITHDRAWN, // Rút tiền theo yêu cầu
        BACK_FROM_BANK, // Từ ngân hàng hoàn về
        AUTO_WITHDRAWN, // Rú tiền tự động
    }
}
