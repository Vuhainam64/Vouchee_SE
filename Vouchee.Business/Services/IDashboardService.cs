using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Other;

namespace Vouchee.Business.Services
{
    public interface IDashboardService
    {
        // CREATE

        // READ
        public Task<dynamic> GetVoucherDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetModalDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetVoucherCodeDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetOrderDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetActiveUserDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetOtherDashboard();
        // UPDATE

        // DELETE
    }
}
