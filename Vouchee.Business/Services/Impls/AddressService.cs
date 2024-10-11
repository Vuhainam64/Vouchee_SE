using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

namespace Vouchee.Business.Services.Impls
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository, 
                                IFileUploadService fileUploadService, 
                                IMapper mapper)
        {
            _addressRepository = addressRepository;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateAddressAsync(Guid shopId, CreateAddressDTO createAddressDTO, ThisUserObj thisUserObj)
        {
            var address = _mapper.Map<Address>(createAddressDTO);
            address.CreateBy = Guid.Parse(thisUserObj.userId);
            address.ShopId = shopId;

            var addressId = await _addressRepository.AddAsync(address);

            if (addressId != null && createAddressDTO.image != null)
            {
                address.Image = await _fileUploadService.UploadImageToFirebase(createAddressDTO.image, thisUserObj.userId, StoragePathEnum.ADDRESS);

                await _addressRepository.UpdateAsync(address);
            }

            return addressId;
        }

        public async Task<bool> DeleteAddressAsync(Guid id)
        {
            var existedAddress = await _addressRepository.FindAsync(id);
        
            if (existedAddress == null)
            {
                throw new NotFoundException("Không tìm thấy địa chỉ");
            }

            return await _addressRepository.DeleteAsync(existedAddress);
        }

        public async Task<GetAddressDTO> GetAddressByIdAsync(Guid id)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            var addressDTO = _mapper.Map<GetAddressDTO>(address);

            return addressDTO;
        }

        public async Task<IList<GetAddressDTO>> GetAddresssAsync()
        {
            IQueryable<GetAddressDTO> result;
            result = _addressRepository.GetTable()
                        .ProjectTo<GetAddressDTO>(_mapper.ConfigurationProvider);

            return result.ToList();
        }

        public async Task<bool> UpdateAddressAsync(Guid id, UpdateAddressDTO updateAddressDTO, ThisUserObj thisUserObj)
        {
            var existedAddress = await _addressRepository.FindAsync(id);

            return false;
        }
    }
}
