using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;
using static Vouchee.Business.Services.Impls.VoucherService;

namespace Vouchee.Business.Services
{
    public interface IVoucherService
    {
        // CREATE
        public Task<ResponseMessage<dynamic>> CreateVoucherAsync(CreateVoucherDTO createVoucherDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetDetailVoucherDTO> GetVoucherByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetAllVoucherDTO>> GetVouchersAsync(PagingRequest pagingRequest,
                                                                            VoucherFilter voucherFiler,
                                                                            decimal lon,
                                                                            decimal lat,
                                                                            List<Guid>? categoryIds);
        public Task<IList<GetNewestVoucherDTO>> GetNewestVouchers();
        //public Task<IList<GetAllVoucherDTO>> GetBestSoldVouchers();
        public Task<IList<GetNearestVoucherDTO>> GetNearestVouchers(decimal lon, decimal lat);
        public Task<IList<GetBestBuyVoucherDTO>> GetTopSaleVouchers();
        public Task<IList<GetNewestVoucherDTO>> GetSalestVouchers();
        public Task<DynamicResponseModel<GetNewestVoucherDTO>> GetVoucherBySellerId(Guid sellerId, PagingRequest pagingRequest);

        // UPDATE
        public Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO);

        // DELETE
        public Task<bool> DeleteVoucherAsync(Guid id);
    }
}
