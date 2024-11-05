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

        public async Task<Guid?> CreateAddressAsync(CreateAddressDTO createAddressDTO, ThisUserObj thisUserObj)
        {
            try
            {
                Address address = _mapper.Map<Address>(createAddressDTO);

                address.CreateBy = thisUserObj.userId;
                address.Status = ObjectStatusEnum.ACTIVE.ToString();

                var addressId = await _addressRepository.AddAsync(address);
                return addressId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException("Lỗi không xác định khi tạo address");
            }
        }

        public async Task<bool> DeleteAddressAsync(Guid id)
        {
            try
            {
                bool result = false;
                var address = await _addressRepository.GetByIdAsync(id);
                if (address != null)
                {
                    result = await _addressRepository.DeleteAsync(address);
                }
                else
                {
                    throw new NotFoundException($"Không tìm thấy address với id {id}");
                }
                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new DeleteObjectException("Lỗi không xác định khi xóa address");
            }
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
                throw new LoadException("Lỗi không xác định khi tải address");
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
            try
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
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new UpdateObjectException("Lỗi không xác định khi cập nhật address");
            }
        }
    }
}
