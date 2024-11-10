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
        public Task<ResponseMessage<Guid>> CreateTopUpRequest(CreateTopUpRequestDTO createTopUpRequestDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetTopUpRequestDTO> GetTopUpRequestById(Guid id);
        public Task<DynamicResponseModel<GetTopUpRequestDTO>> GetTopUpRequestsAsync(PagingRequest pagingRequest, TopUpRequestFilter topUpRequestFilter);

        // UPDATE
        public Task<ResponseMessage<GetWalletDTO>> UpdateTopUpRequest(Guid id, bool success = false, string description = null, ThisUserObj currentUser = null); 
    }
}
