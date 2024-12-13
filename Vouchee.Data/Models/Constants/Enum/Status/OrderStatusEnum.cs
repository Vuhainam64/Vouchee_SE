using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Constants.Enum.Status
{
    public enum OrderStatusEnum
    {
        FAIL,
        SUSPICIOUS,
        PENDING,
        DONE,
        ERROR_AT_TRANSACTION,
        FINISH_TRANSACTION,
        PAID,
        UN_PAID
    }
}
