using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services
{
    public interface IVoucherService
    {
        // CREATE
        public Task<Guid?> CreateVoucherAsync(CreateVoucherDTO createVoucherDTO);

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
