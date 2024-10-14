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
        public Task<GetDetailVoucherDTO> GetVoucherByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetAllVoucherDTO>> GetVouchersAsync(PagingRequest pagingRequest,
                                                                            VoucherFilter voucherFiler,
                                                                            decimal lon,
                                                                            decimal lat,
                                                                            decimal maxDistance,
                                                                            List<Guid>? categoryIds);
        public Task<IList<GetAllVoucherDTO>> GetNewestVouchers();
        public Task<IList<GetAllVoucherDTO>> GetBestSoldVouchers();
        public Task<IList<GetAllVoucherDTO>> GetNearestVouchers(PagingRequest pagingRequest, decimal lon, decimal lat);
        public Task<IList<GetAllVoucherDTO>> GetTopSaleVouchers(PagingRequest pagingRequest);

        // UPDATE
        public Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO);

        // DELETE
        public Task<bool> DeleteVoucherAsync(Guid id);
        public Task<IList<GetAllVoucherDTO>> GetNearestVouchers(decimal lon, decimal lat);
    }
}
