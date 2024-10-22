using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IModalService
    {
        // CREATE
        public Task<Guid?> CreateModalAsync(Guid voucherId, CreateModalDTO createModalDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetModalDTO> GetModalByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetModalDTO>> GetModalsAsync(PagingRequest pagingRequest, ModalFilter modalFilter);

        // UPDATE
        public Task<bool> UpdateModalAsync(Guid id, UpdateModalDTO updateModalDTO);

        // DELETE
        public Task<bool> DeleteModalAsync(Guid id);
    }
}
