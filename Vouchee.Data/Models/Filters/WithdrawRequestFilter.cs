using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.Filters
{
    public class WithdrawRequestFilter
    {
        public WithdrawRequestStatusEnum? status { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
