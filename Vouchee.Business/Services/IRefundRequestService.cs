using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IRefundRequestService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateRefundRequestAsync(CreateRefundRequestDTO createRefundRequestDTO, ThisUserObj thisUserObj);

        // READ
        public Task<DynamicResponseModel<GetRefundRequestDTO>> GetAllRefundRequestAsync(PagingRequest pagingRequest, RefundRequestFilter refundRequestFilter);
        public Task<DynamicResponseModel<GetRefundRequestDTO>> GetSupplierRefundRequestAsync(ThisUserObj thisUserObj, PagingRequest pagingRequest, RefundRequestFilter refundRequestFilter);
        public Task<GetRefundRequestDTO> GetRefundRequestByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetRefundRequestDTO>> GetBuyerRefundRequestAsync(ThisUserObj thisUserObj, PagingRequest pagingRequest, RefundRequestFilter refundRequestFilter);

        // UPDATE
        public Task<ResponseMessage<bool>> UpdateRefundRequestAsync(Guid id, UpdateRefundRequestDTO updateRefundRequestDTO, ThisUserObj thisUserObj);
        public Task<ResponseMessage<bool>> UpdateRefundRequestStatusAsync(Guid id, RefundRequestStatusEnum refundRequestStatusEnum, string reason, ThisUserObj thisUserObj);

        // DELETE
        public Task<ResponseMessage<bool>> DeleteRefundRequestAsync(Guid id);
    }
}
