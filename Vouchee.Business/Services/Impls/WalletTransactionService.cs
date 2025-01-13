using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class WalletTransactionService : IWalletTransactionService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<WalletTransaction> _walletTransactionRepository;
        private readonly IMapper _mapper;

        public WalletTransactionService(IBaseRepository<User> userRepository,
                                        IBaseRepository<Order> orderRepository,
                                        IBaseRepository<WalletTransaction> walletTransactionRepository,
                                        IMapper mapper)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _mapper = mapper;
        }

        public async Task<dynamic> GetBuyerInChart(ThisUserObj currentUser)
        {
            // Query for wallet transactions linked to the supplier's wallet
            var walletTransactions = _walletTransactionRepository.GetTable()
                                        .Where(x => x.BuyerWallet != null
                                                        && x.BuyerWallet.Id == currentUser.userId
                                                        && x.WithdrawRequestId == null);

            // Generate all months of the current year
            var currentYear = DateTime.Now.Year;
            var allMonths = Enumerable.Range(1, 12)
                .Select(m => new { Year = currentYear, Month = m })
                .ToList();

            // Group transactions by month
            var monthDashboard = await walletTransactions
                .GroupBy(x => new { x.CreateDate.Value.Year, x.CreateDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalTransactions = g.Count(),
                    TotalAmount = g.Sum(t => t.Amount) // Assuming there's an Amount property
                })
                .ToListAsync();

            // Merge with all months to ensure each month is included
            var completeMonthDashboard = allMonths
                .GroupJoin(
                    monthDashboard,
                    all => new { all.Year, all.Month },
                    db => new { db.Year, db.Month },
                    (all, dbGroup) => new
                    {
                        Year = all.Year,
                        Month = all.Month,
                        TotalTransactions = dbGroup.FirstOrDefault()?.TotalTransactions ?? 0,
                        TotalAmount = dbGroup.FirstOrDefault()?.TotalAmount ?? 0
                    }
                )
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();
            var revenue = walletTransactions.Sum(x => x.Amount);

            // Return the dashboard data
            return new ResponseMessage<dynamic>()
            {
                message = "Đã lấy dữ liệu cho buyer thành công",
                result = true,
                value = new
                {
                    revenue = revenue,
                    monthDashboard = completeMonthDashboard
                },
            };
        }

        public async Task<DynamicResponseModel<GetBuyerWalletTransactionDTO>> GetBuyerInTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj)
        {
            (int, IQueryable<GetBuyerWalletTransactionDTO>) result;

            result = _walletTransactionRepository.GetTable().Where(x => x.BuyerWallet != null 
                                                                            && x.BuyerWallet.Id == thisUserObj.userId
                                                                            && x.WithdrawRequestId == null
                                                                            && (walletTransactionFilter.fromDate == null || x.CreateDate >= walletTransactionFilter.fromDate) 
                                                                            && (walletTransactionFilter.toDate == null || x.CreateDate <= walletTransactionFilter.toDate))
                        .ProjectTo<GetBuyerWalletTransactionDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetBuyerWalletTransactionDTO>(walletTransactionFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetBuyerWalletTransactionDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<DynamicResponseModel<GetBuyerWalletTransactionDTO>> GetBuyerOutTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj)
        {
            var startDateTime = walletTransactionFilter.fromDate;
            var endDateTime = walletTransactionFilter.toDate;

            var existedUser = await _userRepository.FindAsync(thisUserObj.userId);

            (int, IQueryable<GetBuyerWalletTransactionDTO>) result;

            result = _walletTransactionRepository.GetTable()
                             .Where(x => x.BuyerWalletId == existedUser.BuyerWallet.Id
                                            && x.WithdrawRequestId != null
                                            && (startDateTime == null || x.CreateDate >= startDateTime) 
                                            && (endDateTime == null || x.CreateDate <= endDateTime))
                             .ProjectTo<GetBuyerWalletTransactionDTO>(_mapper.ConfigurationProvider)
                             .DynamicFilter(_mapper.Map<GetBuyerWalletTransactionDTO>(walletTransactionFilter))
                             .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetBuyerWalletTransactionDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<DynamicResponseModel<GetBuyerWalletTransactionDTO>> GetBuyerWalletTransactionsAsync(PagingRequest pagingRequest, 
                                                                                                                BuyerWalletTransactionFilter buyerWalletTransactionFilter, 
                                                                                                                ThisUserObj currentUser)
        {
            var startDateTime = buyerWalletTransactionFilter.startDate?.ToDateTime(TimeOnly.MinValue);
            var endDateTime = buyerWalletTransactionFilter.endDate?.ToDateTime(TimeOnly.MaxValue);

            var existedUser = await _userRepository.FindAsync(currentUser.userId);

            (int, IQueryable<GetBuyerWalletTransactionDTO>) result;

            result = _walletTransactionRepository.GetTable()
                             .Where(x => x.BuyerWalletId == existedUser.BuyerWallet.Id &&
                                         (startDateTime == null || x.CreateDate >= startDateTime) &&
                                         (endDateTime == null || x.CreateDate <= endDateTime) &&
                                         (buyerWalletTransactionFilter.id == null || x.Id == buyerWalletTransactionFilter.id))
                             .ProjectTo<GetBuyerWalletTransactionDTO>(_mapper.ConfigurationProvider)
                             .DynamicFilter(_mapper.Map<GetBuyerWalletTransactionDTO>(buyerWalletTransactionFilter))
                             .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetBuyerWalletTransactionDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<dynamic> GetSellerInChart(ThisUserObj thisUserObj)
        {
            // Query for wallet transactions linked to the supplier's wallet
            var walletTransactions = _walletTransactionRepository.GetTable()
                                        .Where(x => x.SellerWallet != null
                                                        && x.BuyerWallet.Id == thisUserObj.userId
                                                        && x.WithdrawRequestId == null);

            // Generate all months of the current year
            var currentYear = DateTime.Now.Year;
            var allMonths = Enumerable.Range(1, 12)
                .Select(m => new { Year = currentYear, Month = m })
                .ToList();

            // Group transactions by month
            var monthDashboard = await walletTransactions
                .GroupBy(x => new { x.CreateDate.Value.Year, x.CreateDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalTransactions = g.Count(),
                    TotalAmount = g.Sum(t => t.Amount) // Assuming there's an Amount property
                })
                .ToListAsync();

            // Merge with all months to ensure each month is included
            var completeMonthDashboard = allMonths
                .GroupJoin(
                    monthDashboard,
                    all => new { all.Year, all.Month },
                    db => new { db.Year, db.Month },
                    (all, dbGroup) => new
                    {
                        Year = all.Year,
                        Month = all.Month,
                        TotalTransactions = dbGroup.FirstOrDefault()?.TotalTransactions ?? 0,
                        TotalAmount = dbGroup.FirstOrDefault()?.TotalAmount ?? 0
                    }
                )
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();
            var revenue = walletTransactions.Sum(x => x.Amount);

            // Return the dashboard data
            return new ResponseMessage<dynamic>()
            {
                message = "Đã lấy dữ liệu cho seller thành công",
                result = true,
                value = new
                {
                    revenue = revenue,
                    monthDashboard = completeMonthDashboard
                },
            };
        }

        public async Task<DynamicResponseModel<GetSellerWalletTransaction>> GetSellerInTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj)
        {
            (int, IQueryable<GetSellerWalletTransaction>) result;

            result = _walletTransactionRepository.GetTable().Where(x => x.SellerWallet != null 
                                                                            && x.SellerWallet.SellerId == thisUserObj.userId
                                                                            && x.WithdrawRequestId == null
                                                                            && (walletTransactionFilter.fromDate == null || x.CreateDate >= walletTransactionFilter.fromDate)
                                                                            && (walletTransactionFilter.toDate == null || x.CreateDate <= walletTransactionFilter.toDate))
                        .ProjectTo<GetSellerWalletTransaction>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetSellerWalletTransaction>(walletTransactionFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetSellerWalletTransaction>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<DynamicResponseModel<GetSellerWalletTransaction>> GetSellerOutTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj)
        {
            (int, IQueryable<GetSellerWalletTransaction>) result;

            result = _walletTransactionRepository.GetTable().Where(x => x.SellerWallet != null
                                                                            && x.SellerWallet.SellerId == thisUserObj.userId
                                                                            && x.WithdrawRequestId != null
                                                                            && (walletTransactionFilter.fromDate == null || x.CreateDate >= walletTransactionFilter.fromDate)
                                                                            && (walletTransactionFilter.toDate == null || x.CreateDate <= walletTransactionFilter.toDate))
                        .ProjectTo<GetSellerWalletTransaction>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetSellerWalletTransaction>(walletTransactionFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetSellerWalletTransaction>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<DynamicResponseModel<GetSellerWalletTransaction>> GetSellerWalletTransactionsAsync(PagingRequest pagingRequest, 
                                                                                                                SellerWalletTransactionFilter sellerWalletTransactionFilter, 
                                                                                                                ThisUserObj currentUser)
        {
            var startDateTime = sellerWalletTransactionFilter.startDate?.ToDateTime(TimeOnly.MinValue);
            var endDateTime = sellerWalletTransactionFilter.endDate?.ToDateTime(TimeOnly.MaxValue);

            var existedUser = await _userRepository.FindAsync(currentUser.userId);

            if (existedUser.SellerWallet == null)
            {
                throw new NotFoundException("Không tìm thấy ví bán hàng");
            }

            (int, IQueryable<GetSellerWalletTransaction>) result;

            result = _walletTransactionRepository.GetTable()
                .Where(x => x.SellerWalletId == existedUser.SellerWallet.Id &&
                            (startDateTime == null || x.CreateDate >= startDateTime) &&
                            (endDateTime == null || x.CreateDate <= endDateTime) &&
                            (sellerWalletTransactionFilter.orderId == null || x.OrderId == sellerWalletTransactionFilter.orderId))
                .ProjectTo<GetSellerWalletTransaction>(_mapper.ConfigurationProvider)
                .DynamicFilter(_mapper.Map<GetSellerWalletTransaction>(sellerWalletTransactionFilter))
                .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetSellerWalletTransaction>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<dynamic> GetSupplerInChart(ThisUserObj currentUser)
        {
            var existedUser = await _userRepository.GetByIdAsync(currentUser.userId, includeProperties: x => x.Include(x => x.Supplier));

            if (existedUser == null)
            {
                throw new NotFoundException("User này không tồn tại");
            }
            if (existedUser.Supplier == null)
            {
                throw new NotFoundException("User này không thuộc về supplier nào");
            }

            // Query for wallet transactions linked to the supplier's wallet
            var walletTransactions = _walletTransactionRepository.GetTable()
                                        .Where(x => x.SupplierWallet != null
                                                        && x.SupplierWallet.SupplierId == existedUser.SupplierId
                                                        && x.WithdrawRequestId == null);

            // Generate all months of the current year
            var currentYear = DateTime.Now.Year;
            var allMonths = Enumerable.Range(1, 12)
                .Select(m => new { Year = currentYear, Month = m })
                .ToList();

            // Group transactions by month
            var monthDashboard = await walletTransactions
                .GroupBy(x => new { x.CreateDate.Value.Year, x.CreateDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalTransactions = g.Count(),
                    TotalAmount = g.Sum(t => t.Amount) // Assuming there's an Amount property
                })
                .ToListAsync();

            // Merge with all months to ensure each month is included
            var completeMonthDashboard = allMonths
                .GroupJoin(
                    monthDashboard,
                    all => new { all.Year, all.Month },
                    db => new { db.Year, db.Month },
                    (all, dbGroup) => new
                    {
                        Year = all.Year,
                        Month = all.Month,
                        TotalTransactions = dbGroup.FirstOrDefault()?.TotalTransactions ?? 0,
                        TotalAmount = dbGroup.FirstOrDefault()?.TotalAmount ?? 0
                    }
                )
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();
            var revenue = walletTransactions.Sum(x => x.Amount);

            // Return the dashboard data
            return new ResponseMessage<dynamic>()
            {
                message = "Đã lấy dữ liệu cho supplier thành công",
                result = true,
                value = new
                {
                    revenue = revenue,
                    monthDashboard = completeMonthDashboard
                },
            };
        }

        public async Task<DynamicResponseModel<GetSupplierWalletTransactionDTO>> GetSupplierInTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj)
        {
            var existedUser = await _userRepository.GetByIdAsync(thisUserObj.userId, includeProperties: x => x.Include(x => x.Supplier));

            if (existedUser == null)
            {
                throw new NotFoundException("User này không tồn tại");
            }
            if (existedUser.Supplier == null)
            {
                throw new NotFoundException("User này không thuộc về supplier nào");
            }

            (int, IQueryable<GetSupplierWalletTransactionDTO>) result;

            result = _walletTransactionRepository.GetTable().Where(x => x.SupplierWallet != null
                                                                            && x.SupplierWallet.Supplier.Id == existedUser.SupplierId
                                                                            && x.WithdrawRequestId == null
                                                                            && (walletTransactionFilter.fromDate == null || x.CreateDate >= walletTransactionFilter.fromDate)
                                                                            && (walletTransactionFilter.toDate == null || x.CreateDate <= walletTransactionFilter.toDate))
                        .ProjectTo<GetSupplierWalletTransactionDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetSupplierWalletTransactionDTO>(walletTransactionFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetSupplierWalletTransactionDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<DynamicResponseModel<GetSupplierWalletTransactionDTO>> GetSupplierOutTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj)
        {
            var existedUser = await _userRepository.GetByIdAsync(thisUserObj.userId, includeProperties: x => x.Include(x => x.Supplier));

            if (existedUser == null)
            {
                throw new NotFoundException("User này không tồn tại");
            }
            if (existedUser.Supplier == null)
            {
                throw new NotFoundException("User này không thuộc về supplier nào");
            }

            (int, IQueryable<GetSupplierWalletTransactionDTO>) result;

            result = _walletTransactionRepository.GetTable().Where(x => x.SupplierWallet != null
                                                                            && x.SupplierWallet.Supplier.Id == existedUser.SupplierId
                                                                            && x.WithdrawRequestId != null
                                                                            && (walletTransactionFilter.fromDate == null || x.CreateDate >= walletTransactionFilter.fromDate)
                                                                            && (walletTransactionFilter.toDate == null || x.CreateDate <= walletTransactionFilter.toDate))
                        .ProjectTo<GetSupplierWalletTransactionDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetSupplierWalletTransactionDTO>(walletTransactionFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetSupplierWalletTransactionDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<DynamicResponseModel<GetWalletTransactionDTO>> GetWalletTransactionAsync(
    PagingRequest pagingRequest,
    WalletTransactionFilter walletTransactionFilter)
        {
            (int totalRecords, IQueryable<GetWalletTransactionDTO> queryableData) result;

            // Start building the query from the repository
            var query = _walletTransactionRepository.GetTable();

            //// Apply userId filter if provided
            //if (walletTransactionFilter.userId.HasValue)
            //{
            //    query = query.Where(transaction => transaction.USer == walletTransactionFilter.userId.Value);
            //}

            //// Apply status filter if provided
            //if (walletTransactionFilter.status.HasValue)
            //{
            //    query = query.Where(transaction => transaction.Status == walletTransactionFilter.status.Value);
            //}

            //// Apply type filter if provided
            //if (walletTransactionFilter.type.HasValue)
            //{
            //    query = query.Where(transaction => transaction.Type == walletTransactionFilter.type.Value);
            //}

            // Apply date range filter if provided
            if (walletTransactionFilter.fromDate.HasValue)
            {
                query = query.Where(transaction => transaction.CreateDate >= walletTransactionFilter.fromDate.Value);
            }

            if (walletTransactionFilter.toDate.HasValue)
            {
                query = query.Where(transaction => transaction.CreateDate <= walletTransactionFilter.toDate.Value);
            }

            // Project the query to DTO
            var projectedQuery = query.ProjectTo<GetWalletTransactionDTO>(_mapper.ConfigurationProvider);

            // Apply dynamic filters if necessary (based on mapping from WalletTransactionFilter)
            projectedQuery = projectedQuery.DynamicFilter(_mapper.Map<GetWalletTransactionDTO>(walletTransactionFilter));

            // Apply pagination
            result = projectedQuery.PagingIQueryable(
                pagingRequest.page,
                pagingRequest.pageSize,
                PageConstant.LIMIT_PAGING,
                PageConstant.DEFAULT_PAPING);

            // Return the result with metadata
            return new DynamicResponseModel<GetWalletTransactionDTO>
            {
                metaData = new MetaData
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.totalRecords
                },
                results = await result.queryableData.ToListAsync()
            };
        }

        public async Task<dynamic> GetWalletTransactionsAsync(ThisUserObj currentUser)
        {
            var existedUser = await _userRepository.FindAsync(currentUser.userId);

            if (existedUser == null)
            {
                throw new NotFoundException("Không tìm thấy user này");
            }

            var recentBuyerTransactions = existedUser.BuyerWallet.BuyerWalletTransactions.OrderByDescending(x => x.CreateDate).Take(5);

            var buyerTransactionDashboard = existedUser.BuyerWallet.BuyerWalletTransactions
                    .GroupBy(t => new { t.CreateDate.Value.Year, t.CreateDate.Value.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Increase = g.Where(t => t.TopUpRequest != null).Sum(t => t.Amount),
                        Decrease = g.Where(t => (t.Order != null || t.WithDrawRequest != null)).Sum(t => t.Amount),
                        //Increase = g.Where(t => t.TopUpRequest != null && t.Status == "PAID").Sum(t => t.Amount),
                        //Decrease = g.Where(t => (t.Order != null || t.WithDrawRequest != null) && t.Status == "PAID").Sum(t => t.Amount),
                    })
                    .OrderByDescending(x => x.Year)
                    .ThenByDescending(x => x.Month)
                    .ToList();

            return new
            {
                totalBalance = existedUser.BuyerWallet.Balance,
                totalTopUpRequestAmount = existedUser.MoneyRequests.Where(x => x.TopUpWalletTransaction.Status.Equals("PAID")).Sum(x => x.TopUpWalletTransaction.Amount),
                totalWithdrawRequestAmount = existedUser.MoneyRequests.Where(x => x.WithdrawWalletTransaction.Status.Equals("PAID")).Sum(x => x.WithdrawWalletTransaction.Amount),
                totalOrderAmount = existedUser.Orders.Sum(x => x.FinalPrice),
                numerOfTransaction = new
                {
                    topUpRequest = existedUser.MoneyRequests.Count(x => x.TopUpWalletTransaction != null),
                    withdrawRequest = existedUser.MoneyRequests.Count(x => x.WithdrawWalletTransaction != null),
                    order = existedUser.BuyerWallet.BuyerWalletTransactions.Count()
                },
                recentBuyerTransactions = _mapper.Map<IList<GetBuyerWalletTransactionDTO>>(recentBuyerTransactions.ToList()),
                dashboard = buyerTransactionDashboard
            };
        }
    }
}
