using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IVoucherTypeService
    {
        // CREATE
        public Task<Guid?> CreateVoucherTypeAsync(CreateVoucherTypeDTO createVoucherTypeDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetVoucherTypeDTO> GetVoucherTypeByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetVoucherTypeDTO>> GetVoucherTypesAsync(PagingRequest pagingRequest, VoucherTypeFilter voucherTypeFilter);

        // UPDATE
        public Task<bool> UpdateVoucherTypeAsync(Guid id, UpdateVoucherTypeDTO updateVoucherTypeDTO);

        // DELETE
        public Task<bool> DeleteVoucherTypeAsync(Guid id);
    }
}
