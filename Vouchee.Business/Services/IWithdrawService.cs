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
        public Task<DynamicResponseModel<dynamic>> GetWithdrawWalletTransactionByUpdateId(PagingRequest pagingRequest, WithdrawRequestFilter walletTransactionFilter);

        // UPDATE
        public Task<ResponseMessage<Guid>> UpdateWithdrawRequest(List<UpdateWithDrawRequestDTO> updateWithDrawRequestDTOs, ThisUserObj thisUserObj);

        public Task<ResponseMessage<bool>> UpdateWithdrawRequest(string id, int amount, ThisUserObj thisUserObj);

        // DELETE
        public Task<ResponseMessage<bool>> DeleteWithdrawRequest(string id, ThisUserObj thisUserObj);
    }
}
