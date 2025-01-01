using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
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
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IBaseRepository<VoucherType> _voucherTypeRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IBaseRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IBaseRepository<Voucher> voucherRepository,
                               IBaseRepository<VoucherType> voucherTypeRepository,
                               IFileUploadService fileUploadService,
                               IBaseRepository<Category> categoryRepository,
                               IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _voucherTypeRepository = voucherTypeRepository;
            _fileUploadService = fileUploadService;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateCategoryAsync(Guid voucherTypeId, 
                                                        CreateCategoryDTO createCategoryDTO, 
                                                        ThisUserObj thisUserObj)
        {
            var existedVoucherType = await _voucherTypeRepository.FindAsync(voucherTypeId, isTracking: true);

            if (existedVoucherType == null)
            {
                throw new NotFoundException("Không tìm thấy voucher type này");
            }    

            Category category = _mapper.Map<Category>(createCategoryDTO);
            category.CreateBy = thisUserObj.userId;
            category.VoucherTypeId = voucherTypeId;

            var categoryId = await _categoryRepository.AddAsync(category);

            return new ResponseMessage<Guid>()
            {
                message = "Tạo danh mục thành công",
                result = true,
                value = (Guid) categoryId
            };
        }

        public async Task<ResponseMessage<bool>> DeleteCategoryAsync(Guid id, ThisUserObj thisUserObj)
        {
            var existedCategory = await _categoryRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.Vouchers), isTracking: true);

            if (existedCategory == null)
            {
                throw new NotFoundException("Không thấy category này");
            }

            if (existedCategory.Vouchers.Count != 0)
            {
                existedCategory.UpdateDate = DateTime.Now;
                existedCategory.IsActive = false;
                existedCategory.UpdateBy = thisUserObj.userId;

                await _categoryRepository.SaveChanges();

                return new ResponseMessage<bool>()
                {
                    message = "Cập nhật category thành công",
                    result = true,
                    value = true
                };
            }

            await _categoryRepository.DeleteAsync(existedCategory);

            return new ResponseMessage<bool>()
            {
                message = "Xóa category thành công",
                result = true,
                value = true
            };
        }

        public async Task<DynamicResponseModel<GetCategoryDTO>> GetCategoriesAsync(PagingRequest pagingRequest, CategoryFilter categoryFilter)
        {
            (int, IQueryable<GetCategoryDTO>) result;

            result = _categoryRepository.GetTable()
                        .ProjectTo<GetCategoryDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetCategoryDTO>(categoryFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetCategoryDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = await result.Item2.ToListAsync() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<GetDetailCategoryDTO> GetCategoryByIdAsync(Guid id)
        {
            var existedCategory = await _categoryRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.Vouchers)
                                                                                                            .ThenInclude(x => x.Medias)
                                                                                                        .Include(x => x.Vouchers)
                                                                                                            .ThenInclude(x => x.Modals)
                                                                                                        .Include(x => x.VoucherType));
            
            if (existedCategory == null)
            {
                throw new NotFoundException("Không tìm thấy category");
            }

            return _mapper.Map<GetDetailCategoryDTO>(existedCategory);
        }

        public async Task<ResponseMessage<bool>> RemoveCategoryFromVoucherAsync(Guid categoryId, Guid voucherId)
        {
            var existedVoucher = await _voucherRepository.GetByIdAsync(voucherId, includeProperties: x => x.Include(x => x.Categories), isTracking: true);

            if (existedVoucher == null)
            {
                throw new NotFoundException("Không tìm thấy voucher này");
            }

            var existedCategory = existedVoucher.Categories.FirstOrDefault(x => x.Id == categoryId);

            if (existedCategory == null)
            {
                throw new NotFoundException("Không tìm thấy category trong voucher này");
            }

            existedVoucher.Categories.Remove(existedCategory);

            await _categoryRepository.SaveChanges();

            return new ResponseMessage<bool>()
            {
                message = "Xóa category ra khỏi voucher thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> UpdateCategoryAsync(Guid id, UpdateCategoryDTO updateCategoryDTO, ThisUserObj currentUser)
        {
            var existedCategory = await _categoryRepository.GetByIdAsync(id, isTracking: true);

            if (existedCategory == null)
            {
                throw new NotFoundException("Không tìm thấy category");
            }

            existedCategory = _mapper.Map(updateCategoryDTO, existedCategory);
            existedCategory.UpdateBy = currentUser.userId;

            await _categoryRepository.UpdateAsync(existedCategory);

            return new ResponseMessage<bool>
            {
                message = "Cập nhật thông tin thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> UpdateCategoryStateAsync(Guid id, bool isActive, ThisUserObj currentUser)
        {
            var existedCategory = await _categoryRepository.GetByIdAsync(id, isTracking: true);

            if (existedCategory == null)
            {
                throw new NotFoundException("Không tìm thấy category");
            }

            existedCategory.IsActive = isActive;
            existedCategory.UpdateDate = DateTime.Now;
            existedCategory.UpdateBy = currentUser.userId;

            await _categoryRepository.UpdateAsync(existedCategory);

            return new ResponseMessage<bool>
            {
                message = "Cập nhật thông tin thành công",
                result = true,
                value = true
            };
        }
    }
}
