using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IWithdrawService
    {
        // CREATE
        public Task<ResponseMessage<string>> CreateWithdrawRequestAsync(WalletTypeEnum walletTypeEnum, CreateWithdrawRequestDTO createWithdrawRequestDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetWithdrawRequestDTO> GetWithdrawRequestById(string id);
        public Task<DynamicResponseModel<GetWithdrawRequestDTO>> GetWithdrawRequestAsync(PagingRequest pagingRequest, WithdrawRequestFilter withdrawRequestFilter);
        public Task<DynamicResponseModel<GetWithdrawRequestDTO>> GetWithdrawRequestAsync(PagingRequest pagingRequest, WithdrawRequestFilter withdrawRequestFilter, ThisUserObj thisUserObj);
    }
}
