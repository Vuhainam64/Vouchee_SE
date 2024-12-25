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
        public Task<dynamic> GetVoucherDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetModalDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetVoucherCodeDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetOrderDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetTopUpRequestDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetWithdrawRequestDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetRefundRequestDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetOrderWalletTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetTopupWalletTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetWithdrawWalletTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetRefundWalletTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetActiveUserDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
        public Task<dynamic> GetPartnerTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum);
    }
}
