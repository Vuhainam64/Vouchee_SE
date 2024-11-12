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
        private readonly IBaseRepository<WalletTransaction> _walletTransaction;
        private readonly IMapper _mapper;

        public WalletTransactionService(IBaseRepository<WalletTransaction> walletTransaction,
                                            IMapper mapper)
        {
            _walletTransaction = walletTransaction;
            _mapper = mapper;
        }

        public async Task<DynamicResponseModel<GetSellerWalletTransaction>> GetWalletTransactionsAsync(PagingRequest pagingRequest, 
                                                                                                WalletTransactionFilter walletTransactionFilter, 
                                                                                                ThisUserObj currentUser, 
                                                                                                WalletTransactionTypeEnum walletTransactionTypeEnum)
        {
            (int, IQueryable<GetSellerWalletTransaction>) result;
            try
            {
                result = _walletTransaction.GetTable()
                            .Where(x => x.SellerWalletId == currentUser.userRoleId)
                            .ProjectTo<GetSellerWalletTransaction>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetSellerWalletTransaction>(walletTransactionFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
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
