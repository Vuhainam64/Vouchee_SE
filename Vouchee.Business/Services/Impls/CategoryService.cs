using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services.Impls
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, 
                                IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateCategoryAsync(Guid voucherTypeId, 
                                                CreateCategoryDTO createCategoryDTO, 
                                                ThisUserObj thisUserObj)
        {
            try
            {
                Category category = _mapper.Map<Category>(createCategoryDTO);

                var categoryId = await _categoryRepository.AddAsync(category);

                return categoryId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            try
            {
                var existedCategory = await _categoryRepository.FindAsync(id);

                if (existedCategory == null)
                {
                    throw new NotFoundException($"Không tìm thấy category với id {id}");
                }

                var result = await _categoryRepository.DeleteAsync(existedCategory);

                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public Task<DynamicResponseModel<GetCategoryDTO>> GetCategoriesAsync(PagingRequest pagingRequest, CategoryFilter categoryFilter, SortEnum sortEnum)
        {
            throw new NotImplementedException();
        }

        public Task<GetCategoryDTO> GetCategoryByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCategoryAsync(Guid id, UpdateCategoryDTO updateCategoryDTO)
        {
            throw new NotImplementedException();
        }
    }
}
