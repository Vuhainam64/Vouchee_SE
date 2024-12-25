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
        private readonly IBaseRepository<PartnerTransaction> _partnerTransactionRepository;
        private readonly IBaseRepository<RefundRequest> _refundRequestRepository;
        private readonly IBaseRepository<MoneyRequest> _moneyRequestRepository;
        private readonly IBaseRepository<Modal> _modalRepository;
        private readonly IBaseRepository<VoucherCode> _voucherCodeRepository;
        private readonly IBaseRepository<WalletTransaction> _walletTransactionRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;

        public DashboardService(IBaseRepository<PartnerTransaction> partnerTransactionRepository,
                                IBaseRepository<RefundRequest> refundRequestRepository,
                                IBaseRepository<MoneyRequest> moneyRequestRepository,
                                IBaseRepository<Modal> modalRepository,
                                IBaseRepository<VoucherCode> voucherCodeRepository,
                                IBaseRepository<WalletTransaction> walletTransactionRepository,
                                IBaseRepository<Order> orderRepository,
                                IBaseRepository<User> userRepository,
                                IBaseRepository<Voucher> voucherRepository)
        {
            _partnerTransactionRepository = partnerTransactionRepository;
            _refundRequestRepository = refundRequestRepository;
            _moneyRequestRepository = moneyRequestRepository;
            _modalRepository = modalRepository;
            _voucherCodeRepository = voucherCodeRepository;
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

        public async Task<dynamic> GetModalDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
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

            // Fetch all vouchers
            var modals = await _modalRepository.GetTable().ToListAsync();

            // Filter vouchers based on the date range
            var filteredModals = modals.Where(modal =>
                modal.CreateDate >= fromDateTime && modal.CreateDate <= toDateTime);

            // Group vouchers based on the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedModals;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedModals = filteredModals.GroupBy(voucher => (object)voucher.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedModals = filteredModals.GroupBy(voucher => (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(voucher.CreateDate.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedModals = filteredModals.GroupBy(voucher => (object)voucher.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedModals = filteredModals.GroupBy(voucher => (object)voucher.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var modalDashboard = groupedModals.Select(g => new
            {
                Period = g.Key,
                totalModals = g.Count(),
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var modalSummary = new
            {
                TotalModals = filteredModals.Count(),
            };

            return new
            {
                modalSummary,
                modalDashboard
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

        public async Task<dynamic> GetRefundRequestDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
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

            // Fetch all withdraw transactions
            var refundRequests = await _refundRequestRepository.GetTable().ToListAsync();

            // Filter transactions based on the date range
            var filteredTransactions = refundRequests.Where(transaction =>
                transaction.CreateDate.HasValue &&
                transaction.CreateDate.Value >= fromDateTime &&
                transaction.CreateDate.Value <= toDateTime);

            // Group transactions based on the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedTransactions;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedTransactions = filteredTransactions.GroupBy(transaction => (object)transaction.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedTransactions = filteredTransactions.GroupBy(transaction =>
                        (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(transaction.CreateDate.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedTransactions = filteredTransactions.GroupBy(transaction => (object)transaction.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedTransactions = filteredTransactions.GroupBy(transaction => (object)transaction.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var requestDashboard = groupedTransactions.Select(g => new
            {
                Period = g.Key, // Ensure this matches the grouping logic used earlier
                TotalRequests = g.Count(),
                DeclinedRequests = g.Count(t => t.Status == RefundRequestStatusEnum.DECLINED.ToString()),
                AcceptedRequests = g.Count(t => t.Status == RefundRequestStatusEnum.ACCEPTED.ToString()),
                OtherRequests = g.Count(t => t.Status != RefundRequestStatusEnum.DECLINED.ToString() && t.Status != RefundRequestStatusEnum.ACCEPTED.ToString())
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var requestSummary = new
            {
                TotalRequests = filteredTransactions.Count(),
                DeclinedRequests = filteredTransactions.Count(t => t.Status == RefundRequestStatusEnum.DECLINED.ToString()),
                AcceptedRequests = filteredTransactions.Count(t => t.Status == RefundRequestStatusEnum.ACCEPTED.ToString()),
                OtherRequests = filteredTransactions.Count(t => t.Status != RefundRequestStatusEnum.DECLINED.ToString() && t.Status != RefundRequestStatusEnum.ACCEPTED.ToString()),
            };

            return new
            {
                requestSummary,
                requestDashboard
            };
        }

        public async Task<dynamic> GetTopUpRequestDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
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

            // Fetch all withdraw transactions
            var withdrawTransactions = await _moneyRequestRepository.GetTable()
                .Where(x => x.Type.Equals(MoneyRequestTypeEnum.TOPUP.ToString()))
                .ToListAsync();

            // Filter transactions based on the date range
            var filteredTransactions = withdrawTransactions.Where(transaction =>
                transaction.CreateDate.HasValue &&
                transaction.CreateDate.Value >= fromDateTime &&
                transaction.CreateDate.Value <= toDateTime);

            // Group transactions based on the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedTransactions;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedTransactions = filteredTransactions.GroupBy(transaction => (object)transaction.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedTransactions = filteredTransactions.GroupBy(transaction =>
                        (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(transaction.CreateDate.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedTransactions = filteredTransactions.GroupBy(transaction => (object)transaction.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedTransactions = filteredTransactions.GroupBy(transaction => (object)transaction.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var requestDashboard = groupedTransactions.Select(g => new
            {
                Period = g.Key,
                TotalRequests = g.Count(),
                TotalAmount = g.Sum(t => t.Amount), // Assuming each transaction has an Amount property
                FailRequests = g.Count(t => t.Status == "FAILED"),
                SuccessRequests = g.Count(t => t.Status == "PAID"),
                OtherRequests = g.Count(t => t.Status != "FAILED" && t.Status != "PAID")
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var requestSummary = new
            {
                TotalRequests = filteredTransactions.Count(),
                TotalAmount = filteredTransactions.Sum(t => t.Amount), // Assuming each transaction has an Amount property
                FailRequests = filteredTransactions.Count(t => t.Status == "FAILED"),
                SuccessRequests = filteredTransactions.Count(t => t.Status == "PAID"),
                OtherRequests = filteredTransactions.Count(t => t.Status != "FAILED" && t.Status != "PAID")
            };

            return new
            {
                requestSummary,
                requestDashboard
            };
        }

        public async Task<dynamic> GetOrderWalletTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
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

            // Fetch all vouchers
            var walletTransactions = await _walletTransactionRepository.GetTable()
                                                .Where(x => x.OrderId != null)
                                                .ToListAsync();

            // Filter vouchers based on the date range
            var filteredWalletTransactions = walletTransactions.Where(walletTransanction =>
                walletTransanction.CreateDate >= fromDateTime && walletTransanction.CreateDate <= toDateTime);

            // Group vouchers based on the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedWalletTransaction;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(voucher.CreateDate.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var totalOrderTransactionDashboard = groupedWalletTransaction.Select(g => new
            {
                Period = g.Key,
                TotalOrderTransaction = g.Count(),
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var orderTransactionSummary = new
            {
                TotalOrderTransaction = filteredWalletTransactions.Count(),
            };

            return new
            {
                orderTransactionSummary,
                totalOrderTransactionDashboard
            };
        }

        public async Task<dynamic> GetVoucherCodeDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
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

            // Fetch all vouchers
            var voucherCodes = await _voucherCodeRepository.GetTable().ToListAsync();

            // Filter vouchers based on the date range
            var filteredVoucherCodes = voucherCodes.Where(voucherCode =>
                voucherCode.CreateDate >= fromDateTime && voucherCode.CreateDate <= toDateTime);

            // Group vouchers based on the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedVoucherCodes;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedVoucherCodes = filteredVoucherCodes.GroupBy(voucher => (object)voucher.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedVoucherCodes = filteredVoucherCodes.GroupBy(voucher => (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(voucher.CreateDate.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedVoucherCodes = filteredVoucherCodes.GroupBy(voucher => (object)voucher.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedVoucherCodes = filteredVoucherCodes.GroupBy(voucher => (object)voucher.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var voucherCodeDashboard = groupedVoucherCodes.Select(g => new
            {
                Period = g.Key,
                TotalVoucherCodes = g.Count(),
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var voucherCodeSummary = new
            {
                TotalVoucherCodes = filteredVoucherCodes.Count(),
            };

            return new
            {
                voucherCodeSummary,
                voucherCodeDashboard
            };
        }

        public async Task<dynamic> GetVoucherDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
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

            // Fetch all vouchers
            var vouchers = await _voucherRepository.GetTable().ToListAsync();

            // Filter vouchers based on the date range
            var filteredVouchers = vouchers.Where(voucher =>
                voucher.CreateDate >= fromDateTime && voucher.CreateDate <= toDateTime);

            // Group vouchers based on the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedVouchers;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedVouchers = filteredVouchers.GroupBy(voucher => (object)voucher.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedVouchers = filteredVouchers.GroupBy(voucher => (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(voucher.CreateDate.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedVouchers = filteredVouchers.GroupBy(voucher => (object)voucher.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedVouchers = filteredVouchers.GroupBy(voucher => (object)voucher.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var voucherDashboard = groupedVouchers.Select(g => new
            {
                Period = g.Key,
                TotalVouchers = g.Count(),
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var voucherSummary = new
            {
                TotalVouchers = filteredVouchers.Count(),
            };

            return new
            {
                voucherSummary,
                voucherDashboard
            };
        }

        public async Task<dynamic> GetWithdrawRequestDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
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

            // Fetch all withdraw transactions
            var withdrawRequests = await _moneyRequestRepository.GetTable()
                .Where(x => x.Type.Equals(MoneyRequestTypeEnum.WITHDRAW.ToString()))
                .ToListAsync();

            // Filter transactions based on the date range
            var filteredRequests = withdrawRequests.Where(transaction =>
                transaction.CreateDate.HasValue &&
                transaction.CreateDate.Value >= fromDateTime &&
                transaction.CreateDate.Value <= toDateTime);

            // Group transactions based on the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedTransactions;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedTransactions = filteredRequests.GroupBy(transaction => (object)transaction.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedTransactions = filteredRequests.GroupBy(transaction =>
                        (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(transaction.CreateDate.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedTransactions = filteredRequests.GroupBy(transaction => (object)transaction.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedTransactions = filteredRequests.GroupBy(transaction => (object)transaction.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var requestDashboard = groupedTransactions.Select(g => new
            {
                Period = g.Key,
                TotalRequests = g.Count(),
                TotalAmount = g.Sum(t => t.Amount), // Assuming each transaction has an Amount property
                FailRequests = g.Count(t => t.Status == "FAILED"),
                SuccessRequests = g.Count(t => t.Status == "PAID"),
                OtherRequests = g.Count(t => t.Status != "FAILED" && t.Status != "PAID")
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var requestSummary = new
            {
                TotalRequests = filteredRequests.Count(),
                TotalAmount = filteredRequests.Sum(t => t.Amount), // Assuming each transaction has an Amount property
                FailRequests = filteredRequests.Count(t => t.Status == "FAILED"),
                SuccessRequests = filteredRequests.Count(t => t.Status == "PAID"),
                OtherRequests = filteredRequests.Count(t => t.Status != "FAILED" && t.Status != "PAID")
            };

            return new
            {
                requestSummary,
                requestDashboard
            };
        }

        public async Task<dynamic> GetTopupWalletTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
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

            // Fetch all vouchers
            var walletTransactions = await _walletTransactionRepository.GetTable()
                                                .Where(x => x.TopUpRequestId != null)
                                                .ToListAsync();

            // Filter vouchers based on the date range
            var filteredWalletTransactions = walletTransactions.Where(walletTransanction =>
                walletTransanction.CreateDate >= fromDateTime && walletTransanction.CreateDate <= toDateTime);

            // Group vouchers based on the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedWalletTransaction;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(voucher.CreateDate.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var totalTopUpTransactionDashboard = groupedWalletTransaction.Select(g => new
            {
                Period = g.Key,
                TotalTopUpDashboard = g.Count(),
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var topUpTransactionSummary = new
            {
                TotalTopUpTransaction = filteredWalletTransactions.Count(),
            };

            return new
            {
                topUpTransactionSummary,
                totalTopUpTransactionDashboard
            };
        }

        public async Task<dynamic> GetWithdrawWalletTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
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

            // Fetch all vouchers
            var walletTransactions = await _walletTransactionRepository.GetTable()
                                                .Where(x => x.WithdrawRequestId != null)
                                                .ToListAsync();

            // Filter vouchers based on the date range
            var filteredWalletTransactions = walletTransactions.Where(walletTransanction =>
                walletTransanction.CreateDate >= fromDateTime && walletTransanction.CreateDate <= toDateTime);

            // Group vouchers based on the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedWalletTransaction;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(voucher.CreateDate.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var totalWithdrawTransactionDashboard = groupedWalletTransaction.Select(g => new
            {
                Period = g.Key,
                TotalWithdrawTransaction = g.Count(),
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var withdrawTransactionSummary = new
            {
                TotalWithdrawTransaction = filteredWalletTransactions.Count(),
            };

            return new
            {
                withdrawTransactionSummary,
                totalWithdrawTransactionDashboard
            };
        }

        public async Task<dynamic> GetRefundWalletTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
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

            // Fetch all vouchers
            var walletTransactions = await _walletTransactionRepository.GetTable()
                                                .Where(x => x.RefundRequestId != null)
                                                .ToListAsync();

            // Filter vouchers based on the date range
            var filteredWalletTransactions = walletTransactions.Where(walletTransanction =>
                walletTransanction.CreateDate >= fromDateTime && walletTransanction.CreateDate <= toDateTime);

            // Group vouchers based on the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedWalletTransaction;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(voucher.CreateDate.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedWalletTransaction = filteredWalletTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var totalRefundTransactionDashboard = groupedWalletTransaction.Select(g => new
            {
                Period = g.Key,
                TotalRefundTransaction = g.Count(),
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var refundTransactionSummary = new
            {
                TotalRefundTransaction = filteredWalletTransactions.Count(),
            };

            return new
            {
                refundTransactionSummary,
                totalRefundTransactionDashboard
            };
        }

        public async Task<dynamic> GetPartnerTransactionDashboard(DateOnly fromDate, DateOnly toDate, bool today, DateFilterTypeEnum dateFilterTypeEnum)
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

            // Fetch all vouchers
            var parnerTransactions = await _partnerTransactionRepository.GetTable()
                                                .ToListAsync();

            // Filter vouchers based on the date range
            var filteredPartnerTransactions = parnerTransactions.Where(partnerTransaction =>
                partnerTransaction.TransactionDate >= fromDateTime && partnerTransaction.TransactionDate <= toDateTime);

            // Group vouchers based on the specified date filter type
            IEnumerable<IGrouping<object, dynamic>> groupedWalletTransaction;

            switch (dateFilterTypeEnum)
            {
                case DateFilterTypeEnum.DAILY:
                    groupedWalletTransaction = filteredPartnerTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Date);
                    break;
                case DateFilterTypeEnum.WEEKLY:
                    groupedWalletTransaction = filteredPartnerTransactions.GroupBy(voucher => (object)CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(voucher.CreateDate.Value.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday));
                    break;
                case DateFilterTypeEnum.MONTHLY:
                    groupedWalletTransaction = filteredPartnerTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Month);
                    break;
                case DateFilterTypeEnum.YEARLY:
                    groupedWalletTransaction = filteredPartnerTransactions.GroupBy(voucher => (object)voucher.CreateDate.Value.Year);
                    break;
                default:
                    throw new ArgumentException("Invalid date filter type");
            }

            // Generate the dashboard data
            var totalPartnerTransactionDashboard = groupedWalletTransaction.Select(g => new
            {
                Period = g.Key,
                TotalRefundTransaction = g.Count(),
                TotalAmount = g.Sum(x => x.AmountIn)
            }).OrderBy(x => x.Period).ToList();

            // Create summary metrics
            var partnerTransactionSummary = new
            {
                TotalRefundTransaction = filteredPartnerTransactions.Count(),
                TotalAmount = filteredPartnerTransactions.Sum(transaction => transaction.AmountIn) // Calculate total amount overall
            };

            return new
            {
                partnerTransactionSummary,
                totalPartnerTransactionDashboard
            };
        }
    }
}