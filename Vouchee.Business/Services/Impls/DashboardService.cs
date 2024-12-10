using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services.Impls
{
    public class DashboardService : IDashboardService
    {
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IMapper _mapper;

        public DashboardService(IBaseRepository<Order> orderRepository,
                                IBaseRepository<User> userRepository,
                                IBaseRepository<Voucher> voucherRepository,
                                IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        public async Task<dynamic> GetOrderDashboard(int? year)
        {
            year ??= DateTime.Now.Year;

            var result = await _orderRepository.GetTable()
                                   .Where(x => x.CreateDate.Value.Year == year) // Filter by year
                                   .ToListAsync();

            var totalOrders = result.Count;

            // Calculate order status with percentages
            var successOrders = result.Count(x => x.Status == OrderStatusEnum.PAID.ToString());
            var failOrders = result.Count(x => x.Status == OrderStatusEnum.ERROR_AT_TRANSACTION.ToString());
            var pendingOrders = result.Count(x => x.Status == OrderStatusEnum.PENDING.ToString());
            var otherOrders = result.Count(x => x.Status != OrderStatusEnum.PAID.ToString() &&
                                                 x.Status != OrderStatusEnum.ERROR_AT_TRANSACTION.ToString() &&
                                                 x.Status != OrderStatusEnum.PENDING.ToString());

            var orderStatusChart = new
            {
                successOrder = successOrders,
                successOrderPercentage = totalOrders > 0 ? Math.Round((successOrders * 100.0) / totalOrders, 2) : 0,
                failOrder = failOrders,
                failOrderPercentage = totalOrders > 0 ? Math.Round((failOrders * 100.0) / totalOrders, 2) : 0,
                pendingOrder = pendingOrders,
                pendingOrderPercentage = totalOrders > 0 ? Math.Round((pendingOrders * 100.0) / totalOrders, 2) : 0,
                otherOrder = otherOrders,
                otherOrderPercentage = totalOrders > 0 ? Math.Round((otherOrders * 100.0) / totalOrders, 2) : 0,
            };


            // Group orders by month
            var groupedByMonth = result
                .GroupBy(x => x.CreateDate.Value.Month)
                .Select(g => new
                {
                    Month = g.Key, // The month number (1 = January, 2 = February, etc.)
                    TotalOrders = g.Count() // Total number of orders created in that month
                })
                .OrderBy(x => x.Month) // Ensure the result is ordered by month
                .ToList();

            return new
            {
                orderStatusChart,
                orderDashboard = groupedByMonth
            };
        }

        public Task<dynamic> GetTransactionDashboard()
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetUserDashboard()
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetVoucherDashboard()
        {
            throw new NotImplementedException();
        }
    }
}
