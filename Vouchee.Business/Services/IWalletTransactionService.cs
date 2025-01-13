using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IWalletTransactionService
    {
        // READ
        public Task<DynamicResponseModel<GetSellerWalletTransaction>> GetSellerWalletTransactionsAsync(PagingRequest pagingRequest,
                                                                                                            SellerWalletTransactionFilter sellerWalletTransactionFilter,
                                                                                                            ThisUserObj currentUser);
        public Task<DynamicResponseModel<GetBuyerWalletTransactionDTO>> GetBuyerWalletTransactionsAsync(PagingRequest pagingRequest,
                                                                                                            BuyerWalletTransactionFilter buyerWalletTransactionFilter,
                                                                                                            ThisUserObj currentUser);

        public Task<DynamicResponseModel<GetWalletTransactionDTO>> GetWalletTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter);

        public Task<dynamic> GetWalletTransactionsAsync(ThisUserObj currentUser);


        // -- //

        public Task<DynamicResponseModel<GetSellerWalletTransaction>> GetSellerInTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj);
        public Task<DynamicResponseModel<GetSellerWalletTransaction>> GetSellerOutTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj);
        public Task<DynamicResponseModel<GetBuyerWalletTransactionDTO>> GetBuyerInTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj);
        public Task<DynamicResponseModel<GetBuyerWalletTransactionDTO>> GetBuyerOutTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj);
        public Task<DynamicResponseModel<GetSupplierWalletTransactionDTO>> GetSupplierInTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj);
        public Task<DynamicResponseModel<GetSupplierWalletTransactionDTO>> GetSupplierOutTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj);
    }
}