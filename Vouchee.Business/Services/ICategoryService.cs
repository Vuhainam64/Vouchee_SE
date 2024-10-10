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
        public Task<Guid?> CreateCategoryAsync(Guid voucherTypeId, CreateCategoryDTO createCategoryDTO, ThisUserObj thisUserObj);

        // READ
        public Task<GetCategoryDTO> GetCategoryByIdAsync(Guid id);
        public Task<IList<GetCategoryDTO>> GetCategoriesAsync();

        // UPDATE
        public Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryDTO updateCategoryDTO);

        // DELETE
        public Task<bool> DeleteCategoryAsync(Guid id);
    }
}
