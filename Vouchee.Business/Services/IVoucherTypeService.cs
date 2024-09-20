using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.Constants.Enum;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models.Helpers;

namespace Vouchee.Business.Services
{
    public interface IVoucherTypeService
    {
        // CREATE
        public Task<Guid?> CreateVoucherTypeAsync(CreateVoucherTypeDTO createVoucherTypeDTO);

        // READ
        public Task<GetVoucherTypeDTO> GetVoucherTypeByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetVoucherTypeDTO>> GetVoucherTypesAsync(PagingRequest pagingRequest,
                                                                                    VoucherTypeFilter voucherTypeFilter,
                                                                                    SortVoucherTypeEnum sortVoucherTypeEnum);

        // UPDATE
        public Task<bool> UpdateVoucherTypeAsync(Guid id, UpdateVoucherTypeDTO updateVoucherTypeDTO);

        // DELETE
        public Task<bool> DeleteVoucherTypeAsync(Guid id);
    }
}
