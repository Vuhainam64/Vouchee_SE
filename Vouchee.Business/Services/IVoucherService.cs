﻿using Vouchee.Business.Models;
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
        public Task<GetAllVoucherDTO> GetVoucherByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetAllVoucherDTO>> GetVouchersAsync(PagingRequest pagingRequest,
                                                                            VoucherFilter voucherFiler);
        public Task<IList<GetAllVoucherDTO>> GetNewestVouchers();
        public Task<IList<GetAllVoucherDTO>> GetBestSoldVouchers();
        public Task<IList<GetAllVoucherDTO>> GetNearestVouchers(PagingRequest pagingRequest, decimal lon, decimal lat);
        public Task<IList<GetAllVoucherDTO>> GetTopSaleVouchers(PagingRequest pagingRequest);

        // UPDATE
        public Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO);

        // DELETE
        public Task<bool> DeleteVoucherAsync(Guid id);
    }
}
