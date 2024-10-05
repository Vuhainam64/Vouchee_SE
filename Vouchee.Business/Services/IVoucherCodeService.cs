using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IVoucherCodeService
    {
        // CREATE
        public Task<Guid?> CreateVoucherCodeAsync(Guid voucherId, CreateVoucherCodeDTO createVoucherCodeDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetVoucherDTO> GetVoucherCodeByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetVoucherDTO>> GetVoucherCodesAsync(PagingRequest pagingRequest,
                                                                                VoucherCodeFilter voucherCodeFilter,
                                                                                SortVoucherCodeEnum sortVoucherCodeEnum);

        // UPDATE
        public Task<bool> UpdateVoucherAsync(Guid id, UpdateVoucherDTO updateVoucherDTO);

        // DELETE
        public Task<bool> DeleteVoucherAsync(Guid id);
    }
}
