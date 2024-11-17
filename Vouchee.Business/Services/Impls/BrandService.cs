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
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class BrandService : IBrandService
    {
        private readonly IBaseRepository<Address> _addressRepository;
        private readonly IBaseRepository<Brand> _brandRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;

        public BrandService(IBaseRepository<Address> addressRepository,
                                IBaseRepository<Brand> brandRepository,
                                IFileUploadService fileUploadService, 
                                IMapper mapper)
        {
            _addressRepository = addressRepository;
            _brandRepository = brandRepository;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateBrandAsync(CreateBrandDTO createBrandDTO, ThisUserObj thisUserObj)
        {
            var brand = _mapper.Map<Brand>(createBrandDTO);
            brand.CreateBy = thisUserObj.userId;

            var duplicateAddresses = createBrandDTO.addresses?.GroupBy(a => new { a.lon, a.lat })
                                                        .Where(g => g.Count() > 1)
                                                        .Select(g => g.Key)
                                                        .ToList();

            if (duplicateAddresses != null && duplicateAddresses.Any())
            {
                var duplicateCoordinates = string.Join(", ",
                        duplicateAddresses.Select(d => $"({d.lon}, {d.lat})"));

                throw new ConflictException($"Tọa độ trùng là {duplicateCoordinates}");
            }

            foreach (var address in createBrandDTO.addresses)
            {
                var existedAddress = await _addressRepository.GetFirstOrDefaultAsync(x => x.Lon == address.lon && x.Lat == address.lat, isTracking: true);
                if (existedAddress != null)
                {
                    brand.Addresses.Add(existedAddress);
                }
                else
                {
                    var newAddess = _mapper.Map<Address>(address);
                    brand.Addresses.Add(newAddess);
                }
            }

            var brandId = await _brandRepository.AddAsync(brand);

            return new ResponseMessage<Guid>()
            {
                message = "Tạo thương hiệu thành công",
                result = true,
                value = (Guid)brandId
            };
        }

        public async Task<GetDetalBrandDTO> GetBrandByIdAsync(Guid id)
        {
            var brand = await _brandRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.Addresses));
            if (brand != null)
            {
                var brandDTO = _mapper.Map<GetDetalBrandDTO>(brand);
                return brandDTO;
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy thương hiệu với id {id}");
            }
        }

        public async Task<DynamicResponseModel<GetBrandDTO>> GetBrandsAsync(PagingRequest pagingRequest, BrandFilter brandFilter)
        {
            (int, IQueryable<GetBrandDTO>) result;

            result = _brandRepository.GetTable()
                        .ProjectTo<GetBrandDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetBrandDTO>(brandFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetBrandDTO>()
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

        public async Task<IList<GetBrandDTO>> GetBrandsbynameAsync(string name)
        {
            IQueryable<GetBrandDTO> result;
            result = _brandRepository.GetTable()
                        .Where(b => b.Name.ToLower().Contains(name.ToLower()))
                        .ProjectTo<GetBrandDTO>(_mapper.ConfigurationProvider);
            return await result.ToListAsync();
        }

        public async Task<ResponseMessage<bool>> UpdateBrandAsync(Guid id, UpdateBrandDTO updateBrandDTO, ThisUserObj thisUserObj)
        {
            var existedBrand = await _brandRepository.GetByIdAsync(id, isTracking: true);

            if (existedBrand == null)
            {
                throw new NotFoundException("Không tìm thấy thương hiệu này");
            }

            existedBrand = _mapper.Map(updateBrandDTO, existedBrand);
            existedBrand.UpdateBy = thisUserObj.userId;

            await _brandRepository.UpdateAsync(existedBrand);

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật thương hiệu thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> UpdateBrandStateAsync(Guid id, bool isActive, ThisUserObj thisUserObj)
        {
            var existedBrand = await _brandRepository.GetByIdAsync(id, isTracking: true);

            if (existedBrand == null)
            {
                throw new NotFoundException("Không tìm thấy thương hiệu này");
            }

            existedBrand.UpdateDate = DateTime.Now;
            existedBrand.UpdateBy = thisUserObj.userId;
            existedBrand.IsActive = isActive;

            await _brandRepository.UpdateAsync(existedBrand);

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật thương hiệu thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> UpdateBrandStatusAsync(Guid id, ObjectStatusEnum status, ThisUserObj thisUserObj)
        {
            var existedBrand = await _brandRepository.GetByIdAsync(id, isTracking: true);

            if (existedBrand == null)
            {
                throw new NotFoundException("Không tìm thấy thương hiệu này");
            }

            existedBrand.UpdateDate = DateTime.Now;
            existedBrand.UpdateBy = thisUserObj.userId;
            existedBrand.Status = status.ToString();

            await _brandRepository.UpdateAsync(existedBrand);

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật thương hiệu thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> VerifyBrand(Guid id, ThisUserObj thisUserObj)
        {
            var existedBrand = await _brandRepository.GetByIdAsync(id, isTracking: true);

            if (existedBrand == null)
            {
                throw new NotFoundException("Không tìm thấy thương hiệu này");
            }

            existedBrand.VerifiedBy = thisUserObj.userId;
            existedBrand.VerifiedDate = DateTime.Now;
            existedBrand.IsVerified = true;

            await _brandRepository.UpdateAsync(existedBrand);

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật thương hiệu thành công",
                result = true,
                value = true
            };
        }
    }
}
