using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IVoucherService
    {
        // CREATE
        public Task<Guid?> CreateVoucherAsync(CreateVoucherDTO createVoucherDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetVoucherDTO> GetVoucherByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetVoucherDTO>> GetVouchersAsync(PagingRequest pagingRequest,
                                                                            VoucherFiler voucherFiler,
                                                                            SortVoucherEnum sortVoucherEnum);

        // UPDATE
        public Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO);

        // DELETE
        public Task<bool> DeleteVoucherAsync(Guid id);
    }
}
