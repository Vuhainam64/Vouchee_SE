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
    public interface ICategoryService
    {
        // CREATE
        public Task<ResponseMessage<Guid>> CreateCategoryAsync(Guid voucherTypeId, CreateCategoryDTO createCategoryDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetDetailCategoryDTO> GetCategoryByIdAsync(Guid id);
        public Task<DynamicResponseModel<GetCategoryDTO>> GetCategoriesAsync(PagingRequest pagingRequest, CategoryFilter categoryFilter);

        // UPDATE
        public Task<ResponseMessage<bool>> UpdateCategoryAsync(Guid id, UpdateCategoryDTO updateCategoryDTO, ThisUserObj currentUser);
        public Task<ResponseMessage<bool>> UpdateCategoryStateAsync(Guid id, bool isActive, ThisUserObj currentUser);
    }
}
