using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IWithdrawService
    {
        // CREATE
        public Task<ResponseMessage<string>> CreateWithdrawRequestAsync(WalletTypeEnum walletTypeEnum, CreateWithdrawRequestDTO createWithdrawRequestDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> CreateWithdrawRequestInAllWalletAsync();

        // READ
        public Task<GetWithdrawRequestDTO> GetWithdrawRequestById(string id);
        public Task<DynamicResponseModel<GetWithdrawRequestDTO>> GetWithdrawRequestAsync(PagingRequest pagingRequest, WithdrawRequestFilter withdrawRequestFilter);
        public Task<DynamicResponseModel<GetWithdrawRequestDTO>> GetWithdrawRequestAsync(PagingRequest pagingRequest, WithdrawRequestFilter withdrawRequestFilter, ThisUserObj thisUserObj);
        public Task<DynamicResponseModel<GetWalletTransactionDTO>> GetWithdrawWalletTransactionAsync(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, ThisUserObj thisUserObj);
        public Task<dynamic> GetWithdrawRequestbyMonthAsync(WithdrawRequestFilter withdrawRequestFilter);
        public Task<DynamicResponseModel<GetWalletTransactionDTO>> GetWithdrawWalletTransactionByUpdateId(PagingRequest pagingRequest, WalletTransactionFilter walletTransactionFilter, Guid updateId);

        // UPDATE
        public Task<ResponseMessage<Guid>> UpdateWithdrawRequest(List<UpdateWithDrawRequestDTO> updateWithDrawRequestDTOs, ThisUserObj thisUserObj);
    }
}
