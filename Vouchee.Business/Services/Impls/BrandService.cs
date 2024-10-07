using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;

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

        public Task<Guid?> CreateBrandAsync(CreateBrandDTO createBrandDTO, ThisUserObj thisUserObj)
        {
            try
            {
                var brand = _mapper.Map<Brand>(createBrandDTO);
                brand.CreateBy = Guid.Parse(thisUserObj.userId);

                var brandId = _brandRepository.AddAsync(brand);

                return null;
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

        public Task<DynamicResponseModel<GetBrandDTO>> GetBrandsAsync(PagingRequest pagingRequest, BrandFilter voucherCodeFilter, SortEnum sortEnum)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateBrandAsync(Guid id, UpdateBrandDTO updateBrandDTO)
        {
            throw new NotImplementedException();
        }
    }
}
