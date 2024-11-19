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
    public interface ITopUpRequestService
    {
        // CREATE
        public Task<ResponseMessage<string>> CreateTopUpRequest(CreateTopUpRequestDTO createTopUpRequestDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetTopUpRequestDTO> GetTopUpRequestById(Guid id);
        public Task<DynamicResponseModel<GetTopUpRequestDTO>> GetTopUpRequestsAsync(PagingRequest pagingRequest, TopUpRequestFilter topUpRequestFilter);

        // UPDATE
        public Task<ResponseMessage<GetSellerWallet>> UpdateTopUpRequest(Guid id, Guid partTransactionId, ThisUserObj currentUser = null); 
    }
}
