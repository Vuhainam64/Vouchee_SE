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
using Vouchee.Business.Models.DTOs;
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
            var brand = _mapper.Map<Brand>(createBrandDTO);
            brand.CreateBy = Guid.Parse(thisUserObj.userId);

            var brandId = await _brandRepository.AddAsync(brand);

            if (brandId != null && createBrandDTO.image != null)
            {
                brand.Image = await _fileUploadService.UploadImageToFirebase(createBrandDTO.image, thisUserObj.userId, StoragePathEnum.BRAND);

                await _brandRepository.UpdateAsync(brand);
            }

            return brandId;
        }

        public Task<bool> DeleteBrandAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<GetBrandDTO> GetBrandByIdAsync(Guid id)
        {
            try
            {
                var brand = await _brandRepository.GetByIdAsync(id);
                if (brand != null)
                {
                    var brandDTO = _mapper.Map<GetBrandDTO>(brand);
                    return brandDTO;
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy brand với id {id}");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải brand");
            }
        }

        public async Task<DynamicResponseModel<GetBrandDTO>> GetBrandsAsync(PagingRequest pagingRequest, BrandFilter brandFilter)
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
                    total = result.Item1 // Total vouchers count for metadata
                },
                results = result.Item2.ToList() // Return the paged voucher list with nearest address and distance
            };
        }

        public async Task<IList<GetBrandDTO>> GetBrandsbynameAsync(string name)
        {
            IQueryable<GetBrandDTO> result;
            try
            {
                result = _brandRepository.GetTable()
                            .Where(b => b.Name.Equals(name))
                            .ProjectTo<GetBrandDTO>(_mapper.ConfigurationProvider);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải brand");
            }
            return result.ToList();
        }

        public Task<bool> UpdateBrandAsync(Guid id, UpdateBrandDTO updateBrandDTO)
        {
            throw new NotImplementedException();
        }
    }
}
