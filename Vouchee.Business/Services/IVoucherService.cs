using Google.Rpc;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.ViewModels;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Filters;
using static Vouchee.Business.Services.Impls.VoucherService;

namespace Vouchee.Business.Services
{
    public interface IVoucherService
    {
        // CREATE
        public Task<ResponseMessage<dynamic>> CreateVoucherAsync(CreateVoucherDTO createVoucherDTO, ThisUserObj thisUserObj);

        // READ
        public Task<dynamic> GetVoucherByIdAsync(Guid id, PagingRequest pagingRequest);
        public Task<DynamicResponseModel<GetVoucherDTO>> GetVoucherAsync(PagingRequest pagingRequest,
                                                                            VoucherFilter voucherFilter,
                                                                            SortVoucherEnum sortVoucherEnum);
        public Task<DynamicResponseModel<GetNearestVoucherDTO>> GetNearestVouchersAsync(PagingRequest pagingRequest,
                                                                                            DistanceFilter distanceFilter,
                                                                                            VoucherFilter voucherFiler,
                                                                                            IList<Guid>? categoryIds);
        public Task<IList<GetVoucherDTO>> GetNewestVouchers(int numberOfVoucher);
        public Task<IList<GetVoucherDTO>> GetTopSaleVouchers(int numberOFVoucher);
        public Task<IList<GetVoucherDTO>> GetSalestVouchers(int numberOfVoucher);
        public Task<DynamicResponseModel<GetVoucherSellerDTO>> GetVoucherBySellerId(Guid sellerId, 
                                                                                        PagingRequest pagingRequest, 
                                                                                        VoucherFilter voucherFilter,
                                                                                        IList<Guid>? categoryIds);

        public Task<IList<MiniVoucher>> GetMiniVoucherAsync(string title);

        // UPDATE
        public Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO);
        public Task<ResponseMessage<GetVoucherDTO>> UpdateVoucherStatusAsync(Guid id, VoucherStatusEnum voucherStatus);
        public Task<ResponseMessage<GetVoucherDTO>> UpdateVoucherisActiveAsync(Guid id, bool isActive);
        public Task<ResponseMessage<bool>> RemoveCategoryFromVoucherAsync(Guid categoryId, Guid voucherId, ThisUserObj thisUserObj);

        // DELETE
        public Task<ResponseMessage<bool>> DeleteVoucherAsync(Guid id);
    }
}
