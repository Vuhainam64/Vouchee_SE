using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

namespace Vouchee.Business.Services.Impls
{
    public class CategoryService : ICategoryService
    {
        private readonly IVoucherTypeRepository _voucherTypeRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IVoucherTypeRepository voucherTypeRepository,
                                IFileUploadService fileUploadService,
                                ICategoryRepository categoryRepository, 
                                IMapper mapper)
        {
            _voucherTypeRepository = voucherTypeRepository;
            _fileUploadService = fileUploadService;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateCategoryAsync(Guid voucherTypeId, 
                                                CreateCategoryDTO createCategoryDTO, 
                                                ThisUserObj thisUserObj)
        {
            try
            {
                var existedVoucherType = await _voucherTypeRepository.FindAsync(voucherTypeId);

                if (existedVoucherType == null)
                {
                    throw new NotFoundException("Không tìm thấy voucher type");
                }    

                Category category = _mapper.Map<Category>(createCategoryDTO);
                category.CreateBy = thisUserObj.userId;
                category.VoucherTypeId = voucherTypeId;

                var categoryId = await _categoryRepository.AddAsync(category);

                if (categoryId != null && createCategoryDTO.image != null)
                {
                    category.Image = await _fileUploadService.UploadImageToFirebase(createCategoryDTO.image, thisUserObj.userId.ToString(), StoragePathEnum.CATEGORY);

                    if (!await _categoryRepository.UpdateAsync(category)) 
                    {
                        throw new UpdateObjectException("Không thể update category");
                    }
                }

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

        public async Task<DynamicResponseModel<GetCategoryDTO>> GetCategoriesAsync(PagingRequest pagingRequest, CategoryFilter categoryFilter)
        {
            (int, IQueryable<GetCategoryDTO>) result;
            try
            {
                result = _categoryRepository.GetTable()
                            .ProjectTo<GetCategoryDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetCategoryDTO>(categoryFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải category");
            }
            return new DynamicResponseModel<GetCategoryDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = result.Item2.ToList() // Return the paged voucher list with nearest address and distance
            };
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
