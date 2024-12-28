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
        public Task<GetTopUpRequestDTO> GetTopUpRequestById(string id);
        public Task<DynamicResponseModel<GetTopUpRequestDTO>> GetTopUpRequestsAsync(PagingRequest pagingRequest, TopUpRequestFilter topUpRequestFilter);
        public Task<DynamicResponseModel<GetTopUpRequestDTO>> GetUserTopUpRequestsAsync(PagingRequest pagingRequest, TopUpRequestFilter topUpRequestFilter, ThisUserObj thisUserObj);

        // UPDATE
        public Task<ResponseMessage<bool>> UpdateTopUpRequest(Guid id, int amount, ThisUserObj currentUser);

        // DELETE
        public Task<ResponseMessage<bool>> DeleteTopUpRequest(Guid id, ThisUserObj currentUser);
    }
}
