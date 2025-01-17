using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.Filters
{
    public class WithdrawRequestFilter
    {
        public WithdrawRequestStatusEnum? status { get; set; }
        public MoneyRequestTypeEnum? type { get; set; }

        public DateTime? CreateDate { get; set; }
        public Guid? updateId { get; set; }
        public DateTime? updateDate { get; set; }
        public string? BankName { get; set; }
        public string? BankNumber { get; set; }
        public string? note { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
