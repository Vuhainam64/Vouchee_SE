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
                                    x.CreateDate >= startDateTime &&
                                    x.CreateDate <= endDateTime &&
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
    }
}
