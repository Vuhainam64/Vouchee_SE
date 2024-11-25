using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IModalService
    {
        // CREATE
        public Task<Guid?> CreateModalAsync(Guid voucherId, CreateModalDTO createModalDTO, ThisUserObj thisUserObj);

        // READ
        public Task<dynamic> GetModalByIdAsync(Guid id, PagingRequest pagingRequest);
        public Task<DynamicResponseModel<GetModalDTO>> GetModalsAsync(PagingRequest pagingRequest, ModalFilter modalFilter);
        //public Task<DynamicResponseModel<GetOrderedModalDTO>> GetOrderedModals(Guid buyerId, PagingRequest pagingRequest, ModalFilter modalFilter);
        //public Task<DynamicResponseModel<GetPendingModalDTO>> GetPendingModals(Guid sellerId, PagingRequest pagingRequest, ModalFilter modalFilter);

        // UPDATE
        public Task<bool> UpdateModalAsync(Guid id, UpdateModalDTO updateModalDTO);
        public Task<ResponseMessage<GetModalDTO>> UpdateModalStatusAsync(Guid id, VoucherStatusEnum modalStatus);
        public Task<ResponseMessage<GetModalDTO>> UpdateModalisActiveAsync(Guid id, bool isActive);

        // DELETE
        public Task<ResponseMessage<bool>> DeleteModalAsync(Guid id);
    }
}
