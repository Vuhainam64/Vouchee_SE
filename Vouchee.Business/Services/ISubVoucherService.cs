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
    public interface ISubVoucherService
    {
        // CREATE
        public Task<Guid?> CreateSubVoucherAsync(Guid voucherId, CreateSubVoucherDTO createSubVoucherDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetSubVoucherDTO> GetSubVoucherByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetSubVoucherDTO>> GetSubVouchersAsync(PagingRequest pagingRequest, SubVoucherFilter subVoucherFilter);

        // UPDATE
        public Task<bool> UpdateSubVoucherAsync(Guid id, UpdateSubVoucherDTO updateSubVoucherDTO);

        // DELETE
        public Task<bool> DeleteSubVoucherAsync(Guid id);
    }
}
