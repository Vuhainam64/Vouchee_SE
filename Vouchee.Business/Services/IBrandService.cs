using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Models;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Services
{
    public interface IBrandService
    {
        // CREATE
        public Task<Guid?> CreateBrandAsync(CreateBrandDTO createBrandDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetBrandDTO> GetBrandByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetBrandDTO>> GetBrandsAsync(PagingRequest pagingRequest,
                                                                                BrandFilter voucherCodeFilter,
                                                                                SortEnum sortEnum);

        // UPDATE
        public Task<bool> UpdateBrandAsync(Guid id, UpdateBrandDTO updateBrandDTO);

        // DELETE
        public Task<bool> DeleteBrandAsync(Guid id);
    }
}
