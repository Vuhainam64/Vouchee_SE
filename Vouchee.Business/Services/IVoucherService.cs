using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
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
        public Task<DynamicResponseModel<GetVoucherDTO>> GetVoucherAsync(PagingRequest pagingRequest,
                                                                            VoucherFilter voucherFilter,
                                                                            IList<Guid>? categoryIds);
        public Task<DynamicDistanceResponseModel<GetDetailVoucherDTO>> GetDetailVouchersAsync(DistanceFilter distanceFilter,
                                                                                        VoucherFilter voucherFiler,
                                                                                        IList<Guid>? categoryIds);
        public Task<IList<GetVoucherDTO>> GetNewestVouchers(int numberOfVoucher = 8);
        public Task<IList<GetBestSoldVoucherDTO>> GetTopSaleVouchers(int numberOFVoucher = 8);
        public Task<IList<GetVoucherDTO>> GetSalestVouchers(int numberOfVoucher = 8);
        public Task<DynamicResponseModel<GetVoucherDTO>> GetVoucherBySellerId(Guid sellerId, 
                                                                                PagingRequest pagingRequest, 
                                                                                VoucherFilter voucherFilter,
                                                                                IList<Guid>? categoryIds);

        // UPDATE
        public Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO);

        // DELETE
        public Task<bool> DeleteVoucherAsync(Guid id);
    }
}
