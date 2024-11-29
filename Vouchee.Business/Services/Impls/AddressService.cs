using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class AddressService : IAddressService
    {
        private readonly IBaseRepository<Brand> _brandRepository;
        private readonly IBaseRepository<Address> _addressRepository;
        private readonly IMapper _mapper;

        public AddressService(IBaseRepository<Brand> brandRepository,
                                IBaseRepository<Address> addressRepository,
                                IMapper mapper)
        {
            _brandRepository = brandRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateAddressAsync(Guid brandId,  CreateAddressDTO createAddressDTO, ThisUserObj thisUserObj)
        {
            Guid? addressId = Guid.Empty;

            var existedAddress = await _addressRepository.GetFirstOrDefaultAsync(x => x.Lon == createAddressDTO.lon && x.Lat == createAddressDTO.lat, isTracking: true);

            var existedBrand = await _brandRepository.GetByIdAsync(brandId, isTracking: true);

            if (existedBrand == null)
            {
                throw new NotFoundException("Không tìm thấy brand");
            }

            if (existedAddress != null)
            {
                existedBrand.Addresses.Add(existedAddress);
                addressId = existedAddress.Id;
            }
            else
            {
                Address newAddress = _mapper.Map<Address>(createAddressDTO);
                newAddress.CreateBy = thisUserObj.userId;
                newAddress.IsVerified = false;
                newAddress.Brands.Add(existedBrand);
            }

            return new ResponseMessage<Guid>()
            {
                message = "Tạo địa chỉ thành công",
                result =  true,
                value = (Guid) addressId
            };
        }

        public async Task<ResponseMessage<bool>> DeleteAddressAsync(Guid id)
        {
            var existedAddress = await _addressRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.Brands), isTracking: true);

            if (existedAddress == null)
            {
                throw new NotFoundException("Không tìm thấy địa chỉ này");
            }

            if (existedAddress.Brands.Count != 0)
            {
                throw new ConflictException("Có các brand phụ thuộc vào địa chỉ này");
            }

            if (await _addressRepository.DeleteAsync(existedAddress))
            {
                return new ResponseMessage<bool>()
                {
                    message = "Xóa địa chỉ thầnh công",
                    result = true,
                    value = true
                };
            }

            return null;
        }

        public async Task<GetDetailAddressDTO> GetAddressByIdAsync(Guid id)
        {
            var address = await _addressRepository.GetByIdAsync(id, includeProperties: x => x.Include(x => x.Brands));
            if (address != null)
            {
                GetDetailAddressDTO addressDTO = _mapper.Map<GetDetailAddressDTO>(address);
                return addressDTO;
            }
            else
            {
                throw new NotFoundException($"Không tìm thấy address với id {id}");
            }
        }

        public async Task<DynamicResponseModel<GetAddressDTO>> GetAddressesAsync(PagingRequest pagingRequest, AddressFilter addressFilter)
        {
            (int, IQueryable<GetAddressDTO>) result;
            result = _addressRepository.GetTable()
                        .ProjectTo<GetAddressDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetAddressDTO>(addressFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetAddressDTO>()
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

        public async Task<ResponseMessage<bool>> RemoveAddressFromBrandAsync(Guid addressId, Guid brandId, ThisUserObj thisUserObj)
        {
            var existedBrand = await _brandRepository.GetByIdAsync(brandId, includeProperties: x => x.Include(x => x.Addresses), isTracking: true);

            if (existedBrand == null)
            {
                throw new NotFoundException("Không tìm thấy brand này");
            }

            existedBrand.UpdateDate = DateTime.Now;
            existedBrand.UpdateBy = thisUserObj.userId;

            var existedAddress = existedBrand.Addresses.FirstOrDefault(x => x.Id == addressId);
            if (existedAddress == null)
            {
                throw new NotFoundException("Không tìm thấy địa chỉ trong brand");
            }
            else
            {
                existedBrand.Addresses.Remove(existedAddress);
                await _brandRepository.SaveChanges();

                return new ResponseMessage<bool>()
                {
                    message = "Xóa địa chỉ khỏi brand thành công",
                    result = true,
                    value = true
                };
            }
        }

        public async Task<bool> UpdateAddressAsync(Guid id, UpdateAddressDTO updateAddressDTO)
        {
            var existedAddress = await _addressRepository.GetByIdAsync(id);
            if (existedAddress != null)
            {
                var entity = _mapper.Map<Address>(updateAddressDTO);
                return await _addressRepository.UpdateAsync(entity);
            }
            else
            {
                throw new NotFoundException("Không tìm thấy addess");
            }
        }

        public async Task<ResponseMessage<bool>> UpdateAddressAsync(Guid id, UpdateAddressDTO updateAddressDTO, ThisUserObj thisUserObj)
        {
            var existedAddress = await _addressRepository.GetByIdAsync(id, isTracking: true);
            if (existedAddress == null)
            {
                throw new NotFoundException("Không tìm thấy địa chỉ");
            }

            existedAddress = _mapper.Map(updateAddressDTO, existedAddress);
            await _addressRepository.UpdateAsync(existedAddress);

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật địa chỉ thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> UpdateAddressStateAsync(Guid id, bool isActive, ThisUserObj thisUserObj)
        {
            var existedAddress = await _addressRepository.GetByIdAsync(id, isTracking: true);
            if (existedAddress == null)
            {
                throw new NotFoundException("Không tìm thấy địa chỉ");
            }

            existedAddress.IsActive = isActive;
            existedAddress.UpdateDate = DateTime.Now;
            existedAddress.UpdateBy = thisUserObj.userId;

            await _addressRepository.UpdateAsync(existedAddress);

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật địa chỉ thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> UpdateAddressStatusAsync(Guid id, ObjectStatusEnum statusEnum, ThisUserObj thisUserObj)
        {
            var existedAddress = await _addressRepository.GetByIdAsync(id, isTracking: true);
            if (existedAddress == null)
            {
                throw new NotFoundException("Không tìm thấy địa chỉ");
            }

            existedAddress.Status = statusEnum.ToString();
            existedAddress.UpdateDate = DateTime.Now;
            existedAddress.UpdateBy = thisUserObj.userId;

            await _addressRepository.UpdateAsync(existedAddress);

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật địa chỉ thành công",
                result = true,
                value = true
            };
        }

        public async Task<ResponseMessage<bool>> VerifyAddressAsync(Guid id, bool isVerify, ThisUserObj thisUserObj)
        {
            var existedAddress = await _addressRepository.GetByIdAsync(id, isTracking: true);
            if (existedAddress == null)
            {
                throw new NotFoundException("Không tìm thấy địa chỉ");
            }

            existedAddress.VerifiedBy = thisUserObj.userId;
            existedAddress.VerifiedDate = DateTime.Now;
            existedAddress.IsVerified = isVerify;
            existedAddress.UpdateDate = DateTime.Now;
            existedAddress.UpdateBy = thisUserObj.userId;

            await _addressRepository.UpdateAsync(existedAddress);

            return new ResponseMessage<bool>()
            {
                message = "Cập nhật địa chỉ thành công",
                result = true,
                value = true
            };
        }
    }
}
