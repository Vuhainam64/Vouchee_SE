using AutoMapper;
using AutoMapper.QueryableExtensions;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Helpers;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Constants.Number;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Business.Services.Impls
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository,
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

                address.CreateBy = Guid.Parse(thisUserObj.userId);
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

        public async Task<GetAllAddressDTO> GetAddressByIdAsync(Guid id)
        {
            try
            {
                var address = await _addressRepository.GetByIdAsync(id);
                if (address != null)
                {
                    GetAllAddressDTO addressDTO = _mapper.Map<GetAllAddressDTO>(address);
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

        public async Task<IList<GetAllAddressDTO>> GetAddressesAsync()
        {
            IQueryable<GetAllAddressDTO> result;
            try
            {
                result = _addressRepository.GetTable()
                            .ProjectTo<GetAllAddressDTO>(_mapper.ConfigurationProvider);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải address");
            }
            return result.ToList();
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
