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
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;

        public BrandService(IBrandRepository brandRepository,
                                IFileUploadService fileUploadService, 
                                IMapper mapper)
        {
            _brandRepository = brandRepository;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateBrandAsync(CreateBrandDTO createBrandDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var brand = _mapper.Map<Brand>(createBrandDTO);
                brand.CreateBy = Guid.Parse(thisUserObj.userId);

                var brandId = await _brandRepository.AddAsync(brand);

                if (brandId != null && createBrandDTO.image != null)
                {
                    brand.Image = await _fileUploadService.UploadImageToFirebase(createBrandDTO.image, thisUserObj.userId, StoragePathEnum.BRAND);

                    if(!await _brandRepository.UpdateAsync(brand))
                    {
                        throw new UpdateObjectException("Lỗi không xác định khi cập nhật brand");
                    }
                }

                return brandId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException("Lỗi không xác định khi tạo brand");
            }
        }

        public Task<bool> DeleteBrandAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GetBrandDTO> GetBrandByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<DynamicResponseModel<GetBrandDTO>> GetBrandsAsync(PagingRequest pagingRequest, BrandFilter brandFilter, SortEnum sortEnum)
        {
            (int, IQueryable<GetBrandDTO>) result;
            try
            {
                result = _brandRepository.GetTable()
                            .ProjectTo<GetBrandDTO>(_mapper.ConfigurationProvider)
                            .DynamicFilter(_mapper.Map<GetBrandDTO>(brandFilter))
                            .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải brand");
            }
            return new DynamicResponseModel<GetBrandDTO>()
            {
                metaData = new MetaData()
                {
                    page = pagingRequest.page,
                    size = pagingRequest.pageSize,
                    total = result.Item1
                },
                results = result.Item2.ToList()
            };
        }

        public Task<bool> UpdateBrandAsync(Guid id, UpdateBrandDTO updateBrandDTO)
        {
            throw new NotImplementedException();
        }
    }
}
