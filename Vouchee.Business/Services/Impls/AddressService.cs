using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IBaseRepository<Address> _addressRepository;
        private readonly IMapper _mapper;

        public AddressService(IBaseRepository<Address> addressRepository,
                            IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<ResponseMessage<Guid>> CreateAddressAsync(CreateAddressDTO createAddressDTO, ThisUserObj thisUserObj)
        {
            try
            {
                Address address = _mapper.Map<Address>(createAddressDTO);

                address.CreateBy = thisUserObj.userId;

                var addressId = await _addressRepository.AddAsync(address);
                return new ResponseMessage<Guid>()
                {
                    message = "Tạo địa chỉ thành công",
                    result =  true,
                    value = (Guid) addressId
                };
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException(ex.Message);
            }
        }

        public async Task<ResponseMessage<bool>> DeleteAddressAsync(Guid id)
        {
            var existedAddress = await _addressRepository.GetByIdAsync(id, isTracking: true);

            if (existedAddress == null)
            {
                throw new NotFoundException("Không tìm thấy địa chỉ này");
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
            try
            {
                var address = await _addressRepository.GetByIdAsync(id);
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
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException(ex.Message);
            }
        }

        public async Task<DynamicResponseModel<GetDetailAddressDTO>> GetAddressesAsync(PagingRequest pagingRequest, AddressFilter addressFilter)
        {
            (int, IQueryable<GetDetailAddressDTO>) result;
            result = _addressRepository.GetTable()
                        .ProjectTo<GetDetailAddressDTO>(_mapper.ConfigurationProvider)
                        .DynamicFilter(_mapper.Map<GetDetailAddressDTO>(addressFilter))
                        .PagingIQueryable(pagingRequest.page, pagingRequest.pageSize, PageConstant.LIMIT_PAGING, PageConstant.DEFAULT_PAPING);

            return new DynamicResponseModel<GetDetailAddressDTO>()
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

            existedAddress.VerifiedDate = DateTime.Now;
            existedAddress.IsVerfied = isVerify;
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
