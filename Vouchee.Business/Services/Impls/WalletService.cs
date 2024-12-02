using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services.Impls
{
    public class WalletService : IWalletService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Wallet> _walletRepository;
        private readonly IMapper _mapper;

        public WalletService(IBaseRepository<User> userRepository,
                             IBaseRepository<Wallet> walletRepository,
                             IMapper mapper)
        {
            _userRepository = userRepository;
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<GetUserDTO>> CreateWalletAsync(ThisUserObj currenUser)
        {
            try
            {
                var existedUser = await _userRepository.GetByIdAsync(currenUser.userId,
                                                                        includeProperties: x => x.Include(x => x.BuyerWallet)
                                                                                                    .Include(x => x.SellerWallet),
                                                                        isTracking: true);

                if (existedUser == null)
                {
                    throw new NotFoundException("Không tìm thấy user này");
                }

                if (existedUser.BuyerWallet == null)
                {
                    existedUser.BuyerWallet = new()
                    {
                        CreateDate = DateTime.Now,
                        Status = ObjectStatusEnum.ACTIVE.ToString()
                    };
                }
                if (existedUser.SellerWallet == null)
                {
                    existedUser.SellerWallet = new()
                    {
                        CreateDate = DateTime.Now,
                        Status = ObjectStatusEnum.ACTIVE.ToString()
                    };
                }

                var result = await _userRepository.SaveChanges();

                if (result)
                {
                    return new ResponseMessage<GetUserDTO>()
                    {
                        message = "Tạo ví thành công",
                        result = true,
                        value = _mapper.Map<GetUserDTO>(existedUser)
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<dynamic> GetBuyerWalletAsync(ThisUserObj currentUser, PagingRequest pagingRequest, BuyerWalletTransactionFilter buyerWalletTransactionFilter)
        {
            // Retrieve the user along with their BuyerWallet and associated transactions
            var user = await _userRepository.GetByIdAsync(currentUser.userId, includeProperties: x => x.Include(x => x.BuyerWallet.BuyerWalletTransactions));

            if (user.BuyerWallet == null)
            {
                throw new NotFoundException("Người dùng này chưa có ví buyer");
            }

            // Apply filtering based on the filter criteria
            if (buyerWalletTransactionFilter.startDate.HasValue)
            {
                user.BuyerWallet.BuyerWalletTransactions = user.BuyerWallet.BuyerWalletTransactions
                    .Where(t => t.CreateDate >= buyerWalletTransactionFilter.startDate.Value).ToList();
            }

            if (buyerWalletTransactionFilter.endDate.HasValue)
            {
                user.BuyerWallet.BuyerWalletTransactions = user.BuyerWallet.BuyerWalletTransactions
                    .Where(t => t.CreateDate <= buyerWalletTransactionFilter.endDate.Value).ToList();
            }

            if (!string.IsNullOrEmpty(buyerWalletTransactionFilter.orderId))
            {
                user.BuyerWallet.BuyerWalletTransactions = user.BuyerWallet.BuyerWalletTransactions
                    .Where(t => t.OrderId == buyerWalletTransactionFilter.orderId).ToList();
            }

            // Create chart data by grouping transactions by month and year
            var chartData = user.BuyerWallet.BuyerWalletTransactions
                .GroupBy(t => new
                {
                    Year = t.CreateDate.Value.Year,
                    Month = t.CreateDate.Value.Month
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month) // Ensure the data is ordered by year and month
                .ToList();

            // Count total transactions after filtering
            var totalTransactions = user.BuyerWallet.BuyerWalletTransactions.Count();

            // Implement pagination
            var transactions = user.BuyerWallet.BuyerWalletTransactions
                .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                .Take(pagingRequest.pageSize)
                .ToList();

            // Prepare the result object
            var result = new
            {
                metadata = new
                {
                    page = pagingRequest.page,
                    pageSize = pagingRequest.pageSize,
                    total = totalTransactions
                },
                chartData,
                walletId = user.BuyerWallet.Id,
                balance = user.BuyerWallet.Balance,
                status = user.BuyerWallet.Status,
                buyerWalletTransactions = _mapper.Map<IList<GetBuyerWalletTransactionDTO>>(transactions),
            };

            return result;
        }

        public async Task<dynamic> GetSellerWalletAsync(ThisUserObj currentUser, PagingRequest pagingRequest, SellerWalletTransactionFilter sellerWalletTransactionFilter)
        {
            {
                var user = await _userRepository.GetByIdAsync(currentUser.userId, includeProperties: x => x.Include(x => x.SellerWallet.SellerWalletTransactions));

                if (user.SellerWallet == null)
                {
                    throw new NotFoundException("Người dùng này chưa có ví seller");
                }

                if (sellerWalletTransactionFilter.startDate.HasValue)
                {
                    user.SellerWallet.SellerWalletTransactions = user.SellerWallet.SellerWalletTransactions
                                                                    .Where(t => t.CreateDate >= sellerWalletTransactionFilter.startDate.Value).ToList();
                }

                if (sellerWalletTransactionFilter.endDate.HasValue)
                {
                    user.SellerWallet.SellerWalletTransactions = user.SellerWallet.SellerWalletTransactions
                                                                    .Where(t => t.CreateDate <= sellerWalletTransactionFilter.endDate.Value).ToList();
                }

                if (!string.IsNullOrEmpty(sellerWalletTransactionFilter.orderId))
                {
                    user.SellerWallet.SellerWalletTransactions = user.SellerWallet.SellerWalletTransactions
                                                                    .Where(t => t.OrderId == sellerWalletTransactionFilter.orderId).ToList();
                }

                var chartData = user.SellerWallet.SellerWalletTransactions.ToList()
                                    .GroupBy(t => new
                                    {
                                        Year = t.CreateDate.Value.Year,
                                        Month = t.CreateDate.Value.Month
                                    })
                                    .Select(g => new
                                    {
                                        Year = g.Key.Year,
                                        Month = g.Key.Month,
                                        TotalAmount = g.Sum(t => t.Amount)
                                    })
                                    .OrderBy(x => x.Year).ThenBy(x => x.Month) // Ensure the data is ordered by year and month
                                    .ToList();

                var totalTransactions = user.SellerWallet.SellerWalletTransactions.Count();
                var transactions = user.SellerWallet.SellerWalletTransactions
                                    .Skip((pagingRequest.page - 1) * pagingRequest.pageSize)
                                    .Take(pagingRequest.pageSize)
                                    .ToList();

                var result = new
                {
                    metadata = new
                    {
                        page = pagingRequest.page,
                        pageSize = pagingRequest.pageSize,
                        total = totalTransactions
                    },
                    chartData,
                    walletId = user.SellerWallet.Id,
                    balance = user.SellerWallet.Balance,
                    status = user.SellerWallet.Status,
                    sellerWalletTransactions = _mapper.Map<IList<GetSellerWalletTransaction>>(transactions),
                };

                return result;
            }
        }
    }
}
