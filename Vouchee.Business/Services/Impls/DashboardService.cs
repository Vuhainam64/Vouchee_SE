using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services.Impls
{
    public class DashboardService : IDashboardService
    {
        private readonly IBaseRepository<WalletTransaction> _walletTransactionRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;

        public DashboardService(IBaseRepository<WalletTransaction> walletTransactionRepository,
                                IBaseRepository<Order> orderRepository,
                                IBaseRepository<User> userRepository,
                                IBaseRepository<Voucher> voucherRepository)
        {
            _walletTransactionRepository = walletTransactionRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _voucherRepository = voucherRepository;
        }

        public async Task<dynamic> GetActiveUserDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
        {
            // Adjust fromDate and toDate if today is true
            if (today)
            {
                fromDate = DateOnly.FromDateTime(DateTime.Today);
                toDate = DateOnly.FromDateTime(DateTime.Today);
            }

            // Convert DateOnly to DateTime for filtering
            DateTime fromDateTime = fromDate.ToDateTime(TimeOnly.MinValue);
            DateTime toDateTime = toDate.ToDateTime(TimeOnly.MaxValue);

            // Fetch all users
            var users = await _userRepository.GetTable().ToListAsync();

            // Filter users based on activity within the specified date range
            var activeUsers = users.Where(user =>
                user.LastAccessTime >= fromDateTime && user.LastAccessTime <= toDateTime);

            // Group active users by the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedActiveUsers;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedActiveUsers = activeUsers.GroupBy(user => (object)user.LastAccessTime.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedActiveUsers = activeUsers.GroupBy(user => (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(user.LastAccessTime.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedActiveUsers = activeUsers.GroupBy(user => (object)user.LastAccessTime.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedActiveUsers = activeUsers.GroupBy(user => (object)user.LastAccessTime.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var activeUserDashboard = groupedActiveUsers.Select(g => new
            {
                Period = g.Key,
                ActiveUsers = g.Count()
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var activeUserSummary = new
            {
                TotalActiveUsers = activeUsers.Count(),
                NewUsers = activeUsers.Count(user => user.CreateDate >= fromDateTime && user.CreateDate <= toDateTime),
                ReturningUsers = activeUsers.Count(user => user.CreateDate < fromDateTime)
            };

            return new
            {
                inactivatedAccounts = users.Count(x => x.Status.Equals(UserStatusEnum.INACTIVE.ToString())),
                bannedAccounts = users.Count(x => x.Status.Equals(UserStatusEnum.BANNED.ToString())),
                adminAccounts = users.Count(x => x.Role.Equals(RoleEnum.ADMIN.ToString())),
                supplierAccounts = users.Count(x => x.Role.Equals(RoleEnum.SUPPLIER.ToString())),
                memberAccounts = users.Count(x => x.Role.Equals(RoleEnum.USER.ToString())),
                activeUserSummary,
                activeUserDashboard
            };
        }

        public async Task<dynamic> GetOrderDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
        {
            var result = await _orderRepository.GetTable().ToListAsync();

            // Adjust fromDate and toDate if today is true
            if (today)
            {
                fromDate = DateOnly.FromDateTime(DateTime.Today);
                toDate = DateOnly.FromDateTime(DateTime.Today); // Set toDate to today
            }

            // Convert DateOnly to DateTime for filtering
            DateTime fromDateTime = fromDate.ToDateTime(TimeOnly.MinValue);
            DateTime toDateTime = toDate.ToDateTime(TimeOnly.MaxValue);

            // Filter based on date range (make sure CreateDate has a value)
            var filteredOrders = result.Where(x =>
                x.CreateDate.HasValue &&
                x.CreateDate.Value >= fromDateTime &&
                x.CreateDate.Value <= toDateTime);

            // Group orders by month, week, or day depending on the dateFilterTypeEnum
            IEnumerable<IGrouping<object, Order>> groupedBy;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedBy = filteredOrders.GroupBy(x => (object)x.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedBy = filteredOrders.GroupBy(x => (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(x.CreateDate.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedBy = filteredOrders.GroupBy(x => (object)x.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedBy = filteredOrders.GroupBy(x => (object)x.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            var orderDashboard = groupedBy.Select(g => new
            {
                Period = g.Key,
                TotalOrders = g.Count(),
                SuccessOrders = g.Count(x => x.Status == OrderStatusEnum.PAID.ToString()),
                FailOrders = g.Count(x => x.Status == OrderStatusEnum.ERROR_AT_TRANSACTION.ToString()),
                OtherOrders = g.Count(x => x.Status != OrderStatusEnum.PAID.ToString() && x.Status != OrderStatusEnum.ERROR_AT_TRANSACTION.ToString()),
                TotalRevenue = g.Where(x => x.Status == OrderStatusEnum.PAID.ToString()).Sum(x => x.TotalPrice),
            }).OrderBy(x => x.Period).ToList();

            return new
            {
                orderStatus = new
                {
                    successOrder = filteredOrders.Count(x => x.Status == OrderStatusEnum.PAID.ToString()),
                    failOrder = filteredOrders.Count(x => x.Status == OrderStatusEnum.ERROR_AT_TRANSACTION.ToString()),
                    otherOrder = filteredOrders.Count(x => x.Status != OrderStatusEnum.PAID.ToString() && x.Status != OrderStatusEnum.ERROR_AT_TRANSACTION.ToString()),
                },
                orderDashboard
            };
        }

        public async Task<dynamic> GetTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
        {
            var result = await _walletTransactionRepository.GetTable().ToListAsync();

            return new
            {
                transactionDashboard = new
                {
                    withdraw = result.Where(x => x.WithdrawRequestId != null),
                    topUp = result.Where(x => x.TopUpRequestId != null),
                    order = result.Where(x => x.OrderId != null),
                },
                withdrawDashboard = new
                {
                    success = result.Where(x => x.WithdrawRequestId != null && x.Status == WithdrawRequestStatusEnum.PAID.ToString()), 
                    fail = result.Where(x => x.WithdrawRequestId != null && x.Status == WithdrawRequestStatusEnum.FAIL.ToString()),
                    other = result.Where(x => x.WithdrawRequestId != null 
                                                    && x.Status != WithdrawRequestStatusEnum.PAID.ToString() 
                                                    && x.Status != WithdrawRequestStatusEnum.FAIL.ToString()) 
                },
                topUpDashboard = new
                {
                    success = result.Where(x => x.TopUpRequestId != null && x.Status == TopUpRequestStatusEnum.PAID.ToString()),
                    fail = result.Where(x => x.TopUpRequestId != null && x.Status == TopUpRequestStatusEnum.FAIL.ToString()),
                    other = result.Where(x => x.TopUpRequestId != null
                                                    && x.Status != TopUpRequestStatusEnum.PAID.ToString()
                                                    && x.Status != TopUpRequestStatusEnum.FAIL.ToString())
                },
                orderDashboard = new
                {
                    success = result.Where(x => x.OrderId != null && x.Status == OrderStatusEnum.PAID.ToString()),
                    fail = result.Where(x => x.OrderId != null && x.Status == OrderStatusEnum.FAIL.ToString()),
                    other = result.Where(x => x.OrderId != null
                                                    && x.Status != OrderStatusEnum.PAID.ToString()
                                                    && x.Status != OrderStatusEnum.FAIL.ToString())
                }
            };
        }

        public Task<dynamic> GetUserDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetVoucherDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
        {
            throw new NotImplementedException();
        }
    }
}